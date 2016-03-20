// -----------------------------------------------------------------------
// <copyright file="IMonitorAPISession.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;

    /// <summary>
    /// Delegate type for monitoring API calls
    /// </summary>
    /// <param name="symbol">Symbol of API call</param>
    public delegate void MonitorAPIDelegate(IMonitorAPISession symbol);

    /// <summary>
    /// Interface for API Monitoring session
    /// </summary>
    public interface IMonitorAPISession : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the symbol has been found
        /// </summary>
        bool IsSymbolFound { get; }

        /// <summary>
        /// Gets the symbol name associated with this MonitorAPISession
        /// </summary>
        string SymbolName { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the monitored function has been called
        /// </summary>
        bool WasCalled { get; set; }
    }
}
