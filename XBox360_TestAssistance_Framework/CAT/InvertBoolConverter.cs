// -----------------------------------------------------------------------
// <copyright file="InvertBoolConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Converts a boolean value to an inversion of its original value
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBoolConverter : IValueConverter
    {
        /// <summary>
        /// Conversion handler for converting a boolean to an inversion of its original value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Type of value passed in value argument</param>
        /// <param name="parameter">An optional conversion parameter</param>
        /// <param name="culture">The culture info for this conversion</param>
        /// <returns>An inversion of the specified value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            return !(bool)value;
        }

        /// <summary>
        /// Conversion handler for converting a boolean to an inversion of its original value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="targetType">Type of value passed in value argument</param>
        /// <param name="parameter">An optional conversion parameter</param>
        /// <param name="culture">The culture info for this conversion</param>
        /// <returns>An inversion of the specified value</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            return !(bool)value;
        }
    }
}