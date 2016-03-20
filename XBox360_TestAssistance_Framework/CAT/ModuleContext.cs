// -----------------------------------------------------------------------
// <copyright file="ModuleContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;

    /// <summary>
    /// Module Context
    /// An object of this interface is passed to the module, and provides the Module with an API.
    /// </summary>
    public class ModuleContext : IModuleContext, INotifyPropertyChanged
    {
        /// <summary>
        /// Backing field for DefaultLogFilename property
        /// </summary>
        private const string DefaultLogFileName = "results.log";

        /// <summary>
        /// A reference to the MainViewModel
        /// </summary>
        private readonly MainViewModel mainViewModel;

        /// <summary>
        /// Backing field for TestCase property
        /// </summary>
        private readonly ITCRTestCase testCase;

        /// <summary>
        /// Backing field for IsModal property
        /// </summary>
        private bool isModal;

        /// <summary>
        /// Backing field for AllDevices property
        /// </summary>
        private List<IDevice> allDevices;

        /// <summary>
        /// Backing for LogDirectory property
        /// </summary>
        private string logDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleContext" /> class.
        /// </summary>
        /// <param name="tcrTestCase">TestCase to associate with this ModuleContext</param>
        /// <param name="viewModel">A reference to the MainViewModel</param>
        public ModuleContext(ITCRTestCase tcrTestCase, MainViewModel viewModel)
        {
            this.mainViewModel = viewModel;
            this.testCase = tcrTestCase;
        }

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a reference to the MainViewModel
        /// </summary>
        public MainViewModel MainViewModel
        {
            get { return this.mainViewModel; }
        }

        /// <summary>
        /// Gets the test case associated with this module
        /// Implements TestCase from IModuleContext
        /// </summary>
        public ITCRTestCase TestCase
        {
            get { return this.testCase; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the module should be considered Modal.
        /// If modal, the user will be prompted to confirm exit of the module.
        /// Implements IsModal from IModuleContext
        /// </summary>
        public bool IsModal 
        { 
            get
            {
                return this.isModal;
            }

            set
            {
                if (this.isModal != value)
                {
                    this.isModal = value;
                    if (value == false)
                    {
                        MainViewModel.NavigationPanelVisibility = Visibility.Visible;
                        MainViewModel.DevicePoolVisibility = Visibility.Visible;

                        foreach (XboxViewItem xboxViewItem in MainViewModel.XboxList)
                        {
                            if (xboxViewItem.ProfileManagerViewModel != null)
                            {
                                xboxViewItem.ProfileManagerViewModel.Show();
                            }
                        }
                    }
                    else
                    {
                        MainViewModel.NavigationPanelVisibility = Visibility.Collapsed;
                        MainViewModel.DevicePoolVisibility = Visibility.Collapsed;

                        foreach (XboxViewItem xboxViewItem in MainViewModel.XboxList)
                        {
                            if (xboxViewItem.ProfileManagerViewModel != null)
                            {
                                xboxViewItem.ProfileManagerViewModel.Hide();
                            }
                        }
                    }

                    this.NotifyPropertyChanged();
                }
            } 
        }

        /// <summary>
        /// Gets or sets all devices.
        /// Note: The module should only utilize devices that are selected (use SelectedDevices).
        /// Implements AllDevices from IModuleContext
        /// </summary>
        public List<IDevice> AllDevices
        {
            get { return this.allDevices; }
            set { this.allDevices = value; }
        }

        /// <summary>
        /// Gets the currently selected set of devices this module may use.
        /// Implements SelectedDevices from IModuleContext
        /// </summary>
        public List<IDevice> SelectedDevices
        {
            get
            {
                List<IDevice> list = new List<IDevice>();
                foreach (IDevice d in this.allDevices)
                {
                    if (d.IsSelected)
                    {
                        list.Add(d);
                    }
                }

                return list;
            }
        }

        /// <summary>
        /// Gets the directory this module instance may log to.
        /// Implements LogDirectory in IModuleContext
        /// </summary>
        public string LogDirectory
        {
            get
            {
                if (this.logDirectory == null)
                {
                    this.StartLog();
                }

                return this.logDirectory;
            }
        }

        /// <summary>
        /// Gets the directory where platform and version specific CAT data files are found
        /// </summary>
        public string PlatformDataPath
        {
            get { return Path.Combine(@"Platforms", this.mainViewModel.CurrentPlatform.Name, this.mainViewModel.CurrentTCRVersion.Name); } 
        }

        /// <summary>
        /// Starts log output for the module.
        /// Implements StringLog in IModuleContext
        /// </summary>
        public void StartLog()
        {
            if (this.logDirectory == null)
            {
                // Compose a unique name. TODO: verify valid file and path name
                string name = '_' + this.testCase.Name;
                name = name.Replace(',', '_');
                name = name.Replace(':', '_');
                name = name.Replace('-', '_');
                name = name.Replace(' ', '_');
                string folderName = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + name;

                this.logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CAT", folderName);
                Directory.CreateDirectory(this.logDirectory);

                this.Log("Starting Module for test case: " + this.TestCase.Name);
                List<IDevice> devices = this.SelectedDevices;
                if (devices.Count == 0)
                {
                    this.Log("No devices selected");
                }
                else if (devices.Count == 1)
                {
                    this.Log("On device: " + devices[0].Name);
                }
                else
                {
                    this.Log("On devices:");
                    foreach (IDevice d in devices)
                    {
                        this.Log(d.Name);
                    }
                }
            }
        }

        /// <summary>
        /// Appends to the module's log file
        /// Implements Log in IModuleContext
        /// </summary>
        /// <param name="text">Text to append to the log</param>
        public void Log(string text)
        {
            if (this.logDirectory == null)
            {
                this.StartLog();
            }

            string timeStamp = DateTime.Now.ToString();
            File.AppendAllText(Path.Combine(this.logDirectory, DefaultLogFileName), timeStamp + ": " + text + "\r\n");
        }

        /// <summary>
        /// Outputs a summary and closes the log file.
        /// Implements EndLog from IModuleContext
        /// </summary>
        public void EndLog()
        {
            if (this.logDirectory != null)
            {
                string summary = "Terminating test module for test case: " + this.TestCase.Name;
                this.Log(summary);
                this.logDirectory = null;
            }
        }

        /// <summary>
        /// Creates a progress bar window with the specified title.
        /// </summary>
        /// <param name="title">Title for the progress bar window</param>
        /// <returns>An instance of a class derived from IProgressBar</returns>
        public IProgressBar OpenProgressBarWindow(string title)
        {
            return new ProgressBarViewModel(title, MainViewModel.MainWindow);
        }

        /// <summary>
        /// NotifyPropertyChanged triggers the PropertyChanged event for the specified property
        /// </summary>
        /// <param name="propertyName">Name of property that has changed</param>
        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
