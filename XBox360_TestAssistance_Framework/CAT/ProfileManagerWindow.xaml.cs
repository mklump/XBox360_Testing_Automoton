// -----------------------------------------------------------------------
// <copyright file="ProfileManagerWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ProfileManagerWindow
    /// </summary>
    public partial class ProfileManagerWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfileManagerWindow" /> class.
        /// </summary>
        /// <param name="viewModel">The view model</param>
        public ProfileManagerWindow(ProfileManagerViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
