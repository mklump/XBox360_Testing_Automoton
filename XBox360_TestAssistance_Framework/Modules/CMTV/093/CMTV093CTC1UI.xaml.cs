// -----------------------------------------------------------------------
// <copyright file="CMTV093CTC1UI.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CMTV093
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// UI class for CMTV093CTC1 module
    /// </summary>
    public partial class CMTV093CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CMTV093CTC1UI" /> class.
        /// </summary>
        /// <param name="module">The module class associated with this UI</param>
        public CMTV093CTC1UI(CMTV093CTC1 module)
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
            CMTV093CTC1 module = this.DataContext as CMTV093CTC1;
            module.NextPage();
        }
    }
}
