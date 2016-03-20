// -----------------------------------------------------------------------
// <copyright file="XboxDevice.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Xml;
    using Microsoft.Test.Xbox.Profiles;
    using Microsoft.Win32;
    using XDevkit;

    /// <summary>
    /// XboxDevice is associated with a specific Xbox device in
    /// the user's network neighborhood.
    /// </summary>
    public class XboxDevice : IXboxDevice, INotifyPropertyChanged
    {
        /// <summary>
        /// A static XboxManager manager
        /// </summary>
        private static XboxManager xboxManager = new XboxManager();

        /// <summary>
        /// GUID used to uniquely identify this application instance's debug connection
        /// </summary>
        private static Guid debugGuid = Guid.NewGuid();

        /// <summary>
        /// A set of all disc emulation Process Ids that CAT knows about
        /// </summary>
        private static HashSet<int> discEmulationProcessIds = new HashSet<int>();

        /// <summary>
        /// Backing field for IsSelected property
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Delegate callback for changes to selected state.
        /// </summary>
        private OnSelectedChangedDelegate onSelectedChangedDelegate;

        /// <summary>
        /// Backing field for Connected property
        /// </summary>
        private bool connected;

        /// <summary>
        /// Backing field for Connecting property
        /// </summary>
        private bool connecting;

        /// <summary>
        /// The delegate to call when a connection attempt completes (or fails).
        /// </summary>
        private ConnectCompleteDelegate connectCompleteDelegate;

        /// <summary>
        /// XboxConsole object used internally.
        /// This is recycled on reboot, to avoid event dropout issues.
        /// </summary>
        private XboxConsole internalXboxConsole;

        /// <summary>
        /// Backing field for XboxConsole property
        /// </summary>
        private XboxConsole xboxConsole;

        /// <summary>
        /// IXboxDebugTarget for this device, or null if the debugger is not currently connected
        /// </summary>
        private IXboxDebugTarget debugTarget;

        /// <summary>
        /// Instance of handler for Xbox events
        /// </summary>
        private XboxEvents_OnStdNotifyEventHandler onStdNotify;

        /// <summary>
        /// Dictionary mapping of symbols to the delegates to call when the functions are called.
        /// </summary>
        private Dictionary<string, MonitoredAPISymbol> monitorSymbols = new Dictionary<string, MonitoredAPISymbol>();

        /// <summary>
        /// A lock used to synchronize access to symbol monitoring data
        /// </summary>
        private object monitorAPILock = new object();

        /// <summary>
        /// Dictionary mapping of module base address to a list of function addresses
        /// </summary>
        private Dictionary<uint, List<uint>> moduleBaseAddrToBreakpointMap = new Dictionary<uint, List<uint>>();

        /// <summary>
        /// Dictionary mapping of function addresses to monitored symbols
        /// </summary>
        private Dictionary<uint, MonitoredAPISymbol> breakpointToMonitoredAPISymbol = new Dictionary<uint, MonitoredAPISymbol>();

        /// <summary>
        /// Current delegate for intercepting debug output
        /// </summary>
        private MonitorDebugDelegate debugMonitorDelegate;

        /// <summary>
        /// Whether or not the debugger is currently being disconnected
        /// </summary>
        private bool disconnectingDebugger;

        /// <summary>
        /// A lock used to serialize access to disconnectingDebugger
        /// </summary>
        private object disconnectingDebuggerLock = new object();

        /// <summary>
        /// Backing field for XboxTitle property
        /// </summary>
        private IXboxTitle xboxTitle;

        /// <summary>
        /// Delegate callback for notification of conditions that might require collecting a dump (assert, exception, RIP).
        /// </summary>
        private OnTitleFailureDelegate onTitleFailureDelegate;

        /// <summary>
        /// ProcessId of the game disc emulation process
        /// </summary>
        private int discEmulationProcessId = -1;    // processId of -1 is invalid - not running

        /// <summary>
        /// synchronizes access to disconnectDeferCount
        /// </summary>
        private object disconnectLock = new object();

        /// <summary>
        /// Whether there is a deferred disconnect pending
        /// </summary>
        private bool isDisconnectPending;

        /// <summary>
        /// Counts disconnect deferrals
        /// </summary>
        private uint disconnectDeferCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxDevice" /> class.
        /// </summary>
        /// <param name="nameOrIpAddress">name or IP address of Xbox to associate with this XboxDevice object</param>
        /// <param name="xboxTitle">A reference to the currently configured Xbox title</param>
        public XboxDevice(string nameOrIpAddress, DataModel.XboxTitle xboxTitle)
        {
            this.xboxTitle = xboxTitle;
            this.onStdNotify = new XboxEvents_OnStdNotifyEventHandler(this.XboxConsole_OnStdNotify);
            this.ConnectTo = nameOrIpAddress;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxDevice" /> class.
        /// </summary>
        /// <param name="console">XboxConsole object to associate with this XboxDevice object</param>
        /// <param name="xboxTitle">A reference to the currently configured Xbox title</param>
        public XboxDevice(IXboxConsole console, DataModel.XboxTitle xboxTitle)
        {
            this.xboxTitle = xboxTitle;
            this.onStdNotify = new XboxEvents_OnStdNotifyEventHandler(this.XboxConsole_OnStdNotify);
            this.ConnectTo = string.Empty;
            try
            {
                long ip = (long)EndianSwap(console.IPAddress);
                IPAddress addr = new IPAddress(ip);
                this.ConnectTo = addr.ToString();
            }
            catch (COMException)
            {
            }

            try
            {
                string name = console.Name;
                this.ConnectTo = name;
            }
            catch (COMException)
            {
            }
        }

        /// <summary>
        /// Delegate for notification of a completed or failed connection attempt.
        /// May actually be called repeatedly - once each time the device becomes available after a reboot
        /// </summary>
        /// <param name="xbd">The Xbox to which the connection was attempted</param>
        /// <param name="connected">Whether or not the connection was successful</param>
        public delegate void ConnectCompleteDelegate(XboxDevice xbd, bool connected);

        /// <summary>
        /// Delegate type for monitoring debug output
        /// </summary>
        /// <param name="logText">Text to log</param>
        public delegate void MonitorDebugDelegate(string logText);

        /// <summary>
        /// Delegate used to delivery notification of title failures such as asserts, exceptions, and RIP
        /// </summary>
        /// <param name="description">Description of the failure, i.e. Assert, Exception or RIP</param>
        /// <param name="eventInfo">The event info associated with the failure.  Pass to ContinueExecution() to resume.</param>
        public delegate void OnTitleFailureDelegate(string description, IXboxEventInfo eventInfo);

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// This enumeration encompasses the assigned controller port numbers for the XBox Controllers.
        /// </summary>
        public enum ControllerPort
        {
            /// <summary>
            /// Controller port 1
            /// </summary>
            Number1 = 0,

            /// <summary>
            /// Controller port 2
            /// </summary>
            Number2 = 1,

            /// <summary>
            /// Controller port 3
            /// </summary>
            Number3 = 2,

            /// <summary>
            /// Controller port 4
            /// </summary>
            Number4 = 3
        }

        /// <summary>
        /// Gets the base directory the Xbox 360 Development Kit is installed on this computer
        /// </summary>
        /// <returns>Path to the Xbox 360 development kit software</returns>
        public static string XdkPath
        {
            get
            {
                string xdkPath = Environment.GetEnvironmentVariable("XEDK");
                if (string.IsNullOrEmpty(xdkPath))
                {
                    throw new Exception("XEDK is missing from environment variables");
                }

                DirectoryInfo info = new DirectoryInfo(xdkPath);
                if (!info.Exists)
                {
                    throw new Exception("XDK not found");
                }

                return xdkPath;
            }
        }

        /// <summary>
        /// Gets the path for win32 XDK tools
        /// </summary>
        public static string XdkToolPath
        {
            get
            {
                string toolPath = XdkPath + "\\bin\\win32";
                DirectoryInfo info = new DirectoryInfo(toolPath);
                if (!info.Exists)
                {
                    throw new Exception("XDK cmd tools are missing from disk");
                }

                return toolPath;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the XDK is present on this computer
        /// </summary>
        public static bool IsXdkInstalled
        {
            get
            {
                string xdkPath = Environment.GetEnvironmentVariable("XEDK");
                return !string.IsNullOrEmpty(xdkPath);
            }
        }

        /// <summary>
        /// Gets the XDK version installed on this computer
        /// </summary>
        public static string XdkVersion
        {
            get
            {
                string value = string.Empty;
                value = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Xbox\2.0\SDK", "InstalledVersion", value);
                if (!string.IsNullOrEmpty(value))
                {
                    value = value.Replace(',', '.');
                }

                return value;
            }
        }

        /// <summary>
        /// Gets or sets the string used to connect to the Xbox (IP, or Name)
        /// </summary>
        public string ConnectTo { get; set; }

        /// <summary>
        /// Gets or sets a reference to the XboxViewItem associated with this Xbox
        /// </summary>
        public XboxViewItem XboxViewItem { get; set; }

        /// <summary>
        /// Gets the name of this Xbox
        /// Implements Name from IDevice
        /// </summary>
        public string Name
        {
            get
            {
                string name = this.ConnectTo;
                try
                {
                    if (this.Connected && this.Responding)
                    {
                        name = this.internalXboxConsole.Name;
                    }
                }
                catch (COMException)
                {
                    this.Responding = false;
                }

                return name;
            }
        }

        /// <summary>
        /// Gets the IP address of the Xbox
        /// Implements IP from IXboxDevice
        /// </summary>
        public string IP
        {
            get
            {
                string addressString = string.Empty;
                try
                {
                    if (this.Connected && this.Responding)
                    {
                        long ip = (long)EndianSwap(this.internalXboxConsole.IPAddress);
                        IPAddress addr = new IPAddress(ip);
                        addressString = addr.ToString();
                    }
                }
                catch (COMException)
                {
                    this.Responding = false;
                }

                return addressString;
            }
        }

        /// <summary>
        /// Gets the IP address of the Xbox
        /// </summary>
        public IPAddress IPAddress
        {
            get
            {
                IPAddress result = null;
                try
                {
                    if (this.Connected && this.Responding)
                    {
                        long ip = (long)EndianSwap(this.internalXboxConsole.IPAddress);
                        result = new IPAddress(ip);
                    }
                }
                catch (COMException)
                {
                    this.Responding = false;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this device is selected in the device pool
        /// Implements IsSelected from IDevice
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }

            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    if (this.onSelectedChangedDelegate != null)
                    {
                        this.onSelectedChangedDelegate(this, value);
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this Xbox is capable of debugging
        /// Implements CanDebug from IXboxDevice
        /// </summary>
        public bool CanDebug
        {
            get
            {
                bool result = false;
                try
                {
                    if (this.Connected && this.Responding)
                    {
                        result = this.internalXboxConsole.ConsoleFeatures.HasFlag(XboxConsoleFeatures.Debugging);
                    }
                }
                catch (COMException)
                {
                    this.Responding = false;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the type of Xbox
        /// Implements Type from IXboxDevice
        /// </summary>
        public XboxKitType Type
        {
            get
            {
                XboxKitType result = XboxKitType.Unknown;
                if (this.Connected && this.Responding)
                {
                    try
                    {
                        switch (XboxConsole.ConsoleType)
                        {
                            case XboxConsoleType.TestKit:
                                result = XboxKitType.TestKit;
                                break;
                            case XboxConsoleType.DevelopmentKit:
                                result = XboxKitType.DevelopmentKit;
                                break;
                            case XboxConsoleType.ReviewerKit:
                                result = XboxKitType.ReviewerKit;
                                break;
                        }
                    }
                    catch (COMException)
                    {
                        this.Responding = false;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Xbox is currently configured to 1 Gig of RAM
        /// Implements Has1GB from IXboxDevice
        /// </summary>
        public bool Has1GB
        {
            get
            {
                bool result = false;
                if (this.Connected && this.Responding)
                {
                    // Point native IP calls to this Xbox
                    XboxDebugManagerNative.DmSetXboxNameNoRegister(this.IP);

                    XboxDebugManagerNative.ConsoleMemConfig config;
                    XboxDebugManagerNative.DmGetConsoleDebugMemoryStatus(out config);
                    if (config == XboxDebugManagerNative.ConsoleMemConfig.DM_CONSOLEMEMCONFIG_ADDITIONALMEMENABLED)
                    {
                        result = true;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Xbox is capable of 1GB RAM (not necessarily in 1 Gig RAM mode right now).
        /// Implements Is1GBCapable in IXboxDevice
        /// </summary>
        public bool Is1GBCapable
        {
            get
            {
                bool result = false;
                try
                {
                    if (this.Connected && this.Responding)
                    {
                        result = this.internalXboxConsole.ConsoleFeatures.HasFlag(XboxConsoleFeatures.GB_RAM);
                    }
                }
                catch (COMException)
                {
                    this.Responding = false;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Xbox is currently connected and addressable 
        /// Implements Connected from IXboxDevice
        /// </summary>
        public bool Connected
        {
            get { return this.connected; } 
        }

        /// <summary>
        /// Gets a value indicating whether a connection is pending to this Xbox.
        /// Note: Do not call Connect if the Xbox is already in the process of connecting.
        /// </summary>
        public bool Connecting 
        {
            get { return this.connecting; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not this is the default Xbox as set in the network neighborhood
        /// </summary>
        public bool IsDefault
        {
            get
            {
                bool result = false;
                try
                {
                    string defaultConsole = xboxManager.DefaultConsole;
                    if (this.Connected)
                    {
                        result = defaultConsole == this.Name;
                    }

                    if (!result)
                    {
                        result = (defaultConsole == this.ConnectTo) || (defaultConsole == this.IP);
                    }
                }
                catch (COMException)
                {
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this is a Slim Xbox model.
        /// Guesses based on existence of INTUSB drive.
        /// Implements IsSlim in IXboxDevice
        /// </summary>
        public bool IsSlim
        {
            get
            {
                bool result = false;
                try
                {
                    if (this.Connected && this.Responding)
                    {
                        result = this.internalXboxConsole.Drives.Contains("INTUSB");
                    }
                }
                catch (COMException)
                {
                    this.Responding = false;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the XboxConsole instance for this XboxDevice
        /// </summary>
        public XboxConsole XboxConsole
        {
            get { return this.xboxConsole; }
        }

        /// <summary>
        /// Gets the IXboxAutomation associated with this XboxConsole
        /// </summary>
        public IXboxAutomation IXboxAutomation
        {
            get
            {
                IXboxAutomation result = null;
                try
                {
                    if (this.internalXboxConsole != null)
                    {
                        result = this.internalXboxConsole.XboxAutomation;
                    }
                }
                catch (COMException)
                {
                    this.Responding = false;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets information about a Game Title.  This game title may be installed/uninstalled/launched on this Xbox
        /// </summary>
        public IXboxTitle XboxTitle
        {
            get { return this.xboxTitle; }
        }

        /// <summary>
        /// Gets the TitleId of the currently configured Xbox title.
        /// The TitleId is read from the game config file
        /// </summary>
        public string TitleId
        {
            get
            {
                string result = string.Empty;
                if (!string.IsNullOrEmpty(this.xboxTitle.GameConfigPath))
                {
                    // Read XML config file
                    XmlDocument configFile = new XmlDocument();
                    configFile.Load(this.xboxTitle.GameConfigPath);
                    if (configFile.DocumentElement.Name == "XboxLiveSubmissionProject")
                    {
                        XmlNode xboxLiveSubmissionProjectNode = configFile.DocumentElement;
                        if (xboxLiveSubmissionProjectNode.ChildNodes.Count > 0)
                        {
                            foreach (XmlNode n in xboxLiveSubmissionProjectNode.ChildNodes)
                            {
                                if (n.Name == "GameConfigProject")
                                {
                                    try
                                    {
                                        result = n.Attributes["titleId"].InnerText.Trim();
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    if (result.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                                    {
                                        result = result.Substring(2);
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the appropriate path for a content package, based on its TitleId
        /// </summary>
        public string ContentPackageBasedTitleFilePath
        {
            get
            {
                string result = string.Empty;
                string titleId = this.TitleId;
                if (!string.IsNullOrEmpty(titleId))
                {
                    string destDirectory1 = string.Format(@"Content\0000000000000000\");
                    string destDirectory2 = Path.Combine(destDirectory1, titleId);
                    result = Path.Combine(destDirectory2, "000D0000");
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the currently configured filename of a content package based title
        /// </summary>
        public string ContentPackageBasedTitleFileName
        {
            get
            {
                string result = string.Empty;
                if (!string.IsNullOrEmpty(this.xboxTitle.ContentPackage))
                {
                    result = Path.GetFileName(this.xboxTitle.ContentPackage);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the currently configured filename of a content package based demo
        /// </summary>
        public string DemoContentPackageBasedTitleFileName
        {
            get
            {
                string result = string.Empty;
                if (!string.IsNullOrEmpty(this.xboxTitle.DemoContentPackage))
                {
                    result = Path.GetFileName(this.xboxTitle.DemoContentPackage);
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether disc image emulation is currently running.
        /// </summary>
        public bool IsDiscEmulationRunning
        {
            get
            {
                bool result = false;
                if (this.discEmulationProcessId != -1)
                {
                    try
                    {
                        Process process = Process.GetProcessById(this.discEmulationProcessId);
                        if (process != null)
                        {
                            result = true;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a RAW title has been installed
        /// </summary>
        public bool IsRawTitleInstalled
        {
            get
            {
                bool result = false;
                if (this.Connected && this.Responding)
                {
                    string titleFilePath = @"DEVKIT:\CATRawTitle";
                    try
                    {
                        IXboxFiles files = this.internalXboxConsole.DirectoryFiles(titleFilePath);
                        result = files.Count != 0;
                    }
                    catch (Exception)
                    {
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the title is installed on the HDD
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledOnHDD
        {
            get { return this.IsContentPackageBasedTitleInstalledOn("HDD"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the title is installed on MU0
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledOnMU0
        {
            get { return this.IsContentPackageBasedTitleInstalledOn("MU0"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the title is installed on MU1
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledOnMU1
        {
            get { return this.IsContentPackageBasedTitleInstalledOn("MU1"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the title is installed on the Internal USB storage device
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledOnINTUSB
        {
            get { return this.IsContentPackageBasedTitleInstalledOn("INTUSB"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the title is installed on the Internal MU
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledOnMUINT
        {
            get { return this.IsContentPackageBasedTitleInstalledOn("MUINT"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the title is installed on USB0
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledOnUSB0
        {
            get { return this.IsContentPackageBasedTitleInstalledOn("USBMASS0MU"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the title is installed on USB1
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledOnUSB1
        {
            get { return this.IsContentPackageBasedTitleInstalledOn("USBMASS1MU"); }
        }

        /// <summary>
        /// Gets a value indicating whether the title installed (on any currently active storage device)
        /// </summary>
        public bool IsContentPackageBasedTitleInstalled
        {
            get
            {
                return this.IsContentPackageBasedTitleInstalledOnHDD
                    || this.IsContentPackageBasedTitleInstalledOnMU0
                    || this.IsContentPackageBasedTitleInstalledOnMU1
                    || this.IsContentPackageBasedTitleInstalledOnINTUSB
                    || this.IsContentPackageBasedTitleInstalledOnMUINT
                    || this.IsContentPackageBasedTitleInstalledOnUSB0
                    || this.IsContentPackageBasedTitleInstalledOnUSB1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the currently configured title is installed.
        /// </summary>
        public bool IsTitleInstalled
        {
            get
            {
                bool result = false;
                if (this.xboxTitle.GameInstallType == "Content Package")
                {
                    result = this.IsContentPackageBasedTitleInstalled;
                }
                else if (this.xboxTitle.GameInstallType == "Disc Emulation")
                {
                    result = this.IsDiscEmulationRunning;
                }
                else if (this.xboxTitle.GameInstallType == "Raw")
                {
                    result = this.IsRawTitleInstalled;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be installed.
        /// </summary>
        public bool CanInstallContentPackageBasedTitle
        {
            get
            {
                return this.Connected &&
                    this.Responding &&
                    this.IsAnyStorageDeviceEnabled &&
                    (this.xboxTitle.GameInstallType == "Content Package") &&
                    (!string.IsNullOrEmpty(this.xboxTitle.ContentPackage));
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not a disc image title can be installed
        /// </summary>
        public bool CanInstallDiscImageTitle
        {
            get
            {
                return (this.xboxTitle.GameInstallType == "Disc Emulation") &&
                    (!string.IsNullOrEmpty(this.xboxTitle.DiscImage)) &&
                    this.Connected &&
                    this.Responding &&
                    this.IsAnyStorageDeviceEnabled &&
                    !this.IsDiscEmulationRunning;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not a raw title can be installed
        /// </summary>
        public bool CanInstallRawTitle
        {
            get
            {
                return (this.xboxTitle.GameInstallType == "Raw") &&
                    (!string.IsNullOrEmpty(this.xboxTitle.RawGameDirectory)) &&
                    this.Connected &&
                    this.Responding &&
                    this.IsDevKitDriveEnabled &&
                    !this.IsRawTitleInstalled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the currently configured game title can be installed
        /// </summary>
        public bool CanInstallTitle
        {
            get
            {
                bool result = false;
                if (this.xboxTitle.GameInstallType == "Content Package")
                {
                    result = this.CanInstallContentPackageBasedTitle;
                }
                else if (this.xboxTitle.GameInstallType == "Disc Emulation")
                {
                    result = this.CanInstallDiscImageTitle;
                }
                else if (this.xboxTitle.GameInstallType == "Raw")
                {
                    result = this.CanInstallRawTitle;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a version string for the XDK installed on the Xbox
        /// </summary>
        public string XDKVersion
        {
            get
            {
                // Point native IP calls to this Xbox
                XboxDebugManagerNative.DmSetXboxNameNoRegister(this.IP);

                string xdkVersion = string.Empty;
                XboxDebugManagerNative.DM_SYSTEM_INFO systemInfo = new XboxDebugManagerNative.DM_SYSTEM_INFO();
                systemInfo.SizeOfStruct = Marshal.SizeOf(systemInfo);
                XboxDebugManagerNative.HResult hr = XboxDebugManagerNative.DmGetSystemInfo(ref systemInfo);
                if (hr == XboxDebugManagerNative.HResult.XBDM_NOERR)
                {
                    xdkVersion = systemInfo.XDKVersion.Major.ToString();
                    xdkVersion += "." + systemInfo.XDKVersion.Minor.ToString();
                    xdkVersion += "." + systemInfo.XDKVersion.Build.ToString();
                    xdkVersion += "." + systemInfo.XDKVersion.Qfe.ToString();
                }

                return xdkVersion;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Xbox is currently responding to API calls
        /// </summary>
        public bool Responding { get; set; }

        /// <summary>
        /// Gets a list of drives on this Xbox
        /// </summary>
        public List<string> Drives
        {
            get
            {
                List<string> drives = new List<string>();
                if (this.Connected && this.Responding)
                {
                    string drivesString = string.Empty;
                    try
                    {
                        drivesString = this.internalXboxConsole.Drives;
                        if (!string.IsNullOrEmpty(drivesString))
                        {
                            drives = drivesString.Split(',').ToList<string>();
                        }
                    }
                    catch (COMException)
                    {
                    }
                }

                return drives;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the DEVKIT drive is currently enabled
        /// </summary>
        public bool IsDevKitDriveEnabled 
        {
            get { return this.IsDriveEnabled("DEVKIT"); } 
        }

        /// <summary>
        /// Gets a value indicating whether or not the HDD is currently enabled
        /// </summary>
        public bool IsHDDEnabled 
        {
            get { return this.IsDriveEnabled("HDD"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not MU0 is currently enabled
        /// </summary>
        public bool IsMU0Enabled 
        {
            get { return this.IsDriveEnabled("MU0"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not MU1 is currently enabled
        /// </summary>
        public bool IsMU1Enabled 
        {
            get { return this.IsDriveEnabled("MU1"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not the Internal USB storage device is currently enabled
        /// </summary>
        public bool IsINTUSBEnabled
        {
            get { return this.IsDriveEnabled("INTUSB"); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Internal MU is currently enabled
        /// </summary>
        public bool IsMUINTEnabled
        {
            get 
            {
                return this.IsDriveEnabled("MUINT"); 
            } 

            set
            {
                if (value != this.IsDriveEnabled("MUINT"))
                {
                    string script;
                    if (value)
                    {
                        script = @"Scripts\Enable_Internal_MU.xboxautomation";
                    }
                    else
                    {
                        script = @"Scripts\Disable_Internal_MU.xboxautomation";
                    }

                    this.RunIXBoxAutomationScript(script);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not USB0 is currently enabled
        /// </summary>
        public bool IsUSB0Enabled 
        {
            get { return this.IsDriveEnabled("USBMASS0MU"); } 
        }

        /// <summary>
        /// Gets a value indicating whether or not USB1 is currently enabled
        /// </summary>
        public bool IsUSB1Enabled 
        {
            get { return this.IsDriveEnabled("USBMASS1MU"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not any storage device is currently enabled
        /// </summary>
        public bool IsAnyStorageDeviceEnabled
        {
            get
            {
                return this.IsHDDEnabled ||
                    this.IsMU0Enabled ||
                    this.IsMU1Enabled ||
                    this.IsINTUSBEnabled ||
                    this.IsMUINTEnabled ||
                    this.IsUSB0Enabled ||
                    this.IsUSB1Enabled;
            }
        }

        /// <summary>
        /// Gets the primary drive of the Xbox
        /// </summary>
        public string PrimaryDrive
        {
            get
            {
                string drives;
                try
                {
                    drives = this.XboxConsole.Drives;
                }
                catch
                {
                    return string.Empty;
                }

                if (drives.Contains("HDD"))
                {
                    return "HDD";
                }

                if (drives.Contains("MUINT"))
                {
                    return "MUINT";
                }

                if (drives.Contains("INTUSB"))
                {
                    return "INTUSB";
                }

                if (drives.Contains("MU0"))
                {
                    return "MU0";
                }

                if (drives.Contains("MU1"))
                {
                    return "MU1";
                }

                if (drives.Contains("USBMASS0MU"))
                {
                    return "USBMASS0MU";
                }

                if (drives.Contains("USBMASS1MU"))
                {
                    return "USBMASS1MU";
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets any drive the content package based title is installed on
        /// </summary>
        public string GetAnyDriveContentPackageBasedTitleIsInstalledOn
        {
            get
            {
                string result = string.Empty;
                if (this.IsContentPackageBasedTitleInstalledOnHDD)
                {
                    result = "HDD";
                }
                else if (this.IsContentPackageBasedTitleInstalledOnMU0)
                {
                    result = "MU0";
                }
                else if (this.IsContentPackageBasedTitleInstalledOnMU1)
                {
                    result = "MU1";
                }
                else if (this.IsContentPackageBasedTitleInstalledOnINTUSB)
                {
                    result = "INTUSB";
                }
                else if (this.IsContentPackageBasedTitleInstalledOnMUINT)
                {
                    result = "MUINT";
                }
                else if (this.IsContentPackageBasedTitleInstalledOnUSB0)
                {
                    result = "USBMASS0MU";
                }
                else if (this.IsContentPackageBasedTitleInstalledOnUSB1)
                {
                    result = "USBMASS1MU";
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the filenames of all running processes on the Xbox
        /// </summary>
        private List<string> AllRunningProcesses
        {
            get
            {
                this.Responding = true; 
                bool disconnectDebugger = false;
                List<string> result = new List<string>();
                try
                {
                    if (this.debugTarget == null)
                    {
                        this.ConnectDebugger();
                        disconnectDebugger = true;
                    }

                    IXboxModules modules = this.internalXboxConsole.DebugTarget.Modules;
                    foreach (IXboxModule module in modules)
                    {
                        result.Add(module.ModuleInfo.Name);
                    }
                }
                catch (COMException)
                {
                    this.Responding = false;
                }
                finally
                {
                    if (disconnectDebugger)
                    {
                        try
                        {
                            this.DisconnectDebugger();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the filename of the active running title on the Xbox
        /// </summary>
        private string CurrentProcessName
        {
            get
            {
                this.Responding = true;
                string result = string.Empty;
                try
                {
                    result = this.internalXboxConsole.DebugTarget.RunningProcessInfo.ProgramName;
                }
                catch (COMException)
                {
                    this.Responding = false;
                }

                return result;
            }
        }

        /// <summary>
        /// Unsets the default Xbox
        /// </summary>
        public static void UnsetDefault()
        {
            try
            {
                xboxManager.DefaultConsole = null;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Gets a list of Xbox's from the users Xbox Neighborhood
        /// </summary>
        /// <param name="xboxTitle">The xbox title to associate</param>
        /// <returns>List of Xbox's</returns>
        public static List<XboxDevice> GetXboxList(DataModel.XboxTitle xboxTitle)
        {
            List<XboxDevice> list = new List<XboxDevice>();
            foreach (string s in xboxManager.Consoles)
            {
                XboxDevice xbd = new XboxDevice(s, xboxTitle);
                list.Add(xbd);
            }

            return list;
        }

        /// <summary>
        /// Append the win32 bin path to XDK path.
        /// </summary>
        public static void AppendXdkToolPathToExecutingEnvironment()
        {
            string path = Environment.GetEnvironmentVariable("PATH");
            string xdkPath = XdkToolPath;
            if (!path.StartsWith(xdkPath))
            {
                path = xdkPath + ";" + path;
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }

        /// <summary>
        /// Launches the AddConsole wizard
        /// </summary>
        public static void AddDevice()
        {
            xboxManager.RunAddConsoleWizardEx(0, true);
        }

        /// <summary>
        /// If we have successfully connected to an Xbox, it may become unresponsive.
        /// It may become responsive again, without need to reconnect.  Instead of disconnecting
        /// when it becomes unresponsive, NotResponding is set.  Calls to API routines will fail
        /// as if the connection is not present, but a new connection attempt will not be issued.
        /// </summary>
        /// <returns>A value indicating whether or not the Xbox is currently responding</returns>
        public bool VerifyOnline()
        {
            bool isResponding = false;
            lock (this)
            {
                if (this.Connected)
                {
                    isResponding = true;
                    try
                    {
                        this.internalXboxConsole.FindConsole(0, 0);
                    }
                    catch (COMException)
                    {
                        isResponding = false;
                    }
                }

                this.Responding = isResponding;
            }

            return isResponding;
        }

        /// <summary>
        /// Starts monitoring for changes to this device's selected state
        /// Implements StartMonitoringSelectionChanges in IDevice
        /// </summary>
        /// <param name="d">Delegate to call when the selection state changes</param>
        public void StartMonitoringSelectionChanges(OnSelectedChangedDelegate d)
        {
            this.onSelectedChangedDelegate = d;
        }

        /// <summary>
        /// Stops monitoring for changes to this device's selected state
        /// Implements StopMonitoringSelectionChanges in IDevice
        /// </summary>
        public void StopMonitoringSelectionChanges()
        {
            this.onSelectedChangedDelegate = null;
        }

        /// <summary>
        /// Sets this Xbox as the default Xbox 
        /// </summary>
        public void SetDefault()
        {
            try
            {
                xboxManager.DefaultConsole = this.IP;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Connects debugging capabilities to the Xbox.
        /// This will fail if the Xbox is Test kit, or not running.
        /// This is intended to be called from a module (or from another call made by a module).
        /// The debugger will be automatically disconnected when the module stops.
        /// Implements ConnectDebugger in IXboxDevice.
        /// </summary>
        /// <param name="force">Whether or not for force connection to the debugger if already connected to by someone else</param>
        /// <returns>true if successfully connected, false if not</returns>
        public bool ConnectDebugger(bool force = false)
        {
            bool result = true;
            lock (this.disconnectingDebuggerLock)
            {
                this.disconnectingDebugger = false;
                if (this.debugTarget == null)
                {
                    string debuggerName = "CAT-" + debugGuid.ToString();
                    try
                    {
                        // Attach as debugger.
                        this.debugTarget = this.internalXboxConsole.DebugTarget;

                        string debugAlreadyConnected;
                        string debugAlreadyConnectedUser;

                        if (force || (!this.debugTarget.IsDebuggerConnected(out debugAlreadyConnected, out debugAlreadyConnectedUser) || (debugAlreadyConnected == debuggerName)))
                        {
                            this.debugTarget.ConnectAsDebugger(debuggerName, XboxDebugConnectFlags.Force);
                        }
                        else
                        {
                            Marshal.ReleaseComObject(this.debugTarget);
                            this.debugTarget = null;
                            result = false;
                        }
                    }
                    catch (COMException)
                    {
                        // In case the user changes the title in the middle, debugTarget calls will throw exceptions
                        result = false;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Looks up a symbol in the specified module
        /// </summary>
        /// <param name="symbolName">Name of symbol to look up</param>
        /// <param name="baseAddress">Base address of module to look up symbol in</param>
        /// <returns>Address of the symbol, or 0 if not found</returns>
        public uint LookupSymbolInModule(string symbolName, uint baseAddress)
        {
            uint result = 0;
            try
            {
                // Point native IP calls to this Xbox
                XboxDebugManagerNative.DmSetXboxNameNoRegister(this.IP);
                XboxDebugManagerNative.DM_PDB_SIGNATURE pdbSignature;
                XboxDebugManagerNative.HResult nativeResult = XboxDebugManagerNative.DmFindPdbSignature(baseAddress, out pdbSignature);
                if (nativeResult == XboxDebugManagerNative.HResult.XBDM_NOERR)
                {
                    // Search for xdb files in symbol path
                    DirectoryInfo dirInfo;
                    try
                    {
                        dirInfo = new DirectoryInfo(this.xboxTitle.SymbolsPath);
                    }
                    catch (Exception)
                    {
                        return result;
                    }

                    FileInfo[] files = null;
                    try
                    {
                        files = dirInfo.GetFiles("*.xdb", SearchOption.AllDirectories);
                        foreach (FileInfo fileInfo in files)
                        {
                            result = Native.LookupSymbol(symbolName, fileInfo.FullName, baseAddress, ref pdbSignature);
                            if (result != 0)
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception) 
                    {
                        // failure to find xbds or the xdb directory, is not fatal
                    }
                }
            }
            catch (COMException)
            {
            }

            return result;
        }

        /// <summary>
        /// Configures symbol monitoring relationships.  Must be called when the MonitoredAPISymbol is not currently set up.
        /// </summary>
        /// <param name="monitoredAPISymbol">MonitoredAPISymbol object associated with the symbol to register</param>
        public void RefreshSymbol(MonitoredAPISymbol monitoredAPISymbol)
        {
            lock (this.monitorAPILock)
            {
                monitoredAPISymbol.SymbolLoadCount = 0;
                
                // Check to see if currently accessible.  If not, try again when title starts up.
                try
                {
                    IXboxModules modules = this.internalXboxConsole.DebugTarget.Modules;
                    foreach (IXboxModule module in modules)
                    {
                        uint funcAddr = this.LookupSymbolInModule(monitoredAPISymbol.SymbolName, module.ModuleInfo.BaseAddress);
                        if (funcAddr != 0)
                        {
                            if (!this.moduleBaseAddrToBreakpointMap.ContainsKey(module.ModuleInfo.BaseAddress))
                            {
                                this.moduleBaseAddrToBreakpointMap.Add(module.ModuleInfo.BaseAddress, new List<uint>());
                            }

                            this.moduleBaseAddrToBreakpointMap[module.ModuleInfo.BaseAddress].Add(funcAddr);
                            this.breakpointToMonitoredAPISymbol.Add(funcAddr, monitoredAPISymbol);
                            monitoredAPISymbol.SymbolLoadCount++;
                            try
                            {
                                this.debugTarget.SetBreakpoint(funcAddr);
                            }
                            catch (COMException)
                            {
                            }
                        }
                    }
                }
                catch (COMException)
                {
                }
            }
        }

        /// <summary>
        /// If path to XDBs changed, attempt to associate all registered API monitoring again
        /// </summary>
        public void RefreshSymbols()
        {
            lock (this.monitorAPILock)
            {
                if (this.debugTarget != null)
                {
                    this.debugTarget.RemoveAllBreakpoints();

                    this.moduleBaseAddrToBreakpointMap.Clear();
                    this.breakpointToMonitoredAPISymbol.Clear();

                    foreach (KeyValuePair<string, MonitoredAPISymbol> monitorSymbolsPair in this.monitorSymbols)
                    {
                        this.RefreshSymbol(monitorSymbolsPair.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Enables monitoring of API calls in the currently running title, given a symbol name.
        /// The debugger must already be connected.
        /// </summary>
        /// <param name="symbolName">symbol name of a function to monitor</param>
        /// <param name="d">Delegate to invoke when API call is detected</param>
        /// <returns>A MonitorAPISession representing the registered symbol</returns>
        public IMonitorAPISession MonitorAPI(string symbolName, MonitorAPIDelegate d)
        {
            MonitorAPISession result = null;
            MonitoredAPISymbol monitoredAPISymbol;
            if ((this.debugTarget != null) && !string.IsNullOrEmpty(symbolName) && !this.monitorSymbols.TryGetValue(symbolName, out monitoredAPISymbol))
            {
                monitoredAPISymbol = new MonitoredAPISymbol(this, symbolName);
                this.monitorSymbols.Add(symbolName, monitoredAPISymbol);
                result = new MonitorAPISession(monitoredAPISymbol, d);
                this.debugTarget.StopOn(XboxStopOnFlags.OnModuleLoad, true);

                this.RefreshSymbol(monitoredAPISymbol);
            }

            return result;
        }

        /// <summary>
        /// Enables monitoring of debug output.
        /// The Framework will automatically disabling monitoring when the module stops. 
        /// </summary>
        /// <param name="d">Delegate to use to monitor for Debug output</param>
        public void StartMonitoringDebugOutput(MonitorDebugDelegate d)
        {
            this.debugMonitorDelegate = d;
        }

        /// <summary>
        /// Disables calling of delegate previously passed to MonitorDebug()
        /// Implements ClearMonitorDebug in IXboxDevice
        /// </summary>
        public void StopMonitoringDebugOutput()
        {
            this.debugMonitorDelegate = null;
        }

        /// <summary>
        /// Attempt to connect to this Xbox.
        /// Note: Do not call Connect if the Xbox is already in the process of connecting.
        /// To avoid pausing the UI thread, the connection is attempted in another thread.
        /// The ConnectCompleteDelegate will be called from a thread other than the UI thread.
        /// </summary>
        /// <param name="d">Delegate to call when the connection attempt completes (or fails)</param>
        public void Connect(ConnectCompleteDelegate d)
        {
            this.connectCompleteDelegate = d;
            this.connecting = true;
            new Thread(this.ConnectThread).Start();
            ShutdownSynchronization.DeferShutdown();
        }

        /// <summary>
        /// Close the connection to the Xbox 360 console.
        /// Note: Disconnect() is intended to be used only when the object is about to be released.
        /// Do not try to reconnect an XboxDevice object after disconnecting it - create a new XboxDevice object.
        /// </summary>
        public void Disconnect()
        {
            lock (this.disconnectLock)
            {
                this.isDisconnectPending = true;
                if (this.disconnectDeferCount != 0)
                {
                    return;
                }
            }

            this.StopEmulatingDiscTitle();
            ShutdownSynchronization.DeferShutdown();
            new Thread(new ThreadStart(this.DisconnectThread)).Start();
        }

        /// <summary>
        /// Disconnect only the debugger, but do not disconnect the Xbox.
        /// If the debugger is subsequently connected again before disconnect is complete,
        /// the disconnect attempt will be aborted.
        /// </summary>
        public void DisconnectDebugger()
        {
            // Disconnecting a debugger only happens if the console is already connected.
            // There is no need to worry about contending over connection.  However,
            // the user might try to start using the debugger again before we've removed it.
            // if so, we need to protect those state changed (with lock). 
            lock (this.disconnectingDebuggerLock)
            {
                if (this.debugTarget != null)
                {
                    if (!this.disconnectingDebugger)
                    {
                        this.disconnectingDebugger = true;
                        ShutdownSynchronization.DeferShutdown();
                        new Thread(new ThreadStart(this.DisconnectDebuggerThread)).Start();
                    }
                }
            }
        }

        /// <summary>
        /// Starts monitoring for title failures such as asserts, exceptions and RIP
        /// </summary>
        /// <param name="d">delegate to receive notification of title failure</param>
        public void StartMonitoringTitleFailures(OnTitleFailureDelegate d)
        {
            this.onTitleFailureDelegate = d;
        }

        /// <summary>
        /// Stops monitoring for changes to this device's selected state
        /// Implements StopMonitoringSelectionChanges in IDevice
        /// </summary>
        public void StopMonitoringTitleFailures()
        {
            this.onTitleFailureDelegate = null;
        }

        /// <summary>
        /// Continues execution of the Xbox 360 console.
        /// </summary>
        /// <param name="eventInfo">Event info for the event being handled</param>
        public void ContinueExecution(IXboxEventInfo eventInfo)
        {
            bool isStopped = false;
            try
            {
                isStopped = eventInfo.Info.IsThreadStopped != 0;
                if (isStopped)
                {
                    lock (this.disconnectingDebuggerLock)
                    {
                        if (this.debugTarget != null)
                        {
                            try
                            {
                                eventInfo.Info.Thread.Continue(true);
                                bool notStopped;
                                this.debugTarget.Go(out notStopped);
                            }
                            catch (COMException)
                            {
                                // In case the user changes the title in the middle, debugTarget calls will throw exceptions
                            }
                        }
                    }
                }
            }
            catch (COMException)    
            {
                // in case the Xbox goes offline
            }

            if (isStopped)
            {
                XBOX_EVENT_INFO info = eventInfo.Info;

                Marshal.ReleaseComObject(eventInfo);

                if (info.Module != null)
                {
                    Marshal.ReleaseComObject(info.Module);
                }

                if (info.Section != null)
                {
                    Marshal.ReleaseComObject(info.Section);
                }

                if (info.Thread != null)
                {
                    Marshal.ReleaseComObject(info.Thread);
                }
            }
        }

        /// <summary>
        /// Uninstalls the currently configured content-package based title from the specified drive
        /// </summary>
        /// <param name="drive">Drive to uninstall the content-package based title from</param>
        /// <returns>true if successfully uninstalled, false on failure</returns>
        public bool UninstallContentPackageBasedTitle(string drive = "")
        {
            string driveName;
            if (string.IsNullOrEmpty(drive))
            {
                driveName = this.PrimaryDrive;
            }
            else
            {
                driveName = drive;
            }

            bool result = true;
            if (this.IsContentPackageBasedTitleInstalledOn(driveName))
            {
                try
                {
                    // Actually uninstalls both demo and non-demo for this title, if the other exists

                    // uninstall full title
                    string contentPackage = this.ContentPackageBasedTitleFileName;
                    if (!string.IsNullOrEmpty(contentPackage))
                    {
                        try
                        {
                            XboxConsole.DeleteFile(Path.Combine(driveName + @":\" + this.ContentPackageBasedTitleFilePath, contentPackage));
                        }
                        catch (Exception)
                        {
                            result = false;
                        }
                    }

                    // uninstall demo
                    string demoContentPackage = this.DemoContentPackageBasedTitleFileName;
                    if (!string.IsNullOrEmpty(demoContentPackage))
                    {
                        try
                        {
                            XboxConsole.DeleteFile(Path.Combine(driveName + @":\" + this.ContentPackageBasedTitleFilePath, demoContentPackage));
                        }
                        catch (Exception)
                        {
                            result = false;
                        }
                    }

                    // uninstall title update
                    if (this.IsHDDEnabled)
                    {
                        this.UninstallTitleUpdate("HDD");
                    }
                    else if (this.IsMUINTEnabled)
                    {
                        this.UninstallTitleUpdate("MUINT");
                    }
                    else if (this.IsINTUSBEnabled)
                    {
                        this.UninstallTitleUpdate("INTUSB");
                    }
                    else if (this.IsMU0Enabled)
                    {
                        this.UninstallTitleUpdate("MU0");
                    }
                    else if (this.IsMU1Enabled)
                    {
                        this.UninstallTitleUpdate("MU1");
                    }
                    else if (this.IsUSB0Enabled)
                    {
                        this.UninstallTitleUpdate("USBMASS0MU");
                    }
                    else if (this.IsUSB1Enabled)
                    {
                        this.UninstallTitleUpdate("USBMASS1MU");
                    }
                }
                catch (COMException)
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Starts disc image emulation for the specified disc image
        /// </summary>
        /// <param name="discImageFile">Disc image file to load</param>
        /// <param name="progressBar">An optional progress bar to report the progress of the copy operation</param>
        /// <returns>true if successful, false on failure</returns>
        public bool StartEmulatingDiscTitle(string discImageFile, IProgressBar progressBar = null)
        {
            KillUnownedEmulation();

            bool result = false;

            // If progress bar, defer to delegate called while progressBar is modal
            if (progressBar != null)
            {
                progressBar.Delegate = delegate { result = this.StartEmulatingDiscTitle_Internal(discImageFile, progressBar); };
                progressBar.Show();
            }
            else
            {
                result = this.StartEmulatingDiscTitle_Internal(discImageFile);
            }

            return result;
        }

        /// <summary>
        /// Starts disc image emulation for the currently configured Xbox title
        /// </summary>
        /// <returns>true if successful, false on failure</returns>
        /// <param name="progressBar">An optional progress bar to report the progress of the copy operation</param>
        public bool StartEmulatingDiscTitle(IProgressBar progressBar = null)
        {
            return this.StartEmulatingDiscTitle(this.xboxTitle.DiscImage, progressBar);
        }

        /// <summary>
        /// Disables disc image emulation
        /// </summary>
        public void StopEmulatingDiscTitle()
        {
            if (this.discEmulationProcessId != -1)
            {
                // If the process is still running
                if (Process.GetProcesses().Any(x => x.Id == this.discEmulationProcessId))
                {
                    StopEmulator(this.discEmulationProcessId);
                }

                discEmulationProcessIds.Remove(this.discEmulationProcessId);
                this.discEmulationProcessId = -1;

                if (this.IsHDDEnabled)
                {
                    this.UninstallTitleUpdate("HDD");
                }
                else if (this.IsMUINTEnabled)
                {
                    this.UninstallTitleUpdate("MUINT");
                }
                else if (this.IsINTUSBEnabled)
                {
                    this.UninstallTitleUpdate("INTUSB");
                }
                else if (this.IsMU0Enabled)
                {
                    this.UninstallTitleUpdate("MU0");
                }
                else if (this.IsMU1Enabled)
                {
                    this.UninstallTitleUpdate("MU1");
                }
                else if (this.IsUSB0Enabled)
                {
                    this.UninstallTitleUpdate("USBMASS0MU");
                }
                else if (this.IsUSB1Enabled)
                {
                    this.UninstallTitleUpdate("USBMASS1MU");
                }
            }
        }

        /// <summary>
        /// Checks if the currently configured content-package based title is installed
        /// </summary>
        /// <param name="drive">Drive to check for the title</param>
        /// <returns>A value indicating whether or not the title was found</returns>
        public bool IsContentPackageBasedTitleInstalledOn(string drive)
        {
            bool result = false;
            string titleFilePath = drive + @":\" + this.ContentPackageBasedTitleFilePath;
            
            string titleFileName;
            if (this.xboxTitle.UseDemo)
            {
                titleFileName = this.DemoContentPackageBasedTitleFileName;
            }
            else
            {
                titleFileName = this.ContentPackageBasedTitleFileName;
            }

            if (this.Connected && this.Responding && !string.IsNullOrEmpty(titleFilePath))
            {
                try
                {
                    IXboxFiles files = this.internalXboxConsole.DirectoryFiles(titleFilePath);
                    foreach (IXboxFile file in files)
                    {
                        string fileName = Path.GetFileName(file.Name);
                        if (fileName == titleFileName)
                        {
                            result = true;
                            break;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        /// <summary>
        /// Recursively copy the contents of a directory into a directory on the xbox.
        /// </summary>
        /// <param name="desktopDir">Desktop directory to copy contents from</param>
        /// <param name="xboxDir">Xbox directory to copy to</param>
        /// <param name="progressBar">An optional progress bar to report the progress of the copy operation</param>
        /// <returns>true on success, false on failure</returns>
        public bool RecursiveDirectoryCopy(string desktopDir, string xboxDir, IProgressBar progressBar = null)
        {
            bool result = false;

            // If progress bar, defer to delegate called while progressBar is modal
            if (progressBar != null)    
            {
                progressBar.Delegate = delegate { result = this.RecursiveDirectoryCopy_Internal(desktopDir, xboxDir, progressBar); };
                progressBar.Show();
            }
            else
            {
                result = this.RecursiveDirectoryCopy_Internal(desktopDir, xboxDir);
            }

            return result;
        }

        /// <summary>
        /// Recursively deletes the contents of a directory on the Xbox
        /// </summary>
        /// <param name="xboxDir">Xbox directory to delete</param>
        /// <param name="progressBar">An optional progress bar to report progress of the delete operation</param>
        /// <returns>true if successful, false on failure</returns>
        public bool RecursiveDirectoryDelete(string xboxDir, IProgressBar progressBar = null)
        {
            bool result = false;

            // If progress bar, defer to delegate called while progressBar is modal
            if (progressBar != null)    
            {
                progressBar.Delegate = delegate { result = this.RecursiveDirectoryDelete_Internal(xboxDir, progressBar); };
                progressBar.Show();
            }
            else
            {
                result = this.RecursiveDirectoryDelete_Internal(xboxDir);
            }

            return result;
        }

        /// <summary>
        /// Uninstalls a Raw title
        /// </summary>
        /// <param name="progressBar">An optional progress bar to report progress of the uninstall operation</param>
        /// <returns>true if successful, false on failure</returns>
        public bool UninstallRawTitle(IProgressBar progressBar = null)
        {
            bool result = false;
            if (this.Connected &&
                this.Responding &&
                this.IsRawTitleInstalled)
            {
                result = this.RecursiveDirectoryDelete(@"DEVKIT:\CATRawTitle", progressBar);
            }

            return result;
        }

        /// <summary>
        /// Install the currently configured game title on the specified drive.
        /// </summary>
        /// <param name="drive">Drive to install game title on, ignored if not a content-package based title</param>
        /// <param name="progressBar">An optional progress bar to report progress of the installation</param>
        /// <returns>true if successful, false on failure</returns>
        public bool InstallTitle(string drive = "", IProgressBar progressBar = null)
        {
            string driveName;
            if (string.IsNullOrEmpty(drive))
            {
                driveName = this.PrimaryDrive;
            }
            else
            {
                driveName = drive;
            }

            bool result = true;
            if (!this.IsTitleInstalled)
            {
                result = false;
                if (this.CanInstallContentPackageBasedTitle)
                {
                    // If progress bar, defer to delegate called while progressBar is modal
                    if (progressBar != null)
                    {
                        progressBar.Delegate = delegate { result = this.InstallContentPackageBasedTitle(driveName, progressBar); };
                        progressBar.Show();
                    }
                    else
                    {
                        result = this.InstallContentPackageBasedTitle(driveName);
                    }
                }
                else if (this.CanInstallDiscImageTitle)
                {
                    result = this.StartEmulatingDiscTitle(progressBar);
                }
                else if (this.CanInstallRawTitle)
                {
                    result = this.InstallRawTitle(progressBar);
                }
            }

            return result;
        }

        /// <summary>
        /// Launch the currently configured title.
        /// If a content package installed on multiple drives, a drive will be chosen automatically.
        /// </summary>
        /// <returns>true if successful, false on failure</returns>
        public bool LaunchTitle()
        {
            return this.LaunchTitle(this.GetAnyDriveContentPackageBasedTitleIsInstalledOn);
        }

        /// <summary>
        /// Launches the currently configured game title, if installed
        /// </summary>
        /// <param name="drive">Drive to launch the game title on, ignored if not a content-package based title</param>
        /// <returns>true if successful, false on failure</returns>
        public bool LaunchTitle(string drive = "")
        {
            string driveName;
            if (string.IsNullOrEmpty(drive))
            {
                driveName = this.PrimaryDrive;
            }
            else
            {
                driveName = drive;
            }

            bool result = false;
            if ((this.xboxTitle.GameInstallType == "Content Package") && this.IsContentPackageBasedTitleInstalled)
            {
                string titleFilePath = this.ContentPackageBasedTitleFilePath;
                try
                {
                    string localFile = "LaunchXContent.xex";
                    string launcherPath = @"HDD:\" + localFile;
                    this.internalXboxConsole.SendFile(localFile, launcherPath);

                    // Map PC drive names to Xbox drive names
                    string xboxTitlePath = @"\Device\";
                    switch (driveName)
                    {
                        case "HDD":
                            xboxTitlePath += @"Harddisk0\Partition1\";
                            break;
                        case "MU0":
                            xboxTitlePath += @"Mu0\";
                            break;
                        case "MU1":
                            xboxTitlePath += @"Mu1\";
                            break;
                        case "USBMASS0MU":
                            xboxTitlePath += @"Mass0PartitionFile\Storage\";
                            break;
                        case "USBMASS1MU":
                            xboxTitlePath += @"Mass1PartitionFile\Storage\";
                            break;
                        case "MUINT":
                            xboxTitlePath += @"BuiltInMuSfc\";
                            break;
                        case "INTUSB":
                            xboxTitlePath += @"BuiltInMuUsb\";
                            break;
                    }

                    string titleFileName;
                    if (this.xboxTitle.UseDemo)
                    {
                        titleFileName = this.DemoContentPackageBasedTitleFileName;
                    }
                    else
                    {
                        titleFileName = this.ContentPackageBasedTitleFileName;
                    }

                    xboxTitlePath += Path.Combine(titleFilePath, titleFileName);

                    this.internalXboxConsole.Reboot(launcherPath, null, xboxTitlePath, XboxRebootFlags.Title);
                    Thread.Sleep(3000);
                    result = true;
                }
                catch (COMException)
                {
                    result = false;
                }
            }
            else if ((this.xboxTitle.GameInstallType == "Disc Emulation") && this.IsDiscEmulationRunning)
            {
                string path = @"DVD:\default.xex";
                this.internalXboxConsole.Reboot(path, null, null, XboxRebootFlags.Title);
                Thread.Sleep(5000);
                result = true;
            }
            else if ((this.xboxTitle.GameInstallType == "Raw") && this.IsRawTitleInstalled)
            {
                string mediaPath = @"DEVKIT:\CATRawTitle";
                string path = @"DEVKIT:\CATRawTitle\default.xex";
                this.internalXboxConsole.Reboot(path, mediaPath, null, XboxRebootFlags.Title);
                Thread.Sleep(3000);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Takes a screenshot and stores it at the specified path.
        /// </summary>
        /// <param name="filePath">Path to write the screenshot to</param>
        /// <param name="convertToJpg">True to convert to jpg, false to save as bmp</param>
        /// <returns>true if successful, false on failure</returns>
        public bool ScreenShot(string filePath, bool convertToJpg = true)
        {
            bool success = false;
            if (this.Connected && this.Responding)
            {
                for (uint retry = 0; retry < 3; retry++)
                {
                    try
                    {
                        if (!convertToJpg)
                        {
                            this.internalXboxConsole.ScreenShot(filePath);
                        }
                        else
                        {
                            string tempFilePath = filePath + ".bmp";

                            try
                            {
                                File.Delete(tempFilePath);
                            }
                            catch
                            {
                            }

                            try
                            {
                                File.Delete(filePath);
                            }
                            catch
                            {
                            }

                            this.internalXboxConsole.ScreenShot(tempFilePath);

                            BitmapImage image = LoadImage(tempFilePath);
                            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(image));
                            using (var filestream = new FileStream(filePath, FileMode.Create))
                            {
                                encoder.Save(filestream);
                            }

                            File.Delete(tempFilePath);
                        }

                        success = true;
                        break;
                    }
                    catch (COMException)
                    {
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Waits for the device to become responsive after a reboot.
        /// Only one call to WaitForRebootToComplete() are valid to be outstanding against the same Xbox at any given time.
        /// </summary>
        public void WaitForRebootToComplete()
        {
            TimeSpan timeout = TimeSpan.FromSeconds(120);    // 2 minute timeout max, if we can't detect its back.
            DateTime startTime = DateTime.Now;
            DateTime dueTime = startTime.Add(timeout);

            // Assumes the device has already been rebooted
            Thread.Sleep(5000);

            this.Responding = false;
            this.XboxViewItem.NotifyPropertyChanged("IsOffline");

            bool isRestarting = true;
            try
            {
                this.internalXboxConsole.FindConsole(0, 0);
                isRestarting = false;
            }
            catch (Exception)
            {
            }

            if (isRestarting)
            {
                for (;;)
                {
                    bool wasTimeout = dueTime <= DateTime.Now;
                    if (wasTimeout)
                    {
                        break;
                    }

                    try
                    {
                        this.internalXboxConsole.FindConsole(0, 0);
                        break;
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(500);
                }

                Thread.Sleep(4000);

                for (;;)
                {
                    bool wasTimeout = dueTime <= DateTime.Now;
                    if (wasTimeout)
                    {
                        break;
                    }

                    try
                    {
                        this.internalXboxConsole.FindConsole(0, 0);
                        break;
                    }
                    catch (Exception)
                    {
                    }

                    Thread.Sleep(500);
                }

                // May still have bootanim.xex up, wait for it to exit.
                for (;;)
                {
                    try
                    {
                        List<string> runningProcesses = this.AllRunningProcesses;
                        bool wasTimeout = dueTime <= DateTime.Now;
                        if (wasTimeout)
                        {
                            break;
                        }

                        if ((runningProcesses.Contains("xshell.xex") || runningProcesses.Contains("default.xex")) && (!runningProcesses.Contains("bootanim.xex")))
                        {
                            break;
                        }
                    }
                    catch
                    {
                    }

                    Thread.Sleep(1000);
                }
            }

            this.Responding = true;
            this.ReconfigureConsole();

            // Call connection complete delegate every time the Xbox reboots.
            if (this.connectCompleteDelegate != null)
            {
                this.connectCompleteDelegate(this, true);
            }

            this.RefreshSymbols();
        }

        /// <summary>
        /// Launches the Developer Dashboard.
        /// </summary>
        /// <param name="wait">true to wait for the operation to complete, false to return immediately</param>
        public void LaunchDevDashboard(bool wait = true)
        {
            try
            {
                bool restartDiscEmulation = false;
                if (this.discEmulationProcessId != -1)
                {
                    restartDiscEmulation = true;
                    this.StopEmulatingDiscTitle();
                }
                else
                {
                    KillUnownedEmulation();
                }

                this.internalXboxConsole.Reboot(null, null, null, XboxRebootFlags.Title);
                
                if (restartDiscEmulation)
                {
                    this.StartEmulatingDiscTitle();
                }
                else if (wait)
                {
                    Thread.Sleep(5000);
                }
            }
            catch (COMException)
            {
            }
        }

        /// <summary>
        /// Performs a cold reboot of the Xbox.
        /// Only pass false for wait if there is a subsequent call to WaitForRebootToComplete().
        /// The call to WaitForRebootToComplete() is required to reestablish various an XboxConsole object and breakpoints
        /// To reboot asynchronously, run Reboot(true) in a thread.
        /// </summary>
        /// <param name="wait">true to wait for the reboot to complete, false to return immediately</param>
        public void Reboot(bool wait = true)
        {
            if (this.Connected && this.Responding)
            {
                this.Responding = false;
                this.XboxViewItem.NotifyPropertyChanged("IsOffline");
                try
                {
                    this.internalXboxConsole.Reboot(null, null, null, XboxRebootFlags.Cold);
                    if (wait)
                    {
                        this.WaitForRebootToComplete();
                    }
                }
                catch (COMException)
                {
                }
            }
        }

        /// <summary>
        /// Create a console profiles manager
        /// </summary>
        /// <returns>A new console profiles manager</returns>
        public ConsoleProfilesManager CreateConsoleProfilesManager()
        {
            return this.XboxConsole.CreateConsoleProfilesManager();
        }

        /// <summary>
        /// Removes this Xbox from the Xbox Neighborhood
        /// </summary>
        public void Remove()
        {
            if (this.IsDefault)
            {
                UnsetDefault();
            }

            xboxManager.RemoveConsole(this.ConnectTo);
        }

        /// <summary>
        /// Checks for the presence of the SEP
        /// </summary>
        /// <param name="drive">drive to check for the SEP</param>
        /// <returns>true if the SEP is present, false is it is not (or cannot be determined to be present)</returns>
        public bool HasSystemExtendedPackage(string drive = "")
        {
            string driveName;
            if (string.IsNullOrEmpty(drive))
            {
                driveName = this.PrimaryDrive;
            }
            else
            {
                driveName = drive;
            }

            bool result = false;
            string path;
            string fileNameSEP;
            if (drive == "HDD")
            {
                path = @"HDD:\Content\0000000000000000\FFFE07DF\00040000";
                fileNameSEP = "MicArrayCalibrate.mec"; // "ContentCache.pkg", "XlfsUploadCache.dat"
            }
            else
            {
                path = drive + @":\";
                fileNameSEP = "ExtendedSystem.Partition";
            }

            try
            {
                IXboxFiles files = this.internalXboxConsole.DirectoryFiles(path);
                foreach (IXboxFile file in files)
                {
                    string fileName = Path.GetFileName(file.Name);
                    if (fileName == fileNameSEP)
                    {
                        result = true;
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        /// <summary>
        /// Checks if the specified drive is enabled.
        /// </summary>
        /// <param name="drive">Drive to check for</param>
        /// <returns>true if the drive is present, false if it is not</returns>
        public bool IsDriveEnabled(string drive)
        {
            bool result = false;
            try
            {
                if (this.Connected && this.Responding)
                {
                    string drives = this.internalXboxConsole.Drives;
                    result = drives.Contains(drive);
                }
            }
            catch (COMException)
            {
            }

            return result;
        }

        /// <summary>
        /// Enable or Disable the Xbox 360 on-board HDD
        /// A reboot is required before this change takes affect.
        /// </summary>
        /// <param name="enable">Enable or Disable the on-board HDD</param>
        /// <param name="rebootOrReflashWhenDone">Whether or not to also flash or reboot when done</param>
        /// <param name="reflashOnDisable">true to flash on disable, false to just reboot</param>
        public void SetHDDEnabled(bool enable, bool rebootOrReflashWhenDone = true, bool reflashOnDisable = true)
        {
            if (this.Connected && this.Responding)
            {
                this.Run_xbsetcfg(string.Format("/X {0} /HDD {1}", this.IP, (enable == true) ? "enable" : "disable"));
                if (rebootOrReflashWhenDone)
                {
                    if (reflashOnDisable && !enable)
                    {
                        this.Flash(true);    // If disabling the HD, we need to reflash the device to install the System Extended package to the MU
                    }
                    else
                    {
                        this.Reboot();   // Need to reboot for the change to take affect
                    }
                }
            }
        }

        /// <summary>
        /// Runs the recovery exe with the specified options
        /// </summary>
        /// <param name="installSEP">true to also install the SEP</param>
        /// <param name="unattended">True to run in unattended mode, false to display UI</param>
        public void Flash(bool installSEP, bool unattended = false)
        {
            // Close profile manager
            if (this.XboxViewItem.ProfileManagerViewModel != null)
            {
                this.XboxViewItem.ProfileManagerViewModel.Close();
            }

            // Close debug output
            if (this.XboxViewItem.DebugOutputViewModel != null)
            {
                this.XboxViewItem.DebugOutputViewModel.Close();
            }

            this.Responding = false;
            this.XboxViewItem.NotifyPropertyChanged("IsOffline");

            string oldIP = this.IP;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = this.xboxTitle.XdkRecoveryPath;
            startInfo.Arguments = "/F /S /fhdd /T min";
            if (unattended)
            {
                startInfo.Arguments += " /U ";
            }

            if (!installSEP)
            {
                startInfo.Arguments += " /ED";
            }

            startInfo.Arguments += " /X " + this.IP;
            if (unattended)
            {
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }

            Process process = new Process();
            process.StartInfo = startInfo;
            try
            {
                process.Start();
            }
            catch (Exception)
            {
                return;
            }

            process.WaitForExit();
            this.WaitForRebootToComplete();

            // At this point the old XboxConsole connection is gone, replace even the XboxConsole object used by modules
            Marshal.ReleaseComObject(this.xboxConsole);
            this.xboxConsole = xboxManager.OpenConsole(oldIP);
        }

        /// <summary>
        /// Sets the current languages of the Xbox (dashboard)
        /// </summary>
        /// <param name="lang">Language to set it to</param>
        /// <param name="rebootNow">Whether or not to launch the developer dashboard</param>
        public void SetLanguage(Language lang, bool rebootNow = true)
        {
            string languageString = string.Empty;
            switch (lang)
            {
                case Language.English:
                    languageString = "1";
                    break;
                case Language.Japanese:
                    languageString = "2";
                    break;
                case Language.German:
                    languageString = "3";
                    break;
                case Language.French:
                    languageString = "4";
                    break;
                case Language.Spanish:
                    languageString = "5";
                    break;
                case Language.Italian:
                    languageString = "6";
                    break;
                case Language.Korean:
                    languageString = "7";
                    break;
                case Language.TraditionalChinese:
                    languageString = "8";
                    break;
                case Language.BrazilianPortuguese:
                    languageString = "9";
                    break;
                case Language.Polish:
                    languageString = "B";
                    break;
                case Language.Russian:
                    languageString = "C";
                    break;
                case Language.Swedish:
                    languageString = "D";
                    break;
                case Language.Turkish:
                    languageString = "E";
                    break;
                case Language.Norwegian:
                    languageString = "F";
                    break;
                case Language.Dutch:
                    languageString = "10";
                    break;
                case Language.SimplifiedChinese:
                    languageString = "11";
                    break;
            }

            this.Run_xbsetcfg(string.Format("/X {0} /LANG {1}", this.IP, languageString));

            // note you must reboot the development kit for the change to take affect
            // cold reboot is required due to caching issues (per TCR documentation)
            if (rebootNow)
            {
                this.Reboot();
            }
        }

        /// <summary>
        /// Sets the video mode of the xbox
        /// </summary>
        /// <param name="resolution">Resolution to set to</param>
        /// <param name="mode">Video format to set to</param>
        /// <param name="reboot">true to launch the developer dashboard to cause the settings to take effect now</param>
        public void SetVideoMode(VideoResolution resolution, VideoStandard mode, bool reboot)
        {
            switch (resolution)
            {
                // set VGA resolution
                case VideoResolution.Mode640x480:
                case VideoResolution.Mode640x480Wide:
                case VideoResolution.Mode848x480:
                case VideoResolution.Mode1024x768:
                case VideoResolution.Mode1024x768Wide:
                case VideoResolution.Mode1280x720:
                case VideoResolution.Mode1280x768:
                case VideoResolution.Mode1280x1024:
                case VideoResolution.Mode1280x1024Wide:
                case VideoResolution.Mode1360x768:
                case VideoResolution.Mode1440x900:
                case VideoResolution.Mode1680x1050:
                case VideoResolution.Mode1920x1080:
                    this.SetVGAVideoMode(resolution, reboot);
                    break;

                // set other resolution
                case VideoResolution.Mode480:
                case VideoResolution.Mode480Wide:
                case VideoResolution.Mode720p:
                case VideoResolution.Mode1080i:
                case VideoResolution.Mode1080p:
                    switch (mode)
                    {
                        case VideoStandard.NTSCJ:
                            this.SetNonVGAVideoMode(resolution, VideoStandard.NTSCJ, reboot);
                            break;
                        case VideoStandard.NTSCM:
                            this.SetNonVGAVideoMode(resolution, VideoStandard.NTSCM, reboot); 
                            break;
                        case VideoStandard.PAL50:
                        case VideoStandard.PAL60:
                            this.SetNonVGAVideoMode(resolution, VideoStandard.NTSCM, false); 
                            break;
                    }

                    break;
            }

            // set PAL if necessary
            switch (mode)
            {
                case VideoStandard.PAL50:
                    this.SwitchToPAL(false, reboot);
                    break;
                case VideoStandard.PAL60:
                    this.SwitchToPAL(true, reboot);
                    break;
            }
        }

        /// <summary>
        /// Gets the current video resolution.
        /// </summary>
        /// <param name="width">Receives the current width</param>
        /// <param name="height">Receives the current height</param>
        public void GetCurrentResolution(out int width, out int height)
        {
            // Take a screen shot
            // Save it in a temp folder
            // Open it, get it's height and width
            // Delete it.
            string tempPath = Path.GetTempPath();
            string tempFile = "CatTempScreenshot.bmp";
            string tempFullPath = Path.Combine(tempPath, tempFile);
            File.Delete(tempFullPath);

            this.internalXboxConsole.ScreenShot(tempFullPath);

            Bitmap bitMap = new Bitmap(tempFullPath);
            width = bitMap.Width;
            height = bitMap.Height;

            bitMap.Dispose();
            File.Delete(tempFile);
        }

        /// <summary>
        /// Returns a screen shot
        /// </summary>
        /// <returns>The screen shot image</returns>
        public BitmapImage ScreenShot()
        {
            // Take a screen shot
            // Save it in a temp folder
            // Open it, get it's height and width
            // Delete it.
            string tempPath = Path.GetTempPath();
            string tempFile = "CatTempScreenshot.bmp";
            string tempFullPath = Path.Combine(tempPath, tempFile);
            File.Delete(tempFullPath);

            this.internalXboxConsole.ScreenShot(tempFullPath);

            BitmapImage image = new BitmapImage();
            using (FileStream stream = File.OpenRead(tempFullPath))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = stream;
                image.EndInit();
            }

            return image;
        }

        /// <summary>
        /// Helper device function that activates the Friends Status Presence Screen, if only after the current selected
        /// profile of the Xbox Device this is called against is signed in. This operation makes use of a an IXboxAutomation
        /// compiled script to cause this required input sequence.
        /// </summary>
        /// <param name="profile">The current Console Profile that IXboxAutomation script is executing on.</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        public bool OpenFriendPresenceScreen(ConsoleProfile profile)
        {
            return this.RunIXBoxAutomationScript(@"Scripts\Open_Friend_Presence_Screen.xboxautomation");
        }

        /// <summary>
        /// Helper device function that wakes up the Xbox Developer kits currently under test by moving the right thumb arrow
        /// key to the right one time. This operation makes use of a an IXboxAutomation compiled script to cause this
        /// required input sequence.
        /// </summary>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        public bool WakeUpXboxDevkit()
        {
            return this.RunIXBoxAutomationScript(@"Scripts\Wake_Up_Xbox_Devkit.xboxautomation");
        }

        /// <summary>
        /// This function is an IXBoxAutomation utility function that accepts a script file name that specifies what
        /// IXBoxAutomation script to execute, and then runs it.
        /// Note: Scripts comments are acceptable, but must be as [//] double forward slash comment, and also must
        /// only begin any separate line to be ignored.
        /// </summary>
        /// <param name="scriptFileName">IXBoxAutomation script file name to execute</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        public bool RunIXBoxAutomationScript(string scriptFileName)
        {
            bool result = false;
            uint queueLength = 0; // QueryGamepadQueue out parameter
            uint itemsInQueue = 0; // QueryGamepadQueue out parameter
            uint timedDurationRemaining = 0; // QueryGamepadQueue out parameter
            uint countDurationRemaining = 0; // QueryGamepadQueue out parameter
            if (!string.IsNullOrEmpty(scriptFileName))
            {
                try
                {
                    using (StreamReader script = new StreamReader(File.Open(scriptFileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        string strInput = script.ReadToEnd();
                        string[] arrayInput = strInput.Split(new string[] { "\r\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);
                        XBOX_AUTOMATION_GAMEPAD gamepad = new XBOX_AUTOMATION_GAMEPAD();
                        IXboxAutomation.BindController((uint)ControllerPort.Number1, 999);
                        IXboxAutomation.ClearGamepadQueue((uint)ControllerPort.Number1);
                        IXboxAutomation.ConnectController((uint)ControllerPort.Number1);
                        foreach (string input in arrayInput)
                        {
                            if (input.StartsWith("wait ") && input.Count() > 5)
                            {
                                uint waitTimeMilliSeconds = uint.Parse(input.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)[1]);
                                gamepad = new XBOX_AUTOMATION_GAMEPAD();
                                IXboxAutomation.QueueGamepadState((uint)ControllerPort.Number1, gamepad, waitTimeMilliSeconds, 0);
                            }

                            switch (input)
                            {
                                case "A_Button":
                                    gamepad.Buttons = XboxAutomationButtonFlags.A_Button;
                                    break;
                                case "B_Button":
                                    gamepad.Buttons = XboxAutomationButtonFlags.B_Button;
                                    break;
                                case "BackButton":
                                    gamepad.Buttons = XboxAutomationButtonFlags.BackButton;
                                    break;
                                case "Bind_Button":
                                    gamepad.Buttons = XboxAutomationButtonFlags.Bind_Button;
                                    break;
                                case "DPadDown":
                                    gamepad.Buttons = XboxAutomationButtonFlags.DPadDown;
                                    break;
                                case "DPadLeft":
                                    gamepad.Buttons = XboxAutomationButtonFlags.DPadLeft;
                                    break;
                                case "DPadRight":
                                    gamepad.Buttons = XboxAutomationButtonFlags.DPadRight;
                                    break;
                                case "DPadUp":
                                    gamepad.Buttons = XboxAutomationButtonFlags.DPadUp;
                                    break;
                                case "LeftShoulderButton":
                                    gamepad.Buttons = XboxAutomationButtonFlags.LeftShoulderButton;
                                    break;
                                case "LeftThumbButton":
                                    gamepad.Buttons = XboxAutomationButtonFlags.LeftThumbButton;
                                    break;
                                case "RightShoulderButton":
                                    gamepad.Buttons = XboxAutomationButtonFlags.RightShoulderButton;
                                    break;
                                case "RightThumbButton":
                                    gamepad.Buttons = XboxAutomationButtonFlags.RightThumbButton;
                                    break;
                                case "StartButton":
                                    gamepad.Buttons = XboxAutomationButtonFlags.StartButton;
                                    break;
                                case "X_Button":
                                    gamepad.Buttons = XboxAutomationButtonFlags.X_Button;
                                    break;
                                case "Xbox360_Button":
                                    gamepad.Buttons = XboxAutomationButtonFlags.Xbox360_Button;
                                    break;
                                case "Y_Button":
                                    gamepad.Buttons = XboxAutomationButtonFlags.Y_Button;
                                    break;
                                case "launchdevdashboard":
                                    // Wait for queue to catch up, since this needs to happen when the queue is done
                                    do
                                    {
                                        IXboxAutomation.QueryGamepadQueue((uint)ControllerPort.Number1, out queueLength, out itemsInQueue, out timedDurationRemaining, out countDurationRemaining);
                                        Thread.Sleep(50); // Wait here 50ms until itemsInQueue drops to 0.
                                    }
                                    while (0 < itemsInQueue);

                                    this.LaunchDevDashboard();
                                    continue;
                                case "waitforreboottocomplete":
                                    // Wait for queue to catch up, since this needs to happen when the queue is done
                                    do
                                    {
                                        try
                                        {
                                            IXboxAutomation.QueryGamepadQueue((uint)ControllerPort.Number1, out queueLength, out itemsInQueue, out timedDurationRemaining, out countDurationRemaining);
                                        }
                                        catch
                                        {
                                            // Rebooting
                                        }

                                        Thread.Sleep(50); // Wait here 50ms until itemsInQueue drops to 0.
                                    }
                                    while (0 < itemsInQueue);

                                    this.WaitForRebootToComplete();
                                    IXboxAutomation.BindController((uint)ControllerPort.Number1, 999);
                                    IXboxAutomation.ConnectController((uint)ControllerPort.Number1);
                                    continue;
                                default:
                                    continue;
                            }

                            IXboxAutomation.QueueGamepadState((uint)ControllerPort.Number1, gamepad, 200, 0);
                            result = true;
                        } // End of: foreach (string input in arrayInput)

                        do
                        {
                            IXboxAutomation.QueryGamepadQueue((uint)ControllerPort.Number1, out queueLength, out itemsInQueue, out timedDurationRemaining, out countDurationRemaining);
                            Thread.Sleep(50); // Wait here 50ms until itemsInQueue drops to 0.
                        }
                        while (0 < itemsInQueue);
                    } // End of: using (StreamReader script = new StreamReader(File.Open(scriptFileName, FileMode.Open, FileAccess.Read, FileShare.Read)))
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    result = false;
                }
                finally
                {
                    this.IXboxAutomation.UnbindController((uint)ControllerPort.Number1);
                }
            }

            return result;
        }

        /// <summary>
        /// This function runs a script that lives in the standard CAT location
        /// </summary>
        /// <param name="scriptShortName">name of the script with no paths and no extensions</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        public bool RunCatScript(string scriptShortName)
        {
            string script = "Scripts/" + scriptShortName + ".xboxautomation";
            return this.RunIXBoxAutomationScript(script);
        }

        /// <summary>
        /// Change the current Xbox console region
        /// </summary>
        /// <param name="region">Region to change to</param>
        /// <returns>true if successful, false on failure</returns>
        public bool SetRegion(Region region)
        {
            bool result = false;

            switch (region)
            {
                case Region.Asia:
                    result = this.RunIXBoxAutomationScript(@"Scripts\Asia_Region.xboxautomation");
                    break;
                case Region.Australia:
                    result = this.RunIXBoxAutomationScript(@"Scripts\Australia_NewZealand_Region.xboxautomation");
                    break;
                case Region.China:
                    result = this.RunIXBoxAutomationScript(@"Scripts\China_Region.xboxautomation");
                    break;
                case Region.Europe:
                    result = this.RunIXBoxAutomationScript(@"Scripts\Europe_Region.xboxautomation");
                    break;
                case Region.Japan:
                    result = this.RunIXBoxAutomationScript(@"Scripts\Japan_Region.xboxautomation");
                    break;
                case Region.NorthAmerica:
                    result = this.RunIXBoxAutomationScript(@"Scripts\North_American_Region.xboxautomation");
                    break;
                case Region.RestOfWorld:
                    result = this.RunIXBoxAutomationScript(@"Scripts\Rest_of_World_Region.xboxautomation");
                    break;
            }

            return false;
        }

        /// <summary>
        /// Get Free Space on the Xbox in bytes
        /// </summary>
        /// <param name="drive">drive to get free space from</param>
        /// <returns>Free Space on the Xbox in bytes</returns>
        public ulong GetFreeSpace(string drive = "")
        {
            string drivename;
            if (string.IsNullOrEmpty(drive))
            {
                drivename = this.PrimaryDrive;
            }
            else
            {
                drivename = drive;
            }

            // Point native IP calls to this Xbox
            XboxDebugManagerNative.DmSetXboxNameNoRegister(this.IP);

            ulong freebytes = 0, totalbytes = 0, totalfreebytes = 0;
            XboxDebugManagerNative.HResult error = 0;

            if (!drivename.EndsWith(":\\"))
            {
                drivename += ":\\";
            }

            error = XboxDebugManagerNative.DmGetDiskFreeSpace(drivename, out freebytes, out totalbytes, out totalfreebytes);

            if (((uint)error & 0x80000000) != 0)
            {
                throw new Exception("Error " + error.ToString() + " reading drive " + drive + " on console " + this.IP);
            }

            return freebytes;
        }

        /// <summary>
        /// Get Total Space on the Xbox in bytes
        /// </summary>
        /// <param name="drive">Drive to check the total space of</param>
        /// <returns>Total Space on the Xbox in bytes</returns>
        public ulong TotalSpace(string drive = "")
        {
            string drivename;
            if (string.IsNullOrEmpty(drive))
            {
                drivename = this.PrimaryDrive;
            }
            else
            {
                drivename = drive;
            }

            // Point native IP calls to this Xbox
            XboxDebugManagerNative.DmSetXboxNameNoRegister(this.IP);

            ulong freebytes = 0, totalbytes = 0, totalfreebytes = 0;
            XboxDebugManagerNative.HResult error = 0;

            if (!drivename.EndsWith(":\\"))
            {
                drivename += ":\\";
            }

            error = XboxDebugManagerNative.DmGetDiskFreeSpace(drivename, out freebytes, out totalbytes, out totalfreebytes);

            if (((uint)error & 0x80000000) != 0)
            {
                throw new Exception("Error " + error.ToString() + " reading drive " + drive + " on console " + this.IP);
            }

            return totalbytes;
        }

        /// <summary>
        /// Fills the specified drive
        /// </summary>
        /// <param name="freeBytes">the number of bytes to leave free on the drive</param>
        /// <param name="drive">the name of the drive that will be filled</param>
        /// <returns>true if successful</returns>
        public bool FillDrive(ulong freeBytes = 0, string drive = "")
        {
            string fillGuid;
            string fillFile;
            string drivename;
            if (string.IsNullOrEmpty(drive))
            {
                drivename = this.PrimaryDrive;
            }
            else
            {
                drivename = drive;
            }

            if (!drivename.EndsWith(":\\"))
            {
                drivename += ":\\";
            }

            ulong remainingtofill = this.GetFreeSpace(drive) - freeBytes;
            uint filesize = int.MaxValue; // largest file size that seems to always work

            while (remainingtofill > 0)
            {
                if (remainingtofill < int.MaxValue)
                {
                    filesize = (uint)remainingtofill;
                }

                fillGuid = Guid.NewGuid().ToString("N");
                fillFile = drivename + "junk_" + fillGuid + ".cat";
                try
                {
                    this.internalXboxConsole.SetFileSize(fillFile, filesize, XboxCreateDisposition.CreateAlways);
                }
                catch (Exception e)
                {
                    if (((uint)e.HResult) == (uint)XboxDebugManagerNative.HResult.XBDM_DEVICEFULL) 
                    {
                        // device full
                        return true;
                    }
                    else if (((uint)e.HResult) == (uint)XboxDebugManagerNative.HResult.XBDM_CONNECTIONLOST) 
                    {
                        // lost connection
                        throw new Exception("Lost connection to " + this.IP);
                    }
                    else
                    {
                        throw;
                    }
                }

                // Ask xbox for free space, it may have been reduced by an amount different that what we just wrote
                if (this.GetFreeSpace(drive) > freeBytes)
                {
                    remainingtofill = this.GetFreeSpace(drive) - freeBytes;
                }
                else
                {
                    remainingtofill = 0;
                }
            }

            return true;
        }

        /// <summary>
        /// Adds a 1KB file to the specified drive
        /// </summary>
        /// <param name="drive">The drive that will have a 1KB file added</param>
        /// <returns>true if successful</returns>
        public bool Add1KBToDrive(string drive = "")
        {
            bool result = false;
            string fillGuid;
            string fillFile;
            string drivename;

            if (string.IsNullOrEmpty(drive))
            {
                drivename = this.PrimaryDrive;
            }
            else
            {
                drivename = drive;
            }

            if (!drivename.EndsWith(":\\"))
            {
                drivename += ":\\";
            }

            if (this.GetFreeSpace(drive) >= 1024)
            {
                fillGuid = Guid.NewGuid().ToString("N");
                fillFile = drivename + "junk_" + fillGuid + ".cat";
                try
                {
                    this.internalXboxConsole.SetFileSize(fillFile, 1024, XboxCreateDisposition.CreateAlways);
                }
                catch (Exception e)
                {
                    if (((uint)e.HResult) != (uint)XboxDebugManagerNative.HResult.XBDM_DEVICEFULL) 
                    {
                        // not device full
                        if (((uint)e.HResult) == (uint)XboxDebugManagerNative.HResult.XBDM_CONNECTIONLOST) 
                        {
                            // lost connection
                            throw new Exception("Lost connection to " + this.IP);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                result = true;
            }

            return result;
        }

        /// <summary>
        /// Launches an explorer window for this Xbox.
        /// </summary>
        /// <param name="subdirectory">If set, specifies the drive to open in Explorer</param>
        public void LaunchExplorer(string subdirectory = "")
        {
            // XboxManager.OpenWindowsExplorer() does not work.
            // Explore the console by calling Explorer with the Neighborhood plugin.
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = string.Concat(@"::{57B2D0D9-32DD-476a-B663-C85D2EA24AA5}\", this.IP, "\\", subdirectory);
            startInfo.Arguments.TrimEnd('\\');
            startInfo.FileName = Path.Combine(Environment.ExpandEnvironmentVariables("%windir%"), "explorer.exe");
            startInfo.UseShellExecute = false;
            Process p = Process.Start(startInfo);
        }

        /// <summary>
        /// Synchronizes the Xbox with the current time on this PC
        /// </summary>
        public void SynchronizeTimeWithPC()
        {
            this.Run_xbsetcfg(string.Format("/X {0} /t", this.IP));
        }

        /// <summary>
        /// Enables/Disables the additional 512M of RAM on an Xbox that supports up to 1G.
        /// </summary>
        /// <param name="enable">true to enable 1GB, false to use 512M</param>
        public void Set1GBEnabled(bool enable)
        {
            if (this.Connected && this.Responding)
            {
                this.Run_xbsetcfg(string.Format("/X {0} /M {1}", this.IP, (enable == true) ? "on" : "off"));

                // automatically reboots.  Wait for it.
                this.WaitForRebootToComplete();
            }
        }

        /// <summary>
        /// Generates a Dump (optionally with heap), saving to the specified file.
        /// </summary>
        /// <param name="filename">Name of file to generate the dump to</param>
        /// <param name="withHeap">Whether or not to include the full heap</param>
        public void SaveDump(string filename, bool withHeap)
        {
            try
            {
                // NOTE: IXboxDebugTarget::WriteDump cannot always overwrite a file.
                // If the file exists, delete it first.
                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                // Generate the minidump.
                XboxDumpFlags flags = withHeap ? XboxDumpFlags.WithFullMemory : XboxDumpFlags.Normal;
                this.debugTarget.WriteDump(filename, flags);
            }
            catch (COMException)
            {
            }
        }

        /// <summary>
        /// Delete all profiles from the specified drive
        /// </summary>
        /// <param name="drive">Drive to delete all profiles from</param>
        /// <returns>true if successful, false if something failed to delete</returns>
        public bool DeleteAllProfilesFrom(string drive)
        {
            bool result = true;

            if (this.Connected && this.Responding && this.IsDriveEnabled(drive))
            {
                try
                {
                    this.internalXboxConsole.CreateConsoleProfilesManager().SignOutAllUsers();
                }
                catch (COMException)
                {
                    return false;
                }

                IXboxFiles files;
                string contentPath = drive + @":\Content";
                try
                {
                    files = this.internalXboxConsole.DirectoryFiles(contentPath);
                }
                catch (COMException ex) 
                {
                    if ((uint)ex.HResult == (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE)
                    {
                        return true;
                    }
                    
                    throw;
                }

                foreach (IXboxFile file in files)
                {
                    string fileName = Path.GetFileName(file.Name);
                    if ((fileName != "0000000000000000") && file.IsDirectory)
                    {
                        result &= this.RecursiveDirectoryDelete(file.Name);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Delete all profiles from all drives
        /// </summary>
        /// <returns>true if successful, false if something failed to delete</returns>
        public bool DeleteAllProfiles()
        {
            bool result = this.DeleteAllProfilesFrom("HDD");
            result &= this.DeleteAllProfilesFrom("MU0");
            result &= this.DeleteAllProfilesFrom("MU1");
            result &= this.DeleteAllProfilesFrom("MUINT");
            result &= this.DeleteAllProfilesFrom("INTUSB");
            result &= this.DeleteAllProfilesFrom("USBMASS0MU");
            result &= this.DeleteAllProfilesFrom("USBMASS1MU");
            return result;
        }

        /// <summary>
        /// Delete all saves from the specified drive
        /// </summary>
        /// <param name="drive">Drive to delete all saves from</param>
        /// <returns>true if successful, false if something failed to delete</returns>
        public bool DeleteAllSavesFrom(string drive)
        {
            bool result = true;
            if (this.Connected && this.Responding && this.IsDriveEnabled(drive))
            {
                IXboxFiles files;
                string contentPath = drive + @":\Content\0000000000000000";
                try
                {
                    files = this.internalXboxConsole.DirectoryFiles(contentPath);
                }
                catch (COMException ex)
                {
                    if ((uint)ex.HResult == (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE)
                    {
                        return true;
                    }

                    throw;
                }

                foreach (IXboxFile file in files)
                {
                    string fileName = Path.GetFileName(file.Name);
                    if (file.IsDirectory)
                    {
                        if (fileName.ToUpper() == "FFFE07D1")
                        {
                            result &= this.RecursiveDirectoryDelete(Path.Combine(file.Name, "00020000"));
                        }
                        else if (fileName.ToUpper() != "FFFE07DF")
                        {
                            result &= this.RecursiveDirectoryDelete(Path.Combine(file.Name, "00000001"));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Delete all title saves from the specified drive
        /// </summary>
        /// <param name="drive">Drive to delete all title saves from</param>
        /// <returns>true if successful, false if something failed to delete</returns>
        public bool DeleteAllTitleSavesFrom(string drive)
        {
            bool result = true;
            if (this.Connected && this.Responding && this.IsDriveEnabled(drive))
            {
                // delete any avatar saves
                IXboxFiles files;
                string contentPath = drive + @":\Content\0000000000000000\FFFF07D1\00020000\" + this.TitleId;
                try
                {
                    this.internalXboxConsole.DeleteFile(contentPath);
                }
                catch (COMException ex)
                {
                    if ((uint)ex.HResult != (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE)
                    {
                        throw;
                    }
                }

                // delete any shared saves
                contentPath = drive + @":\Content\0000000000000000\" + this.TitleId;
                try
                {
                    files = this.internalXboxConsole.DirectoryFiles(contentPath);
                }
                catch (COMException ex)
                {
                    if ((uint)ex.HResult != (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE)
                    {
                        throw;
                    }
                }

                // delete any standard saves for this title on any profile
                contentPath = drive + @":\Content";
                try
                {
                    files = this.internalXboxConsole.DirectoryFiles(contentPath);

                    foreach (IXboxFile file in files)
                    {
                        string fileName = Path.GetFileName(file.Name);
                        if ((fileName != "0000000000000000") && file.IsDirectory)
                        {
                            try
                            {
                                result &= this.RecursiveDirectoryDelete(this.TitleId);
                            }
                            catch (COMException ex)
                            {
                                if ((uint)ex.HResult != (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }
                catch (COMException ex)
                {
                    if ((uint)ex.HResult != (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE)
                    {
                        throw;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Delete all saves from all drives
        /// </summary>
        /// <returns>true if successful, false if something failed to delete</returns>
        public bool DeleteAllSaves()
        {
            bool result = this.DeleteAllSavesFrom("HDD");
            result &= this.DeleteAllSavesFrom("MU0");
            result &= this.DeleteAllSavesFrom("MU1");
            result &= this.DeleteAllSavesFrom("MUINT");
            result &= this.DeleteAllSavesFrom("INTUSB");
            result &= this.DeleteAllSavesFrom("USBMASS0MU");
            result &= this.DeleteAllSavesFrom("USBMASS1MU");
            return result;
        }

        /// <summary>
        /// Deletes all games from the specified drive
        /// </summary>
        /// <param name="drive">Drive to delete all games from</param>
        /// <returns>true if successful, false if something failed to delete</returns>
        public bool DeleteAllGamesFrom(string drive)
        {
            bool result = true;
            if (this.Connected && this.Responding && this.IsDriveEnabled(drive))
            {
                if (drive == "HDD")
                {
                    this.UninstallRawTitle();
                }

                this.UninstallTitleUpdate(drive);

                IXboxFiles files;
                string contentPath = drive + @":\Content\0000000000000000";
                try
                {
                    files = this.internalXboxConsole.DirectoryFiles(contentPath);
                }
                catch (COMException ex)
                {
                    if ((uint)ex.HResult == (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE)
                    {
                        return true;
                    }

                    throw;
                }

                foreach (IXboxFile file in files)
                {
                    string fileName = Path.GetFileName(file.Name);
                    if (file.IsDirectory)
                    {
                        if ((fileName != "FFFE07D1") && (fileName != "FFFE07DF"))
                        {
                            result &= this.RecursiveDirectoryDelete(file.Name);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes all games from all drives
        /// </summary>
        /// <returns>true if successful, false if something failed to delete</returns>
        public bool DeleteAllGames()
        {
            bool result = this.DeleteAllGamesFrom("HDD");
            result &= this.DeleteAllGamesFrom("MU0");
            result &= this.DeleteAllGamesFrom("MU1");
            result &= this.DeleteAllGamesFrom("MUINT");
            result &= this.DeleteAllGamesFrom("INTUSB");
            result &= this.DeleteAllGamesFrom("USBMASS0MU");
            result &= this.DeleteAllGamesFrom("USBMASS1MU");
            this.StopEmulatingDiscTitle();
            return result;
        }

        /// <summary>
        /// run any script file
        /// </summary>
        public void RunScript()
        {
            // Browse
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts");
            dlg.DefaultExt = "xboxautomation";
            bool result = (bool)dlg.ShowDialog();
            if (result)
            {
                this.RunIXBoxAutomationScript(dlg.FileName);
            }
        }

        /// <summary>
        /// Sends the specified file to the xbox
        /// </summary>
        /// <param name="sourcePath">Path to file on PC to send</param>
        /// <param name="destPath">Path to write to on Xbox</param>
        /// <param name="progressBar">An optional progress bar to report progress of the file send operation</param>
        /// <returns>true if successful, false on failure</returns>
        public bool SendFile(string sourcePath, string destPath, IProgressBar progressBar = null)
        {
            bool result = false;

            // If progress bar, defer to delegate called while progressBar is modal
            if (progressBar != null)
            {
                progressBar.Delegate = delegate { result = this.SendFile_Internal(sourcePath, destPath, progressBar); };
                progressBar.Show();
            }
            else
            {
                result = this.SendFile_Internal(sourcePath, destPath);
            }

            return result;
        }

        /// <summary>
        /// Defer subsequent attempts to disconnect
        /// </summary>
        public void DeferDisconnect()
        {
            lock (this.disconnectLock)
            {
                this.disconnectDeferCount++;
                ShutdownSynchronization.DeferShutdown();
            }
        }

        /// <summary>
        /// Remove a disconnect deferral
        /// </summary>
        public void AllowDisconnect()
        {
            lock (this.disconnectLock)
            {
                this.disconnectDeferCount--;
                if (this.disconnectDeferCount == 0)
                {
                    if (this.isDisconnectPending)
                    {
                        this.Disconnect();
                    }
                }

                ShutdownSynchronization.AllowShutdown();
            }
        }

        /// <summary>
        /// Checks if a directory exists on the Xbox
        /// </summary>
        /// <param name="xboxDir">Directory to check for the existence of</param>
        /// <returns>true if the directory exists, false otherwise</returns>
        public bool DirectoryExists(string xboxDir)
        {
            bool result = true;
            IXboxFiles xboxFiles;
            string[] xboxDirParts = xboxDir.Split('\\');
            string pathSoFar = xboxDirParts[0] + @"\";
            for (int i = 1; i < xboxDirParts.Length; i++)
            {
                pathSoFar = Path.Combine(pathSoFar, xboxDirParts[i]);
                try
                {
                    xboxFiles = this.internalXboxConsole.DirectoryFiles(xboxDir);
                }
                catch (Exception ex)
                {
                    if ((uint)ex.HResult == (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE)
                    {
                        result = false;
                        break;
                    }

                    throw;
                }
            }
            
            return result;
        }

        /// <summary>
        /// Prompts use to kill all unused emulator instances.
        /// Must only be called when CAT is not running emulation.
        /// </summary>
        private static void KillUnownedEmulation()
        {
            IEnumerable<Process> processes = Process.GetProcessesByName("xbEmulateGUI").Where(p => !discEmulationProcessIds.Contains(p.Id));
            if (processes.Count() > 0)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Disc emulation is running outside of CAT.  Shut it down now?", "Disc Emulation Already Running", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    foreach (Process process in processes)
                    {
                        StopEmulator(process.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Stops an instance of the Xbox Disc Emulator
        /// </summary>
        /// <param name="processId">Process Id of emulator process to stop</param>
        private static void StopEmulator(int processId)
        {
            // If the process is still running
            if (Process.GetProcesses().Any(x => x.Id == processId))
            {
                string exePath = XdkToolPath + "\\xbEmulate.exe";
                FileInfo info = new FileInfo(exePath);
                if (!info.Exists)
                {
                    throw new Exception("xbEmulate.exe not found");
                }

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = exePath;
                startInfo.Arguments = string.Format("/nologo /Quit /Process " + processId.ToString());
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                Thread.Sleep(9000);
            }
        }

        /// <summary>
        /// Swaps the byte-order of the specified value.
        /// </summary>
        /// <param name="value">value to swap bytes of</param>
        /// <returns>Value with bytes swapped</returns>
        private static uint EndianSwap(uint value)
        {
            return (uint)(((value & 0x000000ff) << 24)
                | ((value & 0x0000ff00) << 8)
                | ((value & 0x00ff0000) >> 8)
                | ((value & 0xff000000) >> 24));
        }

        /// <summary>
        /// Loads a image from file into a BitmapImage object
        /// </summary>
        /// <param name="fileName">File to load bitmap from</param>
        /// <returns>A BitmapImage object containing the bitmap image from the specified file</returns>
        private static BitmapImage LoadImage(string fileName)
        {
            BitmapImage result = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                result.UriSource = new Uri(fileName, UriKind.Absolute);
                result.EndInit();
            }

            return result;
        }

        /// <summary>
        /// Starts disc image emulation for the specified disc image
        /// </summary>
        /// <param name="discImageFile">Disc image file to load</param>
        /// <param name="progressBar">An optional progress bar to report the progress of the copy operation</param>
        /// <returns>true if successful, false on failure</returns>
        private bool StartEmulatingDiscTitle_Internal(string discImageFile, IProgressBar progressBar = null)
        {
            bool result = false;
            if (this.Connected &&
                this.Responding &&
                (this.discEmulationProcessId == -1) &&
                !string.IsNullOrEmpty(discImageFile))
            {
                // Copy title update first
                if (this.IsHDDEnabled)
                {
                    this.InstallTitleUpdate("HDD", progressBar);
                }
                else if (this.IsMUINTEnabled)
                {
                    this.InstallTitleUpdate("MUINT", progressBar);
                }
                else if (this.IsINTUSBEnabled)
                {
                    this.InstallTitleUpdate("INTUSB", progressBar);
                }
                else if (this.IsMU0Enabled)
                {
                    this.InstallTitleUpdate("MU0", progressBar);
                }
                else if (this.IsMU1Enabled)
                {
                    this.InstallTitleUpdate("MU1", progressBar);
                }
                else if (this.IsUSB0Enabled)
                {
                    this.InstallTitleUpdate("USBMASS0MU", progressBar);
                }
                else if (this.IsUSB1Enabled)
                {
                    this.InstallTitleUpdate("USBMASS1MU", progressBar);
                }

                string exePath = XdkToolPath + "\\xbEmulate.exe";
                FileInfo info = new FileInfo(exePath);
                if (!info.Exists)
                {
                    throw new Exception("xbEmulate.exe not found");
                }

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = exePath;
                startInfo.Arguments = string.Format("/nologo /Minimize /Console " + this.Name + " /Media \"" + discImageFile + "\" /Disc 1 /TimingMode typical /Emulate start");
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardInput = true;
                startInfo.RedirectStandardError = true;
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                Process process = new Process();
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();
                string output = process.StandardOutput.ReadToEnd();
                Match m = Regex.Match(output, @"ProcessId(\s+)=(\s+)([0123456789]+)");
                if (m.Success)
                {
                    string matchString = m.ToString();
                    string[] parts = matchString.Split('=');
                    this.discEmulationProcessId = int.Parse(parts[1]);
                    discEmulationProcessIds.Add(this.discEmulationProcessId);
                    result = true;
                    Thread.Sleep(12000);
                }
            }

            return result;
        }

        /// <summary>
        /// Recursively copies a directory's contents from the desktop PC to the Xbox
        /// </summary>
        /// <param name="desktopDir">Desktop directory to copy</param>
        /// <param name="xboxDir">Xbox directory to copy to</param>
        /// <param name="progressBar">An optional progress bar to report progress of the copy operation</param>
        /// <returns>true if successful, false on failure</returns>
        private bool RecursiveDirectoryCopy_Internal(string desktopDir, string xboxDir, IProgressBar progressBar = null)
        {
            // Sift through the directory to get the max of all file sizes
            if (progressBar != null)
            {
                uint max = 0;
                uint numFiles = 0;
                List<string> savedXboxDirs = new List<string>();
                List<string> savedDesktopDirs = new List<string>();
                savedXboxDirs.Add(xboxDir);
                savedDesktopDirs.Add(desktopDir);

                while (savedXboxDirs.Count != 0)
                {
                    string desktopDir2 = savedDesktopDirs[0];
                    string xboxDir2 = savedXboxDirs[0];
                    savedDesktopDirs.RemoveAt(0);
                    savedXboxDirs.RemoveAt(0);

                    DirectoryInfo dirInfo;
                    try
                    {
                        dirInfo = new DirectoryInfo(desktopDir2);
                    }
                    catch (Exception)
                    {
                        return false;
                    }

                    FileInfo[] files = dirInfo.GetFiles();
                    foreach (FileInfo fileInfo in files)
                    {
                        numFiles += 1;
                        max += (uint)fileInfo.Length;
                    }

                    DirectoryInfo[] dirs = dirInfo.GetDirectories();
                    foreach (DirectoryInfo dirInfo2 in dirs)
                    {
                        savedDesktopDirs.Add(Path.Combine(desktopDir2, dirInfo2.Name));
                        savedXboxDirs.Add(Path.Combine(xboxDir2, dirInfo2.Name));
                    }
                }

                max += numFiles;
                progressBar.Max = max;
            }

            List<string> savedXboxDirs2 = new List<string>();
            List<string> savedDesktopDirs2 = new List<string>();
            savedXboxDirs2.Add(xboxDir);
            savedDesktopDirs2.Add(desktopDir);
            while (savedXboxDirs2.Count != 0)
            {
                string desktopDir2 = savedDesktopDirs2[0];
                string xboxDir2 = savedXboxDirs2[0];
                savedDesktopDirs2.RemoveAt(0);
                savedXboxDirs2.RemoveAt(0);

                DirectoryInfo dirInfo;
                try
                {
                    dirInfo = new DirectoryInfo(desktopDir2);
                }
                catch (Exception)
                {
                    return false;
                }

                FileInfo[] files = dirInfo.GetFiles();
                foreach (FileInfo fileInfo in files)
                {
                    try
                    {
                        this.SendFile_Internal(fileInfo.FullName, Path.Combine(xboxDir2, fileInfo.Name), progressBar);
                    }
                    catch (COMException ex)
                    {
                        if ((uint)ex.HResult == (uint)XboxDebugManagerNative.HResult.XBDM_DEVICEFULL)
                        {
                            MessageBox.Show("Insufficient storage.", "Device Full");
                        }
                        else
                        {
                            MessageBox.Show("File copy failed.", "Copy Failed");
                        }

                        return false;
                    }

                    if (progressBar != null)
                    {
                        progressBar.Progress += 1;
                    }
                }

                DirectoryInfo[] dirs = dirInfo.GetDirectories();
                foreach (DirectoryInfo dirInfo2 in dirs)
                {
                    savedDesktopDirs2.Add(Path.Combine(desktopDir2, dirInfo2.Name));
                    savedXboxDirs2.Add(Path.Combine(xboxDir2, dirInfo2.Name));
                }
            }

            return true;
        }

        /// <summary>
        /// Recursively deletes the contents of a directory on the Xbox
        /// </summary>
        /// <param name="xboxDir">path to the directory to delete</param>
        /// <param name="progressBar">An optional progress bar to report progress of the delete operation</param>
        /// <returns>true if successful, false on failure</returns>
        private bool RecursiveDirectoryDelete_Internal(string xboxDir, IProgressBar progressBar = null)
        {
            if (!this.DirectoryExists(xboxDir))
            {
                return true;
            }

            bool result = true;
            List<string> savedXboxDirs = new List<string>();
            savedXboxDirs.Add(xboxDir);
            uint numFiles = 0;
            int curIndex = 0;
            while (curIndex < savedXboxDirs.Count)
            {
                string curXboxDir = savedXboxDirs[curIndex];
                curIndex++;

                IXboxFiles xboxFiles;
                try
                {
                    xboxFiles = this.internalXboxConsole.DirectoryFiles(curXboxDir);
                }
                catch (COMException ex)
                {
                    if ((uint)ex.HResult == (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE)
                    {
                        continue;
                    }

                    throw;
                }

                foreach (IXboxFile xboxFile in xboxFiles)
                {
                    numFiles++;
                    if (xboxFile.IsDirectory)
                    {
                        savedXboxDirs.Add(xboxFile.Name);
                    }
                }
            }

            if (progressBar != null)
            {
                progressBar.Max = numFiles;
            }

            curIndex = savedXboxDirs.Count;
            while (curIndex > 0)
            {
                curIndex--;
                string curXboxDir = savedXboxDirs[curIndex];
                IXboxFiles xboxFiles;

                try
                {
                    xboxFiles = this.internalXboxConsole.DirectoryFiles(curXboxDir);
                }
                catch (COMException ex)
                {
                    if ((uint)ex.HResult == (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE)
                    {
                        continue;
                    }

                    throw;
                }

                foreach (IXboxFile xboxFile in xboxFiles)
                {
                    if (!xboxFile.IsDirectory)
                    {
                        string fileName = xboxFile.Name;
                        try
                        {
                            this.internalXboxConsole.DeleteFile(fileName);
                        }
                        catch (Exception)
                        {
                            result = false;
                        }
                    }

                    if (progressBar != null)
                    {
                        progressBar.Progress += 1;
                    }
                }

                try
                {
                    this.internalXboxConsole.RemoveDirectory(curXboxDir);
                }
                catch (Exception)
                {
                    result = false;
                }

                if (progressBar != null)
                {
                    progressBar.Progress += 1;
                }
            }

            return result;
        }

        /// <summary>
        /// Sends the specified file to the xbox
        /// </summary>
        /// <param name="sourcePath">Path to file on PC to send</param>
        /// <param name="destPath">Path to write to on Xbox</param>
        /// <param name="progressBar">An optional progress bar to report progress of the file send operation</param>
        /// <returns>true if successful, false on failure</returns>
        private bool SendFile_Internal(string sourcePath, string destPath, IProgressBar progressBar = null)
        {
            uint progress = 0;
            FileInfo fileInfo = new FileInfo(sourcePath);
            uint max = (uint)fileInfo.Length;
            if ((progressBar != null) && (progressBar.Max == uint.MaxValue))
            {
                progressBar.Max = max;
            }

            try
            {
                XboxConsole.SetFileSize(destPath, max, XboxCreateDisposition.CreateAlways);
            }
            catch (COMException ex)
            {
                if ((uint)ex.HResult != (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE)   
                {
                    return false;
                }

                string[] xboxDirParts = destPath.Split('\\');
                string pathSoFar = xboxDirParts[0] + @"\";
                for (int i = 1; i < xboxDirParts.Length - 1; i++)
                {
                    pathSoFar = Path.Combine(pathSoFar, xboxDirParts[i]);
                    try
                    {
                        this.internalXboxConsole.MakeDirectory(pathSoFar);
                    }
                    catch (COMException ex2)
                    {
                        if ((uint)ex2.HResult != (uint)XboxDebugManagerNative.HResult.XBDM_ALREADYEXISTS)
                        {
                            if ((uint)ex2.HResult == (uint)XboxDebugManagerNative.HResult.XBDM_DEVICEFULL)
                            {
                                MessageBox.Show("Insufficient storage.", "Device Full");
                            }
                            else
                            {
                                MessageBox.Show("File copy failed.", "Copy Failed");
                            }

                            return false;
                        }
                    }
                }

                XboxConsole.SetFileSize(destPath, max, XboxCreateDisposition.CreateAlways);
            }

            FileStream fs = File.OpenRead(sourcePath);
            using (fs)
            {
                while (progress < max)
                {
                    uint blockSize = 1024 * 1024 * 5; // 1 meg at a time
                    if (progress + blockSize > max)
                    {
                        blockSize = max - progress;
                    }

                    byte[] data = new byte[blockSize];
                    blockSize = (uint)fs.Read(data, 0, (int)blockSize);
                    uint written = 0;
                    bool didWrite = false;
                    for (uint retry = 0; retry < 3; retry++)
                    {
                        try
                        {
                            XboxConsole.WriteFileBytes(destPath, progress, blockSize, data, out written);
                            didWrite = false;
                            break;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    
                    if (!didWrite)
                    {
                        XboxConsole.WriteFileBytes(destPath, progress, blockSize, data, out written);
                    }

                    progress += written;
                    if (written != blockSize)
                    {
                        MessageBox.Show("Insufficient storage space.", "Device Full");
                        return false;
                    }

                    try
                    {
                        if (progressBar != null)
                        {
                            progressBar.Progress += written;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// NotifyPropertyChanged triggers the PropertyChanged event for the specified property
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param>
        private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Reconnects the XboxConsole object.
        /// Useful if the existing object stops talking to us.
        /// </summary>
        private void ReconfigureConsole()
        {
            try
            {
                lock (this.disconnectingDebuggerLock)
                {
                    try
                    {
                        Marshal.ReleaseComObject(this.internalXboxConsole);
                    }
                    catch
                    {
                    }

                    // Open the specified Xbox 360 console.  We may recycle this if events drop out.
                    this.internalXboxConsole = xboxManager.OpenConsole(this.ConnectTo);

                    if (this.debugTarget != null)
                    {
                        try
                        {
                            this.internalXboxConsole.DebugTarget.DisconnectAsDebugger();
                        }
                        catch (COMException)
                        {
                        }

                        this.debugTarget = this.internalXboxConsole.DebugTarget;

                        string debuggerName = "CAT-" + debugGuid.ToString();
                        this.debugTarget.ConnectAsDebugger(debuggerName, XboxDebugConnectFlags.Force);

                        // Reinstall API monitoring on new object
                        this.debugTarget.RemoveAllBreakpoints();
                        this.debugTarget.StopOn(XboxStopOnFlags.OnModuleLoad, false);
                    }

                    this.internalXboxConsole.EventDeferFlags = 0;

                    // Receive notifications.
                    this.internalXboxConsole.OnStdNotify += this.onStdNotify;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Worker thread for Connect.
        /// </summary>
        private void ConnectThread()
        {
            lock (this)
            {
                if (this.connecting)
                {
                    try
                    {
                        // Open the specified Xbox 360 console.  We may recycle this if events drop out.
                        this.internalXboxConsole = xboxManager.OpenConsole(this.ConnectTo);

                        // Open an additional console reference, just for module use, which we never recycle.
                        this.xboxConsole = xboxManager.OpenConsole(this.ConnectTo);

                        // Clean up old registration as debugger?
                        try
                        {
                            this.internalXboxConsole.DebugTarget.DisconnectAsDebugger();
                        }
                        catch (COMException)
                        {
                        }

                        // Call this before any native APIs that are contextual to the currently set Xbox
                        this.internalXboxConsole.EventDeferFlags = 0;

                        // Receive notifications.
                        this.internalXboxConsole.OnStdNotify += this.onStdNotify;
                    }
                    catch (COMException)
                    {
                        this.connecting = false;
                        if (this.connectCompleteDelegate != null)
                        {
                            this.connectCompleteDelegate(this, false);
                        }
                    }
                }
            }

            ShutdownSynchronization.AllowShutdown();
        }

        /// <summary>
        /// Worker thread for Disconnect.
        /// A sleep is used to avoid a race condition in which the API monitoring has been disabled,
        /// but there are still some events pending.  The sleep will ensure those events are complete before disconnecting.
        /// </summary>
        private void DisconnectThread()
        {
            XboxConsole console = null;
            XboxConsole console2 = null;

            // In case subscribed to breakpoints or debug output, give a couple seconds
            // for any pending notifications to clear before stomping on the objects
            Thread.Sleep(1000);
            lock (this)
            {
                this.connecting = false;
                this.connected = false;
                this._DisconnectDebugger();

                console = this.internalXboxConsole;
                console2 = this.xboxConsole;

                this.internalXboxConsole = null;
                this.xboxConsole = null;
            }

            if (console != null)
            {
                try
                {
                    // Stop receiving notifications.
                    console.OnStdNotify -= this.onStdNotify;

                    Marshal.ReleaseComObject(console);
                    Marshal.ReleaseComObject(console2);
                }
                catch (COMException)
                {
                }
            }

            ShutdownSynchronization.AllowShutdown();
        }

        /// <summary>
        /// _DisconnectDebugger() is used by both DisconnectThread() and DisconnectDebuggerThread(),
        /// to do the common debugger disconnect logic.
        /// </summary>
        private void _DisconnectDebugger()
        {
            lock (this.disconnectingDebuggerLock)
            {
                if (this.disconnectingDebugger)
                {
                    try
                    {
                        this.debugTarget.StopOn(XboxStopOnFlags.OnModuleLoad, false);
                        this.debugTarget.DisconnectAsDebugger();
                    }
                    catch (COMException)
                    {
                        // In case the user changes the title in the middle, debugTarget calls will throw exceptions
                    }

                    Marshal.ReleaseComObject(this.debugTarget);
                    this.debugTarget = null;
                    this.disconnectingDebugger = false;
                }
            }
        }

        /// <summary>
        /// Worker thread for Disconnect.
        /// </summary>
        private void DisconnectDebuggerThread()
        {
            // Because there might be pending breakpoints queued up, we can't remove the debugger
            // immediately.  Delay it for a couple of seconds.
            Thread.Sleep(1000);
            lock (this)
            {
                try
                {
                    this._DisconnectDebugger();
                }
                catch (COMException)
                {
                }
            }

            ShutdownSynchronization.AllowShutdown();
        }

        /// <summary>
        /// Handle the ExecStateChange (DM_EXEC) notification.
        /// </summary>
        /// <param name="eventInfo">Notification event</param>
        private void OnExecStateChange(IXboxEventInfo eventInfo)
        {
            switch (eventInfo.Info.ExecState)
            {
                case XboxExecutionState.Pending:
                    break;

                case XboxExecutionState.PendingTitle:
                    break;

                case XboxExecutionState.Rebooting:
                    break;

                case XboxExecutionState.RebootingTitle:
                    break;

                case XboxExecutionState.Running:
                    break;

                case XboxExecutionState.Stopped:
                    break;
            }
        }

        /// <summary>
        /// Handler for debug notifications from the Xbox 360 console.
        /// </summary>
        /// <param name="eventCode">XboxDebugEventType for this Xbox event</param>
        /// <param name="eventInfo">Info about this event</param>
        private void XboxConsole_OnStdNotify(XboxDebugEventType eventCode, IXboxEventInfo eventInfo)
        {
            lock (this)
            {
                if (this.connecting == true)
                {
                    this.connecting = false;
                    this.connected = true;
                    if (this.connectCompleteDelegate != null)
                    {
                        this.connectCompleteDelegate(this, true);
                    }
                }
            }

            this.Responding = true;

            bool isStopped = eventInfo.Info.IsThreadStopped != 0;

            switch (eventCode)
            {
                case XboxDebugEventType.ExecStateChange:    // DM_EXEC
                    {
                        this.OnExecStateChange(eventInfo);
                    }

                    break;

                case XboxDebugEventType.DebugString:        // DM_DEBUGSTR
                    {
                        // Log the message.
                        string message = eventInfo.Info.Message.TrimEnd(" \t\n".ToCharArray());
                        if (!string.IsNullOrEmpty(message))
                        {
                            if (this.debugMonitorDelegate != null)
                            {
                                this.debugMonitorDelegate(message);
                            }
                        }

                        this.ContinueExecution(eventInfo);
                    }

                    break;

                case XboxDebugEventType.AssertionFailed:    // DM_ASSERT
                    {
                        // Only handle if we're the connected debugger
                        if (this.debugTarget != null)
                        {
                            if (this.onTitleFailureDelegate != null)
                            {
                                this.onTitleFailureDelegate("Assert", eventInfo);
                            }
                            else
                            {
                                this.ContinueExecution(eventInfo);
                            }
                        }
                    }

                    break;

                case XboxDebugEventType.Exception:          // DM_EXCEPTION
                    {
                        // Only handle if we're the connected debugger
                        if (this.debugTarget != null)
                        {
                            // Only handle first-chance exceptions.
                            if (eventInfo.Info.Flags != XboxExceptionFlags.FirstChance)
                            {
                                break;
                            }

                            // Ignore exception caused by SetThreadName.
                            if (eventInfo.Info.Code == 0x406D1388)
                            {
                                break;
                            }

                            if (this.onTitleFailureDelegate != null)
                            {
                                this.onTitleFailureDelegate("Exception", eventInfo);
                            }
                            else
                            {
                                this.ContinueExecution(eventInfo);
                            }
                        }
                    }

                    break;

                case XboxDebugEventType.RIP:                // DM_RIP
                    {
                        // Only handle if we're the connected debugger
                        if (this.debugTarget != null)
                        {
                            if (this.onTitleFailureDelegate != null)
                            {
                                this.onTitleFailureDelegate("RIP", eventInfo);
                            }
                            else
                            {
                                this.ContinueExecution(eventInfo);
                            }
                        }
                    }

                    break;
                case XboxDebugEventType.ModuleLoad:
                    {
                        // Only handle if we're the connected debugger
                        if (this.debugTarget != null)
                        {
                            uint baseAddr = eventInfo.Info.Module.ModuleInfo.BaseAddress;
                            lock (this.monitorAPILock)
                            {
                                foreach (KeyValuePair<string, MonitoredAPISymbol> p in this.monitorSymbols)
                                {
                                    string symbolName = p.Key;
                                    MonitoredAPISymbol monitoredAPISymbol = p.Value;
                                    uint funcAddr = this.LookupSymbolInModule(symbolName, baseAddr);
                                    if (funcAddr != 0)
                                    {
                                        if (!this.moduleBaseAddrToBreakpointMap.ContainsKey(baseAddr))
                                        {
                                            this.moduleBaseAddrToBreakpointMap.Add(baseAddr, new List<uint>());
                                        }

                                        if (!this.moduleBaseAddrToBreakpointMap[baseAddr].Contains(funcAddr))
                                        {
                                            this.moduleBaseAddrToBreakpointMap[baseAddr].Add(funcAddr);
                                            this.breakpointToMonitoredAPISymbol.Add(funcAddr, monitoredAPISymbol);
                                            this.monitorSymbols[symbolName].SymbolLoadCount++;
                                            try
                                            {
                                                if (this.ConnectDebugger())
                                                {
                                                    this.debugTarget.SetBreakpoint(funcAddr);
                                                }
                                            }
                                            catch (COMException)
                                            {
                                            }
                                        }
                                    }
                                }
                            }

                            this.ContinueExecution(eventInfo);
                        }
                    }

                    break;

                case XboxDebugEventType.ModuleUnload:
                    {
                        // Only handle if we're the connected debugger
                        if (this.debugTarget != null)
                        {
                            string moduleName = eventInfo.Info.Module.ModuleInfo.Name;
                            uint baseAddr = eventInfo.Info.Module.ModuleInfo.BaseAddress;

                            lock (this.monitorAPILock)
                            {
                                if (this.moduleBaseAddrToBreakpointMap.ContainsKey(baseAddr))
                                {
                                    foreach (uint funcAddr in this.moduleBaseAddrToBreakpointMap[baseAddr])
                                    {
                                        MonitoredAPISymbol monitoredAPISymbol;
                                        if (this.breakpointToMonitoredAPISymbol.TryGetValue(funcAddr, out monitoredAPISymbol))
                                        {
                                            monitoredAPISymbol.SymbolLoadCount--;
                                            this.breakpointToMonitoredAPISymbol.Remove(funcAddr);
                                        }
                                    }

                                    this.moduleBaseAddrToBreakpointMap.Remove(baseAddr);
                                }
                            }

                            this.ContinueExecution(eventInfo);
                        }
                    }

                    break;
                case XboxDebugEventType.ExecutionBreak:     // DM_BREAK
                    {
                        // Only handle if we're the connected debugger
                        if (this.debugTarget != null)
                        {
                            if (this.breakpointToMonitoredAPISymbol.Count == 0)
                            {
                                break;
                            }

                            uint funcAddr = 0;
                            try
                            {
                                IXboxStackFrame stackFrame = eventInfo.Info.Thread.TopOfStack;
                                funcAddr = stackFrame.FunctionInfo.BeginAddress;
                                this.ContinueExecution(eventInfo);  // Only continue if we were the ones with the debug connection
                            }
                            catch (Exception)
                            {
                                // Ignore failures to get stack from Xbox being debugged by someone else
                            }

                            if (funcAddr != 0)
                            {
                                lock (this.monitorAPILock)
                                {
                                    MonitoredAPISymbol monitoredAPISymbol;
                                    if (this.breakpointToMonitoredAPISymbol.TryGetValue(funcAddr, out monitoredAPISymbol))
                                    {
                                        monitoredAPISymbol.Notify();
                                    }
                                }
                            }
                        }
                    }

                    break;

                default:
                    break;
            }

            // Clean up COM objects.  If stopped, these will be disposed by ContinueExecution()
            if (!isStopped)
            {
                XBOX_EVENT_INFO info = eventInfo.Info;

                Marshal.ReleaseComObject(eventInfo);

                if (info.Module != null)
                {
                    Marshal.ReleaseComObject(info.Module);
                }

                if (info.Section != null)
                {
                    Marshal.ReleaseComObject(info.Section);
                }

                if (info.Thread != null)
                {
                    Marshal.ReleaseComObject(info.Thread);
                }
            }
        }

        /// <summary>
        /// Installs a title update
        /// </summary>
        /// <param name="drive">Drive to install title update to</param>
        /// <param name="progressBar">An optional progress bar to use to report the progress of the installation</param>
        private void InstallTitleUpdate(string drive = "", IProgressBar progressBar = null)
        {
            string driveName;
            if (string.IsNullOrEmpty(drive))
            {
                driveName = this.PrimaryDrive;
            }
            else
            {
                driveName = drive;
            }

            if (this.Connected && this.Responding && !string.IsNullOrEmpty(this.xboxTitle.TitleUpdatePath))
            {
                string titleFilePath = "$TitleUpdate\\" + this.TitleId + "\\";
                string titleFileName = Path.GetFileName(this.xboxTitle.TitleUpdatePath);

                if ((progressBar != null) && (progressBar.Max == uint.MaxValue))
                {
                    FileInfo fileInfo;
                    fileInfo = new FileInfo(this.xboxTitle.TitleUpdatePath);
                    progressBar.Max = (uint)fileInfo.Length;
                }

                string toPath = driveName + ":\\" + titleFilePath;
                try
                {
                    this.SendFile_Internal(this.xboxTitle.TitleUpdatePath, Path.Combine(toPath, titleFileName), progressBar);
                }
                catch (COMException ex)
                {
                    if ((uint)ex.HResult == (uint)XboxDebugManagerNative.HResult.XBDM_DEVICEFULL)
                    {
                        MessageBox.Show("Insufficient storage space to install the title update.", "Device Full");
                    }
                    else
                    {
                        MessageBox.Show("Title Update installation failed.", "Install Failed");
                    }
                }
            }
        }

        /// <summary>
        /// Installs the currently configured content-package based title on the specified drive
        /// </summary>
        /// <param name="drive">Drive to install the content-package based title on</param>
        /// <param name="progressBar">An optional progress bar to report progress of the installation</param>
        /// <returns>true if installation was successful, false on failure</returns>
        private bool InstallContentPackageBasedTitle(string drive = "", IProgressBar progressBar = null)
        {
            string driveName;
            if (string.IsNullOrEmpty(drive))
            {
                driveName = this.PrimaryDrive;
            }
            else
            {
                driveName = drive;
            }

            bool result = false;
            string titleFilePath = this.ContentPackageBasedTitleFilePath;
            string titleFileName = this.ContentPackageBasedTitleFileName;
            string demoFileName = this.DemoContentPackageBasedTitleFileName;
            if (this.Connected && this.Responding && !string.IsNullOrEmpty(titleFilePath))
            {
                FileInfo fileInfo;
                uint titleUpdateSize = 0;
                if (!string.IsNullOrEmpty(this.xboxTitle.TitleUpdatePath))
                {
                    fileInfo = new FileInfo(this.xboxTitle.TitleUpdatePath);
                    titleUpdateSize = (uint)fileInfo.Length;
                }

                uint packageSize = 0;
                if (this.xboxTitle.UseDemo && !string.IsNullOrEmpty(this.xboxTitle.DemoContentPackage))
                {
                    fileInfo = new FileInfo(this.xboxTitle.DemoContentPackage);
                    packageSize = (uint)fileInfo.Length;
                }

                if (!this.xboxTitle.UseDemo && !string.IsNullOrEmpty(this.xboxTitle.ContentPackage))
                {
                    fileInfo = new FileInfo(this.xboxTitle.ContentPackage);
                    packageSize = (uint)fileInfo.Length;
                }

                if ((progressBar != null) && (progressBar.Max == uint.MaxValue))
                {
                    progressBar.Max = titleUpdateSize + packageSize;
                }

                // copy title update first (NOTE we may not need this if demo)
                this.InstallTitleUpdate(driveName, progressBar);
                string toPath = driveName + ":\\" + titleFilePath;
                
                // copy the package to the xbox
                try
                {
                    // install demo
                    if (this.xboxTitle.UseDemo && !string.IsNullOrEmpty(this.xboxTitle.DemoContentPackage))
                    {
                        if (!string.IsNullOrEmpty(this.xboxTitle.DemoContentPackage))
                        {
                            this.SendFile_Internal(this.xboxTitle.DemoContentPackage, Path.Combine(toPath, demoFileName), progressBar);
                            result = true;
                        }
                    }

                    if (!this.xboxTitle.UseDemo && !string.IsNullOrEmpty(this.xboxTitle.ContentPackage))
                    {
                        // or full title
                        this.SendFile_Internal(this.xboxTitle.ContentPackage, Path.Combine(toPath, titleFileName), progressBar);
                        result = true;
                    }
                }
                catch (COMException ex)
                {
                    if ((uint)ex.HResult == (uint)XboxDebugManagerNative.HResult.XBDM_DEVICEFULL)
                    {
                        MessageBox.Show("Insufficient storage space to install the title.", "Device Full");
                    }
                    else
                    {
                        MessageBox.Show("Title installation failed.", "Install Failed");
                    }

                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Uninstalls a title update
        /// </summary>
        /// <param name="drive">Drive to uninstall the title update from</param>
        /// <returns>true if successful, false on failure</returns>
        private bool UninstallTitleUpdate(string drive)
        {
            string titleFilePath = drive + @":\$TitleUpdate\" + this.TitleId;
            return this.RecursiveDirectoryDelete(titleFilePath);
        }

        /// <summary>
        /// Installs the currently configured Raw title
        /// </summary>
        /// <param name="progressBar">An optional progress bar reporting the progress of the installation</param>
        /// <returns>true if successful, false on failure</returns>
        private bool InstallRawTitle(IProgressBar progressBar = null)
        {
            bool result = false;
            if (this.Connected &&
                this.Responding && 
                !this.IsRawTitleInstalled && 
                !string.IsNullOrEmpty(this.xboxTitle.RawGameDirectory))
            {
                result = this.RecursiveDirectoryCopy(this.xboxTitle.RawGameDirectory, @"DEVKIT:\CATRawTitle", progressBar);
                if (!result)
                {
                    // We failed, remove what we added
                    result = this.RecursiveDirectoryDelete(@"DEVKIT:\CATRawTitle");
                }
            }

            return result;
        }

        /// <summary>
        /// Switches video to PAL mode (50 or 60).
        /// </summary>
        /// <param name="pal60">If true, PAL60 is used, otherwise PAL50 is used</param>
        /// <param name="rebootNow">Whether or not to launch the developer dashboard now</param>
        private void SwitchToPAL(bool pal60, bool rebootNow)
        {
            // None of the video modes are supposed by PAL
            string videoMode = " /V PAL-I ";
            if (pal60)
            {
                videoMode += "/pal60 ";
            }

            this.Run_xbsetcfg(string.Format("/X {0} {1}", this.IP, videoMode));

            // note you must reboot the development kit for the change to take affect
            if (rebootNow)
            {
                this.LaunchDevDashboard();
            }
        }

        /// <summary>
        /// Sets a NonVGA video mode
        /// </summary>
        /// <param name="res">Resolution to set video to</param>
        /// <param name="standard">Format to set the video to</param>
        /// <param name="rebootNow">Whether or not to launch the developer dashboard now</param>
        private void SetNonVGAVideoMode(VideoResolution res, VideoStandard standard, bool rebootNow)
        {
            bool wide = false;
            string videoMode = string.Empty;
            videoMode = "/V ";
            if (standard == VideoStandard.NTSCJ)
            {
                videoMode += " NTSC-J ";
            }
            else if (standard == VideoStandard.NTSCM)
            {
                videoMode += " NTSC-M ";
            }
            else
            {
                throw new Exception("Unknown video format");
            }

            switch (res)
            {
                case VideoResolution.Mode480:
                    videoMode += " /480p "; // this could be another argument
                    break;

                case VideoResolution.Mode480Wide:
                    videoMode += " /480p "; // this could be another argument
                    wide = true;
                    break;

                case VideoResolution.Mode720p:
                    videoMode += " /720p ";
                    break;

                case VideoResolution.Mode1080i:
                    videoMode += " /1080i ";
                    break;

                case VideoResolution.Mode1080p:
                    videoMode += " /1080p ";
                    break;

                default:
                    throw new Exception("Unknown video resolution");
            }

            if (wide)
            {
                videoMode += " /WIDESCREEN ";
            }

            this.Run_xbsetcfg(string.Format("/X {0} {1}", this.IP, videoMode));

            // note you must reboot the development kit for the change to take affect
            if (rebootNow)
            {
                this.LaunchDevDashboard();
            }
        }

        /// <summary>
        /// Sets a VGA video mode
        /// </summary>
        /// <param name="res">Mode to set video to</param>
        /// <param name="rebootNow">Whether or not to launch the developer dashboard now</param>
        private void SetVGAVideoMode(VideoResolution res, bool rebootNow)
        {
            bool wide = false;
            string videoMode = string.Empty;
            videoMode = " /V VGA ";
            switch (res)
            {
                case VideoResolution.Mode640x480:
                    videoMode += "640x480";
                    break;
                case VideoResolution.Mode640x480Wide:
                    videoMode += "640x480";
                    wide = true;
                    break;
                case VideoResolution.Mode848x480:
                    {
                        videoMode += "848x480";
                        wide = true;
                    }

                    break;
                case VideoResolution.Mode1024x768:
                    videoMode += "1024x768";
                    break;
                case VideoResolution.Mode1024x768Wide:
                    videoMode += "1024x768";
                    wide = true;
                    break;
                case VideoResolution.Mode1280x720:
                    {
                        videoMode += "1280x720";
                        wide = true;
                    }

                    break;
                case VideoResolution.Mode1280x1024:
                    videoMode += "1280x1024";
                    break;
                case VideoResolution.Mode1280x1024Wide:
                    videoMode += "1280x1024";
                    wide = true;
                    break;
                case VideoResolution.Mode1360x768:
                    {
                        videoMode += "1360x768";
                        wide = true;
                    }

                    break;
                case VideoResolution.Mode1440x900:
                    {
                        videoMode += "1440x900";
                        wide = true;
                    }
                    
                    break;
                case VideoResolution.Mode1680x1050:
                    {
                        videoMode += "1680x1050";
                        wide = true;
                    }

                    break;
                case VideoResolution.Mode1920x1080:
                    {
                        videoMode += "1920x1080";
                        wide = true;
                    }
                    
                    break;
            }

            if (wide)
            {
                videoMode += " /WIDESCREEN ";
            }

            this.Run_xbsetcfg(string.Format("/X {0} {1}", this.IP, videoMode));

            // note you must reboot the development kit for the change to take affect
            if (rebootNow)
            {
                this.LaunchDevDashboard();
            }
        }

        /// <summary>
        /// Helper function to launch XBSETCFG
        /// </summary>
        /// <param name="args">args to pass to XBSETCFG</param>
        private void Run_xbsetcfg(string args)
        {
            string defaultXbox = string.Empty;
            try
            {
                // Every time we call xbsetcfg, it will change the default xbox out from under us.  We need to save it off and restore it afterwards.
                defaultXbox = xboxManager.DefaultConsole;
            }
            catch (Exception)
            {
                // there was no default xbox
            }

            string exePath = XdkToolPath + "\\xbsetcfg.exe";
            FileInfo info = new FileInfo(exePath);
            if (!info.Exists)
            {
                throw new Exception("xbsetcfg.exe not found");
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = exePath;
            startInfo.Arguments = args;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();

            // must wait for command to finish so Xbox has time to take setting change
            process.WaitForExit();

            if (!string.IsNullOrEmpty(defaultXbox))
            {
                xboxManager.DefaultConsole = defaultXbox;
            }
        }

        /// <summary>
        /// Removes the monitoring relationship for the specified MonitoredAPISymbol
        /// </summary>
        /// <param name="monitoredAPISymbol">MonitoredAPISymbol to stop monitoring</param>
        private void StopMonitoringAPI(MonitoredAPISymbol monitoredAPISymbol)
        {
            lock (this.monitorAPILock)
            {
                this.monitorSymbols.Remove(monitoredAPISymbol.SymbolName);
                List<uint> moduleBaseAddrToRemove = new List<uint>();
                foreach (KeyValuePair<uint, List<uint>> pair in this.moduleBaseAddrToBreakpointMap)
                {
                    List<uint> keysToRemove = new List<uint>();
                    foreach (uint funcAddr in pair.Value)
                    {
                        MonitoredAPISymbol monitoredAPISymbol2;
                        if (this.breakpointToMonitoredAPISymbol.TryGetValue(funcAddr, out monitoredAPISymbol2))
                        {
                            if (monitoredAPISymbol == monitoredAPISymbol2)
                            {
                                keysToRemove.Add(funcAddr);
                            }
                        }
                    }

                    foreach (uint funcAddr in keysToRemove)
                    {
                        pair.Value.Remove(funcAddr);
                    }

                    if (pair.Value.Count == 0)
                    {
                        moduleBaseAddrToRemove.Add(pair.Key);
                    }
                }

                foreach (uint moduleBaseAddr in moduleBaseAddrToRemove)
                {
                    this.moduleBaseAddrToBreakpointMap.Remove(moduleBaseAddr);
                }

                List<uint> breakpointsToRemove = new List<uint>();
                foreach (KeyValuePair<uint, MonitoredAPISymbol> pair in this.breakpointToMonitoredAPISymbol)
                {
                    if (pair.Value == monitoredAPISymbol)
                    {
                        breakpointsToRemove.Add(pair.Key);
                        try
                        {
                            this.debugTarget.RemoveBreakpoint(pair.Key);
                        }
                        catch
                        {
                        }
                    }
                }

                foreach (uint funcAddr in breakpointsToRemove)
                {
                    this.breakpointToMonitoredAPISymbol.Remove(funcAddr);
                }
            }
        }

        /// <summary>
        /// A class representing a monitored symbol
        /// </summary>
        public class MonitoredAPISymbol : INotifyPropertyChanged
        {
            /// <summary>
            /// Number of breakpoints loaded that are associated with this MonitoredAPISymbol
            /// </summary>
            private uint symbolLoadCount;

            /// <summary>
            /// The XboxDevice associated with this MonitoredAPISymbol
            /// </summary>
            private XboxDevice xboxDevice;

            /// <summary>
            /// All MonitorAPISessions associated with this MonitoredAPISymbol
            /// </summary>
            private HashSet<MonitorAPISession> sessions = new HashSet<MonitorAPISession>();

            /// <summary>
            /// Initializes a new instance of the <see cref="MonitoredAPISymbol" /> class.
            /// </summary>
            /// <param name="xboxDevice">XboxDevice associated with this MonitoredAPISymbol</param>
            /// <param name="symbolName">Symbol name to monitor</param>
            public MonitoredAPISymbol(XboxDevice xboxDevice, string symbolName)
            {
                this.xboxDevice = xboxDevice;
                this.SymbolName = symbolName;
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets the name of the symbol to monitor
            /// </summary>
            public string SymbolName { get; set; }

            /// <summary>
            /// Gets a value indicating whether the symbol was found in any currently loaded library
            /// </summary>
            public bool IsSymbolFound
            {
                get
                {
                    return this.symbolLoadCount > 0;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether the monitored function has been called
            /// </summary>
            public uint SymbolLoadCount
            {
                get
                {
                    return this.symbolLoadCount;
                }

                set
                {
                    bool anyLoadedChanged = ((this.symbolLoadCount == 0) && (value > 0)) || ((this.symbolLoadCount > 0) && (value == 0));
                    this.symbolLoadCount = value;
                    this.NotifyPropertyChanged();
                    if (anyLoadedChanged)
                    {
                        this.NotifyPropertyChanged("IsSymbolFound");
                        foreach (MonitorAPISession session in this.sessions)
                        {
                            session.NotifyPropertyChanged("IsSymbolFound");
                        }
                    }
                }
            }

            /// <summary>
            /// Adds a session to this MonitoredAPISymbol.
            /// All calls to RemoveSession() and AddSession() must be in the main/UI thread to avoid race conditions
            /// </summary>
            /// <param name="monitorAPISession">MonitorAPISession to add</param>
            public void AddSession(MonitorAPISession monitorAPISession)
            {
                this.sessions.Add(monitorAPISession);
            }

            /// <summary>
            /// Removes a session from this MonitoredAPISymbol.
            /// All calls to RemoveSession() and AddSession() must be in the main/UI thread to avoid race conditions
            /// </summary>
            /// <param name="monitorAPISession">MonitorAPISession to remove</param>
            public void RemoveSession(MonitorAPISession monitorAPISession)
            {
                this.sessions.Remove(monitorAPISession);
                if (this.sessions.Count == 0)
                {
                    // Remove API monitoring for this function
                    this.xboxDevice.StopMonitoringAPI(this);
                }
            }

            /// <summary>
            /// Invokes the notification delegate associated with this symbol
            /// </summary>
            public void Notify()
            {
                foreach (MonitorAPISession session in this.sessions)
                {
                    session.Notify();
                }
            }

            /// <summary>
            /// NotifyPropertyChanged triggers the PropertyChanged event for the specified property
            /// </summary>
            /// <param name="propertyName">Name of property that has changed</param>
            private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// A class representing a caller requested symbol monitoring sessions.  Multiple may exist for the same monitored symbol name.
        /// </summary>
        public class MonitorAPISession : INotifyPropertyChanged, IMonitorAPISession
        {
            /// <summary>
            /// Backing field for WasCalled property
            /// </summary>
            private bool wasCalled;

            /// <summary>
            /// Initializes a new instance of the <see cref="MonitorAPISession" /> class.
            /// </summary>
            /// <param name="monitoredAPISymbol">MonitoredAPISymbol associated with this MonitorAPISession</param>
            /// <param name="monitorAPIDelegate">Delegate to call when the monitored function is called</param>
            public MonitorAPISession(MonitoredAPISymbol monitoredAPISymbol, MonitorAPIDelegate monitorAPIDelegate)
            {
                this.MonitoredAPISymbol = monitoredAPISymbol;
                this.MonitorAPIDelegate = monitorAPIDelegate;
                monitoredAPISymbol.AddSession(this);
            }
            
            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets the MonitoredAPISymbol associated with this MonitorAPISession
            /// </summary>
            public MonitoredAPISymbol MonitoredAPISymbol { get; set; }

            /// <summary>
            /// Gets a value indicating whether the symbol has been found
            /// </summary>
            public bool IsSymbolFound
            {
                get { return MonitoredAPISymbol.IsSymbolFound; }
            } 

            /// <summary>
            /// Gets the symbol name associated with this MonitorAPISession
            /// </summary>
            public string SymbolName 
            {
                get { return MonitoredAPISymbol.SymbolName; } 
            }

            /// <summary>
            /// Gets or sets the delegate to call when the monitored function is called
            /// </summary>
            public MonitorAPIDelegate MonitorAPIDelegate { get; set; }

             /// <summary>
            /// Gets or sets a value indicating whether the monitored function has been called
            /// </summary>
            public bool WasCalled
            {
                get
                {
                    return this.wasCalled;
                }

                set
                {
                    this.wasCalled = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Invokes the notification delegate associated with this symbol
            /// </summary>
            public void Notify()
            {
                try
                {
                    this.WasCalled = true;
                    this.MonitorAPIDelegate(this);
                }
                catch
                {
                    // Ignore exceptions in user callback
                }
            }

            /// <summary>
            /// Disposes of this symbol monitoring relationship
            /// Must be called in main/UI thread to avoid race conditions.
            /// </summary>
            public void Dispose()
            {
                this.MonitoredAPISymbol.RemoveSession(this);
            }

            /// <summary>
            /// NotifyPropertyChanged triggers the PropertyChanged event for the specified property
            /// </summary>
            /// <param name="propertyName">Name of property that has changed</param>
            public void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}