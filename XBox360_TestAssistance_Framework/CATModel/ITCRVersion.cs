// -----------------------------------------------------------------------
// <copyright file="ITCRVersion.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for TCR Versions
    /// </summary>
    public interface ITCRVersion
    {
        /// <summary>
        /// Gets the name of this TCR Version
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the platform associated with this TCR Version
        /// </summary>
        ITCRPlatform Platform { get; }

        /// <summary>
        /// Gets a list of categories associated with this TCR Version
        /// </summary>
        List<ITCRCategory> TCRCategories { get; }

        /// <summary>
        /// Gets a list of TCRs associated with this TCR Version
        /// </summary>
        List<ITCR> TCRs { get; }
    }
}
