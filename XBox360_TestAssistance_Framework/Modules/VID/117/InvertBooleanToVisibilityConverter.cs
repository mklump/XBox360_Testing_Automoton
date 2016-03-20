// -----------------------------------------------------------------------
// <copyright file="InvertBooleanToVisibilityConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace VID117
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Value conversion class from boolean to Visibility
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InvertBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Value conversion function from boolean to Visibility type
        /// </summary>
        /// <param name="value">Object boxed type to be converted</param>
        /// <param name="targetType">Target type to be convert to</param>
        /// <param name="parameter">Actual object that requires converting</param>
        /// <param name="culture">Applicable Culture Information required parameter</param>
        /// <returns>Converted value of boolean to Visibility</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Value conversion function back from an object type of Visibility to a boolean
        /// </summary>
        /// <param name="value">Object boxed type to be converted</param>
        /// <param name="targetType">Target type to be convert to</param>
        /// <param name="parameter">Actual object that requires converting</param>
        /// <param name="culture">Applicable Culture Information required parameter</param>
        /// <returns>Converted value of Visibility to boolean, other wise null</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}