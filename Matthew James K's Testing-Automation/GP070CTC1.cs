// -----------------------------------------------------------------------
// <copyright file="GP070CTC1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace GP070
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using System.Xml;
    using CAT;
    using Microsoft.Test.Xbox.Profiles;
    using XDevkit;

    /// <summary>
    /// GP 70 Module
    /// </summary>
    public class GP070CTC1 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// Private module context instance for this current working module context
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// Private GP070CTC1UI user interface property for this user interface component
        /// </summary>
        private GP070CTC1UI moduleUI;

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
        /// Backing field for AllObservedLanguages property
        /// </summary>
        private SortedDictionary<string, ObservedLanguage> allObservedLanguages;

        /// <summary>
        /// Backing field for LanguagesWithImages property
        /// </summary>
        private ObservableCollection<ObservedLanguage> languagesWithImages;

        /// <summary>
        /// Backing field for ActiveProfileName
        /// </summary>
        private string activeProfileName;

        /// <summary>
        /// Backing field for InactiveProfileName property
        /// </summary>
        private string inactiveProfileName;

        /// <summary>
        /// Backing field for HasMoreLanguagesThanObservers property
        /// </summary>
        private bool hasMoreLanguagesThanObservers;

        /// <summary>
        /// Backing field for HasMoreLanguages property
        /// </summary>
        private bool hasMoreLanguages;

        /// <summary>
        /// Backing field for HasAllSnapshots property
        /// </summary>
        private bool hasAllSnapshots;

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
        /// Backing field for IsEnglishSupported property
        /// </summary>
        private bool isEnglishSupported;

        /// <summary>
        /// Backing field for IsFrenchSupported property
        /// </summary>
        private bool isFrenchSupported;

        /// <summary>
        /// Backing field for IsKoreanSupported property
        /// </summary>
        private bool isKoreanSupported;

        /// <summary>
        /// Backing field for IsRussianSupported property
        /// </summary>
        private bool isRussianSupported;

        /// <summary>
        /// Backing field for IsChineseSimplifiedSupported property
        /// </summary>
        private bool isChineseSimplifiedSupported;

        /// <summary>
        /// Backing field for IsPortugueseSupported property
        /// </summary>
        private bool isPortugueseSupported;

        /// <summary>
        /// Backing field for IsItalianSupported property
        /// </summary>
        private bool isItalianSupported;

        /// <summary>
        /// Backing field for IsGermanSupported property
        /// </summary>
        private bool isGermanSupported;

        /// <summary>
        /// Backing field for IsSpanishSupported property
        /// </summary>
        private bool isSpanishSupported;

        /// <summary>
        /// Backing field for IsJapaneseSupported property
        /// </summary>
        private bool isJapaneseSupported;

        /// <summary>
        /// Backing field for IsPolishSupported property
        /// </summary>
        private bool isPolishSupported;

        /// <summary>
        /// Backing field for IsChineseTraditionalSupported property
        /// </summary>
        private bool isChineseTraditionalSupported;

        /// <summary>
        /// Backing field for IsDutchSupported property
        /// </summary>
        private bool isDutchSupported;

        /// <summary>
        /// Backing field for IsNorwegianSupported property
        /// </summary>
        private bool isNorwegianSupported;

        /// <summary>
        /// Backing field for IsSwedishSupported property
        /// </summary>
        private bool isSwedishSupported;

        /// <summary>
        /// Backing field for IsTurkishSupported property
        /// </summary>
        private bool isTurkishSupported;

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
        /// Value indicating which page is currently visible
        /// </summary>
        private int page = 1;

        /// <summary>
        /// Indicates whether we've yet tried to land on page 4
        /// </summary>
        private bool firstMoveToPage4 = true;

        /// <summary>
        /// A value indicating whether or not HTML logging has started
        /// </summary>
        private bool htmlLogStarted;

        /// <summary>
        /// Backing field for SupportedLanguages property
        /// </summary>
        private ObservableCollection<string> supportedLanguages;

        /// <summary>
        /// Backing field for SelectedLanguage property
        /// </summary>
        private string selectedLanguage = "en-US";

        /// <summary>
        /// Backing field for LocalizedStrings property
        /// </summary>
        private SortedDictionary<uint, LocalizedString> localizedStrings;

        /// <summary>
        /// Backing field for StringContexts property
        /// </summary>
        private SortedDictionary<uint, StringContext> stringContexts;

        /// <summary>
        /// Backing field for StringProperties property
        /// </summary>
        private SortedDictionary<uint, StringProperty> stringProperties;

        /// <summary>
        /// Final passed or failed status flag when Stop() is finally called
        /// </summary>
        private string passedOrFailed = "DONE";

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
        /// Gets or sets the list of all observed languages
        /// </summary>
        public SortedDictionary<string, ObservedLanguage> AllObservedLanguages
        {
            get
            {
                return this.allObservedLanguages;
            }

            set
            {
                this.allObservedLanguages = value;
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
        /// Gets or sets of a list of unused observed languages
        /// </summary>
        public SortedDictionary<string, ObservedLanguage> UnusedObservedLanguages { get; set; }

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
        /// Gets or sets the inactive profile name
        /// </summary>
        public string InactiveProfileName
        {
            get
            {
                return this.inactiveProfileName;
            }

            set
            {
                this.inactiveProfileName = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not there are more languages than observers
        /// </summary>
        public bool HasMoreLanguagesThanObservers
        {
            get
            {
                return this.hasMoreLanguagesThanObservers;
            }

            set
            {
                this.hasMoreLanguagesThanObservers = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not there are more languages remaining
        /// </summary>
        public bool HasMoreLanguages
        {
            get
            {
                return this.hasMoreLanguages;
            }

            set
            {
                this.hasMoreLanguages = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not there are snapshots for all languages
        /// </summary>
        public bool HasAllSnapshots
        {
            get
            {
                return this.hasAllSnapshots;
            }

            set
            {
                this.hasAllSnapshots = value;
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
                if (this.TestInEnglish)
                {
                    i++;
                }

                if (this.TestInFrench)
                {
                    i++;
                }

                if (this.TestInKorean)
                {
                    i++;
                }

                if (this.TestInRussian)
                {
                    i++;
                }

                if (this.TestInChineseSimplified)
                {
                    i++;
                }

                if (this.TestInPortuguese)
                {
                    i++;
                }

                if (this.TestInItalian)
                {
                    i++;
                }

                if (this.TestInGerman)
                {
                    i++;
                }

                if (this.TestInSpanish)
                {
                    i++;
                }

                if (this.TestInJapanese)
                {
                    i++;
                }

                if (this.TestInPolish)
                {
                    i++;
                }

                if (this.TestInChineseTraditional)
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
        /// Gets or sets a value indicating whether English is supported
        /// </summary>
        public bool IsEnglishSupported
        {
            get
            {
                return this.isEnglishSupported;
            }

            set
            {
                this.isEnglishSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether French is supported
        /// </summary>
        public bool IsFrenchSupported
        {
            get
            {
                return this.isFrenchSupported;
            }

            set
            {
                this.isFrenchSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Korean is supported
        /// </summary>
        public bool IsKoreanSupported
        {
            get
            {
                return this.isKoreanSupported;
            }

            set
            {
                this.isKoreanSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Russian is supported
        /// </summary>
        public bool IsRussianSupported
        {
            get
            {
                return this.isRussianSupported;
            }

            set
            {
                this.isRussianSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Simplified Chinese is supported
        /// </summary>
        public bool IsChineseSimplifiedSupported
        {
            get
            {
                return this.isChineseSimplifiedSupported;
            }

            set
            {
                this.isChineseSimplifiedSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Portuguese is supported
        /// </summary>
        public bool IsPortugueseSupported
        {
            get
            {
                return this.isPortugueseSupported;
            }

            set
            {
                this.isPortugueseSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Italian is supported
        /// </summary>
        public bool IsItalianSupported
        {
            get
            {
                return this.isItalianSupported;
            }

            set
            {
                this.isItalianSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether German is supported
        /// </summary>
        public bool IsGermanSupported
        {
            get
            {
                return this.isGermanSupported;
            }

            set
            {
                this.isGermanSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Spanish is supported
        /// </summary>
        public bool IsSpanishSupported
        {
            get
            {
                return this.isSpanishSupported;
            }

            set
            {
                this.isSpanishSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Japanese is supported
        /// </summary>
        public bool IsJapaneseSupported
        {
            get
            {
                return this.isJapaneseSupported;
            }

            set
            {
                this.isJapaneseSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Polish is supported
        /// </summary>
        public bool IsPolishSupported
        {
            get
            {
                return this.isPolishSupported;
            }

            set
            {
                this.isPolishSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Traditional Chinese is supported
        /// </summary>
        public bool IsChineseTraditionalSupported
        {
            get
            {
                return this.isChineseTraditionalSupported;
            }

            set
            {
                this.isChineseTraditionalSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Dutch is supported
        /// </summary>
        public bool IsDutchSupported 
        {
            get
            {
                return this.isDutchSupported;
            }

            set
            {
                this.isDutchSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Norwegian is supported
        /// </summary>
        public bool IsNorwegianSupported
        {
            get
            {
                return this.isNorwegianSupported;
            }

            set
            {
                this.isNorwegianSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Swedish is supported
        /// </summary>
        public bool IsSwedishSupported
        {
            get
            {
                return this.isSwedishSupported;
            }

            set
            {
                this.isSwedishSupported = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Turkish is supported
        /// </summary>
        public bool IsTurkishSupported
        {
            get
            {
                return this.isTurkishSupported;
            }

            set
            {
                this.isTurkishSupported = value; 
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
        /// Gets or sets the default language
        /// </summary>
        public string DefaultLanguage { get; set; }

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
        /// Gets a sorted dictionary of all presence modes
        /// </summary>
        public SortedDictionary<uint, PresenceMode> PresenceModes
        {
            get { return PresenceMode.AllModes; }
        }

        /// <summary>
        /// Gets or sets a sorted dictionary of localized strings
        /// </summary>
        public SortedDictionary<uint, LocalizedString> LocalizedStrings
        {
            get
            {
                return this.localizedStrings;
            }

            set
            {
                this.localizedStrings = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a sorted dictionary of string contexts
        /// </summary>
        public SortedDictionary<uint, StringContext> StringContexts
        {
            get
            {
                return this.stringContexts;
            }

            set
            {
                this.stringContexts = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a sorted dictionary of string properties
        /// </summary>
        public SortedDictionary<uint, StringProperty> StringProperties
        {
            get
            {
                return this.stringProperties;
            }

            set
            {
                this.stringProperties = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the active profile
        /// </summary>
        private ConsoleProfile ActiveProfile { get; set; }

        /// <summary>
        /// Gets or sets the inactive profile
        /// </summary>
        private ConsoleProfile InactiveProfile { get; set; }

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
            this.page = 1;
            this.moduleUI = new GP070CTC1UI(this);
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

                    // ==> move to page 2 setup
                    this.moduleContext.IsModal = true;
                    this.OverviewPageVisibility = Visibility.Collapsed;
                    this.Page2Visibility = Visibility.Visible;
                    this.page = 2;
                    this.InitializeAvailableObserverList();
                    this.ObserverConsoles = new ObservableCollection<ObserverConsole>();
                    this.AllObservedLanguages = new SortedDictionary<string, ObservedLanguage>();
                    this.UnusedObservedLanguages = new SortedDictionary<string, ObservedLanguage>();
                    this.LanguagesWithImages = new ObservableCollection<ObservedLanguage>();
                    this.LoadPresenceStrings(this.DrivingConsole);
                    this.SetLanguageVisibilities();
                    this.moduleContext.Log("Controlling Console: " + this.DrivingConsole.Name + " = " + this.DrivingConsole.IP);
                    break;
                case 2: // on page 2 choose languages and consoles
                    if (this.ConsoleCount == 0)
                    {
                        MessageBox.Show("At least one observer console must be selected.", "No consoles selected.");
                    }
                    else
                    {
                        bool selectionDone = true;
                        if (this.ConsoleCount < this.LanguageCount)
                        {
                            MessageBoxResult result = MessageBox.Show("More languages are selected than consoles.  Observer consoles will cycle through languages as needed.  This will take longer than having one observer console per language.  Continue?", "More languages than consoles", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
                            if (result != MessageBoxResult.OK)
                                selectionDone = false;
                        }
                        else if (this.ConsoleCount > this.LanguageCount)
                        {
                            MessageBox.Show("More consoles are selected than languages.  Please deselect consoles.", "Too many consoles");
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
                            Mouse.OverrideCursor = Cursors.Wait;
                            this.SetupConsoles();
                            this.IsSetupDone = true;
                            Mouse.OverrideCursor = null;
                        }
                    }

                    break;
                case 3: // on page 3 - no button for user, we call 'next' when auto-setup is done
                        // ==> move to page 4 test
                    if (this.firstMoveToPage4)
                    {
                        this.CaptureScreens();
                        this.firstMoveToPage4 = false;
                    }

                    this.Page3Visibility = Visibility.Collapsed;
                    this.Page4Visibility = Visibility.Visible;
                    this.page = 4;
                    break;
                case 4: // on page 4 - going through rich-presence strings. button now says 'back'
                        // ==> move back to page 3
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
        }

        /// <summary>
        /// Save all log snapshots
        /// </summary>
        public void SaveAndLogSnapshots()
        {
            // Log this set of images

            // Look for checked presenceMode or checked expanded string
            string language = this.SelectedLanguage;
            LocalizedString localizedString = null;
            ExpandedString expandedString = null;

            Mouse.OverrideCursor = Cursors.Wait;

            foreach (KeyValuePair<uint, PresenceMode> pair in PresenceMode.AllModes)
            {
                if (pair.Value.IsChecked)
                {
                    localizedString = pair.Value.LocalizedString;
                    pair.Value.IsChecked = false;
                    pair.Value.DisplayBrush = Brushes.Gray;
                    break;
                }
            }

            if (localizedString == null)
            {
                foreach (ExpandedString expandedString2 in ExpandedString.AllStrings)
                {
                    if (expandedString2.IsChecked)
                    {
                        expandedString = expandedString2;
                        localizedString = expandedString.TranslatedString.LocalizedString;
                        expandedString2.IsChecked = false;
                        expandedString2.DisplayBrush = Brushes.Gray;
                        break;
                    }
                }
            }

            if (localizedString == null)
            {
                // Warn user they haven't selected a string. don't save anything.
                MessageBox.Show(
                    "We need you to tell us which rich presence string these screen shots represent",
                    "Certification Assistance Tool");
            }
            else
            {
                List<Thread> logThreads = new List<Thread>();
                LogLineData logLineData = new LogLineData();
                logLineData.LocalizedString = localizedString.FriendlyName; // Store checked localized string for this set of captures.

                foreach (KeyValuePair<string, ObservedLanguage> lang in this.AllObservedLanguages)
                {
                    CaptureData captureData = new CaptureData();

                    captureData.ImagePath = Path.Combine(this.moduleContext.LogDirectory, "images", lang.Value.ImageName);
                    captureData.LanguageName = lang.Value.LanguageName;
                    captureData.LanguageID = lang.Value.LanguageID;

                    try
                    {
                        string languageID = captureData.LanguageID;
                        if (expandedString == null)
                        {
                            // Presence was selected
                            if (!localizedString.TranslatedStrings.ContainsKey(languageID))
                            {
                                languageID = this.DefaultLanguage;
                            }

                            // If a presense mode was selected, then we know there is only 1 string per language in the translatedStrings on the localizedString
                            captureData.TranslatedString = localizedString.TranslatedStrings[languageID].ExpandedStrings[0].Value;
                        }
                        else
                        {
                            if (!expandedString.TranslatedString.LocalizedString.TranslatedStrings.ContainsKey(languageID))
                            {
                                languageID = this.DefaultLanguage;
                            }

                            int expandedIndex = expandedString.TranslatedString.ExpandedStrings.IndexOf(expandedString);

                            // If an expanded string, we need to use it's index in the list of expanded string in the current language, to determine the index of other languages
                            captureData.TranslatedString = expandedString.TranslatedString.LocalizedString.TranslatedStrings[languageID].ExpandedStrings[expandedIndex].Value;
                        }
                    }
                    catch
                    {
                        captureData.TranslatedString = "ERROR Malformed Gameconfig";
                    }

                    logLineData.CaptureData.Add(captureData.LanguageID, captureData);
                }

                this.AddToHtmlLog(logLineData);
                this.imagesHaveBeenSaved = true;

                foreach (KeyValuePair<string, ObservedLanguage> pair in this.AllObservedLanguages)
                {
                    pair.Value.ImageContents = null;
                    pair.Value.ImageWholePath = string.Empty;
                    this.LanguagesWithImages.Remove(pair.Value);
                }
            }

            Mouse.OverrideCursor = null;
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

            Mouse.OverrideCursor = Cursors.Wait;

            if (!this.firstMoveToPage4)
            {
                this.WakeUpXboxDevkits();
                Thread.Sleep(1500);
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // keep the last set of images if we saved them. othewise overwrite them with the same index
            if (!this.imagesHaveBeenSaved)
            {
                foreach (KeyValuePair<string, ObservedLanguage> pair in this.AllObservedLanguages)
                {
                    if (!string.IsNullOrEmpty(pair.Value.ImageWholePath))
                    {
                        this.throwawayImages.Add(pair.Value.ImageWholePath);
                        this.LanguagesWithImages.Remove(pair.Value);
                        pair.Value.ImageWholePath = string.Empty;
                        pair.Value.ImageContents = null;
                    }
                }
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
            this.HasAllSnapshots = this.UnusedObservedLanguages.Count == 0;
            this.HasMoreLanguages = this.UnusedObservedLanguages.Count != 0;
            this.UpdateUIImmediately();

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Capture more screens
        /// </summary>
        public void CaptureMoreScreens()
        {
            string path = Path.Combine(this.moduleContext.LogDirectory, "images");

            Mouse.OverrideCursor = Cursors.Wait;

            // Make a list of all Observers to use.  If we don't need the one in the default language, don't use it.
            int numMoreLanguages = this.UnusedObservedLanguages.Count;
            int numObserverConsoles = this.ObserverConsoles.Count;
            int numRounds = 1;
            if (numMoreLanguages > numObserverConsoles)
            {
                numRounds = numMoreLanguages / numObserverConsoles;
                if ((numMoreLanguages % numObserverConsoles) != 0)
                {
                    numRounds++;
                }
            }

            List<ObservedLanguage> missingLanguages = new List<ObservedLanguage>();
            foreach (KeyValuePair<string, ObservedLanguage> pair in this.UnusedObservedLanguages)
            {
                missingLanguages.Add(pair.Value);
            }

            while (missingLanguages.Count != 0)
            {
                int remainingCount = missingLanguages.Count;
                List<ObserverConsole> consolesToUse = new List<ObserverConsole>();
                foreach (ObserverConsole observer in this.ObserverConsoles)
                {
                    if ((numMoreLanguages >= remainingCount) || (observer.ObservedLanguage.LanguageID != this.SelectedLanguage))
                    {
                        consolesToUse.Add(observer);
                        remainingCount--;
                        if (remainingCount == 0)
                        {
                            break;
                        }
                    }
                }

                // Thread simultaneous snapshots
                List<Thread> captureThreads = new List<Thread>();
                foreach (ObserverConsole observer in consolesToUse)
                {
                    observer.ObservedLanguage.IsObserved = false;
                    observer.ObservedLanguage.Observer = null;
                    observer.ObservedLanguage = missingLanguages[0];
                    observer.ObservedLanguage.IsObserved = true;
                    observer.ObservedLanguage.Observer = observer;
                    missingLanguages.RemoveAt(0);
                    this.LanguagesWithImages.Add(observer.ObservedLanguage);
                    Thread th = new Thread(new ParameterizedThreadStart(delegate
                    {
                        observer.Device.SetLanguage(observer.ObservedLanguage.Language, false);
                        observer.Device.Reboot(true);

                        // sign in a profile
                        Thread.Sleep(2000);
                        observer.Profile.SignIn(UserIndex.Zero);
                        Thread.Sleep(500);

                        observer.Device.OpenFriendPresenceScreen(observer.Profile);

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

                foreach (ObserverConsole observer in consolesToUse)
                {
                    observer.ObservedLanguage.ImageContents = LoadImage(observer.ObservedLanguage.ImageWholePath);
                }
            }

            // Rebuild list of UnusedObservedLanguages
            this.UnusedObservedLanguages.Clear();
            foreach (KeyValuePair<string, ObservedLanguage> pair in this.AllObservedLanguages)
            {
                if (!pair.Value.IsObserved)
                {
                    this.UnusedObservedLanguages.Add(pair.Value.LanguageID, pair.Value);
                }
            }

            this.HasMoreLanguages = false;
            this.HasAllSnapshots = true;
            this.UpdateUIImmediately();

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Stop the module
        /// </summary>
        public void Stop()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.FinishHtmlLog();

            // remove friend relationships
            try
            {
                foreach (ObserverConsole item in this.ObserverConsoles)
                {
                    item.Profile.Friends.RemoveFriend(this.ActiveProfile);
                    item.Profile.Friends.RemoveFriend(this.InactiveProfile);
                }
            }
            catch
            {
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

            // reset observer boxes to english
            try
            {
                foreach (ObserverConsole item in this.ObserverConsoles)
                {
                    item.Device.SetLanguage(Language.English, false);
                    item.Device.SetVideoMode(VideoResolution.Mode1080p, VideoStandard.NTSCM, false);
                    item.Device.Reboot(false);
                    item.Device.DeferDisconnect();
                    Thread th = new Thread(new ParameterizedThreadStart(delegate
                    {
                        item.Device.WaitForRebootToComplete();
                        item.Device.AllowDisconnect();
                    }));
                    th.Name = "GP070.Stop";
                    th.Start();
                }
            }
            catch
            {
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
            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("RESULT: " + this.passedOrFailed + "\r\n");
            this.moduleContext.Log("*************************************************************\r\n");

            Mouse.OverrideCursor = null;
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
            Mouse.OverrideCursor = Cursors.Wait;
            ObserverConsole observer = observedLanguage.Observer;
            if (observer == null)
            {
                // Claim some other language's observer
                observer = this.ObserverConsoles[0];
                if ((observer.ObservedLanguage.LanguageID == this.SelectedLanguage) && (this.ObserverConsoles.Count > 1))
                {
                    observer = this.ObserverConsoles[1];
                }

                observer.ObservedLanguage.Observer = null;
                observer.ObservedLanguage.IsObserved = false;
                observer.ObservedLanguage = observedLanguage;
                observedLanguage.Observer = observer;
                observedLanguage.IsObserved = true;

                observer.Device.SetLanguage(observedLanguage.Language, false);
                observer.Device.Reboot(true);

                // sign in a profile
                Thread.Sleep(2000);
                observer.Profile.SignIn(UserIndex.Zero);
                Thread.Sleep(500);

                observer.Device.OpenFriendPresenceScreen(observer.Profile);

                observedLanguage.ImageName = observedLanguage.LanguageName + observer.Device.IP + "image" + this.logInstance.ToString() + ".jpg";
                string path = Path.Combine(this.moduleContext.LogDirectory, "images");
                observedLanguage.ImageWholePath = Path.Combine(path, observedLanguage.ImageName);
            }

            File.Delete(observedLanguage.ImageWholePath);
            observer.Device.ScreenShot(observedLanguage.ImageWholePath);
            observedLanguage.ImageContents = LoadImage(observedLanguage.ImageWholePath);
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Retries navigation and refreshes the specified image
        /// </summary>
        /// <param name="observedLanguage">The language associated with the image to retry</param>
        private void RetryNavigation(ObservedLanguage observedLanguage)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            ObserverConsole observer = observedLanguage.Observer;

            observer.Device.Reboot(true);

            // sign in a profile
            Thread.Sleep(2000);
            observer.Profile.SignIn(UserIndex.Zero);
            Thread.Sleep(500);

            observer.Device.OpenFriendPresenceScreen(observer.Profile);

            observedLanguage.ImageName = observedLanguage.LanguageName + observer.Device.IP + "image" + this.logInstance.ToString() + ".jpg";
            string path = Path.Combine(this.moduleContext.LogDirectory, "images");
            observedLanguage.ImageWholePath = Path.Combine(path, observedLanguage.ImageName);
            File.Delete(observedLanguage.ImageWholePath);
            observer.Device.ScreenShot(observedLanguage.ImageWholePath);
            observedLanguage.ImageContents = LoadImage(observedLanguage.ImageWholePath);

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Populate setup table
        /// </summary>
        private void PopulateSetupTable()
        {
            ObserverConsole config = null;
            List<Language> languageList = new List<Language>();
            this.ObserverConsoles.Clear();
            this.UnusedObservedLanguages.Clear();
            this.AllObservedLanguages.Clear();

            Command refreshCommand = new Command((o) => this.Refresh(o as ObservedLanguage));
            Command retryNavigationCommand = new Command((o) => this.RetryNavigation(o as ObservedLanguage));

            // Get currently selected languages
            if (this.TestInEnglish)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.English), new ObservedLanguage(this.GetLanguageName(Language.English), this.GetLanguageID(Language.English), Language.English, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInFrench)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.French), new ObservedLanguage(this.GetLanguageName(Language.French), this.GetLanguageID(Language.French), Language.French, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInKorean)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.Korean), new ObservedLanguage(this.GetLanguageName(Language.Korean), this.GetLanguageID(Language.Korean), Language.Korean, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInRussian)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.Russian), new ObservedLanguage(this.GetLanguageName(Language.Russian), this.GetLanguageID(Language.Russian), Language.Russian, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInChineseSimplified)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.SimplifiedChinese), new ObservedLanguage(this.GetLanguageName(Language.SimplifiedChinese), this.GetLanguageID(Language.SimplifiedChinese), Language.SimplifiedChinese, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInPortuguese)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.BrazilianPortuguese), new ObservedLanguage(this.GetLanguageName(Language.BrazilianPortuguese), this.GetLanguageID(Language.BrazilianPortuguese), Language.BrazilianPortuguese, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInItalian)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.Italian), new ObservedLanguage(this.GetLanguageName(Language.Italian), this.GetLanguageID(Language.Italian), Language.Italian, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInGerman)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.German), new ObservedLanguage(this.GetLanguageName(Language.German), this.GetLanguageID(Language.German), Language.German, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInSpanish)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.Spanish), new ObservedLanguage(this.GetLanguageName(Language.Spanish), this.GetLanguageID(Language.Spanish), Language.Spanish, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInJapanese)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.Japanese), new ObservedLanguage(this.GetLanguageName(Language.Japanese), this.GetLanguageID(Language.Japanese), Language.Japanese, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInPolish)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.Polish), new ObservedLanguage(this.GetLanguageName(Language.Polish), this.GetLanguageID(Language.Polish), Language.Polish, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInChineseTraditional)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.TraditionalChinese), new ObservedLanguage(this.GetLanguageName(Language.TraditionalChinese), this.GetLanguageID(Language.TraditionalChinese), Language.TraditionalChinese, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInDutch)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.Dutch), new ObservedLanguage(this.GetLanguageName(Language.Dutch), this.GetLanguageID(Language.Dutch), Language.Dutch, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInNorwegian)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.Norwegian), new ObservedLanguage(this.GetLanguageName(Language.Norwegian), this.GetLanguageID(Language.Norwegian), Language.Norwegian, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInSwedish)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.Swedish), new ObservedLanguage(this.GetLanguageName(Language.Swedish), this.GetLanguageID(Language.Swedish), Language.Swedish, refreshCommand, retryNavigationCommand));
            }

            if (this.TestInTurkish)
            {
                this.AllObservedLanguages.Add(this.GetLanguageID(Language.Turkish), new ObservedLanguage(this.GetLanguageName(Language.Turkish), this.GetLanguageID(Language.Turkish), Language.Turkish, refreshCommand, retryNavigationCommand));
            }

            // Get currently selected consoles and show pending configuration of each one
            foreach (AvailableConsole available in this.AvailableConsoles)
            {
                if (available.IsSelected)
                {
                    // language = GetNextLanguage(languageList);
                    config = new ObserverConsole(available.Device, null);
                    this.ObserverConsoles.Add(config);
                }
            }

            foreach (KeyValuePair<string, ObservedLanguage> pair in this.AllObservedLanguages)
            {
                this.UnusedObservedLanguages.Add(pair.Key, pair.Value);
            }

            List<ObserverConsole> consolesWithoutLanguages = new List<ObserverConsole>();
            foreach (ObserverConsole console in this.ObserverConsoles)
            {
                consolesWithoutLanguages.Add(console);
            }

            if (this.UnusedObservedLanguages.ContainsKey(this.SelectedLanguage))
            {
                consolesWithoutLanguages[0].ObservedLanguage = this.UnusedObservedLanguages[this.SelectedLanguage];
                consolesWithoutLanguages[0].ObservedLanguage.IsObserved = true;
                consolesWithoutLanguages[0].ObservedLanguage.Observer = consolesWithoutLanguages[0];
                consolesWithoutLanguages.RemoveAt(0);
                this.UnusedObservedLanguages.Remove(this.SelectedLanguage);
            }

            foreach (ObserverConsole console in consolesWithoutLanguages)
            {
                KeyValuePair<string, ObservedLanguage> pair = this.UnusedObservedLanguages.First();
                console.ObservedLanguage = pair.Value;
                pair.Value.IsObserved = true;
                pair.Value.Observer = console;
                this.UnusedObservedLanguages.Remove(pair.Key);
            }

            this.HasMoreLanguagesThanObservers = this.UnusedObservedLanguages.Count != 0;
            this.HasMoreLanguages = false;
            this.HasAllSnapshots = false;
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
        /// setup up to 14 (18?) consoles. the active console has the title installed and friends all of
        /// the observer consoles. the observer consoles reboot into a unique language and accept
        /// the friend request
        /// </summary>
        private void SetupConsoles()
        {
            // start progress bar. woo hoo
            this.SetupProgress = 0;

            // get selected languages
            this.ChangeSetupProgress("Assigning Languages", 0);
            this.PopulateSetupTable();

            this.moduleContext.Log(this.ObserverConsoles.Count.ToString() + " observer consoles.  Setting up:");
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                this.moduleContext.Log("\t" + observer.ConsoleIP + " - " + observer.ConsoleName);
            }

            //////////////////////////////////////////////////////////////////
            // T H R E A D I N G
            //////////////////////////////////////////////////////////////////
            // O1  Observer : choose profiles
            // C1  Controller : create active profile, create inactive profile, sign in
            // O2  Observer : WAIT O1 : set language, cold boot, sign in
            // C2  Controller : WAIT O1 C1 : send friend requests
            // C3  Controller : WAIT C2 : install title
            // O3  Observer : WAIT O2 C2 : accept friend requests
            // 04  Observer : Navigate to friends screen
            // WAIT C3 O4

            // O1 choose a profile on each observer box. do this in parallel with threads
            this.ChangeSetupProgress("Choosing Observer Profiles", 1);
            List<Thread> profileThreads = new List<Thread>();
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                Thread th = new Thread(new ParameterizedThreadStart(delegate
                {
                    observer.Profile = this.GetAProfile(observer.Device, "last");
                    if (observer.Profile != null)
                    {
                        observer.ProfileName = observer.Profile.Gamertag;
                    }
                }));

                th.Name = "Get Profile";
                th.Start();
                profileThreads.Add(th);
            }

            // C1 find or create active and inactive profiles on the driving console
            this.ChangeSetupProgress("Choosing Two Profiles on " + this.DrivingConsole.Name, 1);
            Thread thcreate = new Thread(new ParameterizedThreadStart(delegate
            {
                this.DrivingConsole.WakeUpXboxDevkit();
                this.CreateActiveAndInactiveProfiles(this.DrivingConsole);
                this.ActiveProfileName = this.ActiveProfile.Gamertag;
                this.InactiveProfileName = this.InactiveProfile.Gamertag;
                this.ActiveProfile.SignIn(UserIndex.Zero);
                this.InactiveProfile.SignIn(UserIndex.One);
            }));
            thcreate.Name = "Active and Inactive Profiles";
            thcreate.Start();

            // WAIT for all observer profiles
            foreach (Thread th in profileThreads)
            {
                th.Join();
            }

            // WAIT for created profiles
            thcreate.Join();
            if (this.ActiveProfile == null || this.InactiveProfile == null)
            {
                return;
            }

            this.moduleContext.Log("Created two new profiles on " + this.DrivingConsole.Name + ": " + this.ActiveProfile.Gamertag + " and " + this.InactiveProfile.Gamertag);

            // C2 send out friend invitations to each observer profile
            //    no threads - everyone WAITs for this
            this.ChangeSetupProgress("Sending Friend Invitations", 2);
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                try
                {
                    this.ActiveProfile.SignIn(UserIndex.Zero);
                    this.moduleContext.Log("Sending friend request from " + this.ActiveProfile.Gamertag + " to " + observer.ProfileName);
                    this.ActiveProfile.Friends.SendFriendRequest(observer.Profile);
                    observer.ActiveFriendRequestSent = true;
                    this.moduleContext.Log("Sent friend request from " + this.ActiveProfile.Gamertag + " to " + observer.Profile.Gamertag);
                }
                catch
                {
                    this.moduleContext.Log("Sending friend request from Active profile failed.");
                }

                try
                {
                    this.InactiveProfile.SignIn(UserIndex.One);
                    this.moduleContext.Log("Sending friend request from " + this.InactiveProfile.Gamertag + " to " + observer.ProfileName);
                    this.InactiveProfile.Friends.SendFriendRequest(observer.Profile);
                    observer.InactiveFriendRequestSent = true;
                    this.moduleContext.Log("Sent friend request from " + this.InactiveProfile.Gamertag + " to " + observer.Profile.Gamertag);
                }
                catch
                {
                    this.moduleContext.Log("Sending friend request from Inactive profile failed.");
                }
            }

            this.IsSetupGood(); // check for friend and sign-in status

            // O3 sign in observers, accept friend invitations and navigate to rich presence observation
            //    no threads
            this.ChangeSetupProgress("Signing in and Friending observer consoles", 2);
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                try
                {
                    // sign in a profile
                    Thread.Sleep(2000);
                    observer.Profile.SignIn(UserIndex.Zero);
                    Thread.Sleep(500);
                    if (observer.Profile.GetUserSigninState() != SignInState.NotSignedIn)
                    {
                        observer.SignedIn = "Signed-In";
                    }

                    // friend default console's active and inactive profiles
                    try
                    {
                        observer.Profile.Friends.AcceptFriendRequest(this.ActiveProfile);
                    }
                    catch
                    {
                    }

                    try
                    {
                        observer.Profile.Friends.AcceptFriendRequest(this.InactiveProfile);
                    }
                    catch
                    {
                    }
                }
                catch (Exception e)
                {
                    this.moduleContext.Log("Unable to use profile " + observer.ProfileName + " due to exception:\n\n" + e.Message);
                    this.passedOrFailed = "FAILED";
                }
            }

            // O2 set language for each observer console and cold boot
            this.ChangeSetupProgress("Changing Observer Languages and Rebooting", 2);
            List<Thread> bootThreads = new List<Thread>();
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                Thread th = new Thread(new ParameterizedThreadStart(delegate
                {
                    // set language
                    observer.Device.SetLanguage(observer.ObservedLanguage.Language, false);
                    observer.Device.SetVideoMode(VideoResolution.Mode480, VideoStandard.NTSCM, false);
                    observer.Device.Reboot(true);
                    Thread.Sleep(2000);
                    observer.Profile.SignIn(UserIndex.Zero);
                    Thread.Sleep(500);
                }));
                bootThreads.Add(th);
                th.Name = "Set Language";
                th.Start();
            }

            // C3 install title on default xbox, if necessary
            this.ChangeSetupProgress("Setting up friend relationships and checking if " + this.moduleContext.XboxTitle.Name + " is installed to " + this.DrivingConsole.Name, 1);
            Thread thinstall = new Thread(new ParameterizedThreadStart(delegate
                {
                    this.InstallTitleToDrivingConsole();
                }));
            bootThreads.Add(thinstall);
            thinstall.Name = "Install Title";
            thinstall.Start();

            // WAIT for all reboots
            foreach (Thread th in bootThreads)
            {
                th.Join();
            }

            // O4 navigate observers to observer friends screen
            this.ChangeSetupProgress("Navigating to friend observation screens", 1);
            List<Thread> openFriendsThreads = new List<Thread>();
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                Thread th = new Thread(new ParameterizedThreadStart(delegate
                {
                    // navigate to friends screen
                    observer.Device.OpenFriendPresenceScreen(observer.Profile);
                }));

                th.Name = "Open Friends Screen";
                th.Start();
                openFriendsThreads.Add(th);
            }

            this.ChangeSetupProgress("Verifying Setup", 1);

            // log setup status
            this.moduleContext.Log("SETUP\tConsole Name\t Console IP\tLanguage\tSigned In\tFriended");
            foreach (ObserverConsole observer in this.ObserverConsoles)
            {
                this.moduleContext.Log("Observer\t" + observer.ConsoleName + "\t" + observer.ConsoleIP + "\t" + observer.ObservedLanguage.LanguageName
                    + "\t" + observer.SignedIn + "\t" + observer.Friended);
            }

            // wait for observer navigation and title install to finish. that's the last of the threads
            // WAIT for all navigation
            foreach (Thread th in openFriendsThreads)
            {
                th.Join();
            }
            
            thinstall.Join();

            // allocate space for one image per observer
            this.throwawayImages = new List<string>();

            // if there were any failures, retry
            if (!this.IsSetupGood())
            {
                this.RetrySetup();
            }

            if (this.IsSetupGood())
            {
                this.ChangeSetupProgress("Done", 0);
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
                    this.ActiveProfile.Friends.SendFriendRequest(observer.Profile);
                    observer.ActiveFriendRequestSent = true;
                    this.InactiveProfile.Friends.SendFriendRequest(observer.Profile);
                    observer.InactiveFriendRequestSent = true;
                    if (observer.SignedIn == "*")
                    {
                        observer.Profile.SignIn(UserIndex.Zero);
                    }

                    if (observer.Friended == "*")
                    {
                        observer.Profile.Friends.AcceptFriendRequest(this.ActiveProfile);
                        observer.Profile.Friends.AcceptFriendRequest(this.InactiveProfile);
                    }
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
            bool result = false;
            int activeFriends = 0;
            int inactiveFriends = 0;
            int friendsOnConsole;

            // check for friend relationships
            try
            {
                foreach (ObserverConsole observer in this.ObserverConsoles)
                {
                    friendsOnConsole = 0;
                    foreach (Friend friend in observer.Profile.Friends.EnumerateFriends())
                    {
                        if (friend.Gamertag == this.ActiveProfileName)
                        {
                            activeFriends++;
                            friendsOnConsole++;
                        }

                        if (friend.Gamertag == this.InactiveProfileName)
                        {
                            inactiveFriends++;
                            friendsOnConsole++;
                        }

                        if (friendsOnConsole == 2)
                        {
                            observer.Friended = "Friended";
                        }
                    }
                }

                if (activeFriends == this.ObserverConsoles.Count() &&
                    inactiveFriends == this.ObserverConsoles.Count())
                {
                    result = true;
                }
            }
            catch
            {
                result = false;
            }

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
                }
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        /// Install the title to the driving console
        /// </summary>
        private void InstallTitleToDrivingConsole()
        {
            if (!this.DrivingConsole.IsTitleInstalled)
            {
                foreach (string drive in this.DrivingConsole.Drives)
                {
                    if (drive.Contains("HDD"))
                    {
                        this.DrivingConsole.InstallTitle(drive);
                        this.moduleContext.Log("Installed " + this.moduleContext.XboxTitle.Name + " to " + this.DrivingConsole.Name + " on hard drive");
                    }
                }

                if (!this.DrivingConsole.IsTitleInstalled)
                {
                    foreach (string drive in this.DrivingConsole.Drives)
                    {
                        if (drive.Contains("USB") || drive.Contains("MU"))
                        {
                            this.DrivingConsole.InstallTitle(drive);
                            this.moduleContext.Log("Installed " + this.moduleContext.XboxTitle.Name + " to " + this.DrivingConsole.Name + " on USB or MU");
                        }
                    }
                }

                if (!this.DrivingConsole.IsTitleInstalled)
                {
                    this.moduleContext.Log("Failed to install " + this.moduleContext.XboxTitle.Name + " to " + this.DrivingConsole.Name);
                    MessageBox.Show(
                        "Unable to install " + this.moduleContext.XboxTitle.Name +
                        " to console " + this.DrivingConsole.IP + "\n\nAborting Test",
                        "Certification Assistance Tool");
                }
            }
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
            string s = string.Empty;

            // at least one selection
            if (this.moduleContext.SelectedDevices.Count() == 0)
            {
                MessageBox.Show("No consoles are selected.  Select one.", catString);
                return null;
            }
            else if (this.moduleContext.SelectedDevices.Count() > 1)
            {
                MessageBox.Show("Multiple consoles are selected.  Select only one.", catString);
                return null;
            }

            // make sure any selected Xboxes are connected
            foreach (IXboxDevice device in this.moduleContext.SelectedDevices)
            {
                // connected
                if (!device.Connected)
                {
                    s += "The selected device " + device.Name + " is not connected. Connect the device.";
                }
            }

            // if there were any error messages, fail
            if (!string.IsNullOrEmpty(s))
            {
                MessageBox.Show(s, catString);
                return null;
            }

            // if only one console is selected, use it
            if (this.moduleContext.SelectedDevices.Count() == 1)
            {
                return (IXboxDevice)this.moduleContext.SelectedDevices.First();
            }

            // if the default is selected, use it
            foreach (IXboxDevice device in this.moduleContext.SelectedDevices)
            {
                // connected
                if (device.IsDefault)
                {
                    return device;
                }
            }

            // if multiple consoles are selected but none is the default, pick one then verify
            IXboxDevice mainDevice = (IXboxDevice)this.moduleContext.SelectedDevices.First();
            s = "Multiple consoles are selected.\n\nUse " + mainDevice.Name + " as the controlling console?";
            if (MessageBoxResult.Yes == MessageBox.Show(s, catString, MessageBoxButton.YesNo))
            {
                return mainDevice;
            }

            return null;
        }

        /// <summary>
        /// Create an active and an inactive profile
        /// </summary>
        /// <param name="device">Xbox to create the profile on</param>
        private void CreateActiveAndInactiveProfiles(IXboxDevice device)
        {
            // Use existing profiles if there are at least 2 of them
            this.ActiveProfile = this.GetAProfile(device, "first");
            this.InactiveProfile = this.GetAProfile(device, "last");

            if (this.ActiveProfile != null && this.InactiveProfile != null && (this.ActiveProfile != this.InactiveProfile))
            {
                return; // worked fine
            }

            // If there was a problem, create one or two profiles, and retry if creation fails once
            int retries = 0;
            if (this.InactiveProfile == this.ActiveProfile)
            {
                this.InactiveProfile = null; // create a new profile for the inactive one
            }
            
            try
            {
                ConsoleProfilesManager profilesManager = device.XboxConsole.CreateConsoleProfilesManager();

                while (this.ActiveProfile == null && retries < 2)
                {
                    retries++;
                    try
                    {
                        this.ActiveProfile = profilesManager.CreateConsoleProfile(true);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("There was a problem creating a profile on " + device.Name +
                            "The exception message is: " + e.Message);
                    }
                }

                while (this.InactiveProfile == null && retries < 2)
                {
                    retries++;
                    try
                    {
                        this.InactiveProfile = profilesManager.CreateConsoleProfile(true);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("There was a problem creating a profile on " + device.Name +
                            "The exception message is: " + e.Message);
                    }
                }
            }
            catch (Exception)
            {
            }

            if (this.ActiveProfile == null || this.InactiveProfile == null)
            {
                string s = "Unable to find or create any profiles on " + this.DrivingConsole.Name + "\n\nSetup cannot continue.";
                this.passedOrFailed = "FAILED";
                MessageBox.Show(s, "Certification Assistance Tool");
                throw new Exception(s);
            }
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

                if (which == "new")
                {
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
                        profile = profilesManager.CreateConsoleProfile(true);
                    }
                }

                // make this the default profile so it will log in at bootup
                profilesManager.SetDefaultProfile(profile);
            }
            catch (Exception e)
            {
                this.passedOrFailed = "FAILED";
                MessageBox.Show(
                    "There was an error getting a profile from console " + device.IP + "\n\n" + e.Message,
                    "Certification Assistance Tool");
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
        /// Start the HTML log
        /// </summary>
        private void StartHtmlLog()
        {
            File.AppendAllText(Path.Combine(this.moduleContext.LogDirectory, "captures.html"), "<HTML>\r\n<TITLE>Captures</TITLE>\r\n<HEAD><meta http-equiv='Content-Type' content='text/html; charset=utf-8'></HEAD><BODY>\r\n<TABLE>\r\n");
        }

        /// <summary>
        /// Adds a row of HTML log output to the HTML log
        /// </summary>
        /// <param name="logLineData">The row of HTML log output to log to the HTML log</param>
        private void AddToHtmlLog(LogLineData logLineData)
        {
            if (!this.htmlLogStarted)
            {
                this.htmlLogStarted = true;
                this.StartHtmlLog();
            }

            string htmlRow = "<TR>\r\n<TD>" + logLineData.LocalizedString + "</TD>\r\n";
            if (logLineData.CaptureData.ContainsKey(this.DefaultLanguage))
            {
                string imageFile = logLineData.CaptureData[this.DefaultLanguage].ImagePath;
                htmlRow += "<TD>\r\n    <A HREF='" + imageFile + "'><IMG SRC='" + imageFile + "' width='320' height='240' ></A><BR>\r\n    " + logLineData.CaptureData[this.DefaultLanguage].LanguageName + ": " + logLineData.CaptureData[this.DefaultLanguage].TranslatedString + "\r\n<BR><BR></TD>\r\n";
            }

            foreach (KeyValuePair<string, CaptureData> pair in logLineData.CaptureData)
            {
                if (pair.Key != this.DefaultLanguage)
                {
                    string imageFile = pair.Value.ImagePath;
                    htmlRow += "<TD>\r\n    <A HREF='" + imageFile + "'><IMG SRC='" + imageFile + "' width='320' height='240' ></A><BR>\r\n    " + pair.Value.LanguageName + ": " + pair.Value.TranslatedString + "\r\n<BR><BR></TD>\r\n";
                }
            }
            
            htmlRow += "</TR>\r\n";
            File.AppendAllText(Path.Combine(this.moduleContext.LogDirectory, "captures.html"), htmlRow, Encoding.UTF8);
        }

        /// <summary>
        /// Finish the HTML log
        /// </summary>
        private void FinishHtmlLog()
        {
            if (this.htmlLogStarted)
            {
                File.AppendAllText(Path.Combine(this.moduleContext.LogDirectory, "captures.html"), "</TABLE>\r\n</BODY>\r\n</HTML>");
                this.htmlLogStarted = false;
            }
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
        /// Parses the game configuration file, pulls out all Presence related strings
        /// </summary>
        /// <param name="xbd">Xbox with a XboxTitle associated with it, from which the currently configured game configuration file is used.</param>
        private void LoadPresenceStrings(IXboxDevice xbd)
        {
            this.SupportedLanguages = new ObservableCollection<string>();
            this.LocalizedStrings = new SortedDictionary<uint, LocalizedString>();
            this.StringContexts = new SortedDictionary<uint, StringContext>();
            this.StringProperties = new SortedDictionary<uint, StringProperty>();
            PresenceMode.AllModes = new SortedDictionary<uint, PresenceMode>();
            ExpandedString.AllStrings = new List<ExpandedString>();

            if (xbd != null)
            {
                IXboxTitle xboxTitle = this.moduleContext.XboxTitle;
                if (xboxTitle != null)
                {
                    string gameConfigPath = xboxTitle.GameConfigPath;
                    if (!string.IsNullOrEmpty(gameConfigPath))
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(gameConfigPath);
                        XmlElement root = xmlDoc.DocumentElement;
                        foreach (XmlNode node in root.ChildNodes)
                        {
                            // Look at root level for "GameConfigProject"
                            if (node.Name == "GameConfigProject")
                            {
                                foreach (XmlNode node2 in node.ChildNodes)
                                {
                                    // Look at root level for "Presence"
                                    if (node2.Name == "Presence")
                                    {
                                        foreach (XmlNode node3 in node2.ChildNodes)
                                        {
                                            if (node3.Name == "PresenceMode")
                                            {
                                                PresenceMode presenceMode = new PresenceMode();
                                                foreach (XmlAttribute attr in node3.Attributes)
                                                {
                                                    if (attr.Name == "contextValue")
                                                    {
                                                        presenceMode.ContextValue = this.ParseDecimalOrHexInt(attr.Value);
                                                    }
                                                    else if (attr.Name == "stringId")
                                                    {
                                                        presenceMode.StringId = this.ParseDecimalOrHexInt(attr.Value);
                                                    }
                                                    else if (attr.Name == "friendlyName")
                                                    {
                                                        presenceMode.FriendlyName = attr.Value;
                                                    }
                                                }

                                                this.PresenceModes.Add(presenceMode.ContextValue, presenceMode);
                                            }
                                        }
                                    }
                                    else if (node2.Name == "LocalizedStrings")
                                    {
                                        foreach (XmlAttribute attr in node2.Attributes)
                                        {
                                            if (attr.Name == "defaultLocale")
                                            {
                                                this.DefaultLanguage = attr.Value;
                                            }
                                        }

                                        foreach (XmlNode node4 in node2.ChildNodes)
                                        {
                                            if (node4.Name == "SupportedLocale")
                                            {
                                                foreach (XmlAttribute attr in node4.Attributes)
                                                {
                                                    if (attr.Name == "locale")
                                                    {
                                                        this.SupportedLanguages.Add(attr.Value);
                                                    }
                                                }
                                            }
                                            else if (node4.Name == "LocalizedString")
                                            {
                                                LocalizedString localizedString = new LocalizedString();
                                                foreach (XmlAttribute attr in node4.Attributes)
                                                {
                                                    if (attr.Name == "id")
                                                    {
                                                        localizedString.Id = this.ParseDecimalOrHexInt(attr.Value);
                                                    }
                                                    else if (attr.Name == "friendlyName")
                                                    {
                                                        localizedString.FriendlyName = attr.Value;
                                                    }
                                                }

                                                foreach (XmlNode node5 in node4.ChildNodes)
                                                {
                                                    if (node5.Name == "Translation")
                                                    {
                                                        TranslatedString translation = new TranslatedString();
                                                        translation.LocalizedString = localizedString;
                                                        foreach (XmlAttribute attr in node5.Attributes)
                                                        {
                                                            if (attr.Name == "locale")
                                                            {
                                                                translation.Language = attr.Value;
                                                            }
                                                        }

                                                        translation.RawValue = node5.InnerText;
                                                        translation.Value = node5.InnerText;

                                                        localizedString.TranslatedStrings.Add(translation.Language, translation);
                                                    }
                                                }

                                                this.LocalizedStrings.Add(localizedString.Id, localizedString);
                                            }
                                        }
                                    }
                                    else if (node2.Name == "Contexts")
                                    {
                                        foreach (XmlNode node6 in node2.ChildNodes)
                                        {
                                            if (node6.Name == "Context")
                                            {
                                                StringContext stringContext = new StringContext();
                                                foreach (XmlAttribute attr in node6.Attributes)
                                                {
                                                    if (attr.Name == "stringId")
                                                    {
                                                        stringContext.StringId = this.ParseDecimalOrHexInt(attr.Value);
                                                    }
                                                    else if (attr.Name == "id")
                                                    {
                                                        stringContext.Id = this.ParseDecimalOrHexInt(attr.Value);
                                                    }
                                                    else if (attr.Name == "defaultValue")
                                                    {
                                                        stringContext.DefaultValue = this.ParseDecimalOrHexInt(attr.Value);
                                                    }
                                                    else if (attr.Name == "friendlyName")
                                                    {
                                                        stringContext.FriendlyName = attr.Value;
                                                    }
                                                }

                                                foreach (XmlNode node7 in node6.ChildNodes)
                                                {
                                                    if (node7.Name == "ContextValue")
                                                    {
                                                        StringContextValue stringContextValue = new StringContextValue();
                                                        stringContextValue.StringContext = stringContext;
                                                        foreach (XmlAttribute attr in node7.Attributes)
                                                        {
                                                            if (attr.Name == "stringId")
                                                            {
                                                                stringContextValue.StringId = this.ParseDecimalOrHexInt(attr.Value);
                                                            }
                                                            else if (attr.Name == "value")
                                                            {
                                                                stringContextValue.Value = this.ParseDecimalOrHexInt(attr.Value);
                                                            }
                                                            else if (attr.Name == "friendlyName")
                                                            {
                                                                stringContextValue.FriendlyName = attr.Value;
                                                            }
                                                        }

                                                        stringContext.ContextStrings.Add(stringContextValue.Value, stringContextValue);
                                                    }
                                                }

                                                this.StringContexts.Add(stringContext.Id, stringContext);
                                            }
                                        }
                                    }
                                    else if (node2.Name == "Properties")
                                    {
                                        foreach (XmlNode node6 in node2.ChildNodes)
                                        {
                                            if (node6.Name == "Property")
                                            {
                                                StringProperty stringProperty = new StringProperty();
                                                foreach (XmlAttribute attr in node6.Attributes)
                                                {
                                                    if (attr.Name == "id")
                                                    {
                                                        stringProperty.Id = this.ParseDecimalOrHexInt(attr.Value);
                                                    }
                                                    else if (attr.Name == "stringId")
                                                    {
                                                        stringProperty.StringId = this.ParseDecimalOrHexInt(attr.Value);
                                                    }
                                                    else if (attr.Name == "friendlyName")
                                                    {
                                                        stringProperty.FriendlyName = attr.Value;
                                                    }
                                                }

                                                this.StringProperties.Add(stringProperty.Id, stringProperty);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Now that everything is read...
                        // Link up presenceMode's with their associated strings by stringId
                        foreach (KeyValuePair<uint, PresenceMode> pair3 in this.PresenceModes)
                        {
                            PresenceMode presenceMode = pair3.Value;
                            presenceMode.LocalizedString = this.LocalizedStrings[presenceMode.StringId];
                        }

                        // Link up all contexts with their strings
                        foreach (KeyValuePair<uint, StringContext> pair4 in this.StringContexts)
                        {
                            foreach (KeyValuePair<uint, StringContextValue> pair5 in pair4.Value.ContextStrings)
                            {
                                StringContextValue stringContextValue = pair5.Value;
                                stringContextValue.LocalizedString = this.LocalizedStrings[stringContextValue.StringId];
                            }
                        }

                        // Link up all properties with their strings
                        foreach (KeyValuePair<uint, StringProperty> pair6 in this.StringProperties)
                        {
                            StringProperty stringProperty = pair6.Value;
                            stringProperty.LocalizedString = this.LocalizedStrings[stringProperty.StringId];
                        }

                        // Find all contexts and properties that a each string is associated with, based on its contents
                        foreach (KeyValuePair<uint, LocalizedString> pair in this.LocalizedStrings)
                        {
                            LocalizedString localizedString = pair.Value;
                            foreach (KeyValuePair<string, TranslatedString> pair2 in localizedString.TranslatedStrings)
                            {
                                TranslatedString translatedString = pair2.Value;
                                string newString = string.Empty;
                                string origString = translatedString.RawValue;
                                int offset = 0;
                                Regex regex = new Regex("{(.*?)}");
                                MatchCollection matches = regex.Matches(origString);
                                if (matches.Count != 0)
                                {
                                    // Make sure we traverse these in order of index
                                    SortedDictionary<int, Match> sortedMatches = new SortedDictionary<int, Match>();

                                    foreach (Match match in matches)
                                    {
                                        sortedMatches.Add(match.Index, match);
                                    }

                                    foreach (KeyValuePair<int, Match> pair3 in sortedMatches)
                                    {
                                        Match match = pair3.Value;
                                        int matchIndex = match.Index - offset;
                                        int matchEnd = matchIndex + match.Length;
                                        newString += origString.Substring(0, matchIndex);
                                        origString = origString.Substring(matchIndex + match.Length);
                                        offset += matchIndex + match.Length;
                                        
                                        string matchString = match.ToString();
                                        if (matchString.Length > 3)
                                        {
                                            char stringTypeChar = matchString[1];
                                            string objIdString = matchString.Substring(2, matchString.Length - 3);
                                            uint objId = this.ParseDecimalOrHexInt(objIdString);
                                            if ((stringTypeChar == 'c') || (stringTypeChar == 'C'))
                                            {
                                                // Context
                                                StringContext ctx = this.StringContexts[objId];
                                                newString += "{Context:" + ctx.FriendlyName + "}";
                                                translatedString.Contexts.Add(ctx.Id, ctx);
                                            }
                                            else if ((stringTypeChar == 'p') || (stringTypeChar == 'P'))
                                            { 
                                                // Property
                                                StringProperty prop = this.StringProperties[objId];
                                                newString += "{Property:" + prop.FriendlyName + "}";
                                                translatedString.Properties.Add(prop.Id, prop);
                                            }
                                            else
                                            {
                                                // Unknown variable type in the localized string
                                            }
                                        }
                                    }

                                    translatedString.Value = newString;
                                }
                            }
                        }

                        // Expand all strings.
                        List<LocalizedString> pendingLocalizedStrings = new List<LocalizedString>();
                        
                        // In order to handle the dependencies, maintain a list and put
                        // things at the end of the list if they have dependencies
                        foreach (KeyValuePair<uint, LocalizedString> pair in this.LocalizedStrings)
                        {
                            pendingLocalizedStrings.Add(pair.Value);
                        }

                        // Expand each localizedString
                        int numRequeued = 0;
                        while (pendingLocalizedStrings.Count != 0)
                        {
                            LocalizedString localizedString = pendingLocalizedStrings[0];
                            pendingLocalizedStrings.RemoveAt(0);

                            bool foundAnyUnresolved = false;

                            // Only process it now if all contexts are already expanded
                            foreach (KeyValuePair<string, TranslatedString> pair in localizedString.TranslatedStrings)
                            {
                                TranslatedString translatedString = pair.Value;
                                foreach (KeyValuePair<uint, StringContext> pair2 in translatedString.Contexts)
                                {
                                    StringContext stringContext = pair2.Value;
                                    foreach (KeyValuePair<uint, StringContextValue> pair3 in stringContext.ContextStrings)
                                    {
                                        StringContextValue stringContextValue = pair3.Value;
                                        if (!stringContextValue.LocalizedString.ExpansionDone)
                                        {
                                            foundAnyUnresolved = true;
                                            break;
                                        }
                                    }

                                    if (foundAnyUnresolved)
                                    {
                                        break;
                                    }
                                }

                                if (foundAnyUnresolved)
                                {
                                    break;
                                }
                            }

                            if (foundAnyUnresolved)
                            {
                                numRequeued++;
                                pendingLocalizedStrings.Add(localizedString);
                                if (numRequeued >= pendingLocalizedStrings.Count)
                                {
                                    // Circular references?  Report an error
                                    break;
                                }
                            }
                            else
                            {
                                // Verify that all translatedStrings have the same contexts.  Otherwise, our matching will not work.
                                TranslatedString firstTranslatedString = null;
                                foreach (KeyValuePair<string, TranslatedString> pair in localizedString.TranslatedStrings)
                                {
                                    TranslatedString translatedString = pair.Value;
                                    if (firstTranslatedString == null)
                                    {
                                        firstTranslatedString = translatedString;
                                    }
                                    else
                                    {
                                        bool sameContents = true;
                                        if (firstTranslatedString.Contexts.Count != translatedString.Contexts.Count)
                                        {
                                            sameContents = false;
                                        }
                                        else
                                        {
                                            foreach (KeyValuePair<uint, StringContext> pair2 in firstTranslatedString.Contexts)
                                            {
                                                if (!translatedString.Contexts.ContainsKey(pair2.Key))
                                                {
                                                    sameContents = false;
                                                    break;
                                                }
                                            }

                                            if (!sameContents) 
                                            {
                                                // Mismatch!
                                                throw new Exception();
                                            }
                                        }
                                    }
                                }

                                foreach (KeyValuePair<string, TranslatedString> pair in localizedString.TranslatedStrings)
                                {
                                    TranslatedString translatedString = pair.Value;
                                    List<string> stringsSoFar = new List<string>();
                                    stringsSoFar.Add(translatedString.RawValue);
                                    foreach (KeyValuePair<uint, StringContext> pair2 in translatedString.Contexts)
                                    {
                                        StringContext stringContext = pair2.Value;
                                        List<KeyValuePair<uint, StringContextValue>> contextValueList = stringContext.ContextStrings.ToList();
                                        
                                        List<string> newStrings = new List<string>();
                                        for (int j = 0; j < stringsSoFar.Count; j++)
                                        {
                                            // Scan origString specifically for this string.  Parse out each context, but only use the one that matches.
                                            Regex regex = new Regex("{(.*?)}");
                                            Match match = regex.Match(stringsSoFar[j]);
                                            while (null != match && match.Success)
                                            {
                                                string matchString = match.ToString();
                                                if (matchString.Length > 2)
                                                {
                                                    char varType = matchString[1];
                                                    string contextIdString = matchString.Substring(2, matchString.Length - 3);
                                                    uint contextId = this.ParseDecimalOrHexInt(contextIdString);
                                                    if (contextId == stringContext.Id)
                                                    {
                                                        for (int i = 1; i < contextValueList.Count; i++)
                                                        {
                                                            newStrings.Add(stringsSoFar[j].Replace(matchString, contextValueList[i].Value.LocalizedString.TranslatedStrings[translatedString.Language].Value));
                                                        }
                                                        
                                                        stringsSoFar[j] = stringsSoFar[j].Replace(matchString, contextValueList[0].Value.LocalizedString.TranslatedStrings[translatedString.Language].Value);
                                                    }

                                                    match = match.NextMatch();
                                                }
                                            }
                                        }

                                        stringsSoFar.AddRange(newStrings);
                                    }

                                    foreach (string s in stringsSoFar)
                                    {
                                        ExpandedString expandedString = new ExpandedString(s, translatedString);
                                        ExpandedString.AllStrings.Add(expandedString);
                                        translatedString.ExpandedStrings.Add(expandedString);
                                    }
                                }

                                numRequeued = 0;
                                localizedString.ExpansionDone = true;
                            }
                        }
                    }
                }
            }

            foreach (KeyValuePair<uint, PresenceMode> pair in this.PresenceModes)
            {
                PresenceMode presenceMode = pair.Value;
                LocalizedString localizedString = pair.Value.LocalizedString;
                foreach (KeyValuePair<string, TranslatedString> pair2 in localizedString.TranslatedStrings)
                {
                    TranslatedString translatedString = pair2.Value;
                }
            }

            this.NotifyPropertyChanged("PresenceModes");
        }

        /// <summary>
        /// Sets the a value indicating the visibility of each supported language in the UI
        /// </summary>
        private void SetLanguageVisibilities()
        {
            this.IsEnglishSupported = false;
            this.IsFrenchSupported = false;
            this.IsKoreanSupported = false;
            this.IsRussianSupported = false;
            this.IsChineseSimplifiedSupported = false;
            this.IsPortugueseSupported = false;
            this.IsItalianSupported = false;
            this.IsGermanSupported = false;
            this.IsSpanishSupported = false;
            this.IsJapaneseSupported = false;
            this.IsPolishSupported = false;
            this.IsChineseTraditionalSupported = false;
            this.IsDutchSupported = false;
            this.IsNorwegianSupported = false;
            this.IsSwedishSupported = false;
            this.IsTurkishSupported = false;

            foreach (string s in this.SupportedLanguages)
            {
                switch (s)
                {
                    case "en-US":
                        this.IsEnglishSupported = true;
                        break;
                    case "fr-FR":
                        this.IsFrenchSupported = true;
                        break;
                    case "ko-KR":
                        this.IsKoreanSupported = true;
                        break;
                    case "ru-RU":
                        this.IsRussianSupported = true;
                        break;
                    case "zh-CN":
                        this.IsChineseSimplifiedSupported = true;
                        break;
                    case "pt-PT":
                        this.IsPortugueseSupported = true;
                        break;
                    case "it-IT":
                        this.IsItalianSupported = true;
                        break;
                    case "de-DE":
                        this.IsGermanSupported = true;
                        break;
                    case "es-ES":
                        this.IsSpanishSupported = true;
                        break;
                    case "ja-JP":
                        this.IsJapaneseSupported = true;
                        break;
                    case "pl-PL":
                        this.IsPolishSupported = true;
                        break;
                    case "zh-CHT":
                        this.IsChineseTraditionalSupported = true;
                        break;
                    case "da-DK":
                        this.IsDutchSupported = true;
                        break;
                    case "nb-NO":
                        this.IsNorwegianSupported = true;
                        break;
                    case "sv-SE":
                        this.IsSwedishSupported = true;
                        break;
                    case "tr-TR":
                        this.IsTurkishSupported = true;
                        break;
                    default:
                        break;
                }
            }
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
            /// Backing field for IsObserved property
            /// </summary>
            private bool isObserved;

            /// <summary>
            /// Initializes a new instance of the <see cref="ObservedLanguage" /> class.
            /// </summary>
            /// <param name="languageName">Language name to associate with this object</param>
            /// <param name="languageID">Language ID to associate with this object</param>
            /// <param name="language">Language enumeration value to associate with this object</param>
            /// <param name="refreshCommand">A command to execute when Refresh is selected on this language's screen snapshot</param>
            /// <param name="retryNavigationCommand">A command to execute when Retry Navigation is selected on this language's screen snapshot</param>
            public ObservedLanguage(string languageName, string languageID, Language language, Command refreshCommand, Command retryNavigationCommand)
            {
                this.Language = language;
                this.LanguageID = languageID;
                this.LanguageName = languageName;
                this.RefreshCommand = refreshCommand;
                this.RetryNavigationCommand = retryNavigationCommand;
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
            /// Gets or sets a value indicating whether or not this language is currently being observed
            /// </summary>
            public bool IsObserved
            {
                get
                {
                    return this.isObserved;
                }

                set
                {
                    this.isObserved = value;
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
            public Command RetryNavigationCommand { get; set; }

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
            /// Backing field for Friended property
            /// </summary>
            private string friended;

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
                this.Friended = "*";
                this.ActiveFriendRequestSent = false;
                this.InactiveFriendRequestSent = false;
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
            /// Gets or sets a value indicating whether or not this console has initiated a friend request for the active profile
            /// </summary>
            public bool ActiveFriendRequestSent { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not this console has initiated a friend request for the inactive profile
            /// </summary>
            public bool InactiveFriendRequestSent { get; set; }

            /// <summary>
            /// Gets or sets a string to display as the Friended status of the profile on this console
            /// </summary>
            public string Friended
            {
                get
                {
                    return this.friended;
                }

                set
                {
                    this.friended = value;
                    this.NotifyPropertyChanged();
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

        /// <summary>
        /// Presence mode
        /// </summary>
        public class PresenceMode : INotifyPropertyChanged
        {
            /// <summary>
            /// Backing field for IsChecked property
            /// </summary>
            private bool isChecked;

            /// <summary>
            /// Backing field for DisplayBrush property
            /// </summary>
            private Brush displayBrush = Brushes.Black;

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets of a list of all presence modes
            /// </summary>
            public static SortedDictionary<uint, PresenceMode> AllModes { get; set; }

            /// <summary>
            /// Gets or sets a context value associated with this presence mode
            /// </summary>
            public uint ContextValue { get; set; }

            /// <summary>
            /// Gets or sets the string ID associated with this presence mode
            /// </summary>
            public uint StringId { get; set; }

            /// <summary>
            /// Gets or sets the friendly-name associated with this presence mode
            /// </summary>
            public string FriendlyName { get; set; }

            /// <summary>
            /// Gets or sets the localized string associated with this presence mode
            /// </summary>
            public LocalizedString LocalizedString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not this presence mode is checked
            /// </summary>
            public bool IsChecked
            {
                get
                { 
                    return this.isChecked;
                }
                
                set
                {
                    if (value == true)
                    {
                        // Uncheck all (other) presenceModes AND Expanded strings.
                        foreach (KeyValuePair<uint, PresenceMode> pair in AllModes)
                        {
                            pair.Value.IsChecked = false;
                        }

                        foreach (ExpandedString expandedString in ExpandedString.AllStrings)
                        {
                            expandedString.IsChecked = false;
                        }
                    }

                    this.isChecked = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets the brush to use to display this presence mode
            /// </summary>
            public Brush DisplayBrush
            {
                get
                {
                    return this.displayBrush;
                }

                set
                {
                    this.displayBrush = value;
                    this.NotifyPropertyChanged();
                }
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

        /// <summary>
        /// Localized string
        /// </summary>
        public class LocalizedString
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LocalizedString" /> class.
            /// </summary>
            public LocalizedString()
            {
                this.TranslatedStrings = new SortedDictionary<string, TranslatedString>();
            }

            /// <summary>
            /// Gets or sets the ID of this localized string
            /// </summary>
            public uint Id { get; set; }

            /// <summary>
            /// Gets or sets the friendly-name of this localized string
            /// </summary>
            public string FriendlyName { get; set; }

            /// <summary>
            /// Gets or sets translated strings associated with this localized string
            /// </summary>
            public SortedDictionary<string, TranslatedString> TranslatedStrings { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether expansion has been completed for this localized string
            /// </summary>
            public bool ExpansionDone { get; set; }
        }

        /// <summary>
        /// Expanded string
        /// </summary>
        public class ExpandedString : INotifyPropertyChanged
        {
            /// <summary>
            /// Backing field for IsChecked property
            /// </summary>
            private bool isChecked;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExpandedString" /> class.
            /// </summary>
            /// <param name="value">The value to set this expanded string to</param>
            /// <param name="translatedString">The translated string to associate with this expanded string</param>
            public ExpandedString(string value, TranslatedString translatedString)
            {
                this.DisplayBrush = Brushes.Black;
                this.TranslatedString = translatedString;
                this.Value = value;
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets a list of all expanded strings
            /// </summary>
            public static List<ExpandedString> AllStrings { get; set; }

            /// <summary>
            /// Gets or sets the translated string associated with this expanded string
            /// </summary>
            public TranslatedString TranslatedString { get; set; }

            /// <summary>
            /// Gets or sets the value of this expanded string
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not this expanded string is checked
            /// </summary>
            public bool IsChecked
            {
                get 
                {
                    return this.isChecked; 
                }

                set
                {
                    if (value)
                    {
                        // Uncheck all (other) presenceModes AND Expanded strings.
                        foreach (KeyValuePair<uint, PresenceMode> pair in PresenceMode.AllModes)
                        {
                            pair.Value.IsChecked = false;
                        }

                        foreach (ExpandedString expandedString in ExpandedString.AllStrings)
                        {
                            expandedString.IsChecked = false;
                        }
                    }

                    this.isChecked = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets a brush used to display this expanded string in the UI
            /// </summary>
            public Brush DisplayBrush { get; set; }

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

        /// <summary>
        /// Translated string
        /// </summary>
        public class TranslatedString
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TranslatedString" /> class.
            /// </summary>
            public TranslatedString()
            {
                this.Contexts = new SortedDictionary<uint, StringContext>();
                this.Properties = new SortedDictionary<uint, StringProperty>();
                this.ExpandedStrings = new List<ExpandedString>();
            }

            /// <summary>
            /// Gets or sets the localized string associated with this translated string
            /// </summary>
            public LocalizedString LocalizedString { get; set; }

            /// <summary>
            /// Gets or sets the language associated with this translated string
            /// </summary>
            public string Language { get; set; }

            /// <summary>
            /// Gets or sets the value of this translated string
            /// </summary>
            public string Value { get; set; } 

            /// <summary>
            /// Gets or sets the raw value of this translated string
            /// Might include “{}” variables that would get replaced with Context values
            /// </summary>
            public string RawValue { get; set; }

            /// <summary>
            /// Gets or sets a sorted dictionary of contexts associated with this translated string
            /// All contexts found in the value, i.e. {C1234}
            /// </summary>
            public SortedDictionary<uint, StringContext> Contexts { get; set; }

            /// <summary>
            /// Gets or sets a sorted dictionary of properties associated with this translated string 
            /// All contexts found in the value, i.e. {P1234}
            /// </summary>
            public SortedDictionary<uint, StringProperty> Properties { get; set; }

            /// <summary>
            /// Gets or sets a list of expanded strings associated with this translated string
            /// </summary>
            public List<ExpandedString> ExpandedStrings { get; set; }
        }

        /// <summary>
        /// String context
        /// </summary>
        public class StringContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StringContext" /> class.
            /// </summary>
            public StringContext()
            {
                this.ContextStrings = new SortedDictionary<uint, StringContextValue>();
            }

            /// <summary>
            /// Gets or sets the ID of this string context
            /// </summary>
            public uint Id { get; set; }

            /// <summary>
            /// Gets or sets the StringID of this string context
            /// </summary>
            public uint StringId { get; set; }

            /// <summary>
            /// Gets or sets the default value of this string context
            /// </summary>
            public uint DefaultValue { get; set; }

            /// <summary>
            /// Gets or sets a friendly-name for this string context
            /// </summary>
            public string FriendlyName { get; set; }

            /// <summary>
            /// Gets or sets a sorted dictionary of string context values associated with this string context
            /// </summary>
            public SortedDictionary<uint, StringContextValue> ContextStrings { get; set; }
        }

        /// <summary>
        /// String context value
        /// </summary>
        public class StringContextValue
        {
            /// <summary>
            /// Gets or sets the string context associated with this string context value
            /// </summary>
            public StringContext StringContext { get; set; }

            /// <summary>
            /// Gets or sets the value of this string context value
            /// </summary>
            public uint Value { get; set; }

            /// <summary>
            /// Gets or sets the string ID of this string context value
            /// </summary>
            public uint StringId { get; set; }

            /// <summary>
            /// Gets or sets a friend-name associated with this string context value
            /// </summary>
            public string FriendlyName { get; set; }

            /// <summary>
            /// Gets or sets a localized string associated with this string context value
            /// </summary>
            public LocalizedString LocalizedString { get; set; }
        }

        /// <summary>
        /// String property
        /// </summary>
        public class StringProperty
        {
            /// <summary>
            /// Gets or sets the ID of this string property
            /// </summary>
            public uint Id { get; set; }

            /// <summary>
            /// Gets or sets the string ID of this string property
            /// </summary>
            public uint StringId { get; set; }
            
            /// <summary>
            /// Gets or sets the friendly-name associated with this string property
            /// </summary>
            public string FriendlyName { get; set; }

            /// <summary>
            /// Gets or sets a localized string associated with this string property
            /// </summary>
            public LocalizedString LocalizedString { get; set; }
        }

        /// <summary>
        /// A class containing information about a specific capture to log
        /// </summary>
        private class CaptureData
        {
            /// <summary>
            /// Gets or sets the name of the language for this capture
            /// </summary>
            public string LanguageName { get; set; }

            /// <summary>
            /// Gets or sets the ID of the language for this capture
            /// </summary>
            public string LanguageID { get; set; }

            /// <summary>
            /// Gets or sets the translated string for this capture
            /// </summary>
            public string TranslatedString { get; set; }

            /// <summary>
            /// Gets or sets a path to the image file for this capture
            /// </summary>
            public string ImagePath { get; set; }
        }

        /// <summary>
        /// Represents a single row of HTML log output
        /// </summary>
        private class LogLineData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LogLineData" /> class.
            /// </summary>
            public LogLineData()
            {
                this.CaptureData = new SortedDictionary<string, CaptureData>();
            }

            /// <summary>
            /// Gets or sets a localized string represented by this log line
            /// </summary>
            public string LocalizedString { get; set; }

            /// <summary>
            /// Gets or sets of sorted dictionary of capture data
            /// </summary>
            public SortedDictionary<string, CaptureData> CaptureData { get; set; }
        }
    } // End of: public class GP070CTC1 : IModule, INotifyPropertyChanged
} // End of: namespace GP070 in code file GP070CTC1.cs