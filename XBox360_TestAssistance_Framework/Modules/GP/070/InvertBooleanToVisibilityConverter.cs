// -----------------------------------------------------------------------
// <copyright file="InvertBooleanToVisibilityConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace GP070
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converts a boolean to a visibility.  If false, visibility will be Visible, if true, visibility will be Collapsed
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class InvertBooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Conversion handler for converting a boolean to an inversion of its original value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Type of value passed in value argument</param>
        /// <param name="parameter">An optional conversion parameter</param>
        /// <param name="culture">The culture info for this conversion</param>
        /// <returns>The resulting Visibility value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a visibility back to a boolean.  If Visible the boolean will be false, otherwise the boolean will be true
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Type of value passed in value argument</param>
        /// <param name="parameter">An optional conversion parameter</param>
        /// <param name="culture">The culture info for this conversion</param>
        /// <returns>The resulting boolean value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (Visibility)value != Visibility.Visible;
        }
    }
}