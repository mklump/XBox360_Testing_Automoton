// -----------------------------------------------------------------------
// <copyright file="BAS015.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BAS015
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;
    using CAT;
    using Microsoft.Test.Xbox.Profiles;
    using XDevkit;

    /// <summary>
    /// Implementation class for the BAS015 module
    /// </summary>
    public class BAS015 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// Member for working with the current working ModuleContext after Start() is called
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// Backing field for UIElement property
        /// </summary>
        private BAS015UI moduleUI;

        /// <summary>
        /// Backing field for LogStarted property
        /// </summary>
        private bool logStarted = false;

        /// <summary>
        /// Backing field for ProfilesManager property
        /// </summary>
        private ConsoleProfilesManager profilesManager;

        /// <summary>
        /// Backing field for Profile1Enabled property
        /// </summary>
        private bool profile1Enabled;

        /// <summary>
        /// Backing field for Profile2Enabled property
        /// </summary>
        private bool profile2Enabled;

        /// <summary>
        /// Backing field for Profile3Enabled property
        /// </summary>
        private bool profile3Enabled;

        /// <summary>
        /// Backing field for Profile4Enabled property
        /// </summary>
        private bool profile4Enabled;

        /// <summary>
        /// Backing field for Profile1State property
        /// </summary>
        private string profile1State;

        /// <summary>
        /// Backing field for Profile2State property
        /// </summary>
        private string profile2State;

        /// <summary>
        /// Backing field for Profile3State property
        /// </summary>
        private string profile3State;

        /// <summary>
        /// Backing field for Profile4State property
        /// </summary>
        private string profile4State;

        /// <summary>
        /// Backing field for Profile1SignedIn property
        /// </summary>
        private bool profile1SignedIn;

        /// <summary>
        /// Backing field for Profile2SignedIn property
        /// </summary>
        private bool profile2SignedIn;

        /// <summary>
        /// Backing field for Profile3SignedIn property
        /// </summary>
        private bool profile3SignedIn;

        /// <summary>
        /// Backing field for Profile4SignedIn property
        /// </summary>
        private bool profile4SignedIn;

        /// <summary>
        /// Backing field for Profile1Auto property
        /// </summary>
        private bool profile1Auto;

        /// <summary>
        /// Backing field for Profile2Auto property
        /// </summary>
        private bool profile2Auto;

        /// <summary>
        /// Backing field for Profile3Auto property
        /// </summary>
        private bool profile3Auto;

        /// <summary>
        /// Backing field for Profile4Auto property
        /// </summary>
        private bool profile4Auto;

        /// <summary>
        /// Backing field for Profile1Busy property
        /// </summary>
        private bool profile1Busy = false;

        /// <summary>
        /// Backing field for Profile2Busy property
        /// </summary>
        private bool profile2Busy = false;

        /// <summary>
        /// Backing field for Profile3Busy property
        /// </summary>
        private bool profile3Busy = false;

        /// <summary>
        /// Backing field for Profile4Busy property
        /// </summary>
        private bool profile4Busy = false;

        /// <summary>
        /// Backing field for numberOfProfilesToCreate property
        /// </summary>
        private int numberOfProfilesToCreate;

        /// <summary>
        /// Backing field for SecondsAuto property
        /// </summary>
        private int secondsAuto;

        /// <summary>
        /// Backing field for TotalSignIns property
        /// </summary>
        private int totalSignIns;

        /// <summary>
        /// Backing field for TotalSignOuts property
        /// </summary>
        private int totalSignOuts;

        /// <summary>
        /// Random member for random number generation
        /// </summary>
        private Random random;

        /// <summary>
        /// Modules are often 'wizards' with first, second and subsequent pages. module 15 has two pages
        /// </summary>
        private Visibility firstPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing private member for the SecondPageVisibility property
        /// </summary>
        private Visibility secondPageVisibility = Visibility.Visible;

        /// <summary>
        /// The current Xbox
        /// </summary>
        private IXboxDevice xboxDevice;

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the FirstPageVisibility
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
        /// Gets or sets a value indicating whether the Profile1Enabled
        /// </summary>
        public bool Profile1Enabled
        {
            get
            {
                return this.profile1Enabled;
            }

            set
            {
                this.profile1Enabled = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Profile2Enabled
        /// </summary>
        public bool Profile2Enabled
        {
            get
            {
                return this.profile2Enabled;
            }

            set
            {
                this.profile2Enabled = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Profile3Enabled
        /// </summary>
        public bool Profile3Enabled
        {
            get
            {
                return this.profile3Enabled;
            }

            set
            {
                this.profile3Enabled = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Profile4Enabled
        /// </summary>
        public bool Profile4Enabled
        {
            get
            {
                return this.profile4Enabled;
            }

            set
            {
                this.profile4Enabled = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Profile1State
        /// </summary>
        public string Profile1State
        {
            get
            {
                return this.profile1State;
            }

            set
            {
                this.profile1State = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Profile2State
        /// </summary>
        public string Profile2State
        {
            get
            {
                return this.profile2State;
            }

            set
            {
                this.profile2State = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Profile3State
        /// </summary>
        public string Profile3State
        {
            get
            {
                return this.profile3State;
            }

            set
            {
                this.profile3State = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Profile4State
        /// </summary>
        public string Profile4State
        {
            get
            {
                return this.profile4State;
            }

            set
            {
                this.profile4State = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Profile1SignedIn
        /// </summary>
        public bool Profile1SignedIn
        {
            get
            {
                return this.profile1SignedIn;
            }

            set
            {
                this.profile1SignedIn = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Profile2SignedIn
        /// </summary>
        public bool Profile2SignedIn
        {
            get
            {
                return this.profile2SignedIn;
            }

            set
            {
                this.profile2SignedIn = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Profile3SignedIn
        /// </summary>
        public bool Profile3SignedIn
        {
            get
            {
                return this.profile3SignedIn;
            }

            set
            {
                this.profile3SignedIn = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Profile4SignedIn
        /// </summary>
        public bool Profile4SignedIn
        {
            get
            {
                return this.profile4SignedIn;
            }

            set
            {
                this.profile4SignedIn = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Profile1Auto
        /// </summary>
        public bool Profile1Auto
        {
            get
            {
                return this.profile1Auto;
            }

            set
            {
                this.profile1Auto = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Profile2Auto
        /// </summary>
        public bool Profile2Auto
        {
            get
            {
                return this.profile2Auto;
            }

            set
            {
                this.profile2Auto = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Profile3Auto
        /// </summary>
        public bool Profile3Auto
        {
            get
            {
                return this.profile3Auto;
            }

            set
            {
                this.profile3Auto = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Profile4Auto
        /// </summary>
        public bool Profile4Auto
        {
            get
            {
                return this.profile4Auto;
            }

            set
            {
                this.profile4Auto = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the Number of Profiles to Create
        /// </summary>
        public int NumberOfProfilesToCreate
        {
            get
            {
                return this.numberOfProfilesToCreate;
            }

            set
            {
                this.numberOfProfilesToCreate = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the SecondsAuto
        /// </summary>
        public int SecondsAuto
        {
            get
            {
                return this.secondsAuto;
            }

            set
            {
                this.secondsAuto = value;
                this.Auto();
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the TotalSignIns
        /// </summary>
        public int TotalSignIns
        {
            get
            {
                return this.totalSignIns;
            }

            set
            {
                this.totalSignIns = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the TotalSignOuts
        /// </summary>
        public int TotalSignOuts
        {
            get
            {
                return this.totalSignOuts;
            }

            set
            {
                this.totalSignOuts = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets dispatcherTimer for interval related event triggering/raising
        /// </summary>
        public DispatcherTimer AutoMode { get; set; }

        /// <summary>
        /// Gets the user interface object for this module
        /// </summary>
        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
        }

        /// <summary>
        /// Gets or sets the LogMessage
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// Gets or sets XboxName
        /// </summary>
        public string XboxName { get; set; }

        /// <summary>
        /// Gets or sets AllProfiles
        /// </summary>
        public ObservableCollection<ConsoleProfileViewItem> AllProfiles { get; set; }

        /// <summary>
        /// Gets or sets ConsoleProfileViewItem Profile1
        /// Properties associated with profiles- object, enabled, state, auto-sign, busy-signing
        /// </summary>
        public ConsoleProfileViewItem Profile1 { get; set; }

        /// <summary>
        /// Gets or sets ConsoleProfileViewItem Profile2
        /// Properties associated with profiles- object, enabled, state, auto-sign, busy-signing
        /// </summary>
        public ConsoleProfileViewItem Profile2 { get; set; }

        /// <summary>
        /// Gets or sets ConsoleProfileViewItem Profile3
        /// Properties associated with profiles- object, enabled, state, auto-sign, busy-signing
        /// </summary>
        public ConsoleProfileViewItem Profile3 { get; set; }

        /// <summary>
        /// Gets or sets ConsoleProfileViewItem Profile4
        /// Properties associated with profiles- object, enabled, state, auto-sign, busy-signing
        /// </summary>
        public ConsoleProfileViewItem Profile4 { get; set; }

        /// <summary>
        /// Gets or sets the SecondPageVisibility
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
        /// Gets a value indicating whether there is one connected and selected Xbox
        /// </summary>
        /// <returns>True if exactly one Xbox is selected and that Xbox is connected</returns>
        private bool IsOneConnectedXboxSelected
        {
            get
            {
                string s = string.Empty;

                if (this.moduleContext.SelectedDevices.Count() == 0)
                { // At least one
                    s += "No consoles are selected. Select one. ";
                }
                else if (this.moduleContext.SelectedDevices.Count() > 1)
                { // Only one
                    s += this.moduleContext.SelectedDevices.Count().ToString() + " consoles are selected. Select just one. ";
                }

                foreach (IXboxDevice device in this.moduleContext.SelectedDevices)
                {
                    if (device.IsSelected)
                    {
                        // Connected
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
        /// Start begins the module wizard, and first shows any instructions.
        /// </summary>
        /// <param name="ctx">Current working module context</param>
        public void Start(IModuleContext ctx)
        {
            this.moduleContext = ctx as IXboxModuleContext;
            this.AutoMode = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
            this.AutoMode.Tick += new EventHandler(this.DoAuto);
            this.TotalSignIns = 0;
            this.TotalSignOuts = 0;
            this.FirstPageVisibility = Visibility.Visible;
            this.SecondPageVisibility = Visibility.Collapsed;
            this.moduleUI = new BAS015UI(this);
        }

        /// <summary>
        /// Next Step changes which page of the wizard is visible. This module has only two pages. Advance to the working page.
        /// </summary>
        public void NextStep()
        {
            if (this.IsOneConnectedXboxSelected)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // disable xbox selection, tcr selection and title changes
                this.moduleContext.IsModal = true;

                // perform profile query and set profile properties
                this.InitProfilePage();

                // advance to profiles page
                this.FirstPageVisibility = Visibility.Collapsed;
                this.SecondPageVisibility = Visibility.Visible;

                Mouse.OverrideCursor = null;
            }
        }

        /// <summary>
        /// ToggleSignInOut - toggles sign-in state of the user
        /// </summary>
        /// <param name="user">integer value 1, 2, 3 or 4</param>
        public void ToggleSignInOut(int user)
        {
            // user must be 1, 2, 3 or 4
            if (user <= 0 || user > 4)
            {
                return;
            }

            ConsoleProfileViewItem profile = null;
            switch (user)
            {
                case 1:
                    if (this.profile1Busy)
                    {
                        break;
                    }

                    profile = this.Profile1;
                    break;
                case 2:
                    if (this.profile2Busy)
                    {
                        break;
                    }

                    profile = this.Profile2;
                    break;
                case 3:
                    if (this.profile3Busy)
                    {
                        break;
                    }

                    profile = this.Profile3;
                    break;
                case 4:
                    if (this.profile4Busy)
                    {
                        break;
                    }

                    profile = this.Profile4;
                    break;
            }

            if (profile == null)
            {
                return;
            }

            // toggle sign-in state
            if (SignInState.NotSignedIn == profile.Profile.GetUserSigninState())
            {
                this.SignIn(user);
            }
            else
            {
                this.SignOut(user);
            }
        }

        /// <summary>
        /// Sign in all function
        /// </summary>
        public void SignInAll()
        {
            this.SignIn(1);
            this.SignIn(2);
            this.SignIn(3);
            this.SignIn(4);
        }

        /// <summary>
        /// Sign out all function
        /// </summary>
        public void SignOutAll()
        {
            this.SignOut(1);
            this.SignOut(2);
            this.SignOut(3);
            this.SignOut(4);
        }

        /// <summary>
        /// Auto - update the interval
        /// note: AutoMode.Enabled is bound to a user control. We don't have to stop the timer here.
        /// </summary>
        public void Auto()
        {
            this.AutoMode.Interval = new TimeSpan(0, 0, this.SecondsAuto);
        }

        /// <summary>
        /// Creates any number of profiles, and uses the number in the property Number of Profiles To Create.
        /// </summary>
        public void CreateProfiles()
        {
            if (MessageBoxResult.Yes == MessageBox.Show(
                "Create " + this.NumberOfProfilesToCreate.ToString() + " new profiles on console " +
                this.XboxName + "\n\nThis may take several minutes. \n\nCreate profiles now?",
                "Certification Assistance Tool",
                MessageBoxButton.YesNo))
            {
                // Create
                for (int i = 0; i < this.NumberOfProfilesToCreate; i++)
                {
                    try
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        ConsoleProfile profile = this.profilesManager.CreateConsoleProfile(true);
                        this.AllProfiles.Add(new ConsoleProfileViewItem(profile));
                        this.moduleContext.Log(" Created Profile " + profile.Gamertag);

                        // use the new profile
                        if (this.Profile1 == null)
                        {
                            this.Profile1 = this.AllProfiles[this.AllProfiles.Count - 1];
                            this.NotifyPropertyChanged("Profile1");
                        }
                        else if (this.Profile2 == null)
                        {
                            this.Profile2 = this.AllProfiles[this.AllProfiles.Count - 1];
                            this.NotifyPropertyChanged("Profile2");
                        }
                        else if (this.Profile3 == null)
                        {
                            this.Profile3 = this.AllProfiles[this.AllProfiles.Count - 1];
                            this.NotifyPropertyChanged("Profile3");
                        }
                        else if (this.Profile4 == null)
                        {
                            this.Profile4 = this.AllProfiles[this.AllProfiles.Count - 1];
                            this.NotifyPropertyChanged("Profile4");
                        }
                    }
                    catch (Exception e)
                    {
                        string more = string.Empty;
                        if ((uint)e.HResult == 0x80070070)
                        {
                            more = ".\n\nXbox drive is full.";
                        }

                        MessageBox.Show("There was a problem creating a profile: " + e.Message + more, "Certification Assistance Tool");
                        return;
                    }
                    finally
                    {
                        Mouse.OverrideCursor = null;
                    }
                }

                this.UpdateAllStates();
            }
        }

        /// <summary>
        /// Delete the profile currently signed in to the user quadrant
        /// quadrants referred to as 1, 2, 3, 4
        /// </summary>
        /// <param name="user">User number</param>
        public void DeleteProfile(int user)
        {
            // user must be 1, 2, 3 or 4
            if (user <= 0 || user > 4)
            {
                return;
            }

            ConsoleProfileViewItem profile = null;
            switch (user)
            {
                case 1: profile = this.Profile1;
                    break;
                case 2: profile = this.Profile2;
                    break;
                case 3: profile = this.Profile3;
                    break;
                case 4: profile = this.Profile4;
                    break;
            }

            if (profile == null)
            {
                return;
            }

            string deletedGamertag = profile.Profile.Gamertag;
            if (MessageBoxResult.Yes == MessageBox.Show(
                "Delete profile " +
                profile.Profile.ToString() + " from console "
                + this.XboxName + "?",
                "Certification Assistance Tool",
                MessageBoxButton.YesNo))
            {
                // Remove the profile from our local list
                this.AllProfiles.Remove(profile);

                // Shuffle list off and back so any bound controls will update:
                // ObservableCollection<ConsoleProfile> tempProfileList = new ObservableCollection<ConsoleProfile>();
                // foreach (ConsoleProfile p in AllProfiles)
                //    tempProfileList.Add(p);
                // tempProfileList.AddRange(AllProfiles);
                // AllProfiles = tempProfileList;
                // NotifyPropertyChanged("AllProfiles");

                // delete the profile from the console
                try
                {
                    this.profilesManager.DeleteConsoleProfile(profile.Profile);
                }
                catch (Exception exception)
                {
                    MessageBox.Show(
                        "There was a problem deleting profile " + profile.Profile + "\n\nException " + exception.Message,
                        "Certification Assistance Tool");
                }

                this.moduleContext.Log(" Deleted Profile " + deletedGamertag);

                this.UpdateAllStates();

                return;
            }
        }

        /// <summary>
        /// UpdateAllStates - update the properties associated with profiles
        /// </summary>
        public void UpdateAllStates()
        {
            if (this.Profile1 != null)
            {
                this.Profile1Enabled = true;
                this.Profile1SignedIn = this.Profile1.Profile.GetUserSigninState() != SignInState.NotSignedIn;
                this.Profile1State = this.Profile1.Profile.GetUserSigninState().ToString();
            }
            else
            {
                this.Profile1Enabled = false;
                this.Profile1SignedIn = false;
                this.Profile1State = "--no profile--";
            }

            if (this.Profile2 != null)
            {
                this.Profile2Enabled = true;
                this.Profile2SignedIn = this.Profile2.Profile.GetUserSigninState() != SignInState.NotSignedIn;
                this.Profile2State = this.Profile2.Profile.GetUserSigninState().ToString();
            }
            else
            {
                this.Profile2Enabled = false;
                this.Profile2SignedIn = false;
                this.Profile2State = "--no profile--";
            }

            if (this.Profile3 != null)
            {
                this.Profile3Enabled = true;
                this.Profile3SignedIn = this.Profile3.Profile.GetUserSigninState() != SignInState.NotSignedIn;
                this.Profile3State = this.Profile3.Profile.GetUserSigninState().ToString();
            }
            else
            {
                this.Profile3Enabled = false;
                this.Profile3SignedIn = false;
                this.Profile3State = "--no profile--";
            }

            if (this.Profile4 != null)
            {
                this.Profile4Enabled = true;
                this.Profile4SignedIn = this.Profile4.Profile.GetUserSigninState() != SignInState.NotSignedIn;
                this.Profile4State = this.Profile4.Profile.GetUserSigninState().ToString();
            }
            else
            {
                this.Profile4Enabled = false;
                this.Profile4SignedIn = false;
                this.Profile4State = "--no profile--";
            }

            this.NotifyPropertyChanged("AllProfiles");
        }

        /// <summary>
        /// Stop - called when the module is stopping
        /// </summary>
        public void Stop()
        {
            this.AutoMode.IsEnabled = false;

            if (this.logStarted)
            {
                string passedOrFailed = "DONE";

                this.moduleContext.Log("*************************************************************");
                this.moduleContext.Log("*************************************************************");
                this.moduleContext.Log("RESULT: " + passedOrFailed);
                this.moduleContext.Log("*************************************************************");
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
        /// Queries profiles for the Xbox, which must be selected, and log profiles found
        /// </summary>
        private void InitProfilePage()
        {
            // Start a log file
            this.logStarted = true;
            this.moduleContext.Log("\tMODULE 15 SIGN-IN SIGN OUT");

            // Find the Xbox name
            foreach (IXboxDevice device in this.moduleContext.SelectedDevices)
            {
                if (device.IsSelected)
                {
                    this.moduleContext.Log(" Xbox " + device.Name + " " + device.IP);
                    this.XboxName = device.IP;
                    this.xboxDevice = device as IXboxDevice;
                    break;
                }
            }

            // Find all profiles on the Xbox
            this.QueryProfileList();

            // Assign profiles on the Xbox to slots
            try
            {
                foreach (ConsoleProfileViewItem item in this.AllProfiles)
                {
                    // Give signed-in profiles the correct quadrant first
                    if (item.Profile.GetUserSigninState() != SignInState.NotSignedIn)
                    {
                        switch (item.Profile.GetUserIndex())
                        {
                            case UserIndex.Zero:
                                this.Profile1 = item;
                                this.Profile1Enabled = true;
                                this.Profile1State = this.Profile1.Profile.GetUserSigninState().ToString();
                                this.NotifyPropertyChanged("Profile1");
                                break;
                            case UserIndex.One:
                                this.Profile2 = item;
                                this.Profile2Enabled = true;
                                this.Profile2State = this.Profile2.Profile.GetUserSigninState().ToString();
                                this.NotifyPropertyChanged("Profile2");
                                break;
                            case UserIndex.Two:
                                this.Profile3 = item;
                                this.Profile3Enabled = true;
                                this.Profile3State = this.Profile3.Profile.GetUserSigninState().ToString();
                                this.NotifyPropertyChanged("Profile3");
                                break;
                            case UserIndex.Three:
                                this.Profile4 = item;
                                this.Profile4Enabled = true;
                                this.Profile4State = this.Profile4.Profile.GetUserSigninState().ToString();
                                this.NotifyPropertyChanged("Profile4");
                                break;
                        }
                    }
                }

                foreach (ConsoleProfileViewItem item in this.AllProfiles)
                {
                    // assign other profiles to the remaining slots
                    if (item.Profile.GetUserSigninState() == SignInState.NotSignedIn)
                    {
                        if (this.Profile1 == null)
                        {
                            this.Profile1 = item;
                            this.Profile1Enabled = true;
                            this.Profile1State = this.Profile1.Profile.GetUserSigninState().ToString();
                            this.NotifyPropertyChanged("Profile1");
                        }
                        else if (this.Profile2 == null)
                        {
                            this.Profile2 = item;
                            this.Profile2Enabled = true;
                            this.Profile2State = this.Profile2.Profile.GetUserSigninState().ToString();
                            this.NotifyPropertyChanged("Profile2");
                        }
                        else if (this.Profile3 == null)
                        {
                            this.Profile3 = item;
                            this.Profile3Enabled = true;
                            this.Profile3State = this.Profile3.Profile.GetUserSigninState().ToString();
                            this.NotifyPropertyChanged("Profile3");
                        }
                        else if (this.Profile4 == null)
                        {
                            this.Profile4 = item;
                            this.Profile4Enabled = true;
                            this.Profile4State = this.Profile4.Profile.GetUserSigninState().ToString();
                            this.NotifyPropertyChanged("Profile4");
                        }
                    }
                }
            }
            catch
            {
            } // Xbox went away during setup. don't try to select any more profiles automatically

            this.UpdateAllStates();

            // default the number of profiles to create so that we will end up with at least 4 profiles
            // NumberOfProfilesToCreate = (AllProfiles.Count > 3) ? 0 : 4 - AllProfiles.Count;
            // default to create just one profile at a time, per tester feedback
            this.NumberOfProfilesToCreate = 1;

            // set default seconds between auto profile sign-in
            this.SecondsAuto = 10;

            this.random = new Random();

            this.LogMessage = this.AllProfiles.Count.ToString() + " profiles initially detected:" + Environment.NewLine;
            foreach (ConsoleProfileViewItem cpvi in this.AllProfiles)
            {
                ConsoleProfile cp = cpvi.Profile;
                this.LogMessage += "\t" + cp.Gamertag + " \t\tLIVE=" + cp.IsLiveProfile.ToString() +
                    " " + cp.Country.ToString() + " " + cp.Tier.ToString() + Environment.NewLine;
            }

            this.moduleContext.Log(this.LogMessage);
        }

        /// <summary>
        /// QueryProfileList - get all profiles on the Xbox
        /// </summary>
        /// <returns>list of profiles found on the Xbox</returns>
        private ObservableCollection<ConsoleProfileViewItem> QueryProfileList()
        {
            // create an empty list
            this.AllProfiles = new ObservableCollection<ConsoleProfileViewItem>();

            // get a profile manager for the console
            try
            {
                this.profilesManager = this.xboxDevice.XboxConsole.CreateConsoleProfilesManager();
            }
            catch
            {
                // If something goes wrong, return the empty list
                return this.AllProfiles;
            }

            // Get profiles from the selected console
            IEnumerable<ConsoleProfile> profiles = this.profilesManager.EnumerateConsoleProfiles();
            if (!profiles.Any())
            {
                // If there are no profiles, return the empty list
                return this.AllProfiles;
            }

            // Store the profiles we found
            foreach (ConsoleProfile profile in profiles)
            {
                this.AllProfiles.Add(new ConsoleProfileViewItem(profile));
            }

            this.NotifyPropertyChanged("AllProfiles");

            return this.AllProfiles;
        }

        /// <summary>
        /// SignIn - signs in a profile to the console
        /// </summary>
        /// <param name="user">quadrant for sign-in</param>
        private void SignIn(int user)
        {
            // user must be 1, 2, 3 or 4
            if (user <= 0 || user > 4)
            {
                return;
            }

            switch (user)
            {
                case 1:
                    if (this.Profile1 == null)
                    {
                        break;
                    }

                    this.Profile1State = "Signing in...";
                    this.UpdateUIImmediately("Profile1State");
                    this.moduleContext.Log(" Signing in " + this.Profile1.Profile.Gamertag + " with " + this.CurrentNumberSignedIn() + " other profiles signed in");
                    this.Profile1.Profile.SignIn(UserIndex.Zero);
                    this.TotalSignIns++;
                    break;
                case 2:
                    if (this.Profile2 == null)
                    {
                        break;
                    }

                    this.Profile2State = "Signing in...";
                    this.UpdateUIImmediately("Profile2State");
                    this.moduleContext.Log(" Signing in " + this.Profile2.Profile.Gamertag + " with " + this.CurrentNumberSignedIn() + " other profiles signed in");
                    this.Profile2.Profile.SignIn(UserIndex.One);
                    this.TotalSignIns++;
                    break;
                case 3:
                    if (this.Profile3 == null)
                    {
                        break;
                    }

                    this.Profile3State = "Signing in...";
                    this.UpdateUIImmediately("Profile3State");
                    this.moduleContext.Log(" Signing in " + this.Profile3.Profile.Gamertag + " with " + this.CurrentNumberSignedIn() + " other profiles signed in");
                    this.Profile3.Profile.SignIn(UserIndex.Two);
                    this.TotalSignIns++;
                    break;
                case 4:
                    if (this.Profile4 == null)
                    {
                        break;
                    }

                    this.Profile4State = "Signing in...";
                    this.UpdateUIImmediately("Profile4State");
                    this.moduleContext.Log(" Signing in " + this.Profile4.Profile.Gamertag + " with " + this.CurrentNumberSignedIn() + " other profiles signed in");
                    this.Profile4.Profile.SignIn(UserIndex.Three);
                    this.TotalSignIns++;
                    break;
            }

            this.UpdateAllStates();
        }

        /// <summary>
        /// SignOut - signs out a profile
        /// </summary>
        /// <param name="user">quadrant for sign-out</param>
        private void SignOut(int user)
        {
            // user must be 1, 2, 3 or 4
            if (user <= 0 || user > 4)
            {
                return;
            }

            switch (user)
            {
                case 1:
                    if (this.Profile1 == null)
                    {
                        break;
                    }

                    if (this.Profile1.Profile.GetUserSigninState() == SignInState.NotSignedIn)
                    {
                        break;
                    }

                    this.profile1Busy = true;
                    this.Profile1State = "Signing out...";
                    this.UpdateUIImmediately("Profile1State");
                    this.moduleContext.Log(" Signing out " + this.Profile1.Profile.Gamertag + " when " + this.CurrentNumberSignedIn() + " profiles signed in");
                    try
                    {
                        this.Profile1.Profile.SignOut();
                        this.TotalSignOuts++;
                    }
                    catch { }
                    this.profile1Busy = false;
                    break;
                case 2:
                    if (this.Profile2 == null)
                    {
                        break;
                    }

                    if (this.Profile2.Profile.GetUserSigninState() == SignInState.NotSignedIn)
                    {
                        break;
                    }

                    this.profile2Busy = true;
                    this.Profile2State = "Signing out...";
                    this.UpdateUIImmediately("Profile2State");
                    this.Profile2Enabled = false;
                    this.moduleContext.Log(" Signing out " + this.Profile2.Profile.Gamertag + " when " + this.CurrentNumberSignedIn() + " profiles signed in");
                    try
                    {
                        this.Profile2.Profile.SignOut();
                        this.TotalSignOuts++;
                    }
                    catch { }
                    this.Profile2Enabled = true;
                    this.profile2Busy = false;
                    break;
                case 3:
                    if (this.Profile3 == null)
                    {
                        break;
                    }

                    if (this.Profile3.Profile.GetUserSigninState() == SignInState.NotSignedIn)
                    {
                        break;
                    }

                    this.profile3Busy = true;
                    this.Profile3State = "Signing out...";
                    this.UpdateUIImmediately("Profile3State");
                    this.Profile3Enabled = false;
                    this.moduleContext.Log(" Signing out " + this.Profile3.Profile.Gamertag + " when " + this.CurrentNumberSignedIn() + " profiles signed in");
                    try
                    {
                        this.Profile3.Profile.SignOut();
                        this.TotalSignOuts++;
                    }
                    catch { }
                    this.Profile3Enabled = true;
                    this.profile3Busy = false;
                    break;
                case 4:
                    if (this.Profile4 == null)
                    {
                        break;
                    }

                    if (this.Profile4.Profile.GetUserSigninState() == SignInState.NotSignedIn)
                    {
                        break;
                    }

                    this.profile4Busy = true;
                    this.Profile4State = "Signing out...";
                    this.UpdateUIImmediately("Profile4State");
                    this.Profile4Enabled = false;
                    this.moduleContext.Log(" Signing out " + this.Profile4.Profile.Gamertag + " when " + this.CurrentNumberSignedIn() + " profiles signed in");
                    try
                    {
                        this.Profile4.Profile.SignOut();
                        this.TotalSignOuts++;
                    }
                    catch { }
                    this.Profile4Enabled = true;
                    this.profile4Busy = false;
                    break;
            }

            this.UpdateAllStates();
        }

        /// <summary>
        /// DoAuto - if AutoMost is enabled, DoAuto is called every time the timer 'ticks'
        /// </summary>
        /// <param name="obj">Sending object source as another control or event</param>
        /// <param name="args">Applicable event processing args, if any</param>
        private void DoAuto(object obj, EventArgs args)
        {
            List<int> autousers = new List<int>();
            if (this.Profile1Auto)
            {
                autousers.Add(1);
            }

            if (this.Profile2Auto)
            {
                autousers.Add(2);
            }

            if (this.Profile3Auto)
            {
                autousers.Add(3);
            }

            if (this.Profile4Auto)
            {
                autousers.Add(4);
            }

            if (autousers.Count == 0)
            {
                return;
            }

            this.ToggleSignInOut(autousers[this.random.Next(0, autousers.Count)]);

            this.UpdateAllStates();
        }

        /// <summary>
        /// UpdateUIImmediately - update a property at the earliest idle
        /// </summary>
        /// <param name="property">Bond property to immediately update</param>
        private void UpdateUIImmediately(string property)
        {
            try
            {
                // try to update UI immediately
                Dispatcher dispatcher = this.moduleUI.Dispatcher;
                if (dispatcher != null)
                {
                    dispatcher.Invoke(new Action(() => { NotifyPropertyChanged(property); }), DispatcherPriority.ApplicationIdle);
                }
            }
            catch
            {
                // exceptions do not matter, the UI will update eventually regardless
            }
        }

        /// <summary>
        /// Returns the current number of profiles that are signed in
        /// </summary>
        /// <returns>Number of signed in profiles</returns>
        private int CurrentNumberSignedIn()
        {
            int i = 0;
            foreach (ConsoleProfileViewItem cpvi in this.AllProfiles)
            {
                if (cpvi.Profile.GetUserSigninState() != SignInState.NotSignedIn)
                {
                    i++;
                }
            }

            return i;
        }

        /// <summary>
        /// Class for providing a ConsoleProfileViewItem with members and functions to work with it
        /// </summary>
        public class ConsoleProfileViewItem : INotifyPropertyChanged
        {
            /// <summary>
            /// Backing field for SelectedInComboBoxIndex property
            /// 1-4, or zero if not selected
            /// </summary>
            private uint selectedInComboBoxIndex;

            /// <summary>
            /// Initializes a new instance of the ConsoleProfileViewItem class
            /// ConsoleProfileViewItem function
            /// </summary>
            /// <param name="p">ConsoleProfile for which to use as ViewItem</param>
            public ConsoleProfileViewItem(ConsoleProfile p)
            {
                this.Profile = p;
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
    }
}