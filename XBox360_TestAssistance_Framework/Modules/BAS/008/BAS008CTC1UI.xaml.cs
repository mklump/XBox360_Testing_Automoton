// -----------------------------------------------------------------------
// <copyright file="BAS008CTC1UI.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BAS008
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Implementation class for the BAS008CTC1UI user interface
    /// </summary>
    public partial class BAS008CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the BAS008CTC1UI class
        /// </summary>
        /// <param name="module">BAS008CTC1 module class object to initialize</param>
        public BAS008CTC1UI(BAS008CTC1 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// Event handler function for Next Page button click
        /// </summary>
        /// <param name="sender">Control sending the activation event</param>
        /// <param name="e">Routed event args., if any, required for the event</param>
        private void Begin(object sender, RoutedEventArgs e)
        {
            BAS008CTC1 module = this.DataContext as BAS008CTC1;
            module.Begin();
        }
    } // End of: public partial class BAS008CTC1UI : Grid
} // End of: namespace BAS008