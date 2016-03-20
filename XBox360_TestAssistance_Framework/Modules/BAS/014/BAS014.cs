// -----------------------------------------------------------------------
// <copyright file="BAS014.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BAS014
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using CAT;

    /// <summary>
    /// Module implementation class for 14: BAS Debug Output
    /// </summary>
    public class BAS014 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// Private module context instance for this current working module context
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// Private BAS014UI user interface property for this user interface component
        /// </summary>
        private BAS014UI moduleUI;

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the current working BAS014UI user interface
        /// </summary>
        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
        }

        /// <summary>
        /// Derived class implementation of inherited IModule.Start() declaration.
        /// </summary>
        /// <param name="ctx">Current working Module Context in which this module is working</param>
        public void Start(IModuleContext ctx)
        {
            this.moduleContext = ctx as IXboxModuleContext;
            this.moduleUI = new BAS014UI(this);
        }
        
        /// <summary>
        /// Opens the Debug Output window where this module tool is now placed
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
        /// Derived class implementation of inherited IModule.Stop() declaration.
        /// </summary>
        public void Stop()
        {
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