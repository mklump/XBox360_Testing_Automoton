// -----------------------------------------------------------------------
// <copyright file="PER033CTC1UI.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace PER033
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using XDevkit;

    /// <summary>
    /// UI class for PER033CTC1 module
    /// </summary>
    public partial class PER033CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PER033CTC1UI" /> class.
        /// </summary>
        /// <param name="module">The module class associated with this UI</param>
        public PER033CTC1UI(PER033CTC1 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// Advance to the next page
        /// </summary>
        /// <param name="sender">UI Element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void NextPage(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.NextPage();
        }

        /// <summary>
        /// Event handler for previewing loss of keyboard focus
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void StackPanel_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            e.Handled = module.KeyboardIsCaptured;
        }

        /// <summary>
        /// Event handler for clicking on A
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void A_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.A_Button);
        }

        /// <summary>
        /// Event handler for clicking on B
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void B_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.B_Button);
        }

        /// <summary>
        /// Event handler for clicking on X
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void X_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.X_Button);
        }

        /// <summary>
        /// Event handler for clicking on Y
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Y_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.Y_Button);
        }

        /// <summary>
        /// Event handler for clicking on Down
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Down_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.DPadDown);
        }

        /// <summary>
        /// Event handler for clicking on Up
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Up_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.DPadUp);
        }

        /// <summary>
        /// Event handler for clicking on Left
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Left_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.DPadLeft);
        }

        /// <summary>
        /// Event handler for clicking on Right
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Right_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.DPadRight);
        }

        /// <summary>
        /// Event handler for clicking on Guide
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Guide_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.Xbox360_Button);
        }

        /// <summary>
        /// Event handler for clicking on Back
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.BackButton);
        }

        /// <summary>
        /// Event handler for clicking on Start
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.StartButton);
        }

        /// <summary>
        /// Event handler for clicking on Left Shoulder
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void LeftShoulder_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.LeftShoulderButton);
        }

        /// <summary>
        /// Event handler for clicking on Right Shoulder
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void RightShoulder_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerButton(XboxAutomationButtonFlags.RightShoulderButton);
        }

        /// <summary>
        /// Event handler for clicking on Left Trigger
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void LeftTrigger_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerLeftTrigger();
        }

        /// <summary>
        /// Event handler for clicking on Right Trigger
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void RightTrigger_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.SendControllerLeftTrigger();
        }

        /// <summary>
        /// Event handler for passing first test
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Pass1_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.PassStep(1);
        }

        /// <summary>
        /// Event handler for failed first test
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Fail1_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.FailStep(1);
        }

        /// <summary>
        /// Event handler for passing second test
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Pass2_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.PassStep(2);
        }

        /// <summary>
        /// Event handler for failing second test
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Fail2_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.FailStep(2);
        }

        /// <summary>
        /// Event handler for passing third test
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Pass3_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.PassStep(3);
        }

        /// <summary>
        /// Event handler for failing third test
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Fail3_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.FailStep(3);
        }

        /// <summary>
        /// Event handler for passing fourth test
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Pass4_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.PassStep(4);
        }

        /// <summary>
        /// Event handler for failing fourth test
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Fail4_Click(object sender, RoutedEventArgs e)
        {
            PER033CTC1 module = this.DataContext as PER033CTC1;
            module.FailStep(4);
        }
    }
}
