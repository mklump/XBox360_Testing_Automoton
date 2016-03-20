// -----------------------------------------------------------------------
// <copyright file="VirtualControllerViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Input;
    using XDevkit;

    /// <summary>
    /// View Model class for ProgressBarWindow
    /// </summary>
    public class VirtualControllerViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// A reference to the main view model
        /// </summary>
        private readonly MainViewModel mainViewModel;

        /// <summary>
        /// Gets or sets the virtual controller window
        /// </summary>
        private VirtualControllerWindow virtualControllerWindow;

        /// <summary>
        /// Backing field for AllDevices property
        /// </summary>
        private ObservableCollection<ControllableXboxViewItem> allDevices = new ObservableCollection<ControllableXboxViewItem>();

        /// <summary>
        /// Backing field for IsControllerConnected property
        /// </summary>
        private bool isControllerConnected;

        /// <summary>
        /// Timer for polling PC controller state
        /// </summary>
        private Timer controllerTimer;

        /// <summary>
        /// A value indicating whether the virtual controller window has been closed
        /// </summary>
        private bool closed;

        /// <summary>
        /// A value indicating whether we are currently handling a virtual key press
        /// </summary>
        private bool handlingVirtualPress;

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualControllerViewModel" /> class.
        /// </summary>
        /// <param name="mainViewModel">A reference to the main view model</param>
        public VirtualControllerViewModel(MainViewModel mainViewModel)
        {
            // Initialize member data
            this.mainViewModel = mainViewModel;

            foreach (XboxViewItem xboxViewItem in this.mainViewModel.XboxList)
            {
                if (xboxViewItem.ConnectedAndResponding)
                {
                    ControllableXboxViewItem controllableXboxViewItem = new ControllableXboxViewItem(this, xboxViewItem);
                    this.allDevices.Add(controllableXboxViewItem);
                }
            }

            this.virtualControllerWindow = new VirtualControllerWindow(this);
            this.virtualControllerWindow.Closing += this.OnWindowClosing;
            this.virtualControllerWindow.Show();

            this.controllerTimer = new Timer(_ => this.ControllerTimerExpired(), null, TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a reference to the MainViewModel
        /// </summary>
        public MainViewModel MainViewModel
        {
            get { return this.mainViewModel; }
        }

        /// <summary>
        /// Gets a reference to the current Theme
        /// </summary>
        public Theme CurrentTheme
        {
            get { return MainViewModel.CurrentTheme; }
        }

        /// <summary>
        /// Gets or sets a list of all Xbox's
        /// </summary>
        public ObservableCollection<ControllableXboxViewItem> AllDevices
        {
            get
            {
                return this.allDevices;
            }

            set
            {
                this.allDevices = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a controller is connected to the PC
        /// </summary>
        public bool IsControllerConnected
        {
            get
            {
                return this.isControllerConnected;
            }

            set
            {
                this.isControllerConnected = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Sets a controller button as down (only 1 button can be down at a time using the virtual controller)
        /// </summary>
        /// <param name="buttons">Button(s) to press</param>
        public void SetControllerButtonDown(XboxAutomationButtonFlags buttons)
        {
            lock (this)
            {
                this.handlingVirtualPress = true;
                XBOX_AUTOMATION_GAMEPAD pad = new XBOX_AUTOMATION_GAMEPAD();
                pad.Buttons = buttons;
                this.SetGamepadStates(pad);
            }
        }

        /// <summary>
        /// Release all controller buttons
        /// </summary>
        public void ReleaseControllerButton()
        {
            lock (this)
            {
                if (this.handlingVirtualPress)
                {
                    this.handlingVirtualPress = false;
                    XBOX_AUTOMATION_GAMEPAD pad = new XBOX_AUTOMATION_GAMEPAD();
                    this.SetGamepadStates(pad);
                }
            }
        }

        /// <summary>
        /// Send a left trigger
        /// </summary>
        public void PressLeftTrigger()
        {
            lock (this)
            {
                this.handlingVirtualPress = true;
                XBOX_AUTOMATION_GAMEPAD pad = new XBOX_AUTOMATION_GAMEPAD();
                pad.LeftTrigger = byte.MaxValue;
                this.SetGamepadStates(pad);
            }
        }

        /// <summary>
        /// Send a right trigger
        /// </summary>
        public void PressRightTrigger()
        {
            lock (this)
            {
                this.handlingVirtualPress = true;
                XBOX_AUTOMATION_GAMEPAD pad = new XBOX_AUTOMATION_GAMEPAD();
                pad.RightTrigger = byte.MaxValue;
                this.SetGamepadStates(pad);
            }
        }

        /// <summary>
        /// An event handler called when the window closes.
        /// </summary>
        /// <param name="sender">Originator of the event</param>
        /// <param name="e">An instance of CancelEventArgs</param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            this.mainViewModel.VirtualControllerViewModel = null;

            foreach (ControllableXboxViewItem controllableXboxViewItem in this.AllDevices)
            {
                if (controllableXboxViewItem.Quadrant1Selected)
                {
                    this.UnregisterController(controllableXboxViewItem.XboxViewItem, 0);
                }
                else if (controllableXboxViewItem.Quadrant2Selected)
                {
                    this.UnregisterController(controllableXboxViewItem.XboxViewItem, 1);
                }
                else if (controllableXboxViewItem.Quadrant3Selected)
                {
                    this.UnregisterController(controllableXboxViewItem.XboxViewItem, 2);
                }
                else if (controllableXboxViewItem.Quadrant4Selected)
                {
                    this.UnregisterController(controllableXboxViewItem.XboxViewItem, 3);
                }
            }

            this.closed = true;
        }

        /// <summary>
        /// Close the profile manager
        /// </summary>
        public void Close()
        {
            this.virtualControllerWindow.Close();
        }

        /// <summary>
        /// Show the profile manager, if hidden
        /// </summary>
        public void Show()
        {
            this.virtualControllerWindow.Show();
        }

        /// <summary>
        /// Hide the profile manager
        /// </summary>
        public void Hide()
        {
            this.virtualControllerWindow.Hide();
        }
        
        /// <summary>
        /// Activate the profile manager
        /// </summary>
        public void Activate()
        {
            this.virtualControllerWindow.Activate();
        }

        /// <summary>
        /// Selects the current Xbox
        /// </summary>
        /// <param name="xboxViewItem">Xbox to select</param>
        public void SelectXbox(XboxViewItem xboxViewItem)
        {
            foreach (ControllableXboxViewItem controllableXboxViewItem in this.AllDevices)
            {
                controllableXboxViewItem.IsSelected = controllableXboxViewItem.XboxViewItem == xboxViewItem;
            }
        }

        /// <summary>
        /// Sets the gamepad states of all controlled Xbox's
        /// </summary>
        /// <param name="pad">Gamepad state to set</param>
        private void SetGamepadStates(XBOX_AUTOMATION_GAMEPAD pad)
        {
            foreach (ControllableXboxViewItem controllableXboxViewItem in this.AllDevices)
            {
                try
                {
                    if (controllableXboxViewItem.Quadrant1Selected)
                    {
                        controllableXboxViewItem.XboxViewItem.XboxDevice.XboxConsole.XboxAutomation.SetGamepadState(0, pad);
                    }
                    else if (controllableXboxViewItem.Quadrant2Selected)
                    {
                        controllableXboxViewItem.XboxViewItem.XboxDevice.XboxConsole.XboxAutomation.SetGamepadState(1, pad);
                    }
                    else if (controllableXboxViewItem.Quadrant3Selected)
                    {
                        controllableXboxViewItem.XboxViewItem.XboxDevice.XboxConsole.XboxAutomation.SetGamepadState(2, pad);
                    }
                    else if (controllableXboxViewItem.Quadrant4Selected)
                    {
                        controllableXboxViewItem.XboxViewItem.XboxDevice.XboxConsole.XboxAutomation.SetGamepadState(3, pad);
                    }
                }
                catch
                {
                    // Ignore failure
                }
            }
        }

        /// <summary>
        /// Timer expiration for polling PC controller state
        /// </summary>
        private void ControllerTimerExpired()
        {
            if (this.closed)
            {
                // Short circuit the polling timer
                return;
            }

            GamePadState state = GamePad.GetState(PlayerIndex.One);
            this.IsControllerConnected = state.IsConnected;
            if (!state.IsConnected)
            {
                // Poll less frequently, for controller to become attached
                this.controllerTimer.Change(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(-1));
                return;
            }

            lock (this)
            {
                if (this.handlingVirtualPress == false)
                {
                    // convert button presses from GamePadState to XBOX_AUTOMATION_GAMEPAD
                    XBOX_AUTOMATION_GAMEPAD pad = new XBOX_AUTOMATION_GAMEPAD();

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
                    pad.LeftThumbX = (int)((double)state.ThumbSticks.Left.X * short.MaxValue);
                    pad.LeftThumbY = (int)((double)state.ThumbSticks.Left.Y * short.MaxValue);

                    pad.RightThumbX = (int)((double)state.ThumbSticks.Right.X * short.MaxValue);
                    pad.RightThumbY = (int)((double)state.ThumbSticks.Right.Y * short.MaxValue);

                    pad.LeftTrigger = (uint)((double)state.Triggers.Left * byte.MaxValue);
                    pad.RightTrigger = (uint)((double)state.Triggers.Right * byte.MaxValue);

                    this.SetGamepadStates(pad);
                }
            }

            this.controllerTimer.Change(TimeSpan.FromMilliseconds(10), TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// Binds a controller
        /// </summary>
        /// <param name="xboxViewItem">Xbox to bind a controller on</param>
        /// <param name="quadrant">Quadrant to bind controller to</param>
        private void RegisterController(XboxViewItem xboxViewItem, uint quadrant)
        {
            try
            {
                xboxViewItem.XboxDevice.XboxConsole.XboxAutomation.BindController(quadrant, 999);
                xboxViewItem.XboxDevice.XboxConsole.XboxAutomation.ConnectController(quadrant);
            }
            catch
            {
                // Ignore failure
            }
        }

        /// <summary>
        /// Unbinds a controller
        /// </summary>
        /// <param name="xboxViewItem">Xbox to unbind a controller on</param>
        /// <param name="quadrant">Quadrant to unbind controller from</param>
        private void UnregisterController(XboxViewItem xboxViewItem, uint quadrant)
        {
            try
            {
                xboxViewItem.XboxDevice.XboxConsole.XboxAutomation.UnbindController(quadrant);
            }
            catch
            {
                // Ignore failure
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
        /// A class representing a controllable Xbox
        /// </summary>
        public class ControllableXboxViewItem : INotifyPropertyChanged
        {
            /// <summary>
            /// Reference to the associated XboxViewItem
            /// </summary>
            private VirtualControllerViewModel virtualControllerViewModel;

            /// <summary>
            /// Reference to the associated XboxViewItem
            /// </summary>
            private XboxViewItem xboxViewItem;

            /// <summary>
            /// Backing field for IsSelected property
            /// </summary>
            private bool isSelected;

            /// <summary>
            /// Backing field for Quadrant1Selected property
            /// </summary>
            private bool quadrant1Selected;

            /// <summary>
            /// Backing field for Quadrant1Selected property
            /// </summary>
            private bool quadrant2Selected;

            /// <summary>
            /// Backing field for Quadrant1Selected property
            /// </summary>
            private bool quadrant3Selected;

            /// <summary>
            /// Backing field for Quadrant1Selected property
            /// </summary>
            private bool quadrant4Selected;

            /// <summary>
            /// Initializes a new instance of the <see cref="ControllableXboxViewItem" /> class.
            /// </summary>
            /// <param name="virtualControllerViewModel">A reference to the virtual controller view model</param>
            /// <param name="xboxViewItem">Xbox to associate with this object</param>
            public ControllableXboxViewItem(VirtualControllerViewModel virtualControllerViewModel, XboxViewItem xboxViewItem)
            {
                this.virtualControllerViewModel = virtualControllerViewModel;
                this.xboxViewItem = xboxViewItem;
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets the name of this Xbox
            /// </summary>
            public string Name
            {
                get { return this.xboxViewItem.XboxDevice.Name; }
            }

            /// <summary>
            /// Gets a sets the associated XboxViewItem
            /// </summary>
            public XboxViewItem XboxViewItem
            {
                get { return this.xboxViewItem; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this item is selected
            /// </summary>
            public bool IsSelected
            {
                get
                {
                    return this.isSelected;
                }

                set
                {
                    if (this.isSelected != value)
                    {
                        if (value)
                        {
                            this.Quadrant1Selected = true;
                            this.Quadrant2Selected = false;
                            this.Quadrant3Selected = false;
                            this.Quadrant4Selected = false;
                        }
                        else
                        {
                            this.Quadrant1Selected = false;
                            this.Quadrant2Selected = false;
                            this.Quadrant3Selected = false;
                            this.Quadrant4Selected = false;
                        }

                        this.isSelected = value;
                        this.NotifyPropertyChanged();
                    }
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether quadrant 1 is selected
            /// </summary>
            public bool Quadrant1Selected
            {
                get
                {
                    return this.quadrant1Selected;
                }

                set
                {
                    if (this.quadrant1Selected != value)
                    {
                        this.quadrant1Selected = value;
                        this.NotifyPropertyChanged();
                        if (value == true)
                        {
                            this.Quadrant2Selected = false;
                            this.Quadrant3Selected = false;
                            this.Quadrant4Selected = false;
                            this.virtualControllerViewModel.RegisterController(this.xboxViewItem, 0);
                        }
                        else
                        {
                            this.virtualControllerViewModel.UnregisterController(this.xboxViewItem, 0);
                        }
                    }
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether quadrant 2 is selected
            /// </summary>
            public bool Quadrant2Selected
            {
                get
                {
                    return this.quadrant2Selected;
                }

                set
                {
                    if (this.quadrant2Selected != value)
                    {
                        this.quadrant2Selected = value;
                        this.NotifyPropertyChanged();
                        if (value == true)
                        {
                            this.Quadrant1Selected = false;
                            this.Quadrant3Selected = false;
                            this.Quadrant4Selected = false;
                            this.virtualControllerViewModel.RegisterController(this.xboxViewItem, 1);
                        }
                        else
                        {
                            this.virtualControllerViewModel.UnregisterController(this.xboxViewItem, 1);
                        }
                    }
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether quadrant 3 is selected
            /// </summary>
            public bool Quadrant3Selected
            {
                get
                {
                    return this.quadrant3Selected;
                }

                set
                {
                    if (this.quadrant3Selected != value)
                    {
                        this.quadrant3Selected = value;
                        this.NotifyPropertyChanged();
                        if (value == true)
                        {
                            this.Quadrant1Selected = false;
                            this.Quadrant2Selected = false;
                            this.Quadrant4Selected = false;
                            this.virtualControllerViewModel.RegisterController(this.xboxViewItem, 2);
                        }
                        else
                        {
                            this.virtualControllerViewModel.UnregisterController(this.xboxViewItem, 2);
                        }
                    }
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether quadrant 4 is selected
            /// </summary>
            public bool Quadrant4Selected
            {
                get
                {
                    return this.quadrant4Selected;
                }

                set
                {
                    if (this.quadrant4Selected != value)
                    {
                        this.quadrant4Selected = value;
                        this.NotifyPropertyChanged();
                        if (value == true)
                        {
                            this.Quadrant1Selected = false;
                            this.Quadrant2Selected = false;
                            this.Quadrant3Selected = false;
                            this.virtualControllerViewModel.RegisterController(this.xboxViewItem, 3);
                        }
                        else
                        {
                            this.virtualControllerViewModel.UnregisterController(this.xboxViewItem, 3);
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
        }
    }
}
