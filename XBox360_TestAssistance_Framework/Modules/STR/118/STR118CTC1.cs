// -----------------------------------------------------------------------
// <copyright file="STR118CTC1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace STR118
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
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
    /// STR 118 Module
    /// </summary>
    public class STR118CTC1 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// Member for working with the current working ModuleContext after Start() is called
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// Private STR118CTC1UI user interface property for this user interface component
        /// </summary>
        private STR118CTC1UI moduleUI;

        /// <summary>
        /// A string indicating whether the result of the test
        /// </summary>
        private string passedOrFailed = "PASSED";

        /// <summary>
        /// XboxDevice being targeted
        /// </summary>
        private IXboxDevice xboxDevice;

        /// <summary>
        /// Drive the title is on
        /// </summary>
        private string titleDrive;

        /// <summary>
        /// A value indicating whether whether or not the console setup is complete
        /// </summary>
        private bool setupDone;

        /// <summary>
        /// A value indicating whether whether or not the currently selected save has been passed or failed
        /// </summary>
        private bool canLoad;

        /// <summary>
        /// A value indicating whether whether or not the currently selected save has been loaded
        /// </summary>
        private bool canPassFail;

        /// <summary>
        /// Backing field for FirstPageVisibility property
        /// </summary>
        private Visibility firstPageVisibility = Visibility.Visible;

        /// <summary>
        /// Backing field for SecondPageVisibility property
        /// </summary>
        private Visibility secondPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for ThirdPageVisibility property
        /// </summary>
        private Visibility thirdPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for FourthPageVisibility property
        /// </summary>
        private Visibility fourthPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for SkipVisible property
        /// </summary>
        private Visibility skipVisible = Visibility.Collapsed;

        /// <summary>
        /// Backing field for CurrentSaveMessage property
        /// </summary>
        private string currentSaveMessage;

        /// <summary>
        /// Backing field for SetupProgressMessage property
        /// </summary>
        private string setupProgressMessage;

        /// <summary>
        /// Backing field for AvailableSaves property
        /// </summary>
        private ObservableCollection<XboxSaveFile> availableSaves;

        /// <summary>
        /// Backing field for AdjunctSaves property
        /// </summary>
        private ObservableCollection<XboxSaveFile> adjunctSaves;

        /// <summary>
        /// Backing field for SavesOnBox property
        /// </summary>
        private ObservableCollection<string> savesOnBox;

        /// <summary>
        /// Backing field for SharedOrOtherOnBox property
        /// </summary>
        private ObservableCollection<string> sharedOrOtherOnBox;

        /// <summary>
        /// Backing field for SelectedSave property
        /// </summary>
        private XboxSaveFile selectedSave;

        /// <summary>
        /// Backing field for SelectedAdjunct property
        /// </summary>
        private XboxSaveFile selectedAdjunct;

        /// <summary>
        /// Backing field for SelectedSaveOnBox property
        /// </summary>
        private string selectedSaveOnBox;

        /// <summary>
        /// Backing field for CurrentProfileName property
        /// </summary>
        private string currentProfileName;

        /// <summary>
        /// Backing field for FilesBefore property
        /// </summary>
        private List<string> filesBefore;

        /// <summary>
        /// Backing field for FilesAfter property
        /// </summary>
        private List<string> filesAfter;
        
        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the UI element associated with this module
        /// </summary>
        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the visibility of the first page
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
        /// Gets or sets a value indicating whether the visibility of the second page
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
        /// Gets or sets a value indicating whether the visibility of the third page
        /// </summary>
        public Visibility ThirdPageVisibility
        {
            get
            {
                return this.thirdPageVisibility;
            }

            set
            {
                this.thirdPageVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the visibility of the fourth page
        /// </summary>
        public Visibility FourthPageVisibility
        {
            get
            {
                return this.fourthPageVisibility;
            }

            set
            {
                this.fourthPageVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the visibility of the skip button
        /// </summary>
        public Visibility SkipVisible
        {
            get
            {
                return this.skipVisible;
            }

            set
            {
                this.skipVisible = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether that setup is complete
        /// </summary>
        public bool SetupDone
        {
            get
            {
                return this.setupDone;
            }

            set
            {
                this.setupDone = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether user can load a save file
        /// </summary>
        public bool CanLoad
        {
            get
            {
                return this.canLoad;
            }

            set
            {
                this.canLoad = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether save is loaded and user can pass or fail that save
        /// </summary>
        public bool CanPassFail
        {
            get
            {
                return this.canPassFail;
            }

            set
            {
                this.canPassFail = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a the current save message
        /// </summary>
        public string CurrentSaveMessage
        {
            get
            {
                return this.currentSaveMessage;
            }

            set
            {
                this.currentSaveMessage = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the setup progress message
        /// </summary>
        public string SetupProgressMessage
        {
            get
            {
                return this.setupProgressMessage;
            }

            set
            {
                this.setupProgressMessage = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a collection of available saves
        /// </summary>
        public ObservableCollection<XboxSaveFile> AvailableSaves
        {
            get
            {
                return this.availableSaves;
            }

            set
            {
                this.availableSaves = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a collection of adjunct saves
        /// </summary>
        public ObservableCollection<XboxSaveFile> AdjunctSaves
        {
            get
            {
                return this.adjunctSaves;
            }

            set
            {
                this.adjunctSaves = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a collection of individual save files currently on the console
        /// </summary>
        public ObservableCollection<string> SavesOnBox
        {
            get
            {
                return this.savesOnBox;
            }

            set
            {
                this.savesOnBox = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a collection of shared save files and other files that have appeared on the console
        /// </summary>
        public ObservableCollection<string> SharedOrOtherOnBox
        {
            get
            {
                return this.sharedOrOtherOnBox;
            }

            set
            {
                this.sharedOrOtherOnBox = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected save
        /// </summary>
        public XboxSaveFile SelectedSave
        {
            get
            {
                return this.selectedSave;
            }

            set
            {
                this.selectedSave = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected adjunct save
        /// </summary>
        public XboxSaveFile SelectedAdjunct
        {
            get
            {
                return this.selectedAdjunct;
            }

            set
            {
                this.selectedAdjunct = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected of the save files we have restored
        /// </summary>
        public string SelectedSaveOnBox
        {
            get
            {
                return this.selectedSaveOnBox;
            }

            set
            {
                this.selectedSaveOnBox = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the current profile name
        /// </summary>
        public string CurrentProfileName
        {
            get
            {
                return this.currentProfileName;
            }

            set
            {
                this.currentProfileName = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a list of all files found in the Xbox file system
        /// </summary>
        public List<string> AllFilesOnXbox
        {
            get
            {
                List<string> files = new List<string>();
                if (this.xboxDevice == null)
                {
                    return files;
                }

                foreach (string drivename in this.xboxDevice.Drives)
                {
                    if ((drivename == "E") || (drivename == "DVD") || (drivename == "DEVKIT"))
                    {
                        continue;
                    }

                    files.AddRange(this.GetDirectoryFiles(this.xboxDevice.XboxConsole, drivename + ":\\"));
                }

                return files;
            }
        }

        /// <summary>
        /// Gets a value indicating whether whether there is one connected xbox selected
        /// </summary>
        private bool IsOneConnectedXboxSelected
        {
            get
            {
                string s = string.Empty;
                if (this.moduleContext.SelectedDevices.Count() == 0)
                {
                    s = "No consoles are selected. Select one. ";
                }
                else if (this.moduleContext.SelectedDevices.Count() > 1)
                {
                    s = this.moduleContext.SelectedDevices.Count().ToString() + " consoles are selected. Select just one. ";
                }

                foreach (IXboxDevice dev in this.moduleContext.SelectedDevices)
                {
                    // connected
                    if (dev.Connected == false)
                    {
                        s += "The selected device " + dev.Name + " is not connected. Connect the device.";
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
        /// Start - called when the module is first entered
        /// This function is called to show the overview or intro to the module.
        /// Typically the framework is active and user should choose a device in the device pool.
        /// </summary>
        /// <param name="ctx">The current working context for which this test will execute.</param>
        public void Start(IModuleContext ctx)
        {
            this.moduleContext = ctx as IXboxModuleContext;
            this.moduleUI = new STR118CTC1UI(this);
        }

        /// <summary>
        /// NextPage - called to leave the the module overview or intro screen entered by Start()
        /// The framework goes modal in this call and the module gains control.
        /// This function is called repeatedly to advance to multiple screens in the module.
        /// </summary>
        public void NextPage()
        {
            if (this.FirstPageVisibility == Visibility.Visible)
            {
                // check prerequisites - we need a console selected
                if (!this.IsOneConnectedXboxSelected)
                {
                    return;
                }

                this.xboxDevice = (IXboxDevice)this.moduleContext.SelectedDevices.First();

                // we need something to test
                if (!this.xboxDevice.IsTitleInstalled && !this.xboxDevice.CanInstallTitle)
                {
                    MessageBox.Show("Title cannot be installed as configured.\n\nPlease open Settings to fix the problem.\n", "Certification Assistance Tool");
                    return;
                }

                // move to second page and setup the console
                this.FirstPageVisibility = Visibility.Collapsed;
                this.SecondPageVisibility = Visibility.Visible;
                this.UpdateUIImmediately();
                this.moduleContext.IsModal = true;
                this.SetupDone = false;
                this.SetupSaveDependencyTest();
                this.SetupDone = true;
                this.NextPage();
            }
            else if (this.SecondPageVisibility == Visibility.Visible)
            {
                // move to third page where user can load save games
                this.SecondPageVisibility = Visibility.Collapsed;
                this.ThirdPageVisibility = Visibility.Visible;
                this.UpdateUIImmediately();

                // we need some saved games
                this.PieceTogetherSaves(this.moduleContext.XboxTitle.GameDirectory, this.xboxDevice.TitleId);
                this.CanLoad = true;
            }
            else if (this.ThirdPageVisibility == Visibility.Visible)
            {
                // move to fourth page to test saving with no profile
                this.ThirdPageVisibility = Visibility.Collapsed;
                this.FourthPageVisibility = Visibility.Visible;
                this.CurrentSaveMessage = string.Empty;
                this.UpdateUIImmediately();
                this.CanPassFail = true;

                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;

                    // sign out all profiles
                    ConsoleProfilesManager profilesManager = this.xboxDevice.XboxConsole.CreateConsoleProfilesManager();
                    profilesManager.SignOutAllUsers();
                    Thread.Sleep(500);

                    Mouse.OverrideCursor = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "There was a problem signing out users, exception: " + ex.Message,
                        "Certificaton Assistance Tool");
                }
            }
        }

        /// <summary>
        /// Adds saves to list of available profile saves
        /// </summary>
        public void AddSaves()
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = this.moduleContext.XboxTitle.GameDirectory;
            dialog.Tag = "Select a folder with saved games";
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.PieceTogetherSaves(dialog.SelectedPath, this.xboxDevice.TitleId);
            }
        }

        /// <summary>
        /// Loads the next save
        /// Removes profiles from console and puts the selected one on.
        /// </summary>
        public void LoadSelectedSave()
        {
            if (this.SelectedSave == null)
            {
                MessageBox.Show("Select a save", "Certificaton Assistance Tool");
                return;
            }

            this.RestoreSave(this.SelectedSave, true);

            // enable recording of pass fail
            this.CanPassFail = true;
            this.CanLoad = false;
        }

        /// <summary>
        /// Loads the next optional save
        /// Removes profiles from console and puts the selected one on.
        /// </summary>
        public void LoadSelectedAdjunct()
        {
            if (this.SelectedAdjunct == null)
            {
                MessageBox.Show("Select a shared save or other file to restore", "Certificaton Assistance Tool");
                return;
            }

            this.RestoreSave(this.SelectedAdjunct, false);

            if (this.SkipVisible != Visibility.Visible)
            {
                // enable recording of pass fail
                this.CanPassFail = true;
                this.CanLoad = false;
            }
        }

        /// <summary>
        /// delete selected save file
        /// </summary>
        public void DeleteSelectedSaveOnbox()
        {
            if (this.SelectedSaveOnBox != null)
            {
                if (MessageBoxResult.Yes == MessageBox.Show(
                    "Delete file " + this.SelectedSaveOnBox + " from console?",
                    "Certification Assistance Tool",
                    MessageBoxButton.YesNo))
                {
                    // this creates a custom test. 

                    // sign out
                    try
                    {
                        ConsoleProfilesManager profilesManager = this.xboxDevice.XboxConsole.CreateConsoleProfilesManager();
                        profilesManager.SignOutAllUsers();
                        Thread.Sleep(500);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    // set available actions
                    this.CanLoad = false;
                    this.CanPassFail = true;

                    // remove the file
                    this.xboxDevice.XboxConsole.DeleteFile(this.SelectedSaveOnBox);
                    this.SavesOnBox.Remove(this.SelectedSaveOnBox);
                    this.NotifyPropertyChanged("SavesOnBox");
                }
            }
        }

        /// <summary>
        /// PassSave - mark current save as passing
        /// </summary>
        public void PassSave()
        {
            if (this.SelectedSave == null && this.SelectedAdjunct == null)
            {
                MessageBox.Show("Select the row you want to pass or fail", "Certification Assistance Tool");
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;

            // log test case
            this.moduleContext.Log("Tested using save file(s): " + this.SelectedSave.ToString());
            this.moduleContext.Log("Passed");

            // check for unexpected files
            this.filesAfter = this.AllFilesOnXbox;
            ObservableCollection<string> allNewFiles = this.DetectNewFiles(this.filesAfter, this.filesBefore);
            if (this.SharedOrOtherOnBox == null)
            {
                this.SharedOrOtherOnBox = new ObservableCollection<string>();
            }
            else
            {
                this.SharedOrOtherOnBox.Clear();
            }

            foreach (string s in allNewFiles)
            {
                if (!this.SavesOnBox.Contains(s))
                {
                    if (!s.Contains("HDD:\\Cache"))
                    {
                        this.SharedOrOtherOnBox.Add(s);
                        this.moduleContext.Log("Found unexpected file: " + s);
                    }
                }
            }

            // set actions
            this.CanLoad = true;
            this.CanPassFail = false;

            // set visual result
            if (this.SelectedSave != null)
            {
                this.SelectedSave.Passed = true;
                this.SelectedSave.ResultText = "Pass";
                this.SelectedSave.ResultImage = "Images/good.png";
            }

            this.NotifyPropertyChanged("AvailableSaves");
            this.NotifyPropertyChanged("AdjunctSaves");
            this.NotifyPropertyChanged("SavesOnBox");
            this.NotifyPropertyChanged("SharedOrOtherOnBox");

            // if there are shared saves or other files, show them
            if (this.SharedOrOtherOnBox.Any())
            {
                MessageBox.Show(
                    "Some unexpected files were found. The files have been logged.",
                    "Certification Assistance Tool");
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// FailSave - mark current save as failing
        /// </summary>
        public void FailSave()
        {
            if (this.SelectedSave == null)
            {
                MessageBox.Show("Select the row you want to pass or fail", "Certification Assistance Tool");
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;

            // log test case
            this.moduleContext.Log("Tested using save file(s): " + this.SelectedSave.ToString());
            this.moduleContext.Log("Failed");

            // check for unexpected files
            this.filesAfter = this.AllFilesOnXbox;
            ObservableCollection<string> allNewFiles = this.DetectNewFiles(this.filesAfter, this.filesBefore);
            if (this.SharedOrOtherOnBox == null)
            {
                this.SharedOrOtherOnBox = new ObservableCollection<string>();
            }
            else
            {
                this.SharedOrOtherOnBox.Clear();
            }

            foreach (string s in allNewFiles)
            {
                if (!this.SavesOnBox.Contains(s))
                {
                    if ((!s.Contains("HDD:\\Cache")) && (!s.Contains("LaunchXContent.xex")) && (!s.Contains("HDD:\\SysCache0")))
                    {
                        this.SharedOrOtherOnBox.Add(s);
                        this.moduleContext.Log("Found unexpected file: " + s);
                    }
                }
            }

            // set actions
            this.CanLoad = true;
            this.CanPassFail = false;

            // set visual result
            if (this.SelectedSave != null)
            {
                this.SelectedSave.Passed = false;
                this.SelectedSave.ResultText = "Fail";
                this.SelectedSave.ResultImage = "Images/bad.png";
            }

            this.NotifyPropertyChanged("AvailableSaves");
            this.NotifyPropertyChanged("AdjunctSaves");
            this.NotifyPropertyChanged("SavesOnBox");
            this.NotifyPropertyChanged("SharedOrOtherOnBox");

            this.moduleContext.Log(this.SelectedSave.ToString() + " Failed");
            this.passedOrFailed = "Fail";

            // if there are shared saves or other files, ask if user wants to investigate the failure using these
            if (this.AdjunctSaves.Any())
            {
                if (MessageBoxResult.Yes == MessageBox.Show(
                    "Do you want to investigate this failure? We found some additional types of files",
                    "Certification Assistance Tool", 
                    MessageBoxButton.YesNo))
                {
                    this.SkipVisible = Visibility.Visible;
                    this.CanPassFail = false;
                    this.CanLoad = true;
                }
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// stop investigation mode
        /// </summary>
        public void InvestigationIsDone()
        {
            this.SkipVisible = Visibility.Collapsed;
            this.CanPassFail = false;
            this.CanLoad = true;
            this.SharedOrOtherOnBox.Clear();
            this.SavesOnBox.Clear();
        }

        /// <summary>
        /// PassNoProfile - mark test of no profile as passing
        /// </summary>
        public void PassNoProfile()
        {
            this.moduleContext.Log("No profile test: Passed");
            this.CurrentSaveMessage = "We recorded Pass for the test of this title with no profile. This is the last test of the module.";
        }

        /// <summary>
        /// FailNoProfile - mark test of no profile as failing
        /// </summary>
        public void FailNoProfile()
        {
            this.moduleContext.Log("No profile test: Failed");
            this.CurrentSaveMessage = "We recorded Fail for the test of this title with no profile. This is the last test of the module.";
        }

        /// <summary>
        /// Stop - called when the module is done or aborted
        /// </summary>
        public void Stop()
        {
            if (!this.moduleContext.IsModal)
            {
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;

            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("RESULT: " + this.passedOrFailed + "\r\n");
            this.moduleContext.Log("*************************************************************\r\n");

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Figure out files belonging to a single save within a directory
        /// </summary>
        /// <param name="path">directory that contains saves</param>
        /// <param name="title">title number of the game</param>
        private void PieceTogetherSaves(string path, string title)
        {
            if (this.AvailableSaves == null)
            {
                this.AvailableSaves = new ObservableCollection<XboxSaveFile>();
            }

            if (this.AdjunctSaves == null)
            {
                this.AdjunctSaves = new ObservableCollection<XboxSaveFile>();
            }

            // find all title and retail (FFFE07D1) folders in this location
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            DirectoryInfo[] dirInfoRetailPaths = dirInfo.GetDirectories("FFFE07D1", SearchOption.AllDirectories);
            DirectoryInfo[] dirInfoTitlePaths = dirInfo.GetDirectories(title, SearchOption.AllDirectories);

            foreach (DirectoryInfo rdi in dirInfoRetailPaths)
            {
                // find avatar saves: 0000000000000000 / RetailID / 00020000 / *
                if (rdi.Parent.ToString().Contains("000000000000"))
                {
                    DirectoryInfo[] dirInfoAvatarPaths = rdi.GetDirectories("00020000", SearchOption.TopDirectoryOnly);
                    foreach (DirectoryInfo adi in dirInfoAvatarPaths)
                    {
                        foreach (FileInfo afi in adi.GetFiles("*", SearchOption.TopDirectoryOnly))
                        {
                            this.AdjunctSaves.Add(new XboxSaveFile(afi.FullName, null, null, null, "Avatar"));
                        }
                    }
                }
                else
                {
                    // find profiles: ProfileID / RetailID / 00010000 / ProfileID
                    DirectoryInfo[] dirInfoProfilePaths = rdi.GetDirectories("00010000", SearchOption.TopDirectoryOnly);
                    foreach (DirectoryInfo pdi in dirInfoProfilePaths)
                    {
                        foreach (FileInfo pfi in pdi.GetFiles("*", SearchOption.TopDirectoryOnly))
                        {
                            this.AvailableSaves.Add(new XboxSaveFile(null, pfi.FullName, null, null, "Profile"));
                        }
                    }
                }
            }

            foreach (DirectoryInfo tdi in dirInfoTitlePaths)
            {
                // find shared saves: 0000000000000000 / TitleID / 00000001 / *
                if (tdi.Parent.ToString().Contains("000000000000"))
                {
                    DirectoryInfo[] dirInfoSharedPaths = tdi.GetDirectories("00000001", SearchOption.TopDirectoryOnly);
                    foreach (DirectoryInfo sdi in dirInfoSharedPaths)
                    {
                        foreach (FileInfo sfi in sdi.GetFiles("*", SearchOption.TopDirectoryOnly))
                        {
                            this.AdjunctSaves.Add(new XboxSaveFile(null, null, sfi.FullName, null, "Shared"));
                        }
                    }
                }
                else
                {
                    // find standard saves: ProfileID / TitleID / 00000001 / *
                    DirectoryInfo[] dirInfoStandardPaths = tdi.GetDirectories("00000001", SearchOption.TopDirectoryOnly);
                    foreach (DirectoryInfo sdi in dirInfoStandardPaths)
                    {
                        foreach (FileInfo sfi in sdi.GetFiles("*", SearchOption.TopDirectoryOnly))
                        {
                            this.AvailableSaves.Add(new XboxSaveFile(null, tdi.Parent.FullName + "\\FFFE07D1\\00010000\\" + tdi.Parent.Name, null, sfi.FullName, "Standard"));
                        }
                    }
                }
            }

            this.NotifyPropertyChanged("AvailableSaves");
        }

        /// <summary>
        /// restore selected save to xbox
        /// </summary>
        /// <param name="save">save object</param>
        /// <param name="replace">true to clear previous saves first</param>
        private void RestoreSave(XboxSaveFile save, bool replace)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            // logout profile so we can remove previous save game
            try
            {
                ConsoleProfilesManager profilesManager = this.xboxDevice.XboxConsole.CreateConsoleProfilesManager();
                profilesManager.SignOutAllUsers();
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (this.SavesOnBox == null)
            {
                this.SavesOnBox = new ObservableCollection<string>();
            }

            if (this.SharedOrOtherOnBox == null)
            {
                this.SharedOrOtherOnBox = new ObservableCollection<string>();
            }

            if (replace)
            {
                // stop the game
                this.xboxDevice.Reboot();

                // remove avatar, shared and standard saves
                if (!this.xboxDevice.DeleteAllTitleSavesFrom("HDD"))
                {
                    MessageBox.Show("Unable to remove previous saves", "Certification Assistance Tool");
                    Mouse.OverrideCursor = null;
                    return;
                }

                /*
                // remove avatar and shared saves
                if (!this.xboxDevice.DeleteAllSaves())
                {
                    MessageBox.Show("Unable to remove previous save", "Certification Assistance Tool");
                    Mouse.OverrideCursor = null;
                    return;
                }

                // remove profile and standard saves (and profiles)
                if (!this.xboxDevice.DeleteAllProfiles())
                {
                    MessageBox.Show("Unable to remove previous profiles", "Certification Assistance Tool");
                    Mouse.OverrideCursor = null;
                    return;
                }
                */

                // remove any unexpected files we found last round
                foreach (string s in this.SharedOrOtherOnBox)
                {
                    try
                    {
                        this.xboxDevice.XboxConsole.DeleteFile(s);
                    }
                    catch (Exception ex)
                    {
                        this.moduleContext.Log("Unable to delete file " + s + " Exception: " + ex.Message);
                    }
                }

                // clear local lists
                this.SavesOnBox.Clear();
                this.SharedOrOtherOnBox.Clear();
            }

            // .
            // Move all parts of this save onto the Xbox
            // .
            string destination;

            // restore profile
            if (!string.IsNullOrEmpty(save.ProfileFile))
            {
                destination = "HDD:\\Content\\" + save.ProfileDisplay + "\\FFFE07D1\\00010000\\" + save.ProfileDisplay;
                this.xboxDevice.SendFile(save.ProfileFile, destination, null);
                this.SavesOnBox.Add(destination);
            }

            // restore standard save
            if (!string.IsNullOrEmpty(save.StandardFile))
            {
                destination = "HDD:\\Content\\" + save.ProfileDisplay + "\\" + this.xboxDevice.TitleId + "\\00000001\\" + save.StandardDisplay;
                this.xboxDevice.SendFile(save.StandardFile, destination, null);
                this.SavesOnBox.Add(destination);
            }

            // restore avatar saves
            if (!string.IsNullOrEmpty(save.AvatarFile))
            {
                destination = "HDD:\\Content\\0000000000000000\\FFF07D1\\00020000";
                this.xboxDevice.SendFile(save.AvatarFile, destination, null);
                this.SavesOnBox.Add(destination);
            }

            // restore shared saved
            if (!string.IsNullOrEmpty(save.SharedFile))
            {
                destination = "HDD:\\Content\\0000000000000000\\" + this.xboxDevice.TitleId + "\\00000001\\" + save.SharedDisplay;
                this.xboxDevice.SendFile(save.SharedFile, destination, null);
                this.SavesOnBox.Add(destination);
            }

            if (replace)
            {
                // Figure out profile name
                try
                {
                    ConsoleProfilesManager profilesManager = this.xboxDevice.XboxConsole.CreateConsoleProfilesManager();
                    this.CurrentProfileName = profilesManager.EnumerateConsoleProfiles().First().Gamertag;
                    save.ProfileName = this.CurrentProfileName;

                    // sign-in profile
                    profilesManager.EnumerateConsoleProfiles().First().SignIn(UserIndex.Zero);
                }
                catch (Exception)
                {
                }
            }

            this.NotifyPropertyChanged("AvailableSaves");
            this.NotifyPropertyChanged("AdjunctSaves");
            this.NotifyPropertyChanged("SavesOnBox");
            this.NotifyPropertyChanged("SharedOrOtherOnBox");

            // Launch game
            this.CurrentSaveMessage = "Launching " + this.moduleContext.XboxTitle.Name;
            this.xboxDevice.LaunchTitle("HDD");

            // prompt to test gameplay
            this.CurrentSaveMessage += "\nReady";

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// From the specified XBox Console and Drive name, this utility/helper function returns a List
        /// structures of the found directory files.
        /// </summary>
        /// <param name="console">XBoxConsole of which to get files</param>
        /// <param name="drivename">Drive name from which to get those files. IMPORTANT - root drive names must end in :\</param>
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

                    // ignore devkit files
                    if (s.Contains("DEVKIT"))
                    {
                        continue;
                    }

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
            catch (Exception ex)
            {
                MessageBox.Show("There was an error reading files for drive " + drivename + "\n\nException: " + ex.Message);
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
        private ObservableCollection<string> DetectNewFiles(List<string> after, List<string> before)
        {
            ObservableCollection<string> files = new ObservableCollection<string>();
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
        /// Sets up save dependency test
        /// </summary>
        private void SetupSaveDependencyTest()
        {
            bool cleared = false;

            // Disable all drives except HDD
            foreach (string drive in this.xboxDevice.Drives)
            {
                if (drive.Contains("MUINT"))
                {
                    this.SetupProgressMessage += "\nDisabling internal MU.......";
                    this.xboxDevice.IsMUINTEnabled = false;
                    this.SetupProgressMessage += " Disabled.";
                }
                else if (drive.Contains("INTUSB"))
                {
                    this.SetupProgressMessage += "\nDisabling internal USB.......";

                    // NOTE: internal MU and internal USB are disabled as a set
                    this.xboxDevice.IsMUINTEnabled = false;
                    this.SetupProgressMessage += " Disabled.";
                }
                else if (drive.Contains("USB") || drive.Contains("MU"))
                {
                    MessageBox.Show("Please remove all USB sticks and external MU devices.\n\nWe detected a device named " + drive);
                    continue;
                }
            }

            // Make sure there is a harddrive
            foreach (string drive in this.xboxDevice.Drives)
            {
                if (drive.Contains("HDD"))
                {
                    this.titleDrive = drive;
                    break;
                }
            }

            if (string.IsNullOrEmpty(this.titleDrive))
            {
                MessageBox.Show("We were unable to find a drives on " + this.xboxDevice.Name, "Certification Assistance Tool");
                return;
            }

            // Clear or Flash the Xbox
            Mouse.OverrideCursor = Cursors.Wait;

            while (!cleared)
            {
                try
                {
                    this.SetupProgressMessage = "Removing all content from Xbox.......";
                    this.xboxDevice.Reboot();
                    this.xboxDevice.RunIXBoxAutomationScript(@"Scripts\BackTwice.xboxautomation");
                    //this.xboxDevice.DeleteAllProfilesFrom("HDD");
                    //this.xboxDevice.DeleteAllGamesFrom("HDD");
                    //this.xboxDevice.DeleteAllSavesFrom("HDD");
                    this.xboxDevice.DeleteAllTitleSavesFrom("HDD");
                    this.moduleContext.Log(this.xboxDevice.IP + " removed all content.");
                    this.SetupProgressMessage += this.xboxDevice.IP + " Removed.";
                    cleared = true;
                }
                catch (Exception ex)
                {
                    if (MessageBoxResult.No == MessageBox.Show(
                        "There was a problem removing some content. Exception: " + ex.Message + "\n\nTry again?",
                        "Certification Assistance Tool",
                        MessageBoxButton.YesNo))
                    {
                        cleared = true;
                    }
                }
            }

            // Install the Title
            this.SetupProgressMessage += "\nInstalling " + this.moduleContext.XboxTitle.Name + ".......";
            this.UpdateUIImmediately();
            this.xboxDevice.LaunchDevDashboard();
            this.xboxDevice.InstallTitle("HDD", this.moduleContext.OpenProgressBarWindow("Installing " + this.moduleContext.XboxTitle.Name + "..."));
            this.moduleContext.Log(this.moduleContext.XboxTitle.Name + " installed to drive " + this.titleDrive);
            this.SetupProgressMessage += this.moduleContext.XboxTitle.Name + " installed.";
            this.UpdateUIImmediately();

            // get a snapshot of files now, before any new saves are tried
            this.filesBefore = this.AllFilesOnXbox;

            // done setting up
            Mouse.OverrideCursor = null;
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
        /// Update the UI immediately without returning control to the main thread
        /// </summary>
        private void UpdateUIImmediately()
        {
            this.moduleUI.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);
        }

        /// <summary>
        /// represents a game save
        /// </summary>
        public class XboxSaveFile : INotifyPropertyChanged
        {
            /// <summary>
            /// Backing field for Passed property
            /// </summary>
            private bool passed;

            /// <summary>
            /// Backing field of ResultImage property
            /// </summary>
            private string resultImage;

            /// <summary>
            /// Backing field for ResultText property
            /// </summary>
            private string resultText;

            /// <summary>
            /// Backing field for ProfileName property
            /// </summary>
            private string profileName;

            /// <summary>
            /// Initializes a new instance of the <see cref="XboxSaveFile" /> class.
            /// </summary>
            /// <param name="avatar">path to avatar save</param>
            /// <param name="profile">path to profile save</param>
            /// <param name="shared">path to shared save</param>
            /// <param name="standard">path to standard save</param>
            /// <param name="type">name of save type</param>
            public XboxSaveFile(string avatar, string profile, string shared, string standard, string type)
            {
                this.SaveType = type;
                this.AvatarFile = avatar;
                this.ProfileFile = profile;
                this.SharedFile = shared;
                this.StandardFile = standard;
                this.ResultImage = "Images/untested.png";
                this.ResultText = "?";
                try
                {
                    if (!string.IsNullOrEmpty(avatar))
                    {
                        this.AvatarDisplay = Path.GetFileName(avatar);
                    }
                }
                catch
                {
                    this.AvatarDisplay = this.AvatarFile;
                }

                try
                {
                    if (!string.IsNullOrEmpty(profile))
                    {
                        this.ProfileDisplay = Path.GetFileName(profile);
                    }
                }
                catch
                {
                    this.ProfileDisplay = this.ProfileFile;
                }

                try
                {
                    if (!string.IsNullOrEmpty(shared))
                    {
                        this.SharedDisplay = Path.GetFileName(shared);
                    }
                }
                catch
                {
                    this.SharedDisplay = this.SharedFile;
                }

                try
                {
                    if (!string.IsNullOrEmpty(standard))
                    {
                        this.StandardDisplay = Path.GetFileName(standard);
                    }
                }
                catch
                {
                    this.StandardDisplay = this.StandardFile;
                }
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets a path to an avatar
            /// </summary>
            public string AvatarFile { get; set; }

            /// <summary>
            /// Gets or sets a path to a profile
            /// </summary>
            public string ProfileFile { get; set; }

            /// <summary>
            /// Gets or sets a path to a shared save file
            /// </summary>
            public string SharedFile { get; set; }

            /// <summary>
            /// Gets or sets a path to a standard save file
            /// </summary>
            public string StandardFile { get; set; }

            /// <summary>
            /// Gets or sets the save type.  Possible types are "Profile" "Standard" "Avatar" "Shared"
            /// </summary>
            public string SaveType { get; set; }

            /// <summary>
            /// Gets or sets a string to display for an avatar file
            /// </summary>
            public string AvatarDisplay { get; set; }

            /// <summary>
            /// Gets or sets a string to display for a profile save file
            /// </summary>
            public string ProfileDisplay { get; set; }

            /// <summary>
            /// Gets or sets a string to display for a shared save file
            /// </summary>
            public string SharedDisplay { get; set; }

            /// <summary>
            /// Gets or sets a string to display for a standard save file
            /// </summary>
            public string StandardDisplay { get; set; }

            /// <summary>
            /// Gets or sets the profile name
            /// </summary>
            public string ProfileName 
            {
                get
                { 
                    return this.profileName;
                }
                
                set
                {
                    this.profileName = value;
                    this.NotifyPropertyChanged(); 
                } 
            }

            /// <summary>
            /// Gets or sets a value indicating whether the test has passed
            /// </summary>
            public bool Passed 
            {
                get 
                {
                    return this.passed;
                } 
                
                set
                {
                    this.passed = value;
                    this.NotifyPropertyChanged(); 
                } 
            }

            /// <summary>
            /// Gets or sets the resulting image file
            /// </summary>
            public string ResultImage 
            {
                get 
                {
                    return this.resultImage; 
                }
                
                set 
                {
                    this.resultImage = value;
                    this.NotifyPropertyChanged(); 
                }
            }

            /// <summary>
            /// Gets or sets result text 
            /// </summary>
            public string ResultText 
            {
                get 
                {
                    return this.resultText; 
                }
                
                set 
                {
                    this.resultText = value;
                    this.NotifyPropertyChanged(); 
                }
            }

            /// <summary>
            /// Converts this XboxSaveFile to a string
            /// </summary>
            /// <returns>A string conversion of this object</returns>
            public override string ToString()
            {
                string result = this.SaveType;

                if (!string.IsNullOrEmpty(this.ProfileFile))
                {
                    result += Environment.NewLine + this.ProfileFile;
                }

                if (!string.IsNullOrEmpty(this.StandardFile))
                {
                    result += Environment.NewLine + this.StandardFile;
                }

                if (!string.IsNullOrEmpty(this.SharedFile))
                {
                    result += Environment.NewLine + this.SharedFile;
                }

                if (!string.IsNullOrEmpty(this.AvatarFile))
                {
                    result += Environment.NewLine + this.AvatarFile;
                }

                return result;
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
        }
    } // End of: public class STR118CTC1 : IModule, INotifyPropertyChanged
} // End of: namespace STR118 in code file STR118CTC1.cs