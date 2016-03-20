// -----------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Microsoft">
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
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// MainViewModel is the primary ViewModel
    /// UI is bound to properties in this class.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// A reference to the data model
        /// </summary>
        private static DataModel dataModel = new DataModel();

        /// <summary>
        /// The main window
        /// </summary>
        private Window mainWindow;

        /// <summary>
        /// Backing field for DevicePool property
        /// </summary>
        private ObservableCollection<XboxViewItem> xboxList = new ObservableCollection<XboxViewItem>();

        /// <summary>
        /// Backing field for PlatformPickedVisibility property
        /// </summary>
        private Visibility platformPickedVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for TCRVersionPickedVisibility property
        /// </summary>
        private Visibility tcrVersionPickedVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for TCRVersionNotPickedVisibility property
        /// </summary>
        private Visibility tcrVersionNotPickedVisibility = Visibility.Visible;

        /// <summary>
        /// Backing field for TCRCategoryPickedVisibility property
        /// </summary>
        private Visibility categoryPickedVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for TCRPickedVisibility property
        /// </summary>
        private Visibility tcrPickedVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for TestPickedVisibility property
        /// </summary>
        private Visibility testPickedVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for ModulePickedVisibility property
        /// </summary>
        private Visibility modulePickedVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for TCRSelectorVisibility property
        /// </summary>
        private Visibility tcrSelectorVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for TestSelectorVisibility property
        /// </summary>
        private Visibility testSelectorVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for ModuleSelectorVisibility property
        /// </summary>
        private Visibility moduleSelectorVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for TCRNotSelectedVisibility property
        /// </summary>
        private Visibility tcrNotSelectedVisibility = Visibility.Visible;

        /// <summary>
        /// Backing field for DeviceVisibility property
        /// </summary>
        private Visibility devicePoolVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for DeviceVisibility property
        /// </summary>
        private Visibility navigationPanelVisibility = Visibility.Visible;

        /// <summary>
        /// Backing field for IsFilteringModules property
        /// </summary>xbox
        private bool isFilteringModules = true;

        /// <summary>
        /// Backing field for CurrentTCRVersionList property
        /// </summary>
        private List<TCRVersionViewItem> currentTCRVersionList;

        /// <summary>
        /// Backing field for CurrentTCRCategoryList property
        /// </summary>
        private List<TCRCategoryViewItem> currentTCRCategoryList;

        /// <summary>
        /// Backing field for CurrentTCRList property
        /// </summary>
        private List<TCRViewItem> currentTCRList;

        /// <summary>
        /// Backing field for CurrentTestList property
        /// </summary>
        private List<TCRTestCaseViewItem> currentTestList;

        /// <summary>
        /// Backing field for CurrentModuleList property
        /// </summary>
        private List<ModuleViewItem> currentModuleList;

        /// <summary>
        /// Backing field for CurrentModuleUIElement property
        /// </summary>
        private UIElement currentModuleUIElement;

        /// <summary>
        /// Backing field for CurrentTitleFriendlyName property
        /// </summary>
        private string currentFriendlyTitleName;

        /// <summary>
        /// Backing field for CurrentTitle property
        /// </summary>
        private DataModel.XboxTitle currentTitle = new DataModel.XboxTitle();

        /// <summary>
        /// List of TCR Platforms
        /// </summary>
        private List<TCRPlatformViewItem> platformList = new List<TCRPlatformViewItem>();

        /// <summary>
        /// List of IDevices objects associated with current XboxList
        /// </summary>
        private List<IDevice> poolDevices = new List<IDevice>();

        /// <summary>
        /// Backing field for CurrentPlatform property
        /// </summary>
        private TCRPlatformViewItem currentPlatform;

        /// <summary>
        /// Backing field for CurrentTCRVersion property
        /// </summary>
        private TCRVersionViewItem currentTCRVersion;

        /// <summary>
        /// Backing field for CurrentTCRCategory property
        /// </summary>
        private TCRCategoryViewItem currentTCRCategory;

        /// <summary>
        /// Backing field for CurrentTCR property
        /// </summary>
        private TCRViewItem currentTCR;

        /// <summary>
        /// Backing field for CurrentModule property
        /// </summary>
        private ModuleViewItem currentModule;

        /// <summary>
        /// Text filter of TCRs and Categories
        /// </summary>
        private string textFilter;

        /// <summary>
        /// A value indicating whether the text filter has contents
        /// </summary>
        private bool filterHasContent;

        /// <summary>
        /// Text to log to a module's log file
        /// </summary>
        private string moduleLogTextContent;

        /// <summary>
        /// Backing field for CurrentTest property
        /// </summary>
        private TCRTestCaseViewItem currentTest;

        /// <summary>
        /// Backing field for CurrentTheme property
        /// </summary>
        private Theme currentTheme;

        /// <summary>
        /// Backing field for TextZoom property
        /// </summary>
        private double textZoom = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            // populate platform list
            foreach (DataModel.Platform p in DataModel.GetPlatforms())
            {
                TCRPlatformViewItem pvi = new TCRPlatformViewItem(p, this);
                this.platformList.Add(pvi);
            }

            // Commands
            this.ChangePlatformCommand = new Command((o) => this.SetCurrentPlatform((TCRPlatformViewItem)o));
            this.ChangeTCRVersionCommand = new Command((o) => this.SetCurrentTCRVersion((TCRVersionViewItem)o));
            this.ChangeTCRCategoryCommand = new Command((o) => this.SetCurrentCategory((TCRCategoryViewItem)o));
            this.ChangeTCRCommand = new Command((o) => this.SetCurrentTCR((TCRViewItem)o));
            this.ChangeTestCommand = new Command((o) => this.SetCurrentTest((TCRTestCaseViewItem)o));

            this.RefreshDevicePoolCommand = new Command((o) => this.PopulateDevicePool(this.CurrentPlatform));
            this.ChangeModuleCommand = new Command((o) => this.ChangeModule((ModuleViewItem)o));
            this.FinishModuleCommand = new Command((o) => this.FinishModule());
            this.OpenSettingsCommand = new Command((o) => this.OpenSettings());
            this.AddDeviceCommand = new Command((o) => this.AddDevice());

            this.ModuleLogTextCommand = new Command((o) => this.ModuleLogText());
        }

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a reference to the data model
        /// </summary>
        public static DataModel DataModel
        {
            get { return dataModel; }
        }

        /// <summary>
        /// Gets a reference to the main window
        /// </summary>
        public Window MainWindow 
        {
            get { return this.mainWindow; } 
        }

        /// <summary>
        /// Gets a list of TCR Platforms
        /// </summary>
        public List<TCRPlatformViewItem> PlatformList 
        {
            get { return this.platformList; } 
        }

        /// <summary>
        /// Gets a list of IDevices objects associated with current XboxList
        /// </summary>
        public List<IDevice> PoolDevices
        {
            get { return this.poolDevices; } 
        }

        /// <summary>
        /// Gets or sets of list of devices
        /// </summary>
        public ObservableCollection<XboxViewItem> XboxList
        {
            get 
            {
                return this.xboxList; 
            }

            set
            {
                if (this.xboxList != value)
                {
                    this.PoolDevices.Clear();
                    if (this.xboxList != null)
                    {
                        foreach (XboxViewItem xvi in this.xboxList)
                        {
                            XboxDevice xb = xvi.XboxDevice;
                            if (xvi.DebugOutputViewModel != null)
                            {
                                xvi.DebugOutputViewModel.Close();
                            }

                            if (xvi.ProfileManagerViewModel != null)
                            {
                                xvi.ProfileManagerViewModel.Close();
                            }

                            xvi.XboxDevice = null;
                            xb.Disconnect();
                        }
                    }

                    this.xboxList = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the version of the CAT
        /// </summary>
        public string Version
        { 
            get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a platform has been picked
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
        /// Gets or sets a value indicating whether or not a TCR Version has been picked
        /// </summary>
        public Visibility TCRVersionPickedVisibility 
        {
            get
            { 
                return this.tcrVersionPickedVisibility; 
            } 
            
            set 
            {
                this.tcrVersionPickedVisibility = value;
                this.NotifyPropertyChanged();
            } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a TCR Version has NOT been picked
        /// </summary>
        public Visibility TCRVersionNotPickedVisibility 
        {
            get
            { 
                return this.tcrVersionNotPickedVisibility; 
            }
            
            set
            {
                this.tcrVersionNotPickedVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a TCR Category has been picked
        /// </summary>
        public Visibility TCRCategoryPickedVisibility 
        {
            get 
            {
                return this.categoryPickedVisibility; 
            }
            
            set 
            {
                this.categoryPickedVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a TCR has been picked
        /// </summary>
        public Visibility TCRPickedVisibility 
        {
            get 
            {
                return this.tcrPickedVisibility; 
            }
            
            set 
            {
                this.tcrPickedVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a Test has been picked
        /// </summary>
        public Visibility TestPickedVisibility
        {
            get
            {
                return this.testPickedVisibility;
            }
            
            set 
            {
                this.testPickedVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a Module has been picked
        /// </summary>
        public Visibility ModulePickedVisibility 
        {
            get
            {
                return this.modulePickedVisibility; 
            }

            set
            { 
                this.modulePickedVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the TCR Selector should be visible. (A Version and/or Category has been selected).
        /// </summary>
        public Visibility TCRSelectorVisibility 
        {
            get
            { 
                return this.tcrSelectorVisibility;
            }

            set 
            { 
                this.tcrSelectorVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Test Case selector should be visible.
        /// </summary>
        public Visibility TestSelectorVisibility 
        {
            get
            { 
                return this.testSelectorVisibility; 
            }

            set 
            {
                this.testSelectorVisibility = value;
                this.NotifyPropertyChanged();
            } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a Module selector should be visible.
        /// </summary>
        public Visibility ModuleSelectorVisibility 
        { 
            get
            { 
                return this.moduleSelectorVisibility; 
            }

            set
            { 
                this.moduleSelectorVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not a TCR is NOT selected.
        /// </summary>
        public Visibility TCRNotSelectedVisibility 
        {
            get 
            {
                return this.tcrNotSelectedVisibility; 
            }

            set 
            {
                this.tcrNotSelectedVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Xbox List should be visible
        /// </summary>
        public Visibility DevicePoolVisibility 
        {
            get 
            {
                return this.devicePoolVisibility;
            }

            set 
            {
                this.devicePoolVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the navigation panel should be visible
        /// </summary>
        public Visibility NavigationPanelVisibility
        {
            get 
            {
                return this.navigationPanelVisibility;
            }

            set 
            {
                this.navigationPanelVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not content should be filtered by existence of modules
        /// </summary>
        public bool IsFilteringModules
        {
            get
            {
                return this.isFilteringModules;
            }

            set
            { 
                this.isFilteringModules = value;
                this.NotifyPropertyChanged();
                this.ApplyTextFilter(this.textFilter); 
            }
        }

        /// <summary>
        /// Gets or sets a TCR Version list specific to the currently selected platform.
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
        /// Gets or sets a TCR Category list specific to the currently selected platform and version.
        /// </summary>
        public List<TCRCategoryViewItem> CurrentTCRCategoryList
        {
            get
            { 
                return this.currentTCRCategoryList; 
            }

            set
            { 
                this.currentTCRCategoryList = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a TCR list specific to the currently selected platform, version and/or category.
        /// </summary>
        public List<TCRViewItem> CurrentTCRList 
        {
            get 
            {
                return this.currentTCRList; 
            }

            set
            { 
                this.currentTCRList = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a TCR Test Case list specific to the currently selected TCR
        /// </summary>
        public List<TCRTestCaseViewItem> CurrentTestList
        {
            get
            { 
                return this.currentTestList; 
            }

            set
            { 
                this.currentTestList = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a Module list specific to the currently selected test case
        /// </summary>
        public List<ModuleViewItem> CurrentModuleList
        { 
            get
            { 
                return this.currentModuleList; 
            }
 
            set
            { 
                this.currentModuleList = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the UI Element of the current module
        /// </summary>
        public UIElement CurrentModuleUIElement
        {
            get
            { 
                return this.currentModuleUIElement; 
            }

            set
            { 
                this.currentModuleUIElement = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the friendly name of the currently configured title
        /// </summary>
        public string CurrentTitleFriendlyName 
        {
            get
            { 
                return this.currentFriendlyTitleName;
            }

            set
            { 
                this.currentFriendlyTitleName = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the current XboxTitle
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
        /// Gets or sets the currently selected Platform
        /// </summary>
        public TCRPlatformViewItem CurrentPlatform 
        {
            get { return this.currentPlatform; }
            set { this.SetCurrentPlatform(value); } 
        }

        /// <summary>
        /// Gets or sets the currently selected TCR Version
        /// </summary>
        public TCRVersionViewItem CurrentTCRVersion
        {
            get { return this.currentTCRVersion; }
            set { this.SetCurrentTCRVersion(value); } 
        }

        /// <summary>
        /// Gets or sets the currently selected TCR Category
        /// </summary>
        public TCRCategoryViewItem CurrentTCRCategory 
        {
            get { return this.currentTCRCategory; }
            set { this.SetCurrentCategory(value); } 
        }

        /// <summary>
        /// Gets or sets the currently selected TCR
        /// </summary>
        public TCRViewItem CurrentTCR 
        { 
            get { return this.currentTCR; }
            set { this.SetCurrentTCR(value); }
        }

        /// <summary>
        /// Gets or sets the currently selected Test Case
        /// </summary>
        public TCRTestCaseViewItem CurrentTest 
        {
            get { return this.currentTest; }
            set { this.SetCurrentTest(value); } 
        }

        /// <summary>
        /// Gets or sets the currently selected Module
        /// </summary>
        public ModuleViewItem CurrentModule 
        { 
            get { return this.currentModule; }
            set { this.SetCurrentModule(value); } 
        }

        /// <summary>
        /// Gets or sets a Command to set the current platform
        /// </summary>
        public Command ChangePlatformCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to set the current TCR Version
        /// </summary>
        public Command ChangeTCRVersionCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to set the current TCR Category
        /// </summary>
        public Command ChangeTCRCategoryCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to set the current TCR
        /// </summary>
        public Command ChangeTCRCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to set the current Test Case
        /// </summary>
        public Command ChangeTestCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to set/start a module
        /// </summary>
        public Command ChangeModuleCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to stop the current module
        /// </summary>
        public Command FinishModuleCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to refresh the device pool
        /// </summary>
        public Command RefreshDevicePoolCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to change the settings windows
        /// </summary>
        public Command OpenSettingsCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to add a new device to the device pool
        /// </summary>
        public Command AddDeviceCommand { get; set; }

        /// <summary>
        /// Gets or sets a Command to add a new device to the device pool
        /// </summary>
        public Command ModuleLogTextCommand { get; set; }

        /// <summary>
        /// Gets or sets a the TCR/Category test filter
        /// </summary>
        public string TextFilter 
        {
            get
            { 
                return this.textFilter;
            } 
            
            set
            {
                this.textFilter = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the text filter has contents
        /// </summary>
        public bool FilterHasContent 
        { 
            get
            { 
                return this.filterHasContent;
            }

            set
            { 
                this.filterHasContent = value;
                this.NotifyPropertyChanged(); 
            } 
        }

        /// <summary>
        /// Gets or sets a string to log to the module's log file
        /// </summary>
        public string ModuleLogTextContent
        { 
            get
            {
                return this.moduleLogTextContent;
            } 
            
            set
            { 
                this.moduleLogTextContent = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the current theme
        /// </summary>
        public Theme CurrentTheme
        {
            get
            {
                return this.currentTheme;
            } 
            
            set
            {
                this.currentTheme = value;                
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the text zoom percentage of TCR and CTC text
        /// </summary>
        public double TextZoom
        {
            get
            {
                return this.textZoom;
            }

            set
            {
                this.textZoom = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a reference to the virtual controller view model
        /// </summary>
        public VirtualControllerViewModel VirtualControllerViewModel { get; set; }

        /// <summary>
        /// Called when the main window is loaded
        /// </summary>
        /// <param name="sender">Window associated with the close event</param>
        /// <param name="e">RoutedEventArgs for window load event</param>
        public void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.mainWindow = sender as Window;
            
            string themeName = "Default";

            // restore Settings
            this.currentTitle.SymbolsPath = Properties.Settings.Default.Symbols;
            this.currentTitle.ContentPackage = Properties.Settings.Default.ContentPackage;
            this.currentTitle.GameConfigPath = Properties.Settings.Default.GameConfig;
            this.currentTitle.Name = Properties.Settings.Default.TitleFriendlyName;
            this.currentTitle.XdkRecoveryPath = Properties.Settings.Default.XDKRecoveryPath;
            this.currentTitle.GameInstallType = Properties.Settings.Default.GameInstallType;
            this.currentTitle.GameDirectory = Properties.Settings.Default.GameDirectory;
            this.currentTitle.RawGameDirectory = Properties.Settings.Default.RawGameDirectory;
            this.currentTitle.DiscImage = Properties.Settings.Default.DiscImage;
            this.currentTitle.TitleUpdatePath = Properties.Settings.Default.TitleUpdatePath;
            this.currentTitle.DemoContentPackage = Properties.Settings.Default.DemoContentPackage;
            this.currentTitle.UseDemo = Properties.Settings.Default.UseDemo;
            this.CurrentTitleFriendlyName = this.currentTitle.Name;
            this.IsFilteringModules = Properties.Settings.Default.ShowOnlyModules;
            this.TextZoom = Properties.Settings.Default.TextZoom;
            themeName = Properties.Settings.Default.Theme;

            this.CurrentTheme = new Theme(themeName);

            // restore last platform choice
            if (!string.IsNullOrEmpty(Properties.Settings.Default.Platform))
            {
                foreach (TCRPlatformViewItem pvi in this.PlatformList)
                {
                    if (pvi.Platform.Name == Properties.Settings.Default.Platform)
                    {
                        this.CurrentPlatform = pvi;

                        // if there was a saved platform, restore version 
                        if (!string.IsNullOrEmpty(Properties.Settings.Default.TCRVersion))
                        {
                            foreach (TCRVersionViewItem vvi in pvi.TCRVersionViewItems)
                            {
                                if (vvi.TCRVersion.Name == Properties.Settings.Default.TCRVersion)
                                {
                                    this.CurrentTCRVersion = vvi;
                                    if (!string.IsNullOrEmpty(Properties.Settings.Default.TCRCategory))
                                    {
                                        foreach (TCRCategoryViewItem cvi in vvi.TCRCategoryViewItems)
                                        {
                                            if (cvi.TCRCategory.Name == Properties.Settings.Default.TCRCategory)
                                            {
                                                this.CurrentTCRCategory = cvi;
                                                foreach (TCRViewItem tcrvi in cvi.TCRViewItems)
                                                {
                                                    if (tcrvi.TCR.Name == Properties.Settings.Default.TCR)
                                                    {
                                                        this.CurrentTCR = tcrvi;
                                                        foreach (TCRTestCaseViewItem tcvi in tcrvi.TCRTestCaseViewItems)
                                                        {
                                                            if (tcvi.TCRTestCase.Name == Properties.Settings.Default.CTC)
                                                            {
                                                                this.CurrentTest = tcvi;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this.CurrentPlatform == null)
            {
                List<TCRPlatformViewItem> platformList = this.PlatformList;
                if (platformList.Count > 0)
                {
                    this.CurrentPlatform = platformList[platformList.Count - 1];
                    List<TCRVersionViewItem> versions = this.CurrentTCRVersionList;
                    if (versions.Count > 0)
                    {
                        this.CurrentTCRVersion = versions[versions.Count - 1];
                    }
                }
            }
        }

        /// <summary>
        /// Called when the main window is closed
        /// </summary>
        /// <param name="sender">Window associated with the close event</param>
        /// <param name="e">CancelEventArgs for Window close event</param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            // Save state
            Properties.Settings.Default.Symbols = this.currentTitle.SymbolsPath;
            Properties.Settings.Default.ContentPackage = this.currentTitle.ContentPackage;
            Properties.Settings.Default.GameConfig = this.currentTitle.GameConfigPath;
            Properties.Settings.Default.XDKRecoveryPath = this.currentTitle.XdkRecoveryPath;
            Properties.Settings.Default.GameInstallType = this.currentTitle.GameInstallType;
            Properties.Settings.Default.GameDirectory = this.currentTitle.GameDirectory;
            Properties.Settings.Default.RawGameDirectory = this.currentTitle.RawGameDirectory;
            Properties.Settings.Default.TitleUpdatePath = this.currentTitle.TitleUpdatePath;
            Properties.Settings.Default.UseDemo = this.currentTitle.UseDemo;
            Properties.Settings.Default.DemoContentPackage = this.currentTitle.DemoContentPackage;
            Properties.Settings.Default.DiscImage = this.currentTitle.DiscImage;
            Properties.Settings.Default.TitleFriendlyName = this.currentTitle.Name;
            Properties.Settings.Default.ShowOnlyModules = this.IsFilteringModules;
            Properties.Settings.Default.Theme = this.CurrentTheme.Name;
            Properties.Settings.Default.TextZoom = this.TextZoom;
            if (this.MainWindow.WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                Properties.Settings.Default.Top = this.MainWindow.RestoreBounds.Top;
                Properties.Settings.Default.Left = this.MainWindow.RestoreBounds.Left;
                Properties.Settings.Default.Height = this.MainWindow.RestoreBounds.Height;
                Properties.Settings.Default.Width = this.MainWindow.RestoreBounds.Width;
            }
            else
            {
                Properties.Settings.Default.Top = this.MainWindow.Top;
                Properties.Settings.Default.Left = this.MainWindow.Left;
                Properties.Settings.Default.Height = this.MainWindow.Height;
                Properties.Settings.Default.Width = this.MainWindow.Width;
            }

            CAT.Properties.Settings.Default.Platform = null;
            if (this.currentPlatform != null)
            {
                CAT.Properties.Settings.Default.Platform = this.currentPlatform.Platform.Name;
            }

            CAT.Properties.Settings.Default.TCRVersion = null;
            if (this.currentTCRVersion != null)
            {
                CAT.Properties.Settings.Default.TCRVersion = this.currentTCRVersion.TCRVersion.Name;
            }

            Properties.Settings.Default.TCRCategory = null;
            if (this.currentTCRCategory != null)
            {
                Properties.Settings.Default.TCRCategory = this.currentTCRCategory.TCRCategory.Name;
            }

            Properties.Settings.Default.TCR = null;
            if (this.currentTCR != null)
            {
                Properties.Settings.Default.TCR = this.currentTCR.TCR.Name;
            }

            Properties.Settings.Default.CTC = null;
            if (this.currentTest != null)
            {
                Properties.Settings.Default.CTC = this.currentTest.TCRTestCase.Name;
            }

            Properties.Settings.Default.Save();

            this.CurrentPlatform = null;
            this.XboxList = null;

            Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            Window w = sender as Window;
            Dispatcher dispatcher = w.Dispatcher;
            if (dispatcher != null)
            {
                ShutdownSynchronization.Shutdown(dispatcher);
            }
        }

        /// <summary>
        /// Applies a text file to the navigation panel
        /// </summary>
        /// <param name="filterText">Text to filter on</param>
        public void ApplyTextFilter(string filterText)
        {
            this.textFilter = filterText;
            if (string.IsNullOrEmpty(filterText))
            {
                if (this.CurrentTCRCategoryList != null)
                {
                    foreach (TCRCategoryViewItem cvi in this.CurrentTCRCategoryList)
                    {
                        cvi.MatchesFilter = true;
                        bool expandedCategory = cvi.IsSelected;
                        foreach (TCRViewItem tvi in cvi.TCRViewItems)
                        {
                            tvi.MatchesFilter = true;
                            if (tvi.IsSelected)
                            {
                                expandedCategory = true;
                            }
                        }

                        cvi.IsExpanded = expandedCategory;
                    }
                }
            }
            else
            {
                foreach (TCRCategoryViewItem cvi in this.CurrentTCRCategoryList)
                {
                    if (cvi.Name.ToLower().Contains(this.textFilter.ToLower()))
                    {
                        cvi.MatchesFilter = true;
                        cvi.IsExpanded = true;

                        // Show all modules under the matching 
                        foreach (TCRViewItem tvi in cvi.TCRViewItems)
                        {
                            if (!this.IsFilteringModules || tvi.HasModules)
                            {
                                tvi.MatchesFilter = true;
                            }
                        }
                    }
                    else
                    {
                        cvi.MatchesFilter = false;
                        foreach (TCRViewItem tvi in cvi.TCRViewItems)
                        {
                            if (tvi.IdAndName.ToLower().Contains(this.textFilter.ToLower()))
                            {
                                if (!this.IsFilteringModules || tvi.HasModules)
                                {
                                    tvi.MatchesFilter = true;
                                    cvi.MatchesFilter = true;
                                    cvi.IsExpanded = true;
                                }
                            }
                            else
                            {
                                tvi.MatchesFilter = false;
                            }
                        }
                    }
                }
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

        /// <summary>
        /// Navigates out of the current module 
        /// </summary>
        /// <param name="mvi">Module to change to</param>
        private void ChangeModule(ModuleViewItem mvi)
        {
            bool doChange = true;
            if (this.CurrentModule != null)
            {
                MessageBoxResult msgResult = MessageBox.Show(
                    "Are you sure you want to abort,\nand exit this module?",
                    "Abort Module Confirmation",
                    MessageBoxButton.OKCancel, 
                    MessageBoxImage.Question);
                doChange = msgResult == MessageBoxResult.OK;
            }

            if (doChange)
            {
                this.CurrentModule = mvi;
            }
        }

        /// <summary>
        /// Completes a module and opens its log directory
        /// </summary>
        private void FinishModule()
        {
            if (this.CurrentModule != null)
            {
                MessageBoxResult msgResult = MessageBox.Show(
                    "Are you sure you want to generate log\nresults, and open them?",
                    "Generate Log Confirmation",
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Question);

                if (msgResult == MessageBoxResult.OK)
                {
                    if (!string.IsNullOrEmpty(this.CurrentModule.ModuleContext.LogDirectory))
                    {
                        Process.Start(this.CurrentModule.ModuleContext.LogDirectory);
                    }

                    this.CurrentModule = null;
                }
            }
        }

        /// <summary>
        /// Opens the settings UI
        /// </summary>
        private void OpenSettings()
        {
            Settings settings = new Settings();
            settings.DataContext = new SettingsViewModel(settings, this, new DataModel.XboxTitle(this.currentTitle));
            settings.ShowDialog();
        }

        /// <summary>
        /// Opens the AddDevice() wizard
        /// </summary>
        private void AddDevice()
        {
            XboxDevice.AddDevice();
            this.PopulateDevicePool(this.CurrentPlatform);
        }

        /// <summary>
        /// Logs user-specified text to the module's log
        /// </summary>
        private void ModuleLogText()
        {
            if ((this.CurrentModule != null) && (!string.IsNullOrEmpty(this.ModuleLogTextContent)))
            {
                this.CurrentModule.ModuleContext.Log("User Logged: " + this.ModuleLogTextContent);
                this.ModuleLogTextContent = string.Empty;
            }
        }

        /// <summary>
        /// Populates list of devices.
        /// Currently only supports Xbox 360 platform
        /// </summary>
        /// <param name="p">Platform to populate the device list for</param>
        private void PopulateDevicePool(TCRPlatformViewItem p)
        {
            List<XboxViewItem> foundItems = new List<XboxViewItem>();
            if (p.Name == "Xbox 360")
            {
                // Populate list of Xboxes
                foreach (XboxDevice xb in XboxDevice.GetXboxList(this.CurrentTitle))
                {
                    // Check if already exists in the list
                    bool alreadyInList = false;
                    foreach (XboxViewItem xbvi2 in this.xboxList)
                    {
                        if (xbvi2.XboxDevice.ConnectTo == xb.ConnectTo)
                        {
                            foundItems.Add(xbvi2);
                            alreadyInList = true;
                            xbvi2.ExpireRetryTimerNow();
                            break;
                        }
                    }

                    if (!alreadyInList)
                    {
                        this.PoolDevices.Add(xb);
                        XboxViewItem xbvi = new XboxViewItem(xb, this);
                        xb.XboxViewItem = xbvi;
                        foundItems.Add(xbvi);
                        this.xboxList.Add(xbvi);
                    }
                }

                // Remove anything from xboxList that is not also in foundItems
                List<XboxViewItem> itemsToRemove = new List<XboxViewItem>();
                foreach (XboxViewItem xbvi3 in this.xboxList)
                {
                    bool found = false;
                    foreach (XboxViewItem xbvi4 in foundItems)
                    {
                        if (xbvi4 == xbvi3)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        itemsToRemove.Add(xbvi3);
                    }
                }

                foreach (XboxViewItem xbvi5 in itemsToRemove)
                {
                    xbvi5.XboxDevice.Disconnect();
                    this.xboxList.Remove(xbvi5);
                    this.PoolDevices.Remove(xbvi5.XboxDevice);
                }

                this.DevicePoolVisibility = Visibility.Visible;
            }
            else
            {
                this.DevicePoolVisibility = Visibility.Collapsed;
            }

            this.NotifyPropertyChanged("DevicePool");
        }

        /// <summary>
        /// Selects a platform
        /// </summary>
        /// <param name="platform">Platform to select</param>
        private void SetCurrentPlatform(TCRPlatformViewItem platform)
        {
            if (this.CurrentPlatform != platform)
            {
                if (this.currentPlatform != null)
                {
                    // Deselect the current platform
                    this.currentPlatform.IsSelected = false;    
                }

                if (platform != null && platform.Name.Contains("Xbox 360"))
                {
                    try
                    {
                        if (XboxDevice.IsXdkInstalled)
                        {
                            XboxDevice.AppendXdkToolPathToExecutingEnvironment();
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(
                            "CAT could not find some parts of the Xbox 360 SDK.\n\nXbox 360 tests may not be able to run.\n\nThe error message is: " + e.Message,
                            "Xbox 360 SDK Error",
                            MessageBoxButton.OK, 
                            MessageBoxImage.Error);
                    }
                }

                this.currentPlatform = platform;
                if (platform == null)
                {
                    // If setting platform to null, remove platform picked visibility and clear the TCR Version List
                    this.PlatformPickedVisibility = Visibility.Collapsed;
                    this.CurrentTCRVersionList = null;
                }
                else
                {
                    platform.IsSelected = true;
                    this.PlatformPickedVisibility = Visibility.Visible;

                    // Populate TCR Version list
                    this.CurrentTCRVersionList = platform.TCRVersionViewItems;
                    this.PopulateDevicePool(platform);
                }

                // Clear current TCR Version selection
                this.CurrentTCRVersion = null;
                this.TCRVersionPickedVisibility = Visibility.Collapsed;
                this.TCRVersionNotPickedVisibility = Visibility.Visible;

                this.NotifyPropertyChanged("CurrentPlatform");
            }
        }

        /// <summary>
        /// Selects a TCR Version
        /// </summary>
        /// <param name="tcrVersion">TCR Version to select</param>
        private void SetCurrentTCRVersion(TCRVersionViewItem tcrVersion)
        {
            if (this.currentTCRVersion != tcrVersion)
            {
                if (this.currentTCRVersion != null)
                {
                    this.currentTCRVersion.IsSelected = false;  // Deselect the current TCR Version
                }

                this.currentTCRVersion = tcrVersion;
                if (tcrVersion == null)
                {
                    // If setting TCR Version to null, remove SDK picked visibility and clear TCRCategory and TCR lists
                    this.TCRVersionPickedVisibility = Visibility.Collapsed;
                    this.TCRVersionNotPickedVisibility = Visibility.Visible;
                    this.CurrentTCRCategoryList = null;
                    this.CurrentTCRList = null;
                }
                else
                {
                    this.TCRVersionPickedVisibility = Visibility.Visible;
                    this.TCRVersionNotPickedVisibility = Visibility.Collapsed;

                    // Populate both the category list and the TCR list
                    this.CurrentTCRCategoryList = tcrVersion.TCRCategoryViewItems;
                    this.CurrentTCRList = tcrVersion.TCRViewItems;

                    // Select related platform, and make platform selection visible, in case not already.
                    if (this.currentPlatform != tcrVersion.TCRPlatformViewItem)
                    {
                        this.currentPlatform = tcrVersion.TCRPlatformViewItem;
                        this.NotifyPropertyChanged("CurrentPlatform");
                        this.PlatformPickedVisibility = Visibility.Visible;
                        this.PopulateDevicePool(this.currentPlatform);
                    }

                    this.currentPlatform.IsSelected = false;
                    this.currentTCRVersion.IsSelected = true;
                }

                // Clear current TCRCategory selection
                this.CurrentTCRCategory = null;
                this.TCRCategoryPickedVisibility = Visibility.Collapsed;

                this.NotifyPropertyChanged("CurrentTCRVersion");
            }
        }

        /// <summary>
        /// Selects a TCR Category
        /// </summary>
        /// <param name="category">TCR Category to select</param>
        private void SetCurrentCategory(TCRCategoryViewItem category)
        {
            if (this.currentTCRCategory != null)
            {
                this.currentTCRCategory.IsSelected = false;
            }

            this.currentTCRCategory = category;
            if (category == null)
            {
                // If setting TCRCategory to null, remove TCRCategory picked visibility and restore TCR to unfiltered list
                this.TCRCategoryPickedVisibility = Visibility.Collapsed;

                if (this.currentTCRVersion != null)
                {
                    this.CurrentTCRList = this.currentTCRVersion.TCRViewItems;
                }
            }
            else
            {
                this.TCRCategoryPickedVisibility = Visibility.Visible;

                // Populate TCR List
                this.CurrentTCRList = category.TCRViewItems;

                // Select related version+platform, and make version+platform selections visible, in case not already.
                if (this.currentPlatform != category.TCRVersionViewItem.TCRPlatformViewItem)
                {
                    this.currentPlatform = category.TCRVersionViewItem.TCRPlatformViewItem;
                    this.NotifyPropertyChanged("CurrentPlatform");
                    this.PlatformPickedVisibility = Visibility.Visible;
                    this.PopulateDevicePool(this.currentPlatform);
                }

                this.currentPlatform.IsSelected = false;

                this.currentTCRVersion = category.TCRVersionViewItem;
                this.NotifyPropertyChanged("CurrentTCRVersion");
                this.TCRVersionPickedVisibility = Visibility.Visible;
                this.TCRVersionNotPickedVisibility = Visibility.Collapsed;
                this.currentTCRVersion.IsSelected = false;

                this.currentTCRCategory.IsSelected = true;
                this.currentTCRCategory.IsExpanded = true;
            }

            // Clear current TCR selection
            this.CurrentTCR = null;
            this.TCRPickedVisibility = Visibility.Collapsed;

            this.NotifyPropertyChanged("CurrentCategory");
        }

        /// <summary>
        /// Selects a TCR
        /// </summary>
        /// <param name="tcr">TCR to select</param>
        private void SetCurrentTCR(TCRViewItem tcr)
        {
            if (this.currentTCR != null)
            {
                this.currentTCR.IsSelected = false;
            }

            this.currentTCR = tcr;
            if (tcr == null)
            {
                // If setting TCR to null, remove TCR picked visibility and restore Test to unfiltered list
                this.TCRPickedVisibility = Visibility.Collapsed;
                this.CurrentTestList = null;
                this.TCRNotSelectedVisibility = Visibility.Visible;

                // If the version is currently selected, ensure the TCRSelector panel is visible
                this.TCRSelectorVisibility = this.TCRVersionPickedVisibility;
            }
            else
            {
                this.TCRPickedVisibility = Visibility.Visible;
                this.TestSelectorVisibility = Visibility.Visible;
                this.TCRNotSelectedVisibility = Visibility.Collapsed;

                // Since TCR is selected, hide TCR Selector panel
                this.TCRSelectorVisibility = Visibility.Collapsed;

                // Populate Test List
                this.CurrentTestList = tcr.TCRTestCaseViewItems;

                // Select related version+platform+category, and make version+platform+category selections visible, in case not already.
                if (this.currentPlatform != tcr.TCRCategoryViewItem.TCRVersionViewItem.TCRPlatformViewItem)
                {
                    this.currentPlatform = tcr.TCRCategoryViewItem.TCRVersionViewItem.TCRPlatformViewItem;
                    this.NotifyPropertyChanged("CurrentPlatform");
                    this.PlatformPickedVisibility = Visibility.Visible;
                    this.PopulateDevicePool(this.currentPlatform);
                }

                this.currentPlatform.IsSelected = false;

                this.currentTCRVersion = tcr.TCRCategoryViewItem.TCRVersionViewItem;
                this.NotifyPropertyChanged("CurrentTCRVersion");
                this.TCRVersionPickedVisibility = Visibility.Visible;
                this.TCRVersionNotPickedVisibility = Visibility.Collapsed;
                this.currentTCRVersion.IsSelected = false;

                this.currentTCRCategory = tcr.TCRCategoryViewItem;
                this.NotifyPropertyChanged("CurrentTCRCategory");
                this.TCRCategoryPickedVisibility = Visibility.Visible;
                this.currentTCRCategory.IsSelected = false;
                this.currentTCRCategory.IsExpanded = true;

                this.currentTCR.IsSelected = true;
            }

            // Clear current Test selection
            this.CurrentTest = null;
            this.TestPickedVisibility = Visibility.Collapsed;

            this.NotifyPropertyChanged("CurrentTCR");
        }

        /// <summary>
        /// Selects a Test Case
        /// </summary>
        /// <param name="test">Test case to select</param>
        private void SetCurrentTest(TCRTestCaseViewItem test)
        {
            if (this.currentTest != null)
            {
                this.currentTest.IsSelected = false;
            }

            this.currentTest = test;
            if (test == null)
            {
                // If setting Test to null, remove Test picked visibility and restore Module to unfiltered list
                this.TestPickedVisibility = Visibility.Collapsed;
                this.CurrentModuleList = null;

                // If the TCR is currently selected, ensure the Test Selector panel is visible
                this.TestSelectorVisibility = this.TCRPickedVisibility;
            }
            else
            {
                this.TestPickedVisibility = Visibility.Visible;
                this.ModuleSelectorVisibility = Visibility.Visible;

                // Since Test is selected, hide Test Selector panel
                this.TestSelectorVisibility = Visibility.Collapsed;

                // Populate Module List
                this.CurrentModuleList = test.CATModuleViewItems;

                // Select related version+platform+category+TCR, and make version+platform+category+TCR selections visible, in case not already.
                if (this.currentPlatform != test.TCRViewItem.TCRCategoryViewItem.TCRVersionViewItem.TCRPlatformViewItem)
                {
                    this.currentPlatform = test.TCRViewItem.TCRCategoryViewItem.TCRVersionViewItem.TCRPlatformViewItem;
                    this.NotifyPropertyChanged("CurrentPlatform");
                    this.PlatformPickedVisibility = Visibility.Visible;
                    this.PopulateDevicePool(this.currentPlatform);
                }

                this.currentPlatform.IsSelected = false;

                this.currentTCRVersion = test.TCRViewItem.TCRCategoryViewItem.TCRVersionViewItem;
                this.NotifyPropertyChanged("CurrentTCRVersion");
                this.TCRVersionPickedVisibility = Visibility.Visible;
                this.TCRVersionNotPickedVisibility = Visibility.Collapsed;
                this.currentTCRVersion.IsSelected = false;

                this.currentTCRCategory = test.TCRViewItem.TCRCategoryViewItem;
                this.NotifyPropertyChanged("CurrentTCRCategory");
                this.TCRCategoryPickedVisibility = Visibility.Visible;
                this.currentTCRCategory.IsSelected = false;
                this.currentTCRCategory.IsExpanded = true;

                this.currentTCR = test.TCRViewItem;
                this.NotifyPropertyChanged("CurrentTCR");
                this.TCRPickedVisibility = Visibility.Visible;
                this.currentTCR.IsSelected = true;

                this.currentTest.IsSelected = true;
            }

            // Clear current Module selection
            this.CurrentModule = null;
            this.ModulePickedVisibility = Visibility.Collapsed;

            this.NotifyPropertyChanged("CurrentTest");
        }

        /// <summary>
        /// Selects a module
        /// </summary>
        /// <param name="module">module to select</param>
        private void SetCurrentModule(ModuleViewItem module)
        {
            if (this.currentModule != null)
            {
                this.currentModule.IsSelected = false;
                if (this.currentModule.ModuleInstance != null)
                {
                    this.currentModule.ModuleInstance.Stop();
                    this.currentModule.ModuleContext.EndLog();
                    this.currentModule.ModuleContext.IsModal = false;
                }
            }

            this.currentModule = module;
            this.CurrentModuleUIElement = null;
            if (module == null)
            {
                // If setting Test to null, remove Test picked visibility and restore Module to unfiltered list
                this.ModulePickedVisibility = Visibility.Collapsed;

                // If the TestCase is currently selected, ensure the Module Selector panel is visible
                this.ModuleSelectorVisibility = this.TestPickedVisibility;
            }
            else
            {
                try
                {
                    // Initialize Module
                    Type type = module.ModuleInfo.Assembly.GetType(module.ModuleInfo.ClassName);

                    // create an instance of the module
                    module.ModuleInstance = (IModule)Activator.CreateInstance(type);
                }
                catch
                {
                    module.ModuleInstance = null;
                }

                if (module.ModuleInstance != null)
                {
                    if (this.CurrentPlatform.Name == "Xbox 360")
                    {
                        module.ModuleContext = new XboxModuleContext(this.CurrentTitle, module.TCRTestCaseViewItem, this);
                        module.ModuleContext.AllDevices = this.PoolDevices;
                        module.ModuleInstance.Start(module.ModuleContext);
                        this.CurrentModuleUIElement = module.ModuleInstance.UIElement;
                    }
                    else
                    {
                        // TBD
                    }
                }

                this.ModulePickedVisibility = Visibility.Visible;

                // Since Module is selected, hide Module Selector panel
                this.ModuleSelectorVisibility = Visibility.Collapsed;

                // Select related version+platform+category+TCR+Test, and make version+platform+category+TCR+Test selections visible, in case not already.
                if (this.currentPlatform != module.TCRTestCaseViewItem.TCRViewItem.TCRCategoryViewItem.TCRVersionViewItem.TCRPlatformViewItem)
                {
                    this.currentPlatform = module.TCRTestCaseViewItem.TCRViewItem.TCRCategoryViewItem.TCRVersionViewItem.TCRPlatformViewItem;
                    this.NotifyPropertyChanged("CurrentPlatform");
                    this.PlatformPickedVisibility = Visibility.Visible;
                    this.PopulateDevicePool(this.currentPlatform);
                }

                this.currentPlatform.IsSelected = false;

                this.currentTCRVersion = module.TCRTestCaseViewItem.TCRViewItem.TCRCategoryViewItem.TCRVersionViewItem;
                this.NotifyPropertyChanged("CurrentTCRVersion");
                this.TCRVersionPickedVisibility = Visibility.Visible;
                this.TCRVersionNotPickedVisibility = Visibility.Collapsed;
                this.currentTCRVersion.IsSelected = false;

                this.currentTCRCategory = module.TCRTestCaseViewItem.TCRViewItem.TCRCategoryViewItem;
                this.NotifyPropertyChanged("CurrentTCRCategory");
                this.TCRCategoryPickedVisibility = Visibility.Visible;
                this.currentTCRCategory.IsSelected = false;
                this.currentTCRCategory.IsExpanded = true;

                this.currentTCR = module.TCRTestCaseViewItem.TCRViewItem;
                this.NotifyPropertyChanged("CurrentTCR");
                this.TCRPickedVisibility = Visibility.Visible;
                this.currentTCR.IsSelected = true;

                this.currentTest = module.TCRTestCaseViewItem;
                this.NotifyPropertyChanged("CurrentTest");
                this.TestPickedVisibility = Visibility.Visible;
                this.currentTest.IsSelected = false;

                this.currentModule.IsSelected = true;
            }

            this.NotifyPropertyChanged("CurrentModule");
        }
    }
}