// -----------------------------------------------------------------------
// <copyright file="TranslationCheckboxVisibilityConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace UI028
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converter for converting a dictionary of translated strings and the current selected language, into a visibility flag
    /// </summary>
    public class TranslationCheckboxVisibilityConverter : IMultiValueConverter
    {
        /// <summary>
        /// Converts a dictionary of translated strings and the current selected language, into a visibility flag
        /// </summary>
        /// <param name="value">An array containing a SortedDictionary of translated strings, and a string to use as a key into it</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="language">The parameter is not used.</param>
        /// <returns>A visibility value</returns>
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo language)
        {
            SortedDictionary<string, UI028CTC1.TranslatedString> d = value[0] as SortedDictionary<string, UI028CTC1.TranslatedString>;
            if (d != null)
            {
                string lang = value[1] as string;
                if (lang != null)
                {
                    if (lang != "All")
                    {
                        UI028CTC1.TranslatedString translatedString = d[lang];
                        return (translatedString.ExpandedStrings.Count != 1) ? Visibility.Collapsed : Visibility.Visible;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Convert-back is unused
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="language">The parameter is not used.</param>
        /// <returns>This parameter is unused.</returns>
        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo language) // unused
        {
            throw new Exception();
        }
    }
}
