// -----------------------------------------------------------------------
// <copyright file="LanguageIdToNameConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace UI028
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converter for converting language ID's to language names
    /// </summary>
    public class LanguageIdToNameConverter : IValueConverter
    {
        /// <summary>
        /// Converts a Language ID to a language name
        /// </summary>
        /// <param name="value">Language ID to convert</param>
        /// <param name="targetType">Type of the value argument - should be string</param>
        /// <param name="parameter">Optional parameter - unused</param>
        /// <param name="culture">Culture Info to use</param>
        /// <returns>A language name based on the specified language ID</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = value as string;
            string languageName = string.Empty;

            switch (s)
            {
                case "en-US":
                    languageName = "English";
                    break;
                case "fr-FR":
                    languageName = "Fresh";
                    break;
                case "ko-KR":
                    languageName = "Korean";
                    break;
                case "ru-RU":
                    languageName = "Russian";
                    break;
                case "zh-CN":
                    languageName = "Simplified Chinese";
                    break;
                case "pt-PT":
                    languageName = "Portuguese";
                    break;
                case "it-IT":
                    languageName = "Italian";
                    break;
                case "de-DE":
                    languageName = "German";
                    break;
                case "es-ES":
                    languageName = "Spanish";
                    break;
                case "ja-JP":
                    languageName = "Japanese";
                    break;
                case "pl-PL":
                    languageName = "Polish";
                    break;
                case "zh-CHT":
                    languageName = "Traditional Chinese";
                    break;
                case "da-DK":
                    languageName = "Dutch";
                    break;
                case "nb-NO":
                    languageName = "Norwegian";
                    break;
                case "sv-SE":
                    languageName = "Swedish";
                    break;
                case "tr-TR":
                    languageName = "Turkish";
                    break;
                default:
                    languageName = "Unknown";
                    break;
            }

            return languageName;
        }

        /// <summary>
        /// Convert-back is unsupported
        /// </summary>
        /// <param name="value">The parameter is not used.</param>
        /// <param name="targetType">The parameter is not used.</param>
        /// <param name="parameter">The parameter is not used.</param>
        /// <param name="culture">The parameter is not used.</param>
        /// <returns>unused result</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
