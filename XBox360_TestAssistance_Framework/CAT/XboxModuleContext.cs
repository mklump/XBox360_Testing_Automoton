// -----------------------------------------------------------------------
// <copyright file="XboxModuleContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Xbox Module Context
    /// An object of this interface is passed to the module, and provides the Module with an API.
    /// </summary>
    public class XboxModuleContext : ModuleContext, IXboxModuleContext
    {
        /// <summary>
        /// Backing property for XboxTitle
        /// </summary>
        private IXboxTitle xboxTitle;

        /// <summary>
        /// Initializes a new instance of the <see cref="XboxModuleContext" /> class.
        /// </summary>
        /// <param name="xboxTitle">The current Xbox game title</param>
        /// <param name="tcrTestCase">TestCase to associate with this ModuleContext</param>
        /// <param name="viewModel">A reference to the MainViewModel</param>
        public XboxModuleContext(IXboxTitle xboxTitle, ITCRTestCase tcrTestCase, MainViewModel viewModel)
         : base(tcrTestCase, viewModel)
        {
            this.xboxTitle = xboxTitle;
        }

        /// <summary>
        /// Gets information about a Game Title.
        /// </summary>
        public IXboxTitle XboxTitle
        {
            get { return this.xboxTitle; }
        }

        /// <summary>
        /// Gets the path to the XDK tools directory.  Usually C:\Program Files (x86)\Microsoft Xbox 360 SDK\bin\win32
        /// </summary>
        public string XdkToolPath
        {
            get { return XboxDevice.XdkToolPath; }
        }

        /// <summary>
        /// Gets the XDK version installed on this computer
        /// </summary>
        public string XdkVersion
        {
            get { return XboxDevice.XdkVersion; }
        }
        
        /// <summary>
        /// Take a screenshot of the specified device and place it in the module's log folder.
        /// Implements ScreenShot in IDevice
        /// </summary>
        /// <param name="d">The device to take a screen shot of</param>
        /// <param name="fileName">Name of screenshot - Use blank string to auto-generated a filename based on current date/time</param>
        /// <param name="convertToJpg">True to convert to jpg, false to save as bmp</param>
        /// <returns>Returns the file name the screen shot was saved to</returns>
        public string ScreenShot(IDevice d, string fileName = "", bool convertToJpg = true)
        {
            if (this.LogDirectory == null)
            {
                throw new Exception("ModuleContext ScreenShot: no directory location was available to store the screen shot. Hint: Module should being logging");
            }

            string name = fileName;

            // only Xbox supported for now
            XboxDevice xb = d as XboxDevice;
            if (xb != null)             
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    name = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "_ScreenShot.";
                    if (convertToJpg)
                    {
                        name += "jpg";
                    }
                    else
                    {
                        name += "bmp";
                    }
                }

                name = Path.Combine(this.LogDirectory, name);
                xb.ScreenShot(name, convertToJpg);
            }

            return name;
        }

        /// <summary>
        /// Opens the Debug Output window for the specified xbox
        /// </summary>
        /// <param name="xbd">Xbox to open the Debug Output window for</param>
        public void OpenDebugOutput(IXboxDevice xbd)
        {
            XboxDevice xbd2 = xbd as XboxDevice;
            xbd2.XboxViewItem.OpenDebugOutput();
        }

        /// <summary>
        /// Loads a bitmap image from file into a BitmapImage object
        /// </summary>
        /// <param name="fileName">File to load bitmap from</param>
        /// <returns>A BitmapImage object containing the bitmap image from the specified file</returns>
        private static BitmapImage LoadImage(string fileName)
        {
            BitmapImage result = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                result.UriSource = new Uri(fileName, UriKind.Absolute);
                result.EndInit();
            }

            return result;
        }
    }
}
