// -----------------------------------------------------------------------
// <copyright file="UI028CTC1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace UI028
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using System.Xml;
    using CAT;
    using Microsoft.Test.Xbox.Profiles;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using XDevkit;

    /// <summary>
    /// UI 28 Module
    /// </summary>
    public class UI028CTC1 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// Indicates whether the controller is bound
        /// </summary>
        private bool bound;

        /// <summary>
        /// List of IXboxAutomation objects for all consoles in use
        /// </summary>
        private List<IXboxAutomation> automation;

        /// <summary>
        /// Current indicated XBox game controller quadrant
        /// </summary>
        private uint quadrant = 0;

        /// <summary>
        /// Current user index
        /// </summary>
        private UserIndex userIndex = UserIndex.Zero;

        /// <summary>
        /// Specified player index
        /// </summary>
        private PlayerIndex playerIndex = PlayerIndex.One;

        /// <summary>
        /// XBox automation gamepad object used with
        /// <![CDATA[ List<IXboxAutomation> automation ]]> member object
        /// </summary>
        private XBOX_AUTOMATION_GAMEPAD gamepad;

        /// <summary>
        /// A terminology area that contains all terms
        /// </summary>
        private TerminologyArea allTerminologyArea;

        /// <summary>
        /// Backing field for CurrentTerm property
        /// </summary>
        private Term currentTerm;

        /// <summary>
        /// XBox automation gamepad object used with
        /// <![CDATA[ List<IXboxAutomation> automation ]]> member object
        /// </summary>
        private XBOX_AUTOMATION_GAMEPAD neutralgamepad = new XBOX_AUTOMATION_GAMEPAD();

        /// <summary>
        /// Backing field for CurrentTerminologyArea property
        /// </summary>
        private TerminologyArea currentTerminologyArea;

        /// <summary>
        /// Backing field for VirtualControllerVisibility property
        /// </summary>
        private Visibility virtualControllerVisibility = Visibility.Hidden;

        /// <summary>
        /// Backing field for CurrentTerminologyLanguage property
        /// </summary>
        private string currentTerminologyLanguage;

        /// <summary>
        /// Backing field for AdjustConsoleName
        /// </summary>
        private string adjustConsoleName;

        /// <summary>
        /// Last text terminology was filtered by
        /// </summary>
        private string lastFilteredText;

        /// <summary>
        /// Private module context instance for this current working module context
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// Private UI028CTC1UI user interface property for this user interface component
        /// </summary>
        private UI028CTC1UI moduleUI;

        /// <summary>
        /// A list of images to delete when the module exits
        /// </summary>
        private List<string> throwawayImages;

        /// <summary>
        /// A value indicating whether or not images have been saved yet
        /// </summary>
        private bool imagesHaveBeenSaved;

        /// <summary>
        /// A value incremented and used as part of the filename for each image created, to avoid file name conflicts.
        /// </summary>
        private int logInstance;

        /// <summary>
        /// Backing field for OverviewPageVisibility property
        /// </summary>
        private Visibility overviewPageVisibility;

        /// <summary>
        /// Backing field for ObserverConsoles property
        /// </summary>
        private ObservableCollection<ObserverConsole> observerConsoles;

        /// <summary>
        /// Backing field for LanguagesWithImages property
        /// </summary>
        private ObservableCollection<ObservedLanguage> languagesWithImages;

        /// <summary>
        /// Backing field for ActiveProfileName
        /// </summary>
        private string activeProfileName;

        /// <summary>
        /// Backing field for TestInEnglish property
        /// </summary>
        private bool testInEnglish;

        /// <summary>
        /// Backing field for TestInFrench property
        /// </summary>
        private bool testInFrench;

        /// <summary>
        /// Backing field for TestInKorean property
        /// </summary>
        private bool testInKorean;

        /// <summary>
        /// Backing field for TestInRussian property
        /// </summary>
        private bool testInRussian;

        /// <summary>
        /// Backing field for TestInChineseSimplified property
        /// </summary>
        private bool testInChineseSimplified;

        /// <summary>
        /// Backing field for TestInPortuguese property
        /// </summary>
        private bool testInPortuguese;

        /// <summary>
        /// Backing field for TestInItalian property
        /// </summary>
        private bool testInItalian;

        /// <summary>
        /// Backing field for TestInGerman property
        /// </summary>
        private bool testInGerman;

        /// <summary>
        /// Backing field for TestInSpanish property
        /// </summary>
        private bool testInSpanish;

        /// <summary>
        /// Backing field for TestInJapanese property
        /// </summary>
        private bool testInJapanese;

        /// <summary>
        /// Backing field for TestInPolish property
        /// </summary>
        private bool testInPolish;

        /// <summary>
        /// Backing field for TestInChineseTraditional property
        /// </summary>
        private bool testInChineseTraditional;

        /// <summary>
        /// Backing field for TestInDutch property
        /// </summary>
        private bool testInDutch;

        /// <summary>
        /// Backing field for TestInNorwegian property
        /// </summary>
        private bool testInNorwegian;

        /// <summary>
        /// Backing field for TestInSwedish property
        /// </summary>
        private bool testInSwedish;

        /// <summary>
        /// Backing field for TestInTurkish property
        /// </summary>
        private bool testInTurkish;

        /// <summary>
        /// Backing field for DrivingConsole property
        /// </summary>
        private IXboxDevice drivingConsole;

        /// <summary>
        /// Backing field for SetupString property
        /// </summary>
        private string setupString;

        /// <summary>
        /// Backing field for IsSetupDone property
        /// </summary>
        private bool isSetupDone;

        /// <summary>
        /// Backing field for SetupProgress
        /// </summary>
        private int setupProgress;

        /// <summary>
        /// Backing field for AvailableConsoles property
        /// </summary>
        private ObservableCollection<AvailableConsole> availableConsoles;

        /// <summary>
        /// Backing field for Page2Visibility property
        /// </summary>
        private Visibility page2Visibility;

        /// <summary>
        /// Backing field for Page3Visibility property
        /// </summary>
        private Visibility page3Visibility;

        /// <summary>
        /// Backing field for Page4Visibility property
        /// </summary>
        private Visibility page4Visibility;

        /// <summary>
        /// Backing field for Page5Visibility property
        /// </summary>
        private Visibility page5Visibility;

        /// <summary>
        /// Value indicating which page is currently visible
        /// </summary>
        private int page = 1;

        /// <summary>
        /// Backing field for SupportedLanguages property
        /// </summary>
        private ObservableCollection<string> supportedLanguages;

        /// <summary>
        /// Backing field for SelectedLanguage property
        /// </summary>
        private string selectedLanguage = "en-US";

        /// <summary>
        /// Backing field for IsControllerConnected property
        /// </summary>
        private bool isControllerConnected = true;

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a command to trigger X on the Xbox controller
        /// </summary>
        public Command CommandX { get; set; }
        
        /// <summary>
        /// Gets or sets a command to trigger Y on the Xbox controller
        /// </summary>
        public Command CommandY { get; set; }

        /// <summary>
        /// Gets or sets a command to trigger A on the Xbox controller
        /// </summary>
        public Command CommandA { get; set; }

        /// <summary>
        /// Gets or sets a command to trigger B on the Xbox controller
        /// </summary>
        public Command CommandB { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the virtual controller should be visible 
        /// </summary>
        public Visibility VirtualControllerVisibility
        {
            get
            {
                return this.virtualControllerVisibility;
            }

            set
            {
                this.virtualControllerVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a list of all terminology areas
        /// </summary>
        public List<TerminologyArea> TerminologyAreas { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the current term
        /// </summary>
        public Term CurrentTerm
        {
            get
            {
                return this.currentTerm;
            }

            set
            {
                this.currentTerm = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the current terminology area
        /// </summary>
        public TerminologyArea CurrentTerminologyArea
        {
            get
            {
                return this.currentTerminologyArea;
            }

            set
            {
                if (this.currentTerminologyArea != value)
                {
                    if (value != this.allTerminologyArea)
                    {
                        if (this.CurrentTerm != null)
                        {
                            if (this.CurrentTerm.TerminologyArea != value)
                            {
                                this.CurrentTerm.IsSelected = false;
                                this.CurrentTerm = null;
                            }
                        }
                    }

                    this.currentTerminologyArea = value;
                }

                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a hash set or terminology languages
        /// </summary>
        public HashSet<string> TerminologyLanguages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the current terminology language
        /// </summary>
        public string CurrentTerminologyLanguage
        {
            get
            {
                return this.currentTerminologyLanguage;
            }

            set
            {
                if (this.currentTerminologyLanguage != value)
                {
                    this.currentTerminologyLanguage = value;
                    this.CurrentTerm = null;
                    this.ApplyTerminologyFilter(this.lastFilteredText);
                }

                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we are processing keyboard input to control the consoles
        /// </summary>
        public bool KeyboardIsCaptured { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether All New Profiles is checked
        /// </summary>
        public bool AllNewProfilesChecked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Change Languages is checked
        /// </summary>
        public bool ChangeLanguagesChecked { get; set; }

        /// <summary>
        /// Gets or sets a value to filter terminology by
        /// </summary>
        public string TerminologyFilterText { get; set; }

        /// <summary>
        /// Gets or sets dispatcherTimer for polling controller input
        /// </summary>
        public DispatcherTimer DetectControllerStateTimer { get; set; }

        /// <summary>
        /// Gets or sets the console currently being adjusted
        /// </summary>
        public string AdjustConsoleName
        {
            get
            {
                return this.adjustConsoleName;
            }

            set
            {
                this.adjustConsoleName = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the controller is connected
        /// </summary>
        public bool IsControllerConnected
        {
            get
            {
                return this.isControllerConnected;
            }

            set
            {
                if (this.isControllerConnected != value)
                {
                    this.isControllerConnected = value;
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the UI element associated with this module
        /// </summary>
        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
        }

        /// <summary>
        /// Gets or sets the list of observer consoles
        /// </summary>
        public ObservableCollection<ObserverConsole> ObserverConsoles
        {
            get
            {
                return this.observerConsoles;
            }

            set
            {
                this.observerConsoles = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a list of languages with images
        /// </summary>
        public ObservableCollection<ObservedLanguage> LanguagesWithImages
        {
            get
            {
                return this.languagesWithImages;
            }

            set
            {
                this.languagesWithImages = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a count of selected consoles
        /// </summary>
        public int ConsoleCount
        {
            get
            {
                if (this.AvailableConsoles == null)
                {
                    return 0;
                }

                int i = 0;
                foreach (AvailableConsole console in this.AvailableConsoles)
                {
                    if (console.IsSelected)
                    {
                        i++;
                    }
                }

                return i;
            }
        }

        /// <summary>
        /// Gets or sets the active profile name
        /// </summary>
        public string ActiveProfileName
        {
            get
            {
                return this.activeProfileName;
            }

            set
            {
                this.activeProfileName = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not English is being tested
        /// </summary>
        public bool TestInEnglish
        {
            get
            {
                return this.testInEnglish;
            }

            set
            {
                this.testInEnglish = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not French is being tested
        /// </summary>
        public bool TestInFrench
        {
            get
            {
                return this.testInFrench;
            }

            set
            {
                this.testInFrench = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Korean is being tested
        /// </summary>
        public bool TestInKorean
        {
            get
            {
                return this.testInKorean;
            }

            set
            {
                this.testInKorean = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Russian is being tested
        /// </summary>
        public bool TestInRussian
        {
            get
            {
                return this.testInRussian;
            }

            set
            {
                this.testInRussian = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Simplified Chinese is being tested
        /// </summary>
        public bool TestInChineseSimplified
        {
            get
            {
                return this.testInChineseSimplified;
            }

            set
            {
                this.testInChineseSimplified = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Portuguese is being tested
        /// </summary>
        public bool TestInPortuguese
        {
            get
            {
                return this.testInPortuguese;
            }

            set
            {
                this.testInPortuguese = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Italian is being tested
        /// </summary>
        public bool TestInItalian
        {
            get
            {
                return this.testInItalian;
            }

            set
            {
                this.testInItalian = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not German is being tested
        /// </summary>
        public bool TestInGerman
        {
            get
            {
                return this.testInGerman;
            }

            set
            {
                this.testInGerman = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Spanish is being tested
        /// </summary>
        public bool TestInSpanish
        {
            get
            {
                return this.testInSpanish;
            }

            set
            {
                this.testInSpanish = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Japanese is being tested
        /// </summary>
        public bool TestInJapanese
        {
            get
            {
                return this.testInJapanese;
            }

            set
            {
                this.testInJapanese = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Polish is being tested
        /// </summary>
        public bool TestInPolish
        {
            get
            {
                return this.testInPolish;
            }

            set
            {
                this.testInPolish = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Traditional Chinese is being tested
        /// </summary>
        public bool TestInChineseTraditional
        {
            get
            {
                return this.testInChineseTraditional;
            }

            set
            {
                this.testInChineseTraditional = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Dutch is being tested
        /// </summary>
        public bool TestInDutch
        {
            get
            {
                return this.testInDutch;
            }

            set
            {
                this.testInDutch = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Norwegian is being tested
        /// </summary>
        public bool TestInNorwegian
        {
            get
            {
                return this.testInNorwegian;
            }

            set
            {
                this.testInNorwegian = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Swedish is being tested
        /// </summary>
        public bool TestInSwedish
        {
            get
            {
                return this.testInSwedish;
            }

            set
            {
                this.testInSwedish = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not Turkish is being tested
        /// </summary>
        public bool TestInTurkish
        {
            get
            {
                return this.testInTurkish;
            }

            set
            {
                this.testInTurkish = value;
                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("LanguageCount");
            }
        }

        /// <summary>
        /// Gets the number of languages selected to be tested
        /// </summary>
        public int LanguageCount
        {
            get
            {
                int i = 0;
                if (this.TestInChineseSimplified)
                {
                    i++;
                }

                if (this.TestInChineseTraditional)
                {
                    i++;
                }

                if (this.TestInEnglish)
                {
                    i++;
                }

                if (this.TestInFrench)
                {
                    i++;
                }

                if (this.TestInGerman)
                {
                    i++;
                }

                if (this.TestInKorean)
                {
                    i++;
                }

                if (this.TestInItalian)
                {
                    i++;
                }

                if (this.TestInJapanese)
                {
                    i++;
                }

                if (this.TestInDutch)
                {
                    i++;
                }

                if (this.TestInNorwegian)
                {
                    i++;
                }

                if (this.TestInPolish)
                {
                    i++;
                }

                if (this.TestInPortuguese)
                {
                    i++;
                }

                if (this.TestInRussian)
                {
                    i++;
                }

                if (this.TestInSpanish)
                {
                    i++;
                }

                if (this.TestInSwedish)
                {
                    i++;
                }

                if (this.TestInTurkish)
                {
                    i++;
                }

                return i;
            }
        }

        /// <summary>
        /// Gets or sets the driving console
        /// </summary>
        public IXboxDevice DrivingConsole
        {
            get
            {
                return this.drivingConsole;
            }

            set
            {
                this.drivingConsole = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the title name
        /// </summary>
        public string TitleName
        {
            get
            {
                if (this.moduleContext.XboxTitle != null)
                {
                    return this.moduleContext.XboxTitle.Name;
                }

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the setup string
        /// </summary>
        public string SetupString
        {
            get
            {
                return this.setupString;
            }

            set
            {
                this.setupString = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not setup is done
        /// </summary>
        public bool IsSetupDone
        {
            get
            {
                return this.isSetupDone;
            }

            set
            {
                this.isSetupDone = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating setup progress
        /// </summary>
        public int SetupProgress
        {
            get
            {
                return this.setupProgress;
            }

            set
            {
                this.setupProgress = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a list of available consoles
        /// </summary>
        public ObservableCollection<AvailableConsole> AvailableConsoles
        {
            get
            {
                return this.availableConsoles;
            }

            set
            {
                this.availableConsoles = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if Overview page is visible
        /// </summary>
        public Visibility OverviewPageVisibility
        {
            get
            {
                return this.overviewPageVisibility;
            }

            set
            {
                this.overviewPageVisibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if Page 2 is visible
        /// </summary>
        public Visibility Page2Visibility
        {
            get
            {
                return this.page2Visibility;
            }

            set
            {
                this.page2Visibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if Page 3 is visible
        /// </summary>
        public Visibility Page3Visibility
        {
            get
            {
                return this.page3Visibility;
            }

            set
            {
                this.page3Visibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if Page 4 is visible
        /// </summary>
        public Visibility Page4Visibility
        {
            get
            {
                return this.page4Visibility;
            }

            set
            {
                this.page4Visibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if Page 5 is visible
        /// </summary>
        public Visibility Page5Visibility
        {
            get
            {
                return this.page5Visibility;
            }

            set
            {
                this.page5Visibility = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a list of supported languages
        /// </summary>
        public ObservableCollection<string> SupportedLanguages
        {
            get
            {
                return this.supportedLanguages;
            }

            set
            {
                this.supportedLanguages = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected language
        /// </summary>
        public string SelectedLanguage
        {
            get
            {
                return this.selectedLanguage;
            }

            set
            {
                this.selectedLanguage = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the active profile
        /// </summary>
        private ConsoleProfile ActiveProfile { get; set; }

        /// <summary>
        /// Apply a filter to the terminology list
        /// </summary>
        /// <param name="filterText">Text to filter by</param>
        public void ApplyTerminologyFilter(string filterText)
        {
            this.lastFilteredText = filterText;
            if (string.IsNullOrEmpty(filterText))
            {
                foreach (Term term in this.allTerminologyArea.Terms)
                {
                    term.MatchesFilter = true;
                }
            }
            else
            {
                foreach (Term term in this.allTerminologyArea.Terms)
                {
                    string language = this.currentTerminologyLanguage;
                    term.MatchesFilter = term.Terms[language].ToLower().Contains(filterText.ToLower());
                }
            }
        }

        /// <summary>
        /// Binds the controllers in quadrant 1 on all Xboxes
        /// </summary>
        public void BindAll()
        {
            if (this.automation == null)
            {
                this.automation = new List<IXboxAutomation>();

                foreach (ObserverConsole console in this.ObserverConsoles)
                {
                    console.Automation = console.Device.XboxConsole.XboxAutomation;
                    this.automation.Add(console.Device.XboxConsole.XboxAutomation);
                }
            }
            
            foreach (IXboxAutomation auto in this.automation)
            {
                auto.BindController(this.quadrant, 10);
                auto.ClearGamepadQueue(this.quadrant);
                auto.ConnectController(this.quadrant);
            }

            this.AdjustConsoleName = "All";
            this.bound = true;
        }

        /// <summary>
        /// Unbinds the controllers in quadrant 1 on all used Xbox's
        /// </summary>
        public void UnbindAll()
        {
            if (this.automation == null)
            {
                return;
            }

            foreach (IXboxAutomation auto in this.automation)
            {
                auto.UnbindController(this.quadrant);
            }

            this.AdjustConsoleName = "None";
            this.bound = false;
        }

        /// <summary>
        /// Send the desired controller signal to every console
        /// </summary>
        /// <param name="buttons">Buttons to press</param>
        public void BroadcastControllerButtons(XboxAutomationButtonFlags buttons)
        {
            this.gamepad.LeftThumbX = 0;
            this.gamepad.LeftThumbY = 0;
            this.gamepad.LeftTrigger = 0;
            this.gamepad.RightThumbX = 0;
            this.gamepad.RightThumbY = 0;
            this.gamepad.RightTrigger = 0;
            this.gamepad.Buttons = buttons;

            foreach (IXboxAutomation auto in this.automation)
            {
                try
                {
                    // press button
                    auto.QueueGamepadState(this.quadrant, this.gamepad, 200, 0);

                    // unpress button
                    auto.QueueGamepadState(this.quadrant, this.neutralgamepad, 100, 0);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Broadcasts left trigger
        /// </summary>
        public void BroadcastControllerLeftTrigger()
        {
            this.gamepad.LeftTrigger = byte.MaxValue;

            this.gamepad.Buttons = 0;
            this.gamepad.LeftThumbX = 0;
            this.gamepad.LeftThumbY = 0;
            this.gamepad.RightThumbX = 0;
            this.gamepad.RightThumbY = 0;
            this.gamepad.RightTrigger = 0;

            foreach (IXboxAutomation auto in this.automation)
            {
                try
                {
                    auto.QueueGamepadState(this.quadrant, this.gamepad, 200, 0);
                    auto.QueueGamepadState(this.quadrant, this.neutralgamepad, 100, 0);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Broadcasts right trigger
        /// </summary>
        public void BroadcastControllerRightTrigger()
        {
            this.gamepad.RightTrigger = byte.MaxValue;

            this.gamepad.Buttons = 0;
            this.gamepad.LeftThumbX = 0;
            this.gamepad.LeftThumbY = 0;
            this.gamepad.RightThumbX = 0;
            this.gamepad.RightThumbY = 0;

            foreach (IXboxAutomation auto in this.automation)
            {
                try
                {
                    auto.QueueGamepadState(this.quadrant, this.gamepad, 200, 0);
                    auto.QueueGamepadState(this.quadrant, this.neutralgamepad, 100, 0);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Send passed-in controller state to all bound controllers
        /// </summary>
        /// <param name="user">User number</param>
        /// <param name="pad">Game pad controller object</param>
        public void BroadcastGamePadState(uint user, XBOX_AUTOMATION_GAMEPAD pad)
        {
            foreach (IXboxAutomation auto in this.automation)
            {
                try
                {
                    auto.SetGamepadState(user, pad);
                }
                catch (Exception ex)
                {
                    if ((uint)ex.HResult != 0x82DA002A) // XBDM_INVALIDSTATE
                    {
                        this.moduleContext.Log("Exception calling SetGamepadState: " + ex.Message);
                    }
                }
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
            this.OverviewPageVisibility = Visibility.Visible;
            this.Page2Visibility = Visibility.Collapsed;
            this.Page3Visibility = Visibility.Collapsed;
            this.Page4Visibility = Visibility.Collapsed;
            this.Page5Visibility = Visibility.Collapsed;
            this.CommandA = new Command((o) => this.BroadcastControllerButtons(XDevkit.XboxAutomationButtonFlags.A_Button));
            this.CommandB = new Command((o) => this.BroadcastControllerButtons(XDevkit.XboxAutomationButtonFlags.B_Button));
            this.CommandX = new Command((o) => this.BroadcastControllerButtons(XDevkit.XboxAutomationButtonFlags.X_Button));
            this.CommandY = new Command((o) => this.BroadcastControllerButtons(XDevkit.XboxAutomationButtonFlags.Y_Button));
            this.page = 1;
            this.LoadTerminology();
            this.AllNewProfilesChecked = true;
            this.ChangeLanguagesChecked = true;

            // capture controller input from controller attached to PC
            this.DetectControllerStateTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
            this.DetectControllerStateTimer.Tick += new EventHandler(this.PollControllerConnected);
            this.DetectControllerStateTimer.Interval = new TimeSpan(0, 0, 0, 0, 150); // last digit is milliseconds
            this.DetectControllerStateTimer.Start();
            this.moduleUI = new UI028CTC1UI(this);
        }

        /// <summary>
        /// NextPage - Called to leave the the module overview or intro screen entered by Start()
        /// The framework goes modal in this call and the module gains control.
        /// This function is called repeatedly to advance to multiple screens in the module.
        /// </summary>
        public void NextPage()
        {
            switch (this.page)
            {
                case 1: // on overview. check a controlling xbox is selected
                    this.DrivingConsole = this.GetDrivingXbox();
                    if (this.DrivingConsole == null)
                    {
                        break;
                    }
                    else if (string.IsNullOrEmpty(this.TitleName))
                    {
                        // check a game is selected
                        MessageBox.Show("Use Settings to select a Title to be tested");
                        break;
                    }
                    else if (!IsControllerConnected)
                    {
                        if (MessageBoxResult.No == MessageBox.Show("No controller is attached to the PC. Are you sure you want to continue?", "Certification Assistance Tool", MessageBoxButton.YesNo))
                        {
                            break;
                        }
                    }

                    // ==> move to page 2 setup
                    this.moduleContext.IsModal = true;
                    this.DetectControllerStateTimer.Stop();
                    this.OverviewPageVisibility = Visibility.Collapsed;
                    this.Page2Visibility = Visibility.Visible;
                    this.page = 2;
                    this.InitializeAvailableObserverList();
                    this.ObserverConsoles = new ObservableCollection<ObserverConsole>();
                    this.LanguagesWithImages = new ObservableCollection<ObservedLanguage>();
                    this.moduleContext.Log("Controlling Console: " + this.DrivingConsole.Name + " = " + this.DrivingConsole.IP);
                    break;
                case 2: // on page 2 choose languages and consoles
                    if (this.ConsoleCount == 100)
                    {
                        MessageBox.Show("At least one observer console must be selected.", "No consoles selected.");
                    }
                    else
                    {
                        bool selectionDone = true;
                        if (this.ConsoleCount < this.LanguageCount)
                        {
                            MessageBox.Show("More languages are selected than consoles.  Please deselect languages.", "Too many languages");
                            selectionDone = false;
                        }
                        else if (this.ConsoleCount > this.LanguageCount)
                        {
                            MessageBox.Show("More consoles are selected than languages.  Please select more languages.", "Not enough languages");
                            selectionDone = false;
                        }

                        if (selectionDone)
                        {
                            // ==> move to page 3 wait
                            this.Page2Visibility = Visibility.Collapsed;
                            this.Page3Visibility = Visibility.Visible;
                            this.page = 3;
                            this.IsSetupDone = false;
                            this.UpdateUIImmediately();
                            System.Windows.Input.Mouse.OverrideCursor = Cursors.Wait;

                            this.SetupConsoles();

                            // add driving console to list, so we capture its screen and treat it just like the other consoles
                            if (DrivingConsole != null)
                            {
                                Command refreshCommand = new Command((o) => this.Refresh(o as ObservedLanguage));
                                Command bindOneCommand = new Command((o) => this.BindOne(o as ObservedLanguage));

                                ObservedLanguage main = new ObservedLanguage(this.GetLanguageName(Language.English), this.GetLanguageID(Language.English), Language.English, refreshCommand, bindOneCommand);
                                main.Observer = new ObserverConsole(DrivingConsole, ActiveProfile);
                                main.Observer.Installed = "Installed";
                                main.Observer.SignedIn = "Signed-In";
                                main.Observer.ObservedLanguage = main;
                                observerConsoles.Add(main.Observer);
                            }

                            // bind controllers
                            this.BindAll();

                            // get initial set of screen images
                            this.CaptureScreens();

                            // capture controller input from controller attached to PC
                            this.DetectControllerStateTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
                            this.DetectControllerStateTimer.Tick += new EventHandler(this.PollController);
                            this.DetectControllerStateTimer.Interval = new TimeSpan(0, 0, 0, 0, 15); // last digit is milliseconds
                            this.DetectControllerStateTimer.Start();
                            
                            this.IsSetupDone = true;
                            System.Windows.Input.Mouse.OverrideCursor = null;
                        }
                    }

                    break;
                case 3: // on page 3 - setup
                    // ==> move to page 4 - controller bind changes
                    this.Page3Visibility = Visibility.Collapsed;
                    this.Page4Visibility = Visibility.Visible;
                    this.page = 4;
                    break;
                case 4: // on page 4 - controller bind changes
                        // ==> move to page 5 test
                    this.Page4Visibility = Visibility.Collapsed;
                    this.Page5Visibility = Visibility.Visible;
                    this.page = 5;

                    // bind controllers and show virtual controller
                    if (!this.bound)
                    {
                        this.BindAll();
                    }

                    this.VirtualControllerVisibility = Visibility.Visible;
                    break;
                case 5: // on page 5 - going through terminology and screens. button now says 'back'
                        // ==> move back to page 4
                    this.Page4Visibility = Visibility.Visible;
                    this.Page5Visibility = Visibility.Collapsed;
                    this.page = 4;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// BackPage
        /// This function is called repeatedly to backup to previous screens in the module.
        /// </summary>
        public void BackPage()
        {
            switch (this.page)
            {
                case 4: // on page 4 - bind controllers
                    // ==> move to page 3 - setup
                    this.Page3Visibility = Visibility.Visible;
                    this.Page4Visibility = Visibility.Collapsed;
                    this.page = 3;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Called when console count has changed
        /// </summary>
        public void UpdateCounts()
        {
            this.NotifyPropertyChanged("ConsoleCount");
            this.NotifyPropertyChanged("PopulateSetupTable");
        }

        /// <summary>
        /// Helper function to call IXboxAutomation, and WAKE UP all Xbox Development kits by moving the right thumb arrow to the right.
        /// </summary>
        public void WakeUpXboxDevkits()
        {
            this.ChangeSetupProgress("Waking up Xbox Devkits...", 1);
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                observer.Device.WakeUpXboxDevkit();
            }
        }

        /// <summary>
        /// Get screens for preview
        /// </summary>
        public void CaptureScreens()
        {
            string path = Path.Combine(this.moduleContext.LogDirectory, "images");

            System.Windows.Input.Mouse.OverrideCursor = Cursors.Wait;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // keep the last set of images if we saved them. othewise overwrite them with the same index
            foreach (ObserverConsole observerConsole in this.ObserverConsoles)
            {
                if (!string.IsNullOrEmpty(observerConsole.ObservedLanguage.ImageWholePath))
                {
                    if (!this.imagesHaveBeenSaved)
                    {
                        this.throwawayImages.Add(observerConsole.ObservedLanguage.ImageWholePath);
                    }

                    observerConsole.ObservedLanguage.ImageWholePath = string.Empty;
                    observerConsole.ObservedLanguage.ImageContents = null;
                }

                this.LanguagesWithImages.Remove(observerConsole.ObservedLanguage);
            }

            this.logInstance++;

            List<Thread> captureThreads = new List<Thread>();
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                Thread th = new Thread(new ParameterizedThreadStart(delegate
                {
                    observer.ObservedLanguage.ImageName = observer.ObservedLanguage.LanguageName + observer.Device.IP + "image" + this.logInstance.ToString() + ".jpg";
                    observer.ObservedLanguage.ImageWholePath = Path.Combine(path, observer.ObservedLanguage.ImageName);
                    File.Delete(observer.ObservedLanguage.ImageWholePath);
                    observer.Device.ScreenShot(observer.ObservedLanguage.ImageWholePath);
                }));

                th.Name = "Capture";
                th.Start();
                captureThreads.Add(th);
            }

            foreach (Thread th in captureThreads)
            {
                th.Join();
            }

            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                observer.ObservedLanguage.ImageContents = LoadImage(observer.ObservedLanguage.ImageWholePath);
            }

            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                this.LanguagesWithImages.Add(observer.ObservedLanguage);
            }

            this.imagesHaveBeenSaved = false;
            this.UpdateUIImmediately();

            System.Windows.Input.Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Stop the module
        /// </summary>
        public void Stop()
        {
            System.Windows.Input.Mouse.OverrideCursor = Cursors.Wait;

            if (this.DetectControllerStateTimer != null)
            {
                this.DetectControllerStateTimer.Stop();
            }

            if (this.automation != null)
            {
                this.UnbindAll();
                this.automation.Clear();
            }

            // stop title if it is in emulation
            try
            {
                if (this.DrivingConsole.IsDiscEmulationRunning)
                {
                    this.DrivingConsole.StopEmulatingDiscTitle();
                }
            }
            catch
            {
            }

            // reset observer boxes to english. remove created profiles. reset video resolution.
            if (this.ChangeLanguagesChecked)
            {
                try
                {
                    foreach (ObserverConsole item in this.ObserverConsoles)
                    {
                        if (this.AllNewProfilesChecked)
                        {
                            item.Device.LaunchDevDashboard();
                            ConsoleProfilesManager profilesManager = item.Device.XboxConsole.CreateConsoleProfilesManager();
                            profilesManager.SignOutAllUsers();
                            profilesManager.DeleteConsoleProfile(item.Profile);
                        }

                        item.Device.SetLanguage(Language.English, false);
                        item.Device.SetVideoMode(VideoResolution.Mode1080p, VideoStandard.NTSCM, false);
                        item.Device.Reboot(false);
                        item.Device.DeferDisconnect();
                        Thread th = new Thread(new ParameterizedThreadStart(delegate
                        {
                            item.Device.WaitForRebootToComplete();
                            item.Device.AllowDisconnect();
                        }));
                        th.Name = "UI028.Stop";
                        th.Start();
                    }
                }
                catch
                {
                }
            }

            // remove throwaway images
            if (this.throwawayImages != null)
            {
                foreach (string throwawayImage in this.throwawayImages)
                {
                    try
                    {
                        File.Delete(throwawayImage);
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            // LogRichPresenceLocalization();
            string passedOrFailed = "DONE";
            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("RESULT: " + passedOrFailed + "\r\n");
            this.moduleContext.Log("*************************************************************\r\n");

            System.Windows.Input.Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Retries navigation and refreshes the specified image
        /// </summary>
        /// <param name="observedLanguage">The language associated with the image to retry</param>
        public void BindOne(ObservedLanguage observedLanguage)
        {
            System.Windows.Input.Mouse.OverrideCursor = Cursors.Wait;

            this.UnbindAll();

            // Bind the observer.Device
            observedLanguage.Observer.Automation.BindController(this.quadrant, 10);
            observedLanguage.Observer.Automation.ClearGamepadQueue(this.quadrant);
            observedLanguage.Observer.Automation.ConnectController(this.quadrant);

            this.AdjustConsoleName = observedLanguage.Observer.ConsoleName;

            System.Windows.Input.Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Save and log snapshots
        /// </summary>
        public void SaveSnapshots()
        {
            this.imagesHaveBeenSaved = true;
            foreach (ObserverConsole console in this.ObserverConsoles)
            {
                this.throwawayImages.Remove(console.ImageWholePath);
            }
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
        /// Load terminology
        /// </summary>
        private void LoadTerminology()
        {
            this.TerminologyAreas = new List<TerminologyArea>();
            this.allTerminologyArea = new TerminologyArea("All");
            this.TerminologyAreas.Add(this.allTerminologyArea);
            this.CurrentTerminologyArea = this.allTerminologyArea;
            this.CurrentTerminologyLanguage = "English";
            this.TerminologyLanguages = new HashSet<string>();
            this.TerminologyLanguages.Add("English");

            XmlDocument configFile = new XmlDocument();
            string filePath = Path.Combine(this.moduleContext.PlatformDataPath, "Terminology.xml");
            configFile.Load(filePath);
            if (configFile.DocumentElement.Name == "Workbook")  
            {
                XmlNode workbookNode = configFile.DocumentElement;
                if (workbookNode.ChildNodes.Count > 0)
                {
                    foreach (XmlNode n in workbookNode.ChildNodes)
                    {
                        // Per worksheet
                        if (n.Name == "Worksheet")  
                        {
                            string areaName = n.Attributes["ss:Name"].Value;
                            TerminologyArea area = new TerminologyArea(areaName);
                            this.TerminologyAreas.Add(area);
                            foreach (XmlNode n2 in n.ChildNodes)
                            {
                                // Per Table in this worksheet
                                if (n2.Name == "Table") 
                                {
                                    List<string> languages = new List<string>();
                                    languages.Add("English");
                                    int rowIndex = 0;
                                    int usedRowIndex = 0;
                                    bool restartTable = false;
                                    foreach (XmlNode n3 in n2.ChildNodes)
                                    {
                                        // Per Row in this worksheet
                                        if (n3.Name == "Row")   
                                        {
                                            if (restartTable)
                                            {
                                                restartTable = false;
                                                usedRowIndex = 0;
                                                languages = new List<string>();
                                                languages.Add("English");
                                            }

                                            rowIndex++;
                                            usedRowIndex++;
                                            if (usedRowIndex == 1)
                                            {
                                                // Skip header row 1
                                                continue;
                                            }

                                            Term term = new Term(area);
                                            int cellIndex = 0;
                                            foreach (XmlNode n4 in n3.ChildNodes)
                                            {
                                                // Per Cell in this Row
                                                if (n4.Name == "Cell")  
                                                {
                                                    cellIndex++;
                                                    if (usedRowIndex == 2)
                                                    {
                                                        if (cellIndex == 2)
                                                        {
                                                            // Dont add language for Description
                                                            continue;
                                                        }
                                                    }

                                                    foreach (XmlNode n5 in n4.ChildNodes)
                                                    {
                                                        // Data within cell
                                                        if (n5.Name == "Data")  
                                                        {
                                                            if (usedRowIndex == 2)
                                                            {
                                                                if (cellIndex <= 2)
                                                                {
                                                                    continue;
                                                                }

                                                                // Add language to array of languages
                                                                languages.Add(n5.InnerText);
                                                                if (!this.TerminologyLanguages.Contains(n5.InnerText))
                                                                {
                                                                    this.TerminologyLanguages.Add(n5.InnerText);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                string cellValue = n5.InnerText;
                                                                if (!string.IsNullOrEmpty(cellValue))
                                                                {
                                                                    if (cellIndex == 2)
                                                                    {
                                                                        term.Description = cellValue;
                                                                    }
                                                                    else
                                                                    {
                                                                        if (cellIndex == 1)
                                                                        {
                                                                            area.Terms.Add(term);
                                                                            this.allTerminologyArea.Terms.Add(term);
                                                                        }

                                                                        // Add term
                                                                        int languageIndex = (cellIndex == 1) ? 0 : (cellIndex - 2);
                                                                        string language = languages[languageIndex];
                                                                        term.Terms.Add(language, cellValue);

                                                                        if (!area.TermsByLanguage.ContainsKey(language))
                                                                        {
                                                                            area.TermsByLanguage.Add(language, new List<KeyValuePair<string, Term>>());
                                                                        }

                                                                        if (!this.allTerminologyArea.TermsByLanguage.ContainsKey(language))
                                                                        {
                                                                            this.allTerminologyArea.TermsByLanguage.Add(language, new List<KeyValuePair<string, Term>>());
                                                                        }

                                                                        area.TermsByLanguage[language].Add(new KeyValuePair<string, Term>(cellValue, term));
                                                                        this.allTerminologyArea.TermsByLanguage[language].Add(new KeyValuePair<string, Term>(cellValue, term));
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    restartTable = true;
                                                                }
                                                            }

                                                            break;  // only 1 data node
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

        /// <summary>
        /// PollControllerConnected - ask controller if it is connected
        /// </summary>
        /// <param name="obj">Sending object source as another control or event</param>
        /// <param name="args">Applicable event processing args, if any</param>
        private void PollControllerConnected(object obj, EventArgs args)
        {
            GamePadState state = GamePad.GetState(this.playerIndex);
            this.IsControllerConnected = state.IsConnected;
        }

        /// <summary>
        /// PollController - ask controller for its state every time the timer 'ticks'
        /// </summary>
        /// <param name="obj">Sending object source as another control or event</param>
        /// <param name="args">Applicable event processing args, if any</param>
        private void PollController(object obj, EventArgs args)
        {
            XBOX_AUTOMATION_GAMEPAD pad = new XBOX_AUTOMATION_GAMEPAD();
            GamePadState state = GamePad.GetState(this.playerIndex);

            this.IsControllerConnected = state.IsConnected;
            if (!state.IsConnected)
            {
                return;
            }

            // Convert button presses from GamePadState to XBOX_AUTOMATION_GAMEPAD
            if (state.Buttons.A == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.A_Button;
            }

            if (state.Buttons.B == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.B_Button;
            }

            if (state.Buttons.Back == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.BackButton;
            }

            if (state.Buttons.BigButton == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.Xbox360_Button;
            }

            if (state.Buttons.LeftShoulder == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.LeftShoulderButton;
            }

            if (state.Buttons.LeftStick == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.LeftThumbButton;
            }

            if (state.Buttons.RightShoulder == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.RightShoulderButton;
            }

            if (state.Buttons.RightStick == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.RightThumbButton;
            }

            if (state.Buttons.Start == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.StartButton;
            }

            if (state.Buttons.X == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.X_Button;
            }

            if (state.Buttons.Y == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.Y_Button;
            }

            // Convert dpad presses from GamePadState to XBOX_AUTOMATION_GAMEPAD
            if (state.DPad.Down == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.DPadDown;
            }

            if (state.DPad.Left == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.DPadLeft;
            }

            if (state.DPad.Right == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.DPadRight;
            }

            if (state.DPad.Up == ButtonState.Pressed)
            {
                pad.Buttons = XboxAutomationButtonFlags.DPadUp;
            }

            // convert movement from GamePadState to XBOX_AUTOMATION_GAMEPAD
            pad.LeftThumbX = (int)((double)state.ThumbSticks.Left.X * short.MaxValue);
            pad.LeftThumbY = (int)((double)state.ThumbSticks.Left.Y * short.MaxValue);

            pad.RightThumbX = (int)((double)state.ThumbSticks.Right.X * short.MaxValue);
            pad.RightThumbY = (int)((double)state.ThumbSticks.Right.Y * short.MaxValue);

            pad.LeftTrigger = (uint)((double)state.Triggers.Left * byte.MaxValue);
            pad.RightTrigger = (uint)((double)state.Triggers.Right * byte.MaxValue);

            this.BroadcastGamePadState(this.quadrant, pad);
        }

        /// <summary>
        /// Update the UI immediately without returning control to the main thread
        /// </summary>
        private void UpdateUIImmediately()
        {
            Dispatcher dispatcher = this.moduleUI.Dispatcher;
            if (dispatcher != null)
            {
                dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);
            }
        }

        /// <summary>
        /// Refreshes the specified image
        /// </summary>
        /// <param name="observedLanguage">The language associated with the image to refresh</param>
        private void Refresh(ObservedLanguage observedLanguage)
        {
            System.Windows.Input.Mouse.OverrideCursor = Cursors.Wait;
            ObserverConsole observer = observedLanguage.Observer;

            observedLanguage.ImageName = observedLanguage.LanguageName + observer.Device.IP + "image" + this.logInstance.ToString() + ".jpg";
            string path = Path.Combine(this.moduleContext.LogDirectory, "images");
            observedLanguage.ImageWholePath = Path.Combine(path, observedLanguage.ImageName);
            File.Delete(observedLanguage.ImageWholePath);
            observer.Device.ScreenShot(observedLanguage.ImageWholePath);
            observedLanguage.ImageContents = LoadImage(observedLanguage.ImageWholePath);

            System.Windows.Input.Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Populate setup table
        /// </summary>
        private void PopulateSetupTable()
        {
            ObserverConsole config = null;
            List<Language> languageList = new List<Language>();
            this.ObserverConsoles.Clear();

            Command refreshCommand = new Command((o) => this.Refresh(o as ObservedLanguage));
            Command bindOneCommand = new Command((o) => this.BindOne(o as ObservedLanguage));

            // Get currently selected consoles and show pending configuration of each one
            foreach (AvailableConsole available in this.AvailableConsoles)
            {
                if (available.IsSelected)
                {
                    config = new ObserverConsole(available.Device, null);
                    this.ObserverConsoles.Add(config);
                }
            }

            IEnumerator<ObserverConsole> currentConsole = this.ObserverConsoles.GetEnumerator();
            currentConsole.MoveNext();

            // Get currently selected languages
            if (this.TestInEnglish)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.English), this.GetLanguageID(Language.English), Language.English, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInFrench)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.French), this.GetLanguageID(Language.French), Language.French, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInKorean)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.Korean), this.GetLanguageID(Language.Korean), Language.Korean, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInRussian)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.Russian), this.GetLanguageID(Language.Russian), Language.Russian, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInChineseSimplified)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.SimplifiedChinese), this.GetLanguageID(Language.SimplifiedChinese), Language.SimplifiedChinese, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInPortuguese)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.BrazilianPortuguese), this.GetLanguageID(Language.BrazilianPortuguese), Language.BrazilianPortuguese, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInItalian)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.Italian), this.GetLanguageID(Language.Italian), Language.Italian, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInGerman)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.German), this.GetLanguageID(Language.German), Language.German, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInSpanish)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.Spanish), this.GetLanguageID(Language.Spanish), Language.Spanish, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInJapanese)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.Japanese), this.GetLanguageID(Language.Japanese), Language.Japanese, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInPolish)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.Polish), this.GetLanguageID(Language.Polish), Language.Polish, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInChineseTraditional)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.TraditionalChinese), this.GetLanguageID(Language.TraditionalChinese), Language.TraditionalChinese, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInDutch)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.Dutch), this.GetLanguageID(Language.Dutch), Language.Dutch, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInNorwegian)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.Norwegian), this.GetLanguageID(Language.Norwegian), Language.Norwegian, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInSwedish)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.Swedish), this.GetLanguageID(Language.Swedish), Language.Swedish, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }

            if (this.TestInTurkish)
            {
                currentConsole.Current.ObservedLanguage = new ObservedLanguage(this.GetLanguageName(Language.Turkish), this.GetLanguageID(Language.Turkish), Language.Turkish, refreshCommand, bindOneCommand);
                currentConsole.Current.ObservedLanguage.Observer = currentConsole.Current;
                currentConsole.MoveNext();
            }
        }

        /// <summary>
        /// find connected consoles that are not the default. make a list of them. never change it.
        /// </summary>
        private void InitializeAvailableObserverList()
        {
            if (this.AvailableConsoles != null)
            {
                return;
            }
            
            this.AvailableConsoles = new ObservableCollection<AvailableConsole>();
            foreach (IXboxDevice device in this.moduleContext.AllDevices)
            {
                if (!device.IsSelected && device.Connected)
                {
                    this.AvailableConsoles.Add(new AvailableConsole(device));
                }
            }
        }

        /// <summary>
        /// setup up to 16 consoles
        /// </summary>
        private void SetupConsoles()
        {
            // start progress bar. woo hoo
            this.SetupProgress = 0;

            // get selected languages
            this.ChangeSetupProgress("Assigning Languages", 0);
            this.PopulateSetupTable();

            this.moduleContext.Log(this.ObserverConsoles.Count.ToString() + " consoles.  Setting up:");
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                this.moduleContext.Log("\t" + observer.ConsoleIP + " - " + observer.ConsoleName);
            }

            //////////////////////////////////////////////////////////////////
            // T H R E A D I N G
            //////////////////////////////////////////////////////////////////
            // WAIT 1 : install title
            // WAIT 2 : create profiles
            // WAIT 3 : set language, cold boot, sign in
            // WAIT 4 : launch title
            // WAIT 5 : sign in

            // 1. install title 
            this.ChangeSetupProgress("Checking that " + this.moduleContext.XboxTitle.Name + " is installed", 2);
            List<Thread> installThreads = new List<Thread>();
            bool drivingConsoleNeedsInstall = !this.DrivingConsole.IsTitleInstalled;

            IXboxDevice observerNeedingInstall = null;
            if (!drivingConsoleNeedsInstall)
            {
                foreach (ObserverConsole observer in this.ObserverConsoles)
                {
                    if (observer.Device.IsTitleInstalled)
                    {
                        observerNeedingInstall = observer.Device;
                        break;
                    }
                }
            }

            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                Thread th = new Thread(new ParameterizedThreadStart(delegate
                {
                    observer.Device.LaunchDevDashboard();
                    if (observer.Device.IsTitleInstalled)
                    {
                        observer.Installed = "Installed";
                    }
                    else
                    {
                        observer.Device.InstallTitle(observer.Device.PrimaryDrive);
                        observer.Installed = "Installed";
                    }
                }));
                installThreads.Add(th);
                th.Name = "Install Title";
                th.Start();
            }

            // do driving title install on main thread
            if (drivingConsoleNeedsInstall)
            {
                this.DrivingConsole.InstallTitle(this.DrivingConsole.PrimaryDrive, this.moduleContext.OpenProgressBarWindow("Installing " + this.moduleContext.XboxTitle.Name + "..."));
                this.DrivingConsole.LaunchDevDashboard();
            }

            // WAIT for all installs
            foreach (Thread th in installThreads)
            {
                th.Join();
            }

            // 2. Create new profiles
            this.ChangeSetupProgress(this.AllNewProfilesChecked ? "Creating Profiles" : "Getting Profiles", 2);
            List<Thread> profileThreads = new List<Thread>();
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                Thread th = new Thread(new ParameterizedThreadStart(delegate
                {
                    observer.Profile = this.GetAProfile(observer.Device, this.AllNewProfilesChecked ? "new" : "first");
                    if (observer.Profile != null)
                    {
                        observer.ProfileName = observer.Profile.Gamertag;
                    }
                }));

                th.Name = "Get Profile";
                th.Start();
                profileThreads.Add(th);
            }

            // create profile on the driving console
            Thread thcreate = new Thread(new ParameterizedThreadStart(delegate
            {
                this.ActiveProfile = this.GetAProfile(this.DrivingConsole, this.AllNewProfilesChecked ? "new" : "first");
                this.ActiveProfileName = this.ActiveProfile.Gamertag;
                this.ActiveProfile.SignIn(this.userIndex);
            }));
            thcreate.Name = "Create Profiles";
            thcreate.Start();
            profileThreads.Add(thcreate);

            // WAIT for all created profiles
            foreach (Thread th in profileThreads)
            {
                th.Join();
            }

            // 3 Set language for each console and cold boot
            if (this.ChangeLanguagesChecked)
            {
                this.ChangeSetupProgress("Changing Languages and Rebooting", 3);
                this.UpdateUIImmediately();
                List<Thread> bootThreads = new List<Thread>();
                foreach (ObserverConsole observer in this.ObserverConsoles)
                {
                    Thread th = new Thread(new ParameterizedThreadStart(delegate
                    {
                        // set language
                        observer.Device.SetLanguage(observer.ObservedLanguage.Language, false);
                        observer.Device.SetVideoMode(VideoResolution.Mode720p, VideoStandard.NTSCM, false);
                        observer.Device.Reboot(true);
                    }));
                    bootThreads.Add(th);
                    th.Name = "Set Language";
                    th.Start();
                }

                if (this.DrivingConsole != null)
                {
                    Thread thboot = new Thread(new ParameterizedThreadStart(delegate
                    {
                        this.DrivingConsole.SetVideoMode(VideoResolution.Mode720p, VideoStandard.NTSCM, false);
                        this.DrivingConsole.Reboot(true);
                    }));
                    bootThreads.Add(thboot);
                    thboot.Name = "Boot Attached ";
                    thboot.Start();
                }

                // WAIT for all reboots
                foreach (Thread th in bootThreads)
                {
                    th.Join();
                }
            }
            else
            {
                this.ChangeSetupProgress(string.Empty, 3);
            }

            // 4. Launch title
            this.ChangeSetupProgress("Launching Title", 2);
            List<Thread> launchThreads = new List<Thread>();
            if (this.moduleContext.XboxTitle.GameInstallType != "Disc Emulation")
            {
                foreach (ObserverConsole observer in this.ObserverConsoles)
                {
                    Thread th = new Thread(new ParameterizedThreadStart((o) => observer.Device.LaunchTitle()));
                    th.Name = "Launch";
                    th.Start();
                    launchThreads.Add(th);
                }
            }

            Thread thlaunch = new Thread(new ParameterizedThreadStart((o) => this.DrivingConsole.LaunchTitle()));
            launchThreads.Add(thlaunch);
            thlaunch.Name = "Launch Title to Attached ";
            thlaunch.Start();

            // WAIT forlaunch
            foreach (Thread th in launchThreads)
            {
                th.Join();
            }

            // 5. sign in
            this.ChangeSetupProgress("Signing in", 2);
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                try
                {
                    // sign in a profile
                    Thread.Sleep(2000);
                    observer.Profile.SignIn(this.userIndex);
                    Thread.Sleep(500);
                    if (observer.Profile.GetUserSigninState() != SignInState.NotSignedIn)
                    {
                        observer.SignedIn = "Signed-In";
                    }
                }
                catch (Exception e)
                {
                    this.moduleContext.Log("Unable to use profile " + observer.ProfileName + " due to exception:\n\n" + e.Message);
                }
            }

            this.ChangeSetupProgress("Verifying Setup", 1);

            // log setup status
            this.moduleContext.Log("SETUP\tConsole Name\t Console IP\tLanguage\tSigned In\tInstalled");
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                this.moduleContext.Log("Observer\t" + observer.ConsoleName + "\t" + observer.ConsoleIP + "\t" + observer.ObservedLanguage.LanguageName + "\t" + observer.SignedIn + "\t");
            }

            // allocate space for one image per observer
            this.throwawayImages = new List<string>();

            // if there were any failures, retry
            if (!this.IsSetupGood())
            {
                this.RetrySetup();
            }

            if (this.IsSetupGood())
            {
                this.ChangeSetupProgress("Done", 1);
            }
            else
            {
                this.ChangeSetupProgress("Setup is not complete.", -1);
            }

            // enable next page if this automation is done, regardless of setup results
            this.IsSetupDone = true;
        }

        /// <summary>
        /// Update the setup progress
        /// </summary>
        /// <param name="message">Message to display in UI</param>
        /// <param name="progress">A value indicating the progress</param>
        private void ChangeSetupProgress(string message, int progress)
        {
            this.SetupString = message;
            this.SetupProgress += progress;
            this.UpdateUIImmediately();
        }

        /// <summary>
        /// Retry setup
        /// </summary>
        private void RetrySetup()
        {
            try
            {
                foreach (ObserverConsole observer in this.ObserverConsoles)
                {
                    if (observer.SignedIn == "*")
                    {
                        observer.Profile.SignIn(this.userIndex);
                    }
                }

                if (this.ActiveProfile.GetUserSigninState() == SignInState.NotSignedIn)
                {
                    this.ActiveProfile.SignIn(this.userIndex);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Checks if setup is good
        /// </summary>
        /// <returns>true if successful, false on failure</returns>
        private bool IsSetupGood()
        {
            bool result = true;

            // check sign-in state
            try
            {
                foreach (ObserverConsole observer in this.ObserverConsoles)
                {
                    if (observer.Profile.GetUserSigninState() != SignInState.NotSignedIn)
                    {
                        observer.SignedIn = "Signed-In";
                    }
                    else
                    {
                        result = false;
                    }

                    if (this.ActiveProfile.GetUserSigninState() == SignInState.NotSignedIn)
                    {
                        result = false;
                    }
                }
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// Convert a Language enumeration value to a string
        /// </summary>
        /// <param name="lang">The language to convert to a string</param>
        /// <returns>A string representation of the specified language</returns>
        private string GetLanguageName(Language lang)
        {
            string returnLanguage;
            switch (lang)
            {
                case Language.BrazilianPortuguese: 
                    returnLanguage = "Portuguese"; 
                    break;
                case Language.Dutch: 
                    returnLanguage = "Dutch"; 
                    break;
                case Language.English: 
                    returnLanguage = "English"; 
                    break;
                case Language.French: 
                    returnLanguage = "French"; 
                    break;
                case Language.German: 
                    returnLanguage = "German"; 
                    break;
                case Language.Italian: 
                    returnLanguage = "Italian"; 
                    break;
                case Language.Japanese: 
                    returnLanguage = "Japanese"; 
                    break;
                case Language.Korean: 
                    returnLanguage = "Korean"; 
                    break;
                case Language.Norwegian: 
                    returnLanguage = "Norwegian"; 
                    break;
                case Language.Polish: 
                    returnLanguage = "Polish"; 
                    break;
                case Language.Russian: 
                    returnLanguage = "Russian"; 
                    break;
                case Language.SimplifiedChinese: 
                    returnLanguage = "Simplified Chinese"; 
                    break;
                case Language.Spanish: 
                    returnLanguage = "Spanish"; 
                    break;
                case Language.Swedish: 
                    returnLanguage = "Swedish"; 
                    break;
                case Language.TraditionalChinese: 
                    returnLanguage = "Traditional Chinese"; 
                    break;
                case Language.Turkish: 
                    returnLanguage = "Turkish"; 
                    break;
                default: 
                    returnLanguage = "Unknown Language " + lang.ToString(); 
                    break;
            }

            return returnLanguage;
        }

        /// <summary>
        /// Gets the language ID of a Language enumeration value
        /// </summary>
        /// <param name="lang">The language to convert to a language id</param>
        /// <returns>The language id string</returns>
        private string GetLanguageID(Language lang)
        {
            string returnLanguage;
            switch (lang)
            {
                case Language.BrazilianPortuguese: 
                    returnLanguage = "pt-PT"; 
                    break;
                case Language.Dutch: 
                    returnLanguage = "da-DK"; 
                    break;
                case Language.English:
                    returnLanguage = "en-US"; 
                    break;
                case Language.French: 
                    returnLanguage = "fr-FR"; 
                    break;
                case Language.German: 
                    returnLanguage = "de-DE"; 
                    break;
                case Language.Italian: 
                    returnLanguage = "it-IT"; 
                    break;
                case Language.Japanese: 
                    returnLanguage = "ja-JP"; 
                    break;
                case Language.Korean: 
                    returnLanguage = "ko-KR"; 
                    break;
                case Language.Norwegian: 
                    returnLanguage = "nb-NO"; 
                    break;
                case Language.Polish: 
                    returnLanguage = "pl-PL"; 
                    break;
                case Language.Russian: 
                    returnLanguage = "ru-RU"; 
                    break;
                case Language.SimplifiedChinese: 
                    returnLanguage = "zh-CN"; 
                    break;
                case Language.Spanish: 
                    returnLanguage = "es-ES"; 
                    break;
                case Language.Swedish: 
                    returnLanguage = "sv-SE"; 
                    break;
                case Language.TraditionalChinese: 
                    returnLanguage = "zh-CHT"; 
                    break;
                case Language.Turkish: 
                    returnLanguage = "tr-TR"; 
                    break;
                default: 
                    returnLanguage = string.Empty;
                    break;
            }

            return returnLanguage;
        }

        /// <summary>
        /// Gets the driving Xbox
        /// </summary>
        /// <returns>The console that will be used for gameplay</returns>
        private IXboxDevice GetDrivingXbox()
        {
            string catString = "Certification Assistance Tool";
            IXboxDevice device = null;

            // at least one selection
            if (this.moduleContext.SelectedDevices.Count() == 0)
            {
                MessageBox.Show("No consoles are selected. Select one.", catString);
                return null;
            }

            // only one
            if (this.moduleContext.SelectedDevices.Count() > 1)
            {
                MessageBox.Show("Select just the Xbox that is connected to your PC. The other consoles will be selected in the next page", catString);
                return null;
            }

            device = (IXboxDevice)this.moduleContext.SelectedDevices.First();

            // make sure the Xboxes is connected
                // connected
            if (!device.Connected)
            {
                MessageBox.Show("The selected device " + device.Name + " is not connected. Connect the device.", catString);
                return null;
            }

            return device;
        }

        /// <summary>
        /// Get an existing profile or create a new one if none exist
        /// </summary>
        /// <param name="device">The xbox with the profiles</param>
        /// <param name="which">
        /// "new" - create a new profile
        /// "first" - return the first profile. if there are no profiles create one
        /// "last" - return the last profile. if there are no profiles create one</param>
        /// <returns>A console profile</returns>
        private ConsoleProfile GetAProfile(IXboxDevice device, string which)
        {
            ConsoleProfile profile = null;
            try
            {
                ConsoleProfilesManager profilesManager = device.XboxConsole.CreateConsoleProfilesManager();
                IEnumerable<ConsoleProfile> profiles = profilesManager.EnumerateConsoleProfiles();

                try
                {
                    if (which == "new")
                    {
                        for (int retry = 0; retry++ < 3; retry++)
                        {
                            try
                            {
                                profile = profilesManager.CreateConsoleProfile(true);
                                return profile;
                            }
                            catch (Exception)
                            {
                            }
                        }

                        profile = profilesManager.CreateConsoleProfile(true);
                    }
                    else
                    {
                        if (profiles.Any())
                        {
                            if (which == "first")
                            {
                                profile = profiles.First();
                            }

                            if (which == "last")
                            {
                                profile = profiles.Last();
                            }
                        }

                        if (!profiles.Any())
                        {
                            for (int retry = 0; retry++ < 3; retry++)
                            {
                                try
                                {
                                    profile = profilesManager.CreateConsoleProfile(true);
                                    return profile;
                                }
                                catch (Exception)
                                {
                                }
                            }

                            profile = profilesManager.CreateConsoleProfile(true);
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        "There was an error creating a profile on console " + device.IP + "\n\n" + e.Message + "\n\nAttempting to use an existing profile",
                        "Certification Assistance Tool");
                    try
                    {
                        if (profiles.Any())
                        {
                            profile = profiles.First();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "There was an error getting an existing profile on console " + device.IP + "\n\n" + ex.Message + "\n\n Suggest you abort the module",
                            "Certification Assistance Tool");
                        this.UnbindAll();
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "There was an error managing profiles on console " + device.IP + "\n\n" + e.Message + "\n\nModule cannot continue",
                    "Certification Assistance Tool");
                this.UnbindAll();
                throw;
            }
             
            return profile;
        }

        /// <summary>
        /// Gets the next language
        /// </summary>
        /// <param name="languages">A list of all languages</param>
        /// <returns>The next language in the list</returns>
        private CAT.Language GetNextLanguage(List<Language> languages)
        {
            if (languages.Any())
            {
                Language l = languages.First();
                languages.RemoveAt(0);
                return l;
            }

            return Language.Dutch; // Dutch is not currently submittable in Rich Presence
        }

        /// <summary>
        /// Parses a string into an integer that may be represented as a decimal or a hexidecimal value
        /// </summary>
        /// <param name="intString">String to convert into an integer</param>
        /// <returns>The integer that was parsed from the specified string</returns>
        private uint ParseDecimalOrHexInt(string intString)
        {
            if (intString.StartsWith("0x") || intString.StartsWith("0X"))
            {
                return Convert.ToUInt32(intString.Substring(2), 16);
            }
            
            return Convert.ToUInt32(intString);
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
        /// Command class
        /// </summary>
        public class Command : ICommand
        {
            /// <summary>
            /// Action to execute on command
            /// </summary>
            private Action<object> action;

            /// <summary>
            /// Initializes a new instance of the Command class
            /// </summary>
            /// <param name="action">Action to execute</param>
            /// <param name="canExecute">Default flag for command being enabled</param>
            public Command(Action<object> action, bool canExecute = true)
            {
                this.action = action;
                this.CanCommandExecute = canExecute;
            }

            /// <summary>
            /// CanExecuteChanged event that we don't really care about at the moment
            /// </summary>
            public event EventHandler CanExecuteChanged = new EventHandler((o, e) => { });

            /// <summary>
            /// Gets or sets a value indicating whether the command can execute
            /// </summary>
            public bool CanCommandExecute { get; set; }

            /// <summary>
            /// Returns true when command can be executed
            /// </summary>
            /// <param name="parameter">unused command parameter</param>
            /// <returns>True if the command can execute</returns>
            public bool CanExecute(object parameter)
            {
                return this.CanCommandExecute;
            }

            /// <summary>
            /// Execute command
            /// </summary>
            /// <param name="parameter">handler parameter</param>
            public void Execute(object parameter)
            {
                if (this.action != null)
                {
                    this.action(parameter);
                }
            }
        }

        /// <summary>
        /// Class representing a single Term
        /// </summary>
        public class Term : INotifyPropertyChanged
        {
            /// <summary>
            /// Backing field for MatchesFilter property
            /// </summary>
            private bool matchesFilter = true;

            /// <summary>
            /// Backing field for IsSelected property
            /// </summary>
            private bool isSelected;

            /// <summary>
            /// Initializes a new instance of the <see cref="Term" /> class.
            /// </summary>
            /// <param name="area">Terminology area of this Term</param>
            public Term(TerminologyArea area)
            {
                this.TerminologyArea = area;
                this.Terms = new Dictionary<string, string>();
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets a value indicating whether this term matches the filter
            /// </summary>
            public bool MatchesFilter
            {
                get
                {
                    return this.matchesFilter;
                }

                set
                {
                    this.matchesFilter = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this term is selected
            /// </summary>
            public bool IsSelected
            {
                get
                {
                    return this.isSelected;
                }

                set
                {
                    this.isSelected = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets the terminology area associated with this Term
            /// </summary>
            public TerminologyArea TerminologyArea { get; set; }

            /// <summary>
            /// Gets or sets the description of this term
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets mappings of language to localized term
            /// </summary>
            public Dictionary<string, string> Terms { get; set; }

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
        /// A class representing a terminology area
        /// </summary>
        public class TerminologyArea
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TerminologyArea" /> class.
            /// </summary>
            /// <param name="name">Name of this terminology area</param>
            public TerminologyArea(string name)
            {
                this.Name = name;
                this.Terms = new List<Term>();
                this.TermsByLanguage = new Dictionary<string, List<KeyValuePair<string, Term>>>();
            }

            /// <summary>
            /// Gets or sets the name of this terminology area
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a list of terms associated with this terminology area
            /// </summary>
            public List<Term> Terms { get; set; }

            /// <summary>
            /// Gets or sets a mapping of language to list of terms
            /// </summary>
            public Dictionary<string, List<KeyValuePair<string, Term>>> TermsByLanguage { get; set; }
        }

        /// <summary>
        /// A class representing an observed language
        /// </summary>
        public class ObservedLanguage : INotifyPropertyChanged
        {
            /// <summary>
            /// Backing field for Observer property
            /// </summary>
            private ObserverConsole observer;

            /// <summary>
            /// Backing field for LanguageName property
            /// </summary>
            private string languageName;

            /// <summary>
            /// Backing field for ImageContents property
            /// </summary>
            private BitmapImage imageContents;

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservedLanguage" /> class.
            /// </summary>
            /// <param name="languageName">Language name to associate with this object</param>
            /// <param name="languageID">Language ID to associate with this object</param>
            /// <param name="language">Language enumeration value to associate with this object</param>
            /// <param name="refreshCommand">A command to execute when Refresh is selected on this language's screen snapshot</param>
            /// <param name="bindOneCommand">A command to execute when you need to control just one Xbox</param>
            public ObservedLanguage(string languageName, string languageID, Language language, Command refreshCommand, Command bindOneCommand)
            {
                this.Language = language;
                this.LanguageID = languageID;
                this.LanguageName = languageName;
                this.RefreshCommand = refreshCommand;
                this.BindOneCommand = bindOneCommand;
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets the associated language
            /// </summary>
            public Language Language { get; set; }

            /// <summary>
            /// Gets or sets the associated language id
            /// </summary>
            public string LanguageID { get; set; }

            /// <summary>
            /// Gets or sets the associated language name
            /// </summary>
            public string LanguageName
            {
                get
                {
                    return this.languageName;
                }

                set
                {
                    this.languageName = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets the name of the screen snapshot for this language
            /// </summary>
            public string ImageName { get; set; }

            /// <summary>
            /// Gets or sets a full path to the screen snapshot for this language
            /// </summary>
            public string ImageWholePath { get; set; }

            /// <summary>
            /// Gets or sets the bitmap image for this screen snapshot
            /// </summary>
            public BitmapImage ImageContents
            {
                get
                {
                    return this.imageContents;
                }

                set
                {
                    this.imageContents = value;
                    this.NotifyPropertyChanged();
                }
            }
        
            /// <summary>
            /// Gets or sets the observing console for this language
            /// </summary>
            public ObserverConsole Observer
            {
                get
                {
                    return this.observer;
                }

                set
                {
                    this.observer = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets the command invoked when Refresh is selected on this language's screen snapshot
            /// </summary>
            public Command RefreshCommand { get; set; }

            /// <summary>
            /// Gets or sets the command invoked when Retry Navigation is selected on this language's screen snapshot
            /// </summary>
            public Command BindOneCommand { get; set; }

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
        /// Observer Console class
        /// </summary>
        public class ObserverConsole : INotifyPropertyChanged
        {
            /// <summary>
            /// Backing field for ConsoleName property
            /// </summary>
            private string consoleName;

            /// <summary>
            /// Backing field for ProfileName property
            /// </summary>
            private string profileName;

            /// <summary>
            /// Backing field for SignedIn property
            /// </summary>
            private string signedIn;

            /// <summary>
            /// Backing field for Installed property
            /// </summary>
            private string installed;

            /// <summary>
            /// Initializes a new instance of the <see cref="ObserverConsole" /> class.
            /// </summary>
            /// <param name="device">Xbox to associate with this object</param>
            /// <param name="profile">Profile to associate with this object</param>
            public ObserverConsole(IXboxDevice device, ConsoleProfile profile)
            {
                this.Device = device;
                this.Profile = profile;
                if (device != null)
                {
                    this.ConsoleName = device.Name;
                    this.ConsoleIP = device.IP;
                }

                if (profile != null)
                {
                    this.ProfileName = profile.Gamertag;
                }
                else
                {
                    this.ProfileName = "*";
                }

                this.SignedIn = "*";
                this.Installed = "*";
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets the xbox object associated with this object
            /// </summary>
            public IXboxDevice Device { get; set; }

            /// <summary>
            /// Gets or sets the profile associated with this object
            /// </summary>
            public ConsoleProfile Profile { get; set; }

            /// <summary>
            /// Gets or sets the name of the console associated with this object
            /// </summary>
            public string ConsoleName
            {
                get
                {
                    return this.consoleName;
                }

                set
                {
                    this.consoleName = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets the IP of the console associated with this object
            /// </summary>
            public string ConsoleIP { get; set; }

            /// <summary>
            /// Gets or sets the ObservedLanguage associated with this console
            /// </summary>
            public ObservedLanguage ObservedLanguage { get; set; }

            /// <summary>
            /// Gets or sets the profile name associated with the profile on this console
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
            /// Gets or sets a string to display as the Signed-In status of the profile on this console
            /// </summary>
            public string SignedIn
            {
                get
                {
                    return this.signedIn;
                }

                set
                {
                    this.signedIn = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets a string to display the Installed status of the title on this console
            /// </summary>
            public string Installed
            {
                get
                {
                    return this.installed;
                }

                set
                {
                    this.installed = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets a path to an image
            /// </summary>
            public string ImageWholePath { get; set; }

            /// <summary>
            /// Gets or sets the automation object associated with this console
            /// </summary>
            public IXboxAutomation Automation { get; set; }

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
        /// Available console
        /// </summary>
        public class AvailableConsole
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AvailableConsole" /> class.
            /// </summary>
            /// <param name="device">The Xbox to associate with this object</param>
            public AvailableConsole(IXboxDevice device)
            {
                this.Name = device.Name;
                this.IsSelected = device.IsSelected;
                this.Device = device;
            }

            /// <summary>
            /// Gets or sets the name of this console
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not this console is selected
            /// </summary>
            public bool IsSelected { get; set; }

            /// <summary>
            /// Gets or sets the xbox associated with this object
            /// </summary>
            public IXboxDevice Device { get; set; }
        }
    }
}
