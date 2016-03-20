// -----------------------------------------------------------------------
// <copyright file="BAS001CTC1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BAS001
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using CAT;

    /// <summary>
    /// Module BAS001CTC1
    /// </summary>
    public class BAS001CTC1 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// Current working module context, inherited from IModule, for this module
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// UIElement control for this module
        /// </summary>
        private BAS001CTC1UI moduleUI;

        /// <summary>
        /// Event declaration for the property changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the UIElement control for this module
        /// </summary>
        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
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
            this.moduleUI = new BAS001CTC1UI(this);
        }

        /// <summary>
        /// OpenDebugOutput - called to leave the the module overview or intro screen entered by Start(IModuleContext context)
        /// The framework goes modal in this call and the module gains control.
        /// This function is called repeatedly to advance to multiple screens in the module.
        /// </summary>
        public void OpenDebugOutput()
        {
            List<IDevice> selectedDevices = this.moduleContext.SelectedDevices;
            if (selectedDevices.Count != 1)
            {
                MessageBox.Show("This module requires 1 device to be selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                IXboxDevice xb = selectedDevices[0] as IXboxDevice;
                if (xb == null)
                {
                    MessageBox.Show("A Xbox must be selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    if (!xb.Connected)
                    {
                        MessageBox.Show("A connected Xbox is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if (!xb.CanDebug)
                    {
                        MessageBox.Show("A Xbox with debugging capability is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        this.moduleContext.OpenDebugOutput(xb);
                    }
                }
            }
        }

        /// <summary>
        /// Stop - called when the module is done or aborted
        /// </summary>
        public void Stop()
        {
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
    } // End of: public class BAS001CTC1Module : IModule, INotifyPropertyChanged
} // End of: namespace BAS001 in code file BAS001CTC1.cs