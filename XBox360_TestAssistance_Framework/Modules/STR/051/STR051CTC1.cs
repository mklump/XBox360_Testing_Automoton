// -----------------------------------------------------------------------
// <copyright file="STR051CTC1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace STR051
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Threading;
    using CAT;

    /// <summary>
    /// STR 051 CTC1 Module
    /// </summary>
    public class STR051CTC1 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// Backing field for Instructions property
        /// </summary>
        private readonly string instructions = "Select the type of test for instructions.";

        /// <summary>
        /// Private GP070CTC1UI user interface property for this user interface component
        /// </summary>
        private STR051CTC1UI moduleUI;

        /// <summary>
        /// Current working module context, inherited from IModule, for this module
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// Target xbox
        /// </summary>
        private IXboxDevice xboxDevice;

        /// <summary>
        /// Backing field for FirstPageVisibility property
        /// </summary>
        private Visibility firstPageVisibility = Visibility.Visible;

        /// <summary>
        /// Backing field for SecondPageVisibility property
        /// </summary>
        private Visibility secondPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for ConfigureEnabledVisibility property
        /// </summary>
        private Visibility configureEnabledVisibility = Visibility.Visible;

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
        /// Gets the instructions displayed in the UI
        /// </summary>
        public string Instructions 
        {
            get { return this.instructions; } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the first page is visible
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
        /// Gets or sets a value indicating whether or not the second page is visible
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
        /// Gets or sets a value indicating whether or not the auto-configure UI element is visible
        /// </summary>
        public Visibility ConfigureEnabledVisibility 
        {
            get 
            {
                return this.configureEnabledVisibility; 
            }

            set 
            {
                this.configureEnabledVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the HDD is currently enabled
        /// </summary>
        public bool IsHDDEnabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsHDDEnabled;
                }
                
                return result;
            }

            set
            {
                if (this.xboxDevice != null && this.xboxDevice.IsHDDEnabled != value)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.xboxDevice.SetHDDEnabled(value, true, false);
                    this.RefreshStates();
                    if (value)
                    {
                        this.moduleContext.Log("Enabled Hard Drive");
                    }
                    else
                    {
                        this.moduleContext.Log("Disabled Hard Drive");
                    }

                    Mouse.OverrideCursor = null;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not MU0 is currently enabled
        /// </summary>
        public bool IsMU0Enabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsMU0Enabled;
                }
                
                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not MU1 is currently enabled
        /// </summary>
        public bool IsMU1Enabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsMU1Enabled;
                }
                
                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the Internal USB storage device is currently enabled
        /// </summary>
        public bool IsINTUSBEnabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsINTUSBEnabled;
                }
                
                return result;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the Internal MU is currently enabled
        /// </summary>
        public bool IsMUINTEnabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsMUINTEnabled;
                }
                
                return result;
            }

            set
            {
                if (this.xboxDevice != null && this.xboxDevice.IsMUINTEnabled != value)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.xboxDevice.IsMUINTEnabled = value;
                    this.RefreshStates();
                    if (value)
                    {
                        this.moduleContext.Log("Enabled Internal MU");
                    }
                    else
                    {
                        this.moduleContext.Log("Disabled Internal MU");
                    }

                    Mouse.OverrideCursor = null;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not USB0 is currently enabled
        /// </summary>
        public bool IsUSB0Enabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsUSB0Enabled;
                }
                
                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not USB1 is currently enabled
        /// </summary>
        public bool IsUSB1Enabled
        {
            get
            {
                bool result = false;
                if (this.xboxDevice != null)
                {
                    result = this.xboxDevice.IsUSB1Enabled;
                }
                
                return result;
            }
        }

        /// <summary>
        /// Gets a value indicating whether there is only one xbox selected
        /// </summary>
        private bool IsOneConnectedXboxSelected
        {
            get
            {
                string s = string.Empty;
                if (this.moduleContext.SelectedDevices.Count() == 0)
                {
                    s += "No consoles are selected. Select one. ";
                }
                else if (this.moduleContext.SelectedDevices.Count() > 1)
                {
                    s += this.moduleContext.SelectedDevices.Count().ToString() + " consoles are selected. Select just one. ";
                }

                foreach (IXboxDevice device in this.moduleContext.SelectedDevices)
                {
                    if (device.IsSelected)
                    {
                        // connected
                        if (!device.Connected)
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
        /// Gets the first found to be selected XBox device for flashing, and places it in the
        /// xboxDevice data member.
        /// </summary>
        private IXboxDevice SelectedXbox
        {
            get
            {
                List<IDevice> xboxDevices = this.moduleContext.SelectedDevices;
                foreach (IDevice xboxdevice in xboxDevices)
                {
                    if (xboxdevice.IsSelected)
                    {
                        this.xboxDevice = xboxdevice as IXboxDevice;
                        return this.xboxDevice;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Refresh various properties bound by UI elements
        /// </summary>
        public void RefreshStates()
        {
            this.NotifyPropertyChanged("Instructions");
            this.NotifyPropertyChanged("IsHDDEnabled");
            this.NotifyPropertyChanged("IsMU0Enabled");
            this.NotifyPropertyChanged("IsMU1Enabled");
            this.NotifyPropertyChanged("IsINTUSBEnabled");
            this.NotifyPropertyChanged("IsMUINTEnabled");
            this.NotifyPropertyChanged("IsUSB0Enabled");
            this.NotifyPropertyChanged("IsUSB1Enabled");

            // log the state
            string msg = "Current Drives: HDD=";
            msg += this.IsHDDEnabled ? "Enabled" : "Disabled";
            msg += " MU0=";
            msg += this.IsMU0Enabled ? "Enabled" : "Disabled";
            msg += " MU1=";
            msg += this.IsMU1Enabled ? "Enabled" : "Disabled";
            msg += " USB0=";
            msg += this.IsUSB0Enabled ? "Enabled" : "Disabled";
            msg += " USB1=";
            msg += this.IsUSB1Enabled ? "Enabled" : "Disabled";
            msg += " MUINT=";
            msg += this.IsMUINTEnabled ? "Enabled" : "Disabled";
            msg += " USBINT=";
            msg += this.IsINTUSBEnabled ? "Enabled" : "Disabled";
            this.moduleContext.Log(msg);

            // push UI update
            Dispatcher dispatcher = this.moduleUI.Dispatcher;
            if (dispatcher != null)
            {
                dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);
            }
        }

        /// <summary>
        /// Grabs a screen snapshot
        /// </summary>
        public void ScreenCapture()
        {
            this.moduleContext.Log("Captured screenshot"); // make sure a log file directory exists
            string output = this.moduleContext.ScreenShot(this.SelectedXbox);
            this.moduleContext.Log(output);
            Process.Start(output);
        }

        /// <summary>
        /// Test execute implementation accepted from IModule interface.
        /// This function is responsible for performing the test execution.
        /// </summary>
        /// <param name="ctx">The current working context for which this test will execute.</param>
        public void Start(IModuleContext ctx)
        {
            this.moduleContext = ctx as IXboxModuleContext;
            this.moduleUI = new STR051CTC1UI(this);
        }

        /// <summary>
        /// Advance to the next page
        /// </summary>
        public void NextPage()
        {
            if (this.IsOneConnectedXboxSelected)
            {
                this.xboxDevice = this.SelectedXbox;

                this.FirstPageVisibility = Visibility.Collapsed;
                this.SecondPageVisibility = Visibility.Visible;
                this.moduleContext.IsModal = true;

                this.RefreshStates();
            }
        }

        /// <summary>
        /// Stop - called when the module is done or aborted
        /// </summary>
        public void Stop()
        {
            string passedOrFailed = "FINISHED";
            this.moduleContext.Log("*************************************************************\r\n");
            this.moduleContext.Log("RESULT: " + passedOrFailed + "\r\n");
            this.moduleContext.Log("*************************************************************\r\n");
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
        /// UpdateUIImmediately - update a property at the earliest idle
        /// </summary>
        private void UpdateUIImmediately()
        {
            Dispatcher dispatcher = this.moduleUI.Dispatcher;
            if (dispatcher != null)
            {
                dispatcher.Invoke(new Action(() => { }), DispatcherPriority.ApplicationIdle);
            }
        }
    }
}
