// -----------------------------------------------------------------------
// <copyright file="TCRVersionViewItem.cs" company="Microsoft">
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
    /// TCRPlatformViewItem is a ViewModel class representing the TCR Version.
    /// It's used to bind TCRVersion-specific properties in the view/XAML
    /// </summary>
    public class TCRVersionViewItem : ITCRVersion, INotifyPropertyChanged
    {
        /// <summary>
        /// DataModel TCRVersion this ViewItem is associated with
        /// </summary>
        private readonly DataModel.TCRVersion tcrVersion;

        /// <summary>
        /// A reference to the MainViewModel
        /// </summary>
        private readonly MainViewModel mainViewModel;

        /// <summary>
        /// Backing field for TCRCategoryViewItems property
        /// </summary>
        private List<TCRCategoryViewItem> tcrCategoryViewItems;

        /// <summary>
        /// Backing field for TCRViewItems property
        /// </summary>
        private List<TCRViewItem> tcrViewItems;

        /// <summary>
        /// Backing field for IsSelected property
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="TCRVersionViewItem" /> class.
        /// </summary>
        /// <param name="version">DataModel TCRVersion to associate this ViewItem with</param>
        /// <param name="parent">Parent TCRPlatformViewItem</param>
        /// <param name="mainViewModel">A reference to the MainViewModel</param>
        public TCRVersionViewItem(DataModel.TCRVersion version, TCRPlatformViewItem parent, MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.tcrVersion = version;
            this.TCRPlatformViewItem = parent;
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
        /// Gets the DataModel TCRVersion this ViewItem is associated with
        /// </summary>
        public DataModel.TCRVersion TCRVersion
        {
            get { return this.tcrVersion; }
        }

        /// <summary>
        /// Gets the name of this TCR Version.
        /// Implements Name in ITCRVersion.
        /// Returns name from TCRVersion DataModel object.
        /// </summary>
        public string Name 
        {
            get { return this.tcrVersion.Name; } 
        }

        /// <summary>
        /// Gets the platform associated with this TCR Version.
        /// Implements Platform in ITCRVersion
        /// </summary>
        public ITCRPlatform Platform 
        {
            get { return this.TCRPlatformViewItem; } 
        }

        /// <summary>
        /// Gets or sets the TCRPlatformViewItem of Platform associated with this TCR Version.
        /// </summary>
        public TCRPlatformViewItem TCRPlatformViewItem { get; set; }

        /// <summary>
        /// Gets a list of ITCRCategory objects representing the categories associated with this TCR Version.
        /// Implements TCRVersions in ITCRVersion.
        /// </summary>
        public List<ITCRCategory> TCRCategories
        {
            get { return this.TCRCategoryViewItems.Cast<ITCRCategory>().ToList(); } 
        }

        /// <summary>
        /// Gets a list of TCRCategoryViewItem objects representing the categories associated with this TCR Version.
        /// </summary>
        public List<TCRCategoryViewItem> TCRCategoryViewItems
        {
            get
            {
                if (this.tcrCategoryViewItems == null)
                {
                    this.tcrCategoryViewItems = new List<TCRCategoryViewItem>();
                    foreach (DataModel.TCRCategory v in MainViewModel.DataModel.GetCategories(this.TCRVersion))
                    {
                        TCRCategoryViewItem cvi = new TCRCategoryViewItem(v, this, this.mainViewModel);
                        this.tcrCategoryViewItems.Add(cvi);
                    }
                }

                return this.tcrCategoryViewItems;
            }
        }

        /// <summary>
        /// Gets a list of ITCR objects representing the TCRs associated with this TCR Version.
        /// Implements TCRs in ITCRVersion.
        /// </summary>
        public List<ITCR> TCRs
        {
            get { return this.TCRViewItems.Cast<ITCR>().ToList(); }
        }

        /// <summary>
        /// Gets a list of TCRViewItems objects representing the TCRs associated with this TCR Version.
        /// </summary>
        public List<TCRViewItem> TCRViewItems
        {
            get
            {
                if (this.tcrViewItems == null)
                {
                    this.tcrViewItems = new List<TCRViewItem>();
                    List<TCRCategoryViewItem> catList = this.TCRCategoryViewItems;
                    foreach (TCRCategoryViewItem cvi in catList)
                    {
                        this.tcrViewItems.AddRange(cvi.TCRViewItems);
                    }
                }

                return this.tcrViewItems;
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
        /// Gets a value indicating whether or not there are any modules available within the context of this TCR Version
        /// </summary>
        public bool HasModules
        {
            get
            {
                bool result = false;
                List<TCRCategoryViewItem> list = this.TCRCategoryViewItems;
                foreach (TCRCategoryViewItem categoryViewItem in list)
                {
                    if (categoryViewItem.HasModules == true)
                    {
                        result = true;
                        break;
                    }
                }

                return result;
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
}
