// -----------------------------------------------------------------------
// <copyright file="IXboxDevice.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Collections.Generic;
    using Microsoft.Test.Xbox.Profiles;
    using XDevkit;

    /// <summary>
    /// Enumeration of all Xbox development kit types
    /// </summary>
    public enum XboxKitType
    {
        /// <summary>
        /// Development kit
        /// </summary>
        DevelopmentKit,

        /// <summary>
        /// Test kit
        /// </summary>
        TestKit,

        /// <summary>
        /// Reviewer kit
        /// </summary>
        ReviewerKit,

        /// <summary>
        /// Unknown kit type
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Enumeration of supported Xbox (dashboard) languages
    /// </summary>
    public enum Language
    {
        /// <summary>
        /// English language
        /// </summary>
        English,

        /// <summary>
        /// Japanese language
        /// </summary>
        Japanese,

        /// <summary>
        /// German language
        /// </summary>
        German,

        /// <summary>
        /// French language
        /// </summary>
        French,

        /// <summary>
        /// Spanish language
        /// </summary>
        Spanish,

        /// <summary>
        /// Italian language
        /// </summary>
        Italian,

        /// <summary>
        /// Korean language
        /// </summary>
        Korean,

        /// <summary>
        /// TraditionalChinese language
        /// </summary>
        TraditionalChinese,

        /// <summary>
        /// BrazilianPortuguese language
        /// </summary>
        BrazilianPortuguese,

        /// <summary>
        /// Polish language
        /// </summary>
        Polish,

        /// <summary>
        /// Russian language
        /// </summary>
        Russian,

        /// <summary>
        /// Swedish language
        /// </summary>
        Swedish,

        /// <summary>
        /// Turkish language
        /// </summary>
        Turkish,

        /// <summary>
        /// Norwegian language
        /// </summary>
        Norwegian,

        /// <summary>
        /// Dutch language
        /// </summary>
        Dutch,

        /// <summary>
        /// SimplifiedChinese language
        /// </summary>
        SimplifiedChinese
    }

    /// <summary>
    /// Enumeration of console regions
    /// </summary>
    public enum Region
    {
        /// <summary>
        /// North America region
        /// </summary>
        NorthAmerica,

        /// <summary>
        /// Japan region
        /// </summary>
        Japan,

        /// <summary>
        /// Asia region
        /// </summary>
        Asia,

        /// <summary>
        /// Australia region
        /// </summary>
        Australia,

        /// <summary>
        /// Europe region
        /// </summary>
        Europe,

        /// <summary>
        /// Chine region
        /// </summary>
        China,

        /// <summary>
        /// Rest of world region
        /// </summary>
        RestOfWorld
    }

    /// <summary>
    /// Enumeration of VGA Video modes and non-VGA video modes
    /// </summary>
    public enum VideoResolution
    {
        /// <summary>
        /// Mode640x480 video mode
        /// </summary>
        Mode640x480,

        /// <summary>
        /// Mode640x480Wide video mode
        /// </summary>
        Mode640x480Wide,

        /// <summary>
        /// Mode848x480 video mode
        /// </summary>
        Mode848x480,

        /// <summary>
        /// Mode1024x768 video mode
        /// </summary>
        Mode1024x768,

        /// <summary>
        /// Mode1024x768Wide video mode
        /// </summary>
        Mode1024x768Wide,

        /// <summary>
        /// Mode1280x720 video mode
        /// </summary>
        Mode1280x720,

        /// <summary>
        /// Mode1280x768 video mode
        /// </summary>
        Mode1280x768,

        /// <summary>
        /// Mode1280x1024 video mode
        /// </summary>
        Mode1280x1024,

        /// <summary>
        /// Mode1280x1024Wide video mode
        /// </summary>
        Mode1280x1024Wide,

        /// <summary>
        /// Mode1360x768 video mode
        /// </summary>
        Mode1360x768,

        /// <summary>
        /// Mode1440x900 video mode
        /// </summary>
        Mode1440x900,

        /// <summary>
        /// Mode1680x1050 video mode
        /// </summary>
        Mode1680x1050,

        ////Mode1920x540,

        /// <summary>
        /// Mode1920x1080 video mode
        /// </summary>
        Mode1920x1080,

        /// <summary>
        /// Mode480 video mode
        /// </summary>
        Mode480,

        /// <summary>
        /// Mode480Wide video mode
        /// </summary>
        Mode480Wide,

        /// <summary>
        /// Mode720p video mode
        /// </summary>
        Mode720p,

        /// <summary>
        /// Mode1080i video mode
        /// </summary>
        Mode1080i,

        /// <summary>
        /// Mode1080p video mode
        /// </summary>
        Mode1080p
    }

    /// <summary>
    /// Enumeration of supported video standards
    /// </summary>
    public enum VideoStandard
    {
        /// <summary>
        /// NTSCM video standard
        /// </summary>
        NTSCM,

        /// <summary>
        /// NTSCJ video standard
        /// </summary>
        NTSCJ,

        /// <summary>
        /// PAL50 video standard
        /// </summary>
        PAL50,

        /// <summary>
        /// PAL60 video standard
        /// </summary>
        PAL60
    }

    /// <summary>
    /// Interface for Xbox devices
    /// </summary>
    public interface IXboxDevice : IDevice
    {
        /// <summary>
        /// Gets the XboxConsole object associated with this Xbox
        /// </summary>
        XboxConsole XboxConsole { get; }

        /// <summary>
        /// Gets the IP address of the Xbox
        /// </summary>
        string IP { get; }

        /// <summary>
        /// Gets a value indicating whether or not the Xbox is currently connected and addressable
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Gets the type of Xbox
        /// </summary>
        XboxKitType Type { get; }

        /// <summary>
        /// Gets a value indicating whether the Xbox is capable of 1GB RAM (not necessarily in 1 Gig RAM mode right now)
        /// </summary>
        bool Is1GBCapable { get; }

        /// <summary>
        /// Gets a value indicating whether the Xbox is currently configured to 1 Gig of RAM
        /// </summary>
        bool Has1GB { get; }

        /// <summary>
        /// Gets a value indicating whether this is a Slim Xbox model
        /// </summary>
        bool IsSlim { get; }

        /// <summary>
        /// Gets a value indicating whether the Xbox is capable of debugging
        /// </summary>
        bool CanDebug { get; }

        /// <summary>
        /// Gets the XDK Version
        /// </summary>
        string XDKVersion { get; }

        /// <summary>
        /// Gets the list of drives on the Xbox
        /// </summary>
        List<string> Drives { get; }

        /// <summary>
        /// Gets a value indicating whether the HDD is currently enabled
        /// </summary>
        bool IsHDDEnabled { get; }

        /// <summary>
        /// Gets a value indicating whether the MU0 is currently enabled
        /// </summary>
        bool IsMU0Enabled { get; }

        /// <summary>
        /// Gets a value indicating whether the MU1 is currently enabled
        /// </summary>
        bool IsMU1Enabled { get; }

        /// <summary>
        /// Gets a value indicating whether the Internal USB storage device is currently enabled
        /// </summary>
        bool IsINTUSBEnabled { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the Internal MU is currently enabled
        /// </summary>
        bool IsMUINTEnabled { get; set; }

        /// <summary>
        /// Gets a value indicating whether the USB0 is currently enabled
        /// </summary>
        bool IsUSB0Enabled { get; }

        /// <summary>
        /// Gets a value indicating whether the USB1 is currently enabled
        /// </summary>
        bool IsUSB1Enabled { get; }

        /// <summary>
        /// Gets a value indicating whether Can Install Title
        /// </summary>
        bool CanInstallTitle { get; }

        /// <summary>
        /// Gets a value indicating whether Is Title Installed
        /// </summary>
        bool IsTitleInstalled { get; }

        /// <summary>
        /// Gets a value indicating whether Is Content Package Based Title Installed
        /// </summary>
        bool IsContentPackageBasedTitleInstalled { get; }

        /// <summary>
        /// Gets a value indicating whether Is Content Package Based Title Installed On HDD
        /// </summary>
        bool IsContentPackageBasedTitleInstalledOnHDD { get; }

        /// <summary>
        /// Gets a value indicating whether Is Content Package Based Title Installed On MU0
        /// </summary>
        bool IsContentPackageBasedTitleInstalledOnMU0 { get; }

        /// <summary>
        /// Gets a value indicating whether Is Content Package Based Title Installed On MU1
        /// </summary>
        bool IsContentPackageBasedTitleInstalledOnMU1 { get; }

        /// <summary>
        /// Gets a value indicating whether Is Content Package Based Title Installed On INT USB
        /// </summary>
        bool IsContentPackageBasedTitleInstalledOnINTUSB { get; }

        /// <summary>
        /// Gets a value indicating whether Is Content Package Based Title Installed On MU INT
        /// </summary>
        bool IsContentPackageBasedTitleInstalledOnMUINT { get; }

        /// <summary>
        /// Gets a value indicating whether Is Content Package Based Title Installed On USB 0
        /// </summary>
        bool IsContentPackageBasedTitleInstalledOnUSB0 { get; }

        /// <summary>
        /// Gets a value indicating whether Is Content Package Based Title Installed On USB 1
        /// </summary>
        bool IsContentPackageBasedTitleInstalledOnUSB1 { get; }

        /// <summary>
        /// Gets a value indicating whether the raw title is installed
        /// </summary>
        bool IsRawTitleInstalled { get; }

        /// <summary>
        /// Gets a value indicating whether the "Game Disc Emulator" Xbox development kit utility is running
        /// </summary>
        bool IsDiscEmulationRunning { get; }

        /// <summary>
        /// Gets a value indicating the Title ID
        /// </summary>
        string TitleId { get; }

        /// <summary>
        /// Gets a value indicating whether this current Xbox development kit instance in the default
        /// </summary>
        bool IsDefault { get; }

        /// <summary>
        /// Gets the current primary drive of the Xbox
        /// </summary>
        string PrimaryDrive { get; }

        /// <summary>
        /// Gets any drive identification that the content package based game title is installed on
        /// </summary>
        /// <returns>Any drive identification that content package based title is installed on</returns>
        string GetAnyDriveContentPackageBasedTitleIsInstalledOn { get; }

        /// <summary>
        /// Returns a value indicating whether the current Xbox is verified online
        /// </summary>
        /// <returns>true if the xbox is still online, false is not online</returns>
        bool VerifyOnline();

        /// <summary>
        /// Returns a value indication whether Content Package is installed based on the current Game Title
        /// </summary>
        /// <param name="drive">Drive identification to determine if content package is installed base on title</param>
        /// <returns>True if the content package is installed based on title, other wise false</returns>
        bool IsContentPackageBasedTitleInstalledOn(string drive);

        /// <summary>
        /// Capture a screen snapshot and store it in the module's log output directory
        /// </summary>
        /// <param name="filePath">Path to the module's log output directory</param>
        /// <returns>True if the function succeeded, other wise false</returns>
        bool ScreenShot(string filePath, bool convertToJpeg = true);

        /// <summary>
        /// Set the hard disk of the Xbox development kit as enabled or disabled based on the enable parameter.
        /// </summary>
        /// <param name="enable">Pass true to enable hard disk drive, other wise pass false to disable</param>
        /// <param name="rebootOrReflashWhenDone">By default, reboots or will re-flash when done, other wise pass false to stop</param>
        /// <param name="reflashOnDisable">By default, will re-flash on disabled, other wise pass false to stop</param>
        void SetHDDEnabled(bool enable, bool rebootOrReflashWhenDone = true, bool reflashOnDisable = true);

        /// <summary>
        /// Flashes the Xbox
        /// </summary>
        /// <param name="installSEP">Pass true to install the SEP, other wise pass false to not install the SEP</param>
        /// <param name="unattended">True to run in unattended mode, false to display UI</param>
        void Flash(bool installSEP, bool unattended = false);

        /// <summary>
        /// Helper device function that activates the Friends Status Presence Screen, if only after the current selected
        /// profile of the Xbox Device this is called against is signed in. This operation makes use of a an IXboxAutomation
        /// compiled script to cause this required input sequence.
        /// </summary>
        /// <param name="profile">The current Console Profile that IXboxAutomation script is executing on.</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        bool OpenFriendPresenceScreen(ConsoleProfile profile);

        /// <summary>
        /// Helper device function that wakes up the Xbox development kits currently under test by moving the right thumb arrow
        /// key to the right one time. This operation makes use of a an IXboxAutomation compiled script to cause this
        /// required input sequence.
        /// </summary>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        bool WakeUpXboxDevkit();

        /// <summary>
        /// This function is an IXBoxAutomation utility function that accepts a script file name that specifies what
        /// IXBoxAutomation script to execute, and then runs it.
        /// Note: Scripts comments are acceptable, but must be as [//] double forward slash comment, and also must
        /// only begin any separate line to be ignored.
        /// </summary>
        /// <param name="scriptFileName">IXBoxAutomation script file name to execute</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        bool RunIXBoxAutomationScript(string scriptFileName);

        /// <summary>
        /// This function runs a script that lives in the standard CAT location
        /// </summary>
        /// <param name="scriptShortName">name of the script with no paths and no extensions</param>
        /// <returns>True if the operation succeeded, otherwise false.</returns>
        bool RunCatScript(string scriptShortName);

        /// <summary>
        /// Change the current Xbox console region
        /// </summary>
        /// <param name="region">Region to change to</param>
        /// <returns>true if successful, false on failure</returns>
        bool SetRegion(Region region);

        /// <summary>
        /// Accept an Xbox 360 drive name
        /// </summary>
        /// <param name="driveName">Name of the drive</param>
        /// <returns>Number of free bytes on the drive</returns>
        ulong GetFreeSpace(string driveName = "");

        /// <summary>
        /// Accept an Xbox 360 drive name
        /// </summary>
        /// <param name="driveName">Name of the drive</param>
        /// <returns>Total number of bytes on the drive</returns>
        ulong TotalSpace(string driveName = "");

        /// <summary>
        /// Fills the specified drive.
        /// </summary>
        /// <param name="freeBytes">Free space specified remaining in unsigned long bytes</param>
        /// <param name="drive">Drive identification through which to fill to specified capacity remaining.</param>
        /// <returns>True if the operation succeeded, other wise false</returns>
        bool FillDrive(ulong freeBytes = 0, string drive = "");

        /// <summary>
        /// Adds 1KB to the specified drive through string drive parameter.
        /// </summary>
        /// <param name="drive">Drive identification through which to add 1KB to drive</param>
        /// <returns>True if the operation succeeded, other wise false</returns>
        bool Add1KBToDrive(string drive = "");

        /// <summary>
        /// Determines whether or not the specified drive has System Extended Packages installed.
        /// </summary>
        /// <param name="drive">Drive identification through which to determine if SEP is installed.</param>
        /// <returns>True the drive has SEP installed, other wise false</returns>
        bool HasSystemExtendedPackage(string drive = "");

        /// <summary>
        /// Stops running the "Game Disc Emulator" Xbox development kit utility
        /// </summary>
        void StopEmulatingDiscTitle();

        /// <summary>
        /// Executes installing the game title to the specified drive
        /// </summary>
        /// <param name="drive">Drive identification for which to install the game title</param>
        /// <param name="progressBar">Progress instance to show install progress, other wise null by default</param>
        /// <returns>True if the operation succeeded, other wise false</returns>
        bool InstallTitle(string drive = "", IProgressBar progressBar = null);

        /// <summary>
        /// Executes launching the game title
        /// </summary>
        /// <returns>True if the operation succeeded, other wise false</returns>
        bool LaunchTitle();

        /// <summary>
        /// Executes launching the game title from the specified drive
        /// </summary>
        /// <param name="drive">Drive identification from which to launch the specified game title</param>
        /// <returns>True if the operation succeeded, other wise false</returns>
        bool LaunchTitle(string drive = "");

        /// <summary>
        /// Executes launching the Xbox development kit's dash board
        /// </summary>
        /// <param name="wait">If true, a delay is used after launching the developer dashboard to entire it's up before continuing</param>
        void LaunchDevDashboard(bool wait = true);

        /// <summary>
        /// Performs a cold reboot of the Xbox.
        /// Only pass false for wait if there is a subsequent call to WaitForRebootToComplete().
        /// The call to WaitForRebootToComplete() is required to reestablish various an XboxConsole object and breakpoints
        /// To reboot asynchronously, run Reboot(true) in a thread.
        /// </summary>
        /// <param name="wait">By default true to wait for the Xbox to finish rebooting, other wise pass false to not wait</param>
        void Reboot(bool wait = true);

        /// <summary>
        /// Waits for the device to become responsive after a reboot.
        /// Only one call to WaitForRebootToComplete() are valid to be outstanding against the same Xbox at any given time.
        /// </summary>
        void WaitForRebootToComplete();

        /// <summary>
        /// Sends the specified file to the xbox
        /// </summary>
        /// <param name="sourcePath">Path to file on PC to send</param>
        /// <param name="destPath">Path to write to on Xbox</param>
        /// <param name="progressBar">An optional progress bar to report progress of the file send operation</param>
        /// <returns>true if successful, false on failure</returns>
        bool SendFile(string sourcePath, string destPath, IProgressBar progressBar = null);

        /// <summary>
        /// Executes recursively copying the specified desktop directory to specified xbox directory
        /// </summary>
        /// <param name="desktopDir">Source directory from which to recursively copy</param>
        /// <param name="xboxDir">Target to which to recursively copy</param>
        /// <param name="progressBar">Progress instance to show recursive copy progress, other wise null by default</param>
        /// <returns>True if the operation succeeded, other wise false</returns>
        bool RecursiveDirectoryCopy(string desktopDir, string xboxDir, IProgressBar progressBar = null);

        /// <summary>
        /// Execute recursively deleting the specified Xbox directory
        /// </summary>
        /// <param name="xboxDir">The specified Xbox directory for which to recursively delete</param>
        /// <param name="progressBar">Progress instance to show recursive copy progress, other wise null by default</param>
        /// <returns>True if the operation succeeded, other wise false</returns>
        bool RecursiveDirectoryDelete(string xboxDir, IProgressBar progressBar = null);

        /// <summary>
        /// Sets the language of this current Xbox development kit instance
        /// </summary>
        /// <param name="lang">Language for which to set this Xbox development kit to</param>
        /// <param name="rebootNow">By default true to reboot now, other wise pass false to stop reboot</param>
        void SetLanguage(Language lang, bool rebootNow = true);

        /// <summary>
        /// Sets the video mode of this current Xbox development kit instance according to the specified
        /// video resolution, video standard mode, and whether to reboot
        /// </summary>
        /// <param name="resolution">Video resolution for which to set</param>
        /// <param name="mode">Video standard mode for which to set</param>
        /// <param name="reboot">True or false to reboot</param>
        void SetVideoMode(VideoResolution resolution, VideoStandard mode, bool reboot);

        /// <summary>
        /// Gets the current resolution for this current Xbox development kit as out parameters
        /// for the width and the height
        /// </summary>
        /// <param name="width">Width of the current resolution</param>
        /// <param name="height">Height of the current resolution</param>
        void GetCurrentResolution(out int width, out int height);

        /// <summary>
        /// Delete all profiles from the specified drive
        /// </summary>
        /// <param name="drive">Drive to delete all profiles from</param>
        /// <returns>true if successful, false if something failed to delete</returns>
        bool DeleteAllProfilesFrom(string drive);

        /// <summary>
        /// Delete all profiles from all drives
        /// </summary>
        /// <returns>true if successful, false if something failed to delete</returns>
        bool DeleteAllProfiles();

        /// <summary>
        /// Delete all saves from the specified drive
        /// </summary>
        /// <param name="drive">Drive to delete all saves from</param>
        /// <returns>true if successful, false if something failed to delete</returns>
        bool DeleteAllSavesFrom(string drive);

        /// <summary>
        /// Delete all saves from all drives
        /// </summary>
        /// <returns>true if successful, false if something failed to delete</returns>
        bool DeleteAllSaves();

        /// <summary>
        /// Delete all title saves from the specified drive
        /// </summary>
        /// <param name="drive">Drive to delete all title saves from</param>
        /// <returns>true if successful, false if something failed to delete</returns>
        bool DeleteAllTitleSavesFrom(string drive);

        /// <summary>
        /// Deletes all games from the specified drive
        /// </summary>
        /// <param name="drive">Drive to delete all games from</param>
        /// <returns>true if successful, false if something failed to delete</returns>
        bool DeleteAllGamesFrom(string drive);

        /// <summary>
        /// Deletes all games from all drives
        /// </summary>
        /// <returns>true if successful, false if something failed to delete</returns>
        bool DeleteAllGames();

        /// <summary>
        /// Connects debugging capabilities to the Xbox.
        /// This will fail if the Xbox is Test kit, or not running.
        /// This is intended to be called from a module (or from another call made by a module).
        /// The debugger will be automatically disconnected when the module stops.
        /// Implements ConnectDebugger in IXboxDevice.
        /// </summary>
        /// <param name="force">Whether or not for force connection to the debugger if already connected to by someone else</param>
        /// <returns>true if successfully connected, false if not</returns>
        bool ConnectDebugger(bool force = false);

        /// <summary>
        /// Disconnect only the debugger, but do not disconnect the Xbox.
        /// If the debugger is subsequently connected again before disconnect is complete,
        /// the disconnect attempt will be aborted.
        /// </summary>
        void DisconnectDebugger();

        /// <summary>
        /// Enables monitoring of API calls in the currently running title, given a symbol name.
        /// The debugger must already be connected.
        /// </summary>
        /// <param name="symbolName">symbol name of a function to monitor</param>
        /// <param name="d">Delegate to invoke when API call is detected</param>
        /// <returns>A IMonitorAPISession representing the registered symbol</returns>
        IMonitorAPISession MonitorAPI(string symbolName, MonitorAPIDelegate d);

        /// <summary>
        /// Defer subsequent attempts to disconnect
        /// </summary>
        void DeferDisconnect();

        /// <summary>
        /// Remove a disconnect deferral
        /// </summary>
        void AllowDisconnect();
    }
}