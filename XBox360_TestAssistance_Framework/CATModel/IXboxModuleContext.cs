// -----------------------------------------------------------------------
// <copyright file="IXboxModuleContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a CAT Xbox Module Context
    /// An object of this interface is passed to the module, and provides the Module with an API.
    /// </summary>
    public interface IXboxModuleContext : IModuleContext
    {
        /// <summary>
        /// Gets the path to the XDK tools directory.  Usually C:\Program Files (x86)\Microsoft Xbox 360 SDK\bin\win32
        /// </summary>
        string XdkToolPath { get; }

        /// <summary>
        /// Gets the XDK version installed on this computer
        /// </summary>
        string XdkVersion { get; }

        /// <summary>
        /// Gets Xbox Title
        /// </summary>
        IXboxTitle XboxTitle { get; }

        /// <summary>
        /// Opens the Debug Output window for the specified xbox
        /// </summary>
        /// <param name="xbd">Xbox to open the Debug Output window for</param>
        void OpenDebugOutput(IXboxDevice xbd);

        /// <summary>
        /// Take a screenshot of the specified device and place it in the module's log folder.
        /// </summary>
        /// <param name="d">The device to take a screen shot of</param>
        /// <param name="fileName">Name of screenshot - Use blank string to auto-generate a filename based on current date/time</param>
        /// <returns>The file name of the stored screenshot</returns>
        string ScreenShot(IDevice d, string fileName = "", bool convertToJpg = true);
    }
}