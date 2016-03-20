// -----------------------------------------------------------------------
// <copyright file="ITCRTestCase.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for TCR test cases
    /// </summary>
    public interface ITCRTestCase
    {
        /// <summary>
        /// Gets the name of this Test Case
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the Id of the TCR Test Case
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets the Requirements field of the TCR test case
        /// </summary>
        string Requirements { get; }

        /// <summary>
        /// Gets the Configuration field of the TCR test case
        /// </summary>
        string Configuration { get; }

        /// <summary>
        /// Gets the Definitions field of the TCR test case
        /// </summary>
        string Definition { get; }

        /// <summary>
        /// Gets the Steps field of the TCR test case
        /// </summary>
        string Steps { get; }

        /// <summary>
        /// Gets the Documentation field of the TCR test case
        /// </summary>
        string Documentation { get; }

        /// <summary>
        /// Gets the Result field of the TCR Test Case
        /// </summary>
        string Result { get; }

        /// <summary>
        /// Gets the Pass Examples field of the TCR Test Case
        /// </summary>
        string PassExamples { get; }

        /// <summary>
        /// Gets the Fail Example field of the TCR test case
        /// </summary>
        string FailExamples { get; }

        /// <summary>
        /// Gets the N/A Examples field of the TCR Test Case
        /// </summary>
        string NaExamples { get; }

        /// <summary>
        /// Gets the Analysis field of the TCR Test case
        /// </summary>
        string Analysis { get; }

        /// <summary>
        /// Gets the FAQ field of the TCR Test Case
        /// </summary>
        string Faq { get; }

        /// <summary>
        /// Gets the Hardware field of the TCR Test Case
        /// </summary>
        string Hardware { get; }

        /// <summary>
        /// Gets the Tools field of the TCR Test Case
        /// </summary>
        string Tools { get; }

        /// <summary>
        /// Gets the TCR associated with this test case
        /// </summary>
        ITCR TCR { get; }

        /// <summary>
        /// Gets the CAT modules associate with this TCR Test Case
        /// </summary>
        List<IModule> CATModules { get; }
    }
}
