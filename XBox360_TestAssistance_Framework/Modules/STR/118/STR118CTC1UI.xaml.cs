// -----------------------------------------------------------------------
// <copyright file="STR118CTC1UI.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace STR118
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic
    /// </summary>
    public partial class STR118CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="STR118CTC1UI" /> class.
        /// </summary>
        /// <param name="module">The module class associated with this UI</param>
        public STR118CTC1UI(STR118CTC1 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// Advance to the next page
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void NextPage(object sender, RoutedEventArgs e) 
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            module.NextPage();
        }

        /// <summary>
        /// Load save
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void LoadSave_Click(object sender, RoutedEventArgs e)
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            module.LoadSelectedSave(); 
        }

        /// <summary>
        /// Load optional save type
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void LoadAdjunct_Click(object sender, RoutedEventArgs e)
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            module.LoadSelectedAdjunct();
        }

        /// <summary>
        /// Add saves
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void AddSaves_Click(object sender, RoutedEventArgs e) 
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            module.AddSaves(); 
        }

        /// <summary>
        /// Pass test of a saved file
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void PassSave_Click(object sender, RoutedEventArgs e)
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            module.PassSave();
        }

        /// <summary>
        /// Fail test of a saved file
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void FailSave_Click(object sender, RoutedEventArgs e)
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            module.FailSave();
        }

        /// <summary>
        /// Investigate Failed test of a saved file - click after
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Investigate_Click(object sender, RoutedEventArgs e)
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            module.InvestigationIsDone();
        }

        /// <summary>
        /// Pass test using no profile
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void PassNoProfile_Click(object sender, RoutedEventArgs e)
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            module.PassNoProfile();
        }

        /// <summary>
        /// Fail test using no profile
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void FailNoProfile_Click(object sender, RoutedEventArgs e)
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            module.FailNoProfile();
        }

        /// <summary>
        /// Handler for Selection Changed event in a ListView
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            module.CanPassFail = false;
            module.CanLoad = true;
        }

        /// <summary>
        /// Handler for KeyUp events in a ListView
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Key event args</param>
        private void ListView_KeyUp(object sender, KeyEventArgs e)
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            if (e.Key == Key.Delete)
            {
                module.DeleteSelectedSaveOnbox();
            }
        }

        /// <summary>
        /// Handler for pressing delete in a ListView
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void ListView_Delete(object sender, RoutedEventArgs e)
        {
            STR118CTC1 module = this.DataContext as STR118CTC1;
            module.DeleteSelectedSaveOnbox();
        }
    }
}