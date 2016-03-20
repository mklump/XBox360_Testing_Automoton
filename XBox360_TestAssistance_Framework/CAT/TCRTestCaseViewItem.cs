// -----------------------------------------------------------------------
// <copyright file="TCRTestCaseViewItem.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// TCRTestCaseViewItem is a ViewModel class representing a TCR test case.
    /// It's used to bind test case specific properties in the view/XAML
    /// </summary>
    public class TCRTestCaseViewItem : ITCRTestCase, INotifyPropertyChanged
    {
        /// <summary>
        /// DataModel TCRTestCase this ViewItem is associated with
        /// </summary>
        private readonly DataModel.TCRTestCase tcrTestCase;

        /// <summary>
        /// A reference to the MainViewModel
        /// </summary>
        private readonly MainViewModel mainViewModel;

        /// <summary>
        /// Backing field for CATModuleViewItems property
        /// </summary>
        private List<ModuleViewItem> catModuleViewItems;

        /// <summary>
        /// Backing field for IsSelected property
        /// </summary>
        private bool isSelected = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="TCRTestCaseViewItem" /> class.
        /// </summary>
        /// <param name="testCase">DataModel TCRTestCase to associate this ViewItem with</param>
        /// <param name="parent">Parent TCRViewItem</param>
        /// <param name="mainViewModel">A reference to the MainViewModel</param>
        public TCRTestCaseViewItem(DataModel.TCRTestCase testCase, TCRViewItem parent, MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.tcrTestCase = testCase;
            this.TCRViewItem = parent;
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
        /// Gets the DataModel TCRTestCase this ViewItem is associated with
        /// </summary>
        public DataModel.TCRTestCase TCRTestCase
        {
            get { return this.tcrTestCase; }
        }

        /// <summary>
        /// Gets the name of this TCR Test Case.
        /// Implements Name in ITCRTestCase.
        /// Returns name from TCRTestCase DataModel object.
        /// </summary>
        public string Name 
        {
            get { return this.TCRTestCase.Name; }
        }

        /// <summary>
        /// Gets the Id of the TCR Test Case
        /// Implements Id in ITCRTestCase.
        /// Returns Id from TCRTestCase DataModel object.
        /// </summary>
        public string Id
        {
            get { return this.TCRTestCase.Id; }
        }

        /// <summary>
        /// Gets the Requirements field of the TCR test case
        /// Implements Requirements in ITCRTestCase.
        /// Returns Requirements from TCRTestCase DataModel object.
        /// </summary>
        public string Requirements
        {
            get { return this.TCRTestCase.Requirements; }
        }

        /// <summary>
        /// Gets the Configuration field of the TCR test case
        /// Implements Configuration in ITCRTestCase.
        /// Returns Configuration from TCRTestCase DataModel object.
        /// </summary>
        public string Configuration
        {
            get { return this.TCRTestCase.Configuration; }
        }

        /// <summary>
        /// Gets the Definitions field of the TCR test case
        /// Implements Definitions in ITCRTestCase.
        /// Returns Definitions from TCRTestCase DataModel object.
        /// </summary>
        public string Definition
        {
            get { return this.TCRTestCase.Definition; }
        }

        /// <summary>
        /// Gets the Steps field of the TCR test case
        /// Implements Steps in ITCRTestCase.
        /// Returns Steps from TCRTestCase DataModel object.
        /// </summary>
        public string Steps
        {
            get { return this.TCRTestCase.Steps; }
        }

        /// <summary>
        /// Gets the Documentation field of the TCR test case
        /// Implements Documentation in ITCRTestCase.
        /// Returns Documentation from TCRTestCase DataModel object.
        /// </summary>
        public string Documentation
        {
            get { return this.TCRTestCase.Documentation; }
        }

        /// <summary>
        /// Gets the Result field of the TCR Test Case
        /// Implements Result in ITCRTestCase.
        /// Returns Result from TCRTestCase DataModel object.
        /// </summary>
        public string Result
        {
            get { return this.TCRTestCase.Result; }
        }

        /// <summary>
        /// Gets the Pass Examples field of the TCR Test Case
        /// Implements Pass Examples in ITCRTestCase.
        /// Returns Pass Examples from TCRTestCase DataModel object.
        /// </summary>
        public string PassExamples
        {
            get { return this.TCRTestCase.PassExamples; }
        }

        /// <summary>
        /// Gets the Fail Example field of the TCR test case
        /// Implements Fail Example in ITCRTestCase.
        /// Returns Fail Example from TCRTestCase DataModel object.
        /// </summary>
        public string FailExamples
        {
            get { return this.TCRTestCase.FailExamples; }
        }

        /// <summary>
        /// Gets the N/A Examples field of the TCR Test Case
        /// Implements N/A Examples in ITCRTestCase.
        /// Returns N/A Examples from TCRTestCase DataModel object.
        /// </summary>
        public string NaExamples
        {
            get { return this.TCRTestCase.NaExamples; }
        }

        /// <summary>
        /// Gets the Analysis field of the TCR Test case
        /// Implements Analysis in ITCRTestCase.
        /// Returns Analysis from TCRTestCase DataModel object.
        /// </summary>
        public string Analysis
        {
            get { return this.TCRTestCase.Analysis; }
        }

        /// <summary>
        /// Gets the FAQ field of the TCR Test Case
        /// Implements FAQ in ITCRTestCase.
        /// Returns FAQ from TCRTestCase DataModel object.
        /// </summary>
        public string Faq
        {
            get { return this.TCRTestCase.Faq; }
        }

        /// <summary>
        /// Gets the Tools field of the TCR Test Case
        /// Implements Tools in ITCRTestCase.
        /// Returns Tools from TCRTestCase DataModel object.
        /// </summary>
        public string Tools
        {
            get { return this.TCRTestCase.Tools; }
        }

        /// <summary>
        /// Gets the Hardware field of the TCR Test Case
        /// Implements Hardware in ITCRTestCase.
        /// Returns Hardware from TCRTestCase DataModel object.
        /// </summary>
        public string Hardware
        {
            get { return this.TCRTestCase.Hardware; }
        }

        /// <summary>
        /// Gets the TCR associated with this TCR Test Case.
        /// Implements TCR in ITCRTestCase
        /// </summary>
        public ITCR TCR 
        {
            get { return this.TCRViewItem; } 
        }

        /// <summary>
        /// Gets or sets the TCRViewItem of TCR associated with this TCR Test Case.
        /// </summary>
        public TCRViewItem TCRViewItem { get; set; }

        /// <summary>
        /// Gets a list of IModule objects representing the CAT Modules associated with this TCR Test Case.
        /// Implements CATModules in ITCRTestCase
        /// </summary>
        public List<IModule> CATModules
        {
            get { return this.CATModuleViewItems.Cast<IModule>().ToList(); } 
        }

        /// <summary>
        /// Gets a list of IModule objects representing the CAT Modules associated with this TCR Test Case.
        /// </summary>
        public List<ModuleViewItem> CATModuleViewItems
        {
            get
            {
                if (this.catModuleViewItems == null)
                {
                    this.catModuleViewItems = new List<ModuleViewItem>();
                    foreach (DataModel.CATModuleInfo catModule in MainViewModel.DataModel.GetModules(this.TCRTestCase))
                    {
                        ModuleViewItem cmvi = new ModuleViewItem(catModule, this, this.mainViewModel);
                        this.catModuleViewItems.Add(cmvi);
                    }
                }

                return this.catModuleViewItems;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the current element is selected in the UI (i.e. in a TreeView)
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
        /// Gets a value indicating whether there is a module associated with this test case
        /// </summary>
        public bool HasModules 
        {
            get { return this.CATModuleViewItems.Count != 0; } 
        }

        /// <summary>
        /// Gets the Id and Name of this test case in a single string
        /// </summary>
        public string IdAndName 
        {
            get { return this.TCRTestCase.Id + " - " + this.TCRTestCase.Name; }
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
}
