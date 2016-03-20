// -----------------------------------------------------------------------
// <copyright file="ITCRCategory.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for TCR Categories
    /// </summary>
    public interface ITCRCategory
    {
        /// <summary>
        /// Gets the name of this TCR Category
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the TCR Version associated with this TCR Category
        /// </summary>
        ITCRVersion TCRVersion { get; }

        /// <summary>
        /// Gets the TCRs associate with this TCR Category
        /// </summary>
        List<ITCR> TCRs { get; }
    }
}
