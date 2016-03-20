// -----------------------------------------------------------------------
// <copyright file="STR050CTC2.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace STR050
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using CAT;
    using Microsoft.Test.Xbox.Profiles;
    using XDevkit;

    /// <summary>
    /// Implementation class of the STR050CTC2 Module
    /// </summary>
    public class STR050CTC2 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// This module user interface reference
        /// </summary>
        private STR050UI moduleUI;

        /// <summary>
        /// String flag Passed or Failed
        /// </summary>
        private string passedOrFailed = "FAILED";
        
        /// <summary>
        /// Current working XboxDevice console instance
        /// </summary>
        private IXboxDevice xboxDevice;

        /// <summary>
        /// Current working module context
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// User interface instructions
        /// </summary>
        private string instructions;

        /// <summary>
        /// Save file location
        /// </summary>
        private string saveFile;

        /// <summary>
        /// Module 50 profile
        /// </summary>
        private ConsoleProfile mod50Profile;

        /// <summary>
        /// Low storage before files list
        /// </summary>
        private List<string> lowStorageBeforeFiles;

        /// <summary>
        /// Low storage after files list
        /// </summary>
        private List<string> lowStorageAfterFiles;

        /// <summary>
        /// First page visibility property
        /// </summary>
        private Visibility firstPageVisibility = Visibility.Visible;

        /// <summary>
        /// Second page visibility property
        /// </summary>
        private Visibility secondPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Advanced paged visibility property
        /// </summary>
        private Visibility advancedPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Begin button visibility property
        /// </summary>
        private Visibility beginButtonVisible = Visibility.Visible;

        /// <summary>
        /// Configure enabled visibility property
        /// </summary>
        private Visibility configureEnabledVisibility = Visibility.Visible;

        /// <summary>
        /// Low storage test visibility property
        /// </summary>
        private Visibility lowStorageTestVisibility = Visibility.Hidden;

        /// <summary>
        /// Next button visibility property
        /// </summary>
        private Visibility nextButtonVisibility = Visibility.Hidden;

        /// <summary>
        /// Check save button visibility property
        /// </summary>
        private Visibility checkSaveButtonVisibility = Visibility.Hidden;

        /// <summary>
        /// Low storage test running flag property
        /// </summary>
        private bool lowStorageTestRunning;

        /// <summary>
        /// Is next enabled flag property
        /// </summary>
        private bool isNextEnabled = true;

        /// <summary>
        /// Property changed event handler event member object
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets SaveSize property
        /// </summary>
        public ulong SaveSize { get; set; }

        /// <summary>
        /// Gets or sets SecondPageVisibility property
        /// </summary>
        public Visibility SecondPageVisibility
        {
            get
            {
                return this.secondPageVisibility;
            }

            set
            {
                this.secondPageVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets AdvancedPageVisibility property
        /// </summary>
        public Visibility AdvancedPageVisibility
        {
            get
            {
                return this.advancedPageVisibility;
            }

            set
            {
                this.advancedPageVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets Instructions property
        /// </summary>
        public string Instructions
        {
            get
            {
                return this.instructions;
            }

            set
            {
                this.instructions = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets BeginButtonVisible property
        /// </summary>
        public Visibility BeginButtonVisible
        {
            get
            {
                return this.beginButtonVisible;
            }

            set
            {
                this.beginButtonVisible = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets ConfigureEnabledVisibility property
        /// </summary>
        public Visibility ConfigureEnabledVisibility
        {
            get
            {
                return this.configureEnabledVisibility;
            }

            set
            {
                this.configureEnabledVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets LowStorageTestVisibility property
        /// </summary>
        public Visibility LowStorageTestVisibility
        {
            get
            {
                return this.lowStorageTestVisibility;
            }

            set
            {
                this.lowStorageTestVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets NextButtonVisibility property
        /// </summary>
        public Visibility NextButtonVisibility
        {
            get
            {
                return this.nextButtonVisibility;
            }

            set
            {
                this.nextButtonVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets CheckSaveButtonVisibility property
        /// </summary>
        public Visibility CheckSaveButtonVisibility
        {
            get
            {
                return this.checkSaveButtonVisibility;
            }

            set
            {
                this.checkSaveButtonVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Low Storage Test is Running
        /// </summary>
        public bool LowStorageTestRunning
        {
            get
            {
                return this.lowStorageTestRunning;
            }

            set
            {
                this.lowStorageTestRunning = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Is Next Enabled
        /// </summary>
        public bool IsNextEnabled
        {
            get
            {
                return this.isNextEnabled;
            }

            set
            {
                this.isNextEnabled = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets SaveFile property
        /// </summary>
        public string SaveFile
        {
            get
            {
                return this.saveFile;
            }

            set
            {
                this.saveFile = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets read only access for the current working module context of type IModuleContext.
        /// </summary>
        public IModuleContext ModuleContext
        {
            get { return this.moduleContext; }
        }

        /// <summary>
        /// Gets UIElement property
        /// </summary>
        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
        }

        /// <summary>
        /// Gets or sets FirstPageVisibility property
        /// </summary>
        public Visibility FirstPageVisibility
        {
            get
            {
                return this.firstPageVisibility;
            }

            set
            {
                this.firstPageVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Low Storage Flash is Done
        /// </summary>
        public bool LowStorage_FlashDone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Low Storage Title is Done
        /// </summary>
        public bool LowStorage_TitleDone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Low Storage Profile Create is Done
        /// </summary>
        public bool LowStorage_ProfileCreateDone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Low Storage Profile Save is Detected
        /// </summary>
        public bool LowStorage_ProfileSaveDetected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Low Storage Fill is Done
        /// </summary>
        public bool LowStorage_FillDone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Low Storage Delete Save File is Done
        /// </summary>
        public bool LowStorage_DeleteSaveFileDone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Low Storage Add 1KB is Done
        /// </summary>
        public bool LowStorage_Add1KBDone { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the HDD is currently enabled
        /// </summary>
        public bool IsHDDEnabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsHDDEnabled;
                }

                return result;
            }

            set
            {
                if (this.xboxDevice.IsHDDEnabled != value)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.xboxDevice.SetHDDEnabled(value, true, false);
                    this.RefreshStates();
                    if (value)
                    {
                        this.moduleContext.Log("Enabled Hard Drive");
                    }
                    else
                    {
                        this.moduleContext.Log("Disabled Hard Drive");
                    }

                    Mouse.OverrideCursor = null;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not MU0 is currently enabled
        /// </summary>
        public bool IsMU0Enabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsMU0Enabled;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not MU1 is currently enabled
        /// </summary>
        public bool IsMU1Enabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsMU1Enabled;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the Internal USB storage device is currently enabled
        /// </summary>
        public bool IsINTUSBEnabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsINTUSBEnabled;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Internal MU is currently enabled
        /// </summary>
        public bool IsMUINTEnabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsMUINTEnabled;
                }

                return result;
            }

            set
            {
                if (this.xboxDevice.IsMUINTEnabled != value)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.xboxDevice.IsMUINTEnabled = value;
                    this.RefreshStates();
                    if (value)
                    {
                        this.moduleContext.Log("Enabled Internal MU");
                    }
                    else
                    {
                        this.moduleContext.Log("Disabling Internal MU");
                    }

                    Mouse.OverrideCursor = null;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not USB0 is currently enabled
        /// </summary>
        public bool IsUSB0Enabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsUSB0Enabled;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not USB1 is currently enabled
        /// </summary>
        public bool IsUSB1Enabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsUSB1Enabled;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether
        /// </summary>
        public bool IsExternalStorageEnabled
        {
            get
            {
                return this.IsMU0Enabled || this.IsMU1Enabled || this.IsUSB0Enabled || this.IsUSB1Enabled;
            }
        }

        /// <summary>
        /// Gets a list of all files found in the content folder
        /// </summary>
        private List<string> AllContentFilesOnXbox
        {
            get
            {
                List<string> files = new List<string>();
                IXboxDevice xbox = this.SelectedXbox;
                if (xbox == null)
                {
                    return files;
                }

                foreach (string drivename in xbox.Drives)
                {
                    if (drivename.Contains("HDD") || drivename.Contains("MU") || drivename.Contains("USB"))
                    {
                        files.AddRange(this.GetDirectoryFiles(xbox.XboxConsole, drivename + ":\\Content"));
                    }
                }

                return files;
            }
        }

        /// <summary>
        /// Gets the number of profiles found on the current working XboxConsole
        /// </summary>
        private int NumberOfProfiles
        {
            get
            {
                try
                {
                    ConsoleProfilesManager profilesManager = this.SelectedXbox.XboxConsole.CreateConsoleProfilesManager();
                    return profilesManager.EnumerateConsoleProfiles().Count();
                }
                catch
                {
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is only one xbox selected
        /// </summary>
        private bool IsOneConnectedXboxSelected
        {
            get
            {
                string s = string.Empty;
                if (this.moduleContext.SelectedDevices.Count() == 0)
                { // at least one
                    s += "No consoles are selected. Select one. ";
                }
                else if (this.moduleContext.SelectedDevices.Count() > 1)
                {
                    // only one
                    s += this.moduleContext.SelectedDevices.Count().ToString() + " consoles are selected. Select just one. ";
                }

                foreach (IXboxDevice device in this.moduleContext.SelectedDevices)
                {
                    if (device.IsSelected)
                    {
                        // connected
                        if (device.Connected == false)
                        {
                            s += "The selected device " + device.Name + " is not connected. Connect the device.";
                        }

                        break;
                    }
                }

                // if there were any error messages, fail
                if (!string.IsNullOrEmpty(s))
                {
                    MessageBox.Show(s, "Certification Assistance Tool");
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the first found to be selected XBox device for flashing, and places it in the
        /// xboxDevice data member.
        /// </summary>
        private IXboxDevice SelectedXbox
        {
            get
            {
                List<IDevice> xboxDevices = this.moduleContext.SelectedDevices;
                foreach (IDevice xboxdevice in xboxDevices)
                {
                    if (true == xboxdevice.IsSelected)
                    {
                        this.xboxDevice = xboxdevice as IXboxDevice;
                        return this.xboxDevice;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Utility/helper function that start the Begin Low Storage Test
        /// </summary>
        public void BeginLowStorage()
        {
            // check prerequisites
            if (!File.Exists(this.moduleContext.XboxTitle.XdkRecoveryPath))
            {
                MessageBox.Show("No Flash Recovery found.\n\nPlease open Settings and set the XDKRecovery executable.\n    Hint : A recovery usually looks like XDKRecoveryXenon[version].exe", "Certification Assistance Tool");
                return;
            }

            if (!this.xboxDevice.IsTitleInstalled && !this.xboxDevice.CanInstallTitle)
            {
                MessageBox.Show("Title cannot be installed as configured.\n\nPlease open Settings to fix the problem.\n", "Certification Assistance Tool");
                return;
            }

            this.ConfigureEnabledVisibility = Visibility.Hidden;
            this.BeginButtonVisible = Visibility.Collapsed;
            this.LowStorage_FlashDone = false;
            this.LowStorage_TitleDone = false;
            this.LowStorage_ProfileCreateDone = false;
            this.LowStorage_ProfileSaveDetected = false;
            this.LowStorage_FillDone = false;
            this.LowStorage_DeleteSaveFileDone = false;
            this.LowStorage_Add1KBDone = false;
            this.LowStorageTestVisibility = Visibility.Visible;
            this.NextButtonVisibility = Visibility.Visible;
            this.LowStorageTestRunning = true;

            this.Instructions = "Current Storage Devices: ";

            foreach (string drive in this.xboxDevice.Drives)
            {
                if (drive.Contains("HDD") || drive.Contains("USB") || drive.Contains("MU"))
                {
                    this.Instructions += drive + ", ";
                }
            }

            this.Instructions += Environment.NewLine;
            this.Instructions += Environment.NewLine;
            this.Instructions += "When you click NEXT, we will begin the low-storage test.";
        }

        /// <summary>
        /// Wizard to walk through low-storage test. Re-entrant.
        /// </summary>
        public void NextLowStorage()
        {
            // Step 1. Flash the Xbox, if necessary
            if (!this.LowStorage_FlashDone)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.IsNextEnabled = false;

                string msg = string.Empty;
                foreach (string drive in this.xboxDevice.Drives)
                {
                    if (drive.Contains("HDD") || drive.Contains("USB") || drive.Contains("MU"))
                    {
                        if (this.xboxDevice.HasSystemExtendedPackage(drive))
                        {
                            msg += "Storage Device " + drive + " appears to be ready to begin the test." + Environment.NewLine
                                   + "You may choose to flash now by pressing YES below. Otherwise, press NO to continue with the test.";
                        }
                        else
                        {
                            msg += "Drive " + drive + " may not have been flashed";
                        }
                    }
                }

                msg += Environment.NewLine + Environment.NewLine + "Flash drive?";
                switch (MessageBox.Show(msg, "Certification Assistance Tool", MessageBoxButton.YesNo))
                {
                    case MessageBoxResult.Yes:
                        this.Instructions = "Getting Console Ready" + Environment.NewLine + Environment.NewLine +
                            "Flashing devkit.  Please avoid interacting with devkit until flashing is complete. " + Environment.NewLine + Environment.NewLine +
                            "This may take a few minutes, please be patient.";
                        this.NotifyPropertyChanged("Instructions");
                        this.UpdateUIImmediately();
                        this.xboxDevice.Flash(true);
                        this.moduleContext.Log(this.xboxDevice.Name + " flashed.");
                        break;
                    default: this.moduleContext.Log(this.xboxDevice.Name + " flash skipped.");
                        break;
                }

                // clean any leftover files. should only happen if test was stopped in debugging mode
                this.CleanupCatLitter();

                this.LowStorage_FlashDone = true;
                this.SaveFile = string.Empty;
                this.Instructions = "Installing " + this.moduleContext.XboxTitle.Name;
                this.NotifyPropertyChanged("LowStorage_FlashDone");
                this.NotifyPropertyChanged("Instructions");
                this.UpdateUIImmediately();
            }

            // Step 2. Install the Title (don't pause or ask user)
            if (!this.LowStorage_TitleDone)
            {
                this.xboxDevice.LaunchDevDashboard();
                Thread.Sleep(2000);
                foreach (string drive in this.xboxDevice.Drives)
                {
                    if (drive.Contains("HDD") || drive.Contains("USB") || drive.Contains("MU"))
                    {
                        if (this.xboxDevice.IsTitleInstalled)
                        { // install on first available device
                            if (MessageBoxResult.No == MessageBox.Show(
                                this.moduleContext.XboxTitle.Name + " is already installed.\n\nReinstall to drive " + drive + "?",
                                "Certification Assistance Tool",
                                MessageBoxButton.YesNo))
                            {
                                this.moduleContext.Log(this.moduleContext.XboxTitle.Name + " install skipped.");
                                break;
                            }
                        }

                        this.xboxDevice.InstallTitle(drive, this.moduleContext.OpenProgressBarWindow("Installing " + this.moduleContext.XboxTitle.Name + "..."));
                        this.moduleContext.Log(this.moduleContext.XboxTitle.Name + " installed to drive " + drive);
                        break;
                    }
                }

                if (this.xboxDevice.IsTitleInstalled)
                {
                    this.LowStorage_TitleDone = true;
                }

                // name = Environment.GetEnvironmentVariable("USERNAME") + DateTime.Now.ToString("MMddyy");
                // name = new string(name.Where(c => @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".Contains(c)).ToArray());
                // Instructions = "Creating Profile " +name;
                this.Instructions = "Creating Profile ";
                this.NotifyPropertyChanged("LowStorage_TitleDone");
                this.NotifyPropertyChanged("Instructions");
                this.UpdateUIImmediately();
            }

            // Step 3. Create and sign-in a profile (don't pause or ask user)
            if (!this.LowStorage_ProfileCreateDone)
            {
                if (this.NumberOfProfiles >= 8)
                {
                    if (MessageBoxResult.Yes == MessageBox.Show(
                        "There may not be enough room to create a new profile. To fix this, use your controller to delete profiles until fewer than 8 remain.\n\nDo you want me to wait while you delete some profiles?",
                        "Certification Assistance Tool",
                        MessageBoxButton.YesNo))
                    {
                        return;
                    }
                }

                this.CreateProfile(string.Empty);

                // check profile creation succeeded
                if (this.mod50Profile == null)
                {
                    return;
                }

                // check what gamertag was actually created
                if (this.mod50Profile != null)
                {
                    this.LowStorage_ProfileCreateDone = true;
                    this.moduleContext.Log("Profile with gamertag " + this.mod50Profile.Gamertag + " created.");
                    this.mod50Profile.SignIn(UserIndex.Zero);
                    this.moduleContext.Log("Profile with gamertag " + this.mod50Profile.Gamertag + " signed-in.");
                    Thread.Sleep(5000);    // Wait for sign in to complete
                }

                // get snapshot of all content files on Xbox
                this.lowStorageBeforeFiles = this.AllContentFilesOnXbox;
                this.moduleContext.Log("Content files detected on console before first save:");
                this.moduleContext.Log(string.Join(Environment.NewLine, this.lowStorageBeforeFiles.ToArray()));

                this.CheckSaveButtonVisibility = Visibility.Visible;
                this.NextButtonVisibility = Visibility.Collapsed;
                this.Instructions = "Alright! The title is installed and we've created a fresh profile and signed in for you. " + Environment.NewLine + Environment.NewLine +
                    "Use profile " + this.mod50Profile.Gamertag + " to enter saveable gameplay in " + this.moduleContext.XboxTitle.Name + "." +
                    Environment.NewLine + Environment.NewLine + "\tSave a game." +
                    Environment.NewLine + Environment.NewLine + "After the game is saved, click CHECK FOR SAVE and we will attempt to locate it.";
                this.NotifyPropertyChanged("LowStorage_ProfileCreateDone");
                this.NotifyPropertyChanged("Instructions");
                this.NotifyPropertyChanged("CheckSaveButtonVisibility");
                this.NotifyPropertyChanged("NextButtonVisibility");
                this.UpdateUIImmediately();
                Mouse.OverrideCursor = null;
                this.IsNextEnabled = true;

                // pause for user to hit Next
                return;
            }

            // Step 4. Detect file save
            if (!this.LowStorage_ProfileSaveDetected)
            {
                this.lowStorageAfterFiles = this.AllContentFilesOnXbox;
                this.moduleContext.Log("Content files detected on console:");
                this.moduleContext.Log(string.Join(Environment.NewLine, this.lowStorageBeforeFiles.ToArray()));

                List<string> changes = this.DetectNewFiles(this.lowStorageAfterFiles, this.lowStorageBeforeFiles);

                // check for at least one new file
                if (changes.Count == 0)
                {
                    this.Instructions = "We aren't detecting a new save! " + Environment.NewLine + Environment.NewLine +
                        " Note: It's possible that the title is saving to a non-standard location. " + Environment.NewLine + Environment.NewLine +
                        "You can click DONE to review the log now for these types of files. " + Environment.NewLine + Environment.NewLine +
                        "Otherwise, you can attempt to create a save again and click CHECK FOR SAVE. ";
                    return;
                }

                // check this file could belong to the current title
                this.Instructions = string.Empty;
                for (int i = 0; i < changes.Count; i++)
                {
                    string profileUID = this.mod50Profile.OfflineXuid.ToString().Substring(5);
                    if (changes[i].Contains(this.xboxDevice.TitleId) && changes[i].Contains(profileUID))
                    {
                        if (string.IsNullOrEmpty(this.SaveFile))
                        {
                            this.SaveFile = changes[i];
                            this.SaveSize = this.xboxDevice.XboxConsole.GetFileObject(this.SaveFile).Size;
                            this.Instructions += "Detected a save file for " + this.moduleContext.XboxTitle.Name +
                                                 " on profile " + profileUID + ". File size is " + (this.SaveSize / 1024) + " kilobytes." + Environment.NewLine + "\t" + changes[i]
                                                 + Environment.NewLine + Environment.NewLine +
                                                 "When you click NEXT, we will: fill the storage device, remove the file save, and add another" +
                                                 "1kB of data to the storage device." + Environment.NewLine + Environment.NewLine +
                                                 "We will then prompt you to once again attempt to create the file save." + Environment.NewLine +
                                                 "Please be patient, this may take a few minutes.";
                            this.CheckSaveButtonVisibility = Visibility.Collapsed;
                            this.NextButtonVisibility = Visibility.Visible;
                            this.LowStorage_ProfileSaveDetected = true;
                            this.moduleContext.Log(this.moduleContext.XboxTitle.Name + " save file detected at path " + this.SaveFile);
                            this.moduleContext.Log("Save file size = " + this.SaveSize + " bytes");
                        }
                        else
                        {
                            this.Instructions += Environment.NewLine + "WARNING : found more than one save file. Only one save file is expected.";
                            this.Instructions += " Extra file found: " + changes[i] + Environment.NewLine;
                            this.moduleContext.Log("WARNING : found more than one save file. Only one save file is expected.");
                            this.moduleContext.Log(" Extra file found: " + changes[i]);
                        }
                    }
                    else
                    {
                        this.Instructions += "This file is new but may not be related to " + this.moduleContext.XboxTitle.Name + ": " + changes[i] + Environment.NewLine;
                    }
                }

                if (!this.LowStorage_ProfileSaveDetected)
                {
                    return;
                }

                this.Instructions += "Click Next to continue. The drive will be filled with not quite enough space for a save game.";
                this.IsNextEnabled = true;
                this.NotifyPropertyChanged("LowStorage_ProfileSaveDetected");
                this.NotifyPropertyChanged("Instructions");
                this.UpdateUIImmediately();
                Mouse.OverrideCursor = null;

                // pause for user to hit Next
                return;
            }

            // Step 5. Fill the drive with the profile (all drives, for now)
            if (!this.LowStorage_FillDone)
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.Instructions = "Filling drive. Please be patient, this may take a few minutes.";
                this.NotifyPropertyChanged("Instructions");
                this.UpdateUIImmediately();

                try
                {
                    this.mod50Profile.SignOut();
                    this.xboxDevice.LaunchDevDashboard();
                }
                catch
                {
                }

                foreach (string drive in this.xboxDevice.Drives)
                {
                    if (drive.Contains("HDD") || drive.Contains("MU") || drive.Contains("USB"))
                    {
                        try
                        {
                            // fill drive.
                            this.xboxDevice.FillDrive(0, drive);
                            this.LowStorage_FillDone = true;
                            this.moduleContext.Log(this.moduleContext.XboxTitle.Name + " drive " + drive + " filled");
                        }
                        catch
                        {
                        }
                    }
                }
            }

            // Step 6. Delete the save file
            if (!this.LowStorage_DeleteSaveFileDone)
            {
                this.xboxDevice.XboxConsole.DeleteFile(this.SaveFile);
                this.LowStorage_DeleteSaveFileDone = true;
                string sizeRemoved;
                try
                {
                    sizeRemoved = (this.SaveSize / 1024).ToString() + " kilobytes";
                }
                catch
                {
                    sizeRemoved = this.SaveSize.ToString() + " bytes";
                }

                this.moduleContext.Log("Save file " + this.SaveFile + " deleted.");
                this.moduleContext.Log("Removed " + sizeRemoved);
            }

            // Step 7. add 1kb to the drive
            if (!this.LowStorage_Add1KBDone)
            {
                string driveRoot = this.SaveFile.Substring(0, this.SaveFile.IndexOf(':'));

                // xboxDevice.FillDrive(SaveSize - 1024, driveRoot);
                this.xboxDevice.Add1KBToDrive(driveRoot); // add 1kb
                this.LowStorage_Add1KBDone = true;

                string sizeRemaining;
                try
                {
                    sizeRemaining = (this.xboxDevice.GetFreeSpace(driveRoot) / 1024).ToString() + " kilobytes";
                }
                catch
                {
                    sizeRemaining = this.xboxDevice.GetFreeSpace(driveRoot).ToString() + " bytes.";
                }

                this.moduleContext.Log("One KB (1024 bytes) added to drive" + driveRoot);
                this.moduleContext.Log("Free space: " + sizeRemaining);

                this.mod50Profile.SignIn(UserIndex.Zero);
                this.moduleContext.Log(this.mod50Profile.Gamertag + " signed-in");

                this.Instructions = "Console " + this.xboxDevice.Name + " has low storage, meaning less than " + (this.SaveSize / 1024).ToString() +
                    " kilobytes." + Environment.NewLine + Environment.NewLine + "Using profile " + this.mod50Profile.Gamertag +
                    ", enter saveable gameplay in " + this.moduleContext.XboxTitle.Name
                    + " and attempt to save a game. Expected (passing) result is that the game is unable to save and tells you so." +
                    Environment.NewLine + Environment.NewLine + "This is the end of the low-storage test.";
                this.NextButtonVisibility = Visibility.Collapsed;
                this.NotifyPropertyChanged("LowStorage_FillDone");
                this.NotifyPropertyChanged("Instructions");
                this.UpdateUIImmediately();
                this.passedOrFailed = "FINISHED";
            }

            if (!this.LowStorage_Add1KBDone)
            {
                this.IsNextEnabled = true;
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// This helper function that has no arguments or return type is the main entry point that
        /// is called immediately before the Start( IModuleContext context ) function is called that
        /// starts the tool running for the execution of this unit test CTC module.
        /// </summary>
        public void NextPage()
        {
            if (!this.IsOneConnectedXboxSelected)
            {
                return;
            }

            this.xboxDevice = this.SelectedXbox;

            if (this.moduleContext.XboxTitle.GameInstallType == "Raw")
            {
                MessageBox.Show(
                    "Raw Game Titles are not supported by this module",
                    "Raw Titles not support",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            if (null == this.xboxDevice)
            {
                MessageBox.Show(
                    "No appropriate selected Xbox Device was found to reflash to reinstall " +
                    "System Extended Packages.",
                    "Selection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            if (false == this.xboxDevice.Connected)
            {
                MessageBox.Show(
                    "No Xbox Device is connected to your XBox Neighborhood.\nPlease recheck your connection(s).",
                    "Connection Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            this.FirstPageVisibility = Visibility.Collapsed;
            this.SecondPageVisibility = Visibility.Visible;
            this.moduleContext.IsModal = true;

            this.RefreshStates();

            this.Instructions = "Use the controls above to set the devkit to the configuration you wish to test. Use the Auto-Configuration buttons below to quickly reach USB-only or MU-only configuration. \n\n\tWhen you have are ready to start the test, hit Begin.";
        }

        /// <summary>
        /// Test execute implementation accepted from IModule interface.
        /// This function is responsible for performing the test execution.
        /// </summary>
        /// <param name="ctx">The current working context for which this test will execute.</param>
        public void Start(IModuleContext ctx)
        {
            this.moduleContext = ctx as IXboxModuleContext;
            this.moduleUI = new STR050UI(this);
        }

        /// <summary>
        /// Utility/helper function that Sets up external USB only
        /// </summary>
        public void SetupExternalUSBOnly()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            for (;;)
            {
                if (!this.IsUSB0Enabled && !this.IsUSB1Enabled)
                {
                    MessageBoxResult result = MessageBox.Show("Please insert a USB drive.", "Insert USB drive", MessageBoxButton.OKCancel, MessageBoxImage.None);
                    if (result == MessageBoxResult.Cancel)
                    {
                        break;
                    }

                    this.RefreshStates();
                    continue;
                }

                if (this.IsUSB0Enabled && this.IsUSB1Enabled)
                {
                    MessageBoxResult result = MessageBox.Show("Multiple USB drives detected.  Please remove one.", "Too many USB drives", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    if (result == MessageBoxResult.Cancel)
                    {
                        break;
                    }

                    this.RefreshStates();
                    continue;
                }

                if (this.IsMU0Enabled || this.IsMU1Enabled)
                {
                    MessageBoxResult result = MessageBox.Show("External MU detected.  Please remove it.", "Remove External MU", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    if (result == MessageBoxResult.Cancel)
                    {
                        break;
                    }

                    this.RefreshStates();
                    continue;
                }

                if (this.IsHDDEnabled)
                {
                    this.xboxDevice.SetHDDEnabled(false, !this.IsMUINTEnabled, false);   // don't reboot if external MU change would force a reboot anyway
                    this.RefreshStates();
                }

                if (this.IsMUINTEnabled)
                {
                    this.xboxDevice.IsMUINTEnabled = false;
                    this.RefreshStates();
                }

                break;
            }

            this.moduleContext.Log("Set drive configuration to External USB Only");
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Utility/helper function that Sets up external MU only
        /// </summary>
        public void SetupExternalMUOnly()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            for (;;)
            {
                if (!this.IsMU0Enabled && !this.IsMU1Enabled)
                {
                    MessageBoxResult result = MessageBox.Show("Please insert an external MU.", "Insert External MU", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    if (result == MessageBoxResult.Cancel)
                    {
                        break;
                    }

                    this.RefreshStates();
                    continue;
                }

                if (this.IsMU0Enabled && this.IsMU1Enabled)
                {
                    MessageBoxResult result = MessageBox.Show("Multiple external MU's detected.  Please remove one.", "Too many External MU's", MessageBoxButton.OKCancel, MessageBoxImage.None);
                    if (result == MessageBoxResult.Cancel)
                    {
                        break;
                    }

                    this.RefreshStates();
                    continue;
                }

                if (this.IsUSB0Enabled || this.IsUSB1Enabled)
                {
                    MessageBoxResult result = MessageBox.Show("External USB detected.  Please remove it.", "Remove USB drive", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    if (result == MessageBoxResult.Cancel)
                    {
                        break;
                    }

                    this.RefreshStates();
                    continue;
                }

                if (this.IsHDDEnabled)
                {
                    this.xboxDevice.SetHDDEnabled(false, !this.IsMUINTEnabled, false);   // don't reboot if external MU change would force a reboot anyway
                    this.RefreshStates();
                }

                if (this.IsMUINTEnabled)
                {
                    this.xboxDevice.IsMUINTEnabled = false;
                    this.RefreshStates();
                }

                break;
            }

            this.RefreshStates();
            this.moduleContext.Log("Set drive configuration to External MU Only");
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Refreshes drive and device property states
        /// </summary>
        public void RefreshStates()
        {
            this.NotifyPropertyChanged("IsHDDEnabled");
            this.NotifyPropertyChanged("IsMU0Enabled");
            this.NotifyPropertyChanged("IsMU1Enabled");
            this.NotifyPropertyChanged("IsINTUSBEnabled");
            this.NotifyPropertyChanged("IsMUINTEnabled");
            this.NotifyPropertyChanged("IsUSB0Enabled");
            this.NotifyPropertyChanged("IsUSB1Enabled");

            // push UI update
            Dispatcher dispatcher = this.moduleUI.Dispatcher;
            if (dispatcher != null)
            {
                dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);
            }
        }

        /// <summary>
        /// Stops test, and uninitializes/cleans up module test run inherited from IModule
        /// </summary>
        public void Stop()
        {
            try
            {
                this.CleanupCatLitter();
                this.DeleteProfile();
            }
            catch
            {
            }

            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("RESULT: " + this.passedOrFailed + "\r\n");
            this.moduleContext.Log("*************************************************************\r\n");
        }

        /// <summary>
        /// Creates the profile to be used to save games
        /// </summary>
        /// <param name="name">Name of the profile for which to save with</param>
        /// <returns>True if the operation succeeded, other wise false</returns>
        private bool CreateProfile(string name)
        {
            bool done = false;
            bool result = false;
            while (!done)
            {
                try
                {
                    ConsoleProfilesManager profilesManager = this.SelectedXbox.XboxConsole.CreateConsoleProfilesManager();
                    if (string.IsNullOrEmpty(name))
                    {
                        this.mod50Profile = profilesManager.CreateConsoleProfile(true);
                    }
                    else
                    {
                        this.mod50Profile = profilesManager.CreateConsoleProfile(true, XboxLiveCountry.UnitedStates, SubscriptionTier.Gold, name);
                    }

                    done = true;
                    result = true;
                }
                catch (Exception e)
                {
                    string more = string.Empty;
                    if ((uint)e.HResult == 0x80070070)
                    {
                        more = ".\n\nXbox drive is full.";
                    }

                    // if there was an error, option to try again
                    if (MessageBoxResult.Yes ==
                        MessageBox.Show(
                            "There was a problem creating a profile: " + e.Message + more + "\n\nTry Again?",
                            "Certification Assistance Tool",
                            MessageBoxButton.YesNo))
                    {
                        continue;
                    }
                    else
                    {
                        done = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Deletes the current working module profile
        /// </summary>
        /// <returns>True if the operation succeeded, other wise false</returns>
        private bool DeleteProfile()
        {
            if (this.mod50Profile == null)
            {
                return false;
            }

            try
            {
                ConsoleProfilesManager profilesManager = this.SelectedXbox.XboxConsole.CreateConsoleProfilesManager();
                profilesManager.DeleteConsoleProfile(this.mod50Profile);
            }
            catch (Exception)
            {
                // MessageBox.Show("There was a problem deleting profile " + mod50Profile.Gamertag + ": " + e.Message, "Certification Assistance Tool");
                return false;
            }

            return true;
        }

        /// <summary>
        /// NotifyPropertyChanged triggers the PropertyChanged event for the specified property
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// UpdateUIImmediately - push pending user interface updates
        /// </summary>
        private void UpdateUIImmediately()
        {
            try
            {
                // try to update UI immediately
                Dispatcher dispatcher = this.moduleUI.Dispatcher;
                if (dispatcher != null)
                {
                    dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);
                }
            }
            catch
            {
                // exceptions do not matter, the UI will update eventually regardless
            }
        }

        /// <summary>
        /// From the specified XBox Console and Drive name, this utility/helper function returns a List
        /// structures of the found directory files.
        /// </summary>
        /// <param name="console">XBoxConsole of which to get files</param>
        /// <param name="drivename">Drive name from which to get those files</param>
        /// <returns>A List of the directory structure found</returns>
        private List<string> GetDirectoryFiles(XboxConsole console, string drivename)
        {
            string s;
            List<string> files = new List<string>();

            try
            {
                foreach (IXboxFile xfile in console.DirectoryFiles(drivename))
                {
                    s = xfile.Name;
                    if (xfile.IsDirectory)
                    {
                        files.AddRange(this.GetDirectoryFiles(console, xfile.Name));
                    }
                    else
                    {
                        files.Add(xfile.Name);
                    }
                }
            }
            catch
            {
            }

            return files;
        }

        /// <summary>
        /// From the specified List after and List before, this utility/helper function
        /// returns a List of detected new files from the two specified lists.
        /// </summary>
        /// <param name="after">List of files before to be compared</param>
        /// <param name="before">List of files after to be compared</param>
        /// <returns>List of newly detected files that is the difference between the two specified lists</returns>
        private List<string> DetectNewFiles(List<string> after, List<string> before)
        {
            List<string> files = new List<string>();
            int count = 0;

            foreach (string file in after)
            {
                if (!before.Contains(file))
                {
                    count++;
                    files.Add(file);
                }
            }

            return files;
        }

        /// <summary>
        /// Removes temporary fill files from the Xbox
        /// </summary>
        /// <returns>True if the operation succeeded, other wise false</returns>
        private bool CleanupCatLitter()
        {
            try
            {
                IXboxFiles files;
                string filename;
                foreach (string drivename in this.SelectedXbox.Drives)
                {
                    if (drivename.Contains("HDD") || drivename.Contains("MU") || drivename.Contains("USB"))
                    {
                        files = this.SelectedXbox.XboxConsole.DirectoryFiles(drivename + ":\\");
                        foreach (IXboxFile file in files)
                        {
                            filename = file.Name;
                            if (filename.Contains("junk_") && filename.EndsWith(".cat"))
                            {
                                this.SelectedXbox.XboxConsole.DeleteFile(file.Name);
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes a single file from the Xbox
        /// </summary>
        /// <param name="filename">The specified file to remove</param>
        /// <returns>True if the operation succeeded, other wise false</returns>
        private bool RemoveFile(string filename)
        {
            bool result = true;
            try
            {
                this.SelectedXbox.XboxConsole.DeleteFile(filename);
            }
            catch
            {
                result = false;
            }

            return result;
        }
    } // End of class STR050CTC2 : IModule, INotifyPropertyChanged {}
} // End of namespace STR050 {}