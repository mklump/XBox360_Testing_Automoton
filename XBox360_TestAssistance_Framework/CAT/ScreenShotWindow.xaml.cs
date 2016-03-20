// -----------------------------------------------------------------------
// <copyright file="ScreenShotWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;

    /// <summary>
    /// Interaction logic for ScreenShotWindow
    /// </summary>
    public partial class ScreenShotWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenShotWindow" /> class.
        /// </summary>
        /// <param name="screenShotViewModel">View model to associate with this window</param>
        public ScreenShotWindow(ScreenShotViewModel screenShotViewModel)
        {
            this.InitializeComponent();
            this.DataContext = screenShotViewModel;
        }
    }
}
