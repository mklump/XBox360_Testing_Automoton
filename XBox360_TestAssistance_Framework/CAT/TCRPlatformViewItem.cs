// -----------------------------------------------------------------------
// <copyright file="TCRPlatformViewItem.cs" company="Microsoft">
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
    /// TCRPlatformViewItem is a ViewModel class representing the platform.
    /// It's used to bind platform-specific properties in the view/XAML
    /// </summary>
    public class TCRPlatformViewItem : ITCRPlatform, INotifyPropertyChanged
    {
        /// <summary>
        /// DataModel Platform this ViewItem is associated with
        /// </summary>
        private readonly DataModel.Platform platform;

        /// <summary>
        /// A reference to the MainViewModel
        /// </summary>
        private readonly MainViewModel mainViewModel;

        /// <summary>
        /// Backing field for TCRVersionViewItems property
        /// </summary>
        private List<TCRVersionViewItem> tcrVersionViewItems;

        /// <summary>
        /// Backing field for IsSelected property
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="TCRPlatformViewItem" /> class.
        /// </summary>
        /// <param name="platform">DataModel Platform object to associate this ViewItem with</param>
        /// <param name="viewModel">A reference to the MainViewModel</param>
        public TCRPlatformViewItem(DataModel.Platform platform, MainViewModel viewModel)
        {
            this.mainViewModel = viewModel;
            this.platform = platform;
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
        /// Gets the DataModel Platform this ViewItem is associated with
        /// </summary>
        public DataModel.Platform Platform 
        {
            get { return this.platform; } 
        }

        /// <summary>
        /// Gets the name of this platform.
        /// Implements Name in ITCRPlatform.
        /// Returns name from Platform DataModel object.
        /// </summary>
        public string Name 
        { 
            get { return this.Platform.Name; } 
        }

        /// <summary>
        /// Gets a list of ITCRVersion objects representing the versions associated with this platform.
        /// Implements TCRVersions in ITCRPlatform.
        /// </summary>
        public List<ITCRVersion> TCRVersions 
        {
            get { return this.TCRVersionViewItems.Cast<ITCRVersion>().ToList(); }
        }

        /// <summary>
        /// Gets a list of TCRVersionViewItem objects representing the versions associated with this platform.
        /// </summary>
        public List<TCRVersionViewItem> TCRVersionViewItems
        {
            get
            {
                if (this.tcrVersionViewItems == null)
                {
                    this.tcrVersionViewItems = new List<TCRVersionViewItem>();
                    foreach (DataModel.TCRVersion v in MainViewModel.DataModel.GetTCRVersions(this.Platform))
                    {
                        TCRVersionViewItem vvi = new TCRVersionViewItem(v, this, MainViewModel);
                        this.tcrVersionViewItems.Add(vvi);
                    }
                }

                return this.tcrVersionViewItems;
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
        /// Gets a value indicating whether or not there are any modules available within the context of this platform
        /// </summary>
        public bool HasModules
        {
            get
            {
                bool result = false;
                List<TCRVersionViewItem> list = this.TCRVersionViewItems;
                foreach (TCRVersionViewItem versionViewItem in list)
                {
                    if (versionViewItem.HasModules == true)
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
