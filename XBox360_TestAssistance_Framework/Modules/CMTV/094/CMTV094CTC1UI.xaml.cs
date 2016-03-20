// -----------------------------------------------------------------------
// <copyright file="CMTV094CTC1UI.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CMTV094
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// UI class for CMTV094CTC1 module
    /// </summary>
    public partial class CMTV094CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CMTV094CTC1UI" /> class.
        /// </summary>
        /// <param name="module">The module class associated with this UI</param>
        public CMTV094CTC1UI(CMTV094CTC1 module)
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
            CMTV094CTC1 module = this.DataContext as CMTV094CTC1;
            module.NextPage();
        }

        private void BeginOrStop(object sender, RoutedEventArgs e)
        {
            CMTV094CTC1 module = this.DataContext as CMTV094CTC1;
            module.BeginOrStop();
        }

        private void A_Click(object sender, RoutedEventArgs e)
        {
            CMTV094CTC1 module = this.DataContext as CMTV094CTC1;
            module.Button1('a');
        }
        private void B_Click(object sender, RoutedEventArgs e)
        {
            CMTV094CTC1 module = this.DataContext as CMTV094CTC1;
            module.Button1('b');
        }
        private void X_Click(object sender, RoutedEventArgs e)
        {
            CMTV094CTC1 module = this.DataContext as CMTV094CTC1;
            module.Button1('x');
        }
        private void Y_Click(object sender, RoutedEventArgs e)
        {
            CMTV094CTC1 module = this.DataContext as CMTV094CTC1;
            module.Button1('y');
        }
        private void Down_Click(object sender, RoutedEventArgs e)
        {
            CMTV094CTC1 module = this.DataContext as CMTV094CTC1;
            module.Button1('d');
        }
        private void Up_Click(object sender, RoutedEventArgs e)
        {
            CMTV094CTC1 module = this.DataContext as CMTV094CTC1;
            module.Button1('u');
        }
        private void Left_Click(object sender, RoutedEventArgs e)
        {
            CMTV094CTC1 module = this.DataContext as CMTV094CTC1;
            module.Button1('l');
        }
        private void Right_Click(object sender, RoutedEventArgs e)
        {
            CMTV094CTC1 module = this.DataContext as CMTV094CTC1;
            module.Button1('r');
        }
    }
}
