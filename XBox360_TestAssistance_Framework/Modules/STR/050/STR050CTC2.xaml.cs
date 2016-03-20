// -----------------------------------------------------------------------
// <copyright file="STR050CTC2.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace STR050
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Interaction logic for STR050CTC2 user interface
    /// </summary>
    public partial class STR050UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the STR050UI class
        /// Constructor for STR050CTC2 to set this instance
        /// </summary>
        /// <param name="module">Instance of STR050CTC2 for which to set to this instance.</param>
        public STR050UI(STR050CTC2 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// StartTest button first click event handler
        /// </summary>
        /// <param name="sender">Sender of the object.</param>
        /// <param name="e">Any required associated arguments.</param>
        private void NextPage(object sender, RoutedEventArgs e)
        {
            STR050CTC2 module = DataContext as STR050CTC2;
            module.NextPage();
        }

        /// <summary>
        /// Click event handler for Disable Internal MU button
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event arguments</param>
        private void DisableInternalMU_Click(object sender, RoutedEventArgs e)
        {
            STR050CTC2 module = DataContext as STR050CTC2;
            module.IsMUINTEnabled = false;
        }

        /// <summary>
        /// Click event handler for Enable Internal MU button
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event arguments</param>
        private void EnableInternalMU_Click(object sender, RoutedEventArgs e)
        {
            STR050CTC2 module = DataContext as STR050CTC2;
            module.IsMUINTEnabled = true;
        }

        /// <summary>
        /// Click event handler for Enable Hard Disk Drive button
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event arguments</param>
        private void EnableHDD(object sender, RoutedEventArgs e)
        {
            STR050CTC2 module = DataContext as STR050CTC2;
            module.IsHDDEnabled = true;
        }

        /// <summary>
        /// Click event handler for Disable Hard Disk Drive button
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event arguments</param>
        private void DisableHDD(object sender, RoutedEventArgs e)
        {
            STR050CTC2 module = DataContext as STR050CTC2;
            module.IsHDDEnabled = false;
        }

        /// <summary>
        /// Click event handler for Begin button
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event arguments</param>
        private void Begin_Click(object sender, RoutedEventArgs e)
        {
            STR050CTC2 module = DataContext as STR050CTC2;
            module.BeginLowStorage();
        }

        /// <summary>
        /// Click event handler for Next Low Storage button
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event arguments</param>
        private void Next_Low_Storage_Click(object sender, RoutedEventArgs e)
        {
            STR050CTC2 module = DataContext as STR050CTC2;
            module.NextLowStorage();
        }

        /// <summary>
        /// Click event handler for External USB Only button
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event arguments</param>
        private void External_USB_Only_Click(object sender, RoutedEventArgs e)
        {
            STR050CTC2 module = DataContext as STR050CTC2;
            module.SetupExternalUSBOnly();
        }

        /// <summary>
        /// Click event handler for External USB Only button
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event arguments</param>
        private void External_MU_Only_Click(object sender, RoutedEventArgs e)
        {
            STR050CTC2 module = DataContext as STR050CTC2;
            module.SetupExternalMUOnly();
        }

        /// <summary>
        /// Click event handler for Refresh button
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event arguments</param>
        private void Refresh(object sender, RoutedEventArgs e)
        {
            STR050CTC2 module = DataContext as STR050CTC2;
            module.RefreshStates();
        }
    }
}
