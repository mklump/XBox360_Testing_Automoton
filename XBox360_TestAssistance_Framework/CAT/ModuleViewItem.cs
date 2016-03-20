// -----------------------------------------------------------------------
// <copyright file="ModuleViewItem.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// View Model class for CAT Modules
    /// </summary>
    public class ModuleViewItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Backing variable for the ModuleInfo property
        /// </summary>
        private readonly DataModel.CATModuleInfo moduleInfo;

        /// <summary>
        /// Backing variable for the IsSelected property
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleViewItem" /> class.
        /// </summary>
        /// <param name="catModuleInfo">CATModuleInfo to associate with this ModuleViewItem</param>
        /// <param name="parent">TCRTestCaseViewItem to associate with this ModuleViewItem</param>
        /// <param name="mainViewModel">A reference to the MainViewModel</param>
        public ModuleViewItem(DataModel.CATModuleInfo catModuleInfo, TCRTestCaseViewItem parent, MainViewModel mainViewModel)
        {
            this.moduleInfo = catModuleInfo;
            this.TCRTestCaseViewItem = parent;
        }

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a reference to the associated IModule object
        /// </summary>
        public IModule ModuleInstance { get; set; }

        /// <summary>
        /// Gets or sets a reference to the associated ModuleContext object
        /// </summary>
        public ModuleContext ModuleContext { get; set; }

        /// <summary>
        /// Gets a reference to the associated CATModuleInfo object
        /// </summary>
        public DataModel.CATModuleInfo ModuleInfo
        {
            get { return this.moduleInfo; }
        }

        /// <summary>
        /// Gets or sets a reference to the associated TCRTestCaseViewItem
        /// </summary>
        public TCRTestCaseViewItem TCRTestCaseViewItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this module is currently selected
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
        /// Gets the name of this module
        /// </summary>
        public string Name
        {
            get { return this.moduleInfo.Name; }
        }

        /// <summary>
        /// Gets the description of this module
        /// </summary>
        public string Description
        {
            get { return this.moduleInfo.Description; }
        }

        /// <summary>
        /// Gets detail of this module
        /// </summary>
        public string Detail
        {
            get { return this.moduleInfo.Detail; }
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
