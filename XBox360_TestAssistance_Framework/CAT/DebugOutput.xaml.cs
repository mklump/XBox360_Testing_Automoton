// -----------------------------------------------------------------------
// <copyright file="DebugOutput.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Code behind class for Debug Output Window
    /// </summary>
    public partial class DebugOutputWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugOutputWindow" /> class.
        /// </summary>
        /// <param name="viewModel">The view model</param>
        public DebugOutputWindow(DebugOutputViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Event handler to scroll to the end of a text box when text changes
        /// </summary>
        /// <param name="sender">Event sender; the bound text box</param>
        /// <param name="e">Text changed event args</param>
        private void ScrollToBottom(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            tb.ScrollToEnd();
        }

        /// <summary>
        /// Event handler for ListView selection changes
        /// </summary>
        /// <param name="sender">Event sender; the ListView</param>
        /// <param name="e">Selection changed event args</param>
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            ListView lv = sender as ListView;
            if (e.AddedItems.Count > 0)
            {
                lv.SelectedItem = null;
            }
        }
    }
}
