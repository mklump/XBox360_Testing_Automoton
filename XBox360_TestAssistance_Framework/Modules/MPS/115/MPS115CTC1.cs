// -----------------------------------------------------------------------
// <copyright file="MPS115CTC1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MPS115
{
    using System;
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

    /// <summary>
    /// Implementation class for the MPS115CTC1 module
    /// </summary>
    public class MPS115CTC1 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// Private module context instance for this current working module context
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// User interface class instance from the MPS115CTC1UI class
        /// </summary>
        private MPS115CTC1UI moduleUI;

        /// <summary>
        /// String flag Passed or Failed
        /// </summary>
        private string passedOrFailed = "PASSED";

        /// <summary>
        /// String indicating the ready state
        /// </summary>
        private string readyMessage;

        /// <summary>
        /// Xbox device of first xbox
        /// </summary>
        private IXboxDevice xboxDevice1;

        /// <summary>
        /// Xbox device of second xbox
        /// </summary>
        private IXboxDevice xboxDevice2;

        /// <summary>
        /// Console profile A
        /// </summary>
        private ConsoleProfile profileA;

        /// <summary>
        /// Console profile B
        /// </summary>
        private ConsoleProfile profileB;
        
        /// <summary>
        /// Console profile C
        /// </summary>
        private ConsoleProfile profileC;

        /// <summary>
        /// Console profile manager for xbox 1
        /// </summary>
        private ConsoleProfilesManager profileManager1;

        /// <summary>
        /// Console profile manager for xbox 2
        /// </summary>
        private ConsoleProfilesManager profileManager2;

        /// <summary>
        /// Install drive on xbox 1
        /// </summary>
        private string installDrive1;

        /// <summary>
        /// Install drive on xbox 2
        /// </summary>
        private string installDrive2;

        /// <summary>
        /// Backing field for FirstPageVisibility property
        /// </summary>
        private Visibility firstPageVisibility = Visibility.Visible;

        /// <summary>
        /// Backing field for SecondPageVisibility property
        /// </summary>
        private Visibility secondPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for Instructions1Visible property
        /// </summary>
        private Visibility instructions1Visible = Visibility.Collapsed;

        /// <summary>
        /// Backing field for Instructions2Visible property
        /// </summary>
        private Visibility instructions2Visible = Visibility.Collapsed;

        /// <summary>
        /// Backing field for Instructions3Visible property
        /// </summary>
        private Visibility instructions3Visible = Visibility.Collapsed;

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the game runs from MU or HD
        /// </summary>
        public bool IsMUTest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether game can be installed to MU (content package less than 512MB)
        /// </summary>
        public bool CanDoMUTestNext { get; set; }

        /// <summary>
        /// Gets a value indicating whether testing is done
        /// </summary>
        public bool IsTestingDone
        {
            get
            {
                return !this.CanPassTestA && !this.CanPassTestB && (this.Instructions3Visible == Visibility.Visible);
            }
        }

        /// <summary>
        /// Gets the UIElement control for this module
        /// </summary>
        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
        }

        /// <summary>
        /// Gets the console name of the first xbox
        /// </summary>
        public string Console1Name
        {
            get
            {
                if (this.xboxDevice1 == null)
                {
                    return "-no console-";
                }
                else
                {
                    return this.xboxDevice1.Name;
                }
            }
        }

        /// <summary>
        /// Gets the console name of the second xbox
        /// </summary>
        public string Console2Name
        {
            get
            {
                if (this.xboxDevice2 == null)
                {
                    return "-no console-";
                }
                else
                {
                    return this.xboxDevice2.Name;
                }
            }
        }

        /// <summary>
        /// Gets the profile name of profile A
        /// </summary>
        public string ProfileAName
        {
            get
            {
                if (this.profileA == null)
                {
                    return "-no profile-";
                }
                else
                {
                    return this.profileA.Gamertag;
                }
            }
        }

        /// <summary>
        /// Gets the profile name of profile B
        /// </summary>
        public string ProfileBName
        {
            get
            {
                if (this.profileB == null)
                {
                    return "-no profile-";
                }
                else
                {
                    return this.profileB.Gamertag;
                }
            }
        }

        /// <summary>
        /// Gets the profile name of profile C
        /// </summary>
        public string ProfileCName
        {
            get
            {
                if (this.profileC == null)
                {
                    return "-no profile-";
                }
                else
                {
                    return this.profileC.Gamertag;
                }
            }
        }

        /// <summary>
        /// Gets or sets a string indicating the installed state of Profile A
        /// </summary>
        public string ProfileAInstalled { get; set; }

        /// <summary>
        /// Gets or sets a string indicating the installed state of Profile B
        /// </summary>
        public string ProfileBInstalled { get; set; }

        /// <summary>
        /// Gets or sets a string indicating the installed state of Profile C
        /// </summary>
        public string ProfileCInstalled { get; set; }

        /// <summary>
        /// Gets or sets a string indicating the launched state of Profile A
        /// </summary>
        public string ProfileALaunched { get; set; }

        /// <summary>
        /// Gets or sets a string indicating the launched state of Profile B
        /// </summary>
        public string ProfileBLaunched { get; set; }

        /// <summary>
        /// Gets or sets a string indicating the launched state of Profile C
        /// </summary>
        public string ProfileCLaunched { get; set; }

        /// <summary>
        /// Gets or sets a message to display indicating test readiness
        /// </summary>
        public string ReadyMessage
        {
            get
            {
                return this.readyMessage; 
            }
            
            set
            {
                this.readyMessage = value; 
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the visibility of instructions
        /// </summary>
        public Visibility Instructions1Visible 
        {
            get
            {
                return this.instructions1Visible; 
            }
            
            set
            {
                this.instructions1Visible = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the visibility of instructions
        /// </summary>
        public Visibility Instructions2Visible
        {
            get
            {
                return this.instructions2Visible;
            }
            
            set
            {
                this.instructions2Visible = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the visibility of instructions
        /// </summary>
        public Visibility Instructions3Visible
        {
            get
            {
                return this.instructions3Visible;
            }
            
            set
            {
                this.instructions3Visible = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether it's OK to go to the next test
        /// </summary>
        public bool CanGoToNextTest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether test A has passed
        /// </summary>
        public bool CanPassTestA { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether test B has passed
        /// </summary>
        public bool CanPassTestB { get; set; }

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
        /// Gets a value indicating whether whether there are two connected xboxes selected
        /// </summary>
        private bool TwoConnectedXboxesSelected
        {
            get
            {
                string s = string.Empty;
                if (this.moduleContext.SelectedDevices.Count == 0)
                {
                    s = "No consoles are selected. Select 2 for this module. ";
                }
                else if (this.moduleContext.SelectedDevices.Count == 1)
                {
                    s = "Please select 2 consoles. 1 is selected.";
                }
                else if (this.moduleContext.SelectedDevices.Count != 2)
                {
                    s = "Please select 2 consoles. " + this.moduleContext.SelectedDevices.Count.ToString() + " are selected.";
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
        /// <para />
        /// This function is called to show the overview or intro to the module.
        /// Typically the framework is active and user should choose a device in the device pool.
        /// </summary>
        /// <param name="ctx">The current working context for which this test will execute.</param>
        public void Start(IModuleContext ctx)
        {
            this.moduleContext = ctx as IXboxModuleContext;
            this.moduleUI = new MPS115CTC1UI(this);
        }

        /// <summary>
        /// BeginMUTest - called to leave the the module overview or intro screen entered by Start(Module Context)
        /// The framework goes modal in this call and the module gains control.
        /// <para />
        /// </summary>
        public void BeginMUTest()
        {
            bool titleIsTooBig = false;
            bool hadException = false;
            this.IsMUTest = true;

            if (!this.TwoConnectedXboxesSelected)
            {
                return;
            }

            // assign xboxes
            if (((IXboxDevice)this.moduleContext.SelectedDevices[1]).IsDefault)
            {
                this.xboxDevice1 = (IXboxDevice)this.moduleContext.SelectedDevices[1];
                this.xboxDevice2 = (IXboxDevice)this.moduleContext.SelectedDevices[0];
            }
            else
            {
                this.xboxDevice1 = (IXboxDevice)this.moduleContext.SelectedDevices[0];
                this.xboxDevice2 = (IXboxDevice)this.moduleContext.SelectedDevices[1];
            }

            this.NotifyPropertyChanged("Console1Name");
            this.NotifyPropertyChanged("Console2Name");

            if (string.IsNullOrEmpty(this.moduleContext.XboxTitle.Name))
            {
                MessageBox.Show("Please select a title from the setup dialog", "Certification Assistance Tool");
                return;
            }

            if (this.moduleContext.XboxTitle.GameInstallType != "Content Package")
            {
                MessageBox.Show("Only a content package title can be run from an MU. Please select a Content Package title from the setup dialog", "Certification Assistance Tool");
                return;
            }

            ulong size = 0;
            try
            {
                FileInfo fi;

                if (!string.IsNullOrEmpty(this.moduleContext.XboxTitle.ContentPackage))
                {
                    fi = new FileInfo(this.moduleContext.XboxTitle.ContentPackage);
                    size += (ulong)fi.Length;
                }

                if (!string.IsNullOrEmpty(this.moduleContext.XboxTitle.TitleUpdatePath))
                {
                    fi = new FileInfo(this.moduleContext.XboxTitle.TitleUpdatePath);
                    size += (ulong)fi.Length;
                }

                // reported free bytes on a 512MB MU when two profiles are on it
                ////if (size > (512 * 1024 * 1024))
                if (size > 495779840) 
                {
                    titleIsTooBig = true;
                }
            }
            catch
            {
                hadException = true;
            }

            if (titleIsTooBig)
            {
                MessageBox.Show(this.moduleContext.XboxTitle.Name + " is too big to fit onto a 512 MB Memory Unit. The MU test cannot be run on this title", "Certification Assistance Tool");
                return;
            }

            if (hadException)
            {
                MessageBox.Show("There was a problem determining the size of " + this.moduleContext.XboxTitle.Name + ". Unable to run MU test", "Certification Assistance Tool");
                return;
            }

            // check for MU or prompt user to insert one
            this.installDrive1 = this.RightDrive(this.xboxDevice1);
            this.installDrive2 = this.RightDrive(this.xboxDevice2);

            if (string.IsNullOrEmpty(this.installDrive1) && string.IsNullOrEmpty(this.installDrive2))
            {
                return;
            }

            // check MU has enough free space. if not, clear the MU
            if (this.xboxDevice1.GetFreeSpace(this.installDrive1) < size)
            {
                this.xboxDevice1.RecursiveDirectoryDelete(this.installDrive1 + ":\\");

                if (this.xboxDevice1.GetFreeSpace(this.installDrive1) < size)
                {
                    MessageBox.Show("Please insert a larger MU into " + this.xboxDevice1.Name, "Certification Assistance Tool");
                    return;
                }
            }

            if (this.xboxDevice2.GetFreeSpace(this.installDrive2) < size)
            {
                this.xboxDevice2.RecursiveDirectoryDelete(this.installDrive2 + ":\\");

                if (this.xboxDevice2.GetFreeSpace(this.installDrive2) < size)
                {
                    MessageBox.Show("Please insert a larger MU into " + this.xboxDevice2.Name, "Certification Assistance Tool");
                    return;
                }
            }

            this.NextPage();
        }

        /// <summary>
        /// BeginHDTest - called to leave the the module overview or intro screen entered by Start(Module Context)
        /// The framework goes modal in this call and the module gains control.
        /// <para />
        /// </summary>
        public void BeginHDTest()
        {
            if (!this.TwoConnectedXboxesSelected)
            {
                return;
            }

            // assign xboxes
            if (((IXboxDevice)this.moduleContext.SelectedDevices[1]).IsDefault)
            {
                this.xboxDevice1 = (IXboxDevice)this.moduleContext.SelectedDevices[1];
                this.xboxDevice2 = (IXboxDevice)this.moduleContext.SelectedDevices[0];
            }
            else
            {
                this.xboxDevice1 = (IXboxDevice)this.moduleContext.SelectedDevices[0];
                this.xboxDevice2 = (IXboxDevice)this.moduleContext.SelectedDevices[1];
            }

            this.NotifyPropertyChanged("Console1Name");
            this.NotifyPropertyChanged("Console2Name");

            if (string.IsNullOrEmpty(this.moduleContext.XboxTitle.Name))
            {
                MessageBox.Show("Please select a title from the setup dialog", "Certification Assistance Tool");
                return;
            }

            // check if this title should be tested from the MU later
            if (this.moduleContext.XboxTitle.GameInstallType == "Content Package")
            {
                try
                {
                    FileInfo fi;
                    long size = 0;

                    if (!string.IsNullOrEmpty(this.moduleContext.XboxTitle.ContentPackage))
                    {
                        fi = new FileInfo(this.moduleContext.XboxTitle.ContentPackage);
                        size += fi.Length;
                    }

                    if (!string.IsNullOrEmpty(this.moduleContext.XboxTitle.TitleUpdatePath))
                    {
                        fi = new FileInfo(this.moduleContext.XboxTitle.TitleUpdatePath);
                        size += fi.Length;
                    }

                    // reported free bytes on a 512MB MU when two profiles are on it
                    ////if (size < (512 * 1024 * 1024))
                    if (size > 495779840)
                    {
                        this.CanDoMUTestNext = true;
                    }
                }
                catch
                {
                }
            }

            this.installDrive1 = "HDD";
            this.installDrive2 = "HDD";
            this.NextPage();
        }

        /// <summary>
        /// NextPage - This function is called repeatedly to advance to multiple screens in the module.
        /// </summary>
        public void NextPage()
        {
            if (this.FirstPageVisibility == Visibility.Visible)
            {
                // go modal
                this.moduleContext.IsModal = true;
                this.FirstPageVisibility = Visibility.Collapsed;
                this.SecondPageVisibility = Visibility.Visible;
                this.moduleUI.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);

                // set up test
                this.Setup();
                this.CanPassTestA = true;
                this.CanPassTestB = true;
                this.Instructions1Visible = Visibility.Visible;
                this.NotifyPropertyChanged("CanPassTestA");
                this.NotifyPropertyChanged("CanPassTestB");
            }
            else if (this.SecondPageVisibility == Visibility.Visible)
            {
                if (this.Instructions1Visible == Visibility.Visible)
                {
                    this.Instructions1Visible = Visibility.Collapsed;
                    this.Instructions2Visible = Visibility.Visible;
                    this.CanGoToNextTest = false;
                    this.CanPassTestA = true;
                    this.CanPassTestB = true;
                }
                else if (this.Instructions2Visible == Visibility.Visible)
                {
                    this.Instructions2Visible = Visibility.Collapsed;
                    this.Instructions3Visible = Visibility.Visible;
                    this.CanGoToNextTest = false;
                    this.CanPassTestA = true;
                    this.CanPassTestB = true;
                }
                else if (this.Instructions3Visible == Visibility.Visible)
                {
                    this.Instructions3Visible = Visibility.Collapsed;
                    this.CanGoToNextTest = false;
                    this.CanPassTestA = true;
                    this.CanPassTestB = true;
                }

                this.NotifyPropertyChanged("CanGoToNextTest");
                this.NotifyPropertyChanged("CanPassTestA");
                this.NotifyPropertyChanged("CanPassTestB");
            }
        }

        /// <summary>
        /// Toggle the Signed In/Out state of the specified profile
        /// </summary>
        /// <param name="name">Name of profile to sign in or out</param>
        /// <returns>A status string representing the signed in state</returns>
        public string SignInOut(string name)
        {
            string state = string.Empty;

            if (name == "-no profile-")
            {
                state = name;
            }
            else if (name == this.ProfileAName)
            {
                state = this.SignInOut(this.profileA);
            }
            else if (name == this.ProfileBName)
            {
                state = this.SignInOut(this.profileB);
            }
            else if (name == this.ProfileCName)
            {
                state = this.SignInOut(this.profileC);
            }

            return state;
        }

        /// <summary>
        /// Toggle the Signed In/Out state of the specified profile
        /// </summary>
        /// <param name="profile">ConsoleProfile to sign in or out</param>
        /// <returns>A status string representing the signed in state</returns>
        public string SignInOut(ConsoleProfile profile)
        {
            string state = string.Empty;

            if (profile.GetUserSigninState() != SignInState.NotSignedIn)
            {
                profile.SignOut();
                state = "Sign In";
            }
            else
            {
                profile.SignIn(UserIndex.Zero);
                state = "Sign Out";
            }

            return state;
        }

        /// <summary>
        /// Indicates that the test has passed
        /// </summary>
        /// <param name="message">String associated with this test</param>
        public void Pass(string message)
        {
            if (this.Instructions1Visible == Visibility.Visible)
            {
                this.moduleContext.Log("Same Title – Active Player - Test " + message + ": Passed");
            }
            else if (this.Instructions2Visible == Visibility.Visible)
            {
                this.moduleContext.Log("Same Title – Inactive Player - Test " + message + ": Passed");
            }
            else if (this.Instructions3Visible == Visibility.Visible)
            {
                this.moduleContext.Log("Cross Title - Test " + message + ": Passed");
            }

            if (message == "Game Invite")
            {
                this.CanPassTestA = false;
            }

            if (message == "Join Session in Progress")
            {
                this.CanPassTestB = false;
            }

            if (!this.CanPassTestA && !this.CanPassTestB && (this.Instructions3Visible == Visibility.Collapsed))
            {
                this.CanGoToNextTest = true;
            }

            this.NotifyPropertyChanged("CanGoToNextTest");
            this.NotifyPropertyChanged("CanPassTestA");
            this.NotifyPropertyChanged("CanPassTestB");
            this.NotifyPropertyChanged("IsTestingDone");
        }

        /// <summary>
        /// Indicates that the test has failed
        /// </summary>
        /// <param name="message">String associated with this test</param>
        public void Fail(string message)
        {
            if (this.Instructions1Visible == Visibility.Visible)
            {
                this.moduleContext.Log("Same Title – Active Player - Test " + message + ": Failed");
            }
            else if (this.Instructions2Visible == Visibility.Visible)
            {
                this.moduleContext.Log("Same Title – Inactive Player - Test " + message + ": Failed");
            }
            else if (this.Instructions3Visible == Visibility.Visible)
            {
                this.moduleContext.Log("Cross Title - Test " + message + ": Failed");
            }

            if (message == "Game Invite")
            {
                this.CanPassTestA = false;
            }

            if (message == "Join Session in Progress")
            {
                this.CanPassTestB = false;
            }

            if (!this.CanPassTestA && !this.CanPassTestB && (this.Instructions3Visible == Visibility.Collapsed))
            {
                this.CanGoToNextTest = true;
            }

            this.NotifyPropertyChanged("CanGoToNextTest");
            this.NotifyPropertyChanged("CanPassTestA");
            this.NotifyPropertyChanged("CanPassTestB");
            this.NotifyPropertyChanged("IsTestingDone");

            this.passedOrFailed = "Fail";
        }

        /// <summary>
        /// Stop - called when the module is done or aborted
        /// </summary>
        public void Stop()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("RESULT: " + this.passedOrFailed + "\r\n");
            this.moduleContext.Log("*************************************************************\r\n");

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Update setup related UI properties
        /// </summary>
        private void UpdateSetup()
        {
            this.NotifyPropertyChanged("ProfileAName");
            this.NotifyPropertyChanged("ProfileBName");
            this.NotifyPropertyChanged("ProfileCName");
            this.NotifyPropertyChanged("Console1Name");
            this.NotifyPropertyChanged("Console2Name");
            this.NotifyPropertyChanged("Console3Name");
            this.NotifyPropertyChanged("ProfileAInstalled");
            this.NotifyPropertyChanged("ProfileBInstalled");
            this.NotifyPropertyChanged("ProfileCInstalled");
            this.NotifyPropertyChanged("ProfileALaunched");
            this.NotifyPropertyChanged("ProfileBLaunched");
            this.NotifyPropertyChanged("ProfileCLaunched");
            this.NotifyPropertyChanged("ReadyMessage");

            this.moduleUI.Dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);
        }

        /// <summary>
        /// Perform setup
        /// </summary>
        private void Setup()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            // get a profile from each console
            this.ReadyMessage = "Getting profiles...";
            this.UpdateSetup();

            try
            {
                this.profileManager1 = this.xboxDevice1.XboxConsole.CreateConsoleProfilesManager();
                this.profileManager2 = this.xboxDevice2.XboxConsole.CreateConsoleProfilesManager();
            }
            catch
            {
            }

            // sign out all profiles
            this.profileManager1.SignOutAllUsers();
            this.profileManager2.SignOutAllUsers();

            // select or create and sign in a profile on each console
            this.profileA = this.GetAProfile(this.profileManager1, 0);
            this.profileB = this.GetAProfile(this.profileManager2, 0);
            this.profileC = this.GetAProfile(this.profileManager2, 1);

            // make sure profiles are not the same one
            if (this.profileB.Gamertag == this.profileA.Gamertag)
            {
                this.profileB = this.GetAProfile(this.profileManager2, 2);
            }
            else if (this.profileC.Gamertag == this.profileA.Gamertag)
            {
                this.profileC = this.GetAProfile(this.profileManager2, 2);
            }

            // sign in profiles A and B. And C, just long enough to accept friend request
            this.profileA.SignIn(UserIndex.Zero);
            this.profileB.SignIn(UserIndex.Zero);
            this.profileC.SignIn(UserIndex.One);

            this.ReadyMessage = "Friending profiles...";
            this.UpdateSetup();

            try
            {
                // Friend profiles A and B
                if (!this.AreFriended(this.profileA, this.profileB))
                {
                    this.profileA.Friends.SendFriendRequest(this.profileB);
                    this.profileB.Friends.AcceptFriendRequest(this.profileA);
                }

                // Friend profiles A and C
                if (!this.AreFriended(this.profileA, this.profileC))
                {
                    this.profileA.Friends.SendFriendRequest(this.profileC);
                    this.profileC.Friends.AcceptFriendRequest(this.profileA);
                }
            }
            catch (Exception ex)
            {
                this.moduleContext.Log("There was an exception friending profiles. Exception: " + ex.Message);
            }

            // verify friending worked
            try
            {
                // Friend profiles A and B
                if (!this.AreFriended(this.profileA, this.profileB))
                {
                    this.profileA.Friends.SendFriendRequest(this.profileB);
                    this.profileB.Friends.AcceptFriendRequest(this.profileA);
                }

                // Friend profiles A and C
                if (!this.AreFriended(this.profileA, this.profileC))
                {
                    this.profileA.Friends.SendFriendRequest(this.profileC);
                    this.profileC.Friends.AcceptFriendRequest(this.profileA);
                }

                // sign out profile C
                this.profileC.SignOut();
            }
            catch (Exception ex)
            {
                this.moduleContext.Log("There was an exception friending profiles. Exception: " + ex.Message);
            }

            // Install and Launch the game on both consoles (unless game is emulated)
            this.ReadyMessage = "Installing and launching " + this.moduleContext.XboxTitle.Name + "...";
            this.UpdateSetup();
            if (this.IsTitleInstalledOnRightDrive(this.xboxDevice1))
            {
                this.ProfileAInstalled = this.moduleContext.XboxTitle.Name + " Installed";
                this.ProfileALaunched = this.moduleContext.XboxTitle.Name + " Launching...";
            }
            else
            {
                this.ProfileAInstalled = this.moduleContext.XboxTitle.Name + " Installing...";
            }

            if (this.IsTitleInstalledOnRightDrive(this.xboxDevice2))
            {
                this.ProfileBInstalled = this.moduleContext.XboxTitle.Name + " Installed";
                this.ProfileCInstalled = this.moduleContext.XboxTitle.Name + " Installed";
                this.ProfileBLaunched = this.moduleContext.XboxTitle.Name + " Launching...";
            }
            else
            {
                this.ProfileBInstalled = this.moduleContext.XboxTitle.Name + " Installing...";
            }

            this.UpdateSetup();

            IXboxDevice deviceWithProgressBar;
            IXboxDevice deviceWithoutProgressBar;

            if (this.IsTitleInstalledOnRightDrive(this.xboxDevice1) && !this.IsTitleInstalledOnRightDrive(this.xboxDevice2))
            {
                deviceWithProgressBar = this.xboxDevice2;
                deviceWithoutProgressBar = this.xboxDevice1;
            }
            else
            {
                deviceWithProgressBar = this.xboxDevice1;
                deviceWithoutProgressBar = this.xboxDevice2;
            }

            Thread th1 = new Thread(new ParameterizedThreadStart(delegate
            {
                if (!this.IsTitleInstalledOnRightDrive(deviceWithoutProgressBar))
                {
                    deviceWithoutProgressBar.LaunchDevDashboard();
                    if (deviceWithoutProgressBar == this.xboxDevice1)
                    {
                        deviceWithoutProgressBar.InstallTitle(this.installDrive1);
                    }
                    else
                    {
                        deviceWithoutProgressBar.InstallTitle(this.installDrive2);
                    }
                }

                if (deviceWithoutProgressBar == this.xboxDevice1)
                {
                    this.ProfileAInstalled = this.moduleContext.XboxTitle.Name + " Installed";
                    this.ProfileALaunched = this.moduleContext.XboxTitle.Name + " Launching...";
                    deviceWithoutProgressBar.LaunchTitle(this.installDrive1);
                    this.ProfileALaunched = this.moduleContext.XboxTitle.Name + " Launched";
                }
                else
                {
                    this.ProfileBInstalled = this.moduleContext.XboxTitle.Name + " Installed";
                    this.ProfileBLaunched = this.moduleContext.XboxTitle.Name + " Launching...";
                    deviceWithoutProgressBar.LaunchTitle(this.installDrive2);
                    this.ProfileBLaunched = this.moduleContext.XboxTitle.Name + " Launched";
                }
            }));

            th1.Start();
            if (!this.IsTitleInstalledOnRightDrive(deviceWithProgressBar))
            {
                deviceWithProgressBar.LaunchDevDashboard();
                if (deviceWithProgressBar == this.xboxDevice1)
                {
                    deviceWithProgressBar.InstallTitle(this.installDrive1, this.moduleContext.OpenProgressBarWindow("Installing " + this.moduleContext.XboxTitle.Name + "..."));
                    this.ProfileAInstalled = this.moduleContext.XboxTitle.Name + " Installed";
                }
                else
                {
                    deviceWithProgressBar.InstallTitle(this.installDrive2, this.moduleContext.OpenProgressBarWindow("Installing " + this.moduleContext.XboxTitle.Name + "..."));
                    this.ProfileBInstalled = this.moduleContext.XboxTitle.Name + " Installed";
                }
            }

            if (this.moduleContext.XboxTitle.GameInstallType == "Disc Emulation")
            {
                this.ProfileBLaunched = "This is a disc emulation title. Please walk over to console " + this.xboxDevice2.Name + " and start game emulation";
            }
            else
            {
                if (deviceWithProgressBar == this.xboxDevice2)
                {
                    deviceWithProgressBar.LaunchTitle(this.installDrive2);
                    this.ProfileBLaunched = this.moduleContext.XboxTitle.Name + " Launched";
                }
                else
                {
                    deviceWithProgressBar.LaunchTitle(this.installDrive1);
                    this.ProfileALaunched = this.moduleContext.XboxTitle.Name + " Launched";
                }
            }

            th1.Join();

            this.ReadyMessage = "Ready";
            this.UpdateSetup();
            this.moduleContext.Log("Setup test on " + this.xboxDevice1.Name + " with profile " + this.ProfileAName);
            this.moduleContext.Log("          and " + this.xboxDevice2.Name + " with profiles " + this.ProfileBName + " and " + this.ProfileCName);

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Checks if title is already installed on hard drive or MU
        /// </summary>
        /// <param name="device">Device to check for title on</param>
        /// <returns>True if title is installed on the right drive</returns>
        private bool IsTitleInstalledOnRightDrive(IXboxDevice device)
        {
            if (!this.IsMUTest)
            {
                return device.IsTitleInstalled;
            }

            return device.IsContentPackageBasedTitleInstalledOnMU0 || device.IsContentPackageBasedTitleInstalledOnMU1;
        }

        /// <summary>
        /// Gets the right drive for the test
        /// </summary>
        /// <param name="device">The device to check for the right drive on</param>
        /// <returns>A string indicating the right drive</returns>
        private string RightDrive(IXboxDevice device)
        {
            while (true)
            {
                if (!this.IsMUTest)
                {
                    return "HDD";
                }

                if (device.IsMU0Enabled)
                {
                    return "MU0";
                }

                if (device.IsMU1Enabled)
                {
                    return "MU1";
                }

                // oops
                if (MessageBoxResult.Cancel == MessageBox.Show("Please insert an external Memory Unit into " + device.Name + ".\n\nBoth Xboxes need an external MU for this test", "Cerfification Assistance Tool", MessageBoxButton.OKCancel))
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or creates a profile
        /// </summary>
        /// <param name="manager">manager for the console</param>
        /// <param name="profileNumber">index of the profile to retrieve. a new profile is created if this index is too big. a max of one profile will be created</param>
        /// <returns>A console profile</returns>
        private ConsoleProfile GetAProfile(ConsoleProfilesManager manager, int profileNumber)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem getting a profile from " + manager.Console.Name + "\n\nException: " + ex.Message, "Certificaton Assistance Tool");
            }

            return profile;
        }

        /// <summary>
        /// AreFriended - checks if two profiles are friends
        /// </summary>
        /// <param name="profile1">a profile</param>
        /// <param name="profile2">another profile</param>
        /// <returns>true if profiles are friends, false otherwise</returns>
        private bool AreFriended(ConsoleProfile profile1, ConsoleProfile profile2)
        {
            bool friended = false;

            try
            {
                foreach (Friend friend in profile1.Friends.EnumerateFriends())
                {
                    if (friend.Gamertag == profile2.Gamertag)
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
        /// Notify property changed event handler function for all property bound controls
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
