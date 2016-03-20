// -----------------------------------------------------------------------
// <copyright file="VID117CTC1UI.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace VID117
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// UI class for VID117CTC1 module
    /// </summary>
    public partial class VID117CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VID117CTC1UI" /> class.
        /// </summary>
        /// <param name="module">The module class associated with this UI</param>
        public VID117CTC1UI(VID117CTC1 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// Move to the next page
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void NextPage(object sender, RoutedEventArgs e) 
        {
            VID117CTC1 module = this.DataContext as VID117CTC1;
            module.NextPage(); 
        }

        /// <summary>
        /// Move to the next page
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void TestCable(object sender, RoutedEventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1;
            module.TestCable();
        }

        /// <summary>
        /// Confirm a resolution/format/cable as OK/good
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void LooksGood(object sender, RoutedEventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1; 
            module.ConfirmCable("good");
        }

        /// <summary>
        /// Confirm a resolution/format/cable as bad
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void LooksBad(object sender, RoutedEventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1; 
            module.ConfirmCable("bad");
        }

        /// <summary>
        /// Confirm a resolution/format/cable as real bad (Catastrophic)
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void Catastrophic(object sender, RoutedEventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1; 
            module.ConfirmCable("real bad");
        }

        /// <summary>
        /// Select the HDMI cable
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void SelectHDMI(object sender, RoutedEventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1; 
            module.SetCable(VID117.CableType.HDMI);
        }

        /// <summary>
        /// Select the component cable
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void SelectComponent(object sender, RoutedEventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1;
            module.SetCable(VID117.CableType.Component);
        }

        /// <summary>
        /// Select the VGA cable
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void SelectVGA(object sender, RoutedEventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1; 
            module.SetCable(VID117.CableType.VGA);
        }

        /// <summary>
        /// Select the Composite cable
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void SelectComposite(object sender, RoutedEventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1; 
            module.SetCable(VID117.CableType.Composite);
        }

        /// <summary>
        /// Select the S-Video cable
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void SelectSVideo(object sender, RoutedEventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1;
            module.SetCable(VID117.CableType.SVideo);
        }

        /// <summary>
        /// Select the SCART cable
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void SelectSCART(object sender, RoutedEventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1; 
            module.SetCable(VID117.CableType.SCART);
        }

        /// <summary>
        /// Select the D-Terminal cable
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Routed event args</param>
        private void SelectDTerminal(object sender, RoutedEventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1;
            module.SetCable(VID117.CableType.DTerminal);
        }

        /// <summary>
        /// Select a specified resolution from the data grid
        /// </summary>
        /// <param name="sender">UI class originating the message</param>
        /// <param name="e">Event args</param>
        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            VID117CTC1 module = this.DataContext as VID117CTC1;
            DataGrid dg = sender as DataGrid;
            DataGridCellInfo dgci = dg.CurrentCell;
            VID117CTC1.ICableResList resList = dgci.Item as VID117CTC1.ICableResList;
            DataGridColumn column = dgci.Column;
            if (column != null)
            {
                int index = column.DisplayIndex;
                module.Reposition(resList, index);
            }
        }
   }
}
