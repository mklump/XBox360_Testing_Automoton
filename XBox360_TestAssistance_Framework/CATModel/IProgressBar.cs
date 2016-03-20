// -----------------------------------------------------------------------
// <copyright file="IProgressBar.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    /// <summary>
    /// Prototype for a delegate used by IProgressBar
    /// </summary>
    public delegate void ProgressDelegate();

    /// <summary>
    /// Interface for test devices
    /// </summary>
    public interface IProgressBar
    {
        /// <summary>
        /// Sets a delegate to be called in a new thread.
        /// Since the Progress Bar window is modal and does not return control to the UI caller,
        /// it's necessary to run the operation we're monitoring the progress of, in another thread.
        /// </summary>
        ProgressDelegate Delegate { set; }

        /// <summary>
        /// Gets or sets the maximum value represented in the progress bar
        /// </summary>
        uint Max { get; set; }

        /// <summary>
        /// Gets or sets the current value represented in the progress bar
        /// </summary>
        uint Progress { get; set; }

        /// <summary>
        /// Shows the progress bar window.  Also invokes the delegate in another thread.
        /// </summary>
        void Show();
    }
}