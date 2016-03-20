// -----------------------------------------------------------------------
// <copyright file="BAS014.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BAS014
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Interaction logic for BAS014 XAML file
    /// </summary>
    public partial class BAS014UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the BAS014UI class
        /// </summary>
        /// <param name="module">BAS014 module instance for which to construct</param>
        public BAS014UI(BAS014 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// Scroll to bottom event handler.
        /// </summary>
        /// <param name="sender">Control sending the event</param>
        /// <param name="e">Applicable text-changed event handler arguments</param>
        private void ScrollToBottom(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.ScrollToEnd();
        }

        /// <summary>
        /// Open Debug Output window event handler
        /// </summary>
        /// <param name="sender">Control sending the event</param>
        /// <param name="e">Applicable Routed Event arguments for the event</param>
        private void OpenDebugOutput(object sender, RoutedEventArgs e)
        {
            BAS014 module = this.DataContext as BAS014;
            module.OpenDebugOutput();
        }
    }
}
