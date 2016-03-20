// -----------------------------------------------------------------------
// <copyright file="TCRCategoryViewItem.cs" company="Microsoft">
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
    /// TCRCategoryViewItem is a ViewModel class representing the TCR Category.
    /// It's used to bind TCRCategory-specific properties in the view/XAML
    /// </summary>
    public class TCRCategoryViewItem : ITCRCategory, INotifyPropertyChanged
    {
        /// <summary>
        /// DataModel TCRVersion this ViewItem is associated with
        /// </summary>
        private readonly DataModel.TCRCategory tcrCategory;

        /// <summary>
        /// backing field for TCRVersionViewItem property
        /// </summary>
        private readonly TCRVersionViewItem tcrVersionViewItem;

        /// <summary>
        /// A reference to the MainViewModel
        /// </summary>
        private readonly MainViewModel mainViewModel;

        /// <summary>
        /// Backing field for TCRViewItems property
        /// </summary>
        private List<TCRViewItem> tcrViewItems;

        /// <summary>
        /// Backing field for IsSelected property
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Backing field for IsExpanded property
        /// </summary>
        private bool isExpanded;

        /// <summary>
        /// Backing field for IsVisible property
        /// </summary>
        private bool matchesFilter = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="TCRCategoryViewItem" /> class.
        /// </summary>
        /// <param name="tcrCategory">DataModel TCRCategory to associate this ViewItem with</param>
        /// <param name="parent">Parent TCRVersionViewItem</param>
        /// <param name="mainViewModel">A reference to the MainViewModel</param>
        public TCRCategoryViewItem(DataModel.TCRCategory tcrCategory, TCRVersionViewItem parent, MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.tcrCategory = tcrCategory;
            this.tcrVersionViewItem = parent;
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
        public DataModel.TCRCategory TCRCategory 
        {
            get { return this.tcrCategory; } 
        }

        /// <summary>
        /// Gets the name of this TCR Category.
        /// Implements Name in ITCRCategory.
        /// Returns name from TCRCategory DataModel object.
        /// </summary>
        public string Name
        {
            get { return this.TCRCategory.Name; } 
        }

        /// <summary>
        /// Gets the TCR Version associated with this TCR Version.
        /// Implements TCRVersion in ITCRCategory
        /// </summary>
        public ITCRVersion TCRVersion 
        {
            get { return this.TCRVersionViewItem; } 
        }

        /// <summary>
        /// Gets the TCRVersionViewItem of TCR Version associated with this TCR Version.
        /// </summary>
        public TCRVersionViewItem TCRVersionViewItem 
        {
            get { return this.tcrVersionViewItem; } 
        }

        /// <summary>
        /// Gets a list of ITCR objects representing the TCRs associated with this TCR Category.
        /// Implements TCRs in ITCRCategory.
        /// </summary>
        public List<ITCR> TCRs 
        {
            get { return this.TCRViewItems.Cast<ITCR>().ToList(); } 
        }

        /// <summary>
        /// Gets a list of TCRViewItems objects representing the TCRs associated with this TCR Category.
        /// </summary>
        public List<TCRViewItem> TCRViewItems
        {
            get
            {
                if (this.tcrViewItems == null)
                {
                    this.tcrViewItems = new List<TCRViewItem>();
                    foreach (DataModel.TCR t in MainViewModel.DataModel.GetTcrs(this.TCRCategory))
                    {
                        TCRViewItem tvi = new TCRViewItem(t, this, this.mainViewModel);
                        this.tcrViewItems.Add(tvi);
                    }
                }

                return this.tcrViewItems;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the current element is selected in the UI (i.e. in a TreeView)
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
        /// Gets or sets a value indicating whether or not the current element is expanded in the UI (i.e. in a TreeView)
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.isExpanded; 
            }

            set
            { 
                this.isExpanded = value;
                this.NotifyPropertyChanged(); 
            } 
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the current element is visible in the UI (i.e. in a TreeView)
        /// </summary>
        public bool MatchesFilter
        {
            get
            { 
                return this.matchesFilter; 
            } 
            
            set
            {
                this.matchesFilter = value;
                this.NotifyPropertyChanged(); 
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not there are any modules available within the context of this TCR Category
        /// </summary>
        public bool HasModules
        {
            get
            {
                bool result = false;
                List<TCRViewItem> list = this.TCRViewItems;
                foreach (TCRViewItem tcrViewItem in list)
                {
                    if (tcrViewItem.HasModules == true)
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
