// -----------------------------------------------------------------------
// <copyright file="XboxViewItem.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    /// <summary>
    /// XboxViewItem is a ViewModel class representing an Xbox.
    /// It's used to bind Xbox-specific properties in the view/XAML
    /// </summary>
    public class XboxViewItem : INotifyPropertyChanged
    {
        /// <summary>
        /// A reference to the main MainViewModel class
        /// </summary>
        private readonly MainViewModel mainViewModel;

        /// <summary>
        /// Backing field for AuxPanelVisibility property
        /// </summary>
        private Visibility auxPanelVisibility = Visibility.Collapsed;
        
        /// <summary>
        /// reportedConnected is set to true only after the Xbox can been connected (at least once).
        /// </summary>
        private bool reportedConnected;

        /// <summary>
        /// Backing field for DebugOutputViewModel property
        /// </summary>
        private DebugOutputViewModel debugOutputViewModel;

        /// <summary>
        /// Backing field for ProfileManagerViewModel property
        /// </summary>
        private ProfileManagerViewModel profileManagerViewModel;

        /// <summary>
        /// Timer used to refresh the display for this Xbox
        /// </summary>
        private Timer refreshTimer;

        /// <summary>
        /// Timer used to reconnect the Xbox
        /// </summary>
        private Timer reconnectTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxViewItem" /> class.
        /// </summary>
        /// <param name="device">XboxDevice to associate this ViewItem with</param>
        /// <param name="mainViewModel">A reference to the MainViewModel</param>
        public XboxViewItem(XboxDevice device, MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.XboxDevice = device;
            this.Connect();

            this.RefreshState();

            this.InstallContentPackageBasedTitleHDDCommand = new Command((o) => this.InstallContentPackageBasedTitle("HDD"));
            this.InstallContentPackageBasedTitleMU0Command = new Command((o) => this.InstallContentPackageBasedTitle("MU0"));
            this.InstallContentPackageBasedTitleMU1Command = new Command((o) => this.InstallContentPackageBasedTitle("MU1"));
            this.InstallContentPackageBasedTitleMUINTCommand = new Command((o) => this.InstallContentPackageBasedTitle("MUINT"));
            this.InstallContentPackageBasedTitleINTUSBCommand = new Command((o) => this.InstallContentPackageBasedTitle("INTUSB"));
            this.InstallContentPackageBasedTitleUSB0Command = new Command((o) => this.InstallContentPackageBasedTitle("USBMASS0MU"));
            this.InstallContentPackageBasedTitleUSB1Command = new Command((o) => this.InstallContentPackageBasedTitle("USBMASS1MU"));

            this.UninstallContentPackageBasedTitleHDDCommand = new Command((o) => this.UninstallContentPackageBasedTitle("HDD"));
            this.UninstallContentPackageBasedTitleMU0Command = new Command((o) => this.UninstallContentPackageBasedTitle("MU0"));
            this.UninstallContentPackageBasedTitleMU1Command = new Command((o) => this.UninstallContentPackageBasedTitle("MU1"));
            this.UninstallContentPackageBasedTitleMUINTCommand = new Command((o) => this.UninstallContentPackageBasedTitle("MUINT"));
            this.UninstallContentPackageBasedTitleINTUSBCommand = new Command((o) => this.UninstallContentPackageBasedTitle("INTUSB"));
            this.UninstallContentPackageBasedTitleUSB0Command = new Command((o) => this.UninstallContentPackageBasedTitle("USBMASS0MU"));
            this.UninstallContentPackageBasedTitleUSB1Command = new Command((o) => this.UninstallContentPackageBasedTitle("USBMASS1MU"));

            this.ReconnectDeviceCommand = new Command((o) => this.Reconnect());
            this.SaveScreenShotCommand = new Command((o) => this.SaveScreenShot());
            this.DisplayScreenShotCommand = new Command((o) => this.DisplayScreenShot());
            this.ColdRebootDeviceCommand = new Command((o) => this.ColdReboot());
            this.LaunchDevDashboardCommand = new Command((o) => this.LaunchDevDashboard());

            this.EnableHDDCommand = new Command((o) => this.EnableHDD());
            this.DisableHDDCommand = new Command((o) => this.DisableHDD());

            this.EnableMUINTCommand = new Command((o) => this.EnableMUINT());
            this.DisableMUINTCommand = new Command((o) => this.DisableMUINT());

            this.RemoveDeviceCommand = new Command((o) => this.RemoveDevice());

            this.FlashDeviceWithSEPCommand = new Command((o) => this.FlashDevice(true));
            this.FlashDeviceWithoutSEPCommand = new Command((o) => this.FlashDevice(false));

            this.StartEmulatingDiscImageTitleCommand = new Command((o) => this.StartEmulatingDiscImageTitle());
            this.StopEmulatingDiscImageTitleCommand = new Command((o) => this.StopEmulatingDiscImageTitle());

            this.InstallRawTitleCommand = new Command((o) => this.InstallRawTitle());
            this.UninstallRawTitleCommand = new Command((o) => this.UninstallRawTitle());

            this.LaunchTitleCommand = new Command((o) => this.LaunchTitle(string.Empty));
            this.LaunchContentPackageBasedTitleHDDCommand = new Command((o) => this.LaunchTitle("HDD"));
            this.LaunchContentPackageBasedTitleMU0Command = new Command((o) => this.LaunchTitle("MU0"));
            this.LaunchContentPackageBasedTitleMU1Command = new Command((o) => this.LaunchTitle("MU1"));
            this.LaunchContentPackageBasedTitleMUINTCommand = new Command((o) => this.LaunchTitle("MUINT"));
            this.LaunchContentPackageBasedTitleINTUSBCommand = new Command((o) => this.LaunchTitle("INTUSB"));
            this.LaunchContentPackageBasedTitleUSB0Command = new Command((o) => this.LaunchTitle("USBMASS0MU"));
            this.LaunchContentPackageBasedTitleUSB1Command = new Command((o) => this.LaunchTitle("USBMASS1MU"));

            this.LaunchExplorerCommand = new Command((o) => this.LaunchExplorer());
            this.SetAsDefaultCommand = new Command((o) => this.SetAsDefault());
            this.SynchronizeTimeCommand = new Command((o) => this.SynchronizeTime());

            this.SetTo1GBCommand = new Command((o) => this.SetTo1GB());
            this.SetTo512MBCommand = new Command((o) => this.SetTo512MB());
            this.GetResolutionCommand = new Command((o) => this.GetResolution());
            this.OpenDebugOutputCommand = new Command((o) => this.OpenDebugOutput());
            this.OpenProfileManagerCommand = new Command((o) => this.OpenProfileManager());
            this.OpenVirtualControllerCommand = new Command((o) => this.OpenVirtualController());

            this.SetVideoNTSCM640x480Command = new Command((o) => this.SetResolution(VideoResolution.Mode480, VideoStandard.NTSCM));
            this.SetVideoNTSCM640x480WideCommand = new Command((o) => this.SetResolution(VideoResolution.Mode480Wide, VideoStandard.NTSCM));
            this.SetVideoNTSCM1280x720pCommand = new Command((o) => this.SetResolution(VideoResolution.Mode720p, VideoStandard.NTSCM));
            this.SetVideoNTSCM1920x1080iCommand = new Command((o) => this.SetResolution(VideoResolution.Mode1080i, VideoStandard.NTSCM));
            this.SetVideoNTSCM1920x1080pCommand = new Command((o) => this.SetResolution(VideoResolution.Mode1080p, VideoStandard.NTSCM));

            this.SetVideoNTSCJ640x480Command = new Command((o) => this.SetResolution(VideoResolution.Mode480, VideoStandard.NTSCJ));
            this.SetVideoNTSCJ640x480WideCommand = new Command((o) => this.SetResolution(VideoResolution.Mode480Wide, VideoStandard.NTSCJ));
            this.SetVideoNTSCJ1280x720pCommand = new Command((o) => this.SetResolution(VideoResolution.Mode720p, VideoStandard.NTSCJ));
            this.SetVideoNTSCJ1920x1080iCommand = new Command((o) => this.SetResolution(VideoResolution.Mode1080i, VideoStandard.NTSCJ));
            this.SetVideoNTSCJ1920x1080pCommand = new Command((o) => this.SetResolution(VideoResolution.Mode1080p, VideoStandard.NTSCJ));

            this.SetVideoPAL50640x576Command = new Command((o) => this.SetResolution(VideoResolution.Mode480, VideoStandard.PAL50));
            this.SetVideoPAL50640x576WideCommand = new Command((o) => this.SetResolution(VideoResolution.Mode480Wide, VideoStandard.PAL50));
            this.SetVideoPAL501280x720pCommand = new Command((o) => this.SetResolution(VideoResolution.Mode720p, VideoStandard.PAL50));
            this.SetVideoPAL501920x1080iCommand = new Command((o) => this.SetResolution(VideoResolution.Mode1080i, VideoStandard.PAL50));
            this.SetVideoPAL501920x1080pCommand = new Command((o) => this.SetResolution(VideoResolution.Mode1080p, VideoStandard.PAL50));

            this.SetVideoPAL60640x480Command = new Command((o) => this.SetResolution(VideoResolution.Mode480, VideoStandard.PAL60));
            this.SetVideoPAL60640x480WideCommand = new Command((o) => this.SetResolution(VideoResolution.Mode480Wide, VideoStandard.PAL60));
            this.SetVideoPAL601280x720pCommand = new Command((o) => this.SetResolution(VideoResolution.Mode720p, VideoStandard.PAL60));
            this.SetVideoPAL601920x1080iCommand = new Command((o) => this.SetResolution(VideoResolution.Mode1080i, VideoStandard.PAL60));
            this.SetVideoPAL601920x1080pCommand = new Command((o) => this.SetResolution(VideoResolution.Mode1080p, VideoStandard.PAL60));

            this.SetVideoVGA640x480Command = new Command((o) => this.SetResolution(VideoResolution.Mode640x480, 0));
            this.SetVideoVGA640x480WideCommand = new Command((o) => this.SetResolution(VideoResolution.Mode640x480Wide, 0));
            this.SetVideoVGA848x480Command = new Command((o) => this.SetResolution(VideoResolution.Mode848x480, 0));
            this.SetVideoVGA1024x768Command = new Command((o) => this.SetResolution(VideoResolution.Mode1024x768, 0));
            this.SetVideoVGA1024x768WideCommand = new Command((o) => this.SetResolution(VideoResolution.Mode1024x768Wide, 0));
            this.SetVideoVGA1280x720Command = new Command((o) => this.SetResolution(VideoResolution.Mode1280x720, 0));
            this.SetVideoVGA1280x768Command = new Command((o) => this.SetResolution(VideoResolution.Mode1280x768, 0));
            this.SetVideoVGA1280x1024Command = new Command((o) => this.SetResolution(VideoResolution.Mode1280x1024, 0));
            this.SetVideoVGA1280x1024WideCommand = new Command((o) => this.SetResolution(VideoResolution.Mode1280x1024Wide, 0));
            this.SetVideoVGA1360x768Command = new Command((o) => this.SetResolution(VideoResolution.Mode1360x768, 0));
            this.SetVideoVGA1440x900Command = new Command((o) => this.SetResolution(VideoResolution.Mode1440x900, 0));
            this.SetVideoVGA1680x1050Command = new Command((o) => this.SetResolution(VideoResolution.Mode1680x1050, 0));
            this.SetVideoVGA1680x1050Command = new Command((o) => this.SetResolution(VideoResolution.Mode1920x1080, 0));

            this.SetLanguageEnglishCommand = new Command((o) => this.SetLanguage(Language.English));
            this.SetLanguageJapaneseCommand = new Command((o) => this.SetLanguage(Language.Japanese));
            this.SetLanguageGermanCommand = new Command((o) => this.SetLanguage(Language.German));
            this.SetLanguageFrenchCommand = new Command((o) => this.SetLanguage(Language.French));
            this.SetLanguageSpanishCommand = new Command((o) => this.SetLanguage(Language.Spanish));
            this.SetLanguageItalianCommand = new Command((o) => this.SetLanguage(Language.Italian));
            this.SetLanguageKoreanCommand = new Command((o) => this.SetLanguage(Language.Korean));
            this.SetLanguageTraditionalChineseCommand = new Command((o) => this.SetLanguage(Language.TraditionalChinese));
            this.SetLanguageBrazilianPortugueseCommand = new Command((o) => this.SetLanguage(Language.BrazilianPortuguese));
            this.SetLanguagePolishCommand = new Command((o) => this.SetLanguage(Language.Polish));
            this.SetLanguageRussianCommand = new Command((o) => this.SetLanguage(Language.Russian));
            this.SetLanguageSwedishCommand = new Command((o) => this.SetLanguage(Language.Swedish));
            this.SetLanguageTurkishCommand = new Command((o) => this.SetLanguage(Language.Turkish));
            this.SetLanguageNorwegianCommand = new Command((o) => this.SetLanguage(Language.Norwegian));
            this.SetLanguageDutchCommand = new Command((o) => this.SetLanguage(Language.Dutch));
            this.SetLanguageSimplifiedChineseCommand = new Command((o) => this.SetLanguage(Language.SimplifiedChinese));

            this.DeleteAllProfilesCommand = new Command((o) => this.DeleteAllProfiles());
            this.DeleteAllSavesCommand = new Command((o) => this.DeleteAllSaves());
            this.DeleteAllGamesCommand = new Command((o) => this.DeleteAllGames());

            this.RunScriptCommand = new Command((o) => this.RunScript());
            this.RunScriptItemCommand = new Command((o) => this.RunScriptItem(o as string));
        }

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the current theme
        /// </summary>
        public Theme CurrentTheme
        {
            get { return this.mainViewModel.CurrentTheme; }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether the Xbox is clicked on in the Device Pool
        /// </summary>
        public bool IsClicked { get; set; }

        /// <summary>
        /// Gets or sets the XboxDevice this ViewItem is associated with.
        /// </summary>
        public XboxDevice XboxDevice { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the Xbox is currently offline
        /// </summary>
        public bool IsOffline 
        {
            get { return this.Connected && !this.XboxDevice.Responding; } 
        }

        /// <summary>
        /// Gets or sets a value indicating the visibility of the auxiliary panel this Xbox
        /// </summary>
        public Visibility AuxPanelVisibility
        { 
            get
            { 
                return this.auxPanelVisibility; 
            }

            set
            {
                this.auxPanelVisibility = value;
                if (value == Visibility.Visible)
                {
                    if (this.refreshTimer == null)
                    {
                        this.refreshTimer = new Timer(_ => this.RefreshTimerExpired(), null, TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(-1));
                    }
                }

                this.NotifyPropertyChanged();
            } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be installed
        /// </summary>
        public bool CanInstallContentPackageBasedTitle 
        {
            get 
            { 
                return (this.mainViewModel.CurrentTitle.GameInstallType == "Content Package") && 
                    (!string.IsNullOrEmpty(this.mainViewModel.CurrentTitle.ContentPackage)) &&
                    this.ConnectedAndResponding && 
                    this.XboxDevice.IsAnyStorageDeviceEnabled;
            } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a disc image based title can be installed
        /// </summary>
        public bool CanInstallDiscImageTitle 
        { 
            get
            { 
                return (this.mainViewModel.CurrentTitle.GameInstallType == "Disc Emulation") &&
                    (!string.IsNullOrEmpty(this.mainViewModel.CurrentTitle.ContentPackage)) &&
                    this.ConnectedAndResponding &&
                    this.XboxDevice.IsAnyStorageDeviceEnabled &&
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
                return (this.mainViewModel.CurrentTitle.GameInstallType == "Raw") && 
                    (!string.IsNullOrEmpty(this.mainViewModel.CurrentTitle.RawGameDirectory)) &&
                    this.ConnectedAndResponding &&
                    this.XboxDevice.IsDevKitDriveEnabled &&
                    !this.XboxDevice.IsRawTitleInstalled; 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the HDD is enabled
        /// </summary>
        public bool IsHDDEnabled
        {
            get 
            {
                return this.XboxDevice.IsHDDEnabled;
            }

            set
            {
                if (value)
                {
                    this.EnableHDD();
                }
                else
                {
                    this.DisableHDD();
                }

                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not MU0 is enabled
        /// </summary>
        public bool IsMU0Enabled 
        {
            get { return this.XboxDevice.IsMU0Enabled; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not MU1 is enabled
        /// </summary>
        public bool IsMU1Enabled 
        {
            get { return this.XboxDevice.IsMU1Enabled; } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the internal MU is enabled
        /// </summary>
        public bool IsMUINTEnabled 
        {
            get
            {
                return this.XboxDevice.IsMUINTEnabled; 
            }

            set
            {
                if (value)
                {
                    this.EnableMUINT();
                }
                else
                {
                    this.DisableMUINT();
                }
                
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the internal USB storage device is enabled
        /// </summary>
        public bool IsINTUSBEnabled 
        {
            get { return this.XboxDevice.IsINTUSBEnabled; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not USB0 is enabled
        /// </summary>
        public bool IsUSB0Enabled 
        {
            get { return this.XboxDevice.IsUSB0Enabled; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not USB1 is enabled
        /// </summary>
        public bool IsUSB1Enabled 
        {
            get { return this.XboxDevice.IsUSB1Enabled; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be installed on HDD
        /// </summary>
        public bool CanInstallContentPackageBasedTitleHDD 
        {
            get { return this.CanInstallContentPackageBasedTitleOn("HDD"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be installed on MU0
        /// </summary>
        public bool CanInstallContentPackageBasedTitleMU0 
        {
            get { return this.CanInstallContentPackageBasedTitleOn("MU0"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be installed on MU1
        /// </summary>
        public bool CanInstallContentPackageBasedTitleMU1
        {
            get { return this.CanInstallContentPackageBasedTitleOn("MU1"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be installed on MUINT
        /// </summary>
        public bool CanInstallContentPackageBasedTitleMUINT
        {
            get { return this.CanInstallContentPackageBasedTitleOn("MUINT"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be installed on INTUSB
        /// </summary>
        public bool CanInstallContentPackageBasedTitleINTUSB 
        {
            get { return this.CanInstallContentPackageBasedTitleOn("INTUSB"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be installed on USB0
        /// </summary>
        public bool CanInstallContentPackageBasedTitleUSB0 
        {
            get { return this.CanInstallContentPackageBasedTitleOn("USBMASS0MU"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be installed on USB1
        /// </summary>
        public bool CanInstallContentPackageBasedTitleUSB1 
        {
            get { return this.CanInstallContentPackageBasedTitleOn("USBMASS1MU"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be uninstalled
        /// </summary>
        public bool CanUninstallContentPackageTitle 
        {
            get { return this.XboxDevice.IsContentPackageBasedTitleInstalled; }
        }

        /// <summary>
        /// Gets a value indicating whether or not a disc image based title can be uninstalled
        /// </summary>
        public bool CanUninstallDiscImageTitle 
        {
            get 
            {
                return this.ConnectedAndResponding &&
                    this.IsDiscEmulationTitle &&
                    this.IsDiscEmulationRunning;
            } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a raw title can be uninstalled
        /// </summary>
        public bool CanUninstallRawTitle
        { 
            get
            {
                return this.ConnectedAndResponding &&
                    this.XboxDevice.IsDevKitDriveEnabled &&
                    this.IsRawTitle &&
                    this.IsRawTitleInstalled;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not a disc image based title can be uninstalled from HDD
        /// </summary>
        public bool CanUninstallContentPackageBasedTitleHDD 
        {
            get { return this.CanUninstallContentPackageBasedTitleFrom("HDD"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a disc image based title can be uninstalled from MU0
        /// </summary>
        public bool CanUninstallContentPackageBasedTitleMU0 
        {
            get { return this.CanUninstallContentPackageBasedTitleFrom("MU0"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a disc image based title can be uninstalled from MU1
        /// </summary>
        public bool CanUninstallContentPackageBasedTitleMU1 
        {
            get { return this.CanUninstallContentPackageBasedTitleFrom("MU1"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a disc image based title can be uninstalled from MUINT
        /// </summary>
        public bool CanUninstallContentPackageBasedTitleMUINT 
        {
            get { return this.CanUninstallContentPackageBasedTitleFrom("MUINT"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a disc image based title can be uninstalled from INTUSB
        /// </summary>
        public bool CanUninstallContentPackageBasedTitleINTUSB
        {
            get { return this.CanUninstallContentPackageBasedTitleFrom("INTUSB"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a disc image based title can be uninstalled from USB0
        /// </summary>
        public bool CanUninstallContentPackageBasedTitleUSB0 
        {
            get { return this.CanUninstallContentPackageBasedTitleFrom("USBMASS0MU"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a disc image based title can be uninstalled from USB1
        /// </summary>
        public bool CanUninstallContentPackageBasedTitleUSB1
        {
            get { return this.CanUninstallContentPackageBasedTitleFrom("USBMASS1MU"); }
        }

        /// <summary>
        /// Gets a value indicating whether or not a disc image based title can be uninstalled
        /// </summary>
        public bool CanUninstallContentPackageBasedTitle 
        { 
            get 
            {
                return this.CanInstallContentPackageBasedTitle &&
                    this.XboxDevice.IsAnyStorageDeviceEnabled &&
                    this.XboxDevice.IsContentPackageBasedTitleInstalled;
            } 
        }

        /// <summary>
        /// Gets a value indicating whether or not the recovery EXE has been configured
        /// </summary>
        public bool HasRecoveryExe 
        {
            get { return !string.IsNullOrEmpty(this.mainViewModel.CurrentTitle.XdkRecoveryPath); } 
        }

        /// <summary>
        /// Gets a value indicating whether or not the HDD can be enabled
        /// </summary>
        public bool CanEnableHDD 
        {
            get { return this.ConnectedAndResponding && !this.IsHDDEnabled; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not the HDD can be disabled
        /// </summary>
        public bool CanDisableHDD 
        {
            get { return this.ConnectedAndResponding && this.IsHDDEnabled; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not MUINT can be enabled
        /// </summary>
        public bool CanEnableMUINT
        {
            get { return this.ConnectedAndResponding && !this.IsMUINTEnabled; } 
        }
        
        /// <summary>
        /// Gets a value indicating whether or not MUINT can be disabled
        /// </summary>
        public bool CanDisableMUINT 
        {
            get { return this.ConnectedAndResponding && this.IsMUINTEnabled; } 
        }

        /// <summary>
        /// Gets a value indicating whether or the Xbox can be flashed (is connected and recovery exe has been set)
        /// </summary>
        public bool CanFlash
        {
            get { return this.ConnectedAndResponding && this.HasRecoveryExe; } 
        }

        /// <summary>
        /// Gets a value indicating whether or the video mode can be set
        /// </summary>
        public bool CanSetVideoMode
        {
            get { return this.ConnectedAndResponding; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the language can be set
        /// </summary>
        public bool CanSetLanguage 
        {
            get { return this.ConnectedAndResponding; } 
        }

        /// <summary>
        /// Gets a value indicating whether or the currently configured title is content package based
        /// </summary>
        public bool IsContentPackageTitle 
        { 
            get { return this.mainViewModel.CurrentTitle.GameInstallType == "Content Package"; } 
        }

        /// <summary>
        /// Gets a value indicating whether or the currently configured title is disc image based
        /// </summary>
        public bool IsDiscEmulationTitle 
        { 
            get { return this.mainViewModel.CurrentTitle.GameInstallType == "Disc Emulation"; } 
        }

        /// <summary>
        /// Gets a value indicating whether or the currently configured title is raw
        /// </summary>
        public bool IsRawTitle
        { 
            get { return this.mainViewModel.CurrentTitle.GameInstallType == "Raw"; }
        }

        /// <summary>
        /// Gets a value indicating whether or a raw title is currently installed
        /// </summary>
        public bool IsRawTitleInstalled 
        {
            get { return this.IsRawTitle && this.XboxDevice.IsRawTitleInstalled; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not the currently configured title is installed
        /// </summary>
        public bool IsTitleInstalled
        {
            get { return this.XboxDevice.IsTitleInstalled; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not disc emulation is running
        /// </summary>
        public bool IsDiscEmulationRunning 
        {
            get { return this.XboxDevice.IsDiscEmulationRunning; }
        }

        /// <summary>
        /// Gets a value indicating whether or not disc emulation can be started
        /// </summary>
        public bool CanMountDiscImage 
        {
            get 
            {
                return this.ConnectedAndResponding && 
                    this.IsDiscEmulationTitle && 
                    !this.IsDiscEmulationRunning; 
            } 
        }

        /// <summary>
        /// Gets a value indicating whether or not disc emulation can be stopped
        /// </summary>
        public bool CanUnmountDiscImage 
        { 
            get
            {
                return this.ConnectedAndResponding &&
                    this.IsDiscEmulationTitle &&
                    this.IsDiscEmulationRunning;
            } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title is installed
        /// </summary>
        public bool IsContentPackageBasedTitleInstalled 
        {
            get { return this.XboxDevice.IsContentPackageBasedTitleInstalled; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title is installed on HDD
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledHDD 
        {
            get { return this.XboxDevice.IsContentPackageBasedTitleInstalledOnHDD; }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title is installed on MU0
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledMU0
        {
            get { return this.XboxDevice.IsContentPackageBasedTitleInstalledOnMU0; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title is installed on MU1
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledMU1 
        {
            get { return this.XboxDevice.IsContentPackageBasedTitleInstalledOnMU1; }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title is installed on MUINT
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledMUINT 
        {
            get { return this.XboxDevice.IsContentPackageBasedTitleInstalledOnMUINT; }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title is installed on INTUSB
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledINTUSB
        {
            get { return this.XboxDevice.IsContentPackageBasedTitleInstalledOnINTUSB; }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title is installed on USB0
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledUSB0
        {
            get { return this.XboxDevice.IsContentPackageBasedTitleInstalledOnUSB0; }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title is installed on USB1
        /// </summary>
        public bool IsContentPackageBasedTitleInstalledUSB1 
        {
            get { return this.XboxDevice.IsContentPackageBasedTitleInstalledOnUSB1; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a NON content package based title can be launched
        /// </summary>
        public bool CanLaunchNonContentPackageBasedTitle 
        {
            get { return this.IsRawTitleInstalled || this.IsDiscEmulationRunning; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be launched
        /// </summary>
        public bool CanLaunchContentPackageBasedTitle 
        { 
            get 
            {
                return this.IsContentPackageTitle &&
                    this.IsContentPackageBasedTitleInstalled &&
                    this.IsHDDEnabled; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be launched on HDD
        /// </summary>
        public bool CanLaunchContentPackageBasedTitleHDD
        {
            get { return this.IsContentPackageTitle && this.IsContentPackageBasedTitleInstalledHDD; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be launched on MU0
        /// </summary>
        public bool CanLaunchContentPackageBasedTitleMU0 
        {
            get { return this.IsContentPackageTitle && this.IsContentPackageBasedTitleInstalledMU0; }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be launched on MU1
        /// </summary>
        public bool CanLaunchContentPackageBasedTitleMU1
        {
            get { return this.IsContentPackageTitle && this.IsContentPackageBasedTitleInstalledMU1; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be launched on MUINT
        /// </summary>
        public bool CanLaunchContentPackageBasedTitleMUINT 
        {
            get { return this.IsContentPackageTitle && this.IsContentPackageBasedTitleInstalledMUINT; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be launched on INTUSB
        /// </summary>
        public bool CanLaunchContentPackageBasedTitleINTUSB 
        {
            get { return this.IsContentPackageTitle && this.IsContentPackageBasedTitleInstalledINTUSB; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be launched on USB0
        /// </summary>
        public bool CanLaunchContentPackageBasedTitleUSB0 
        {
            get { return this.IsContentPackageTitle && this.IsContentPackageBasedTitleInstalledUSB0; }
        }

        /// <summary>
        /// Gets a value indicating whether or not a content package based title can be launched on USB1
        /// </summary>
        public bool CanLaunchContentPackageBasedTitleUSB1 
        {
            get { return this.IsContentPackageTitle && this.IsContentPackageBasedTitleInstalledUSB1; } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this Xbox is currently selected (checked) in the list of devices
        /// </summary>
        public bool IsSelected 
        { 
            get 
            {
                return this.XboxDevice.IsSelected; 
            } 

            set
            {
                this.XboxDevice.IsSelected = value;
                this.NotifyPropertyChanged(); 
            } 
        }

        /// <summary>
        /// Gets a value indicating whether or not the Xbox is currently connected
        /// </summary>
        public bool Connected
        {
            get
            {
                XboxDevice xboxDevice = this.XboxDevice;
                if (xboxDevice == null)
                {
                    return false;
                }

                return xboxDevice.Connected;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the Xbox is currently connected and responding
        /// </summary>
        public bool ConnectedAndResponding
        {
            get
            {
                XboxDevice xboxDevice = this.XboxDevice;
                if (xboxDevice == null)
                {
                    return false;
                }

                return xboxDevice.Connected && xboxDevice.Responding;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the Xbox is currently disconnected
        /// </summary>
        public bool Disconnected
        {
            get { return !this.XboxDevice.Connecting && !this.XboxDevice.Connected; } 
        }

        /// <summary>
        /// Gets a value indicating whether or not the Xbox is currently connecting
        /// </summary>
        public bool Connecting 
        {
            get { return this.XboxDevice.Connecting; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the connection is established or being established
        /// </summary>
        public bool ConnectedOrConnecting 
        {
            get { return this.Connected || this.Connecting; } 
        }

        /// <summary>
        /// Gets the name of this Xbox
        /// </summary>
        public string Name
        {
            get { return this.XboxDevice.Name; }
        }

        /// <summary>
        /// Gets the IP address of this Xbox
        /// </summary>
        public string IP 
        {
            get { return this.XboxDevice.IP; }
        }

        /// <summary>
        /// Gets a value indicating whether or not this Xbox is currently the default
        /// </summary>
        public bool IsDefault
        {
            get { return this.XboxDevice.IsDefault; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not there is currently 1 GB of RAM enabled
        /// </summary>
        public bool Has1GB
        {
            get
            {
                return this.XboxDevice.Has1GB;
            }

            set
            {
                if (value)
                {
                    this.SetTo1GB();
                }
                else
                {
                    this.SetTo512MB();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not there is currently only 512MB of RAM enabled
        /// </summary>
        public bool Has512MB
        {
            get
            {
                return !this.XboxDevice.Has1GB;
            }

            set
            {
                if (value == true)
                {
                    this.SetTo512MB();
                }
                else
                {
                    this.SetTo1GB();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the Xbox is 1GB RAM capable
        /// </summary>
        public bool Is1GBCapable
        {
            get { return this.XboxDevice.Is1GBCapable; }
        }

        /// <summary>
        /// Gets a value indicating whether or not the Xbox can be configured to 1GB RAM (and it not already)
        /// </summary>
        public bool CanSetTo1GB
        {
            get { return this.ConnectedAndResponding && this.Is1GBCapable && !this.Has1GB; } 
        }

        /// <summary>
        /// Gets the RAM capability of this Xbox
        /// </summary>
        public string RamSize
        {
            get 
            {
                string ramSize = string.Empty;
                if (this.XboxDevice != null)
                {
                    if (!this.XboxDevice.Has1GB)
                    {
                        ramSize = "512MB";
                    }
                    else
                    {
                        ramSize = "1GB";
                    }
                }

                return ramSize;
            }
        }

        /// <summary>
        /// Gets the name of image file to display for this Xbox
        /// </summary>
        public string ImageName
        {
            get 
            {
                string imageName = @"Images\xdk_small.png";
                if (this.Connected && !this.IsOffline)
                {
                    if (this.XboxDevice.Is1GBCapable)
                    {
                        if (this.XboxDevice.IsSlim)
                        {
                            imageName = @"Images\xdkSlim_small.png";
                        }
                        else
                        {
                            imageName = @"Images\xdkGB_small.png";
                        }
                    }

                    if (this.XboxDevice.Type == XboxKitType.TestKit)
                    {
                        imageName = @"Images\xdkTest_small.png";
                    }
                }

                return imageName; 
            }
        }

        /// <summary>
        /// Gets the type of Xbox kit for this Xbox
        /// </summary>
        public string KitType
        { 
            get
            {
                string result = string.Empty;
                XboxKitType kitType = this.XboxDevice.Type;
                switch (kitType)
                {
                    case XboxKitType.TestKit:
                        result = "Test Kit";
                        break;
                    case XboxKitType.DevelopmentKit:
                        result = "Development Kit";
                        break;
                    case XboxKitType.ReviewerKit:
                        result = "Reviewer Kit";
                        break;
                    default:
                        break;
                }

                return result; 
            } 
        }

        /// <summary>
        /// Gets or sets the Command to reconnect to this Xbox
        /// </summary>
        public Command ReconnectDeviceCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to install the game title on the HDD
        /// </summary>
        public Command InstallContentPackageBasedTitleHDDCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to install the game title on the M0
        /// </summary>
        public Command InstallContentPackageBasedTitleMU0Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to install the game title on the MU1
        /// </summary>
        public Command InstallContentPackageBasedTitleMU1Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to install the game title on the MUINT
        /// </summary>
        public Command InstallContentPackageBasedTitleMUINTCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to install the game title on the INTUSB
        /// </summary>
        public Command InstallContentPackageBasedTitleINTUSBCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to install the game title on the USB0
        /// </summary>
        public Command InstallContentPackageBasedTitleUSB0Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to install the game title on the USB1
        /// </summary>
        public Command InstallContentPackageBasedTitleUSB1Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to uninstall the game title on the HDD
        /// </summary>
        public Command UninstallContentPackageBasedTitleHDDCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to uninstall the game title on the M0
        /// </summary>
        public Command UninstallContentPackageBasedTitleMU0Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to uninstall the game title on the MU1
        /// </summary>
        public Command UninstallContentPackageBasedTitleMU1Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to uninstall the game title on the MUINT
        /// </summary>
        public Command UninstallContentPackageBasedTitleMUINTCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to uninstall the game title on the INTUSB
        /// </summary>
        public Command UninstallContentPackageBasedTitleINTUSBCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to uninstall the game title on the USB0
        /// </summary>
        public Command UninstallContentPackageBasedTitleUSB0Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to uninstall the game title on the USB1
        /// </summary>
        public Command UninstallContentPackageBasedTitleUSB1Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to start disc image emulation
        /// </summary>
        public Command StartEmulatingDiscImageTitleCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to stop disc image emulation
        /// </summary>
        public Command StopEmulatingDiscImageTitleCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to install a raw title
        /// </summary>
        public Command InstallRawTitleCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to uninstall a raw title
        /// </summary>
        public Command UninstallRawTitleCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to save a screenshot of this Xbox
        /// </summary>
        public Command SaveScreenShotCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to display a screenshot of this Xbox
        /// </summary>
        public Command DisplayScreenShotCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to cold Reboot this Xbox
        /// </summary>
        public Command ColdRebootDeviceCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to launch the developer dashboard
        /// </summary>
        public Command LaunchDevDashboardCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to enable the HDD on the Xbox
        /// </summary>
        public Command EnableHDDCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to disable the HDD on the Xbox
        /// </summary>
        public Command DisableHDDCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to enable MUINT on the Xbox
        /// </summary>
        public Command EnableMUINTCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to disable MUINT on the Xbox
        /// </summary>
        public Command DisableMUINTCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to remove this Xbox from the Xbox neighborhood
        /// </summary>
        public Command RemoveDeviceCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to flash the Xbox w/SEP
        /// </summary>
        public Command FlashDeviceWithSEPCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to flash the Xbox w/o SEP
        /// </summary>
        public Command FlashDeviceWithoutSEPCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set this Xbox as the default
        /// </summary>
        public Command SetAsDefaultCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to launch the currently configured (and installed) title
        /// </summary>
        public Command LaunchTitleCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to launch an Explorer window for this Xbox
        /// </summary>
        public Command LaunchExplorerCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to synchronize the time on the Xbox with this PC
        /// </summary>
        public Command SynchronizeTimeCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to launch a content package based title on the HDD
        /// </summary>
        public Command LaunchContentPackageBasedTitleHDDCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to launch a content package based title on MU0
        /// </summary>
        public Command LaunchContentPackageBasedTitleMU0Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to launch a content package based title on MU1
        /// </summary>
        public Command LaunchContentPackageBasedTitleMU1Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to launch a content package based title on MUINT
        /// </summary>
        public Command LaunchContentPackageBasedTitleMUINTCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to launch a content package based title on INTUSB
        /// </summary>
        public Command LaunchContentPackageBasedTitleINTUSBCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to launch a content package based title on USB0
        /// </summary>
        public Command LaunchContentPackageBasedTitleUSB0Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to launch a content package based title on USB1
        /// </summary>
        public Command LaunchContentPackageBasedTitleUSB1Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the Xbox to 1GB mode
        /// </summary>
        public Command SetTo1GBCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the Xbox to 512MB mode
        /// </summary>
        public Command SetTo512MBCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to get the current resolution of the Xbox
        /// </summary>
        public Command GetResolutionCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to NTSC-M 640x480
        /// </summary>
        public Command SetVideoNTSCM640x480Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to NTSC-M 640x480 Wide
        /// </summary>
        public Command SetVideoNTSCM640x480WideCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to NTSC-M 1280x720p
        /// </summary>
        public Command SetVideoNTSCM1280x720pCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to NTSC-M 1920x1080i
        /// </summary>
        public Command SetVideoNTSCM1920x1080iCommand { get; set; }
        
        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to NTSC-M 1920x1080p
        /// </summary>
        public Command SetVideoNTSCM1920x1080pCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to NTSC-J 640x480
        /// </summary>
        public Command SetVideoNTSCJ640x480Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to NTSC-J 640x480 Wide
        /// </summary>
        public Command SetVideoNTSCJ640x480WideCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to NTSC-J 1280x720p
        /// </summary>
        public Command SetVideoNTSCJ1280x720pCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to NTSC-J 1920x1080i
        /// </summary>
        public Command SetVideoNTSCJ1920x1080iCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to NTSC-J 1920x1080p
        /// </summary>
        public Command SetVideoNTSCJ1920x1080pCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to PAL-50 640x576
        /// </summary>
        public Command SetVideoPAL50640x576Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to PAL-50 640x576 Wide
        /// </summary>
        public Command SetVideoPAL50640x576WideCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to PAL-50 1280x720p
        /// </summary>
        public Command SetVideoPAL501280x720pCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to PAL-50 1920x1080i
        /// </summary>
        public Command SetVideoPAL501920x1080iCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to PAL-50 1920x1080p
        /// </summary>
        public Command SetVideoPAL501920x1080pCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to PAL-60 640x480
        /// </summary>
        public Command SetVideoPAL60640x480Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to PAL-60 640x480 Wide
        /// </summary>
        public Command SetVideoPAL60640x480WideCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to PAL-60 1280x720p
        /// </summary>
        public Command SetVideoPAL601280x720pCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to PAL-60 1920x1080i
        /// </summary>
        public Command SetVideoPAL601920x1080iCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to PAL-60 1920x1080p
        /// </summary>
        public Command SetVideoPAL601920x1080pCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 640x480
        /// </summary>
        public Command SetVideoVGA640x480Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 640x480 Wide
        /// </summary>
        public Command SetVideoVGA640x480WideCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 848x480
        /// </summary>
        public Command SetVideoVGA848x480Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 1024x768
        /// </summary>
        public Command SetVideoVGA1024x768Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 1024x768 Wide
        /// </summary>
        public Command SetVideoVGA1024x768WideCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 1280x720
        /// </summary>
        public Command SetVideoVGA1280x720Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 1280x768
        /// </summary>
        public Command SetVideoVGA1280x768Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 1280x1024
        /// </summary>
        public Command SetVideoVGA1280x1024Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 1280x1024 Wide
        /// </summary>
        public Command SetVideoVGA1280x1024WideCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 1360x768
        /// </summary>
        public Command SetVideoVGA1360x768Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 1440x900
        /// </summary>
        public Command SetVideoVGA1440x900Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 1680x1050
        /// </summary>
        public Command SetVideoVGA1680x1050Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current resolution of the Xbox to VGA 1920x1080
        /// </summary>
        public Command SetVideoVGA1920x1080Command { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to English
        /// </summary>
        public Command SetLanguageEnglishCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Japanese
        /// </summary>
        public Command SetLanguageJapaneseCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to German
        /// </summary>
        public Command SetLanguageGermanCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to French
        /// </summary>
        public Command SetLanguageFrenchCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Spanish
        /// </summary>
        public Command SetLanguageSpanishCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Italian
        /// </summary>
        public Command SetLanguageItalianCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Korean
        /// </summary>
        public Command SetLanguageKoreanCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Traditional Chinese
        /// </summary>
        public Command SetLanguageTraditionalChineseCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Portuguese
        /// </summary>
        public Command SetLanguageBrazilianPortugueseCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Polish
        /// </summary>
        public Command SetLanguagePolishCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Russian
        /// </summary>
        public Command SetLanguageRussianCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Swedish
        /// </summary>
        public Command SetLanguageSwedishCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Turkish
        /// </summary>
        public Command SetLanguageTurkishCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Norwegian
        /// </summary>
        public Command SetLanguageNorwegianCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Dutch
        /// </summary>
        public Command SetLanguageDutchCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to set the current language of the Xbox to Simplified Chinese
        /// </summary>
        public Command SetLanguageSimplifiedChineseCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to open the debug output window for this Xbox
        /// </summary>
        public Command OpenDebugOutputCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to open the profile manager window for this Xbox
        /// </summary>
        public Command OpenProfileManagerCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to open the virtual controller window for this Xbox
        /// </summary>
        public Command OpenVirtualControllerCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command delete all profiles from all drives
        /// </summary>
        public Command DeleteAllProfilesCommand { get; set; }

        /// <summary>
        /// Gets or sets the Command to delete all saves from all drives
        /// </summary>
        public Command DeleteAllSavesCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to delete all games from all drives
        /// </summary>
        public Command DeleteAllGamesCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to run an automation script
        /// </summary>
        public Command RunScriptCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to run a particular XBox automation input script item
        /// </summary>
        public Command RunScriptItemCommand { get; set; }

        /// <summary>
        /// Gets or sets a reference to the view model for the debug output window
        /// </summary>
        public DebugOutputViewModel DebugOutputViewModel
        {
            get
            {
                return this.debugOutputViewModel;
            }

            set
            {
                this.debugOutputViewModel = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("IsDebugOutputOpen");
                this.NotifyPropertyChanged("CanOpenDebugOutput");
            }
        }
        
        /// <summary>
        /// Gets or sets a reference to the view model for the debug output window
        /// </summary>
        public ProfileManagerViewModel ProfileManagerViewModel
        {
            get
            {
                return this.profileManagerViewModel;
            }

            set
            {
                this.profileManagerViewModel = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("IsProfileManagerOpen");
            }
        }

        /// <summary>
        /// Gets a value indicating whether the debug output window is open
        /// </summary>
        public bool IsDebugOutputOpen
        {
            get { return this.DebugOutputViewModel != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the debug open window can be opened
        /// </summary>
        public bool CanOpenDebugOutput
        {
            get { return this.ConnectedAndResponding && this.XboxDevice.CanDebug; }
        }

        /// <summary>
        /// Gets a value indicating whether the debug output window is open
        /// </summary>
        public bool IsProfileManagerOpen
        {
            get { return this.ProfileManagerViewModel != null; }
        }

        /// <summary>
        /// Opens the debug output window
        /// </summary>
        public void OpenDebugOutput()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (this.DebugOutputViewModel != null)
            {
                this.DebugOutputViewModel.Activate();
            }
            else
            {
                bool canOpen = true;
                Process[] processes = Process.GetProcessesByName("xbWatson");
                if (processes.Length > 0)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("xbWatson is known to conflict with CAT's Debug Output monitoring, if connected to the same Xbox.  Are you sure you want to proceed?", "xbWatson is currently running", MessageBoxButton.YesNo);
                    if (messageBoxResult != MessageBoxResult.Yes)
                    {
                        canOpen = false;
                    }
                }

                if (canOpen)
                {
                    processes = Process.GetProcessesByName("ApiMon");
                    if (processes.Length > 0)
                    {
                        MessageBoxResult messageBoxResult = MessageBox.Show("API Monitor is known to conflict with CAT's API monitoring, if connected to the same Xbox.  Are you sure you want to proceed?", "API Monitor is currently running", MessageBoxButton.YesNo);
                        if (messageBoxResult != MessageBoxResult.Yes)
                        {
                            canOpen = false;
                        }
                    }
                }

                if (canOpen)
                {
                    this.DebugOutputViewModel = new DebugOutputViewModel(this, this.mainViewModel);
                    this.DebugOutputViewModel.Start();
                }
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Opens the profile manager window
        /// </summary>
        public void OpenProfileManager()
        {
            if (this.ProfileManagerViewModel != null)
            {
                this.ProfileManagerViewModel.Activate();
            }
            else
            {
                this.ProfileManagerViewModel = new ProfileManagerViewModel(this, this.mainViewModel);
            }
        }

        /// <summary>
        /// Opens the virtual controller window, if not already open, and selects this Xbox
        /// </summary>
        public void OpenVirtualController()
        {
            if (this.mainViewModel.VirtualControllerViewModel == null)
            {
                this.mainViewModel.VirtualControllerViewModel = new VirtualControllerViewModel(this.mainViewModel);
            }

            this.mainViewModel.VirtualControllerViewModel.Activate();
            this.mainViewModel.VirtualControllerViewModel.SelectXbox(this);
        }

        /// <summary>
        /// Triggers the reconnect retry timeout immediately
        /// </summary>
        public void ExpireRetryTimerNow()
        {
            Timer timer = this.reconnectTimer;
            if (timer != null)
            {
                timer.Change(TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(-1));
            }

            timer = this.refreshTimer;
            if (timer != null)
            {
                timer.Change(TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(-1));
            }
        }

        /// <summary>
        /// Refresh all state bound to in XAML
        /// (Not necessary to refresh state here if bound only to ContextMenu which queries state when opened).
        /// </summary>
        public void RefreshState()
        {
            this.NotifyPropertyChanged("Connected");
            this.NotifyPropertyChanged("Connecting");
            this.NotifyPropertyChanged("ConnectedOrConnecting");
            this.NotifyPropertyChanged("Disconnected");

            this.NotifyPropertyChanged("Name");
            this.NotifyPropertyChanged("IP");
            this.NotifyPropertyChanged("IsDefault");
            this.NotifyPropertyChanged("RamSize");
            this.NotifyPropertyChanged("ImageName");
            this.NotifyPropertyChanged("KitType");

            this.NotifyPropertyChanged("CanInstallContentPackageBasedTitle");
            this.NotifyPropertyChanged("CanInstallRawTitle");
            this.NotifyPropertyChanged("CanInstallDiscImageTitle");

            this.NotifyPropertyChanged("IsContentPackageBasedTitleInstalled");
            this.NotifyPropertyChanged("IsRawTitleInstalled");
            this.NotifyPropertyChanged("IsDiscEmulationRunning");
            this.NotifyPropertyChanged("IsTitleInstalled");

            this.NotifyPropertyChanged("CanEnableHDD");
            this.NotifyPropertyChanged("CanDisableHDD");
            this.NotifyPropertyChanged("CanEnableMUINT");
            this.NotifyPropertyChanged("CanDisableMUINT");

            this.NotifyPropertyChanged("IsHDDEnabled");
            this.NotifyPropertyChanged("IsMUINTEnabled");
            this.NotifyPropertyChanged("IsINTUSBEnabled");
            this.NotifyPropertyChanged("IsUSB0Enabled");
            this.NotifyPropertyChanged("IsUSB1Enabled");
            this.NotifyPropertyChanged("IsMU0Enabled");
            this.NotifyPropertyChanged("IsMU1Enabled");

            this.NotifyPropertyChanged("Is1GBCapable");
            this.NotifyPropertyChanged("CanSetTo1GB");
            this.NotifyPropertyChanged("Has1GB");

            this.NotifyPropertyChanged("IsOffline");
            this.NotifyPropertyChanged("ConnectedAndResponding");

            if (this.IsClicked)
            {
                this.AuxPanelVisibility = (this.Connected && !this.IsOffline) ? Visibility.Visible : Visibility.Collapsed;
            }
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

        /// <summary>
        /// Deletes all saves from all drives
        /// </summary>
        private void DeleteAllSaves()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (!this.XboxDevice.DeleteAllSaves())
            {
                MessageBox.Show("Some file delete operations may have failed.", "Delete failed", MessageBoxButton.OK);
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Deletes all games from all drives
        /// </summary>
        private void DeleteAllGames()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (!this.XboxDevice.DeleteAllGames())
            {
                MessageBox.Show("Some file delete operations may have failed.", "Delete failed", MessageBoxButton.OK);
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Deletes all profiles from all drives
        /// </summary>
        private void DeleteAllProfiles()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (!this.XboxDevice.DeleteAllProfiles())
            {
                MessageBox.Show("Some file delete operations may have failed.", "Delete failed", MessageBoxButton.OK);
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Run any automation script for debugging
        /// </summary>
        private void RunScript()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.XboxDevice.RunScript();
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Runs a particular XBox automation input script item based on the specified script
        /// parameter
        /// </summary>
        /// <param name="scriptName">Xbox automation input script name to execute</param>
        private void RunScriptItem(string scriptName)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if (0 == string.Compare("Back_Four", scriptName))
            {
                this.XboxDevice.RunCatScript("BackTwice");
                Thread.Sleep(2000); //// Wait 2 seconds between BackTwice calls
                this.XboxDevice.RunCatScript("BackTwice");
            }
            else
            {
                this.XboxDevice.RunCatScript(scriptName);
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Check if a content-package based title can be installed on the specified drive
        /// </summary>
        /// <param name="drive">The drive to check</param>
        /// <returns>true if the title can be installed, false otherwise</returns>
        private bool CanInstallContentPackageBasedTitleOn(string drive)
        {
            return this.CanInstallContentPackageBasedTitle &&
                this.XboxDevice.IsDriveEnabled(drive) &&
                !this.XboxDevice.IsContentPackageBasedTitleInstalledOn(drive);
        }

        /// <summary>
        /// Check if a content-package based title can be uninstalled on the specified drive
        /// </summary>
        /// <param name="drive">The drive to check</param>
        /// <returns>true if the title can be uninstalled, false otherwise</returns>
        private bool CanUninstallContentPackageBasedTitleFrom(string drive)
        {
            return this.CanInstallContentPackageBasedTitle &&
                this.XboxDevice.IsDriveEnabled(drive) &&
                this.XboxDevice.IsContentPackageBasedTitleInstalledOn(drive);
        }

        /// <summary>
        /// Installs a content-package based title on the specified drive
        /// </summary>
        /// <param name="drive">Drive to install the title on</param>
        private void InstallContentPackageBasedTitle(string drive)
        {
            if (this.CanInstallContentPackageBasedTitleOn(drive))
            {
                MessageBoxResult result = MessageBox.Show(string.Format("Are you sure you want to install \"{0}\" to {1} on the Xbox  \"{2}\"?", this.mainViewModel.CurrentTitle.Name, drive, XboxDevice.Name), "Install Title?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.XboxDevice.InstallTitle(drive, new ProgressBarViewModel("Installing " + this.mainViewModel.CurrentTitle.Name + "...", this.mainViewModel.MainWindow));
                    Mouse.OverrideCursor = null;
                }
            }
        }

        /// <summary>
        /// Uninstalls a content-package based title from the specified drive
        /// </summary>
        /// <param name="drive">Drive to uninstall the title from</param>
        private void UninstallContentPackageBasedTitle(string drive)
        {
            if (this.CanUninstallContentPackageBasedTitleFrom(drive))
            {
                MessageBoxResult result = MessageBox.Show(string.Format("Are you sure you want to uninstall \"{0}\" from {1} on the Xbox  \"{2}\"?", this.mainViewModel.CurrentTitle.Name, drive, this.XboxDevice.Name), "Install Title?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.XboxDevice.UninstallContentPackageBasedTitle(drive);
                    Mouse.OverrideCursor = null;
                }
            }
        }

        /// <summary>
        /// Takes a screen shot and saves it
        /// </summary>
        private void SaveScreenShot()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "ScreenShot";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Jpeg|*.jpg";

            // Show save file dialog box
            bool? result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                Mouse.OverrideCursor = Cursors.Wait;
                this.XboxDevice.ScreenShot(dlg.FileName);
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Takes a screen shot and displays it
        /// </summary>
        private void DisplayScreenShot()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            BitmapImage image = this.XboxDevice.ScreenShot();
            if (image != null)
            {
                new ScreenShotViewModel("Snapshot: " + this.XboxDevice.Name + " " + DateTime.Now.ToString(), image);
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Performs a cold reboot of this Xbox
        /// </summary>
        private void ColdReboot()
        {
            Thread th = new Thread(new ParameterizedThreadStart(delegate
            {
                this.XboxDevice.Reboot(true);
                this.RefreshState();
            }));
            th.Start();
        }

        /// <summary>
        /// Launches the developer dashboard on this Xbox
        /// </summary>
        private void LaunchDevDashboard()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.XboxDevice.LaunchDevDashboard(false);
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Enables the HDD on this Xbox
        /// </summary>
        private void EnableHDD()
        {
            MessageBoxResult result = MessageBox.Show("Enabling the HDD will reboot the Xbox. Are you sure?", "Enable HDD?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.XboxDevice.SetHDDEnabled(true);
                this.RefreshState();
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Disables the HDD on this Xbox
        /// </summary>
        private void DisableHDD()
        {
            MessageBoxResult result = MessageBox.Show("Disabling the HDD will reflash the Xbox. Are you sure?", "Disable HDD?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.XboxDevice.SetHDDEnabled(false);
                this.RefreshState();
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Enables the internal MU and internal USB on this Xbox
        /// </summary>
        private void EnableMUINT()
        {
            MessageBoxResult result = MessageBox.Show("Enabling the internal MU will reboot the Xbox. Are you sure?", "Enable MUINT?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.XboxDevice.IsMUINTEnabled = true;
                this.RefreshState();
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Disables the internal MU and internal USB on this Xbox
        /// </summary>
        private void DisableMUINT()
        {
            MessageBoxResult result = MessageBox.Show("Disabling the internal MU will reflash the Xbox. Are you sure?", "Disable MUINT?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.XboxDevice.IsMUINTEnabled = false;
                this.RefreshState();
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Removes this Xbox from the Xbox Neighborhood
        /// </summary>
        private void RemoveDevice()
        {
            MessageBoxResult result = MessageBox.Show(string.Format("Are you sure you want to remove the Xbox named \"{0}\"?", XboxDevice.Name), "Remove Xbox?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                this.Remove();
            }
        }

        /// <summary>
        /// Flashes this Xbox
        /// </summary>
        /// <param name="withSEP">Whether or not to install the System Extended Packages</param>
        private void FlashDevice(bool withSEP)
        {
            MessageBoxResult result = MessageBox.Show(string.Format("Are you sure you want to flash the Xbox named \"{0}\"?", XboxDevice.Name), "Flash Xbox?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.XboxDevice.Flash(withSEP);
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Starts disc emulation for this Xbox
        /// </summary>
        private void StartEmulatingDiscImageTitle()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.XboxDevice.StartEmulatingDiscTitle(new ProgressBarViewModel("Installing " + this.mainViewModel.CurrentTitle.Name + "...", this.mainViewModel.MainWindow));
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Stops disc emulation for this Xbox
        /// </summary>
        private void StopEmulatingDiscImageTitle()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.XboxDevice.StopEmulatingDiscTitle();
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Installs a raw title on this Xbox
        /// </summary>
        private void InstallRawTitle()
        {
            if (this.CanInstallRawTitle)
            {
                MessageBoxResult result = MessageBox.Show(string.Format("Are you sure you want to install \"{0}\" on the Xbox  \"{1}\"?", this.mainViewModel.CurrentTitle.Name, XboxDevice.Name), "Install Title?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.XboxDevice.InstallTitle(string.Empty, new ProgressBarViewModel("Installing " + this.mainViewModel.CurrentTitle.Name + "...", this.mainViewModel.MainWindow));
                    Mouse.OverrideCursor = null;
                }
            }
        }

        /// <summary>
        /// Uninstalls a raw title from this Xbox
        /// </summary>
        private void UninstallRawTitle()
        {
            if (this.CanUninstallRawTitle)
            {
                MessageBoxResult result = MessageBox.Show(string.Format("Are you sure you want to uninstall \"{0}\" on the Xbox  \"{1}\"?", this.mainViewModel.CurrentTitle.Name, XboxDevice.Name), "Install Title?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.XboxDevice.UninstallRawTitle(new ProgressBarViewModel("Uninstalling " + this.mainViewModel.CurrentTitle.Name + "...", this.mainViewModel.MainWindow));
                    Mouse.OverrideCursor = null;
                }
            }
        }

        /// <summary>
        /// Launches the current configured title on this Xbox
        /// </summary>
        /// <param name="drive">If a content package based title this specifies the drive to launch from, otherwise ignored</param>
        private void LaunchTitle(string drive)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.XboxDevice.LaunchTitle(drive);
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Launches an explorer window for this Xbox
        /// </summary>
        private void LaunchExplorer()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.XboxDevice.LaunchExplorer();
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Sets this Xbox as the default
        /// </summary>
        private void SetAsDefault()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            XboxViewItem oldDefault = null;
            XboxViewItem oldSelected = null;

            foreach (XboxViewItem xbvi in this.mainViewModel.XboxList)
            {
                if (xbvi.IsSelected)
                {
                    if (oldSelected == null)
                    {
                        oldSelected = xbvi;
                    }
                    else
                    {
                        oldSelected = this;  // If 2, point to this
                    }
                }

                if (xbvi.IsDefault)
                {
                    oldDefault = xbvi;
                }
            }

            if ((oldSelected == oldDefault) && (oldSelected != this))
            {
                if (oldSelected != null)
                {
                    oldSelected.IsSelected = false;
                }

                this.IsSelected = true;
            }

            this.XboxDevice.SetDefault();

            this.NotifyPropertyChanged("IsDefault");
            if (oldDefault != null)
            {
                oldDefault.NotifyPropertyChanged("IsDefault");
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Synchronizes the time of this Xbox with the PC
        /// </summary>
        private void SynchronizeTime()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.XboxDevice.SynchronizeTimeWithPC();
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Sets the RAM of this Xbox to 1GB
        /// </summary>
        private void SetTo1GB()
        {
            if (!this.Has1GB)
            {
                MessageBoxResult result = MessageBox.Show("Changing the RAM will reboot the Xbox.  Are you sure?", "Change RAM?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.XboxDevice.Set1GBEnabled(true);
                    this.RefreshState();
                    Mouse.OverrideCursor = null;
                }
            }
        }

        /// <summary>
        /// Sets the RAM of this Xbox to 512MB
        /// </summary>
        private void SetTo512MB()
        {
            if (!this.Has512MB)
            {
                MessageBoxResult result = MessageBox.Show("Changing the RAM will reboot the Xbox.  Are you sure?", "Change RAM?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.OK)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.XboxDevice.Set1GBEnabled(false);
                    this.RefreshState();
                    Mouse.OverrideCursor = null;
                }
            }
        }

        /// <summary>
        /// Gets the video resolution of this Xbox
        /// </summary>
        private void GetResolution()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            int height;
            int width;
            this.XboxDevice.GetCurrentResolution(out width, out height);
            Mouse.OverrideCursor = null;
            MessageBoxResult result = MessageBox.Show("Resolution: " + width + "x" + height);
        }

        /// <summary>
        /// Sets the video resolution of this Xbox
        /// </summary>
        /// <param name="resolution">The resolution to set to</param>
        /// <param name="format">The video format to use</param>
        private void SetResolution(VideoResolution resolution, VideoStandard format)
        {
            MessageBoxResult result = MessageBox.Show("Changing the video mode will reboot the Xbox.  Are you sure?", "Change Video Mode?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.XboxDevice.SetVideoMode(resolution, format, true);
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Sets the language of this Xbox
        /// </summary>
        /// <param name="language">The language to set this Xbox to</param>
        private void SetLanguage(Language language)
        {
            MessageBoxResult result = MessageBox.Show("Changing the language will reboot the Xbox.  Are you sure?", "Change Language?", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.XboxDevice.SetLanguage(language);
                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Connects to the Xbox and starts the reconnect retry timer
        /// </summary>
        private void Connect()
        {
            this.XboxDevice.Connect(this.ConnectComplete);

            // Set up a timer to try again every 30 seconds
            this.reconnectTimer = new Timer(_ => this.RetryTimerExpired(XboxDevice), null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(-1));
            this.RefreshState();
        }

        /// <summary>
        /// Reconnects using a new XboxDevice object, if the previous one had failed to connect
        /// </summary>
        private void Reconnect()
        {
            XboxDevice oldDevice = this.XboxDevice;
            XboxDevice device2 = new XboxDevice(oldDevice.ConnectTo, this.mainViewModel.CurrentTitle);
            device2.XboxViewItem = this;
            this.XboxDevice = device2;
            this.mainViewModel.PoolDevices.Remove(oldDevice);
            this.mainViewModel.PoolDevices.Add(device2);
            this.Connect();
        }

        /// <summary>
        /// Removes this Xbox from the neighborhood (and our UI)
        /// </summary>
        private void Remove()
        {
            if (this.DebugOutputViewModel != null)
            {
                this.DebugOutputViewModel.Close();
            }

            if (this.ProfileManagerViewModel != null)
            {
                this.ProfileManagerViewModel.Close();
            }

            this.XboxDevice.Remove();
            this.mainViewModel.XboxList.Remove(this);
            this.mainViewModel.PoolDevices.Remove(XboxDevice);
        }

        /// <summary>
        /// ConnectComplete is called as the connection to the Xbox is established,
        /// or fails to be established.
        /// Also called when a reboot completes.
        /// </summary>
        /// <param name="xbd">The Xbox associated with this ViewItem</param>
        /// <param name="connected">A value indicating whether or not the connection was successful</param>
        private void ConnectCompleteInUIThread(XboxDevice xbd, bool connected)
        {
            try
            {
                if (xbd == this.XboxDevice)
                {
                    // Handles duplicate Xboxes.
                    // It's possible the same Xbox may be present in the list using 3 separate names:
                    //  - Verbose name
                    //  - IP Address - i.e. 10.161.24.131)
                    //  - IP Address converted into an int - i.e. 178329731
                    if (connected && !this.reportedConnected)
                    {
                        this.reportedConnected = true;

                        // Disconnect, if we've already been removed.
                        if (this.mainViewModel.XboxList.Contains(this))   
                        {
                            // If name or IP is the same as another one, remove the other guy.
                            string myIP = this.IP;
                            string myName = this.Name;
                            List<XboxViewItem> itemsToRemoveAfterEnumerating = new List<XboxViewItem>();
                            foreach (XboxViewItem xbvi in this.mainViewModel.XboxList)
                            {
                                if (xbvi != this)
                                {
                                    uint ipaddr = XboxDevice.XboxConsole.IPAddress;
                                    string myIPInt = ipaddr.ToString();
                                    if ((xbvi.XboxDevice.ConnectTo == myIP) || (xbvi.XboxDevice.ConnectTo == myName) || (xbvi.XboxDevice.ConnectTo == myIPInt))
                                    {
                                        itemsToRemoveAfterEnumerating.Add(xbvi);

                                        // if the dupe was default, take over default status
                                        if (xbvi.IsDefault) 
                                        {
                                            xbd.SetDefault();
                                        }
                                    }
                                }
                            }
                        
                            foreach (XboxViewItem xbvi in itemsToRemoveAfterEnumerating)
                            {
                                this.mainViewModel.XboxList.Remove(xbvi);
                            }
                        }

                        this.IsSelected = XboxDevice.IsDefault;
                    }

                    if (connected)
                    {
                        if ((this.refreshTimer == null) && (this.AuxPanelVisibility == Visibility.Visible))
                        {
                            this.refreshTimer = new Timer(_ => this.RefreshTimerExpired(), null, TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(-1));
                        }
                    }

                    this.RefreshState();
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// ConnectComplete is called as the connection to the Xbox is established,
        /// or fails to be established.
        /// Also called when a reboot completes.
        /// </summary>
        /// <param name="xbd">The Xbox associated with this ViewItem</param>
        /// <param name="connected">A value indicating whether or not the connection was successful</param>
        private void ConnectComplete(XboxDevice xbd, bool connected)
        {
            Dispatcher dispatcher = this.mainViewModel.MainWindow.Dispatcher;
            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(new Action(() => { this.ConnectCompleteInUIThread(xbd, connected); }));
            }
        }

        /// <summary>
        /// Timeout expiration callback for Xbox reconnect attempts
        /// </summary>
        /// <param name="xbd">Xbox to reconnect to</param>
        private void RetryTimerExpiredInUIThread(XboxDevice xbd)
        {
            try
            {
                if (xbd == XboxDevice)
                {
                    if (this.Connected)
                    {
                        if (xbd.Responding)
                        {
                            this.RefreshState();
                        }
                        
                        this.reconnectTimer = new Timer(_ => this.RetryTimerExpired(xbd), null, TimeSpan.FromSeconds(10), TimeSpan.FromMilliseconds(-1));
                    }
                    else if (this.Connecting)
                    {
                        this.Reconnect();    // Connection attempt stalled and hasn't returned, start a fresh one
                    }
                    else
                    {
                        this.Connect();
                    }
                }
            }
            catch (Exception)
            { 
            }
        }

        /// <summary>
        /// Timeout expiration callback for Xbox reconnect attempts
        /// </summary>
        /// <param name="xbd">Xbox to reconnect to</param>
        private void RetryTimerExpired(XboxDevice xbd)
        {
            try
            {
                if (xbd.Connected)
                {
                    xbd.VerifyOnline();   // side effect will update status and block stalling calls
                    this.NotifyPropertyChanged("IsOffline");
                }

                Dispatcher dispatcher = this.mainViewModel.MainWindow.Dispatcher;
                if (dispatcher != null)
                {
                    dispatcher.BeginInvoke(new Action(() => { RetryTimerExpiredInUIThread(xbd); }));
                }
            }
            catch (Exception)
            {
            }
        }
        
        /// <summary>
        /// Timeout expiration callback for Xbox refreshes
        /// </summary>
        private void RefreshTimerExpiredInUIThread()
        {
            if (!this.Connected || (this.AuxPanelVisibility != Visibility.Visible))
            {
                this.refreshTimer = null;
            }
            else
            {
                this.RefreshState();
                this.refreshTimer = new Timer(_ => this.RefreshTimerExpired(), null, TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(-1));
            }
        }

        /// <summary>
        /// Timeout expiration callback for Xbox refreshes
        /// </summary>
        private void RefreshTimerExpired()
        {
            try
            {
                Dispatcher dispatcher = this.mainViewModel.MainWindow.Dispatcher;
                if (dispatcher != null)
                {
                    dispatcher.BeginInvoke(new Action(() => { this.RefreshTimerExpiredInUIThread(); }));
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
