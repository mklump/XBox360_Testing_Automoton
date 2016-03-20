// -----------------------------------------------------------------------
// <copyright file="UI028CTC1.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace UI028
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using XDevkit;

    /// <summary>
    /// UI class for UI028CTC1 module
    /// </summary>
    public partial class UI028CTC1UI : Grid
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UI028CTC1UI" /> class.
        /// </summary>
        /// <param name="module">The module class associated with this UI</param>
        public UI028CTC1UI(UI028CTC1 module)
        {
            this.InitializeComponent();
            this.DataContext = module;
        }

        /// <summary>
        /// Advances to the next page
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Router event args</param>
        private void NextPage(object sender, RoutedEventArgs e) 
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.NextPage(); 
        }

        /// <summary>
        /// Backs to the previous page
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Router event args</param>
        private void BackPage(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BackPage();
        }

        /// <summary>
        /// Event handler for clicking on a language
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Language_Click(object sender, RoutedEventArgs e) 
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.UpdateCounts();
        }

        /// <summary>
        /// Event handler for clicking on a console
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Console_Click(object sender, SelectionChangedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.UpdateCounts(); 
        }

        /// <summary>
        /// Event handler for clicking on a console on the binding page
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Bind_Console_Click(object sender, SelectionChangedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
        }

        /// <summary>
        /// Event handler for capturing screen snapshots
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Capture_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.CaptureScreens(); 
        }

        /// <summary>
        /// Event handler for saving snapshots
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void SaveSnapshots_Click(object sender, RoutedEventArgs e) 
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.SaveSnapshots(); 
        }

        /////////////////////////////////
        // Controller

        /// <summary>
        /// Event handler for clicking on A
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void A_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.A_Button);
        }

        /// <summary>
        /// Event handler for clicking on B
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void B_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.B_Button);
        }

        /// <summary>
        /// Event handler for clicking on X
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void X_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.X_Button);
        }

        /// <summary>
        /// Event handler for clicking on Y
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Y_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.Y_Button);
        }

        /// <summary>
        /// Event handler for clicking on Down
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Down_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.DPadDown);
        }

        /// <summary>
        /// Event handler for clicking on Up
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Up_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.DPadUp);
        }

        /// <summary>
        /// Event handler for clicking on Left
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Left_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.DPadLeft);
        }

        /// <summary>
        /// Event handler for clicking on Right
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Right_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.DPadRight);
        }

        /// <summary>
        /// Event handler for clicking on Guide
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Guide_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.Xbox360_Button);
        }

        /// <summary>
        /// Event handler for clicking on Back
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.BackButton);
        }

        /// <summary>
        /// Event handler for clicking on Start
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.StartButton);
        }

        /// <summary>
        /// Event handler for clicking on Left Shoulder
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void LeftShoulder_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.LeftShoulderButton);
        }

        /// <summary>
        /// Event handler for clicking on Right Shoulder
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void RightShoulder_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerButtons(XboxAutomationButtonFlags.RightShoulderButton);
        }

        /// <summary>
        /// Event handler for clicking on Left Trigger
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void LeftTrigger_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerLeftTrigger();
        }

        /// <summary>
        /// Event handler for clicking on Right Trigger
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Routed event args</param>
        private void RightTrigger_Click(object sender, RoutedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            module.BroadcastControllerRightTrigger();
        }

        /// <summary>
        /// Event handler for selection changes in the terminology list
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Selection changed event args</param>
        private void TermChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                UI028CTC1 module = this.DataContext as UI028CTC1;
                KeyValuePair<string, UI028CTC1.Term> termPair = (KeyValuePair<string, UI028CTC1.Term>)e.AddedItems[0];
                module.CurrentTerm = termPair.Value;
            }
        }

        /// <summary>
        /// Event handler for changes to the terminology filter
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Text changed event args</param>
        private void FilterChanged(object sender, TextChangedEventArgs e)
        {
            UI028CTC1 module = DataContext as UI028CTC1;
            TextBox tb = sender as TextBox;
            module.ApplyTerminologyFilter(tb.Text);
        }

        /// <summary>
        /// Event handler for selection changes in console list
        /// </summary>
        /// <param name="sender">The UI element originating this event</param>
        /// <param name="e">Selection changed event args</param>
        private void Console_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UI028CTC1 module = this.DataContext as UI028CTC1;
            if (module.Page5Visibility != Visibility.Visible)
            {
                if (e.AddedItems.Count != 0)
                {
                    module.BindOne(e.AddedItems[0] as UI028.UI028CTC1.ObservedLanguage);
                }
            }
        }
    }
}
