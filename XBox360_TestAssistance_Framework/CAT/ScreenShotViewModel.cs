// -----------------------------------------------------------------------
// <copyright file="ScreenShotViewModel.cs" company="Microsoft">
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
    using System.Windows.Media.Imaging;

    /// <summary>
    /// A View Model for the screen shot window
    /// </summary>
    public class ScreenShotViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenShotViewModel" /> class.
        /// </summary>
        /// <param name="title">Title for the window</param>
        /// <param name="image">Bitmap image to display in the window</param>
        public ScreenShotViewModel(string title, BitmapImage image)
        {
            this.Title = title;
            this.Image = image;
            ScreenShotWindow screenShotWindow = new ScreenShotWindow(this);
            screenShotWindow.Show();
        }

        /// <summary>
        /// Gets or sets the screen shot image
        /// </summary>
        public BitmapImage Image { get; set; }

        /// <summary>
        /// Gets or sets the title of the window
        /// </summary>
        public string Title { get; set; }
    }
}
