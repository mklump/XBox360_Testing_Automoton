// -----------------------------------------------------------------------
// <copyright file="STR051CTC1.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace STR051
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for STR051
    /// </summary>
    public partial class STR051CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="STR051CTC1UI" /> class.
        /// </summary>
        /// <param name="module">The module class associated with this UI</param>
        public STR051CTC1UI(STR051CTC1 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// Advance to the next page
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Router event args</param>
        private void NextPage(object sender, RoutedEventArgs e) 
        {
            STR051CTC1 module = this.DataContext as STR051CTC1;
            module.NextPage(); 
        }

        /// <summary>
        /// Disable internal MU
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Router event args</param>
        private void DisableInternalMU_Click(object sender, RoutedEventArgs e) 
        {
            STR051CTC1 module = this.DataContext as STR051CTC1;
            module.IsMUINTEnabled = false; 
        }

        /// <summary>
        /// Enable internal MU
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Router event args</param>
        private void EnableInternalMU_Click(object sender, RoutedEventArgs e) 
        {
            STR051CTC1 module = this.DataContext as STR051CTC1;
            module.IsMUINTEnabled = true;
        }

        /// <summary>
        /// Enable HDD
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Router event args</param>
        private void EnableHDD(object sender, RoutedEventArgs e) 
        {
            STR051CTC1 module = this.DataContext as STR051CTC1;
            module.IsHDDEnabled = true; 
        }

        /// <summary>
        /// Disable HDD
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Router event args</param>
        private void DisableHDD(object sender, RoutedEventArgs e)
        {
            STR051CTC1 module = this.DataContext as STR051CTC1;
            module.IsHDDEnabled = false;
        }

        /// <summary>
        /// Refresh the UI
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Router event args</param>
        private void Refresh(object sender, RoutedEventArgs e) 
        {
            STR051CTC1 module = this.DataContext as STR051CTC1;
            module.RefreshStates(); 
        }

        /// <summary>
        /// Capture screen snapshot
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Router event args</param>
        private void Screen_Capture_Click(object sender, RoutedEventArgs e) 
        {
            STR051CTC1 module = this.DataContext as STR051CTC1;
            module.ScreenCapture(); 
        }
    }
}
