// -----------------------------------------------------------------------
// <copyright file="ConsoleProfileSelectedVisibilityConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Console profile selected Visibility converter class
    /// </summary>
    public class ConsoleProfileSelectedVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Value conversion function from ConsoleProfileSelected to Visibility object
        /// </summary>
        /// <param name="value">Object boxed type to be converted</param>
        /// <param name="targetType">Target type to be convert to</param>
        /// <param name="parameter">Actual object that requires converting</param>
        /// <param name="culture">Applicable Culture Information required parameter</param>
        /// <returns>Value of type ConsoleProfileSelected to Visibility object</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility result = Visibility.Collapsed;
            uint oldValue = (uint)value;

            string param = (string)parameter;
            uint index = uint.Parse(param);

            if ((oldValue == index) || (oldValue == 0))
            {
                result = Visibility.Visible;
            }

            return result;
        }

        /// <summary>
        /// Value conversion function back from converted Visibility object back to ConsoleProfileSelected
        /// </summary>
        /// <param name="value">Object boxed type to be converted</param>
        /// <param name="targetType">Target type to be convert to</param>
        /// <param name="parameter">Actual object that requires converting</param>
        /// <param name="culture">Applicable Culture Information required parameter</param>
        /// <returns>Converted Visibility object back to ConsoleProfileSelected, other wise throws Not Supported Exception</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}