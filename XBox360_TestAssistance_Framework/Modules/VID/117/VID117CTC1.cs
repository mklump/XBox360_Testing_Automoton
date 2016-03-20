// -----------------------------------------------------------------------
// <copyright file="VID117CTC1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace VID117
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using CAT;

    /// <summary>
    /// Video cable type
    /// </summary>
    public enum CableType
    {
        /// <summary>
        /// HDMI cable type
        /// </summary>
        HDMI,

        /// <summary>
        /// Component cable type
        /// </summary>
        Component,

        /// <summary>
        /// VGA cable type
        /// </summary>
        VGA,

        /// <summary>
        /// Composite cable type
        /// </summary>
        Composite,

        /// <summary>
        /// S-Video cable type
        /// </summary>
        SVideo,

        /// <summary>
        /// SCART cable type
        /// </summary>
        SCART,

        /// <summary>
        /// D-Terminal cable type
        /// </summary>
        DTerminal,

        /// <summary>
        /// Unknown cable type
        /// </summary>
        Unknown
    }
       
    /// <summary>
    /// Main Class for this Module
    /// </summary>
    public class VID117CTC1 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// A constant string used to generate a message in the UI regarding testing the current resolution
        /// </summary>
        private const string SettingModeMessage = "Current Mode = {0} {1}\n\nThe console has changed to resolution {0} {1} and is starting up {2}. Spend a few minutes check out the game under this resolution and tell us how it looks.";

        /// <summary>
        /// A constant string used to generate a message in the UI regarding progressing to the next resolution
        /// </summary>
        private const string WaitingForConfirmationMessage = "Current Mode = {0} {1}\n\nResult: {2}. If this is correct click next to try the next resolution.";

        /// <summary>
        /// A constant string used to generate a message in the UI regarding returning to the cable selection screen
        /// </summary>
        private const string LastModeMessage = "\n\nThis is the end of the {0} cable test. When you have verified the current resolution return to the Cable Selection screen.";

        /// <summary>
        /// The module context object passed in by CAT framework
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// The UI element associated with this module
        /// </summary>
        private VID117CTC1UI moduleUI;

        /// <summary>
        /// Drive the title is installed or, or to install the title on
        /// </summary>
        private string installDrive;

        /// <summary>
        /// A reference to the xbox being tested
        /// </summary>
        private IXboxDevice xboxDevice;

        /// <summary>
        /// Backing field for FirstPageVisibility property
        /// </summary>
        private Visibility firstPageVisibility = Visibility.Visible;

        /// <summary>
        /// Backing field for SecondPageVisibility property
        /// </summary>
        private Visibility secondPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for ThirdPageVisibility property
        /// </summary>
        private Visibility thirdPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for VGAVisibility property
        /// </summary>
        private Visibility vgaVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for HDMIVisibility property
        /// </summary>
        private Visibility hdmiVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for SVideoVisibility property
        /// </summary>
        private Visibility svideoVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for SCARTVisibility property
        /// </summary>
        private Visibility scartVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for CompositeVisibility property
        /// </summary>
        private Visibility compositeVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for ComponentVisibility property
        /// </summary>
        private Visibility componentVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for DTerminalVisibility property
        /// </summary>
        private Visibility dterminalVisibility = Visibility.Collapsed;

        /// <summary>
        /// Backing field for CanDoNextTest property
        /// </summary>
        private bool canDoNextTest = false;

        /// <summary>
        /// Backing field for CanSkipTest property
        /// </summary>
        private bool canSkipTest = true;

        /// <summary>
        /// The index into the current resolution list of the current resolution being tested
        /// </summary>
        private int currentTestIndex;

        /// <summary>
        /// The current resolution list (of a particular video format standard) being tested
        /// </summary>
        private ICableResList currentResList;

        /// <summary>
        /// The current resolution being testing
        /// </summary>
        private ResolutionItem currentResItem;

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Base class for cable resolution lists
        /// </summary>
        public interface ICableResList
        {
            /// <summary>
            /// Gets a list of resolutions to test for this cable type
            /// </summary>
            List<ResolutionItem> Resolutions { get; }

            /// <summary>
            /// Gets or sets the video format standard used
            /// </summary>
            VideoStandard Standard { get; set; }

            /// <summary>
            /// Gets a string representing the video format standard used
            /// </summary>
            string StandardString { get; }

            /// <summary>
            /// Gets or sets the Xbox console region to use.
            /// </summary>
            Region Region { get; set; }
        }

        /// <summary>
        /// Gets the UI element associated with this module
        /// </summary>
        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
        }

        /// <summary>
        /// Gets or sets the string displayed in the UI when testing a resolution
        /// </summary>
        public string ResolutionPageMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the first page should be visible 
        /// </summary>
        public Visibility FirstPageVisibility 
        {
            get 
            {
                return this.firstPageVisibility;
            }
            
            set
            {
                this.firstPageVisibility = value;
                this.NotifyPropertyChanged();
            } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the second page should be visible 
        /// </summary>
        public Visibility SecondPageVisibility 
        {
            get 
            {
                return this.secondPageVisibility; 
            }

            set 
            {
                this.secondPageVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the third page should be visible 
        /// </summary>
        public Visibility ThirdPageVisibility 
        {
            get 
            {
                return this.thirdPageVisibility; 
            }

            set 
            {
                this.thirdPageVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the VGA cable option should be visible
        /// </summary>
        public Visibility VGAVisibility 
        {
            get 
            {
                return this.vgaVisibility; 
            }

            set 
            {
                this.vgaVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the HDMI cable option should be visible
        /// </summary>
        public Visibility HDMIVisibility 
        {
            get
            {
                return this.hdmiVisibility; 
            }
            
            set
            {
                this.hdmiVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the S-Video cable option should be visible
        /// </summary>
        public Visibility SVideoVisibility
        {
            get
            {
                return this.svideoVisibility; 
            }
            
            set
            {
                this.svideoVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SCART cable option should be visible
        /// </summary>
        public Visibility SCARTVisibility 
        {
            get 
            {
                return this.scartVisibility; 
            }
            
            set
            {
                this.scartVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Composite cable option should be visible
        /// </summary>
        public Visibility CompositeVisibility
        {
            get
            {
                return this.compositeVisibility; 
            }
            
            set
            {
                this.compositeVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Component cable option should be visible
        /// </summary>
        public Visibility ComponentVisibility 
        {
            get 
            {
                return this.componentVisibility; 
            }
            
            set 
            {
                this.componentVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the D-Terminal cable option should be visible
        /// </summary>
        public Visibility DTerminalVisibility 
        {
            get
            {
                return this.dterminalVisibility; 
            }

            set
            {
                this.dterminalVisibility = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets or sets the list of all Video cables
        /// </summary>
        public ObservableCollection<VideoCable> VideoCables { get; set; }

        /// <summary>
        /// Gets or sets the current cable being tested
        /// </summary>
        public VideoCable CableUnderTest { get; set; }

        /// <summary>
        /// Gets or sets the list of all Video cables
        /// </summary>
        public Region? LastRegionUsed { get; set; }

        /// <summary>
        /// Gets or sets the contents of the last image screenshot
        /// </summary>
        public BitmapImage LastScreenshotImageContents { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether there is a next test
        /// </summary>
        public bool CanDoNextTest
        {
            get
            {
                return this.canDoNextTest;
            }

            set
            {
                this.canDoNextTest = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this text can be skipped
        /// </summary>
        public bool CanSkipTest
        {
            get
            {
                return this.canSkipTest;
            }

            set
            {
                this.canSkipTest = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a list of HDMI resolutions per format
        /// </summary>
        public ObservableCollection<ICableResList> HDMIObservedList { get; set; }

        /// <summary>
        /// Gets or sets a list of VGA resolutions per format
        /// </summary>
        public ObservableCollection<ICableResList> VGAObservedList { get; set; }

        /// <summary>
        /// Gets or sets a list of S-Video resolutions per format
        /// </summary>
        public ObservableCollection<ICableResList> SVideoObservedList { get; set; }

        /// <summary>
        /// Gets or sets a list of SCART resolutions per format
        /// </summary>
        public ObservableCollection<ICableResList> SCARTObservedList { get; set; }

        /// <summary>
        /// Gets or sets a list of Composite resolutions per format
        /// </summary>
        public ObservableCollection<ICableResList> CompositeObservedList { get; set; }

        /// <summary>
        /// Gets or sets a list of Component resolutions per format
        /// </summary>
        public ObservableCollection<ICableResList> ComponentObservedList { get; set; }

        /// <summary>
        /// Gets or sets a list of D-Terminal resolutions per format
        /// </summary>
        public ObservableCollection<ICableResList> DTerminalObservedList { get; set; }

        /// <summary>
        /// Gets a value indicating whether the current test is the last test
        /// </summary>
        private bool IsLastTest
        {
            get
            {
                bool currentListFound = false;
                foreach (ICableResList resList in this.CableUnderTest.ResolutionLists)
                {
                    int indexStart = 0;

                    // skip preceeding resolution lists
                    if (this.currentResList == resList) 
                    {
                        currentListFound = true;
                        indexStart = this.currentTestIndex + 1;
                    }

                    if (currentListFound)
                    {
                        for (int i = indexStart; i < resList.Resolutions.Count; i++)
                        {
                            if (string.IsNullOrEmpty(resList.Resolutions[i].Result))
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether only one connected xbox is selected
        /// </summary>
        /// <returns>true if exactly one Xbox is selected and that Xbox is connected</returns>
        private bool IsOneConnectedXboxSelected
        {
            get
            {
                string s = string.Empty;
                if (this.moduleContext.SelectedDevices.Count() == 0)
                {
                    // at least one
                    s += "No consoles are selected. Select one. ";
                }
                else if (this.moduleContext.SelectedDevices.Count() > 1)
                {
                    // only one
                    s += this.moduleContext.SelectedDevices.Count().ToString() + " consoles are selected. Select just one. ";
                }

                foreach (IXboxDevice device in this.moduleContext.SelectedDevices)
                {
                    if (device.IsSelected)
                    {
                        // connected
                        if (!device.Connected)
                        {
                            s += "The selected device " + device.Name + " is not connected. Connect the device.";
                        }

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

        /// <summary>
        /// Gets a value indicating whether the current resolution is valid for the current cable type being tested 
        /// </summary>
        private bool CurrentResolutionIsValidForCable
        {
            get
            {
                // TBD check for more cases
                try
                {
                    int width;
                    int height;
                    this.xboxDevice.GetCurrentResolution(out width, out height);
                    if (height > 576)
                    {
                        switch (this.CableUnderTest.Type)
                        {
                            case CableType.SVideo:
                            case CableType.SCART:
                            case CableType.Composite:
                                MessageBox.Show(
                                    "We've detected that the " +
                                    this.CableUnderTest.Name + 
                                    " cable may not be connected. Please check the cable is connected before continueing",
                                    "Certification Assistance Tool");
                                return false;
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Start - called when the module is first entered
        /// This function is called to show the overview or intro to the module.
        /// Typically the framework is active and user should choose a device in the device pool.
        /// </summary>
        /// <param name="ctx">The current working context for which this test will execute.</param>
        public void Start(IModuleContext ctx)
        {
            this.moduleContext = ctx as IXboxModuleContext;
            this.InitVideoLists();
            this.CanDoNextTest = false;
            this.CanSkipTest = true;
            this.LastRegionUsed = null;
            this.moduleUI = new VID117CTC1UI(this);
        }

        /// <summary>
        /// NextPage - called to leave the the module overview or intro screen entered by Start()
        /// The framework goes modal in this call and the module gains control.
        /// This function is called repeatedly to advance to multiple screens in the module.
        /// </summary>
        public void NextPage()
        {
            if (this.IsOneConnectedXboxSelected)
            {
                this.xboxDevice = (IXboxDevice)this.moduleContext.SelectedDevices[0];
                if (string.IsNullOrEmpty(this.moduleContext.XboxTitle.Name))
                {
                    MessageBox.Show("Please Select a Title", "Certification Assistance Tool");
                }
                else
                {
                    if (this.FirstPageVisibility == Visibility.Visible)
                    {
                        this.FirstPageVisibility = Visibility.Collapsed;
                        this.SecondPageVisibility = Visibility.Visible;
                        this.moduleContext.IsModal = true;
                        if (this.moduleContext.XboxTitle.GameInstallType != "Disc Emulation")
                        {
                            this.InstallTitle();
                        }
                    }
                    else if (this.secondPageVisibility == Visibility.Visible)
                    {
                        this.SecondPageVisibility = Visibility.Collapsed;
                        this.ThirdPageVisibility = Visibility.Visible;
                    }
                    else if (this.thirdPageVisibility == Visibility.Visible)
                    {
                        this.SecondPageVisibility = Visibility.Visible;
                        this.ThirdPageVisibility = Visibility.Collapsed;
                    }
                }
            }
        }

        /// <summary>
        /// Stop - called when the module is done or aborted
        /// </summary>
        public void Stop()
        {
            Mouse.OverrideCursor = Cursors.Wait;

            // Convert all result images to jpg's
            foreach (VideoCable videoCable in this.VideoCables)
            {
                foreach (ICableResList res in videoCable.ResolutionLists)
                {
                    foreach (ResolutionItem resItem in res.Resolutions)
                    {
                        if (!string.IsNullOrEmpty(resItem.ResultImageFile))
                        {
                            resItem.ResultImageFile = Path.Combine(this.moduleContext.LogDirectory, resItem.ResultImageFile);
                        }
                    }
                }
             }

            this.CreateHTMLLog("captures.html", false);
            this.CreateHTMLLog("failed.html", true);

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Resets a cable for testing
        /// </summary>
        public void StartFirstTest()
        {
            ObservableCollection<ICableResList> resolutions = this.CableUnderTest.ResolutionLists;
            this.CableUnderTest.StartedTesting = true;
            this.CanDoNextTest = false;
            this.CanSkipTest = true;

            // In case this cable was already attempted, clear it out.
            foreach (ICableResList list in resolutions)
            {
                foreach (ResolutionItem resItem in list.Resolutions)
                {
                    // mark previous resolution as done
                    resItem.Result = null;
                    resItem.IsActive = false;
                    resItem.StatusImage = @"Images\untested.png";
                    if (!string.IsNullOrEmpty(resItem.ResultImageFile))
                    {
                        try
                        {
                            File.Delete(resItem.ResultImageFile);
                        }
                        catch (Exception)
                        {
                        }

                        resItem.ResultImageFile = null;
                    }
                }
            }

            this.StartNextTest();
        }

        /// <summary>
        /// Change cable under test
        /// </summary>
        /// <param name="cableType">The type of cable type being tested</param>
        public void SetCable(CableType cableType)
        {
            // Change current Cable
            foreach (VideoCable videoCable in this.VideoCables)
            {
                if (videoCable.Type == cableType)
                {
                    this.CableUnderTest = videoCable;
                }
                else
                {
                    videoCable.CurrentlyTesting = false;
                }
            }

            this.VGAVisibility = Visibility.Collapsed;
            this.HDMIVisibility = Visibility.Collapsed;
            this.SVideoVisibility = Visibility.Collapsed;
            this.SCARTVisibility = Visibility.Collapsed;
            this.CompositeVisibility = Visibility.Collapsed;
            this.ComponentVisibility = Visibility.Collapsed;
            this.DTerminalVisibility = Visibility.Collapsed;

            switch (cableType)
            {
                case CableType.VGA:
                    this.VGAVisibility = Visibility.Visible;
                    break;
                case CableType.HDMI:
                    this.HDMIVisibility = Visibility.Visible;
                    break;
                case CableType.SVideo:
                    this.SVideoVisibility = Visibility.Visible; 
                    break;
                case CableType.SCART:
                    this.SCARTVisibility = Visibility.Visible;
                    break;
                case CableType.Composite:
                    this.CompositeVisibility = Visibility.Visible; 
                    break;
                case CableType.Component:
                    this.ComponentVisibility = Visibility.Visible; 
                    break;
                case CableType.DTerminal:
                    this.DTerminalVisibility = Visibility.Visible; 
                    break;
            }

            // clear test screen elements
            this.ResolutionPageMessage = string.Empty;
            this.LastScreenshotImageContents = null;
            this.NotifyPropertyChanged("LastScreenshotImageContents");

            if (this.CableUnderTest.DoneTesting)
            {
                // if this cable is fully tested, tell 'em so
                MessageBoxResult messageBoxResult = MessageBox.Show("This cable has already been tested at all resolutions.  Do you want to reset tests of this cable?", "Certification Assistance Tool", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    this.CableUnderTest.CurrentlyTesting = false;
                    this.CableUnderTest.StartedTesting = false;
                }
            }

            // Otherwise, if this cable is partially tested, just go back to where we were
            if (!this.CableUnderTest.CurrentlyTesting)
            {
                this.currentResList = null;

                // set console to the first resolution
                this.CableUnderTest.CurrentlyTesting = true;

                // If we had not yet started testing, start from the beginning, otherwise continue
                if (this.CableUnderTest.StartedTesting)
                {
                    this.StartNextTest();
                }
                else
                {
                    this.StartFirstTest();
                }
            }

            this.NextPage();
        }

        /// <summary>
        /// Test a video cable
        /// </summary>
        public void TestCable()
        {
            this.LastScreenshotImageContents = null;
            this.NotifyPropertyChanged("LastScreenshotImageContents");
            if (this.CableUnderTest == null)
            {
                MessageBox.Show("Please Select a Cable", "Certification Assistance Tool");
                return;
            }

            // check cable has been changed
            if (!this.CurrentResolutionIsValidForCable)
            {
                return;
            }

            // disable moving to next test until this one is complete
            this.CanDoNextTest = false;
            this.CanSkipTest = true;
            this.CableUnderTest.DoneTesting = false;

            // start testing
            this.StartNextTest(); 

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Repositions testing to the specified resolution
        /// </summary>
        /// <param name="resList">Resolution list for the video format to switch to</param>
        /// <param name="index">Index of resolution to switch to</param>
        public void Reposition(ICableResList resList, int index)
        {
            this.LastScreenshotImageContents = null;
            this.NotifyPropertyChanged("LastScreenshotImageContents");

            ResolutionItem newResItem = resList.Resolutions[index];
            if (newResItem == this.currentResItem)
            {
                // Force retry of region change
                this.LastRegionUsed = null;
                this.StartTest();
            }
            else
            {
                this.currentResItem.IsActive = false;
                this.currentResList = resList;
                this.currentTestIndex = index;
                this.currentResItem = resList.Resolutions[index];
                this.currentResItem.IsActive = true;
            }

            this.StartTest();
        }

        /// <summary>
        /// Confirm testing of a video cable
        /// </summary>
        /// <param name="result">A string representing the result of the test</param>
        public void ConfirmCable(string result)
        {
            this.DoCableConfirm(this.CableUnderTest.ResolutionLists, result);
            
            if (this.CableUnderTest.TestingLastResolution)
            {
                this.CanDoNextTest = false;
                this.CanSkipTest = false;
            }
            else
            {
                this.CanDoNextTest = true;
                this.CanSkipTest = false;
            }

            // Check if done testing
            this.CableUnderTest.DoneTesting = true;
            foreach (ICableResList resList in this.CableUnderTest.ResolutionLists)
            {
                foreach (ResolutionItem resItem in resList.Resolutions)
                {
                    if (string.IsNullOrEmpty(resItem.Result))
                    {
                        this.CableUnderTest.DoneTesting = false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Loads a image from file into a BitmapImage object
        /// </summary>
        /// <param name="fileName">File to load bitmap from</param>
        /// <returns>A BitmapImage object containing the bitmap image from the specified file</returns>
        private static BitmapImage LoadImage(string fileName)
        {
            BitmapImage result = null;
            if (!string.IsNullOrEmpty(fileName))
            {
                result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                result.UriSource = new Uri(fileName, UriKind.Absolute);
                result.EndInit();
            }

            return result;
        }

        #region converters

        /// <summary>
        /// Converts a video format standard to a string
        /// </summary>
        /// <param name="std">The video format standard to convert</param>
        /// <returns>A string representing the specified video standard format</returns>
        private static string StandardToString(VideoStandard std)
        {
            string result = string.Empty;
            switch (std)
            {
                case VideoStandard.NTSCJ:
                    result = "NTSC-J";
                    break;
                case VideoStandard.NTSCM:
                    result = "NTSC-M";
                    break;
                case VideoStandard.PAL50:
                    result = "PAL 50";
                    break;
                case VideoStandard.PAL60:
                    result = "PAL 60";
                    break;
            }

            return result;
        }

        /// <summary>
        /// Converts a Region enumeration to a string
        /// </summary>
        /// <param name="region">Region to convert to a string</param>
        /// <returns>A string representing the specified region</returns>
        private static string RegionToString(Region region)
        {
            string result = string.Empty;
            switch (region)
            {
                case Region.Asia:
                    result = "Asia";
                    break;
                case Region.Australia:
                    result = "Australia";
                    break;
                case Region.China:
                    result = "China";
                    break;
                case Region.Europe:
                    result = "Europe";
                    break;
                case Region.Japan:
                    result = "Japan";
                    break;
                case Region.NorthAmerica:
                    result = "North America";
                    break;
                case Region.RestOfWorld:
                    result = "Rest of world";
                    break;
            }

            return result;
        }

        /// <summary>
        /// Converts a video resolution to a string.
        /// </summary>
        /// <param name="res">The video resolution to convert to a string</param>
        /// <param name="std">The video format standard for the specified resolution</param>
        /// <returns>A string representing the specified video format standard and resolution</returns>
        private static string ResolutionToString(VideoResolution res, VideoStandard std)
        {
            string result = string.Empty;
            switch (res)
            {
                case VideoResolution.Mode640x480:
                    result = "640x480";
                    break;
                case VideoResolution.Mode640x480Wide:
                    result = "640x480 widescreen";
                    break;
                case VideoResolution.Mode848x480:
                    result = "848x480";
                    break;
                case VideoResolution.Mode1024x768:
                    result = "1024x768";
                    break;
                case VideoResolution.Mode1024x768Wide:
                    result = "1024x768 wide";
                    break;
                case VideoResolution.Mode1280x720:
                    result = "1280x720";
                    break;
                case VideoResolution.Mode1280x768:
                    result = "1280x768";
                    break;
                case VideoResolution.Mode1280x1024:
                    result = "1280x1024";
                    break;
                case VideoResolution.Mode1280x1024Wide:
                    result = "1280x1024 wide";
                    break;
                case VideoResolution.Mode1360x768:
                    result = "1360x768";
                    break;
                case VideoResolution.Mode1440x900:
                    result = "1440x900";
                    break;
                case VideoResolution.Mode1680x1050:
                    result = "1680x1050";
                    break;
                case VideoResolution.Mode1920x1080:
                    result = "1920x1080";
                    break;
                case VideoResolution.Mode480:
                    if (std == VideoStandard.PAL50 || std == VideoStandard.PAL60)
                    {
                        result = "480i";
                    }
                    else
                    {
                        result = "480p";
                    }

                    break;
                case VideoResolution.Mode480Wide:
                    if (std == VideoStandard.PAL50 || std == VideoStandard.PAL60)
                    {
                        result = "480i widescreen";
                    }
                    else
                    {
                        result = "480p widescreen";
                    }

                    break;
                case VideoResolution.Mode720p:
                    result = "720p";
                    break;
                case VideoResolution.Mode1080i:
                    result = "1080i";
                    break;
                case VideoResolution.Mode1080p:
                    result = "1080p";
                    break;
                default:
                    result = "Unknown resolution";
                    break;
            }

            return result;
        }

        #endregion

        #region HTML Logging region

        /// <summary>
        /// Add the HTML header to the HTML log file
        /// </summary>
        /// <param name="logFilename">HTML log file to log to</param>
        private void AddHeaderToHTMLLog(string logFilename)
        {
            string htmlContent = "<HTML>\r\n<TITLE>Captures</TITLE>\r\n<HEAD><meta http-equiv='Content-Type' content='text/html; charset=utf-8'></HEAD><BODY>\r\n<TABLE>\r\n";
            File.AppendAllText(Path.Combine(this.moduleContext.LogDirectory, logFilename), htmlContent, Encoding.UTF8);
        }

        /// <summary>
        /// Adds the HTML footer to the HTML log file
        /// </summary>
        /// <param name="logFilename">HTML log file to log to</param>
        private void AddFooterToHTMLLog(string logFilename)
        {
            string htmlContent = "</TABLE>\r\n</BODY>\r\n</HTML>";
            File.AppendAllText(Path.Combine(this.moduleContext.LogDirectory, logFilename), htmlContent, Encoding.UTF8);
        }

        /// <summary>
        /// Adds a cable-specified header to the HTML log file
        /// </summary>
        /// <param name="logFilename">HTML log file to log to</param>
        /// <param name="cableName">The name of the cable</param>
        private void AddCableHeaderToHTMLLog(string logFilename, string cableName)
        {
            string htmlContent = "<TR><TD colspan='100%' align='left'>Cable: " + cableName + "</TD></TR>";
            File.AppendAllText(Path.Combine(this.moduleContext.LogDirectory, logFilename), htmlContent, Encoding.UTF8);
        }

        /// <summary>
        /// Adds a cable-specified footer to the HTML log file
        /// </summary>
        /// <param name="logFilename">HTML log file to log to</param>
        private void AddCableFooterToHTMLLog(string logFilename)
        {
            string htmlContent = "<TR><TD colspan='100%' align='left'><HR></TD></TR>";
            File.AppendAllText(Path.Combine(this.moduleContext.LogDirectory, logFilename), htmlContent, Encoding.UTF8);
        }

        /// <summary>
        /// Adds a format-specific header to the HTML log file
        /// </summary>
        /// <param name="logFilename">HTML log file to log to</param>
        /// <param name="formatName">The name of the format</param>
        private void AddFormatHeaderToHTMLLog(string logFilename, string formatName)
        {
            string htmlContent = "<TR><TD colspan='100%' align='left'><BR>Mode: " + formatName + "</TD></TR><TR>";
            File.AppendAllText(Path.Combine(this.moduleContext.LogDirectory, logFilename), htmlContent, Encoding.UTF8);
        }

        /// <summary>
        /// Adds a format-specific footer to the HTML log file
        /// </summary>
        /// <param name="logFilename">HTML log file to log to</param>
        private void AddFormatFooterToHTMLLog(string logFilename)
        {
            string htmlContent = "</TR>";
            File.AppendAllText(Path.Combine(this.moduleContext.LogDirectory, logFilename), htmlContent, Encoding.UTF8);
        }

        /// <summary>
        /// Adds capture and caption for a specific cable and format, to the HTML log file
        /// </summary>
        /// <param name="logFilename">HTML log file to log to</param>
        /// <param name="imageFile">Path to the screen snapshot image file for this resolution</param>
        /// <param name="videoResolution">The video resolution of this screen snapshot</param>
        /// <param name="videoStandard">The video standard of this screen snapshot</param>
        /// <param name="pass">Whether or not this image was considering passing</param>
        private void AddCaptureToHTMLLog(string logFilename, string imageFile, VideoResolution videoResolution, VideoStandard videoStandard, bool pass)
        {
            string videoResolutionString = ResolutionToString(videoResolution, videoStandard);
            string passOrFailString;
            if (pass)
            {
                passOrFailString = "PASS";
            }
            else
            {
                passOrFailString = "FAIL";
            }

            string htmlContent = "<TD align='center'><A HREF='" + imageFile + "'><IMG SRC='" + imageFile + "' width='160' height='120' /></A><BR>\r\n" + videoResolutionString + "<BR>\r\n" + passOrFailString + "</TD>\r\n";
            File.AppendAllText(Path.Combine(this.moduleContext.LogDirectory, logFilename), htmlContent, Encoding.UTF8);
        }

        /// <summary>
        /// Adds log output for a specific cable to the HTML log
        /// </summary>
        /// <param name="resolutions">The resolutions associated with this cable</param>
        /// <param name="cableName">Name of this cable</param>
        /// <param name="logFileName">Path to the HTML log file to log to</param>
        /// <param name="failingOnly">Whether or not to only log failed cases</param>
        /// <returns>True if there is any data to log for this cable, false otherwise</returns>
        private bool AddCableToHTMLLog(ObservableCollection<ICableResList> resolutions, string cableName, string logFileName, bool failingOnly)
        {
            bool anySnapshots = false;
            bool anyThisCable = false;
            foreach (ICableResList res in resolutions)
            {
                bool anyThisFormat = false;
                foreach (ResolutionItem resItem in res.Resolutions)
                {
                    if ((!string.IsNullOrEmpty(resItem.ResultImageFile)) && (!failingOnly || resItem.Result != "good"))
                    {
                        if (!anySnapshots)
                        {
                            this.AddHeaderToHTMLLog(logFileName);
                            anySnapshots = true;
                        }

                        if (!anyThisCable)
                        {
                            this.AddCableHeaderToHTMLLog(logFileName, cableName);
                            anyThisCable = true;
                        }

                        if (!anyThisFormat)
                        {
                            this.AddFormatHeaderToHTMLLog(logFileName, res.StandardString);
                            anyThisFormat = true;
                        }

                        this.AddCaptureToHTMLLog(logFileName, resItem.ResultImageFile, resItem.VideoResolution, res.Standard, resItem.Result == "good");
                    }
                }
            }

            if (anyThisCable)
            {
                this.AddCableFooterToHTMLLog(logFileName);
            }

            return anySnapshots;
        }

        /// <summary>
        /// Generates an HTML log file with the results of this module
        /// </summary>
        /// <param name="logFileName">Path to the HTML log file to log to</param>
        /// <param name="failingOnly">Whether or not to only log failed cases</param>
        private void CreateHTMLLog(string logFileName, bool failingOnly)
        {
            bool anySnapshots = false;
            foreach (VideoCable videoCable in this.VideoCables)
            {
                anySnapshots |= this.AddCableToHTMLLog(videoCable.ResolutionLists, videoCable.Name, logFileName, failingOnly);
            }

            if (anySnapshots)
            {
                this.AddFooterToHTMLLog(logFileName);
            }
        }

        #endregion

        /// <summary>
        /// Captures a screen snapshot and reports a result
        /// </summary>
        /// <param name="resItem">The resolution item associated with the current test</param>
        /// <param name="result">The result of the current test</param>
        /// <param name="videoStandard">The video format standard used in the current test</param>
        private void CaptureAndReportResult(ResolutionItem resItem, string result, VideoStandard videoStandard)
        {
            VideoResolution res = resItem.VideoResolution;
            string resName = ResolutionToString(res, videoStandard);
            string standardName = StandardToString(videoStandard);

            // get screen shot
            resItem.Result = result;
            resItem.ResultImageFile = this.CableUnderTest.Name + standardName + resName + ".jpg";
            resItem.ResultImageFile = resItem.ResultImageFile.Replace('-', '_');
            string imageFullPath = Path.Combine(this.moduleContext.LogDirectory, resItem.ResultImageFile);
            Mouse.OverrideCursor = Cursors.Wait;
            this.xboxDevice.ScreenShot(imageFullPath);
            Mouse.OverrideCursor = null;
            this.LastScreenshotImageContents = LoadImage(imageFullPath);
            this.NotifyPropertyChanged("LastScreenshotImageContents");
            Mouse.OverrideCursor = null;

            // update UI with result
            this.ResolutionPageMessage = string.Format(WaitingForConfirmationMessage, standardName, resName, result);
            if (this.CableUnderTest.TestingLastResolution)
            {
                this.ResolutionPageMessage += string.Format(LastModeMessage, this.CableUnderTest.Name);
            }

            this.NotifyPropertyChanged("ResolutionPageMessage");
        }

        /// <summary>
        /// Starts the next test.
        /// </summary>
        private void StartNextTest()
        {
            if (this.currentResList == null)
            {
                this.currentResList = this.CableUnderTest.ResolutionLists.First();
                this.currentResItem = this.currentResList.Resolutions[0];
                this.currentTestIndex = -1;

                // Clear up the active field in case we're returning to something we were in the middle of
                foreach (ICableResList resList in this.CableUnderTest.ResolutionLists)
                {
                    foreach (ResolutionItem resItem in resList.Resolutions)
                    {
                        resItem.IsActive = false;
                    }
                }
            }
            else
            {
                this.currentResItem.IsActive = false;
                if (string.IsNullOrEmpty(this.currentResItem.Result))
                {
                    this.currentResItem.StatusImage = @"Images\skipped.png";
                    this.currentResItem.Result = "skipped";

                    // Check if done testing
                    this.CableUnderTest.DoneTesting = true;
                    foreach (ICableResList resList in this.CableUnderTest.ResolutionLists)
                    {
                        foreach (ResolutionItem resItem in resList.Resolutions)
                        {
                            if (string.IsNullOrEmpty(resItem.Result))
                            {
                                this.CableUnderTest.DoneTesting = false;
                                break;
                            }
                        }
                    }
                }
            }

            ResolutionItem previousResItem = this.currentResItem;
            int previousTestIndex = this.currentTestIndex;

            do
            {
                this.currentTestIndex++;
                if (this.currentTestIndex >= this.currentResList.Resolutions.Count())
                {
                    // Advance to next resolution list
                    ObservableCollection<ICableResList> resolutions = this.CableUnderTest.ResolutionLists;
                    for (int i = 0; i < resolutions.Count; i++)
                    {
                        ICableResList resList = resolutions[i];
                        if (resList == this.currentResList)
                        {
                            if (i < resolutions.Count - 1)
                            {
                                this.currentTestIndex = 0;
                                this.currentResList = resolutions[i + 1];
                            }
                            else
                            {
                                if (previousResItem == null)
                                {
                                    // No untested item was found, go to the first item
                                    this.currentTestIndex = 0;
                                    this.currentResList = resolutions[0];
                                }
                                else
                                {
                                    // Unable to step past the end of the list, return to where it was
                                    this.currentResItem = previousResItem;
                                    this.currentTestIndex = previousTestIndex;
                                    this.currentResItem.IsActive = true;
                                    this.CanSkipTest = false;
                                    this.CanDoNextTest = false;
                                    return;
                                }
                            }

                            break;
                        }
                    }
                }

                this.currentResItem = this.currentResList.Resolutions[this.currentTestIndex];
            }
            while (!string.IsNullOrEmpty(this.currentResItem.Result));

            this.currentResItem.IsActive = true;
            this.StartTest();
        }

        /// <summary>
        /// Starts the next test.
        /// </summary>
        private void StartTest()
        {
            if (string.IsNullOrEmpty(this.currentResItem.Result))
            {
                this.CanDoNextTest = false;
                this.CanSkipTest = true;
            }
            else
            {
                this.CanDoNextTest = true;
                this.CanSkipTest = false;
            }

            // check if this is the last item that requires testing
            this.CableUnderTest.TestingLastResolution = this.IsLastTest;
            if (this.CableUnderTest.TestingLastResolution)
            {
                if (!string.IsNullOrEmpty(this.currentResItem.Result))
                {
                    this.CanDoNextTest = false;
                }
            }

            VideoResolution res = this.currentResItem.VideoResolution;
            string resName = ResolutionToString(res, this.currentResList.Standard);

            // update UI with test start
            Mouse.OverrideCursor = Cursors.Wait;
            this.ResolutionPageMessage = string.Format(SettingModeMessage, this.currentResList.StandardString, resName, this.moduleContext.XboxTitle.Name);
            this.NotifyPropertyChanged("ResolutionPageMessage");

            // If the first in this resolution list, also change the region
            if (this.currentResList.Region != this.LastRegionUsed)
            {
                this.xboxDevice.SetRegion(this.currentResList.Region);
                this.LastRegionUsed = this.currentResList.Region;
            }

            this.InstallTitle();

            // change resolution on monitor
            this.SetVideoMode(res, this.currentResList.Standard);
            this.xboxDevice.LaunchTitle(this.installDrive);

            Mouse.OverrideCursor = null;
        }
        
        /// <summary>
        /// Confirm VGA cable
        /// </summary>
        /// <param name="resolutions">A list of resolutions for this cable</param>
        /// <param name="result">The result of the test</param>
        private void DoCableConfirm(ObservableCollection<ICableResList> resolutions, string result)
        {
            foreach (ICableResList list in resolutions)
            {
                foreach (ResolutionItem resItem in list.Resolutions)
                {
                    if (!resItem.IsActive)
                    {
                        continue;
                    }

                    VideoResolution res = resItem.VideoResolution;
                    switch (result)
                    {
                        case "good": 
                            resItem.StatusImage = "Images/good.png"; 
                            break;
                        case "bad":
                            resItem.StatusImage = "Images/bad.png"; 
                            break;
                        case "real bad": 
                            resItem.StatusImage = "Images/bad.png";
                            break;
                    }

                    this.CaptureAndReportResult(resItem, result, list.Standard);
                    return; // only update the selected one
                } // next resolutions
            } // nextstandard
        }

        /// <summary>
        /// Install the currently configured game title
        /// </summary>
        /// <returns>true if successful, false on failure</returns>
        private bool InstallTitle()
        {
            if (this.xboxDevice.IsTitleInstalled)
            {
                this.installDrive = this.xboxDevice.GetAnyDriveContentPackageBasedTitleIsInstalledOn;
                return true;
            }

            if (this.moduleContext.XboxTitle.GameInstallType == "Disc Emulation")
            {
                Mouse.OverrideCursor = Cursors.Wait;
                this.xboxDevice.LaunchDevDashboard();
                this.xboxDevice.InstallTitle(string.Empty, this.moduleContext.OpenProgressBarWindow("Installing " + this.moduleContext.XboxTitle.Name + "..."));
                Mouse.OverrideCursor = null;
                return true;
            }

            // The title is not installed. Find a drive
            foreach (string drv in this.xboxDevice.Drives)
            {
                if (drv.Contains("HDD"))
                {
                    this.installDrive = drv;
                    break;
                }
            }

            if (string.IsNullOrEmpty(this.installDrive))
            {
                foreach (string drv in this.xboxDevice.Drives)
                {
                    if (drv.Contains("USB") || drv.Contains("MU"))
                    {
                        this.installDrive = drv;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(this.installDrive))
            {
                MessageBox.Show(
                    "We weren't able to find any drives on " +
                    this.xboxDevice.Name + 
                    "\n\nCheck that the harddrive is enabled for a raw package game or that the console " +
                    "has a USB or MU storage device attached for a content package.  Restart the module",
                    "Certification Assistance Tool");
                return false;
            }

            // install the content package on any drive
            if (this.moduleContext.XboxTitle.GameInstallType == "Content Package")
            {
                if (MessageBoxResult.Yes == MessageBox.Show(
                    this.moduleContext.XboxTitle.Name +
                    " does not appear to be installed.\n\nInstall it now?",
                    "Certification Assistance Tool", 
                    MessageBoxButton.YesNo))
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    this.xboxDevice.LaunchDevDashboard();
                    this.xboxDevice.InstallTitle(this.installDrive, this.moduleContext.OpenProgressBarWindow("Installing " + this.moduleContext.XboxTitle.Name + "..."));
                    Mouse.OverrideCursor = null;
                    return true;
                }
            }

            // install a raw package on a harddrive only
            if (this.moduleContext.XboxTitle.GameInstallType == "Raw")
            {
                if (this.installDrive.Contains("HDD"))
                {
                    if (MessageBoxResult.Yes == MessageBox.Show(
                        this.moduleContext.XboxTitle.Name + 
                        " does not appear to be installed.\n\nInstall it now?",
                        "Certification Assistance Tool",
                        MessageBoxButton.YesNo))
                    {
                        Mouse.OverrideCursor = Cursors.Wait;
                        this.xboxDevice.LaunchDevDashboard();
                        this.xboxDevice.InstallTitle(this.installDrive, this.moduleContext.OpenProgressBarWindow("Installing " + this.moduleContext.XboxTitle.Name + "..."));
                        Mouse.OverrideCursor = null;
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show(
                        "We weren't able to find a hard drive on " +
                        this.xboxDevice.Name +
                        "\n\nCheck that the harddrive is enabled and restart the module",
                        "Certification Assistance Tool");
                }
            }

            return false;
        }

        /// <summary>
        /// Set the video mode of the Xbox
        /// </summary>
        /// <param name="resolution">The video resolution to set to</param>
        /// <param name="std">The video format standard to set to</param>
        private void SetVideoMode(VideoResolution resolution, VideoStandard std)
        {
            this.xboxDevice.SetVideoMode(resolution, std, false);
        }

        /// <summary>
        /// NotifyPropertyChanged triggers the PropertyChanged event for the specified property
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Initializes video resolutions lists
        /// </summary>
        private void InitVideoLists()
        {
            this.VGAObservedList = new ObservableCollection<ICableResList>();
            this.VGAObservedList.Add(new VGAResList(Region.NorthAmerica));
            this.VGAObservedList.Add(new VGAResList(Region.Japan));
            this.VGAObservedList.Add(new VGAResList(Region.Europe));

            this.HDMIObservedList = new ObservableCollection<ICableResList>();
            this.HDMIObservedList.Add(new HDMIResList(Region.NorthAmerica));
            this.HDMIObservedList.Add(new HDMIResList(Region.Japan));
            this.HDMIObservedList.Add(new HDMIResList(Region.Europe));

            this.SVideoObservedList = new ObservableCollection<ICableResList>();
            this.SVideoObservedList.Add(new SVideoResList(Region.NorthAmerica, VideoStandard.NTSCM));
            this.SVideoObservedList.Add(new SVideoResList(Region.Japan, VideoStandard.NTSCJ));
            this.SVideoObservedList.Add(new SVideoResList(Region.Europe, VideoStandard.PAL50));
            this.SVideoObservedList.Add(new SVideoResList(Region.Europe, VideoStandard.PAL60));

            this.SCARTObservedList = new ObservableCollection<ICableResList>();
            this.SCARTObservedList.Add(new SCARTResList(Region.Europe, VideoStandard.PAL50));
            this.SCARTObservedList.Add(new SCARTResList(Region.Europe, VideoStandard.PAL60));

            this.CompositeObservedList = new ObservableCollection<ICableResList>();
            this.CompositeObservedList.Add(new CompositeResList(Region.NorthAmerica, VideoStandard.NTSCM));
            this.CompositeObservedList.Add(new CompositeResList(Region.Japan, VideoStandard.NTSCJ));
            this.CompositeObservedList.Add(new CompositeResList(Region.Europe, VideoStandard.PAL50));
            this.CompositeObservedList.Add(new CompositeResList(Region.Europe, VideoStandard.PAL60));

            this.ComponentObservedList = new ObservableCollection<ICableResList>();
            this.ComponentObservedList.Add(new ComponentResList(Region.NorthAmerica, VideoStandard.NTSCM));
            this.ComponentObservedList.Add(new ComponentResList(Region.Japan, VideoStandard.NTSCJ));
            this.ComponentObservedList.Add(new ComponentResList(Region.Europe, VideoStandard.PAL50));
            this.ComponentObservedList.Add(new ComponentResList(Region.Europe, VideoStandard.PAL60));

            this.DTerminalObservedList = new ObservableCollection<ICableResList>();
            this.DTerminalObservedList.Add(new DTerminalResList(Region.Japan, VideoStandard.NTSCJ));

            // setup cables list
            this.VideoCables = new ObservableCollection<VideoCable>();
            this.VideoCables.Add(new VideoCable("VGA", CableType.VGA, this.VGAObservedList));
            this.VideoCables.Add(new VideoCable("HDMI", CableType.HDMI, this.HDMIObservedList));
            this.VideoCables.Add(new VideoCable("S-Video", CableType.SVideo, this.SVideoObservedList));
            this.VideoCables.Add(new VideoCable("SCART", CableType.SCART, this.SCARTObservedList));
            this.VideoCables.Add(new VideoCable("Composite", CableType.Composite, this.CompositeObservedList));
            this.VideoCables.Add(new VideoCable("Component", CableType.Component, this.ComponentObservedList));
            this.VideoCables.Add(new VideoCable("D-Terminal", CableType.DTerminal, this.DTerminalObservedList));
        }

        /// <summary>
        /// ResolutionItem contains the result of a test against a specific resolution (res, cable, and format)
        /// </summary>
        public class ResolutionItem : INotifyPropertyChanged
        {
            /// <summary>
            /// Video resolution tested against
            /// </summary>
            private readonly VideoResolution videoResolution;

            /// <summary>
            /// Backing field for StatusImage property
            /// </summary>
            private string statusImage;

            /// <summary>
            /// Backing field for IsActive property
            /// </summary>
            private bool isActive;

            /// <summary>
            /// Initializes a new instance of the <see cref="ResolutionItem" /> class.
            /// </summary>
            /// <param name="videoResolution">Resolution to use</param>
            public ResolutionItem(VideoResolution videoResolution)
            {
                this.videoResolution = videoResolution;
                this.statusImage = @"Images\untested.png";
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets the video resolution tested against
            /// </summary>
            public VideoResolution VideoResolution
            {
                get { return this.videoResolution; }
            }

            /// <summary>
            /// Gets or sets a string indicating the result of the test.  Possible values are: "good", "bad" and "really bad"
            /// </summary>
            public string Result { get; set; }

            /// <summary>
            /// Gets or sets a file path to a snapshot file taken when in the tested resolution
            /// </summary>
            public string ResultImageFile { get; set; }

            /// <summary>
            /// Gets or sets the file path to a status image for this resolution
            /// </summary>
            public string StatusImage
            {
                get
                {
                    return this.statusImage;
                }

                set
                {
                    this.statusImage = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this resolution is the active one being tested
            /// </summary>
            public bool IsActive
            {
                get
                {
                    return this.isActive;
                }

                set
                {
                    this.isActive = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// NotifyPropertyChanged triggers the PropertyChanged event for the specified property
            /// </summary>
            /// <param name="propertyName">Name of property that has changed</param>
            private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// A class containing a list of resolutions to be tested for HDMI cables
        /// </summary>
        public class HDMIResList : ICableResList
        {
            /// <summary>
            /// Backing field for Resolutions property
            /// </summary>
            private List<ResolutionItem> resolutions = new List<ResolutionItem>()
            {
                new ResolutionItem(VideoResolution.Mode480), 
                new ResolutionItem(VideoResolution.Mode480Wide),
                new ResolutionItem(VideoResolution.Mode720p), 
                new ResolutionItem(VideoResolution.Mode1080i),
                new ResolutionItem(VideoResolution.Mode1080p)
            };

            /// <summary>
            /// Initializes a new instance of the <see cref="HDMIResList" /> class.
            /// </summary>
            /// <param name="region">The region to use for this set of resolutions</param>
            public HDMIResList(Region region)
            {
                // Video format standard should not matter for HDMI or VGA, but use something appropriate just in case
                if (region == Region.Japan)
                {
                    this.Standard = VideoStandard.NTSCJ;
                }
                else if (region == Region.Europe)
                {
                    this.Standard = VideoStandard.PAL60;
                }
                else
                {
                    this.Standard = VideoStandard.NTSCM;
                }

                this.Region = region;
            }

            /// <summary>
            /// Gets a list of resolutions to test for this cable type
            /// </summary>
            public List<ResolutionItem> Resolutions
            {
                get { return this.resolutions; }
            }

            /// <summary>
            /// Gets or sets the video format standard used
            /// </summary>
            public VideoStandard Standard { get; set; }

            /// <summary>
            /// Gets a string representing the video format standard used
            /// </summary>
            public string StandardString
            {
                get { return VID117CTC1.RegionToString(this.Region); }
            }

            /// <summary>
            /// Gets or sets the Xbox console region to use.
            /// </summary>
            public Region Region { get; set; }
        }

        /// <summary>
        /// A class containing a list of resolutions to be tested for VGA cables
        /// </summary>
        public class VGAResList : ICableResList
        {
            /// <summary>
            /// Backing field for Resolutions property
            /// </summary>
            private List<ResolutionItem> resolutions = new List<ResolutionItem>()
            {
                new ResolutionItem(VideoResolution.Mode640x480), 
                new ResolutionItem(VideoResolution.Mode640x480Wide),
                new ResolutionItem(VideoResolution.Mode848x480), 
                new ResolutionItem(VideoResolution.Mode1280x720),
                new ResolutionItem(VideoResolution.Mode1024x768),
                new ResolutionItem(VideoResolution.Mode1024x768Wide),
                new ResolutionItem(VideoResolution.Mode1280x768),
                new ResolutionItem(VideoResolution.Mode1360x768),
                new ResolutionItem(VideoResolution.Mode1440x900),
                new ResolutionItem(VideoResolution.Mode1280x1024),
                new ResolutionItem(VideoResolution.Mode1280x1024Wide),
                new ResolutionItem(VideoResolution.Mode1680x1050),
                new ResolutionItem(VideoResolution.Mode1920x1080)
            };

            /// <summary>
            /// Initializes a new instance of the <see cref="VGAResList" /> class.
            /// </summary>
            /// <param name="region">Region to use for this set of resolutions</param>
            public VGAResList(Region region)
            {
                // Video format standard should not matter for HDMI or VGA, but use something appropriate just in case
                if (region == Region.Japan)
                {
                    this.Standard = VideoStandard.NTSCJ;
                }
                else if (region == Region.Europe)
                {
                    this.Standard = VideoStandard.PAL60;
                }
                else
                {
                    this.Standard = VideoStandard.NTSCM;
                }

                this.Region = region;
            }

            /// <summary>
            /// Gets a list of resolutions to test for this cable type
            /// </summary>
            public List<ResolutionItem> Resolutions
            {
                get { return this.resolutions; }
            }
            
            /// <summary>
            /// Gets or sets the video format standard used
            /// </summary>
            public VideoStandard Standard { get; set; }

            /// <summary>
            /// Gets a string representing the video format standard used
            /// </summary>
            public string StandardString
            {
                get { return VID117CTC1.RegionToString(this.Region); }
            }

            /// <summary>
            /// Gets or sets the Xbox console region to use.
            /// </summary>
            public Region Region { get; set; }
        }

        /// <summary>
        /// A class containing a list of resolutions to be tested for S-Video cables
        /// </summary>
        public class SVideoResList : ICableResList
        {
            /// <summary>
            /// Backing field for Resolutions property
            /// </summary>
            private List<ResolutionItem> resolutions = new List<ResolutionItem>()
            {
                new ResolutionItem(VideoResolution.Mode480),
                new ResolutionItem(VideoResolution.Mode480Wide)
            };

            /// <summary>
            /// Initializes a new instance of the <see cref="SVideoResList" /> class.
            /// </summary>
            /// <param name="region">The region to use for this set of resolutions</param>
            /// <param name="std">Video format standard to use for this set of resolutions</param>
            public SVideoResList(Region region, VideoStandard std)
            {
                this.Region = region;
                this.Standard = std;
            }

            /// <summary>
            /// Gets a list of resolutions to test for this cable type
            /// </summary>
            public List<ResolutionItem> Resolutions
            {
                get { return this.resolutions; }
            }

            /// <summary>
            /// Gets or sets the video format standard used
            /// </summary>
            public VideoStandard Standard { get; set; }

            /// <summary>
            /// Gets a string representing the video format standard used
            /// </summary>
            public string StandardString
            {
                get { return VID117CTC1.StandardToString(this.Standard) + " - " + VID117CTC1.RegionToString(this.Region); }
            }

            /// <summary>
            /// Gets or sets the Xbox console region to use.
            /// </summary>
            public Region Region { get; set; }
        }

        /// <summary>
        /// A class containing a list of resolutions to be tested for SCART cables
        /// </summary>
        public class SCARTResList : ICableResList
        {
            /// <summary>
            /// Backing field for Resolutions property
            /// </summary>
            private List<ResolutionItem> resolutions = new List<ResolutionItem>()
            {
                new ResolutionItem(VideoResolution.Mode480),
                new ResolutionItem(VideoResolution.Mode480Wide)
            };

            /// <summary>
            /// Initializes a new instance of the <see cref="SCARTResList" /> class.
            /// </summary>
            /// <param name="region">The region to use for this set of resolutions</param>
            /// <param name="std">Video format standard to use for this set of resolutions</param>
            public SCARTResList(Region region, VideoStandard std)
            {
                this.Region = region;
                this.Standard = std;
            }

            /// <summary>
            /// Gets a list of resolutions to test for this cable type
            /// </summary>
            public List<ResolutionItem> Resolutions
            {
                get { return this.resolutions; }
            }

            /// <summary>
            /// Gets or sets the video format standard used
            /// </summary>
            public VideoStandard Standard { get; set; }

            /// <summary>
            /// Gets a string representing the video format standard used
            /// </summary>
            public string StandardString
            {
                get { return VID117CTC1.StandardToString(this.Standard) + " - " + VID117CTC1.RegionToString(this.Region); }
            }

            /// <summary>
            /// Gets or sets the Xbox console region to use.
            /// </summary>
            public Region Region { get; set; }
        }

        /// <summary>
        /// A class containing a list of resolutions to be tested for Composite cables
        /// </summary>
        public class CompositeResList : ICableResList
        {
            /// <summary>
            /// Backing field for Resolutions property
            /// </summary>
            private List<ResolutionItem> resolutions = new List<ResolutionItem>()
            {
                new ResolutionItem(VideoResolution.Mode480),
                new ResolutionItem(VideoResolution.Mode480Wide)
            };

            /// <summary>
            /// Initializes a new instance of the <see cref="CompositeResList" /> class.
            /// </summary>
            /// <param name="region">The region to use for this set of resolutions</param>
            /// <param name="std">Video format standard to use for this set of resolutions</param>
            public CompositeResList(Region region, VideoStandard std)
            {
                this.Region = region;
                this.Standard = std;
            }

            /// <summary>
            /// Gets a list of resolutions to test for this cable type
            /// </summary>
            public List<ResolutionItem> Resolutions
            {
                get { return this.resolutions; }
            }

            /// <summary>
            /// Gets or sets the video format standard used
            /// </summary>
            public VideoStandard Standard { get; set; }

            /// <summary>
            /// Gets a string representing the video format standard used
            /// </summary>
            public string StandardString
            {
                get { return VID117CTC1.StandardToString(this.Standard) + " - " + VID117CTC1.RegionToString(this.Region); }
            }

            /// <summary>
            /// Gets or sets the Xbox console region to use.
            /// </summary>
            public Region Region { get; set; }
        }

        /// <summary>
        /// A class containing a list of resolutions to be tested for Component cables
        /// </summary>
        public class ComponentResList : ICableResList
        {
            /// <summary>
            /// Backing field for Resolutions property
            /// </summary>
            private List<ResolutionItem> resolutions = new List<ResolutionItem>()
            {
                new ResolutionItem(VideoResolution.Mode480), 
                new ResolutionItem(VideoResolution.Mode480Wide),
                new ResolutionItem(VideoResolution.Mode720p), 
                new ResolutionItem(VideoResolution.Mode1080i),
                new ResolutionItem(VideoResolution.Mode1080p)
            };

            /// <summary>
            /// Initializes a new instance of the <see cref="ComponentResList" /> class.
            /// </summary>
            /// <param name="region">The region to use for this set of resolutions</param>
            /// <param name="std">Video format standard to use for this set of resolutions</param>
            public ComponentResList(Region region, VideoStandard std)
            {
                this.Region = region;
                this.Standard = std;
            }

            /// <summary>
            /// Gets a list of resolutions to test for this cable type
            /// </summary>
            public List<ResolutionItem> Resolutions
            {
                get { return this.resolutions; }
            }

            /// <summary>
            /// Gets or sets the video format standard used
            /// </summary>
            public VideoStandard Standard { get; set; }

            /// <summary>
            /// Gets a string representing the video format standard used
            /// </summary>
            public string StandardString
            {
                get { return VID117CTC1.StandardToString(this.Standard) + " - " + VID117CTC1.RegionToString(this.Region); }
            }

            /// <summary>
            /// Gets or sets the Xbox console region to use.
            /// </summary>
            public Region Region { get; set; }
        }

        /// <summary>
        /// A class containing a list of resolutions to be tested for D-Terminal cables
        /// </summary>
        public class DTerminalResList : ICableResList
        {
            /// <summary>
            /// Backing field for Resolutions property
            /// </summary>
            private List<ResolutionItem> resolutions = new List<ResolutionItem>()
            {
                new ResolutionItem(VideoResolution.Mode480), 
                new ResolutionItem(VideoResolution.Mode480Wide),
                new ResolutionItem(VideoResolution.Mode720p), 
                new ResolutionItem(VideoResolution.Mode1080i),
                new ResolutionItem(VideoResolution.Mode1080p)
            };

            /// <summary>
            /// Initializes a new instance of the <see cref="DTerminalResList" /> class.
            /// </summary>
            /// <param name="region">The region to use for this set of resolutions</param>
            /// <param name="std">Video format standard to use for this set of resolutions</param>
            public DTerminalResList(Region region, VideoStandard std)
            {
                this.Region = region;
                this.Standard = std;
            }

            /// <summary>
            /// Gets a list of resolutions to test for this cable type
            /// </summary>
            public List<ResolutionItem> Resolutions
            {
                get { return this.resolutions; }
            }

            /// <summary>
            /// Gets or sets the video format standard used
            /// </summary>
            public VideoStandard Standard { get; set; }

            /// <summary>
            /// Gets a string representing the video format standard used
            /// </summary>
            public string StandardString
            {
                get { return VID117CTC1.StandardToString(this.Standard) + " - " + VID117CTC1.RegionToString(this.Region); }
            }

            /// <summary>
            /// Gets or sets the Xbox console region to use.
            /// </summary>
            public Region Region { get; set; }
        }

        /// <summary>
        /// A class representing a type of video cable
        /// </summary>
        public class VideoCable : INotifyPropertyChanged
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="VideoCable" /> class.
            /// </summary>
            /// <param name="name">The name of this cable type</param>
            /// <param name="type">The type of cable</param>
            /// <param name="resList">List of all resolutions to associate with this cable</param>
            public VideoCable(string name, CableType type, ObservableCollection<ICableResList> resList)
            {
                this.Name = name;
                this.Type = type;
                this.TestingLastResolution = false;
                this.CurrentlyTesting = false;
                this.StartedTesting = false;
                this.ResolutionLists = resList;
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets the name of the video cable
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the resolutions for this cable
            /// </summary>
            public ObservableCollection<ICableResList> ResolutionLists { get; set; }

            /// <summary>
            /// Gets or sets the type of the cable
            /// </summary>
            public CableType Type { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not testing is done for this cable
            /// </summary>
            public bool DoneTesting { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not testing has progressed to the last resolution
            /// </summary>
            public bool TestingLastResolution { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not testing is in progress for this cable
            /// </summary>
            public bool CurrentlyTesting { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not testing has started for this cable
            /// </summary>
            public bool StartedTesting { get; set; }

            /// <summary>
            /// NotifyPropertyChanged triggers the PropertyChanged event for the specified property
            /// </summary>
            /// <param name="propertyName">Name of property that has changed</param>
            private void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}