// -----------------------------------------------------------------------
// <copyright file="InvertBoolConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BAS015
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Invert boolean function converter
    /// </summary>
    public class InvertBoolConverter : IValueConverter
    {
        /// <summary>
        /// Value conversion function from current boolean value to its inverted opposite
        /// </summary>
        /// <param name="value">Object boxed type to be converted</param>
        /// <param name="targetType">Target type to be convert to</param>
        /// <param name="parameter">Actual object that requires converting</param>
        /// <param name="culture">Applicable Culture Information required parameter</param>
        /// <returns>Value of type boolean to its inverted opposite</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            return !(bool)value;
        }

        /// <summary>
        /// Value conversion function back from an inverted type of boolean to its exact opposite
        /// </summary>
        /// <param name="value">Object boxed type to be converted</param>
        /// <param name="targetType">Target type to be convert to</param>
        /// <param name="parameter">Actual object that requires converting</param>
        /// <param name="culture">Applicable Culture Information required parameter</param>
        /// <returns>Inverted type of boolean to its exact opposite, other wise throws Not Supported Exception</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}