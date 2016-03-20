// -----------------------------------------------------------------------
// <copyright file="CMTV093CTC1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CMTV093
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Threading;
    using System.Collections.ObjectModel;
    using XDevkit;
    using Microsoft.Test.Xbox.Profiles;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.IO;
    using System.Windows.Input;
    using CAT;

    public class CMTV093CTC1 : IModule, INotifyPropertyChanged
    {
        private IXboxModuleContext moduleContext;
        private CMTV093CTC1UI moduleUI;
        public event PropertyChangedEventHandler PropertyChanged;
        private string passedOrFailed = "PASSED";
        private string readyMessage;
        private IXboxDevice xboxDevice1;
        private IXboxDevice xboxDevice2;
        private IXboxDevice xboxDevice3;
        private ConsoleProfile profileA;
        private ConsoleProfile profileB;
        private ConsoleProfile profileC;
        private ConsoleProfilesManager profileManager1;
        private ConsoleProfilesManager profileManager2;
        private ConsoleProfilesManager profileManager3;

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

        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the first page should be visible 
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
        /// Gets or sets a value indicating whether or not the second page should be visible 
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
        /// Gets or sets a value indicating whether or not the third page should be visible 
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

        public string ReadyMessage { get { return readyMessage; } set { readyMessage = value; NotifyPropertyChanged(); } }

        public int SetupProgress { get; set; }
        public bool Setup1Done { get; set; }

        public string Console1Name
        {
            get
            {
                if (xboxDevice1 == null)
                {
                    return "-no console-";
                }
                else
                {
                    return xboxDevice1.Name;
                }
            }
        }
        public string Console2Name
        {
            get
            {
                if (xboxDevice2 == null)
                {
                    return "-no console-";
                }
                else
                {
                    return xboxDevice2.Name;
                }
            }
        }
        public string Console3Name
        {
            get
            {
                if (xboxDevice3 == null)
                {
                    return "-no console-";
                }
                else
                {
                    return xboxDevice3.Name;
                }
            }
        }
        public string ProfileAName
        {
            get
            {
                if (profileA == null)
                {
                    return "-no profile-";
                }
                else
                {
                    return profileA.Gamertag;
                }
            }
        }
        public string ProfileBName
        {
            get
            {
                if (profileB == null)
                {
                    return "-no profile-";
                }
                else
                {
                    return profileB.Gamertag;
                }
            }
        }
        public string ProfileCName
        {
            get
            {
                if (profileC == null)
                {
                    return "-no profile-";
                }
                else
                {
                    return profileC.Gamertag;
                }
            }
        }
        public string ProfileACommunications { get; set; }
        public string ProfileBCommunications { get; set; }
        public string ProfileCCommunications { get; set; }
        public string ProfileAFriended { get; set; }
        public string ProfileBFriended { get; set; }
        public string ProfileCFriended { get; set; }
        public string ProfileALaunched { get; set; }
        public string ProfileBLaunched { get; set; }
        public string ProfileCLaunched { get; set; }

        /// <summary>
        /// Start - called when the module is first entered
        /// 
        /// This function is called to show the overview or intro to the module.
        /// Typically the framework is active and user should choose a device in the device pool.
        /// </summary>
        /// <param name="ctx">The current working context for which this test will execute.</param>
        public void Start(IModuleContext ctx)
        {
            this.moduleContext = ctx as IXboxModuleContext;
            this.moduleUI = new CMTV093CTC1UI(this);
        }

        /// <summary>
        /// NextPage - called to leave the the module overview or intro screen entered by Start(ctx)
        /// The framework goes modal in this call and the module gains control.
        /// 
        /// This function is called repeatedly to advance to multiple screens in the module.
        /// </summary>
        public void NextPage()
        {
            if (FirstPageVisibility == Visibility.Visible)
            {
                string title = "";

                // get the three xboxes
                if (!ThreeConnectedXboxesSelected)
                    return;

                if (this.moduleContext.XboxTitle.Name != "")
                {
                    title = this.moduleContext.XboxTitle.Name;
                }

                if (title == "")
                {
                    MessageBox.Show("Please select a title from the setup dialog", "Certification Assistance Tool");
                    return;
                }

                moduleContext.IsModal = true;
                FirstPageVisibility = Visibility.Collapsed;
                SecondPageVisibility = Visibility.Visible;
                this.moduleUI.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);

                // set up test
                Setup1();
            }
            else if (SecondPageVisibility == Visibility.Visible)
            {
                SecondPageVisibility = Visibility.Collapsed;
                ThirdPageVisibility = Visibility.Visible;
                this.moduleUI.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);
            }
        }

        /// <summary>
        /// Log the setup parameters
        /// </summary>
        public void LogSetup()
        {
            moduleContext.Log("Initial Setup");
            moduleContext.Log("Console1 Name: " + Console1Name);
            moduleContext.Log("ProfileA Name: " + ProfileAName);
            moduleContext.Log("ProfileA Communications: " + ProfileACommunications);
            moduleContext.Log("ProfileA Friended: " + ProfileAFriended);

            moduleContext.Log("Console2 Name: " + Console2Name);
            moduleContext.Log("ProfileB Name: " + ProfileBName);
            moduleContext.Log("ProfileB Communications: " + ProfileBCommunications);
            moduleContext.Log("ProfileB Friended: " + ProfileBFriended);

            moduleContext.Log("Console3 Name: " + Console3Name);
            moduleContext.Log("ProfileC Name: " + ProfileCName);
            moduleContext.Log("ProfileC Communications: " + ProfileCCommunications);
            moduleContext.Log("ProfileC Friended: " + ProfileCFriended);
        }

        public void UpdateSetup()
        {
            NotifyPropertyChanged("Console1Name");
            NotifyPropertyChanged("Console2Name");
            NotifyPropertyChanged("Console3Name");
            NotifyPropertyChanged("ProfileAName");
            NotifyPropertyChanged("ProfileBName");
            NotifyPropertyChanged("ProfileCName");
            NotifyPropertyChanged("ProfileACommunications");
            NotifyPropertyChanged("ProfileBCommunications");
            NotifyPropertyChanged("ProfileCCommunications");
            NotifyPropertyChanged("ProfileAFriended");
            NotifyPropertyChanged("ProfileBFriended");
            NotifyPropertyChanged("ProfileCFriended");
            NotifyPropertyChanged("ProfileALaunched");
            NotifyPropertyChanged("ProfileBLaunched");
            NotifyPropertyChanged("ProfileCLaunched");
            NotifyPropertyChanged("ReadyMessage");
            NotifyPropertyChanged("SetupProgress");
            NotifyPropertyChanged("Setup1Done");

            this.moduleUI.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);
        }

        public bool Setup1()
        {
            bool aborted = false;
            bool friendingError = false;

            Mouse.OverrideCursor = Cursors.Wait;

            // assign xboxes
            foreach (IXboxDevice dev in this.moduleContext.SelectedDevices)
            {
                if (dev.IsDefault)
                {
                    xboxDevice1 = dev;
                }
            }

            foreach (IXboxDevice dev in this.moduleContext.SelectedDevices)
            {
                if (xboxDevice1 == null)
                {
                    xboxDevice1 = dev;
                    NotifyPropertyChanged("Console1Name");
                }
                else if (xboxDevice2 == null && xboxDevice1 != dev)
                {
                    xboxDevice2 = dev;
                    NotifyPropertyChanged("Console2Name");
                }
                else if (xboxDevice3 == null && xboxDevice1 != dev)
                {
                    xboxDevice3 = dev;
                    NotifyPropertyChanged("Console3Name");
                }
            }

            // get a profile manager for each console
            ReadyMessage = "Getting three profiles...";
            SetupProgress = 5;
            UpdateSetup();

            try
            {
                this.profileManager1 = xboxDevice1.XboxConsole.CreateConsoleProfilesManager();
                this.profileManager2 = xboxDevice2.XboxConsole.CreateConsoleProfilesManager();
                this.profileManager3 = xboxDevice3.XboxConsole.CreateConsoleProfilesManager();
            }
            catch
            {
            }

            // sign out all profiles
            profileManager1.SignOutAllUsers();
            profileManager2.SignOutAllUsers();
            profileManager3.SignOutAllUsers();

            // select or create and sign in a default profile on each console
            profileA = SafeGetDefaultProfile(profileManager1, 0);
            profileB = SafeGetDefaultProfile(profileManager2, 0);
            profileC = SafeGetDefaultProfile(profileManager3, 0);

            // make sure profiles are not the same one
            if (profileB.Gamertag == profileA.Gamertag)
            {
                profileB = SafeGetDefaultProfile(profileManager2, 1);
            }

            if (profileC.Gamertag == profileB.Gamertag || profileC.Gamertag == profileA.Gamertag)
            {
                profileC = SafeGetDefaultProfile(profileManager3, 1);
            }

            if (profileC.Gamertag == profileB.Gamertag || profileC.Gamertag == profileA.Gamertag)
            {
                profileC = SafeGetDefaultProfile(profileManager3, 2);
            }

            // set Play Through Speakers on all three consoles
            ReadyMessage = "Setting voice output to speakers only...";
            SetupProgress += 15;
            UpdateSetup();

            Thread th1 = new Thread(new ParameterizedThreadStart(delegate
            {
                xboxDevice1.RunCatScript("Voice_Output_Set_Speakers");
                SetupProgress += 10;
            }));
            th1.Start();
            Thread th2 = new Thread(new ParameterizedThreadStart(delegate
            {
                xboxDevice2.RunCatScript("Voice_Output_Set_Speakers");
                SetupProgress += 10;
            }));
            th2.Start();
            Thread th3= new Thread(new ParameterizedThreadStart(delegate
            {
                xboxDevice3.RunCatScript("Voice_Output_Set_Speakers");
                SetupProgress += 10;
            }));
            th3.Start();

            th1.Join();
            th2.Join();
            th3.Join();

            ReadyMessage = "Friending profiles...";
            UpdateSetup();

            try
            {
                // Friend profiles A and B
                if (!AreFriended(profileA, profileB))
                {
                    profileB.Friends.SendFriendRequest(profileA);
                    profileA.Friends.AcceptFriendRequest(profileB);
                }
                ProfileAFriended = "Friended";
                ProfileBFriended = "Friended";
                SetupProgress += 10;
            }
            catch (Exception ex)
            {
                friendingError = true;
                moduleContext.Log("There was an exception friending profile " + ProfileAName + " with " + ProfileBName + ". Exception: " + ex.Message);
            }

            // enable communications for profiles B and c
            ReadyMessage = "Setting Communication Options...";
            UpdateSetup();
            th2 = new Thread(new ParameterizedThreadStart(delegate
            {
                xboxDevice2.RunCatScript("Communications_Set_Everyone");
                ProfileBCommunications = "Everyone";
                SetupProgress += 10;
            }));
            th2.Start();
            th3 = new Thread(new ParameterizedThreadStart(delegate
            {
                xboxDevice3.RunCatScript("Communications_Set_Everyone");
                ProfileCCommunications = "Everyone";
                SetupProgress += 10;
            }));
            th3.Start();

            // if friending failed, unblock profile A and friend again. then block profile A
            if (friendingError)
            {
                th1 = new Thread(new ParameterizedThreadStart(delegate
                {
                    xboxDevice1.RunCatScript("Communications_Set_Everyone");
                }));
                th1.Start();
                th2.Join();
                th1.Join();
                try
                {
                    // Friend profiles A and B
                    if (!AreFriended(profileA, profileB))
                    {
                        profileB.Friends.SendFriendRequest(profileB);
                        profileA.Friends.AcceptFriendRequest(profileA);
                    }
                    ProfileAFriended = "Friended";
                    ProfileBFriended = "Friended";
                    SetupProgress += 10;
                }
                catch (Exception ex)
                {
                    moduleContext.Log("There was an exception friending profile " + ProfileAName + " with " + ProfileBName + ". Exception: " + ex.Message);
                    MessageBox.Show("Unable to friend profiles " + ProfileAName + " and " + ProfileBName + ". Aborting test", "Certificaton Assistance Tool");
                    ProfileAFriended = "Error Friending";
                    ProfileBFriended = "Error Friending";
                    ReadyMessage = "Aborted";
                    aborted = true;
                }
            }

            UpdateSetup();
            if (aborted)
                return false;

            // block profile A
            th1 = new Thread(new ParameterizedThreadStart(delegate
            {
                xboxDevice1.RunCatScript("Communications_Set_Blocked");
                ProfileACommunications = "Blocked";
                SetupProgress += 10;
            }));
            th1.Start();
            th2.Join();
            th3.Join();
            th1.Join();

            // Install and Launch the game on all three consoles
            ReadyMessage = "Installing and launching " + this.moduleContext.XboxTitle.Name + "...";
            UpdateSetup();
            if (xboxDevice1.IsTitleInstalled)
            {
                ProfileALaunched = "Launching...";
            }
            else
            {
                ProfileALaunched = "Installing...";
            }
            if (xboxDevice2.IsTitleInstalled)
            {
                ProfileBLaunched = "Launching...";
            }
            else
            {
                ProfileBLaunched = "Installing...";
            }
            if (xboxDevice3.IsTitleInstalled)
            {
                ProfileCLaunched = "Launching...";
            }
            else
            {
                ProfileCLaunched = "Installing...";
            }
            UpdateSetup();

            th1 = new Thread(new ParameterizedThreadStart(delegate
            {
                if (!xboxDevice1.IsTitleInstalled)
                {
                    xboxDevice1.InstallTitle(xboxDevice1.PreferredInstallDrive());
                }

                ProfileALaunched = "Launching...";
                xboxDevice1.LaunchTitle();
                ProfileALaunched = "Launched";
                SetupProgress += 10;
            }));
            th1.Start();
            th2 = new Thread(new ParameterizedThreadStart(delegate
            {
                if (!xboxDevice2.IsTitleInstalled)
                {
                    xboxDevice2.InstallTitle(xboxDevice2.PreferredInstallDrive());
                }

                ProfileBLaunched = "Launching...";
                xboxDevice2.LaunchTitle();
                ProfileBLaunched = "Launched";
                SetupProgress += 10;
            }));
            th2.Start();
            th3 = new Thread(new ParameterizedThreadStart(delegate
            {
                if (!xboxDevice3.IsTitleInstalled)
                {
                    xboxDevice3.InstallTitle(xboxDevice3.PreferredInstallDrive());
                }

                ProfileCLaunched = "Launching...";
                xboxDevice3.LaunchTitle();
                ProfileCLaunched = "Launched";
                SetupProgress += 10;
            }));
            th3.Start();
            th1.Join();
            th2.Join();
            th3.Join();
            ReadyMessage = "Ready";
            Setup1Done = true;
            LogSetup();
            UpdateSetup();
            Mouse.OverrideCursor = null;
            return true;
        }

        /// <summary>
        /// Stop - called when the module is done or aborted
        /// </summary>
        public void Stop()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            // restore communication settings
            if (xboxDevice1 != null)
            {
                xboxDevice1.RunCatScript("Communications_Set_Everyone");
            }

            moduleContext.Log("*************************************************************\r\n");
            moduleContext.Log("*************************************************************\r\n");
            moduleContext.Log("RESULT: " + passedOrFailed + "\r\n");
            moduleContext.Log("*************************************************************\r\n");

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Gets or creates a profile and makes sure it is set to default
        /// </summary>
        /// <param name="manager">manager for the console</param>
        /// <param name="profileNumber">index of the profile to retrieve. a new profile is created if this index is too big. a max of one profile will be created</param>
        /// <returns></returns>
        private ConsoleProfile SafeGetDefaultProfile(ConsoleProfilesManager manager, int profileNumber)
        {
            ConsoleProfile profile = null;

            try
            {
                if (manager.EnumerateConsoleProfiles().Count() > profileNumber)
                {
                    profile = manager.EnumerateConsoleProfiles().ElementAt(profileNumber);
                }
                else
                {
                    profile = manager.CreateConsoleProfile(true);
                }

                manager.SetDefaultProfile(profile);
                profile.SignIn(UserIndex.Zero);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem getting a profile from " + manager.Console.Name + "\n\nException: " + ex.Message, "Certificaton Assistance Tool");
            }

            return profile;
        }

        /// <summary>
        /// Gets a value indicating whether whether there are three connected xboxes selected
        /// </summary>
        private bool ThreeConnectedXboxesSelected
        {
            get
            {
                string s = string.Empty;
                if (this.moduleContext.SelectedDevices.Count() == 0)
                {
                    s = "No consoles are selected. Select 3 for this module. ";
                }
                else if (this.moduleContext.SelectedDevices.Count() != 3)
                {
                    s = "Please select 3 consoles. " + this.moduleContext.SelectedDevices.Count().ToString() + " are selected.";
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

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}