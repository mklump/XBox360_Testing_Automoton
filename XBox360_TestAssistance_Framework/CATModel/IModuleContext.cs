// -----------------------------------------------------------------------
// <copyright file="IModuleContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for a CAT Module Context
    /// An object of this interface is passed to the module, and provides the Module with an API.
    /// </summary>
    public interface IModuleContext
    {
        /// <summary>
        /// Gets the test case associated with this module
        /// </summary>
        ITCRTestCase TestCase { get; }

        /// <summary>
        /// Gets the currently selected set of devices
        /// </summary>
        List<IDevice> SelectedDevices { get; }

        /// <summary>
        /// Gets all devices.
        /// Note: The module should only utilize devices that are selected (use SelectedDevices).
        /// </summary>
        List<IDevice> AllDevices { get; }

        /// <summary>
        /// Gets the directory this module may log to.
        /// </summary>
        string LogDirectory { get; }

        /// <summary>
        /// Gets the directory where platform and version specific CAT data files are found
        /// </summary>
        string PlatformDataPath { get; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the module should be considered Modal.
        /// If modal, the user will be prompted to confirm exit of the module.
        /// </summary>
        bool IsModal { get; set;  }

        /// <summary>
        /// Appends to the module's log file
        /// </summary>
        /// <param name="text">Text to append to the log</param>
        void Log(string text);

        /// <summary>
        /// Creates a progress bar window with the specified title.
        /// </summary>
        /// <param name="title">Title for the progress bar window</param>
        /// <returns>An instance of a class derived from IProgressBar</returns>
        IProgressBar OpenProgressBarWindow(string title);
    }
}