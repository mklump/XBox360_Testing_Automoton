// -----------------------------------------------------------------------
// <copyright file="IModule.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Windows;

    /// <summary>
    /// Interface for Module instances
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Gets the UI element to display in the framework for this module
        /// </summary>
        UIElement UIElement { get; }

        /// <summary>
        /// Method to start the module
        /// </summary>
        /// <param name="ctx">IModuleContext providing the module with an API supposed by the framework</param>
        void Start(IModuleContext ctx);

        /// <summary>
        /// Method to stop execution of a module.
        /// </summary>
        void Stop();
    }
}