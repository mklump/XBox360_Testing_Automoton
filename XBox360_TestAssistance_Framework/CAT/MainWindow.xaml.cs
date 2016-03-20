// -----------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            MainViewModel vm = DataContext as MainViewModel;
            this.Closing += vm.OnWindowClosing;
            this.Loaded += vm.OnWindowLoaded;

            if ((Properties.Settings.Default.Height != 0) && (Properties.Settings.Default.Width != 0))
            {
                this.Top = Properties.Settings.Default.Top;
                this.Left = Properties.Settings.Default.Left;
                this.Height = Properties.Settings.Default.Height;
                this.Width = Properties.Settings.Default.Width;
            }
        }

        /// <summary>
        /// Using MouseLeftButtonUp to cause reselect on re-click, whereas SelectedItemChanged is only called when first selected
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="args">Mouse button event args</param>
        private void SelectOrReselect(object sender, MouseButtonEventArgs args)
        {
            TreeViewItem tvi = sender as TreeViewItem;
            if (tvi != null)
            {
                args.Handled = true;
                MainViewModel vm = DataContext as MainViewModel;

                TCRPlatformViewItem pvi = tvi.Header as TCRPlatformViewItem;
                if (pvi != null)
                {
                    vm.CurrentPlatform = pvi;
                }
                else
                {
                    TCRVersionViewItem vvi = tvi.Header as TCRVersionViewItem;
                    if (vvi != null)
                    {
                        vm.CurrentTCRVersion = vvi;
                    }
                    else
                    {
                        TCRCategoryViewItem cvi = tvi.Header as TCRCategoryViewItem;
                        if (cvi != null)
                        {
                            if (vm.CurrentTCRCategory != cvi)
                            {
                                vm.CurrentTCRCategory = cvi;
                                cvi.IsExpanded = true;
                            }
                            else
                            {
                                cvi.IsExpanded = !cvi.IsExpanded;
                            }
                        }
                        else
                        {
                            TCRViewItem tcrvi = tvi.Header as TCRViewItem;
                            if (tcrvi != null)
                            {
                                vm.CurrentTCR = tcrvi;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handler to capture right-click in device pool and ignore it
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Mouse button event args</param>
        private void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Don't change selection when right-clicked.
            e.Handled = true;
        }

        /// <summary>
        /// Handler to trigger refresh of Xbox related properties when Xbox is clicked on in device pool
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Mouse button event args</param>
        private void OnListViewItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Even if selection hasn't changed, refresh the properties each time clicked
            ListViewItem lvi = sender as ListViewItem;
            if (lvi != null)
            {
                XboxViewItem xbvi = lvi.DataContext as XboxViewItem;
                if (xbvi != null)
                {
                    xbvi.RefreshState();
                }
            }

            e.Handled = false;
        }

        /// <summary>
        /// Handler to expand and/or collapse the Xbox auxiliary panel when an Xbox is selected/deselected
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Selection changed event args</param>
        private void OnDeviceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                foreach (object removedItem in e.RemovedItems)
                {
                    XboxViewItem xbvi = removedItem as XboxViewItem;
                    if (xbvi != null)
                    {
                        xbvi.IsClicked = false;
                        xbvi.AuxPanelVisibility = Visibility.Collapsed;
                    }
                }
            }

            if (e.AddedItems.Count > 0)
            {
                foreach (object addedItem in e.AddedItems)
                {
                    XboxViewItem xbvi = addedItem as XboxViewItem;
                    if (xbvi != null)
                    {
                        xbvi.IsClicked = true;
                        xbvi.AuxPanelVisibility = (xbvi.Connected && !xbvi.IsOffline) ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Handler to deselect items in a ListView if area outside of elements has been clicked (other than scroll bar)
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Mouse button event args</param>
        private void DevicePoolPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            ListView lv = sender as ListView;
            while ((dep != null) && !(dep is ListBoxItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
                if (dep is System.Windows.Controls.Primitives.ScrollBar)
                {
                    return;
                }
            }

            if (dep == null)
            {
                lv.SelectedItem = null;
                return;
            }
        }

        /// <summary>
        /// Set an xbox language to English
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageEnglish(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageEnglishCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Japanese
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageJapanese(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageJapaneseCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to German
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageGerman(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageGermanCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to French
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageFrench(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageFrenchCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Spanish
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageSpanish(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageSpanishCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Italian
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageItalian(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageItalianCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Korean
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageKorean(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageKoreanCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Traditional Chinese
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageTraditionalChinese(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageTraditionalChineseCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Simplified Chinese
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageSimplifiedChinese(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageSimplifiedChineseCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Portuguese
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguagePortuguese(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageBrazilianPortugueseCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Polish
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguagePolish(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguagePolishCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Russian
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageRussian(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageRussianCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Swedish
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageSwedish(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageSwedishCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Turkish
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageTurkish(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageTurkishCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Norwegian
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageNorwegian(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageNorwegianCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Set an xbox language to Dutch
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void SetLanguageDutch(object sender, RoutedEventArgs e)
        {
            ComboBoxItem cbi = sender as ComboBoxItem;
            if (cbi != null)
            {
                XboxViewItem xvi = cbi.DataContext as XboxViewItem;
                if (xvi != null)
                {
                    xvi.SetLanguageDutchCommand.Execute(xvi);
                }
            }
        }

        /// <summary>
        /// Handler for changes to the filter text box
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Text changed event args</param>
        private void FilterChanged(object sender, TextChangedEventArgs e)
        {
            MainViewModel vm = DataContext as MainViewModel;
            TextBox tb = sender as TextBox;
            vm.ApplyTextFilter(tb.Text);
            vm.FilterHasContent = !string.IsNullOrEmpty(tb.Text);
        }

        /// <summary>
        /// Handles for clicking on the clear filter button
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Routed event args</param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainViewModel vm = DataContext as MainViewModel;
            vm.TextFilter = string.Empty;
        }
    }
}