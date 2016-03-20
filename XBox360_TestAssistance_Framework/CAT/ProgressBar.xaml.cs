// -----------------------------------------------------------------------
// <copyright file="ProgressBar.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Interaction logic for ProgressBar
    /// </summary>
    public partial class ProgressBarWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBarWindow" /> class.
        /// </summary>
        public ProgressBarWindow()
        {
            this.InitializeComponent();
            this.Loaded += this.OnWindowLoaded;
        }

        /// <summary>
        /// Event handle invoked as the window is loaded.
        /// Changes the window style to appear modal
        /// </summary>
        /// <param name="sender">The UI element this event is being invoked on</param>
        /// <param name="e">Routed event args</param>
        public void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            const int GWL_STYLE = -16;
            const int WS_SYSMENU = 0x80000;

            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newValue);
    }
}
