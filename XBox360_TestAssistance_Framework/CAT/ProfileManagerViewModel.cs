// -----------------------------------------------------------------------
// <copyright file="ProfileManagerViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using Microsoft.Test.Xbox.Profiles;

    /// <summary>
    /// View Model class for ProgressBarWindow
    /// </summary>
    public class ProfileManagerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// A reference to the main view model
        /// </summary>
        private readonly MainViewModel mainViewModel;

        /// <summary>
        /// A reference to the XboxViewItem associated with this profile manager
        /// </summary>
        private readonly XboxViewItem xboxViewItem;

        /// <summary>
        /// Backing field for Quadrant1SignedIn property
        /// </summary>
        private bool quadrant1SignedIn;

        /// <summary>
        /// Backing field for Quadrant2SignedIn property
        /// </summary>
        private bool quadrant2SignedIn;

        /// <summary>
        /// Backing field for Quadrant3SignedIn property
        /// </summary>
        private bool quadrant3SignedIn;

        /// <summary>
        /// Backing field for Quadrant4SignedIn property
        /// </summary>
        private bool quadrant4SignedIn;

        /// <summary>
        /// Backing field for Quadrant1SignedInAndLive property
        /// </summary>
        private bool quadrant1SignedInAndLive;

        /// <summary>
        /// Backing field for Quadrant2SignedInAndLive property
        /// </summary>
        private bool quadrant2SignedInAndLive;

        /// <summary>
        /// Backing field for Quadrant3SignedInAndLive property
        /// </summary>
        private bool quadrant3SignedInAndLive;

        /// <summary>
        /// Backing field for Quadrant4SignedInAndLive property
        /// </summary>
        private bool quadrant4SignedInAndLive;

        /// <summary>
        /// Backing field for Quadrant1 property
        /// </summary>
        private ConsoleProfileViewItem quadrant1;

        /// <summary>
        /// Backing field for Quadrant2 property
        /// </summary>
        private ConsoleProfileViewItem quadrant2;

        /// <summary>
        /// Backing field for Quadrant3 property
        /// </summary>
        private ConsoleProfileViewItem quadrant3;

        /// <summary>
        /// Backing field for Quadrant4 property
        /// </summary>
        private ConsoleProfileViewItem quadrant4;

        /// <summary>
        /// Placeholder for "signed out" field in profile list
        /// </summary>
        private ConsoleProfileViewItem signedOutProfilePlaceholder;

        /// <summary>
        /// A console profiles manager
        /// </summary>
        private ConsoleProfilesManager profilesManager;

        /// <summary>
        /// Backing field for AllProfiles property
        /// </summary>
        private ObservableCollection<ConsoleProfileViewItem> allProfiles = new ObservableCollection<ConsoleProfileViewItem>();

        /// <summary>
        /// Backing field for AllFoundProfiles property
        /// </summary>
        private ObservableCollection<FoundProfileViewItem> allFoundProfiles = new ObservableCollection<FoundProfileViewItem>();

        /// <summary>
        /// Gets or sets the profile manager window
        /// </summary>
        private ProfileManagerWindow profileManagerWindow;

        /// <summary>
        /// Backing field for CreateProfileIsLive property
        /// </summary>
        private bool createProfileIsLive = true;

        /// <summary>
        /// Backing field for CreateProfileIsDefault property
        /// </summary>
        private bool createProfileIsDefault;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileManagerViewModel" /> class.
        /// </summary>
        /// <param name="xboxViewItem">XboxViewItem associated with the Xbox to open the profile manager for</param>
        /// <param name="mainViewModel">A reference to the main view model</param>
        public ProfileManagerViewModel(XboxViewItem xboxViewItem, MainViewModel mainViewModel)
        {
            // Search game directory for profiles
            if (!string.IsNullOrEmpty(mainViewModel.CurrentTitle.GameDirectory))
            {
                try
                {
                    string rootPath = mainViewModel.CurrentTitle.GameDirectory;
                    DirectoryInfo dirInfo = new DirectoryInfo(rootPath);
                    
                    // Make a list of all files with 16 character names
                    FileInfo[] files = dirInfo.GetFiles("??????????????????", SearchOption.AllDirectories);
                    foreach (FileInfo file in files)
                    {
                        string directoryName = file.DirectoryName.Substring(rootPath.Length + 1);
                        string fileName = file.Name;
                        if (directoryName.EndsWith(fileName + @"\FFFE07D1\00010000"))
                        {
                            FoundProfileViewItem foundProfileViewItem = new FoundProfileViewItem(Path.Combine(file.DirectoryName, fileName));
                            this.AllFoundProfiles.Add(foundProfileViewItem);
                        }
                    }
                }
                catch
                {
                    // On failure, just don't display any profiles
                }
            }

            // Initialize member data
            this.xboxViewItem = xboxViewItem;
            this.mainViewModel = mainViewModel;
            this.Title = "Profile Manager - " + xboxViewItem.Name;
 
            this.AllCountries = new List<XboxLiveCountry>();
            foreach (XboxLiveCountry country in Enum.GetValues(typeof(XboxLiveCountry)))
            {
                this.AllCountries.Add(country);
            }

            this.AllSubscriptionTiers = new List<SubscriptionTier>();
            foreach (SubscriptionTier tier in Enum.GetValues(typeof(SubscriptionTier)))
            {
                this.AllSubscriptionTiers.Add(tier);
            }

            this.CreateProfileTier = SubscriptionTier.Gold;
            this.CreateProfileCountry = XboxLiveCountry.UnitedStates;

            this.SignInAllCommand = new Command((o) => this.SignInAll());
            this.SignOutAllCommand = new Command((o) => this.SignOutAll());
            this.DeleteProfile1Command = new Command((o) => this.DeleteProfile1());
            this.DeleteProfile2Command = new Command((o) => this.DeleteProfile2());
            this.DeleteProfile3Command = new Command((o) => this.DeleteProfile3());
            this.DeleteProfile4Command = new Command((o) => this.DeleteProfile4());
            this.DeleteAllProfilesCommand = new Command((o) => this.DeleteAllProfiles());
            this.CreateProfileCommand = new Command((o) => this.CreateProfile());
            this.BrowseForProfileCommand = new Command((o) => this.BrowseForProfile());
            this.AddSelectedProfileCommand = new Command((o) => this.AddSelectedProfile());
            this.CopyProfile1Command = new Command((o) => this.CopyProfile(this.Quadrant1.OfflineXuid));
            this.CopyProfile2Command = new Command((o) => this.CopyProfile(this.Quadrant2.OfflineXuid));
            this.CopyProfile3Command = new Command((o) => this.CopyProfile(this.Quadrant3.OfflineXuid));
            this.CopyProfile4Command = new Command((o) => this.CopyProfile(this.Quadrant4.OfflineXuid));

            this.profilesManager = this.xboxViewItem.XboxDevice.CreateConsoleProfilesManager();

            // Add 'signed out' profile placeholder
            this.signedOutProfilePlaceholder = new ConsoleProfileViewItem(null, this);
            this.allProfiles.Add(this.signedOutProfilePlaceholder);
            this.Quadrant1 = this.signedOutProfilePlaceholder;
            this.Quadrant2 = this.signedOutProfilePlaceholder;
            this.Quadrant3 = this.signedOutProfilePlaceholder;
            this.Quadrant4 = this.signedOutProfilePlaceholder;

            // Get profiles
            IEnumerable<ConsoleProfile> profiles = this.profilesManager.EnumerateConsoleProfiles();
            foreach (ConsoleProfile profile in profiles)
            {
                ConsoleProfileViewItem consoleProfileViewItem = new ConsoleProfileViewItem(profile, this);
                this.allProfiles.Add(consoleProfileViewItem);

                if (profile.GetUserSigninState() != SignInState.NotSignedIn)
                {
                    switch (profile.GetUserIndex())
                    {
                        case UserIndex.Zero:
                            this.quadrant1 = consoleProfileViewItem;
                            consoleProfileViewItem.SelectedInComboBoxIndex = 1;
                            this.Quadrant1SignedIn = true;
                            this.Quadrant1SignedInAndLive = profile.IsLiveProfile;
                            this.NotifyPropertyChanged("Quadrant1");
                            break;
                        case UserIndex.One:
                            this.quadrant2 = consoleProfileViewItem;
                            consoleProfileViewItem.SelectedInComboBoxIndex = 2;
                            this.Quadrant2SignedIn = true;
                            this.Quadrant2SignedInAndLive = profile.IsLiveProfile;
                            this.NotifyPropertyChanged("Quadrant2");
                            break;
                        case UserIndex.Two:
                            this.quadrant3 = consoleProfileViewItem;
                            consoleProfileViewItem.SelectedInComboBoxIndex = 3;
                            this.Quadrant3SignedIn = true;
                            this.Quadrant3SignedInAndLive = profile.IsLiveProfile;
                            this.NotifyPropertyChanged("Quadrant3");
                            break;
                        case UserIndex.Three:
                            this.quadrant4 = consoleProfileViewItem;
                            consoleProfileViewItem.SelectedInComboBoxIndex = 4;
                            this.Quadrant4SignedIn = true;
                            this.Quadrant4SignedInAndLive = profile.IsLiveProfile;
                            this.NotifyPropertyChanged("Quadrant4");
                            break;
                    }
                }
            }

            this.profileManagerWindow = new ProfileManagerWindow(this);
            this.profileManagerWindow.Closing += this.OnWindowClosing;
            this.profileManagerWindow.Show();
        }

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 1 is signed in
        /// </summary>
        public bool Quadrant1SignedIn
        {
            get
            {
                return this.quadrant1SignedIn;
            }

            set
            {
                this.quadrant1SignedIn = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 2 is signed in
        /// </summary>
        public bool Quadrant2SignedIn
        {
            get
            {
                return this.quadrant2SignedIn;
            }

            set
            {
                this.quadrant2SignedIn = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 3 is signed in
        /// </summary>
        public bool Quadrant3SignedIn
        {
            get
            {
                return this.quadrant3SignedIn;
            }

            set
            {
                this.quadrant3SignedIn = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 4 is signed in
        /// </summary>
        public bool Quadrant4SignedIn
        {
            get
            {
                return this.quadrant4SignedIn;
            }

            set
            {
                this.quadrant4SignedIn = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 1 is signed in and an Xbox live profile
        /// </summary>
        public bool Quadrant1SignedInAndLive
        {
            get
            {
                return this.quadrant1SignedInAndLive;
            }

            set
            {
                this.quadrant1SignedInAndLive = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 2 is signed in and an Xbox live profile
        /// </summary>
        public bool Quadrant2SignedInAndLive
        {
            get
            {
                return this.quadrant2SignedInAndLive;
            }

            set
            {
                this.quadrant2SignedInAndLive = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 3 is signed in and an Xbox live profile
        /// </summary>
        public bool Quadrant3SignedInAndLive
        {
            get
            {
                return this.quadrant3SignedInAndLive;
            }

            set
            {
                this.quadrant3SignedInAndLive = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 4 is signed in and an Xbox live profile
        /// </summary>
        public bool Quadrant4SignedInAndLive
        {
            get
            {
                return this.quadrant4SignedInAndLive;
            }

            set
            {
                this.quadrant4SignedInAndLive = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a new profile should be created with Xbox Live (or local)
        /// </summary>
        public bool CreateProfileIsLive
        {
            get
            {
                return this.createProfileIsLive;
            }

            set
            {
                this.createProfileIsLive = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a new profile should be created as the default
        /// </summary>
        public bool CreateProfileIsDefault
        {
            get
            {
                return this.createProfileIsDefault;
            }

            set
            {
                this.createProfileIsDefault = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the title of the window
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets a reference to the MainViewModel
        /// </summary>
        public MainViewModel MainViewModel
        {
            get { return this.mainViewModel; }
        }

        /// <summary>
        /// Gets or sets the command to sign in all
        /// </summary>
        public Command SignInAllCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to sign out all
        /// </summary>
        public Command SignOutAllCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to delete profile in quadrant 1
        /// </summary>
        public Command DeleteProfile1Command { get; set; }

        /// <summary>
        /// Gets or sets the command to delete profile in quadrant 2
        /// </summary>
        public Command DeleteProfile2Command { get; set; }

        /// <summary>
        /// Gets or sets the command to delete profile in quadrant 3
        /// </summary>
        public Command DeleteProfile3Command { get; set; }

        /// <summary>
        /// Gets or sets the command to delete profile in quadrant 4
        /// </summary>
        public Command DeleteProfile4Command { get; set; }

        /// <summary>
        /// Gets or sets the command to delete all profiles
        /// </summary>
        public Command DeleteAllProfilesCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to create a profile
        /// </summary>
        public Command CreateProfileCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to browse for a profile
        /// </summary>
        public Command BrowseForProfileCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to add the selected profile
        /// </summary>
        public Command AddSelectedProfileCommand { get; set; }

        /// <summary>
        /// Gets or sets the command to copy the profile logged into quadrant 1, to the PC
        /// </summary>
        public Command CopyProfile1Command { get; set; }

        /// <summary>
        /// Gets or sets the command to copy the profile logged into quadrant 2, to the PC
        /// </summary>
        public Command CopyProfile2Command { get; set; }

        /// <summary>
        /// Gets or sets the command to copy the profile logged into quadrant 3, to the PC
        /// </summary>
        public Command CopyProfile3Command { get; set; }

        /// <summary>
        /// Gets or sets the command to copy the profile logged into quadrant 4, to the PC
        /// </summary>
        public Command CopyProfile4Command { get; set; }

        /// <summary>
        /// Gets or sets a list of all Xbox live countries
        /// </summary>
        public List<XboxLiveCountry> AllCountries { get; set; }

        /// <summary>
        /// Gets or sets a list of all subscription tiers
        /// </summary>
        public List<SubscriptionTier> AllSubscriptionTiers { get; set; }

        /// <summary>
        /// Gets or sets the subscription tear to use when creating a profile
        /// </summary>
        public SubscriptionTier CreateProfileTier { get; set; }

        /// <summary>
        /// Gets or sets the country to use when creating a profile
        /// </summary>
        public XboxLiveCountry CreateProfileCountry { get; set; }

        /// <summary>
        /// Gets or sets a list of all profiles
        /// </summary>
        public ObservableCollection<ConsoleProfileViewItem> AllProfiles
        {
            get
            {
                return this.allProfiles;
            }

            set
            {
                this.allProfiles = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a list of all profiles found under the game directory
        /// </summary>
        public ObservableCollection<FoundProfileViewItem> AllFoundProfiles
        {
            get
            {
                return this.allFoundProfiles;
            }

            set
            {
                this.allFoundProfiles = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a reference to the current Theme
        /// </summary>
        public Theme CurrentTheme
        {
            get { return MainViewModel.CurrentTheme; }
        }

        /// <summary>
        /// Gets or sets ConsoleProfileViewItem associated with quadrant 1
        /// </summary>
        public ConsoleProfileViewItem Quadrant1
        {
            get
            {
                return this.quadrant1;
            }

            set
            {
                if (this.quadrant1 != value)
                {
                    if ((this.quadrant1 != null) && (this.quadrant1 != this.signedOutProfilePlaceholder))
                    {
                        this.quadrant1.SelectedInComboBoxIndex = 0;
                        try
                        {
                            this.RetryLoop(delegate { this.quadrant1.Profile.SignOut(); });
                        }
                        catch
                        {
                            // Ignore.  Assume a failed signed out means it's already signed out
                        }
                    }

                    this.Quadrant1SignedIn = false;
                    this.Quadrant1SignedInAndLive = false;
                    this.quadrant1 = this.signedOutProfilePlaceholder;
                    if ((value != null) && (value != this.signedOutProfilePlaceholder))
                    {
                        try
                        {
                            this.RetryLoop(delegate { value.Profile.SignIn(UserIndex.Zero); });
                            this.quadrant1 = value;
                            value.SelectedInComboBoxIndex = 1;
                            this.Quadrant1SignedIn = true;
                            this.Quadrant1SignedInAndLive = value.Profile.IsLiveProfile;
                        }
                        catch
                        {
                            MessageBox.Show("Unable to sign in " + value.Profile.Gamertag, "Certification Assistance Tool");
                        }
                    }
                    
                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets ConsoleProfileViewItem associated with quadrant 2
        /// </summary>
        public ConsoleProfileViewItem Quadrant2
        {
            get
            {
                return this.quadrant2;
            }

            set
            {
                if (this.quadrant2 != value)
                {
                    if ((this.quadrant2 != null) && (this.quadrant2 != this.signedOutProfilePlaceholder))
                    {
                        this.quadrant2.SelectedInComboBoxIndex = 0;
                        try
                        {
                            this.RetryLoop(delegate { this.quadrant2.Profile.SignOut(); });
                        }
                        catch
                        {
                            // Ignore.  Assume a failed signed out means it's already signed out
                        }
                    }

                    this.Quadrant2SignedIn = false;
                    this.Quadrant2SignedInAndLive = false;
                    this.quadrant2 = this.signedOutProfilePlaceholder;
                    if ((value != null) && (value != this.signedOutProfilePlaceholder))
                    {
                        try
                        {
                            this.RetryLoop(delegate { value.Profile.SignIn(UserIndex.One); });
                            this.quadrant2 = value;
                            value.SelectedInComboBoxIndex = 2;
                            this.Quadrant2SignedIn = true;
                            this.Quadrant2SignedInAndLive = value.Profile.IsLiveProfile;
                        }
                        catch
                        {
                            MessageBox.Show("Unable to sign in " + value.Profile.Gamertag, "Certification Assistance Tool");
                        }
                    }

                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets ConsoleProfileViewItem associated with quadrant 3
        /// </summary>
        public ConsoleProfileViewItem Quadrant3
        {
            get
            {
                return this.quadrant3;
            }

            set
            {
                if (this.quadrant3 != value)
                {
                    if ((this.quadrant3 != null) && (this.quadrant3 != this.signedOutProfilePlaceholder))
                    {
                        this.quadrant3.SelectedInComboBoxIndex = 0;
                        try
                        {
                            this.RetryLoop(delegate { this.quadrant3.Profile.SignOut(); });
                        }
                        catch
                        {
                            // Ignore.  Assume a failed signed out means it's already signed out
                        }
                    }

                    this.Quadrant3SignedIn = false;
                    this.Quadrant3SignedInAndLive = false;
                    this.quadrant3 = this.signedOutProfilePlaceholder;
                    if ((value != null) && (value != this.signedOutProfilePlaceholder))
                    {
                        try
                        {
                            this.RetryLoop(delegate { value.Profile.SignIn(UserIndex.Two); });
                            this.quadrant3 = value;
                            value.SelectedInComboBoxIndex = 3;
                            this.Quadrant3SignedIn = true;
                            this.Quadrant3SignedInAndLive = value.Profile.IsLiveProfile;
                        }
                        catch
                        {
                            MessageBox.Show("Unable to sign in " + value.Profile.Gamertag, "Certification Assistance Tool");
                        }
                    }

                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets ConsoleProfileViewItem associated with quadrant 4
        /// </summary>
        public ConsoleProfileViewItem Quadrant4
        {
            get
            {
                return this.quadrant4;
            }

            set
            {
                if (this.quadrant4 != value)
                {
                    if ((this.quadrant4 != null) && (this.quadrant4 != this.signedOutProfilePlaceholder))
                    {
                        this.quadrant4.SelectedInComboBoxIndex = 0;
                        try
                        {
                            this.RetryLoop(delegate { this.quadrant4.Profile.SignOut(); });
                        }
                        catch
                        {
                            // Ignore.  Assume a failed signed out means it's already signed out
                        }
                    }

                    this.Quadrant4SignedIn = false;
                    this.Quadrant4SignedInAndLive = false;
                    this.quadrant4 = this.signedOutProfilePlaceholder;
                    if ((value != null) && (value != this.signedOutProfilePlaceholder))
                    {
                        try
                        {
                            this.RetryLoop(delegate { value.Profile.SignIn(UserIndex.Three); });
                            this.quadrant4 = value;
                            value.SelectedInComboBoxIndex = 4;
                            this.Quadrant4SignedIn = true;
                            this.Quadrant4SignedInAndLive = value.Profile.IsLiveProfile;
                        }
                        catch
                        {
                            MessageBox.Show("Unable to sign in " + value.Profile.Gamertag, "Certification Assistance Tool");
                        }
                    }

                    this.NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the gamer tag to use when creating a new profile
        /// </summary>
        public string CreateProfileGamerTag { get; set; }

        /// <summary>
        /// An event handler called when the window closes.
        /// </summary>
        /// <param name="sender">Originator of the event</param>
        /// <param name="e">An instance of CancelEventArgs</param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            this.xboxViewItem.ProfileManagerViewModel = null;
        }

        /// <summary>
        /// Close the profile manager
        /// </summary>
        public void Close()
        {
            this.profileManagerWindow.Close();
        }

        /// <summary>
        /// Show the profile manager, if hidden
        /// </summary>
        public void Show()
        {
            this.profileManagerWindow.Show();
        }

        /// <summary>
        /// Hide the profile manager
        /// </summary>
        public void Hide()
        {
            this.profileManagerWindow.Hide();
        }
        
        /// <summary>
        /// Activate the profile manager
        /// </summary>
        public void Activate()
        {
            this.profileManagerWindow.Activate();
        }

        /// <summary>
        /// Create a profile
        /// </summary>
        private void CreateProfile()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            ConsoleProfile savedFirstProfile = null;
            if (this.Quadrant1 != null)
            {
                savedFirstProfile = this.Quadrant1.Profile;
            }

            ConsoleProfile profile = null;
            try
            {
                if (string.IsNullOrEmpty(this.CreateProfileGamerTag))
                {
                    this.RetryLoop(delegate { profile = this.profilesManager.CreateConsoleProfile(this.CreateProfileIsLive, this.CreateProfileCountry, this.CreateProfileTier); });
                }
                else
                {
                    this.RetryLoop(delegate { profile = this.profilesManager.CreateConsoleProfile(this.CreateProfileIsLive, this.CreateProfileCountry, this.CreateProfileTier, this.CreateProfileGamerTag); });
                }

                this.CreateProfileGamerTag = string.Empty;
            }
            catch
            {
                Mouse.OverrideCursor = null; 
                MessageBox.Show("Unable to create profile", "Certification Assistance Tool", MessageBoxButton.OK);
                return;
            }
            finally
            {
                if (savedFirstProfile != null)
                {
                    try
                    {
                        this.RetryLoop(delegate { savedFirstProfile.SignIn(UserIndex.Zero); });
                    }
                    catch
                    {
                        // Ignore sign in failure
                    }
                }
            }

            if (this.CreateProfileIsDefault)
            {
                try
                {
                    this.RetryLoop(delegate { this.profilesManager.SetDefaultProfile(profile); });
                }
                catch
                {
                    // Ignore.  Not sure why this would fail
                }

                foreach (ConsoleProfileViewItem consoleProfileViewItem in this.AllProfiles)
                {
                    consoleProfileViewItem.NotifyPropertyChanged("IsDefault");
                }
            }

            if (profile != null)
            {
                this.AllProfiles.Add(new ConsoleProfileViewItem(profile, this));
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Delete profile signed in to quadrant 1
        /// </summary>
        private void DeleteProfile1()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if ((this.quadrant1 != null) && (this.quadrant1 != this.signedOutProfilePlaceholder))
            {
                ConsoleProfileViewItem consoleProfileViewItem = this.quadrant1;
                this.Quadrant1 = this.signedOutProfilePlaceholder;
                this.AllProfiles.Remove(consoleProfileViewItem);
                try
                {
                    this.RetryLoop(delegate { this.profilesManager.DeleteConsoleProfile(consoleProfileViewItem.Profile); }, 2);
                }
                catch
                {
                    // Assume we can't delete it because it's already gone
                }
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Delete a profile signed in to quadrant 2
        /// </summary>
        private void DeleteProfile2()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if ((this.quadrant2 != null) && (this.quadrant2 != this.signedOutProfilePlaceholder))
            {
                ConsoleProfileViewItem consoleProfileViewItem = this.quadrant2;
                this.Quadrant2 = this.signedOutProfilePlaceholder;
                this.AllProfiles.Remove(consoleProfileViewItem);
                try
                {
                    this.RetryLoop(delegate { this.profilesManager.DeleteConsoleProfile(consoleProfileViewItem.Profile); }, 2);
                }
                catch
                {
                    // Assume we can't delete it because it's already gone
                }
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Delete a profile signed in to quadrant 3
        /// </summary>
        private void DeleteProfile3()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if ((this.quadrant3 != null) && (this.quadrant3 != this.signedOutProfilePlaceholder))
            {
                ConsoleProfileViewItem consoleProfileViewItem = this.quadrant3;
                this.Quadrant3 = this.signedOutProfilePlaceholder;
                this.AllProfiles.Remove(consoleProfileViewItem);
                try
                {
                    this.RetryLoop(delegate { this.profilesManager.DeleteConsoleProfile(consoleProfileViewItem.Profile); }, 2);
                }
                catch
                {
                    // Assume we can't delete it because it's already gone
                }
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Delete a profile signed in to quadrant 4
        /// </summary>
        private void DeleteProfile4()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if ((this.quadrant4 != null) && (this.quadrant4 != this.signedOutProfilePlaceholder))
            {
                ConsoleProfileViewItem consoleProfileViewItem = this.quadrant4;
                this.Quadrant4 = this.signedOutProfilePlaceholder;
                this.AllProfiles.Remove(consoleProfileViewItem);
                try
                {
                    this.RetryLoop(delegate { this.profilesManager.DeleteConsoleProfile(consoleProfileViewItem.Profile); }, 2);
                }
                catch
                {
                    // Assume we can't delete it because it's already gone
                }
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Delete all profiles
        /// </summary>
        private void DeleteAllProfiles()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.Quadrant1 = this.signedOutProfilePlaceholder;
            this.Quadrant2 = this.signedOutProfilePlaceholder;
            this.Quadrant3 = this.signedOutProfilePlaceholder;
            this.Quadrant4 = this.signedOutProfilePlaceholder;

            List<ConsoleProfileViewItem> itemsToRemove = new List<ConsoleProfileViewItem>();
            foreach (ConsoleProfileViewItem consoleProfileViewItem in this.allProfiles)
            {
                if (consoleProfileViewItem.Profile != null)
                {
                    itemsToRemove.Add(consoleProfileViewItem);
                }
            }

            foreach (ConsoleProfileViewItem consoleProfileViewItem in itemsToRemove)
            {
                this.allProfiles.Remove(consoleProfileViewItem);
            }

            try
            {
                this.RetryLoop(delegate { this.profilesManager.DeleteAllConsoleProfiles(); }, 2);
            }
            catch
            {
                // assume they are all gone
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Sign out all profiles
        /// </summary>
        private void SignOutAll()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            this.Quadrant1 = this.signedOutProfilePlaceholder;
            this.Quadrant2 = this.signedOutProfilePlaceholder;
            this.Quadrant3 = this.signedOutProfilePlaceholder;
            this.Quadrant4 = this.signedOutProfilePlaceholder;
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Sign in all profiles
        /// </summary>
        private void SignInAll()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            if ((this.Quadrant1 == null) || (this.Quadrant1 == this.signedOutProfilePlaceholder))
            {
                foreach (ConsoleProfileViewItem consoleProfileViewItem in this.allProfiles)
                {
                    if ((consoleProfileViewItem != this.signedOutProfilePlaceholder) && (consoleProfileViewItem.SelectedInComboBoxIndex == 0))
                    {
                        this.Quadrant1 = consoleProfileViewItem;
                        break;
                    }
                }
            }

            if ((this.Quadrant2 == null) || (this.Quadrant2 == this.signedOutProfilePlaceholder))
            {
                foreach (ConsoleProfileViewItem consoleProfileViewItem in this.allProfiles)
                {
                    if ((consoleProfileViewItem != this.signedOutProfilePlaceholder) && (consoleProfileViewItem.SelectedInComboBoxIndex == 0))
                    {
                        this.Quadrant2 = consoleProfileViewItem;
                        break;
                    }
                }
            }

            if ((this.Quadrant3 == null) || (this.Quadrant3 == this.signedOutProfilePlaceholder))
            {
                foreach (ConsoleProfileViewItem consoleProfileViewItem in this.allProfiles)
                {
                    if ((consoleProfileViewItem != this.signedOutProfilePlaceholder) && (consoleProfileViewItem.SelectedInComboBoxIndex == 0))
                    {
                        this.Quadrant3 = consoleProfileViewItem;
                        break;
                    }
                }
            }

            if ((this.Quadrant4 == null) || (this.Quadrant4 == this.signedOutProfilePlaceholder))
            {
                foreach (ConsoleProfileViewItem consoleProfileViewItem in this.allProfiles)
                {
                    if ((consoleProfileViewItem != this.signedOutProfilePlaceholder) && (consoleProfileViewItem.SelectedInComboBoxIndex == 0))
                    {
                        this.Quadrant4 = consoleProfileViewItem;
                        break;
                    }
                }
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Copy a profile from the specified drive on the Xbox to the PC
        /// </summary>
        /// <param name="xuid">The Xbox Live ID of the profile to copy</param>
        /// <param name="drive">Drive on Xbox to copy from</param>
        /// <param name="localPath">Local path to copy the file to</param>
        /// <returns>True if successful, or unrecoverable error, false if profile not found on this drive</returns>
        private bool CopyProfile(string xuid, string drive, string localPath)
        {
            bool result = false;
            try
            {
                this.xboxViewItem.XboxDevice.XboxConsole.ReceiveFile(localPath, drive + @":\Content\" + xuid + @"\FFFE07D1\00010000\" + xuid);
                result = true;
            }
            catch (Exception ex)
            {
                if (((uint)ex.HResult != (uint)XboxDebugManagerNative.HResult.XBDM_NOSUCHFILE) || ((uint)ex.HResult == (uint)XboxDebugManagerNative.HResult.XBDM_CANNOTACCESS))
                {
                    Mouse.OverrideCursor = null;
                    MessageBox.Show("Unable to copy profile to PC", "Certification Assistance Tool");
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Copy a profile from the Xbox to the PC
        /// </summary>
        /// <param name="xuid">The Xbox Live ID of the profile to copy</param>
        private void CopyProfile(string xuid)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = xuid;
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                // Save document
                Mouse.OverrideCursor = Cursors.Wait;

                // Not sure what drive it's on, try all of them
                bool done = this.CopyProfile(xuid, "HDD", dlg.FileName);
                if (!done)
                {
                    done = this.CopyProfile(xuid, "MUINT", dlg.FileName);
                }

                if (!done)
                {
                    done = this.CopyProfile(xuid, "INTUSB", dlg.FileName);
                }

                if (!done)
                {
                    done = this.CopyProfile(xuid, "MU0", dlg.FileName);
                }

                if (!done)
                {
                    done = this.CopyProfile(xuid, "MU1", dlg.FileName);
                }

                if (!done)
                {
                    done = this.CopyProfile(xuid, "USBMASS0MU", dlg.FileName);
                }

                if (!done)
                {
                    done = this.CopyProfile(xuid, "USBMASS1MU", dlg.FileName);
                }

                Mouse.OverrideCursor = null;

                if (!done)
                {
                    MessageBox.Show("Unable to find specified profile.", "Certification Assistance Tool");
                }
            }
        }

        /// <summary>
        /// Opens a file browser to select a profile
        /// </summary>
        private void BrowseForProfile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            bool result = (bool)dlg.ShowDialog();
            if (result)
            {
                string fileName = Path.GetFileName(dlg.FileName);
                bool goodFileName = fileName.Length == 16;
                if (goodFileName)
                {
                    string allowableCharacters = "0123456789ABCDEF";
                    foreach (char c in fileName)
                    {
                        if (!allowableCharacters.Contains(c.ToString()))
                        {
                            goodFileName = false;
                            break;
                        }
                    }
                }

                if (goodFileName)
                {
                    FoundProfileViewItem foundProfileViewItem = new FoundProfileViewItem(dlg.FileName);
                    this.AllFoundProfiles.Add(foundProfileViewItem);
                }
                else
                {
                    MessageBox.Show("The selected file does not appear to be a valid Xbox Profile", "Certification Assistance Tool");
                }
            }
        }

        /// <summary>
        /// Adds the selected profile to the Xbox
        /// </summary>
        private void AddSelectedProfile()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            foreach (FoundProfileViewItem foundProfileViewItem in this.AllFoundProfiles)
            {
                if (foundProfileViewItem.IsSelected)
                {
                    foundProfileViewItem.IsSelected = false;

                    string xuid = foundProfileViewItem.OfflineXuid;
                    string destination = this.xboxViewItem.XboxDevice.PrimaryDrive + @":\Content\" + xuid + @"\FFFE07D1\00010000\" + xuid;
                    if (!this.xboxViewItem.XboxDevice.SendFile(foundProfileViewItem.FilePath, destination, null))
                    {
                        Mouse.OverrideCursor = null;
                        MessageBox.Show("Failed to copy profile", "Certification Assistance Tool");
                        return;
                    }
                    
                    // Add profiles to AllProfiles
                    IEnumerable<ConsoleProfile> profiles = this.profilesManager.EnumerateConsoleProfiles();
                    foreach (ConsoleProfile profile in profiles)
                    {
                        bool alreadyPresent = false;
                        foreach (ConsoleProfileViewItem consoleProfileViewItem in this.allProfiles)
                        {
                            if (consoleProfileViewItem.Profile == profile)
                            {
                                alreadyPresent = true;
                                break;
                            }
                        }

                        if (!alreadyPresent)
                        {
                            ConsoleProfileViewItem consoleProfileViewItem = new ConsoleProfileViewItem(profile, this);
                            this.allProfiles.Add(consoleProfileViewItem);
                        }
                    }
                }
            }

            Mouse.OverrideCursor = null;
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
        /// Help function to perform a retry loop
        /// </summary>
        /// <param name="retryThis">Code to retry</param>
        /// <param name="retries">Number of retries</param>
        private void RetryLoop(Action retryThis, uint retries = 3)
        {
            for (uint retry = 0; retry < retries; retry++)
            {
                try
                {
                    retryThis();
                    return;
                }
                catch
                {
                }
            }

            retryThis();    // try one last time, allow the exception to propagate
        }

        /// <summary>
        /// View item for a Console Profile
        /// </summary>
        public class ConsoleProfileViewItem : INotifyPropertyChanged
        {
            /// <summary>
            /// Backing field for SelectedInComboBoxIndex property
            /// 1-4, or zero if not selected
            /// </summary>
            private uint selectedInComboBoxIndex;

            /// <summary>
            /// A reference to the profile manager view model
            /// </summary>
            private ProfileManagerViewModel profileManagerViewModel;

            /// <summary>
            /// Initializes a new instance of the ConsoleProfileViewItem class
            /// ConsoleProfileViewItem function
            /// </summary>
            /// <param name="profile">ConsoleProfile for which to use as ViewItem</param>
            /// <param name="profileManagerViewModel">A reference to the profile manager view model</param>
            public ConsoleProfileViewItem(ConsoleProfile profile, ProfileManagerViewModel profileManagerViewModel)
            {
                this.Profile = profile;
                this.profileManagerViewModel = profileManagerViewModel;
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets the SelectedInComboBoxIndex
            /// </summary>
            public uint SelectedInComboBoxIndex
            {
                get
                {
                    if (this.Profile == null)
                    {
                        return 0;
                    }

                    return this.selectedInComboBoxIndex;
                }

                set
                {
                    this.selectedInComboBoxIndex = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets the profile
            /// </summary>
            public ConsoleProfile Profile { get; set; }

            /// <summary>
            /// Gets the gamer tag associated with this profile
            /// </summary>
            public string Gamertag
            {
                get 
                {
                    if (this.Profile == null)
                    {
                        return "<Signed out>";
                    }
                    
                    return this.Profile.Gamertag; 
                }
            }

            /// <summary>
            /// Gets the Online Xbox user id associated with this profile
            /// </summary>
            public string OnlineXuid
            {
                get
                {
                    if (this.Profile == null)
                    {
                        return string.Empty;
                    }

                    return this.Profile.OnlineXuid.ToString().Substring(5); 
                }
            }

            /// <summary>
            /// Gets the Offline Xbox user id associated with this profile
            /// </summary>
            public string OfflineXuid
            {
                get 
                {
                    if (this.Profile == null)
                    {
                        return string.Empty;
                    }

                    return this.Profile.OfflineXuid.ToString().Substring(5); 
                }
            }

            /// <summary>
            /// Gets a value indicating whether this profile is associate with Xbox Live
            /// </summary>
            public bool IsLiveProfile
            {
                get
                {
                    if (this.Profile == null)
                    {
                        return false;
                    }

                    return this.Profile.IsLiveProfile;
                }
            }

            /// <summary>
            /// Gets the subscription tier of this profile
            /// </summary>
            public SubscriptionTier Tier
            {
                get
                {
                    if (this.Profile == null)
                    {
                        return SubscriptionTier.None;
                    }

                    return this.Profile.Tier;
                }
            }

            /// <summary>
            /// Gets the country associated with this profile
            /// </summary>
            public XboxLiveCountry Country
            {
                get
                {
                    if (this.Profile == null)
                    {
                        return XboxLiveCountry.Unknown;
                    }

                    return this.Profile.Country;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this profile is the default profile
            /// </summary>
            public bool IsDefault
            {
                get
                {
                    if (this.Profile == null)
                    {
                        return false;
                    }

                    ConsoleProfile defaultProfile = this.profileManagerViewModel.profilesManager.GetDefaultProfile();
                    if (defaultProfile == null)
                    {
                        return false;
                    }

                    return defaultProfile == this.Profile;
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
        }

        /// <summary>
        /// View item for Profiles found in game directory
        /// </summary>
        public class FoundProfileViewItem : INotifyPropertyChanged
        {
            /// <summary>
            /// Backing field for FilePath property
            /// </summary>
            private string filePath;

            /// <summary>
            /// Backing field for IsSelected property
            /// </summary>
            private bool isSelected;

            /// <summary>
            /// Initializes a new instance of the FoundProfileViewItem class
            /// ConsoleProfileViewItem function
            /// </summary>
            /// <param name="filePath">Full path to profile file</param>
            public FoundProfileViewItem(string filePath)
            {
                this.filePath = filePath;
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets the profile's offline Xbox Live Id
            /// </summary>
            public string OfflineXuid
            {
                get { return Path.GetFileName(this.filePath); }
            }

            /// <summary>
            /// Gets the path to the profile
            /// </summary>
            public string FilePath
            {
                get { return this.filePath; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether the 
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
