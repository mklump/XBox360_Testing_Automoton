// -----------------------------------------------------------------------
// <copyright file="CMTV094CTC1.xaml.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CMTV094
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Threading;
    using System.Collections.ObjectModel;
    using XDevkit;
    using Microsoft.Test.Xbox.Profiles;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.IO;
    using System.Windows.Input;
    using CAT;

    public class CMTV094CTC1 : IModule, INotifyPropertyChanged
    {
        private IXboxModuleContext moduleContext;
        private CMTV094CTC1UI moduleUI;
        private string passedOrFailed = "PASSED";
        private IXboxDevice xboxDevice1;
        private IXboxDevice xboxDevice2;
        private IXboxDevice xboxDevice3;
        private ConsoleProfile profileA;
        private ConsoleProfile profileB;
        private ConsoleProfile profileC;
        private ConsoleProfilesManager profileManager1;
        private ConsoleProfilesManager profileManager2;
        private ConsoleProfilesManager profileManager3;
        private bool bound;
        private List<IXboxAutomation> automation;
        private uint quadrant = 0;
        private XBOX_AUTOMATION_GAMEPAD gamepad;

        public event PropertyChangedEventHandler PropertyChanged;

        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
        }

        /// <summary>
        /// Start - called when the module is first entered
        /// 
        /// This function is called to show the overview or intro to the module.
        /// Typically the framework is active and user should choose a device in the device pool.
        /// </summary>
        /// <param name="ctx">The current working context for which this test will execute.</param>
        public void Start(IModuleContext ctx)
        {
            this.moduleContext = ctx as IXboxModuleContext;
            this.moduleUI = new CMTV094CTC1UI(this);
        }

        public void BeginOrStop()
        {
            if (!bound)
            {
                if (moduleContext.SelectedDevices.Count == 0)
                {
                    MessageBox.Show("Select one or more consoles");
                    return;
                }

                automation = new List<IXboxAutomation>();

                foreach (IXboxDevice device in moduleContext.SelectedDevices)
                {
                    automation.Add(device.XboxConsole.XboxAutomation);
                }

                //automation = this.xboxDevice1.XboxConsole.XboxAutomation;
                gamepad = new XBOX_AUTOMATION_GAMEPAD();

                foreach (IXboxAutomation auto in automation)
                {
                    auto.BindController(quadrant, 1);
                    auto.ClearGamepadQueue(quadrant);
                    auto.ConnectController(quadrant);
                }
                bound = true;
                moduleContext.IsModal = true;
            }
            else
            {
                foreach (IXboxAutomation auto in automation)
                {
                    auto.UnbindController(quadrant);
                }
                automation.Clear();
                bound = false;
                moduleContext.IsModal = false;
            }
        }

        /// <summary>
        /// NextPage - called to leave the the module overview or intro screen entered by Start(ctx)
        /// The framework goes modal in this call and the module gains control.
        /// 
        /// This function is called repeatedly to advance to multiple screens in the module.
        /// </summary>
        public void NextPage()
        {
            string title = "";

            // get the three xboxes
            if (!ThreeConnectedXboxesSelected)
                return;

            // check a title is selected
            if (this.moduleContext.XboxTitle.Name != "")
            {
                title = this.moduleContext.XboxTitle.Name;
            }

            if (title == "")
            {
                MessageBox.Show("Please select a title from the setup dialog", "Certification Assistance Tool");
                return;
            }


            // assign xboxes
            foreach (IXboxDevice dev in this.moduleContext.SelectedDevices)
            {
                if (dev.IsDefault)
                {
                    xboxDevice1 = dev;
                }
            }

            foreach (IXboxDevice dev in this.moduleContext.SelectedDevices)
            {
                if (xboxDevice1 == null)
                {
                    xboxDevice1 = dev;
                }
                else if (xboxDevice2 == null && xboxDevice1 != dev)
                {
                    xboxDevice2 = dev;
                }
                else if (xboxDevice3 == null && xboxDevice1 != dev)
                {
                    xboxDevice3 = dev;
                }
            }

            // set up test
            setup();
        }

        public void setup()
        {
            // get a profile manager for each console
            try
            {
                this.profileManager1 = xboxDevice1.XboxConsole.CreateConsoleProfilesManager();
                this.profileManager2 = xboxDevice2.XboxConsole.CreateConsoleProfilesManager();
                this.profileManager3 = xboxDevice3.XboxConsole.CreateConsoleProfilesManager();
            }
            catch
            {
            }

            // sign out all profiles
            profileManager1.SignOutAllUsers();
            profileManager2.SignOutAllUsers();
            profileManager3.SignOutAllUsers();

            // select and sign in a profile on each console
            if (profileManager1.EnumerateConsoleProfiles().Any())
            {
                profileA = profileManager1.GetDefaultProfile();
                if (profileA == null)
                {
                    profileA = profileManager1.EnumerateConsoleProfiles().First();
                    profileManager1.SetDefaultProfile(profileA);
                }
            }
            else 
            {
                profileA = profileManager1.CreateConsoleProfile(true);
            }
            profileA.SignIn(UserIndex.Zero);

            if (profileManager2.EnumerateConsoleProfiles().Any())
            {
                profileB = profileManager2.GetDefaultProfile();
                if (profileB == null)
                {
                    profileB = profileManager2.EnumerateConsoleProfiles().First();
                    profileManager2.SetDefaultProfile(profileB);
                }
            }
            else
            {
                profileB = profileManager2.CreateConsoleProfile(true);
            }
            profileB.SignIn(UserIndex.Zero);

            if (profileManager3.EnumerateConsoleProfiles().Any())
            {
                profileC = profileManager3.GetDefaultProfile();
                if (profileC == null)
                {
                    profileC = profileManager3.EnumerateConsoleProfiles().First();
                    profileManager3.SetDefaultProfile(profileC);
                }
            }
            else
            {
                profileC = profileManager3.CreateConsoleProfile(true);
            }
            profileC.SignIn(UserIndex.Zero);

            // set Play Through Speakers on all three consoles
            xboxDevice1.RunCatScript("Voice_Output_Set_Speakers");
            xboxDevice2.RunCatScript("Voice_Output_Set_Speakers");
            xboxDevice3.RunCatScript("Voice_Output_Set_Speakers");

            // Set privacy settings to voice/text/video to Friends Only profile A
            xboxDevice1.RunCatScript("Communications_Set_Friends_Only");

            // Set privace settings to voice/text/video enabled for Everyone on profile B and C
            xboxDevice2.RunCatScript("Communications_Set_Everyone");
            xboxDevice3.RunCatScript("Communications_Set_Everyone");

            // Friend profiles A and B
            profileA.Friends.SendFriendRequest(profileB);
            profileB.Friends.AcceptFriendRequest(profileA);

            // Launch the game on all three consoles
            xboxDevice1.LaunchTitle();
            xboxDevice2.LaunchTitle();
            xboxDevice3.LaunchTitle();
        }

        /// <summary>
        /// Stop - called when the module is done or aborted
        /// </summary>
        public void Stop()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            moduleContext.Log("*************************************************************\r\n");
            moduleContext.Log("*************************************************************\r\n");
            moduleContext.Log("RESULT: " + passedOrFailed + "\r\n");
            moduleContext.Log("*************************************************************\r\n");

            Mouse.OverrideCursor = null;
        }

        public void Button1(char button)
        {
            bool error = false;

            if (!bound)
            {
                MessageBox.Show("Choose one or more consoles and click Begin to activate the keypad");
                return;
            }

            switch (button)
            {
                case 'a': gamepad.Buttons = XboxAutomationButtonFlags.A_Button; break;
                case 'b': gamepad.Buttons = XboxAutomationButtonFlags.B_Button; break;
                case 'x': gamepad.Buttons = XboxAutomationButtonFlags.X_Button; break;
                case 'y': gamepad.Buttons = XboxAutomationButtonFlags.Y_Button; break;
                case 'd': gamepad.Buttons = XboxAutomationButtonFlags.DPadDown; break;
                case 'u': gamepad.Buttons = XboxAutomationButtonFlags.DPadUp; break;
                case 'l': gamepad.Buttons = XboxAutomationButtonFlags.DPadLeft; break;
                case 'r': gamepad.Buttons = XboxAutomationButtonFlags.DPadRight; break;
                default: error = true; break;
            }

            if (error)
                return;

            foreach (IXboxAutomation auto in automation)
            {
                auto.QueueGamepadState(quadrant, gamepad, 200, 0);
            }
        }

        /// <summary>
        /// Gets or creates a profile and makes sure it is set to default
        /// </summary>
        /// <param name="manager">manager for the console</param>
        /// <param name="profileNumber">index of the profile to retrieve. a new profile is created if this index is too big. a max of one profile will be created</param>
        /// <returns></returns>
        private ConsoleProfile SafeGetDefaultProfile(ConsoleProfilesManager manager, int profileNumber)
        {
            ConsoleProfile profile = null;

            try
            {
                if (manager.EnumerateConsoleProfiles().Count() > profileNumber)
                {
                    profile = manager.EnumerateConsoleProfiles().ElementAt(profileNumber);
                }
                else
                {
                    profile = manager.CreateConsoleProfile(true);
                }

                manager.SetDefaultProfile(profile);
                profile.SignIn(UserIndex.Zero);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem getting a profile from " + manager.Console.Name + "\n\nException: " + ex.Message, "Certificaton Assistance Tool");
            }

            return profile;
        }

        /// <summary>
        /// Gets a value indicating whether whether there are three connected xboxes selected
        /// </summary>
        private bool ThreeConnectedXboxesSelected
        {
            get
            {
                string s = string.Empty;
                if (this.moduleContext.SelectedDevices.Count() == 0)
                {
                    s = "No consoles are selected. Select 3 for this module. ";
                }
                else if (this.moduleContext.SelectedDevices.Count() != 3)
                {
                    s = "Please select 3 consoles. " + this.moduleContext.SelectedDevices.Count().ToString() + " are selected.";
                }

                foreach (IXboxDevice dev in this.moduleContext.SelectedDevices)
                {
                    // connected
                    if (dev.Connected == false)
                    {
                        s += "The selected device " + dev.Name + " is not connected. Connect the device.";
                        break;
                    }
                }

                // if there were any error messages, fail
                if (!string.IsNullOrEmpty(s))
                {
                    MessageBox.Show(s, "Certification Assistance Tool");
                    return false;
                }

                return true;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}