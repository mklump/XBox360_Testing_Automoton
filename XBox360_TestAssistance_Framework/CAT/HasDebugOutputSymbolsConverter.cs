// -----------------------------------------------------------------------
// <copyright file="HasDebugOutputSymbolsConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts 2 booleans to a visibility
    /// </summary>
    internal class HasDebugOutputSymbolsConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts 2 booleans to a visibility
        /// </summary>
        /// <param name="value">array of booleans</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="language">The parameter is not used.</param>
        /// <returns>A visibility value</returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo language)
        {
            bool hasSymbolsA = false;
            bool hasSymbolsW = false;
            if (value.Length > 0)
            {
                hasSymbolsA = (bool)value[0];
                if (value.Length > 1)
                {
                    hasSymbolsW = (bool)value[1];
                }
            }

            return (hasSymbolsA || hasSymbolsW) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Convert-back is unsupported
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="language">The parameter is not used.</param>
        /// <returns>The return value is not used.</returns>
        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo language) // unused
        {
            throw new Exception();
        }
    }
}
