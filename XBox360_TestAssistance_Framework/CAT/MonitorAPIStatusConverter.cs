// -----------------------------------------------------------------------
// <copyright file="MonitorAPIStatusConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// Converts 2 booleans to a status color
    /// </summary>
    internal class MonitorAPIStatusConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts 2 booleans to a status color
        /// </summary>
        /// <param name="value">array of booleans</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="language">The parameter is not used.</param>
        /// <returns>A status color</returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo language)
        {
            bool isSymbolFound = false;
            bool wasCalled = false;
            if (value.Length > 0)
            {
                isSymbolFound = (bool)value[0];
                if (value.Length > 1)
                {
                    wasCalled = (bool)value[1];
                }
            }

            Brush textColor;
            if (wasCalled)
            {
                textColor = Brushes.Red;
            }
            else if (!isSymbolFound)
            {
                textColor = Brushes.Yellow;
            }
            else
            {
                textColor = Brushes.LightGreen;
            }

            return textColor;
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
