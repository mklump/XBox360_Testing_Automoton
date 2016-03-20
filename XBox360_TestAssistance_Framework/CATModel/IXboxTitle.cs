// -----------------------------------------------------------------------
// <copyright file="IXboxTitle.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    /// <summary>
    /// Interface for XboxTitle
    /// </summary>
    public interface IXboxTitle
    {
        /// <summary>
        /// Gets the name of this Xbox Title
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether or not to use the demo content package, is a content package title
        /// </summary>
        bool UseDemo { get; }

        /// <summary>
        /// Gets the symbol path
        /// </summary>
        string SymbolsPath { get; }

        /// <summary>
        /// Gets the path to the game config file
        /// </summary>
        string GameConfigPath { get; }
        
        /// <summary>
        /// Gets the path to the content package, if a content package title
        /// </summary>
        string ContentPackage { get; }

        /// <summary>
        /// Gets the path to the demo content package, if a content package title
        /// </summary>
        string DemoContentPackage { get; }

        /// <summary>
        /// Gets the path to the XDK Recovery exe
        /// </summary>
        string XdkRecoveryPath { get; }

        /// <summary>
        /// Gets the game install type
        /// </summary>
        string GameInstallType { get; }

        /// <summary>
        /// Gets the root game directory
        /// </summary>
        string GameDirectory { get; }

        /// <summary>
        /// Gets the path of the RAW game files, if a raw title
        /// </summary>
        string RawGameDirectory { get; }

        /// <summary>
        /// Gets the path to the disc image, if a disc emulation title
        /// </summary>
        string DiscImage { get; }

        /// <summary>
        /// Gets the path to the title update, if present
        /// </summary>
        string TitleUpdatePath { get; }
    }
}
