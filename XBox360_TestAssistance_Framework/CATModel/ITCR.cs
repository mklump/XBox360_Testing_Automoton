// -----------------------------------------------------------------------
// <copyright file="ITCR.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for TCRs
    /// </summary>
    public interface ITCR
    {
        /// <summary>
        /// Gets the name of this TCR
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the TCR number
        /// </summary>
        int Number { get; }

        /// <summary>
        /// Gets the requirements field of the TCR
        /// </summary>
        string Requirements { get; }

        /// <summary>
        /// Gets the intents field of the TCR
        /// </summary>
        string Intent { get; }

        /// <summary>
        /// Gets the Remarks field of the TCR
        /// </summary>
        string Remarks { get; }

        /// <summary>
        /// Gets the Exemptions field of this TCR
        /// </summary>
        string Exemptions { get; }

        /// <summary>
        /// Gets the TCR Categories associated with this TCR
        /// </summary>
        ITCRCategory TCRCategory { get; }

        /// <summary>
        /// Gets the TCR Test Cases associate with this TCR
        /// </summary>
        List<ITCRTestCase> TCRTestCases { get; }
    }
}
