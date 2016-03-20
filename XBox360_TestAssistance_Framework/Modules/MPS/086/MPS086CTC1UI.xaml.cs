// -----------------------------------------------------------------------
// <copyright file="MPS086CTC1UI.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MPS086
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic
    /// </summary>
    public partial class MPS086CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MPS086CTC1UI" /> class.
        /// </summary>
        /// <param name="module">The module class associated with this UI</param>
        public MPS086CTC1UI(MPS086CTC1 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// Advance to the next page
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void NextPage(object sender, RoutedEventArgs e)
        {
            MPS086CTC1 module = this.DataContext as MPS086CTC1;
            module.NextPage();
        }

        /// <summary>
        /// Send an invite
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void SendInvite_Click(object sender, RoutedEventArgs e)
        {
            MPS086CTC1 module = this.DataContext as MPS086CTC1;
            module.SetUpInvitingConsole(); 
        }

        /// <summary>
        /// Indicate the test has passed
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void PassAccess(object sender, RoutedEventArgs e)
        {
            MPS086CTC1 module = this.DataContext as MPS086CTC1;
            module.PassAccess();
        }

        /// <summary>
        /// Indicate the test has failed
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void FailAccess(object sender, RoutedEventArgs e)
        {
            MPS086CTC1 module = this.DataContext as MPS086CTC1;
            module.FailAccess(); 
        }

        /// <summary>
        /// Indicate the test has passed
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void PassInvite(object sender, RoutedEventArgs e) 
        {
            MPS086CTC1 module = this.DataContext as MPS086CTC1;
            module.PassInvite(); 
        }

        /// <summary>
        /// Indicate the test has failed
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void FailInvite(object sender, RoutedEventArgs e) 
        {
            MPS086CTC1 module = this.DataContext as MPS086CTC1;
            module.FailInvite();
        }
    }
}
