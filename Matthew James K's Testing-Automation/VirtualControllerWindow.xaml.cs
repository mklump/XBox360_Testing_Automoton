// -----------------------------------------------------------------------
// <copyright file="VirtualControllerWindow.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Windows;
    using System.Windows.Input;
    using XDevkit;

    /// <summary>
    /// Interaction logic for VirtualControllerWindow
    /// </summary>
    public partial class VirtualControllerWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualControllerWindow" /> class.
        /// </summary>
        /// <param name="viewModel">A reference to the virtual controller view model</param>
        public VirtualControllerWindow(VirtualControllerViewModel viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Handler for key down of any key
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Key event args</param>
        private void AnyButton_KeyDown(object sender, KeyEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            XboxAutomationButtonFlags buttons = new XboxAutomationButtonFlags();
            switch (e.Key)
            {
                case Key.A:
                    buttons = XboxAutomationButtonFlags.DPadLeft;
                    break;
                case Key.S:
                    buttons = XboxAutomationButtonFlags.DPadDown;
                    break;
                case Key.W:
                    buttons = XboxAutomationButtonFlags.DPadUp;
                    break;
                case Key.D:
                    buttons = XboxAutomationButtonFlags.DPadRight;
                    break;
                case Key.D8:
                case Key.NumPad8:
                    buttons = XboxAutomationButtonFlags.Y_Button;
                    break;
                case Key.D6:
                case Key.NumPad6:
                    buttons = XboxAutomationButtonFlags.B_Button;
                    break;
                case Key.D4:
                case Key.NumPad4:
                    buttons = XboxAutomationButtonFlags.X_Button;
                    break;
                case Key.D2:
                case Key.NumPad2:
                    buttons = XboxAutomationButtonFlags.A_Button;
                    break;
            }

            viewModel.SetControllerButtonDown(buttons);
        }

        /// <summary>
        /// Handler for key up of any key
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">Key event args</param>
        private void AnyButton_KeyUp(object sender, KeyEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.ReleaseControllerButton();
        }

        /// <summary>
        /// Handler for mouse down on Up
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressUp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.DPadUp);
        }

        /// <summary>
        /// Handler for mouse down on Down
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressDown_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.DPadDown);
        }

        /// <summary>
        /// Handler for mouse down on Right
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.DPadRight);
        }

        /// <summary>
        /// Handler for mouse down on Left
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.DPadLeft);
        }

        /// <summary>
        /// Handler for mouse down on X
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressX_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.X_Button);
        }

        /// <summary>
        /// Handler for mouse down on Y
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressY_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.Y_Button);
        }

        /// <summary>
        /// Handler for mouse down on A
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressA_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.A_Button);
        }

        /// <summary>
        /// Handler for mouse down on B
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.B_Button);
        }

        /// <summary>
        /// Handler for mouse down on Start
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressStart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.StartButton);
        }

        /// <summary>
        /// Handler for mouse down on Back
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.BackButton);
        }

        /// <summary>
        /// Handler for mouse down on Guide
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressGuide_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.Xbox360_Button);
        }

        /// <summary>
        /// Handler for mouse down on Left Shoulder
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressLeftShoulder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.LeftShoulderButton);
        }

        /// <summary>
        /// Handler for mouse down on Right Shoulder
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressRightShoulder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.SetControllerButtonDown(XboxAutomationButtonFlags.RightShoulderButton);
        }

        /// <summary>
        /// Handler for mouse down on Right Trigger
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressRightTrigger_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.PressRightTrigger();
        }

        /// <summary>
        /// Handler for mouse down on Left Trigger
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void PressLeftTrigger_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.PressLeftTrigger();
        }

        /// <summary>
        /// Handler for mouse up on any button
        /// </summary>
        /// <param name="sender">Originating UI element for this event</param>
        /// <param name="e">mouse button event args</param>
        private void AnyButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            VirtualControllerViewModel viewModel = this.DataContext as VirtualControllerViewModel;
            viewModel.ReleaseControllerButton();
        }
    }
}
