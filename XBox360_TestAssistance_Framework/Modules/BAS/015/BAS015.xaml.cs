// -----------------------------------------------------------------------
// <copyright file="BAS015.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BAS015
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for this BAS015UI user interface implementation class, and for BAS015
    /// </summary>
    public partial class BAS015UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the BAS015UI class
        /// Constructs the BAS015Module class through m parameter
        /// </summary>
        /// <param name="m">Initializes BAS015Module member</param>
        public BAS015UI(BAS015 m)
        {
            this.InitializeComponent();
            this.DataContext = m;
        }

        /// <summary>
        /// Next Page module start button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void ClickNext(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            module.NextStep();
        }

        /// <summary>
        /// Profile1 property selection changed event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Selection changed event arguments</param>
        private void Selection_Changed1(object sender, SelectionChangedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            ComboBox combo = sender as ComboBox;
            if (combo != null)
            {
                BAS015.ConsoleProfileViewItem newSelection = combo.SelectedItem as BAS015.ConsoleProfileViewItem;
                if (newSelection != null)
                {
                    newSelection.SelectedInComboBoxIndex = 1;
                }

                if (e.RemovedItems.Count > 0)
                {
                    BAS015.ConsoleProfileViewItem oldSelection = e.RemovedItems[0] as BAS015.ConsoleProfileViewItem;
                    oldSelection.SelectedInComboBoxIndex = 0;
                }
            }

            module.UpdateAllStates();
        }

        /// <summary>
        /// Profile2 property selection changed event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Selection changed event arguments</param>
        private void Selection_Changed2(object sender, SelectionChangedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            ComboBox combo = sender as ComboBox;
            if (combo != null)
            {
                BAS015.ConsoleProfileViewItem newSelection = combo.SelectedItem as BAS015.ConsoleProfileViewItem;
                if (newSelection != null)
                {
                    newSelection.SelectedInComboBoxIndex = 2;
                }

                if (e.RemovedItems.Count > 0)
                {
                    BAS015.ConsoleProfileViewItem oldSelection = e.RemovedItems[0] as BAS015.ConsoleProfileViewItem;
                    oldSelection.SelectedInComboBoxIndex = 0;
                }
            }

            module.UpdateAllStates();
        }

        /// <summary>
        /// Profile3 property selection changed event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Selection changed event arguments</param>
        private void Selection_Changed3(object sender, SelectionChangedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            ComboBox combo = sender as ComboBox;
            if (combo != null)
            {
                BAS015.ConsoleProfileViewItem newSelection = combo.SelectedItem as BAS015.ConsoleProfileViewItem;
                if (newSelection != null)
                {
                    newSelection.SelectedInComboBoxIndex = 3;
                }

                if (e.RemovedItems.Count > 0)
                {
                   BAS015.ConsoleProfileViewItem oldSelection = e.RemovedItems[0] as BAS015.ConsoleProfileViewItem;
                    oldSelection.SelectedInComboBoxIndex = 0;
                }
            }

            module.UpdateAllStates();
        }

        /// <summary>
        /// Profile4 property selection changed event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Selection changed event arguments</param>
        private void Selection_Changed4(object sender, SelectionChangedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            ComboBox combo = sender as ComboBox;
            if (combo != null)
            {
                BAS015.ConsoleProfileViewItem newSelection = combo.SelectedItem as BAS015.ConsoleProfileViewItem;
                if (newSelection != null)
                {
                    newSelection.SelectedInComboBoxIndex = 4;
                }

                if (e.RemovedItems.Count > 0)
                {
                    BAS015.ConsoleProfileViewItem oldSelection = e.RemovedItems[0] as BAS015.ConsoleProfileViewItem;
                    oldSelection.SelectedInComboBoxIndex = 0;
                }
            }

            module.UpdateAllStates();
        }

        /// <summary>
        /// Sign_1 button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void Sign_1(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            Mouse.OverrideCursor = Cursors.Wait;
            module.ToggleSignInOut(1);
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Sign_2 button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void Sign_2(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            Mouse.OverrideCursor = Cursors.Wait;
            module.ToggleSignInOut(2);
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Sign_3 button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void Sign_3(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            Mouse.OverrideCursor = Cursors.Wait;
            module.ToggleSignInOut(3);
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Sign_4 button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void Sign_4(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            Mouse.OverrideCursor = Cursors.Wait;
            module.ToggleSignInOut(4);
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// SignIn_All button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void SignIn_All(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            Mouse.OverrideCursor = Cursors.Wait;
            module.SignInAll();
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// SignOut_All button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void SignOut_All(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            Mouse.OverrideCursor = Cursors.Wait;
            module.SignOutAll();
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Delete_1 button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void Delete_1(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            Mouse.OverrideCursor = Cursors.Wait;
            module.DeleteProfile(1);
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Delete_2 button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void Delete_2(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            Mouse.OverrideCursor = Cursors.Wait;
            module.DeleteProfile(2);
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Delete_3 button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void Delete_3(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            Mouse.OverrideCursor = Cursors.Wait;
            module.DeleteProfile(3);
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Delete_4 button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void Delete_4(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            Mouse.OverrideCursor = Cursors.Wait;
            module.DeleteProfile(4);
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Create button click event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            Mouse.OverrideCursor = Cursors.Wait;
            module.CreateProfiles();
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Auto mode event handler
        /// </summary>
        /// <param name="sender">Originating object or other controller sender</param>
        /// <param name="e">Routed event arguments</param>
        private void Auto_Mode(object sender, RoutedEventArgs e)
        {
            BAS015 module = DataContext as BAS015;
            module.Auto();
        }
    }
}