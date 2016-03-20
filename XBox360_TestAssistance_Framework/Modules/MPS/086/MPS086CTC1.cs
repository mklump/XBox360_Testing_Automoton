// -----------------------------------------------------------------------
// <copyright file="MPS086CTC1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MPS086
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
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
    /// Main Class for this Module
    /// </summary>
    public class MPS086CTC1 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// The module context object passed in by CAT framework
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// The UI element associated with this module
        /// </summary>
        private MPS086CTC1UI moduleUI;

        /// <summary>
        /// A string indicating the result of the test
        /// </summary>
        private string passedOrFailed = "PASSED";

        /// <summary>
        /// First of 2 xboxes selected for this test
        /// </summary>
        private IXboxDevice xboxDevice;

        /// <summary>
        /// Second of 2 xboxes selected for this test
        /// </summary>
        private IXboxDevice xboxDevice2;

        /// <summary>
        /// First profile
        /// </summary>
        private ConsoleProfile profile1;

        /// <summary>
        /// Second profile
        /// </summary>
        private ConsoleProfile profile2;

        /// <summary>
        /// A value indicating whether the first profile is new
        /// </summary>
        private bool profile1IsNew;

        /// <summary>
        /// A value indicating whether the second profile is new
        /// </summary>
        private bool profile2IsNew;

        /// <summary>
        /// Backing field for ChildConsole property
        /// </summary>
        private IXboxDevice childConsole;

        /// <summary>
        /// Backing field for OverviewPageVisibility property
        /// </summary>
        private Visibility overviewPageVisibility = Visibility.Visible;

        /// <summary>
        /// Backing field for SetupPageVisibility property
        /// </summary>
        private Visibility setupPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for EmulationPageVisibility property
        /// </summary>
        private Visibility emulationPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for EmulationMessageVisibility property
        /// </summary>
        private Visibility emulationMessageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for TestPageVisibility property
        /// </summary>
        private Visibility testPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for AccessSetupText property
        /// </summary>
        private string accessSetupText;

        /// <summary>
        /// Backing field for InviteSetupText property
        /// </summary>
        private string inviteSetupText;

        /// <summary>
        /// Backing field for AccessExecutionText property
        /// </summary>
        private string accessExecutionText;

        /// <summary>
        /// Backing field for InviteExecutionText property
        /// </summary>
        private string inviteExecutionText;

        /// <summary>
        /// Backing field for AvailableConsoles property
        /// </summary>
        private ObservableCollection<IXboxDevice> availableConsoles;

        /// <summary>
        /// Backing field for AccessTestCanAcceptResult property
        /// </summary>
        private bool accessTestCanAcceptResult;

        /// <summary>
        /// Backing field for AccessTestNeedsResult property
        /// </summary>
        private bool accessTestNeedsResult;

        /// <summary>
        /// Backing field for InviteTestCanAcceptResult property
        /// </summary>
        private bool inviteTestCanAcceptResult;

        /// <summary>
        /// Backing field for InviteTestNeedsResult property
        /// </summary>
        private bool inviteTestNeedsResult;
        
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
        /// Gets or sets the child console
        /// </summary>
        public IXboxDevice ChildConsole
        {
            get 
            {
                return this.childConsole;
            }

            set 
            {
                this.childConsole = value; 
                this.NotifyPropertyChanged(); 
            } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether the overview page is visible
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
        /// Gets or sets a value indicating whether the setup page is visible
        /// </summary>
        public Visibility SetupPageVisibility 
        {
            get
            { 
                return this.setupPageVisibility; 
            }
            
            set 
            {
                this.setupPageVisibility = value; 
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Emulation page is visible
        /// </summary>
        public Visibility EmulationPageVisibility 
        {
            get
            { 
                return this.emulationPageVisibility; 
            }
            
            set
            { 
                this.emulationPageVisibility = value; 
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the emulation message is visible
        /// </summary>
        public Visibility EmulationMessageVisibility 
        {
            get 
            {
                return this.emulationMessageVisibility; 
            }
            
            set 
            {
                this.emulationMessageVisibility = value; 
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the test page is visible
        /// </summary>
        public Visibility TestPageVisibility 
        {
            get
            { 
                return this.testPageVisibility;
            }
            
            set
            {
                this.testPageVisibility = value; 
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets access setup text
        /// </summary>
        public string AccessSetupText 
        {
            get
            {
                return this.accessSetupText; 
            }
            
            set
            {
                this.accessSetupText = value; 
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets invite setup text
        /// </summary>
        public string InviteSetupText 
        {
            get
            {
                return this.inviteSetupText; 
            }
            
            set
            {
                this.inviteSetupText = value; 
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets access execution text
        /// </summary>
        public string AccessExecutionText 
        {
            get
            {
                return this.accessExecutionText; 
            }
            
            set
            {
                this.accessExecutionText = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets invite execution text
        /// </summary>
        public string InviteExecutionText
        {
            get
            {
                return this.inviteExecutionText; 
            }
            
            set
            {
                this.inviteExecutionText = value;
                this.NotifyPropertyChanged(); 
            }
        }
        
        /// <summary>
        /// Gets or sets a collection of available consoles
        /// </summary>
        public ObservableCollection<IXboxDevice> AvailableConsoles 
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
        /// Gets or sets a value indicating whether access test can be done
        /// </summary>
        public bool AccessTestCanAcceptResult
        {
            get
            {
                return this.accessTestCanAcceptResult;
            }

            set
            {
                this.accessTestCanAcceptResult = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether invite test can be done
        /// </summary>
        public bool InviteTestCanAcceptResult
        {
            get
            {
                return this.inviteTestCanAcceptResult;
            }

            set
            {
                this.inviteTestCanAcceptResult = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether test 1 is done
        /// </summary>
        public bool AccessTestNeedsResult 
        {
            get
            {
                return this.accessTestNeedsResult; 
            }
            
            set
            {
                this.accessTestNeedsResult = value; 
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether test 2 is done
        /// </summary>
        public bool InviteTestNeedsResult
        {
            get
            {
                return this.inviteTestNeedsResult;
            }
            
            set 
            {
                this.inviteTestNeedsResult = value; 
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets a value indicating whether the title is disc emulation based
        /// </summary>
        private bool IsEmulation
        {
            get
            {
                if (this.moduleContext.XboxTitle == null)
                {
                    return false;
                }

                return this.moduleContext.XboxTitle.GameInstallType == "Disc Emulation";
            }
        }

        /// <summary>
        /// Gets a value indicating whether the test can start
        /// </summary>
        private bool CanStartTest
        {
            get
            {
                // check a console is selected
                if (!this.AreTwoConnectedXboxesSelected)
                {
                    return false;
                }

                // check title
                if (!this.xboxDevice.IsTitleInstalled)
                {
                    if (!this.xboxDevice.CanInstallTitle)
                    {
                        MessageBox.Show(
                            "The title cannot be installed as configured.\n\nPlease open Settings to fix the problem.\n",
                            "Certification Assistance Tool");
                        return false;
                    }
                }

                if (!this.xboxDevice2.IsTitleInstalled)
                {
                    if (!this.xboxDevice2.CanInstallTitle)
                    {
                        MessageBox.Show(
                            "The title cannot be installed as configured.\n\nPlease open Settings to fix the problem.\n",
                            "Certification Assistance Tool");
                        return false;
                    }
                }

                // ok to start test
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there are two connected consoles selected
        /// </summary>
        private bool AreTwoConnectedXboxesSelected
        {
            get
            {
                string s = string.Empty;

                // exactly two
                if (this.moduleContext.SelectedDevices.Count() != 2)
                {
                    s = this.moduleContext.SelectedDevices.Count().ToString() + " consoles are selected. Select two.";
                }

                // connected
                foreach (IXboxDevice dev in this.moduleContext.SelectedDevices)
                {
                    if (dev.Connected == false)
                    {
                        s += "The selected device " + dev.Name + " is not connected. Connect the device.";
                        break;
                    }
                }

                // if there were any error messages so far, fail
                if (!string.IsNullOrEmpty(s))
                {
                    MessageBox.Show(s, "Certification Assistance Tool");
                    return false;
                }

                // use default as controlling box
                foreach (IXboxDevice dev in this.moduleContext.SelectedDevices)
                {
                    if (dev.IsDefault)
                    {
                        this.xboxDevice = dev;
                    }
                    else
                    {
                        this.xboxDevice2 = dev;
                    }
                }

                // if there is no default use the first kit in the list as the controlling box
                if (this.xboxDevice == null)
                {
                    this.xboxDevice = (IXboxDevice)this.moduleContext.SelectedDevices.First();
                    this.xboxDevice2 = (IXboxDevice)this.moduleContext.SelectedDevices.Last();
                }

                // make sure we got two boxes
                if (this.xboxDevice == null || this.xboxDevice2 == null)
                {
                    s = this.moduleContext.SelectedDevices.Count().ToString() + " consoles are selected. Select two.";
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
            this.moduleUI = new MPS086CTC1UI(this);
        }

        /// <summary>
        /// NextPage - called to leave the the module overview or intro screen entered by Start()
        ///            called subsequently to advance between pages
        /// The framework goes modal in this call and the module gains control.
        /// </summary>
        public void NextPage()
        {
            if (this.OverviewPageVisibility == Visibility.Visible)
            {
                // check prerequisites
                if (!this.CanStartTest)
                {
                    return;
                }

                this.AccessTestCanAcceptResult = false;
                this.InviteTestCanAcceptResult = false;
                this.AccessTestNeedsResult = true;
                this.InviteTestNeedsResult = true;

                this.OverviewPageVisibility = Visibility.Collapsed;
                this.moduleContext.IsModal = true;

                // go to appropriate next step
                if (this.IsEmulation)
                {
                    this.EmulationPageVisibility = Visibility.Visible;
                    this.UpdateUIImmediately();

                    // prompt user to choose the console that can emulate
                    this.InitializeAvailableConsoleList();
                }
                else
                {
                    this.SetupPageVisibility = Visibility.Visible;
                    this.UpdateUIImmediately();

                    // setup consoles
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.SetUpBothConsoles();
                    Mouse.OverrideCursor = null;
                }
            }
            else if (this.EmulationPageVisibility == Visibility.Visible)
            {
                // if we are on the intermediate page, check we have chosen a main console and move to setup page

                // check a console xbox is selected to be the main console
                if (this.ChildConsole == null)
                {
                    MessageBox.Show(
                        "Select the console that will have multiplayer disabled.\n\nSelect the console attached to your PC if possible.", 
                        "Certification Assistance Tool");
                    return;
                }

                if (this.ChildConsole == this.xboxDevice2)
                {
                    // swap consoles
                    this.xboxDevice2 = this.xboxDevice;
                    this.xboxDevice = this.ChildConsole;
                }

                this.EmulationPageVisibility = Visibility.Collapsed;
                this.SetupPageVisibility = Visibility.Visible;
                this.SetUpBothConsoles();
            }
        }

        /// <summary>
        /// Indicates that the access test has passed
        /// </summary>
        public void PassAccess()
        {
            this.AccessTestNeedsResult = false; // text
            this.AccessTestCanAcceptResult = false;  // button
            this.moduleContext.Log("Access Multiplayer Test: PASS");
        }

        /// <summary>
        /// Indicates that the access test has failed
        /// </summary>
        public void FailAccess()
        {
            this.AccessTestNeedsResult = false;
            this.AccessTestCanAcceptResult = false;  // button
            this.passedOrFailed = "FAIL";
            this.moduleContext.Log("Access Multiplayer Test: FAIL");
        }

        /// <summary>
        /// Indicates that the invite test has passed
        /// </summary>
        public void PassInvite()
        {
            this.InviteTestNeedsResult = false;
            this.InviteTestCanAcceptResult = false;
            this.moduleContext.Log("Invite No-Multiplayer Test: PASS");
        }

        /// <summary>
        /// Indicates that the invite test has failed
        /// </summary>
        public void FailInvite()
        {
            this.InviteTestNeedsResult = false;
            this.InviteTestCanAcceptResult = false;
            this.passedOrFailed = "FAIL";
            this.moduleContext.Log("Invite No-Multiplayer Test: FAIL");
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

            try
            {
                // sign-out users
                ConsoleProfilesManager profilesManager = this.xboxDevice.XboxConsole.CreateConsoleProfilesManager();
                profilesManager.SignOutAllUsers();

                // remove created profiles
                if (this.profile1IsNew)
                {
                    profilesManager.DeleteConsoleProfile(this.profile1);
                }

                if (this.profile2IsNew)
                {
                    profilesManager.DeleteConsoleProfile(this.profile2);
                }

                // stop game
                this.xboxDevice.LaunchDevDashboard();

                // restore multiplayer privilege to pre-existing profiles
                if (!this.profile1IsNew)
                {
                    if (MessageBoxResult.Yes == MessageBox.Show(
                        "Re-enable multiplayer capability for profile " + this.profile1.Gamertag + "?",
                        "Certification Assistance Tool",
                        MessageBoxButton.YesNo))
                    {
                        this.xboxDevice.RunIXBoxAutomationScript(@"Scripts\Enable_Xbox_Live_Game_Play_Privileges.xboxautomation");
                    }
                }

                this.moduleContext.Log("*************************************************************\r\n");
                this.moduleContext.Log("RESULT: " + this.passedOrFailed + "\r\n");
                this.moduleContext.Log("*************************************************************\r\n");
            }
            catch (Exception)
            {
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Set up the inviting console
        /// </summary>
        public void SetUpInvitingConsole()
        {
            // start game
            if (this.IsEmulation)
            {
                // prompt user to start emulation
                this.EmulationMessageVisibility = Visibility.Visible;
            }
            else
            {
                // install title
                if (this.xboxDevice2.IsTitleInstalled)
                {
                    this.InviteSetupText += "\nChecking " + this.moduleContext.XboxTitle.Name + " is installed on " + this.xboxDevice2.Name + ".......";
                }
                else
                {
                    this.InviteSetupText += "\nInstalling " + this.moduleContext.XboxTitle.Name + " on " + this.xboxDevice2.Name + ".......";

                    this.xboxDevice2.InstallTitle(this.GetBestAvailableDrive(this.xboxDevice2), null);
                }

                this.InviteSetupText += "Installed.";
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
        /// Set up both consoles
        /// </summary>
        private void SetUpBothConsoles()
        {
            // choose profiles
            this.AccessSetupText = "Looking for a profile on console " + this.xboxDevice.Name + ".......";
            this.InviteSetupText = "Looking for a profile on console " + this.xboxDevice2.Name + ".......";
            Thread threadProfile = new Thread(new ParameterizedThreadStart(delegate
            {
                this.profile1 = this.GetFirstProfile(this.xboxDevice, out this.profile1IsNew);
            }));

            threadProfile.Start();
            this.profile2 = this.GetFirstProfile(this.xboxDevice2, out this.profile2IsNew);
            threadProfile.Join(2 * 60 * 1000);
            this.AccessSetupText += "profile " + this.profile1.Gamertag;
            this.InviteSetupText += "profile " + this.profile2.Gamertag;
            this.AccessSetupText += this.profile1IsNew ? " Created." : " Found.";
            this.InviteSetupText += this.profile2IsNew ? " Created." : " Found.";
            this.UpdateUIImmediately();

            // sign in the profiles
            this.AccessSetupText += "\nSigning in " + this.profile1.Gamertag + ".......";
            this.InviteSetupText += "\nSigning in " + this.profile2.Gamertag + ".......";
            this.profile1.SignIn(UserIndex.Zero);
            this.profile2.SignIn(UserIndex.Zero);
            this.AccessSetupText += "Signed-in.";
            this.InviteSetupText += "Signed-in.";

            // friend the two profiles
            try
            {
                if (this.profile1IsNew || this.profile2IsNew || !this.AreFriended(this.profile1, this.profile2))
                {
                    this.InviteSetupText += "\nFriending profile " + this.profile1.Gamertag + ".......";
                    this.UpdateUIImmediately();
                    this.profile2.Friends.SendFriendRequest(this.profile1);
                    this.profile1.Friends.AcceptFriendRequest(this.profile2);
                    this.InviteSetupText += "Friended.";
                }
                else
                {
                    this.InviteSetupText += "\nProfiles were already Friended";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "There was a problem friending the profiles: " + ex.Message,
                    "Certification Assistance Tool");
            }

            // disable multiplayer for first profile, enable it for second profile
            this.xboxDevice.LaunchDevDashboard();
            this.xboxDevice2.LaunchDevDashboard();
            this.AccessSetupText += "\n\nDisabling Multiplayer for profile " + this.profile1.Gamertag + ".......";
            this.InviteSetupText += "\n\nEnabling Multiplayer for profile " + this.profile2.Gamertag + ".......";
            this.UpdateUIImmediately();
            Thread threadMultiplayer = new Thread(new ParameterizedThreadStart(delegate
            {
                try
                {
                    this.xboxDevice2.RunIXBoxAutomationScript(@"Scripts\Enable_Xbox_Live_Game_Play_Privileges.xboxautomation");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was a problem enabling multiplayer: " + ex.Message, "Certification Assistance Tool");
                }
            }));

            if (!this.profile2IsNew)
            {
                threadMultiplayer.Start();
            }

            try
            {
                this.xboxDevice.RunIXBoxAutomationScript(@"Scripts\Disable_Xbox_Live_Game_Play_Privileges.xboxautomation");
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem disabling multiplayer: " + ex.Message, "Certification Assistance Tool");
                return;
            }

            if (!this.profile2IsNew)
            {
                threadMultiplayer.Join();
            }

            this.AccessSetupText += "Disabled.";
            this.InviteSetupText += "Enabled.";

            // install title
            Thread threadInstall = new Thread(new ParameterizedThreadStart(delegate
            {
                this.SetUpInvitingConsole();
            }));

            threadInstall.Start();
            this.SetUpControllingConsole();
            threadInstall.Join(2 * 60 * 1000);

            // launch title
            this.AccessSetupText += "\nLaunching " + this.moduleContext.XboxTitle.Name + "...";
            this.InviteSetupText += "\nLaunching " + this.moduleContext.XboxTitle.Name + "...";
            this.UpdateUIImmediately();
            this.xboxDevice.LaunchTitle();
            this.xboxDevice2.LaunchTitle();

            // done
            this.AccessSetupText += "\n\nChild console is ready.";
            this.InviteSetupText += "\n\nRemote Console is ready.";
            this.AccessTestCanAcceptResult = true;
            this.InviteTestCanAcceptResult = true;
            this.UpdateUIImmediately();

            // prompt user to attempt to enter multiplayer
            this.AccessExecutionText = "Use console " + this.xboxDevice.Name + " profile " + this.profile1.Gamertag + " and attempt to enter a multiplayer session in " +
                this.moduleContext.XboxTitle.Name + ".";

            // prompt user to attempt to invite child profile to a multiplayer session
            this.InviteExecutionText += "Invite multiplayer-disabled profile " + this.profile1.Gamertag + " to a multiplayer session.";

            // log setup actions
            this.moduleContext.Log("Console " + this.xboxDevice.Name + " profile " + this.profile1.Gamertag + " has multiplayer disabled.");
            this.moduleContext.Log("Console " + this.xboxDevice2.Name + " profile " + this.profile2.Gamertag + " can invite " + this.xboxDevice.Name + " to a multiplayer session");
            this.moduleContext.Log("Title: " + this.moduleContext.XboxTitle.Name);
        }

        /// <summary>
        /// Set up controlling console
        /// </summary>
        private void SetUpControllingConsole()
        {
            if (this.IsEmulation)
            {
                this.AccessSetupText += "\nStarting " + this.moduleContext.XboxTitle.Name + " Emulation";

                // start emulation and launch game
                this.xboxDevice.InstallTitle(string.Empty);
            }
            else
            {
                // install title
                if (this.xboxDevice.IsTitleInstalled)
                {
                    this.AccessSetupText += "\nChecking " + this.moduleContext.XboxTitle.Name + " is installed on child console " + this.xboxDevice.Name + ".......";
                }
                else
                {
                    this.AccessSetupText += "\nInstalling " + this.moduleContext.XboxTitle.Name + " on child console " + this.xboxDevice.Name + ".......";

                    this.xboxDevice.InstallTitle(
                        this.GetBestAvailableDrive(this.xboxDevice),
                        this.moduleContext.OpenProgressBarWindow("Installing " + this.moduleContext.XboxTitle.Name + "..."));
                }

                this.AccessSetupText += "Installed.";
            }
        }

        /// <summary>
        /// AreFriended - checks if two profiles are friends
        /// </summary>
        /// <param name="profileA">a profile</param>
        /// <param name="profileB">another profile</param>
        /// <returns>true if profiles are friends, false otherwise</returns>
        private bool AreFriended(ConsoleProfile profileA, ConsoleProfile profileB)
        {
            bool friended = false;

            try
            {
                foreach (Friend friend in profileA.Friends.EnumerateFriends())
                {
                    if (friend.Gamertag == profileB.Gamertag)
                    {
                        friended = true;
                        break;
                    }
                }
            }
            catch
            {
                friended = false;
            }

            return friended;
        }

        /// <summary>
        /// Get the hard drive if it is enabled, otherwise use MU or USB
        /// </summary>
        /// <param name="device">The xbox to get the drive from</param>
        /// <returns>the name of the drive</returns>
        private string GetBestAvailableDrive(IXboxDevice device)
        {
            string result = string.Empty;
            foreach (string drive in device.Drives)
            {
                if (drive.Contains("HDD"))
                {
                    result = drive;
                }

                if (drive.Contains("USB") || drive.Contains("MU"))
                {
                    if (string.IsNullOrEmpty(result))
                    {
                        result = drive;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get alphabetically first existing profile or create a new one if there are none
        /// </summary>
        /// <param name="device">The xbox to get or create a profile on</param>
        /// <param name="wasCreated">Receives a value indicating a profile was created</param>
        /// <returns>Returns the found or created profile</returns>
        private ConsoleProfile GetFirstProfile(IXboxDevice device, out bool wasCreated)
        {
            ConsoleProfile firstProfile = null;
            wasCreated = false;

            try
            {
                ConsoleProfilesManager profilesManager = device.XboxConsole.CreateConsoleProfilesManager();
                IEnumerable<ConsoleProfile> profiles = profilesManager.EnumerateConsoleProfiles();

                if (profiles.Any())
                {
                    profilesManager.SignOutAllUsers();
                    firstProfile = profiles.First();
                }
                else
                {
                    firstProfile = profilesManager.CreateConsoleProfile(true);
                    wasCreated = true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "There was an error getting a profile from console " + this.xboxDevice.Name + "\n\n" + e.Message,
                    "Certification Assistance Tool");
                throw;
            }

            return firstProfile;
        }

        /// <summary>
        /// Puts the two selected consoles into a list
        /// </summary>
        private void InitializeAvailableConsoleList()
        {
            this.AvailableConsoles = new ObservableCollection<IXboxDevice>();
            foreach (IXboxDevice device in this.moduleContext.AllDevices)
            {
                if (device.IsSelected)
                {
                    this.AvailableConsoles.Add(device);
                }
            }
        }

        /// <summary>
        /// UpdateUIImmediately - update a property at the earliest idle
        /// </summary>
        private void UpdateUIImmediately()
        {
            this.moduleUI.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);
        }
    } // End of: public class MPS086CTC1 : IModule, INotifyPropertyChanged
} // End of: namespace MPS086 in code file MPS086CTC1.cs