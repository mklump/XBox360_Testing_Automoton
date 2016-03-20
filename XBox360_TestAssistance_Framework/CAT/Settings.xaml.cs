// -----------------------------------------------------------------------
// <copyright file="Settings.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    /// <summary>
    /// Interaction logic for Settings
    /// </summary>
    public partial class Settings : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings" /> class.
        /// </summary>
        public Settings()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Event handler to scroll to the end of a text box
        /// </summary>
        /// <param name="sender">UI element this event occurred on</param>
        /// <param name="e">Data transfer event args</param>
        private void ScrollToEndOfLineHandler(object sender, DataTransferEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                textBox.CaretIndex = textBox.Text.Length;
                var rect = textBox.GetRectFromCharacterIndex(textBox.CaretIndex);
                textBox.ScrollToHorizontalOffset(rect.Right);
            }
        }
    }
}