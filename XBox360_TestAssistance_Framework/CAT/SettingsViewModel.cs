// -----------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Input;
    using System.Xml;
    
    /// <summary>
    /// MainViewModel is the primary ViewModel
    /// UI is bound to properties in this class.
    /// </summary>
    public class SettingsViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Backing field for MainViewModel property
        /// </summary>
        private MainViewModel mainViewModel;

        /// <summary>
        /// Backing variable for MultipleGameConfigFilesFound property
        /// </summary>
        private bool multipleGameConfigFilesFound;

        /// <summary>
        /// Backing variable for MultipleGameInstallTypesFound property
        /// </summary>
        private bool multipleGameInstallTypesFound;

        /// <summary>
        /// Backing variable for MultipleContentPackagesFound property
        /// </summary>
        private bool multipleContentPackagesFound;

        /// <summary>
        /// Backing variable for MultipleDiscImagesFound property
        /// </summary>
        private bool multipleDiscImagesFound;

        /// <summary>
        /// Backing variable for MultipleRawFound property
        /// </summary>
        private bool multipleRawFound;

        /// <summary>
        /// Backing variable for MultipleTitleUpdatesFound property
        /// </summary>
        private bool multipleTitleUpdatesFound;

        /// <summary>
        /// Backing variable for GameInstallRawVisibility property
        /// </summary>
        private Visibility gameInstallRawVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing variable for GameInstallDiscVisibility property
        /// </summary>
        private Visibility gameInstallDiscVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing variable for GameInstallContentVisibility property
        /// </summary>
        private Visibility gameInstallContentVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing variable for MultipleDiscImagesFoundVisibility property
        /// </summary>
        private Visibility multipleDiscImagesFoundVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing variable for MultipleRawFoundVisibility property
        /// </summary>
        private Visibility multipleRawFoundVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing variable for MultipleContentPackagesFoundVisibility property
        /// </summary>
        private Visibility multipleContentPackagesFoundVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing variable for Is360Selected property
        /// </summary>
        private bool is360Selected;

        /// <summary>
        /// Backing variable for MultipleTitleUpdatesFound property
        /// </summary>
        private TCRPlatformViewItem currentPlatform;

        /// <summary>
        /// Backing variable for PlatformPickedVisibility property
        /// </summary>
        private Visibility platformPickedVisibility;

        /// <summary>
        /// Backing variable for CurrentTCRVersionList property
        /// </summary>
        private List<TCRVersionViewItem> currentTCRVersionList;

        /// <summary>
        /// Backing variable for CurrentTCRVersion property
        /// </summary>
        private TCRVersionViewItem currentTCRVersion;

        /// <summary>
        /// Backing variable for GameInstallTypeList property
        /// </summary>
        private List<string> gameInstallTypeList = new List<string>();

        /// <summary>
        /// Backing variable for CurrentTitle property
        /// </summary>
        private DataModel.XboxTitle currentTitle;

        /// <summary>
        /// Theme name prior to loading settings, to restore if settings is canceled.
        /// </summary>
        private string oldThemeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel" /> class.
        /// Constructor for MainViewModel
        /// </summary>
        /// <param name="settings">A reference to the Settings UI object this View Model is associated with</param>
        /// <param name="mainViewModel">A reference to the MainViewModel</param>
        /// <param name="title">The previously save XboxTitle information</param>
        public SettingsViewModel(Settings settings, MainViewModel mainViewModel, DataModel.XboxTitle title)
        {
            this.oldThemeName = mainViewModel.CurrentTheme.Name;
            this.gameInstallTypeList.Add("Content Package");
            this.gameInstallTypeList.Add("Disc Emulation");
            this.gameInstallTypeList.Add("Raw");

            this.Settings = settings;
            this.mainViewModel = mainViewModel;
            this.CurrentTitle = title;

            settings.Closing += this.OnWindowClosing;

            if (Properties.Settings.Default.SettingsLeft != 0)
            {
                settings.Left = Properties.Settings.Default.SettingsLeft;
                settings.Top = Properties.Settings.Default.SettingsTop;
            }

            if (title.GameInstallType == "Content Package")
            {
                this.GameInstallContentVisibility = Visibility.Visible;
            }
            else if (title.GameInstallType == "Disc Emulation")
            {
                this.GameInstallDiscVisibility = Visibility.Visible;
            }
            else if (title.GameInstallType == "Raw")
            {
                this.GameInstallRawVisibility = Visibility.Visible;
            }

            this.CurrentTitleFriendlyName = title.Name;
            this.CurrentPlatform = MainViewModel.CurrentPlatform;
            this.PlatformPickedVisibility = MainViewModel.PlatformPickedVisibility;
            this.CurrentTCRVersionList = MainViewModel.CurrentTCRVersionList;
            this.CurrentTCRVersion = MainViewModel.CurrentTCRVersion;

            this.BrowseToSymbolsCommand = new Command((o) => this.BrowseToSymbols());
            this.BrowseToContentPackageCommand = new Command((o) => this.BrowseToContentPackage());
            this.BrowseToGameConfigCommand = new Command((o) => this.BrowseToGameConfig());
            this.BrowseToXDKRecoveryCommand = new Command((o) => this.BrowseToXDKRecovery());
            this.BrowseToDiscImageCommand = new Command((o) => this.BrowseToDiscImage());
            this.BrowseToRawGameDirectoryCommand = new Command((o) => this.BrowseToRawGameDirectory());
            this.BrowseToTitleUpdateCommand = new Command((o) => this.BrowseToTitleUpdate());
            this.BrowseToGameDirectoryCommand = new Command((o) => this.BrowseToGameDirectory());
            this.ApplySettingsCommand = new Command((o) => this.ApplySettings());
            this.ApplyAndCloseSettingsCommand = new Command((o) => this.ApplyAndCloseSettings());
            this.CancelSettingsCommand = new Command((o) => this.CancelSettings());
        }

        /// <summary>
        /// A delegate type used for validating file names
        /// </summary>
        /// <param name="fileName">The file name to validate</param>
        /// <returns>A boolean value indicating if the file is valid</returns>
        public delegate bool ValidateFileNameDelegate(string fileName);

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a reference to main MainViewModel
        /// </summary>
        public MainViewModel MainViewModel 
        {
            get { return this.mainViewModel; }
        }

        /// <summary>
        /// Gets or sets the Settings UI object
        /// </summary>
        public Settings Settings { get; set; }

        /// <summary>
        /// Gets or sets the Returns the currently set XboxTitle
        /// </summary>
        public DataModel.XboxTitle CurrentTitle
        {
            get
            {
                return this.currentTitle;
            }
            
            set
            {
                this.currentTitle = value;
                this.NotifyPropertyChanged();
            } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use the demo content package, is a content package title
        /// </summary>
        public bool UseDemo
        { 
            get
            {
                return this.currentTitle.UseDemo;
            }
            
            set
            {
                this.currentTitle.UseDemo = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the symbol path
        /// </summary>
        public string SymbolsPath
        {
            get
            {
                return this.currentTitle.SymbolsPath; 
            }

            set
            {
                this.currentTitle.SymbolsPath = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the path to the game config file
        /// </summary>
        public string GameConfigPath 
        {
            get
            {
                return this.currentTitle.GameConfigPath; 
            }

            set
            {
                this.currentTitle.GameConfigPath = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the path to the content package, if a content package title
        /// </summary>
        public string ContentPackage 
        {
            get
            {
                return this.currentTitle.ContentPackage;
            }
            
            set
            {
                this.currentTitle.ContentPackage = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the path to the demo content package, if a content package title
        /// </summary>
        public string DemoContentPackage
        {
            get
            {
                return this.currentTitle.DemoContentPackage; 
            }
 
            set
            {
                this.currentTitle.DemoContentPackage = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the path to the XDK Recovery exe
        /// </summary>
        public string XDKRecoveryPath 
        {
            get
            {
                return this.currentTitle.XdkRecoveryPath; 
            }

            set 
            {
                this.currentTitle.XdkRecoveryPath = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the game install type
        /// </summary>
        public string GameInstallType
        {
            get
            {
                return this.currentTitle.GameInstallType;
            }

            set
            {
                this.currentTitle.GameInstallType = value;
                this.GameInstallRawVisibility = Visibility.Collapsed;
                this.GameInstallDiscVisibility = Visibility.Collapsed;
                this.GameInstallContentVisibility = Visibility.Collapsed;
                if (value == "Content Package")
                {
                    this.GameInstallContentVisibility = Visibility.Visible;
                    this.MultipleDiscImagesFoundVisibility = Visibility.Collapsed;
                    this.MultipleRawFoundVisibility = Visibility.Collapsed;
                    this.MultipleContentPackagesFoundVisibility = this.MultipleContentPackagesFound ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (value == "Disc Emulation")
                {
                    this.GameInstallDiscVisibility = Visibility.Visible;
                    this.MultipleContentPackagesFoundVisibility = Visibility.Collapsed;
                    this.MultipleRawFoundVisibility = Visibility.Collapsed;
                    this.MultipleDiscImagesFoundVisibility = this.MultipleDiscImagesFound ? Visibility.Visible : Visibility.Collapsed;
                }
                else if (value == "Raw")
                {
                    this.GameInstallRawVisibility = Visibility.Visible;
                    this.MultipleDiscImagesFoundVisibility = Visibility.Collapsed;
                    this.MultipleContentPackagesFoundVisibility = Visibility.Collapsed;
                    this.MultipleRawFoundVisibility = this.MultipleRawFound ? Visibility.Visible : Visibility.Collapsed;
                }

                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the path of the RAW game files, if a raw title
        /// </summary>
        public string RawGameDirectory 
        {
            get
            {
                return this.currentTitle.RawGameDirectory; 
            }
            
            set
            {
                this.currentTitle.RawGameDirectory = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the path to the disc image, if a disc emulation title
        /// </summary>
        public string DiscImage
        { 
            get
            {
                return this.currentTitle.DiscImage; 
            } 
            
            set
            {
                this.currentTitle.DiscImage = value;
                this.NotifyPropertyChanged(); 
            }
        }
 
        /// <summary>
        /// Gets or sets the root game directory
        /// </summary>
        public string GameDirectory
        { 
            get
            {
                return this.currentTitle.GameDirectory; 
            } 
            
            set
            {
                this.currentTitle.GameDirectory = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the path to the title update
        /// </summary>
        public string TitleUpdatePath 
        { 
            get
            {
                return this.currentTitle.TitleUpdatePath;
            }

            set 
            {
                this.currentTitle.TitleUpdatePath = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple game config files were found
        /// </summary>
        public bool MultipleGameConfigFilesFound 
        {
            get
            {
                return this.multipleGameConfigFilesFound; 
            }

            set
            {
                this.multipleGameConfigFilesFound = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple game install types were found
        /// </summary>
        public bool MultipleGameInstallTypesFound
        { 
            get
            {
                return this.multipleGameInstallTypesFound; 
            }

            set
            {
                this.multipleGameInstallTypesFound = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple content packages were found
        /// </summary>
        public bool MultipleContentPackagesFound 
        {
            get
            {
                return this.multipleContentPackagesFound; 
            }

            set 
            {
                this.multipleContentPackagesFound = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple disc images were found
        /// </summary>
        public bool MultipleDiscImagesFound
        {
            get 
            {
                return this.multipleDiscImagesFound;
            } 
            
            set
            {
                this.multipleDiscImagesFound = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple raw titles directories were found
        /// </summary>
        public bool MultipleRawFound
        {
            get
            {
                return this.multipleRawFound;
            }

            set 
            {
                this.multipleRawFound = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether multiple title update directories were found
        /// </summary>
        public bool MultipleTitleUpdatesFound
        { 
            get
            {
                return this.multipleTitleUpdatesFound;
            }
 
            set
            {
                this.multipleTitleUpdatesFound = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the current title friendly name
        /// </summary>
        public string CurrentTitleFriendlyName 
        { 
            get
            {
                return this.currentTitle.Name; 
            }

            set
            {
                this.currentTitle.Name = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a visibility flag indicating whether the raw title fields are visible
        /// </summary>
        public Visibility GameInstallRawVisibility 
        { 
            get
            {
                return this.gameInstallRawVisibility; 
            } 
            
            set 
            {
                this.gameInstallRawVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a visibility flag indicating whether the disc based title fields are visible
        /// </summary>
        public Visibility GameInstallDiscVisibility
        {
            get
            {
                return this.gameInstallDiscVisibility; 
            } 
            
            set 
            {
                this.gameInstallDiscVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a visibility flag indicating whether the content package title fields are visible
        /// </summary>
        public Visibility GameInstallContentVisibility
        { 
            get
            {
                return this.gameInstallContentVisibility; 
            }
            
            set
            {
                this.gameInstallContentVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a visibility flag indicating whether the multiple disc image warning is visible
        /// </summary>
        public Visibility MultipleDiscImagesFoundVisibility 
        {
            get
            {
                return this.multipleDiscImagesFoundVisibility; 
            }

            set
            {
                this.multipleDiscImagesFoundVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a visibility flag indicating whether the multiple raw titles found warning is visible
        /// </summary>
        public Visibility MultipleRawFoundVisibility 
        {
            get
            {
                return this.multipleRawFoundVisibility; 
            } 
            
            set
            {
                this.multipleRawFoundVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a visibility flag indicating whether the multiple content packages found warning is visible
        /// </summary>
        public Visibility MultipleContentPackagesFoundVisibility
        { 
            get
            {
                return this.multipleContentPackagesFoundVisibility; 
            } 
            
            set
            {
                this.multipleContentPackagesFoundVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Xbox 360 settings content should be displayed
        /// </summary>
        public bool Is360Selected
        { 
            get
            {
                return this.is360Selected;
            } 
            
            set
            {
                this.is360Selected = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the current platform
        /// </summary>
        public TCRPlatformViewItem CurrentPlatform 
        { 
            get
            {
                return this.currentPlatform; 
            }
 
            set 
            {
                if (this.currentPlatform != value)
                {
                    this.currentPlatform = value;
                    this.CurrentTCRVersion = null;
                    if (this.currentPlatform == null)
                    {
                        this.PlatformPickedVisibility = Visibility.Collapsed;
                        this.Is360Selected = false;
                    }
                    else
                    {
                        this.CurrentTCRVersionList = value.TCRVersionViewItems;
                        this.PlatformPickedVisibility = Visibility.Visible;
                        this.Is360Selected = value.Name == "Xbox 360";

                        List<TCRVersionViewItem> versionList = this.CurrentTCRVersionList;
                        if (versionList.Count > 0)
                        {
                            this.CurrentTCRVersion = versionList[versionList.Count - 1];
                        }
                    }

                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a visibility flag indicating whether or not items dependant on the platform are visible
        /// </summary>
        public Visibility PlatformPickedVisibility
        {
            get
            {
                return this.platformPickedVisibility; 
            }
            
            set
            {
                this.platformPickedVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a list of TCR Versions for the current platform
        /// </summary>
        public List<TCRVersionViewItem> CurrentTCRVersionList 
        {
            get
            {
                return this.currentTCRVersionList; 
            }

            set 
            {
                this.currentTCRVersionList = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the current TCR Version
        /// </summary>
        public TCRVersionViewItem CurrentTCRVersion 
        {
            get 
            {
                return this.currentTCRVersion;
            }
            
            set
            {
                this.currentTCRVersion = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a list of game install types
        /// </summary>
        public List<string> GameInstallTypeList
        {
            get
            {
                return this.gameInstallTypeList;
            } 
            
            set 
            {
                this.gameInstallTypeList = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets a list of all theme names
        /// </summary>
        public List<string> ThemeNames
        {
            get { return Theme.ThemeNames; }
        }

        /// <summary>
        /// Gets or sets the current theme name
        /// </summary>
        public string CurrentThemeName
        {
            get { return this.CurrentTheme.Name; }
            set { this.CurrentTheme.Copy(new Theme(value)); }
        }

        /// <summary>
        /// Gets the current theme
        /// </summary>
        public Theme CurrentTheme
        {
            get { return MainViewModel.CurrentTheme; }
        }

        /// <summary>
        /// Gets or sets a Command to browse for symbols
        /// </summary>
        public Command BrowseToSymbolsCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to browse to title
        /// </summary>
        public Command BrowseToContentPackageCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to browse to title
        /// </summary>
        public Command BrowseToDemoContentPackageCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to browse to game config
        /// </summary>
        public Command BrowseToGameConfigCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to apply settings
        /// </summary>
        public Command ApplySettingsCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to save and close settings
        /// </summary>
        public Command ApplyAndCloseSettingsCommand { get; set; }
        
        /// <summary>
        /// Gets or sets a Command to close settings
        /// </summary>
        public Command CancelSettingsCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to browse to XDKRecovery exe
        /// </summary>
        public Command BrowseToXDKRecoveryCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to browse to disc image
        /// </summary>
        public Command BrowseToDiscImageCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to browse to root game directory
        /// </summary>
        public Command BrowseToGameDirectoryCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to browse to XDKRecovery exe
        /// </summary>
        public Command BrowseToRawGameDirectoryCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to browse to XDKRecovery exe
        /// </summary>
        public Command BrowseToTitleUpdateCommand { get; set; }

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
        /// Opens the folder browser to select a symbols path
        /// </summary>
        private void BrowseToSymbols()
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.SelectedPath = this.SymbolsPath;
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.SymbolsPath = dlg.SelectedPath;
            }
        }

        /// <summary>
        /// Opens a file browser to select a new content package
        /// </summary>
        private void BrowseToContentPackage()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            bool result = (bool)dlg.ShowDialog();
            if (result)
            {
                this.ContentPackage = Path.GetFullPath(dlg.FileName);
            }
        }

        /// <summary>
        /// Opens a file browser to select a new demo content package
        /// </summary>
        private void BrowseToDemoContentPackage()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            bool result = (bool)dlg.ShowDialog();
            if (result)
            {
                this.DemoContentPackage = Path.GetFullPath(dlg.FileName);
            }
        }

        /// <summary>
        /// Opens a file browser to select a game config file
        /// </summary>
        private void BrowseToGameConfig()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Game Config File|*.gameconfig|All Files|*.*";
            bool result = (bool)dlg.ShowDialog();
            if (result)
            {
                this.GameConfigPath = Path.GetFullPath(dlg.FileName);
                if (!string.IsNullOrEmpty(this.GameConfigPath))
                {
                    // Read XML config file
                    XmlDocument configFile = new XmlDocument();
                    configFile.Load(this.GameConfigPath);
                    if (configFile.DocumentElement.Name == "XboxLiveSubmissionProject")
                    {
                        XmlNode xboxLiveSubmissionProjectNode = configFile.DocumentElement;
                        if (xboxLiveSubmissionProjectNode.ChildNodes.Count > 0)
                        {
                            foreach (XmlNode n in xboxLiveSubmissionProjectNode.ChildNodes)
                            {
                                if (n.Name == "GameConfigProject")
                                {
                                    this.CurrentTitleFriendlyName = n.Attributes["titleName"].InnerText.Trim();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Opens a file browser to select the XDK Recovery program
        /// </summary>
        private void BrowseToXDKRecovery()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "XDKRecoveryXenon|*.exe|All Files|*.*";
            bool result = (bool)dlg.ShowDialog();
            if (result)
            {
                // Check to make sure the version of the recovery program matches the currently installed XDK
                FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo(dlg.FileName);
                string recoveryExeVersion = fileVersion.FileVersion;
                string xdkVersion = XboxDevice.XdkVersion;
                bool useValue = true;
                if (recoveryExeVersion != xdkVersion)
                {
                    MessageBoxResult msgBoxResult = MessageBox.Show(
                        string.Format("The currently installed XDK version is {0}, and this exe's version is {1}, are you sure you want to use this exe?", xdkVersion, recoveryExeVersion),
                        "Version Mismatch",
                        MessageBoxButton.OKCancel,
                        MessageBoxImage.Question);
                    useValue = msgBoxResult == MessageBoxResult.OK;
                }

                if (useValue)
                {
                    this.XDKRecoveryPath = Path.GetFullPath(dlg.FileName);
                }
            }
        }

        /// <summary>
        /// Opens a file browser to select a disc image
        /// </summary>
        private void BrowseToDiscImage()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Disc Images|*.utf;*.xsf;.iso;*.xgd|.utf File|*.utf|.xsf File|*.xsf|.iso File|*.iso|.xgd File|*.xgd|All Files|*.*";
            bool result = (bool)dlg.ShowDialog();
            if (result)
            {
                this.DiscImage = Path.GetFullPath(dlg.FileName);
            }
        }

        /// <summary>
        /// Opens a folder browser to select a raw game directory
        /// </summary>
        private void BrowseToRawGameDirectory()
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.SelectedPath = this.RawGameDirectory;
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.RawGameDirectory = dlg.SelectedPath;
            }
        }

        /// <summary>
        /// Opens folder browser to select a title update folder
        /// </summary>
        private void BrowseToTitleUpdate()
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.SelectedPath = this.TitleUpdatePath;
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.TitleUpdatePath = dlg.SelectedPath;
            }
        }

        /// <summary>
        /// Open a file browser to select a game directory
        /// </summary>
        private void BrowseToGameDirectory()
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();
            dlg.Description = "Select the root folder containing game content.  Other fields will be auto-populated based on its contents.";
            dlg.SelectedPath = this.GameDirectory;
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Use a for loop just so we can break out to do cleanup.
                for (;;)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    string gameDirectory = dlg.SelectedPath;
                    DirectoryInfo dirInfo;
                    try
                    {
                        dirInfo = new DirectoryInfo(dlg.SelectedPath);

                        // Figure out the path to the .gameconfig
                        FileInfo[] files = dirInfo.GetFiles("*.gameconfig", SearchOption.AllDirectories);
                        List<FileInfo> gameConfigFiles = new List<FileInfo>();
                        foreach (FileInfo fileInfo in files)
                        {
                            bool includeThisFile = false;
                            XmlDocument xmlFile = new XmlDocument();
                            xmlFile.Load(fileInfo.FullName);
                            if (xmlFile.DocumentElement.Name == "XboxLiveSubmissionProject")
                            {
                                XmlNode xboxLiveSubmissionProjectNode = xmlFile.DocumentElement;
                                if (xboxLiveSubmissionProjectNode.ChildNodes.Count > 0)
                                {
                                    foreach (XmlNode n in xboxLiveSubmissionProjectNode.ChildNodes)
                                    {
                                        if (n.Name == "GameConfigProject")
                                        {
                                            // Check platform
                                            foreach (XmlNode n2 in n.ChildNodes)
                                            {
                                                if (n2.Name == "ProductInformation")
                                                {
                                                    foreach (XmlNode n3 in n2.ChildNodes)
                                                    {
                                                        if (n3.Name == "Platform")
                                                        {
                                                            string platformName = n3.Attributes["name"].InnerText.Trim();
                                                            if (platformName == "Xbox 360")
                                                            {
                                                                includeThisFile = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            break;
                                        }
                                    }
                                }
                            }

                            if (includeThisFile)
                            {
                                gameConfigFiles.Add(fileInfo);
                            }
                        }

                        bool multi;
                        string gameConfigPath = this.SelectHighestPriorityFile(dlg.SelectedPath, gameConfigFiles, out multi);
                        bool multipleGameConfigFilesFound = multi;
                        if (gameConfigPath.Length == 0)
                        {
                            MessageBox.Show(".gameconfig file for Xbox 360 game could not be found!");
                            break;
                        }

                        // Figure out the path to the XDBs
                        files = dirInfo.GetFiles("*.xdb", SearchOption.AllDirectories);
                        string pathToXdb = string.Empty;
                        foreach (FileInfo fileInfo in files)
                        {
                            string thisXdbPath = Path.GetDirectoryName(fileInfo.FullName);
                            if (string.IsNullOrEmpty(pathToXdb))
                            {
                                pathToXdb = thisXdbPath;
                            }
                            else
                            {
                                while (thisXdbPath != pathToXdb)
                                {
                                    if (thisXdbPath.Length > pathToXdb.Length)
                                    {
                                        thisXdbPath = Path.GetDirectoryName(thisXdbPath);
                                        continue;
                                    }

                                    if (thisXdbPath.Length < pathToXdb.Length)
                                    {
                                        pathToXdb = Path.GetDirectoryName(pathToXdb);
                                        continue;
                                    }

                                    thisXdbPath = Path.GetDirectoryName(thisXdbPath);
                                    pathToXdb = Path.GetDirectoryName(pathToXdb);
                                }
                            }
                        }

                        string symbolsPath = pathToXdb;

                        // Get the game title.
                        // Also get the title ID for use when searching for content directories
                        string titleID = string.Empty;
                        XmlDocument configFile = new XmlDocument();
                        configFile.Load(gameConfigPath);

                        string currentTitleFriendlyName = string.Empty;
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
                                            currentTitleFriendlyName = n.Attributes["titleName"].InnerText.Trim();
                                            titleID = n.Attributes["titleId"].InnerText.Trim();
                                        }
                                        catch (Exception)
                                        {
                                            MessageBox.Show("Unable to read title and titleId from .gamecontent file!");
                                            return;
                                        }

                                        if (titleID.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                                        {
                                            titleID = titleID.Substring(2);
                                        }

                                        break;
                                    }
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(titleID))
                        {
                            MessageBox.Show("Unable to find titleId in gameconfig!");
                            break;
                        }

                        // Set properties once we know we didn't fail to find everything we need
                        this.MultipleGameConfigFilesFound = multipleGameConfigFilesFound;
                        this.MultipleContentPackagesFound = false;
                        this.MultipleDiscImagesFound = false;
                        this.MultipleGameInstallTypesFound = false;
                        this.MultipleRawFound = false;
                        this.MultipleTitleUpdatesFound = false;

                        this.GameDirectory = gameDirectory;
                        this.GameConfigPath = gameConfigPath;
                        this.CurrentTitleFriendlyName = currentTitleFriendlyName;
                        this.SymbolsPath = symbolsPath;

                        // Check for disc image first
                        List<string> extensions = new List<string>();
                        extensions.Add(".utf");
                        extensions.Add(".xsf");
                        extensions.Add(".iso");
                        extensions.Add(".xgd");

                        files = dirInfo.EnumerateFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
                        this.DiscImage = this.SelectHighestPriorityFile(dlg.SelectedPath, files.ToList(), out multi);
                        this.MultipleDiscImagesFound = multi;

                        files = dirInfo.GetFiles("??????????????????", SearchOption.AllDirectories);
                        this.ContentPackage = this.SelectHighestPriorityFile(dlg.SelectedPath, files.ToList(), out multi, delegate(string s) { return s.Contains("0000000000000000\\" + titleID + "\\000D0000\\"); });
                        this.MultipleContentPackagesFound = multi;

                        this.DemoContentPackage = string.Empty;
                        this.UseDemo = false;
                        if (this.ContentPackage.Contains("Content-Full"))
                        {
                            string trialString = Regex.Replace(this.ContentPackage, "Content-Full", "Content-Trial");
                            string path = Path.GetDirectoryName(trialString);
                            DirectoryInfo dirInfo2 = new DirectoryInfo(path);
                            FileInfo[] files2 = dirInfo2.GetFiles(titleID + "????????", SearchOption.AllDirectories);
                            this.DemoContentPackage = this.SelectHighestPriorityFile(path, files2.ToList(), out multi);
                        }

                        files = dirInfo.GetFiles("default.xex", SearchOption.AllDirectories);
                        this.RawGameDirectory = this.SelectHighestPriorityFile(dlg.SelectedPath, files.ToList(), out multi);
                        if (!string.IsNullOrEmpty(this.RawGameDirectory))
                        {
                            this.RawGameDirectory = Path.GetDirectoryName(this.RawGameDirectory);
                        }

                        this.MultipleRawFound = multi;

                        bool hasContentPackage = this.ContentPackage.Length != 0;
                        bool hasDiscImage = this.DiscImage.Length != 0;
                        bool hasRaw = this.RawGameDirectory.Length > 0;

                        this.MultipleGameInstallTypesFound = (hasContentPackage && hasDiscImage) || (hasContentPackage && hasRaw) || (hasDiscImage && hasRaw);
                        if (hasContentPackage)
                        {
                            this.GameInstallType = "Content Package";
                        }
                        else
                        {
                            if (hasDiscImage)
                            {
                                this.GameInstallType = "Disc Emulation";
                            }
                            else
                            {
                                if (hasRaw)
                                {
                                    this.GameInstallType = "Raw";
                                }
                                else
                                {
                                    MessageBox.Show("Unable to auto-detect a content package, disc image, or raw default.xex");
                                    break;
                                }
                            }
                        }

                        files = dirInfo.GetFiles("tu????????_????????", SearchOption.AllDirectories);
                        this.TitleUpdatePath = this.SelectHighestPriorityFile(dlg.SelectedPath, files.ToList(), out multi, delegate(string s) { return s.Contains("$TitleUpdate\\" + titleID + "\\"); });
                        this.MultipleTitleUpdatesFound = multi;
                    }
                    catch (Exception)
                    {
                    }

                    break;
                }

                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// Applies settings
        /// </summary>
        private void ApplySettings()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            bool emulationChanged = false;
            if (MainViewModel.CurrentTitle.GameInstallType == "Disc Emulation")
            {
                emulationChanged = (this.CurrentTitle.GameInstallType != "Disc Emulation") || (this.CurrentTitle.DiscImage != this.MainViewModel.CurrentTitle.DiscImage);
            }

            bool symbolsChanged = this.CurrentTitle.SymbolsPath != this.MainViewModel.CurrentTitle.SymbolsPath;

            this.MainViewModel.CurrentTitle.Copy(this.CurrentTitle);
            this.MainViewModel.CurrentTitleFriendlyName = this.CurrentTitleFriendlyName;
            this.MainViewModel.CurrentPlatform = this.CurrentPlatform;
            this.MainViewModel.CurrentTCRVersion = this.CurrentTCRVersion;

            // Save state
            CAT.Properties.Settings.Default.Symbols = this.CurrentTitle.SymbolsPath;
            CAT.Properties.Settings.Default.ContentPackage = this.CurrentTitle.ContentPackage;
            CAT.Properties.Settings.Default.GameConfig = this.CurrentTitle.GameConfigPath;
            CAT.Properties.Settings.Default.XDKRecoveryPath = this.CurrentTitle.XdkRecoveryPath;
            CAT.Properties.Settings.Default.GameInstallType = this.CurrentTitle.GameInstallType;
            CAT.Properties.Settings.Default.GameDirectory = this.CurrentTitle.GameDirectory;
            CAT.Properties.Settings.Default.RawGameDirectory = this.CurrentTitle.RawGameDirectory;
            CAT.Properties.Settings.Default.TitleUpdatePath = this.CurrentTitle.TitleUpdatePath;
            CAT.Properties.Settings.Default.DiscImage = this.CurrentTitle.DiscImage;
            CAT.Properties.Settings.Default.TitleFriendlyName = this.CurrentTitle.Name;
            Properties.Settings.Default.Save();

            ObservableCollection<XboxViewItem> xboxList = this.mainViewModel.XboxList;
            if (xboxList != null)
            {
                foreach (XboxViewItem xbvi in xboxList)
                {
                    if (emulationChanged)
                    {
                        xbvi.XboxDevice.StopEmulatingDiscTitle();
                    }

                    if (symbolsChanged)
                    {
                        xbvi.XboxDevice.RefreshSymbols();
                    }
                }
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Applies settings and closes settings UI
        /// </summary>
        private void ApplyAndCloseSettings()
        {
            this.ApplySettings();
            this.Settings.Close();
        }

        /// <summary>
        /// Closes settings UI.
        /// </summary>
        private void CancelSettings()
        {
            this.CurrentThemeName = this.oldThemeName;
            this.Settings.Close();
        }

        /// <summary>
        /// A handler called when the window closes
        /// </summary>
        /// <param name="sender">The window that originated the event</param>
        /// <param name="e">A CancelEventArgs object</param>
        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.SettingsLeft = Settings.Left;
            Properties.Settings.Default.SettingsTop = Settings.Top;
        }

        /// <summary>
        /// Given a list of files, decides which file is the most likely match
        /// </summary>
        /// <param name="rootFolder">Root folder all files are within</param>
        /// <param name="files">Files to look for highest priority match in</param>
        /// <param name="resolvedMultiple">Returns a boolean value indicating whether or not multiple were found</param>
        /// <param name="validateFileNameDelegate">An optional delegate used to validate files</param>
        /// <returns>The path to the highest priority file selected from the file set</returns>
        private string SelectHighestPriorityFile(string rootFolder, List<FileInfo> files, out bool resolvedMultiple, ValidateFileNameDelegate validateFileNameDelegate = null)
        {
            string result = string.Empty;

            resolvedMultiple = false;

            List<string> highPriorityStrings = new List<string>();
            List<string> lowPriorityStrings = new List<string>();

            // Read XML config file
            XmlDocument configFile = new XmlDocument();
            configFile.Load(@"Settings.cfg");
            XmlNodeList nodes = configFile.DocumentElement.SelectNodes("/SettingsConfig/HighPriorityPathStrings/String");
            foreach (XmlNode n in nodes)
            {
                highPriorityStrings.Add(n.InnerText);
            }

            nodes = configFile.DocumentElement.SelectNodes("/SettingsConfig/LowPriorityPathStrings/String");
            foreach (XmlNode n in nodes)
            {
                lowPriorityStrings.Add(n.InnerText);
            }

            DateTime candidateDateTime = DateTime.MinValue;
            foreach (FileInfo fileInfo in files)
            {
                int rootLength = rootFolder.Length;   // For the purposes of comparison, ignore the root portion
                string newPath = fileInfo.FullName.Substring(rootLength);
                if ((validateFileNameDelegate == null) || validateFileNameDelegate(newPath))
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = fileInfo.FullName;
                        candidateDateTime = fileInfo.LastWriteTime;
                    }
                    else
                    {
                        resolvedMultiple = true;
                        string oldPath = result.Substring(rootLength);
                        bool done = false;
                        foreach (string s in lowPriorityStrings)
                        {
                            bool inOldPath = oldPath.Contains(s);
                            bool inNewPath = newPath.Contains(s);
                            if (!inNewPath && inOldPath)
                            {
                                result = fileInfo.FullName;
                                candidateDateTime = fileInfo.LastWriteTime;
                            }

                            done = inOldPath != inNewPath;
                            if (done)
                            {
                                break;
                            }
                        }

                        if (!done)
                        {
                            foreach (string s in highPriorityStrings)
                            {
                                bool inOldPath = oldPath.Contains(s);
                                bool inNewPath = newPath.Contains(s);
                                if (inNewPath && !inOldPath)
                                {
                                    result = fileInfo.FullName;
                                    candidateDateTime = fileInfo.LastWriteTime;
                                }

                                done = inOldPath != inNewPath;
                                if (done)
                                {
                                    break;
                                }
                            }
                        }

                        if (!done)
                        {
                            if (fileInfo.LastWriteTime > candidateDateTime)
                            {
                                result = fileInfo.FullName;
                                candidateDateTime = fileInfo.LastWriteTime;
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
