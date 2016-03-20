// -----------------------------------------------------------------------
// <copyright file="TranslationConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace GP070
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converts a dictionary of translated strings and the currently selected language, into a specific translated string.
    /// </summary>
    public class TranslationConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts a dictionary of translated strings and the currently selected language, into a specific translated string.
        /// </summary>
        /// <param name="value">An array containing a SortedDictionary of translated strings, and a string to use as a key into it</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="language">The parameter is not used.</param>
        /// <returns>A translated string value</returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo language)
        {
            SortedDictionary<string, GP070CTC1.TranslatedString> d = value[0] as SortedDictionary<string, GP070CTC1.TranslatedString>;
            if (d != null)
            {
                string lang = value[1] as string;
                if (lang != null)
                {
                    if (lang != "All")
                    {
                        GP070CTC1.TranslatedString translatedString = d[lang];

                        if (value.Length <= 2)
                        {
                            return translatedString.Value;
                        }

                        string contextName = value[2] as string;
                        return translatedString.Value + " (" + contextName + ")";
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Convert-back is not supported
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
