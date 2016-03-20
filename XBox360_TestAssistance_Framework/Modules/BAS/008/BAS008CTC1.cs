// -----------------------------------------------------------------------
// <copyright file="BAS008CTC1.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace BAS008
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using CAT;
    using XDevkit;

    /// <summary>
    /// Module BAS008CTC1
    /// </summary>
    public class BAS008CTC1 : IModule, INotifyPropertyChanged
    {
        /// <summary>
        /// Current working module context, inherited from IModule, for this module
        /// </summary>
        private IXboxModuleContext moduleContext;

        /// <summary>
        /// UIElement control for this module
        /// </summary>
        private BAS008CTC1UI moduleUI;

        /// <summary>
        /// First page visibility property
        /// </summary>
        private Visibility firstPageVisibility = Visibility.Visible;

        /// <summary>
        /// Second page visibility property
        /// </summary>
        private Visibility secondPageVisibility = Visibility.Collapsed;

        /// <summary>
        /// Xbox the module is running against
        /// </summary>
        private IXboxDevice xboxDevice;

        /// <summary>
        /// Backing field for AnyFailed property
        /// </summary>
        private bool anyFailed;

        /// <summary>
        /// Path to temporary directory on PC for files copied from Xbox DVD
        /// </summary>
        private string localDiscEmulationPath;

        /// <summary>
        /// Backing field for AnyInDebugFolder property
        /// </summary>
        private bool anyInDebugFolder;

        /// <summary>
        /// Backing property for ScannedFiles property
        /// </summary>
        private List<FileItem> scannedFiles;

        /// <summary>
        /// Backing field for the CurrentlySelectedFile property
        /// </summary>
        private FileItem currentlySelectedFile;

        /// <summary>
        /// Backing field for the CurrentSystemImportLibraries property
        /// </summary>
        private List<LinkItem> currentSystemImportLibraries;

        /// <summary>
        /// Backing field for the CurrentLibraryVersions property
        /// </summary>
        private List<LinkItem> currentLibraryVersions;

        /// <summary>
        /// Backing field for the CurrentToolVersions property
        /// </summary>
        private List<LinkItem> currentToolVersions;

        /// <summary>
        /// Event declaration for the property changed event handler
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a string representing the version of the XDK installed on this computer
        /// </summary>
        public string XdkVersion
        {
            get { return this.moduleContext.XdkVersion; }
        }

        /// <summary>
        /// Gets the UIElement control for this module
        /// </summary>
        public UIElement UIElement
        {
            get { return this.moduleUI as UIElement; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the first page is visible
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
        /// Gets or sets a value indicating whether or not the second page is visible
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
        /// Gets or sets a value indicating whether or not the second page is visible
        /// </summary>
        public bool AnyFailed
        {
            get
            {
                return this.anyFailed;
            }

            set
            {
                this.anyFailed = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there were any files skipped because they were in a local folder containing the string "debug"
        /// </summary>
        public bool AnyInDebugFolder
        {
            get
            {
                return this.anyInDebugFolder;
            }

            set
            {
                this.anyInDebugFolder = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value containing a list of scanned files
        /// </summary>
        public List<FileItem> ScannedFiles
        {
            get { return this.scannedFiles; }
            set { this.scannedFiles = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating the currently selected scanned file
        /// </summary>
        public FileItem CurrentlySelectedFile
        {
            get
            {
                return this.currentlySelectedFile;
            }

            set
            {
                this.currentlySelectedFile = value;
                this.NotifyPropertyChanged();
                if (value == null)
                {
                    this.CurrentSystemImportLibraries = null;
                    this.CurrentLibraryVersions = null;
                    this.CurrentToolVersions = null;
                }
                else
                {
                    this.CurrentSystemImportLibraries = value.SystemImportLibraries;
                    this.CurrentLibraryVersions = value.LibraryVersions;
                    this.CurrentToolVersions = value.ToolVersions;
                }
            }
        }

        /// <summary>
        /// Gets or sets a current set of linked libraries to display in the UI (for the currently selected file).
        /// </summary>
        public List<LinkItem> CurrentSystemImportLibraries
        {
            get
            {
                return this.currentSystemImportLibraries;
            }

            set
            {
                this.currentSystemImportLibraries = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a current set of linked libraries to display in the UI (for the currently selected file).
        /// </summary>
        public List<LinkItem> CurrentLibraryVersions
        {
            get
            {
                return this.currentLibraryVersions;
            }

            set
            {
                this.currentLibraryVersions = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a current set of linked libraries to display in the UI (for the currently selected file).
        /// </summary>
        public List<LinkItem> CurrentToolVersions
        {
            get
            {
                return this.currentToolVersions;
            }

            set
            {
                this.currentToolVersions = value;
                this.NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether whether there is one connected xbox selected
        /// </summary>
        private bool IsOneConnectedXboxSelected
        {
            get
            {
                string s = string.Empty;
                if (this.moduleContext.SelectedDevices.Count() == 0)
                {
                    s = "No consoles are selected. Select one. ";
                }
                else if (this.moduleContext.SelectedDevices.Count() > 1)
                {
                    s = this.moduleContext.SelectedDevices.Count().ToString() + " consoles are selected. Select just one. ";
                }

                this.xboxDevice = this.moduleContext.SelectedDevices[0] as IXboxDevice;

                // connected
                if (this.xboxDevice.Connected == false)
                {
                    s += "The selected device " + this.xboxDevice.Name + " is not connected. Connect the device.";
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
        /// Start - called when the module is first entered
        /// This function is called to show the overview or intro to the module.
        /// Typically the framework is active and user should choose a device in the device pool.
        /// </summary>
        /// <param name="ctx">The current working context for which this test will execute.</param>
        public void Start(IModuleContext ctx)
        {
            this.localDiscEmulationPath = string.Empty;
            this.AnyFailed = false;
            this.CurrentlySelectedFile = null;
            this.scannedFiles = new List<FileItem>();
            this.moduleContext = ctx as IXboxModuleContext;
            this.moduleUI = new BAS008CTC1UI(this);
        }

        /// <summary>
        /// Stop - called when the module is done or aborted
        /// </summary>
        public void Stop()
        {
            string passedOrFailed = this.AnyFailed ? "FAILED" : "PASSED";

            this.moduleContext.Log("*************************************************************");
            this.moduleContext.Log("RESULT: " + passedOrFailed);
            this.moduleContext.Log("*************************************************************");
        }

        /// <summary>
        /// Advance to the second page of the module
        /// </summary>
        public void Begin()
        {
            bool foundGameDirectory = false;
            if (!string.IsNullOrEmpty(this.moduleContext.XboxTitle.GameDirectory))
            {
                try
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(this.moduleContext.XboxTitle.GameDirectory);
                    foundGameDirectory = true;
                }
                catch (Exception)
                {
                }
            }

            if (!foundGameDirectory)
            {
                MessageBox.Show("Unable to find the game directory, or the game directory is not configured.  Please configure the game title in settings.", "Game Directory not found");
                return;
            }

            if ((this.moduleContext.XboxTitle.GameInstallType == "Disc Emulation") && (!this.IsOneConnectedXboxSelected))
            {
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;

            this.AddDirectoryToList(this.moduleContext.XboxTitle.GameDirectory);

            if (this.moduleContext.XboxTitle.GameInstallType == "Disc Emulation")
            {
                this.LoadDiscFiles();
            }

            Mouse.OverrideCursor = null;

            if (this.ScannedFiles.Count == 0)
            {
                if (this.AnyInDebugFolder)
                {
                    MessageBox.Show("Files in directories containing 'Debug' were excluded.  Unable to find any suitable xex, exe, or dll files to scan.", "Unable to find files");
                }
                else
                {
                    MessageBox.Show("Unable to find any suitable xex, exe, or dll files to scan.", "Unable to find files");
                }
            }
            else
            {
                // disable xbox selection, tcr selection and title changes
                this.moduleContext.IsModal = true;

                this.FirstPageVisibility = Visibility.Collapsed;
                this.SecondPageVisibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Adds a file to the list of scanned files.
        /// </summary>
        /// <param name="fileName">File to scan</param>
        public void AddFileToList(string fileName)
        {
            FileItem fileItem = new FileItem();
            fileItem.FileName = fileName;
            string logFileName;
            if (fileName.StartsWith(this.moduleContext.XboxTitle.GameDirectory))
            {
                fileItem.ShortFileName = fileName.Substring(this.moduleContext.XboxTitle.GameDirectory.Length + 1);
                logFileName = fileItem.ShortFileName;
            }
            else 
            {
                logFileName = fileName.Substring(this.localDiscEmulationPath.Length + 1);
                fileItem.ShortFileName = @"DVD:\" + logFileName;
                logFileName = Path.Combine("DVD", logFileName);
            }

            string exePath = this.moduleContext.XdkToolPath + "\\Imagexex.exe";
            FileInfo info = new FileInfo(exePath);
            if (!info.Exists)
            {
                throw new Exception("Imagexex.exe not found");
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = exePath;
            startInfo.Arguments = string.Format("/DUMP \"{0}\"", fileName);
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardError = true;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            bool foundAny = false;
            string output = process.StandardOutput.ReadToEnd();

            string[] parts = Regex.Split(output, "\r\n");

            int parseMode = 0;  // 0 = no header found yet, 1 = SYSTEM IMPORT LIBRARIES found, 2 = LIBRARY VERSIONS found, 3 = TOOL VERSIONS found
            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part))
                {
                    parseMode = 0;
                }
                else if (part == "SYSTEM IMPORT LIBRARIES")
                {
                    parseMode = 1;
                }
                else if (part == "LIBRARY VERSIONS")
                {
                    parseMode = 2;
                }
                else if (part == "TOOL VERSIONS")
                {
                    parseMode = 3;
                }
                else
                {
                    switch (parseMode)
                    {
                        case 1:
                            {
                                Match m = Regex.Match(part, @"^(\s*)([A-Za-z0-9\.]+)(\s+)version(\s+)([0-9\.]+)(\s+)\(minimum(\s+)([0-9\.]+)\)$");
                                if (m.Success)
                                {
                                    foundAny = true;
                                    string str = m.ToString().Trim();
                                    str = Regex.Replace(str, @"\s+", " ");  // Turn multiple consecutive white space into 1 char
                                    string[] linkParts = str.Split(null);
                                    LinkItem linkItem = new LinkItem();
                                    linkItem.Name = linkParts[0];
                                    linkItem.FullVersionString = linkParts[2];
                                    linkItem.MinimumVersionString = linkParts[4].Substring(0, linkParts[4].Length - 1);
                                    linkItem.Approved = !str.StartsWith("xbdm.xex"); // Only complain if xbdm.xex is present
                                    fileItem.SystemImportLibraries.Add(linkItem);
                                    fileItem.BadFileFound |= !linkItem.Approved;
                                    this.AnyFailed |= fileItem.BadFileFound;
                                }
                            }

                            break;
                        case 2:
                        case 3:
                            // Check for LIBRARY VERSIONS and TOOL VERSIONS
                            {
                                Match m = Regex.Match(part, @"^(\s*)([A-Za-z0-9\.]+)(\s+)([0-9\.]+)(\s+)\[.+\](\s*)$");
                                if (m.Success)
                                {
                                    foundAny = true;
                                    string str = m.ToString().Trim();
                                    string[] linkParts = str.Split(null);
                                    LinkItem linkItem = new LinkItem();
                                    linkItem.Name = linkParts[0];
                                    linkItem.FullVersionString = linkParts[1];
                                    linkItem.ApprovedString = linkParts[2].Substring(1, linkParts[2].Length - 2);
                                    linkItem.Approved = linkItem.ApprovedString == "approved";
                                    if (parseMode == 2)
                                    {
                                        fileItem.LibraryVersions.Add(linkItem);
                                    }
                                    else
                                    {
                                        fileItem.ToolVersions.Add(linkItem);
                                    }
                                    
                                    fileItem.BadFileFound |= !linkItem.Approved;
                                    this.AnyFailed |= fileItem.BadFileFound;
                                }
                            }

                            break;
                        default:
                            break;
                    }
                }
            }

            if (foundAny)
            {
                string dumpDirectory = Path.Combine(this.moduleContext.LogDirectory, "ImageXex Dumps");
                Directory.CreateDirectory(dumpDirectory);
                string filePath = Path.Combine(dumpDirectory, logFileName);
                string fileDirectory = Path.GetDirectoryName(filePath);
                Directory.CreateDirectory(fileDirectory);
                File.AppendAllText(filePath + ".txt", output);

                if (fileItem.BadFileFound)
                {
                    this.moduleContext.Log("FAILED: " + fileItem.ShortFileName);
                }
                else
                {
                    this.moduleContext.Log("PASSED: " + fileItem.ShortFileName);
                }

                this.scannedFiles.Add(fileItem);
                if (this.CurrentlySelectedFile == null)
                {
                    this.CurrentlySelectedFile = fileItem;
                }
            }
        }

        /// <summary>
        /// Add all files with the specified extension, in the specified directory, to the scanned files list
        /// </summary>
        /// <param name="directoryName">Root path of files to scan</param>
        /// <param name="extension">File extension of files to scan</param>
        public void AddDirectoryToList(string directoryName, string extension)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directoryName);
            FileInfo[] files = dirInfo.GetFiles("*." + extension, SearchOption.AllDirectories);
            foreach (FileInfo fileInfo in files)
            {
                string adjustedPath = fileInfo.FullName.Substring(directoryName.Length);
                if (adjustedPath.ToLower().Contains("debug"))
                {
                    this.AnyInDebugFolder = true;
                }
                else
                {
                    this.AddFileToList(fileInfo.FullName);
                }
            }
        }

        /// <summary>
        /// Add all files in the specified directory, with supported extensions, to the scanned files list
        /// </summary>
        /// <param name="directoryName">Root path of files to scan</param>
        public void AddDirectoryToList(string directoryName)
        {
            this.AddDirectoryToList(directoryName, "xex");
            this.AddDirectoryToList(directoryName, "exe");
            this.AddDirectoryToList(directoryName, "dll");
        }

        /// <summary>
        /// Loads disc emulated title and copies all executable files from it, to a temporary directory on the PC
        /// </summary>
        private void LoadDiscFiles()
        {
            // Load disc emulation, copy all xex, exe, and dll files from the disc image
            this.xboxDevice.InstallTitle(string.Empty, this.moduleContext.OpenProgressBarWindow("Installing " + this.moduleContext.XboxTitle.Name + "..."));

            Mouse.OverrideCursor = Cursors.Wait;

            // Create temp dir
            this.localDiscEmulationPath = Path.Combine(Path.GetTempPath(), "FilesExtractedFromGameDVD");
            DirectoryInfo dirInfo = new DirectoryInfo(this.localDiscEmulationPath);
            dirInfo.Create();

            // Delete all old files from temp directory
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo subDirectory in dirInfo.GetDirectories())
            {
                subDirectory.Delete(true);
            }

            this.LoadDiscDirectoryRecursive("DVD:\\", this.localDiscEmulationPath);
            this.AddDirectoryToList(this.localDiscEmulationPath);
        }

        /// <summary>
        /// Recursive scan directories on the Xbox for files, and copy them to the PC.
        /// </summary>
        /// <param name="xboxPath">Root path to files on the Xbox</param>
        /// <param name="localPathRoot">Directory on PC to copy files to</param>
        private void LoadDiscDirectoryRecursive(string xboxPath, string localPathRoot)
        {
            if (xboxPath.Contains("$SystemUpdate"))
            {
                // Don't include contents of system update directory, if present
                return;
            }

            IXboxFiles files = this.xboxDevice.XboxConsole.DirectoryFiles(xboxPath);
            foreach (IXboxFile file in files)
            {
                string fileName = file.Name;
                if (file.IsDirectory)
                {
                    this.LoadDiscDirectoryRecursive(fileName, localPathRoot);
                }
                else
                {
                    if (fileName.EndsWith(".xex") || fileName.EndsWith(".exe") || fileName.EndsWith(".dll"))
                    {
                        // Preserve the path
                        string destinationFile = Path.Combine(localPathRoot, fileName.Substring(5));

                        string destinationDirectory = Path.GetDirectoryName(destinationFile);
                        DirectoryInfo dirInfo = new DirectoryInfo(destinationDirectory);
                        dirInfo.Create();

                        this.xboxDevice.XboxConsole.ReceiveFile(destinationFile, fileName);
                    }
                }
            }
        }

        /// <summary>
        /// Notify property changed event handler function for all property bound controls
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// A class representing a information about a linked library in an executable or library
        /// </summary>
        public class LinkItem
        {
            /// <summary>
            /// Gets or sets the file name associated with this item.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the full version string associated with this item.
            /// </summary>
            public string FullVersionString { get; set; }

            /// <summary>
            /// Gets or sets the minimum version associated with this item
            /// </summary>
            public string MinimumVersionString { get; set; }

            /// <summary>
            /// Gets or sets a string indicating whether this item was indicated as approved
            /// </summary>
            public string ApprovedString { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this item was indicated as approved
            /// </summary>
            public bool Approved { get; set; }

            /// <summary>
            /// Gets a color, either red or green, indicating the status of this item.
            /// </summary>
            public Brush ApprovedColor
            {
                get
                {
                    if (this.Approved)
                    {
                        return Brushes.Green;
                    }

                    return Brushes.Red;
                }
            }
        }

        /// <summary>
        /// A class representing a scanned file
        /// </summary>
        public class FileItem : INotifyPropertyChanged
        {
            /// <summary>
            /// Backing field for FileName property
            /// </summary>
            private string fileName;

            /// <summary>
            /// Backing field for ShortFileName property
            /// </summary>
            private string shortFileName;

            /// <summary>
            /// Backing field for SystemImportLibraries property
            /// </summary>
            private List<LinkItem> systemImportLibraries = new List<LinkItem>();

            /// <summary>
            /// Backing field for LibraryVersions property
            /// </summary>
            private List<LinkItem> libraryVersions = new List<LinkItem>();

            /// <summary>
            /// Backing field for ToolVersions property
            /// </summary>
            private List<LinkItem> toolVersions = new List<LinkItem>();

            /// <summary>
            /// Backing field for BadFileFound property
            /// </summary>
            private bool badFileFound;

            /// <summary>
            /// Event declaration for the property changed event handler
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets or sets the file name associate with this file item
            /// </summary>
            public string FileName
            {
                get
                {
                    return this.fileName;
                }

                set
                {
                    this.fileName = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets a shortened version of the file name associated with this file item
            /// </summary>
            public string ShortFileName
            {
                get
                {
                    return this.shortFileName;
                }

                set
                {
                    this.shortFileName = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets a list of linked items associated with this file item
            /// </summary>
            public List<LinkItem> SystemImportLibraries
            {
                get
                {
                    return this.systemImportLibraries;
                }

                set
                {
                    this.systemImportLibraries = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets a list of linked items associated with this file item
            /// </summary>
            public List<LinkItem> LibraryVersions
            {
                get
                {
                    return this.libraryVersions;
                }

                set
                {
                    this.libraryVersions = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets a list of linked items associated with this file item
            /// </summary>
            public List<LinkItem> ToolVersions
            {
                get
                {
                    return this.toolVersions;
                }

                set
                {
                    this.toolVersions = value;
                    this.NotifyPropertyChanged();
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this file item as a bad linked item associated with it.
            /// </summary>
            public bool BadFileFound
            {
                get
                {
                    return this.badFileFound;
                }

                set
                {
                    this.badFileFound = value;
                    this.NotifyPropertyChanged();
                    this.NotifyPropertyChanged("BadFileBackgroundColor");
                }
            }

            /// <summary>
            /// Gets a color, either red or green, indicating the status of this item.
            /// </summary>
            public Brush BadFileBackgroundColor
            {
                get
                {
                    if (this.badFileFound)
                    {
                        return Brushes.Red;
                    }

                    return Brushes.Green;
                }
            }

            /// <summary>
            /// Notify property changed event handler function for all property bound controls
            /// </summary>
            /// <param name="propertyName">Name of the property that changed</param>
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    } // End of: public class BAS008CTC1Module : IModule, INotifyPropertyChanged
} // End of: namespace BAS008 in code file BAS008CTC1.cs