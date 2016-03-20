// -----------------------------------------------------------------------
// <copyright file="DebugOutputViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Media;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;
    using System.Xml;
    using XDevkit;

    /// <summary>
    /// View Model for Debug Output Window
    /// </summary>
    public class DebugOutputViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// File to log to.
        /// </summary>
        private const string LogFileName = "DebugOutput.log";

        /// <summary>
        /// Backing field for MainViewModel property
        /// </summary>
        private MainViewModel mainViewModel;

        /// <summary>
        /// A count of all intercepted API calls
        /// </summary>
        private uint apiCallCount;

        /// <summary>
        /// A count of all acceptable output lines
        /// </summary>
        private uint goodLineCount;

        /// <summary>
        /// A count of all unacceptable output lines
        /// </summary>
        private uint badLineCount;

        /// <summary>
        /// Directory to log output to.
        /// </summary>
        private string logDirectory;

        /// <summary>
        /// Backing property for TestPassing property
        /// </summary>
        private bool testPassing = true;

        /// <summary>
        /// A count of dumps, used to name dump files uniquely.
        /// </summary>
        private uint dumpIndex = 1;

        /// <summary>
        /// Indicates the current behavior when a dump is detected.
        /// </summary>
        private DumpModeEnum dumpMode = DumpModeEnum.Prompt;

        /// <summary>
        /// A boolean value indicating whether or not to do a full dump, or a partial dump
        /// </summary>
        private bool dumpFullHeap = true;

        /// <summary>
        /// A list of all acceptable debug string prefixes
        /// </summary>
        private List<string> safeDebugOutputPrefixText = new List<string>();

        /// <summary>
        /// Backing property for MonitorAPISymbols
        /// </summary>
        private List<MonitorAPISymbol> monitorAPISymbols = new List<MonitorAPISymbol>();

        /// <summary>
        /// Backing field for MonitorAPISessions property
        /// </summary>
        private ObservableCollection<DebugOutputMonitorAPISession> monitorAPISessions = new ObservableCollection<DebugOutputMonitorAPISession>();

        /// <summary>
        /// Debug output window UI class
        /// </summary>
        private DebugOutputWindow debugOutputWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugOutputViewModel" /> class.
        /// </summary>
        /// <param name="xbvi">The XboxViewItem for the xbox associated with this debug output window</param>
        /// <param name="mainViewModel">A reference to the MainViewModel</param>
        public DebugOutputViewModel(XboxViewItem xbvi, MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.XboxViewItem = xbvi;

            this.OpenLogDirectoryCommand = new Command((o) => Process.Start(this.logDirectory));
            this.ResetTestCommand = new Command((o) => this.ResetTestFailingVisibility());
            this.RemoveSymbolCommand = new Command((o) => this.RemoveSymbol(o as DebugOutputMonitorAPISession));
            this.ResetCalledCommand = new Command((o) => this.ResetCalled(o as DebugOutputMonitorAPISession));
            this.AddSymbolCommand = new Command((o) => this.AddSymbol());
            this.SelectAllCommand = new Command((o) => this.SelectAll());
        }

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Indicates the behavior when a dump is detected
        /// </summary>
        public enum DumpModeEnum
        {
            /// <summary>
            /// Ignores dumps
            /// </summary>
            Ignore,

            /// <summary>
            /// Prompts the user to save a dump
            /// </summary>
            Prompt,

            /// <summary>
            /// Automatically saves the dump
            /// </summary>
            Auto
        }

        /// <summary>
        /// Gets or sets a reference to the associated Xbox's XboxViewItem
        /// </summary>
        public XboxViewItem XboxViewItem { get; set; }

        /// <summary>
        /// Gets the name of the associated Xbox
        /// </summary>
        public string XboxName
        {
            get { return this.XboxViewItem.XboxDevice.Name; }
        }

        /// <summary>
        /// Gets the debug output window title
        /// </summary>
        public string Title
        {
            get { return "Debug/Crash Monitor - " + this.XboxName; }
        }

        /// <summary>
        /// Gets or sets a count of all intercepted API calls
        /// </summary>
        public uint ApiCallCount
        {
            get
            {
                return this.apiCallCount;
            }

            set
            {
                this.apiCallCount = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a count of all acceptable output lines
        /// </summary>
        public uint GoodLineCount
        {
            get
            {
                return this.goodLineCount;
            }

            set
            {
                this.goodLineCount = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a count of all unacceptable output lines
        /// </summary>
        public uint BadLineCount
        {
            get
            {
                return this.badLineCount;
            }

            set
            {
                this.badLineCount = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the passing notification should be visible
        /// </summary>
        public bool TestPassing
        {
            get
            {
                return this.testPassing;
            }

            set
            {
                this.testPassing = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a list of symbols that can be monitored
        /// </summary>
        public List<MonitorAPISymbol> MonitorAPISymbols
        {
            get
            {
                return this.monitorAPISymbols;
            }

            set
            {
                this.monitorAPISymbols = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets command to open the log directory.
        /// </summary>
        public Command OpenLogDirectoryCommand { get; set; }

        /// <summary>
        /// Gets or sets command to reset the test status
        /// </summary>
        public Command ResetTestCommand { get; set; }

        /// <summary>
        /// Gets or sets command to remove a symbol
        /// </summary>
        public Command RemoveSymbolCommand { get; set; }

        /// <summary>
        /// Gets or sets comment to reset the called status of a symbol
        /// </summary>
        public Command ResetCalledCommand { get; set; }

        /// <summary>
        /// Gets or sets a command to add a symbol
        /// </summary>
        public Command AddSymbolCommand { get; set; }

        /// <summary>
        /// Gets or sets a command to select all available functions
        /// </summary>
        public Command SelectAllCommand { get; set; }

        /// <summary>
        /// Gets or sets a value indicates the current behavior when a dump is detected.
        /// </summary>
        public DumpModeEnum DumpMode
        {
            get
            {
                return this.dumpMode;
            }

            set
            {
                this.dumpMode = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to do a full dump, or a partial dump
        /// </summary>
        public bool DumpFullHeap
        {
            get
            {
                return this.dumpFullHeap;
            }

            set
            {
                this.dumpFullHeap = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a reference to the MainViewModel
        /// </summary>
        public MainViewModel MainViewModel
        {
            get { return this.mainViewModel; }
        }

        /// <summary>
        /// Gets a reference to the current Theme
        /// </summary>
        public Theme CurrentTheme
        {
            get { return MainViewModel.CurrentTheme; }
        }

        /// <summary>
        /// Gets a value indicating all symbol monitoring sessions
        /// </summary>
        public ObservableCollection<DebugOutputMonitorAPISession> MonitorAPISessions
        {
            get { return this.monitorAPISessions; }
        }

        /// <summary>
        /// Gets or sets a the session for OutputDebugStringA
        /// </summary>
        public DebugOutputMonitorAPISession OutputDebugStringASymbolSession { get; set; }

        /// <summary>
        /// Gets or sets a the session for OutputDebugStringW
        /// </summary>
        public DebugOutputMonitorAPISession OutputDebugStringWSymbolSession { get; set; }

        /// <summary>
        /// An event handler called when a condition that might require a dump is detected.
        /// This is invoked in the UI thread.
        /// </summary>
        /// <param name="description">String description of the origin of the failure (RIP, Assert, or Exception)</param>
        /// <param name="eventInfo">EventInfo associated with this failure</param>
        public void OnTitleFailureInUIThread(string description, IXboxEventInfo eventInfo)
        {
            bool createDump = this.DumpMode == DumpModeEnum.Auto;
            if (this.DumpMode == DumpModeEnum.Prompt)
            {
                MessageBoxResult result = MessageBox.Show("Detected " + description + " on " + this.XboxViewItem.XboxDevice.Name + ".  Generate dump file?", "Detected: " + description, MessageBoxButton.YesNo, MessageBoxImage.Question);
                createDump = result == MessageBoxResult.Yes;
            }

            if (createDump)
            {
                string fileName = "Dump" + this.dumpIndex + ".dmp";
                this.dumpIndex++;
                Mouse.OverrideCursor = Cursors.Wait;
                this.XboxViewItem.XboxDevice.SaveDump(Path.Combine(this.logDirectory, fileName), this.DumpFullHeap);
                Mouse.OverrideCursor = null;
                if (this.DumpMode == DumpModeEnum.Prompt)
                {
                    Process.Start(this.logDirectory);
                }
            }

            this.XboxViewItem.XboxDevice.ContinueExecution(eventInfo);
        }

        /// <summary>
        /// An event handler called when a condition that might require a dump is detected.
        /// This is NOT invoked in the UI thread.
        /// </summary>
        /// <param name="description">String description of the origin of the failure (RIP, Assert, or Exception)</param>
        /// <param name="eventInfo">EventInfo associated with this failure</param>
        public void OnTitleFailure(string description, IXboxEventInfo eventInfo)
        {
            try
            {
                Dispatcher dispatcher = this.debugOutputWindow.Dispatcher;
                if (dispatcher != null)
                {
                    dispatcher.BeginInvoke(new Action(() => { this.OnTitleFailureInUIThread(description, eventInfo); }));
                }
            }
            catch (TaskCanceledException)
            {
                // Don't complain about late arrival if UI has already gone away
            }
        }

        /// <summary>
        /// Starts API monitoring and intercepting of debug output
        /// </summary>
        public void Start()
        {
            bool canConnectDebugger = this.XboxViewItem.XboxDevice.ConnectDebugger(false);
            if (!canConnectDebugger)
            {
                MessageBoxResult result = MessageBox.Show("Another debugging session with this Xbox is already present.  Do you want to connect anyway?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    canConnectDebugger = this.XboxViewItem.XboxDevice.ConnectDebugger(true);
                }
            }

            if (!canConnectDebugger)
            {
                this.XboxViewItem.DebugOutputViewModel = null;
            }
            else
            {
                this.debugOutputWindow = new DebugOutputWindow(this);
                this.debugOutputWindow.Closing += this.OnWindowClosing;

                this.OutputDebugStringASymbolSession = new DebugOutputMonitorAPISession(this, this.XboxViewItem.XboxDevice.MonitorAPI("OutputDebugStringA", this.InterceptAPICall), false);
                this.OutputDebugStringWSymbolSession = new DebugOutputMonitorAPISession(this, this.XboxViewItem.XboxDevice.MonitorAPI("OutputDebugStringW", this.InterceptAPICall), false);

                this.monitorAPISessions.Add(this.OutputDebugStringASymbolSession);
                this.monitorAPISessions.Add(this.OutputDebugStringWSymbolSession);

                this.XboxViewItem.XboxDevice.StartMonitoringDebugOutput(this.ReceiveDebugOutput);
                this.XboxViewItem.XboxDevice.StartMonitoringTitleFailures(this.OnTitleFailure);

                // Read XML config file for debug string
                XmlDocument configFile = new XmlDocument();
                configFile.Load(@"DebugStrings.cfg");
                XmlNodeList nodes = configFile.DocumentElement.SelectNodes("/DebugStringsConfig/GoodPrefixes/GoodPrefix");
                foreach (XmlNode n in nodes)
                {
                    this.safeDebugOutputPrefixText.Add(n.InnerText);
                }

                // Read XML config file for API monitor symbols
                configFile = new XmlDocument();
                configFile.Load(@"MonitorAPISymbols.cfg");
                nodes = configFile.DocumentElement.SelectNodes("/MonitorAPISymbols/MonitorAPISymbol");
                foreach (XmlNode n in nodes)
                {
                    this.MonitorAPISymbols.Add(new MonitorAPISymbol(n.InnerText));
                }

                // Compose a unique name. TODO: verify valid file and path name
                string folderName = DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss") + "_DebugOutput";

                this.logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CAT", folderName);
                Directory.CreateDirectory(this.logDirectory);

                this.Log("Debug Output For: " + this.XboxViewItem.XboxDevice.Name);

                this.debugOutputWindow.Show();
            }
        }

        /// <summary>
        /// An event handler called when the window closes.
        /// </summary>
        /// <param name="sender">Originator of the event</param>
        /// <param name="e">An instance of CancelEventArgs</param>
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            this.Log("*************************************************************");
            this.Log("Good debug output lines: " + this.GoodLineCount);
            this.Log("Bad  debug output lines: " + this.BadLineCount);
            this.Log("API calls intercepted: " + this.ApiCallCount);
            this.Log("*************************************************************");

            this.XboxViewItem.DebugOutputViewModel = null;

            foreach (DebugOutputMonitorAPISession session in this.monitorAPISessions)
            {
                session.MonitorAPISession.Dispose();
            }

            this.XboxViewItem.XboxDevice.StopMonitoringDebugOutput();
            this.XboxViewItem.XboxDevice.StopMonitoringTitleFailures();
            this.XboxViewItem.XboxDevice.DisconnectDebugger();
        }

        /// <summary>
        /// Reset the test status
        /// </summary>
        public void ResetTestFailingVisibility()
        {
            this.ApiCallCount = 0;
            this.TestPassing = true;
        }

        /// <summary>
        /// Close the debug output window
        /// </summary>
        public void Close()
        {
            this.debugOutputWindow.Close();
        }

        /// <summary>
        /// Show the debug output window, if hidden
        /// </summary>
        public void Show()
        {
            this.debugOutputWindow.Show();
        }

        /// <summary>
        /// Hide the debug output window
        /// </summary>
        public void Hide()
        {
            this.debugOutputWindow.Hide();
        }

        /// <summary>
        /// Activate the debug output window
        /// </summary>
        public void Activate()
        {
            this.debugOutputWindow.Activate();
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

        /// <summary>
        /// A notification that an API call has been intercepted.
        /// This is called in the UI thread.
        /// </summary>
        /// <param name="symbol">The symbol of the intercepted API call</param>
        private void InterceptAPICallInUIThread(IMonitorAPISession symbol)
        {
            this.ApiCallCount++;
            if (this.TestPassing)
            {
                this.TestPassing = false;
                SystemSounds.Beep.Play(); // Play system beep sound on bad output encountered.
            }
        }

        /// <summary>
        /// A notification that an API call has been intercepted.
        /// This is NOT called in the UI thread.
        /// </summary>
        /// <param name="symbol">The symbol of the intercepted API call</param>
        private void InterceptAPICall(IMonitorAPISession symbol)
        {
            try
            {
                Dispatcher dispatcher = this.debugOutputWindow.Dispatcher;
                if (dispatcher != null)
                {
                    dispatcher.BeginInvoke(new Action(() => { this.InterceptAPICallInUIThread(symbol); }), DispatcherPriority.ApplicationIdle);
                }
            }
            catch (TaskCanceledException)
            {
                // Don't complain about late arrival if UI has already gone away
            }
        }

        /// <summary>
        /// Logs debug output to a log file.
        /// </summary>
        /// <param name="incomingLogText">Text to log</param>
        private void LogDebugOutput(string incomingLogText)
        {
            string logText = Regex.Replace(incomingLogText, @"([\r]^[\n])|([^\r][\n])", "\r\n");
            Run r = new Run(logText);
            bool failed = true;
            foreach (string s in this.safeDebugOutputPrefixText)
            {
                if (logText.TrimStart().StartsWith(s))
                {
                    failed = false;
                }
            }

            r.Foreground = Brushes.Green;
            if (failed)
            {
                r.Foreground = Brushes.Red;
                this.BadLineCount++;
                this.Log("BAD : " + logText);

                File.AppendAllText(Path.Combine(this.logDirectory, "bad_output.txt"), logText + "\r\n");
            }
            else
            {
                this.Log("GOOD: " + logText);
                this.GoodLineCount++;
            }

            Paragraph p = new Paragraph(r);
            p.Margin = new Thickness(0);
            this.debugOutputWindow.DebugOutputBox.Document.Blocks.Add(p);

            // If scrolled to bottom, snap to bottom
            if (Math.Ceiling(this.debugOutputWindow.DebugOutputBox.VerticalOffset + this.debugOutputWindow.DebugOutputBox.ViewportHeight) == Math.Ceiling(this.debugOutputWindow.DebugOutputBox.ExtentHeight)
                || Math.Ceiling(this.debugOutputWindow.DebugOutputBox.ExtentHeight) < Math.Ceiling(this.debugOutputWindow.DebugOutputBox.ViewportHeight))
            {
                this.debugOutputWindow.DebugOutputBox.ScrollToEnd();
            }
        }

        /// <summary>
        /// Received debug output.
        /// This is NOT called in the UI thread.
        /// </summary>
        /// <param name="logText">Text to log</param>
        private void ReceiveDebugOutput(string logText)
        {
            try
            {
                Dispatcher dispatcher = this.debugOutputWindow.Dispatcher;
                if (dispatcher != null)
                {
                    dispatcher.BeginInvoke(new Action(() => { LogDebugOutput(logText); }), DispatcherPriority.ApplicationIdle);
                }
            }
            catch (TaskCanceledException)
            {
                // Don't complain about late arrival if UI has already gone away
            }
        }

        /// <summary>
        /// Logs text to the log file.  Prefixes the text with a timestamp.
        /// </summary>
        /// <param name="logText">Text to log</param>
        private void Log(string logText)
        {
            string timeStamp = DateTime.Now.ToString();
            Directory.CreateDirectory(this.logDirectory);
            File.AppendAllText(Path.Combine(this.logDirectory, LogFileName), timeStamp + ": " + logText + "\r\n");
        }

        /// <summary>
        /// Removes a symbol from API monitoring
        /// </summary>
        /// <param name="session">Session associated with symbol to remove</param>
        private void RemoveSymbol(DebugOutputMonitorAPISession session)
        {
            string symbolName = session.MonitorAPISession.SymbolName;
            this.MonitorAPISessions.Remove(session);
            session.MonitorAPISession.Dispose();

            foreach (MonitorAPISymbol symbol in this.MonitorAPISymbols)
            {
                if (symbol.SymbolName == symbolName)
                {
                    symbol.IsAvailable = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Resets the called status of a symbol
        /// </summary>
        /// <param name="session">Session associated with the symbol to reset the called status of</param>
        private void ResetCalled(DebugOutputMonitorAPISession session)
        {
            session.MonitorAPISession.WasCalled = false;
        }

        /// <summary>
        /// Adds a symbol to be monitored
        /// </summary>
        private void AddSymbol()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            List<DebugOutputMonitorAPISession> sessionsToAdd = new List<DebugOutputMonitorAPISession>();
            foreach (MonitorAPISymbol symbol in this.MonitorAPISymbols)
            {
                if (symbol.IsSelected)
                {
                    bool found = false;
                    foreach (DebugOutputMonitorAPISession session in this.monitorAPISessions)
                    {
                        if (session.MonitorAPISession.SymbolName == symbol.SymbolName)
                        {
                            // Already have this one
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        symbol.IsAvailable = false;
                        sessionsToAdd.Add(new DebugOutputMonitorAPISession(this, this.XboxViewItem.XboxDevice.MonitorAPI(symbol.SymbolName, this.InterceptAPICall), true));
                    }

                    symbol.IsSelected = false;
                }
            }

            foreach (DebugOutputMonitorAPISession session in sessionsToAdd)
            {
                this.monitorAPISessions.Add(session);
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Select all available functions in available function list
        /// </summary>
        private void SelectAll()
        {
            foreach (MonitorAPISymbol symbol in this.MonitorAPISymbols)
            {
                symbol.IsSelected = true;
            }
        }

        /// <summary>
        /// Class representing a symbol to monitor
        /// </summary>
        public class MonitorAPISymbol : INotifyPropertyChanged
        {
            /// <summary>
            /// Backing field for IsSelected property
            /// </summary>
            private bool isSelected;

            /// <summary>
            /// Backing field for IsAvailable property
            /// </summary>
            private bool isAvailable;

            /// <summary>
            /// Initializes a new instance of the <see cref="MonitorAPISymbol" /> class.
            /// </summary>
            /// <param name="symbolName">The symbol name</param>
            public MonitorAPISymbol(string symbolName)
            {
                this.SymbolName = symbolName;
                this.isAvailable = true;
            }

            /// <summary>
            /// PropertyChanged event used by NotifyPropertyChanged
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets the symbol name
            /// </summary>
            public string SymbolName { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this symbol is selected
            /// </summary>
            public bool IsSelected
            {
                get
                {
                    return this.isSelected;
                }

                set
                {
                    this.isSelected = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this symbol is available
            /// </summary>
            public bool IsAvailable
            {
                get
                {
                    return this.isAvailable;
                }

                set
                {
                    this.isAvailable = value;
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
        /// A class representing a symbol monitoring session
        /// </summary>
        public class DebugOutputMonitorAPISession
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="DebugOutputMonitorAPISession" /> class.
            /// </summary>
            /// <param name="viewModel">A reference to our view model</param>
            /// <param name="monitorAPISession">The symbol monitoring session</param>
            /// <param name="canRemove">Whether or not this symbol can be removed from monitoring</param>
            public DebugOutputMonitorAPISession(DebugOutputViewModel viewModel, IMonitorAPISession monitorAPISession, bool canRemove)
            {
                this.ViewModel = viewModel;
                this.MonitorAPISession = monitorAPISession;
                this.CanRemove = canRemove;
            }

            /// <summary>
            /// Gets or sets the symbol monitoring session
            /// </summary>
            public IMonitorAPISession MonitorAPISession { get; set; }

            /// <summary>
            /// Gets or sets a reference to our view model
            /// </summary>
            public DebugOutputViewModel ViewModel { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this session can be removed
            /// </summary>
            public bool CanRemove { get; set; }
        }
    }
}
