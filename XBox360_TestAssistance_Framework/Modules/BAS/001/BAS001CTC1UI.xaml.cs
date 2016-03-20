// -----------------------------------------------------------------------
// <copyright file="BAS001CTC1UI.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BAS001
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Implementation class for the BAS001CTC1UI user interface
    /// </summary>
    public partial class BAS001CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the BAS001CTC1UI class
        /// </summary>
        /// <param name="module">BAS001CTC1 module class object to initialize</param>
        public BAS001CTC1UI(BAS001CTC1 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// Event handler function for Open Debug Output button click
        /// </summary>
        /// <param name="sender">Control sending the activation event</param>
        /// <param name="e">Routed event args., if any, required for the event</param>
        private void OpenDebugOutput(object sender, RoutedEventArgs e)
        {
            BAS001CTC1 module = this.DataContext as BAS001CTC1;
            module.OpenDebugOutput();
        }
    } // End of: public partial class BAS001CTC1UI : Grid
} // End of: namespace BAS001