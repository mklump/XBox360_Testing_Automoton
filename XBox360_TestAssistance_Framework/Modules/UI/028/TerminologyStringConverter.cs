// -----------------------------------------------------------------------
// <copyright file="TerminologyStringConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace UI028
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converter for converting Terminology strings
    /// </summary>
    public class TerminologyStringConverter : IMultiValueConverter
    {
        /// <summary>
        /// Builds display string from two strings
        /// </summary>
        /// <param name="value">array of string</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="language">The parameter is not used.</param>
        /// <returns>A visibility value</returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo language)
        {
            string string1 = string.Empty;
            string string2 = string.Empty;
            if (value.Length > 0)
            {
                string1 = (string)value[0];
                if (value.Length > 1)
                {
                    string2 = (string)value[1];
                }
            }

            return string.Format("({1}) \"{0}\"", string1, string2);
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
