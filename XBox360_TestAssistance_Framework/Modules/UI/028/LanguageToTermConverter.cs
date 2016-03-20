// -----------------------------------------------------------------------
// <copyright file="LanguageToTermConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace UI028
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converter for looking up a term by language
    /// </summary>
    public class LanguageToTermConverter : IMultiValueConverter
    {
        /// <summary>
        /// Builds display string from a string and a dictionary
        /// </summary>
        /// <param name="value">array of string</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="language">The parameter is not used.</param>
        /// <returns>A visibility value</returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo language)
        {
            Dictionary<string, List<KeyValuePair<string, UI028CTC1.Term>>> dictionary = value[0] as Dictionary<string, List<KeyValuePair<string, UI028CTC1.Term>>>;
            string string1 = value[1] as string;
            return dictionary[string1];
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
