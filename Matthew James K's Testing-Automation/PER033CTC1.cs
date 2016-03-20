// -----------------------------------------------------------------------
// <copyright file="PER033CTC1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace PER033
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
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using XDevkit;

    /// <summary>
    /// Module PER033CTC1
    /// </summary>
    public class PER033CTC1 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// Current working module context
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// User interface reference object
        /// </summary>
        private PER033CTC1UI moduleUI;

        /// <summary>
        /// A value indicating whether or not controllers are bound
        /// </summary>
        private bool controllersBound;

        /// <summary>
        /// A string indicating the result of the test
        /// </summary>
        private string passedOrFailed = "PASSED";

        /// <summary>
        /// XBox automation gamepad object used with
        /// <![CDATA[ List<IXboxAutomation> automation ]]> member object
        /// </summary>
        private XBOX_AUTOMATION_GAMEPAD gamepad = new XBOX_AUTOMATION_GAMEPAD();

        /// <summary>
        /// Backing field for FirstPageVisibility property
        /// </summary>
        private Visibility firstPageVisibility = Visibility.Visible;

        /// <summary>
        /// Backing field for SecondPageVisibility property
        /// </summary>
        private Visibility secondPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for VirtualControllerVisibility property
        /// </summary>
        private Visibility virtualControllerVisibility = Visibility.Hidden;

        /// <summary>
        /// Backing field for KeyboardLayoverVisibility property
        /// </summary>
        private Visibility keyboardLayoverVisibility = Visibility.Hidden;

        /// <summary>
        /// Backing field property for xboxDevice
        /// </summary>
        private IXboxDevice xboxDevice;

        /// <summary>
        /// Backing field for Quadrant1Connected property
        /// </summary>
        private bool quadrant1Connected;

        /// <summary>
        /// Backing field for Quadrant2Connected property
        /// </summary>
        private bool quadrant2Connected;

        /// <summary>
        /// Backing field for Quadrant3Connected property
        /// </summary>
        private bool quadrant3Connected;

        /// <summary>
        /// Backing field for Quadrant4Connected property
        /// </summary>
        private bool quadrant4Connected;

        /// <summary>
        /// Backing field for Quadrant1Controlled property
        /// </summary>
        private bool quadrant1Controlled;

        /// <summary>
        /// Backing field for Quadrant2Controlled property
        /// </summary>
        private bool quadrant2Controlled;

        /// <summary>
        /// Backing field for Quadrant3Controlled property
        /// </summary>
        private bool quadrant3Controlled;

        /// <summary>
        /// Backing field for Quadrant4Controlled property
        /// </summary>
        private bool quadrant4Controlled;

        /// <summary>
        /// The currently controlled quadrant
        /// </summary>
        private uint controlledQuadrant = 0;

        /// <summary>
        /// Backing field for Quadrant1NotDone property
        /// </summary>
        private bool quadrant1NotDone = true;

        /// <summary>
        /// Backing field for Quadrant2NotDone property
        /// </summary>
        private bool quadrant2NotDone = true;

        /// <summary>
        /// Backing field for Quadrant3NotDone property
        /// </summary>
        private bool quadrant3NotDone = true;

        /// <summary>
        /// Backing field for Quadrant4NotDone property
        /// </summary>
        private bool quadrant4NotDone = true;

        /// <summary>
        /// Backing field for AllQuadrantsDone property
        /// </summary>
        private bool allQuadrantsDone;

        /// <summary>
        /// Backing field for IsControllerConnected property
        /// </summary>
        private bool isControllerConnected = true;

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the module context
        /// </summary>
        public IXboxModuleContext ModuleContext
        {
            get { return this.moduleContext; }
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
        /// Gets or sets a value indicating whether or not the keyboard layover should be visible 
        /// </summary>
        public Visibility KeyboardLayoverVisibility
        {
            get
            {
                return this.keyboardLayoverVisibility;
            }

            set
            {
                this.keyboardLayoverVisibility = value;
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
        /// Gets or sets a value indicating whether quadrant 1 is connected
        /// </summary>
        public bool Quadrant1Connected
        {
            get
            {
                return this.quadrant1Connected;
            }

            set
            {
                this.quadrant1Connected = value;
                if (value)
                {
                    this.ConnectQuadrant(1);
                }
                else
                {
                    this.DisconnectQuadrant(1);
                    this.quadrant1Controlled = false;
                    this.NotifyPropertyChanged("Quadrant1Controlled");
                }

                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 2 is connected
        /// </summary>
        public bool Quadrant2Connected
        {
            get
            {
                return this.quadrant2Connected;
            }

            set
            {
                this.quadrant2Connected = value;
                if (value)
                {
                    this.ConnectQuadrant(2);
                }
                else
                {
                    this.DisconnectQuadrant(2);
                    this.quadrant2Controlled = false;
                    this.NotifyPropertyChanged("Quadrant2Controlled");
                }

                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 3 is connected
        /// </summary>
        public bool Quadrant3Connected
        {
            get
            {
                return this.quadrant3Connected;
            }

            set
            {
                this.quadrant3Connected = value;
                if (value)
                {
                    this.ConnectQuadrant(3);
                }
                else
                {
                    this.DisconnectQuadrant(3);
                    this.quadrant3Controlled = false;
                    this.NotifyPropertyChanged("Quadrant3Controlled");
                }

                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 4 is connected
        /// </summary>
        public bool Quadrant4Connected
        {
            get
            {
                return this.quadrant4Connected;
            }

            set
            {
                this.quadrant4Connected = value;
                if (value)
                {
                    this.ConnectQuadrant(4);
                }
                else
                {
                    this.DisconnectQuadrant(4);
                    this.quadrant4Controlled = false;
                    this.NotifyPropertyChanged("Quadrant4Controlled");
                }

                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 1 is controlled
        /// </summary>
        public bool Quadrant1Controlled
        {
            get
            {
                return this.quadrant1Controlled;
            }

            set
            {
                this.quadrant1Controlled = value;
                if (value)
                {
                    this.Quadrant1Connected = true;
                    this.controlledQuadrant = 1;
                    this.quadrant2Controlled = false;
                    this.quadrant3Controlled = false;
                    this.quadrant4Controlled = false;
                    this.NotifyPropertyChanged("Quadrant2Controlled");
                    this.NotifyPropertyChanged("Quadrant3Controlled");
                    this.NotifyPropertyChanged("Quadrant4Controlled");
                }
                else
                {
                    this.controlledQuadrant = 0;
                }

                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("AnyQuadrantControlled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 1 is controlled
        /// </summary>
        public bool Quadrant2Controlled
        {
            get
            {
                return this.quadrant2Controlled;
            }

            set
            {
                this.quadrant2Controlled = value;
                if (value)
                {
                    this.Quadrant2Connected = true;
                    this.controlledQuadrant = 2;
                    this.quadrant1Controlled = false;
                    this.quadrant3Controlled = false;
                    this.quadrant4Controlled = false;
                    this.NotifyPropertyChanged("Quadrant1Controlled");
                    this.NotifyPropertyChanged("Quadrant3Controlled");
                    this.NotifyPropertyChanged("Quadrant4Controlled");
                }
                else
                {
                    this.controlledQuadrant = 0;
                }

                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("AnyQuadrantControlled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 1 is controlled
        /// </summary>
        public bool Quadrant3Controlled
        {
            get
            {
                return this.quadrant3Controlled;
            }

            set
            {
                this.quadrant3Controlled = value;
                if (value)
                {
                    this.Quadrant3Connected = true;
                    this.controlledQuadrant = 3;
                    this.quadrant1Controlled = false;
                    this.quadrant2Controlled = false;
                    this.quadrant4Controlled = false;
                    this.NotifyPropertyChanged("Quadrant1Controlled");
                    this.NotifyPropertyChanged("Quadrant2Controlled");
                    this.NotifyPropertyChanged("Quadrant4Controlled");
                }
                else
                {
                    this.controlledQuadrant = 0;
                }

                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("AnyQuadrantControlled");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 4 is controlled
        /// </summary>
        public bool Quadrant4Controlled
        {
            get
            {
                return this.quadrant4Controlled;
            }

            set
            {
                this.quadrant4Controlled = value;
                if (value)
                {
                    this.Quadrant4Connected = true;
                    this.controlledQuadrant = 4;
                    this.quadrant1Controlled = false;
                    this.quadrant2Controlled = false;
                    this.quadrant3Controlled = false;
                    this.NotifyPropertyChanged("Quadrant1Controlled");
                    this.NotifyPropertyChanged("Quadrant2Controlled");
                    this.NotifyPropertyChanged("Quadrant3Controlled");
                }
                else
                {
                    this.controlledQuadrant = 0;
                }

                this.NotifyPropertyChanged();
                this.NotifyPropertyChanged("AnyQuadrantControlled");
            }
        }

        /// <summary>
        /// Gets a value indicating whether any quadrant is controlled
        /// </summary>
        public bool AnyQuadrantControlled
        {
            get
            {
                return this.controlledQuadrant != 0;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 1 has been tested
        /// </summary>
        public bool Quadrant1NotDone
        {
            get
            {
                return this.quadrant1NotDone;
            }

            set
            {
                this.quadrant1NotDone = value;
                if (!value && !this.quadrant2NotDone && !this.quadrant3NotDone && !this.quadrant4NotDone)
                {
                    this.AllQuadrantsDone = true;
                }

                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 2 has been tested
        /// </summary>
        public bool Quadrant2NotDone
        {
            get
            {
                return this.quadrant2NotDone;
            }

            set
            {
                this.quadrant2NotDone = value;
                if (!value && !this.quadrant1NotDone && !this.quadrant3NotDone && !this.quadrant4NotDone)
                {
                    this.AllQuadrantsDone = true;
                }

                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether quadrant 1 has been tested
        /// </summary>
        public bool Quadrant3NotDone
        {
            get
            {
                return this.quadrant3NotDone;
            }

            set
            {
                this.quadrant3NotDone = value;
                if (!value && !this.quadrant1NotDone && !this.quadrant2NotDone && !this.quadrant4NotDone)
                {
                    this.AllQuadrantsDone = true;
                }

                this.NotifyPropertyChanged();
            }
        }
            
        /// <summary>
        /// Gets or sets a value indicating whether quadrant 1 has been tested
        /// </summary>
        public bool Quadrant4NotDone
        {
            get
            {
                return this.quadrant4NotDone;
            }

            set
            {
                this.quadrant4NotDone = value;
                if (!value && !this.quadrant1NotDone && !this.quadrant2NotDone && !this.quadrant3NotDone)
                {
                    this.AllQuadrantsDone = true;
                }

                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether all quadrants have been tested
        /// </summary>
        public bool AllQuadrantsDone
        {
            get
            {
                return this.allQuadrantsDone;
            }

            set
            {
                this.allQuadrantsDone = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether we are processing keyboard input to control the consoles
        /// </summary>
        public bool KeyboardIsCaptured { get; set; }

        /// <summary>
        /// Gets or sets dispatcherTimer for polling controller input
        /// </summary>
        public DispatcherTimer DetectControllerStateTimer { get; set; }

        /// <summary>
        /// Gets or sets a command to launch the title
        /// </summary>
        public Command LaunchTitleCommand { get; set; }

        /// <summary>
        /// Gets the UI element associated with this module
        /// </summary>
        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
        }

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
        /// Start - called when the module is first entered
        /// This function is called to show the overview or intro to the module.
        /// Typically the framework is active and user should choose a device in the device pool.
        /// </summary>
        /// <param name="ctx">The current working context for which this test will execute.</param>
        public void Start(IModuleContext ctx)
        {
            this.moduleContext = ctx as IXboxModuleContext;
            this.LaunchTitleCommand = new Command((o) => this.xboxDevice.LaunchTitle());
            this.CommandA = new Command((o) => this.SendControllerButton(XboxAutomationButtonFlags.A_Button));
            this.CommandB = new Command((o) => this.SendControllerButton(XboxAutomationButtonFlags.B_Button));
            this.CommandX = new Command((o) => this.SendControllerButton(XboxAutomationButtonFlags.X_Button));
            this.CommandY = new Command((o) => this.SendControllerButton(XboxAutomationButtonFlags.Y_Button));

            this.DetectControllerStateTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);
            this.DetectControllerStateTimer.Tick += new EventHandler(this.PollController);
            this.DetectControllerStateTimer.Interval = new TimeSpan(0, 0, 0, 0, 10); // ten milliseconds
            this.DetectControllerStateTimer.Start();

            this.moduleUI = new PER033CTC1UI(this);
        }

        /// <summary>
        /// Stop - called when the module is done or aborted
        /// </summary>
        public void Stop()
        {
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            if (this.DetectControllerStateTimer != null)
            {
                this.DetectControllerStateTimer.Stop();
            }

            if (this.controllersBound)
            {
                this.xboxDevice.XboxConsole.XboxAutomation.UnbindController(0);
                this.xboxDevice.XboxConsole.XboxAutomation.UnbindController(1);
                this.xboxDevice.XboxConsole.XboxAutomation.UnbindController(2);
                this.xboxDevice.XboxConsole.XboxAutomation.UnbindController(3);
            }

            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("RESULT: " + this.passedOrFailed + "\r\n");
            this.moduleContext.Log("*************************************************************\r\n");

            System.Windows.Input.Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// NextPage - called to leave the the module overview or intro screen entered by Start(ModuleContext)
        /// The framework goes modal in this call and the module gains control.
        /// This function is called repeatedly to advance to multiple screens in the module.
        /// </summary>
        public void NextPage()
        {
            if (this.FirstPageVisibility == Visibility.Visible)
            {
                List<IDevice> selectedDevices = this.moduleContext.SelectedDevices;
                if (selectedDevices.Count != 1)
                {
                    MessageBox.Show("This module requires 1 device to be selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    this.xboxDevice = selectedDevices[0] as IXboxDevice;
                    if (this.xboxDevice == null)
                    {
                        MessageBox.Show("A Xbox must be selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else if (!this.xboxDevice.Connected)
                    {
                        MessageBox.Show("A connected Xbox is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                string title = string.Empty;
                if (!string.IsNullOrEmpty(this.moduleContext.XboxTitle.Name))
                {
                    title = this.moduleContext.XboxTitle.Name;
                }

                if (string.IsNullOrEmpty(title))
                {
                    MessageBox.Show("Please select a title from the setup dialog", "Certification Assistance Tool");
                    return;
                }

                if (!this.IsControllerConnected)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("No controller is attached to the PC.  Are you sure you want to continue?", "Certification Assistance Tool", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                this.Setup();

                // go modal and show setup selection page
                this.moduleContext.IsModal = true;
                this.FirstPageVisibility = Visibility.Collapsed;
                this.SecondPageVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Send the desired controller signal to every console
        /// </summary>
        /// <param name="buttons">Button pressed</param>
        public void SendControllerButton(XboxAutomationButtonFlags buttons)
        {
            // check if we have a quadrant already set up. quadrant will be 1, 2, 3 or 4
            if (this.controlledQuadrant != 0)
            {
                this.gamepad.LeftThumbX = 0;
                this.gamepad.LeftThumbY = 0;
                this.gamepad.LeftTrigger = 0;
                this.gamepad.RightThumbX = 0;
                this.gamepad.RightThumbY = 0;
                this.gamepad.RightTrigger = 0;
                this.gamepad.Buttons = buttons;

                try
                {
                    this.xboxDevice.XboxConsole.XboxAutomation.QueueGamepadState(this.controlledQuadrant - 1, this.gamepad, 200, 0);
                    this.gamepad.Buttons = new XboxAutomationButtonFlags();
                    this.xboxDevice.XboxConsole.XboxAutomation.QueueGamepadState(this.controlledQuadrant - 1, this.gamepad, 100, 0);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Send a left trigger
        /// </summary>
        public void SendControllerLeftTrigger()
        {
            // check if we have a quadrant already set up. quadrant will be 1, 2, 3 or 4
            if (this.controlledQuadrant != 0)
            {
                this.gamepad.LeftThumbX = 0;
                this.gamepad.LeftThumbY = 0;
                this.gamepad.RightThumbX = 0;
                this.gamepad.RightThumbY = 0;
                this.gamepad.RightTrigger = 0;
                this.gamepad.Buttons = 0;

                this.gamepad.LeftTrigger = byte.MaxValue;

                try
                {
                    this.xboxDevice.XboxConsole.XboxAutomation.QueueGamepadState(this.controlledQuadrant - 1, this.gamepad, 200, 0);
                    this.gamepad.Buttons = new XboxAutomationButtonFlags();
                    this.xboxDevice.XboxConsole.XboxAutomation.QueueGamepadState(this.controlledQuadrant - 1, this.gamepad, 100, 0);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Send a right trigger
        /// </summary>
        public void SendControllerRightTrigger()
        {
            // check if we have a quadrant already set up. quadrant will be 1, 2, 3 or 4
            if (this.controlledQuadrant != 0)
            {
                this.gamepad.LeftThumbX = 0;
                this.gamepad.LeftThumbY = 0;
                this.gamepad.RightThumbX = 0;
                this.gamepad.RightThumbY = 0;
                this.gamepad.LeftTrigger = 0;
                this.gamepad.Buttons = 0;

                this.gamepad.RightTrigger = byte.MaxValue;

                try
                {
                    this.xboxDevice.XboxConsole.XboxAutomation.QueueGamepadState(this.controlledQuadrant - 1, this.gamepad, 200, 0);
                    this.gamepad.Buttons = new XboxAutomationButtonFlags();
                    this.xboxDevice.XboxConsole.XboxAutomation.QueueGamepadState(this.controlledQuadrant - 1, this.gamepad, 100, 0);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Indicates that a test step has passed on the specified quadrant
        /// </summary>
        /// <param name="quadrant">Quadrant associated with the pass</param>
        public void PassStep(uint quadrant)
        {
            this.moduleContext.Log("Test PASS on quadrant " + quadrant);
            switch (quadrant)
            {
                case 1:
                    this.Quadrant1NotDone = false;
                    break;
                case 2:
                    this.Quadrant2NotDone = false;
                    break;
                case 3:
                    this.Quadrant3NotDone = false;
                    break;
                case 4:
                    this.Quadrant4NotDone = false;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Indicates that a test step has failed on the specified quadrant
        /// </summary>
        /// <param name="quadrant">Quadrant associated with the fail</param>
        public void FailStep(uint quadrant)
        {
            this.passedOrFailed = "FAILED";
            this.moduleContext.Log("Test FAIL on quadrant " + quadrant);
            switch (quadrant)
            {
                case 1:
                    this.Quadrant1NotDone = false;
                    break;
                case 2:
                    this.Quadrant2NotDone = false;
                    break;
                case 3:
                    this.Quadrant3NotDone = false;
                    break;
                case 4:
                    this.Quadrant4NotDone = false;
                    break;
                default:
                    break;
            }
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
        /// install title to the consoles
        /// </summary>
        private void Setup()
        {
            System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            ConsoleProfilesManager profilesManager = this.xboxDevice.XboxConsole.CreateConsoleProfilesManager();
            IEnumerable<ConsoleProfile> profiles = profilesManager.EnumerateConsoleProfiles();
            if (profiles.Count() == 0)
            {
                ConsoleProfile profile = profilesManager.CreateConsoleProfile(true);
                profile.SignIn(UserIndex.Zero);
            }
            else
            {
                profilesManager.SignOutAllUsers();
                ConsoleProfile firstProfile = profiles.First();
                firstProfile.SignIn(UserIndex.Zero);
            }

            System.Windows.Input.Mouse.OverrideCursor = null;

            if (this.xboxDevice.IsTitleInstalled)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Launch " + this.moduleContext.XboxTitle.Name + "?", "Certification Assistance Tool", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    this.xboxDevice.LaunchTitle();
                }
            }
            else
            {
                if (this.xboxDevice.CanInstallTitle)
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("Install and launch " + this.moduleContext.XboxTitle.Name + "?", "Certification Assistance Tool", MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        this.xboxDevice.InstallTitle(this.GetBestAvailableDrive(this.xboxDevice), this.moduleContext.OpenProgressBarWindow("Installing " + this.moduleContext.XboxTitle.Name + "..."));
                        this.xboxDevice.LaunchTitle();
                    }
                }
            }

            this.xboxDevice.XboxConsole.XboxAutomation.BindController(0, 10);
            this.xboxDevice.XboxConsole.XboxAutomation.BindController(1, 10);
            this.xboxDevice.XboxConsole.XboxAutomation.BindController(2, 10);
            this.xboxDevice.XboxConsole.XboxAutomation.BindController(3, 10);
            this.controllersBound = true;
            
            this.xboxDevice.XboxConsole.XboxAutomation.ClearGamepadQueue(0);
            this.xboxDevice.XboxConsole.XboxAutomation.ClearGamepadQueue(1);
            this.xboxDevice.XboxConsole.XboxAutomation.ClearGamepadQueue(2);
            this.xboxDevice.XboxConsole.XboxAutomation.ClearGamepadQueue(3);

            this.xboxDevice.XboxConsole.XboxAutomation.ConnectController(0);
            this.xboxDevice.XboxConsole.XboxAutomation.ConnectController(1);
            this.xboxDevice.XboxConsole.XboxAutomation.ConnectController(2);
            this.xboxDevice.XboxConsole.XboxAutomation.ConnectController(3);

            this.Quadrant1Connected = true;
            this.Quadrant2Connected = true;
            this.Quadrant3Connected = true;
            this.Quadrant4Connected = true;
            this.Quadrant1Controlled = true;

            this.VirtualControllerVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Connects the specified quadrant
        /// </summary>
        /// <param name="quadrant">Quadrant to connect</param>
        private void ConnectQuadrant(uint quadrant)
        {
            this.xboxDevice.XboxConsole.XboxAutomation.ConnectController(quadrant - 1);
        }

        /// <summary>
        /// Disconnects the specified quadrant
        /// </summary>
        /// <param name="quadrant">Quadrant to disconnect</param>
        private void DisconnectQuadrant(uint quadrant)
        {
            if (this.controlledQuadrant == quadrant)
            {
                this.controlledQuadrant = 0;
                this.NotifyPropertyChanged("AnyQuadrantControlled");
            }

            this.xboxDevice.XboxConsole.XboxAutomation.DisconnectController(quadrant - 1);
        }

        /// <summary>
        /// PollController - ask controller for its state every time the timer 'ticks'
        /// </summary>
        /// <param name="obj">Sending object source as another control or event</param>
        /// <param name="args">Applicable event processing args, if any</param>
        private void PollController(object obj, EventArgs args)
        {
            XBOX_AUTOMATION_GAMEPAD pad = new XBOX_AUTOMATION_GAMEPAD();
            GamePadState state = GamePad.GetState(PlayerIndex.One);

            this.IsControllerConnected = state.IsConnected;
            if (!state.IsConnected)
            {
                return;
            }

            // convert button presses from GamePadState to XBOX_AUTOMATION_GAMEPAD
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

            // convert dpad presses from GamePadState to XBOX_AUTOMATION_GAMEPAD
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
            // convert movement from GamePadState to XBOX_AUTOMATION_GAMEPAD
            pad.LeftThumbX = (int)((double)state.ThumbSticks.Left.X * short.MaxValue);
            pad.LeftThumbY = (int)((double)state.ThumbSticks.Left.Y * short.MaxValue);

            pad.RightThumbX = (int)((double)state.ThumbSticks.Right.X * short.MaxValue);
            pad.RightThumbY = (int)((double)state.ThumbSticks.Right.Y * short.MaxValue);

            pad.LeftTrigger = (uint)((double)state.Triggers.Left * byte.MaxValue);
            pad.RightTrigger = (uint)((double)state.Triggers.Right * byte.MaxValue);

            if (this.controlledQuadrant != 0)
            {
                try
                {
                    this.xboxDevice.XboxConsole.XboxAutomation.SetGamepadState(this.controlledQuadrant - 1, pad);
                }
                catch (Exception ex)
                {
                    this.moduleContext.Log("Exception calling SetGamepadState: " + ex.Message);
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
    } 
}
