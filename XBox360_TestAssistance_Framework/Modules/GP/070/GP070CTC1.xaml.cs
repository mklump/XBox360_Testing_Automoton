// -----------------------------------------------------------------------
// <copyright file="GP070CTC1.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace GP070
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// UI class for GP070CTC1 module
    /// </summary>
    public partial class GP070CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GP070CTC1UI" /> class.
        /// </summary>
        /// <param name="module">The module class associated with this UI</param>
        public GP070CTC1UI(GP070CTC1 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// Advances to the next page
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Router event args</param>
        private void NextPage(object sender, RoutedEventArgs e) 
        {
            GP070CTC1 module = this.DataContext as GP070CTC1;
            module.NextPage(); 
        }

        /// <summary>
        /// Event handler for clicking on a language
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Language_Click(object sender, RoutedEventArgs e) 
        {
            GP070CTC1 module = this.DataContext as GP070CTC1;
            module.UpdateCounts();
        }

        /// <summary>
        /// Event handler for clicking on a console
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Console_Click(object sender, SelectionChangedEventArgs e)
        {
            GP070CTC1 module = this.DataContext as GP070CTC1;
            module.UpdateCounts(); 
        }

        /// <summary>
        /// Event handler for waking up development kits
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void WakeUp_Click(object sender, RoutedEventArgs e) 
        {
            GP070CTC1 module = this.DataContext as GP070CTC1;
            module.WakeUpXboxDevkits();
        }

        /// <summary>
        /// Event handler for capturing screen snapshots
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            GP070CTC1 module = this.DataContext as GP070CTC1;
            module.CaptureScreens(); 
        }

        /// <summary>
        /// Event handler for capturing more screen snapshots
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void CaptureMore_Click(object sender, RoutedEventArgs e) 
        {
            GP070CTC1 module = this.DataContext as GP070CTC1;
            module.CaptureMoreScreens();
        }

        /// <summary>
        /// Event handler for saving and logging snapshots
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void SaveAndLogSnapshots_Click(object sender, RoutedEventArgs e) 
        {
            GP070CTC1 module = this.DataContext as GP070CTC1;
            module.SaveAndLogSnapshots(); 
        }
    }
}
