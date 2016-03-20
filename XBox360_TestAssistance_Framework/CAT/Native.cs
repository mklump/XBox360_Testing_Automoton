// -----------------------------------------------------------------------
// <copyright file="Native.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// A helper class that makes native CAT functionality accessible in managed code
    /// </summary>
    public class Native
    {
        /// <summary>
        /// Looks up the specified symbol in the specified symbol file (XDB or PDB)
        /// </summary>
        /// <param name="symbolName">Name of symbol to look up</param>
        /// <param name="symbolFile">File to look up symbol in</param>
        /// <param name="baseAddress">Base address of module</param>
        /// <param name="signature">PDB Signature of module</param>
        /// <returns>An HRESULT</returns>
        [DllImport("CATNativeUtils.dll", CharSet = CharSet.Unicode)]
        internal static extern uint LookupSymbol(string symbolName, string symbolFile, uint baseAddress, ref XboxDebugManagerNative.DM_PDB_SIGNATURE signature);
    }
}