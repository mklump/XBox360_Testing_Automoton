// -----------------------------------------------------------------------
// <copyright file="ITCRPlatform.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for TCR Platforms.
    /// </summary>
    public interface ITCRPlatform
    {
        /// <summary>
        /// Gets the name of this platform.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a list of ITCRVersion objects representing the versions associated with this platform.
        /// </summary>
        List<ITCRVersion> TCRVersions { get; }
    }
}
