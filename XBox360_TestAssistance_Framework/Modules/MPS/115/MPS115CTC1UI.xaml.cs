// -----------------------------------------------------------------------
// <copyright file="MPS115CTC1UI.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace MPS115
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic
    /// </summary>
    public partial class MPS115CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the MPS115CTC1UI class.
        /// </summary>
        /// <param name="module">The module class associated with this UI</param>
        public MPS115CTC1UI(MPS115CTC1 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// Event handler which starts the MU test
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void BeginMUTest(object sender, RoutedEventArgs e) 
        {
            MPS115CTC1 module = this.DataContext as MPS115CTC1;
            module.BeginMUTest(); 
        }

        /// <summary>
        /// Event handler which starts the HD test
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void BeginHDTest(object sender, RoutedEventArgs e) 
        {
            MPS115CTC1 module = this.DataContext as MPS115CTC1;
            module.BeginHDTest(); 
        }

        /// <summary>
        /// advance to next page
        /// </summary>
        /// <param name="sender">this is ignored</param>
        /// <param name="e">this is ignored also</param>
        private void NextPage(object sender, System.Windows.RoutedEventArgs e) 
        {
            MPS115CTC1 module = this.DataContext as MPS115CTC1;
            module.NextPage(); 
        }

        /// <summary>
        /// Event handler to sign in/out profile A
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void SignInOut_ProfileA_Click(object sender, RoutedEventArgs e)
        {
            MPS115CTC1 module = this.DataContext as MPS115CTC1;
            (sender as Button).Content = module.SignInOut(module.ProfileAName);
        }

        /// <summary>
        /// Event handler to sign in/out profile B
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void SignInOut_ProfileB_Click(object sender, RoutedEventArgs e)
        {
            MPS115CTC1 module = this.DataContext as MPS115CTC1;
            module.SignInOut(module.ProfileAName);
        }

        /// <summary>
        /// Event handler to sign in/out profile C
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void SignInOut_ProfileC_Click(object sender, RoutedEventArgs e)
        {
            MPS115CTC1 module = this.DataContext as MPS115CTC1;
            module.SignInOut(module.ProfileAName);
        }

        /// <summary>
        /// Event handler to pass test A
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void PassA_Click(object sender, RoutedEventArgs e)
        {
            MPS115CTC1 module = this.DataContext as MPS115CTC1;
            module.Pass("Game Invite");
        }

        /// <summary>
        /// Event handler to fail test A
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void FailA_Click(object sender, RoutedEventArgs e)
        {
            MPS115CTC1 module = this.DataContext as MPS115CTC1;
            module.Fail("Game Invite");
        }

        /// <summary>
        /// Event handler to pass test B
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void PassB_Click(object sender, RoutedEventArgs e)
        {
            MPS115CTC1 module = this.DataContext as MPS115CTC1;
            module.Pass("Join Session in Progress");
        }

        /// <summary>
        /// Event handler to fail test B
        /// </summary>
        /// <param name="sender">The UI element that originated this event</param>
        /// <param name="e">Routed event args</param>
        private void FailB_Click(object sender, RoutedEventArgs e)
        {
            MPS115CTC1 module = this.DataContext as MPS115CTC1;
            module.Fail("Join Session in Progress");
        }
    }
}
