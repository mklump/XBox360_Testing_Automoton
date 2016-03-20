// -----------------------------------------------------------------------
// <copyright file="TCRViewItem.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Documents;
    using System.Windows.Markup;

    /// <summary>
    /// TCRViewItem is a ViewModel class representing the TCR.
    /// It's used to bind TCR-specific properties in the view/XAML
    /// </summary>
    public class TCRViewItem : ITCR, INotifyPropertyChanged
    {
        /// <summary>
        /// DataModel TCR this ViewItem is associated with
        /// </summary>
        private readonly DataModel.TCR tcr;

        /// <summary>
        /// A reference to the MainViewModel
        /// </summary>
        private readonly DataModel.TCRCategory tcrCategory;

        /// <summary>
        /// A reference to the MainViewModel
        /// </summary>
        private readonly TCRCategoryViewItem tcrCategoryViewItem;

        /// <summary>
        /// A reference to the MainViewModel
        /// </summary>
        private readonly MainViewModel mainViewModel;

        /// <summary>
        /// Backing field for IsVisible property
        /// </summary>
        private bool matchesFilter = true;

        /// <summary>
        /// Backing field for IsSelected property
        /// </summary>
        private bool isSelected;

        /// <summary>
        /// Backing field for TCRTestCaseViewItems property
        /// </summary>
        private List<TCRTestCaseViewItem> tcrTestCaseViewItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="TCRViewItem" /> class.
        /// </summary>
        /// <param name="tcr">DataModel TCR to associate this ViewItem with</param>
        /// <param name="parent">Parent TCRCategoryViewItem</param>
        /// <param name="mainViewModel">A reference to the MainViewModel</param>
        public TCRViewItem(DataModel.TCR tcr, TCRCategoryViewItem parent, MainViewModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            this.tcr = tcr;
            this.tcrCategoryViewItem = parent;
            this.tcrCategory = parent.TCRCategory;
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
        /// Gets the DataModel TCR this ViewItem is associated with
        /// </summary>
        public DataModel.TCR TCR
        {
            get { return this.tcr; }
        }

        /// <summary>
        /// Gets the name of this TCR.
        /// Implements Name in ITCR.
        /// Returns name from TCR DataModel object.
        /// </summary>
        public string Name 
        {
            get { return this.TCR.Name; } 
        }

        /// <summary>
        /// Gets the Id field of the TCR
        /// </summary>
        public string Id
        {
            get { return this.TCR.Number.ToString(); }
        }

        /// <summary>
        /// Gets the TCR number
        /// Implements Number in ITCR.
        /// Returns Number from TCR DataModel object.
        /// </summary>
        public int Number 
        {
            get { return this.TCR.Number; }
        }

        /// <summary>
        /// Gets the requirements field of the TCR
        /// Implements Requirements in ITCR.
        /// Returns Requirements from TCR DataModel object.
        /// </summary>
        public string Requirements
        {
            get { return this.TCR.Requirements; }
        }

        /// <summary>
        /// Gets the intents field of the TCR
        /// Implements Intent in ITCR.
        /// Returns Intent from TCR DataModel object.
        /// </summary>
        public string Intent
        {
            get { return this.TCR.Intent; }
        }

        /// <summary>
        /// Gets the Remarks field of the TCR
        /// Implements Remarks in ITCR.
        /// Returns Remarks from TCR DataModel object.
        /// </summary>
        public string Remarks
        {
            get { return this.TCR.Remarks; }
        }

        /// <summary>
        /// Gets the Exemptions field of this TCR
        /// Implements Exemptions in ITCR.
        /// Returns Exemptions from TCR DataModel object.
        /// </summary>
        public string Exemptions
        {
            get { return this.TCR.Exemptions; }
        }

        /// <summary>
        /// Gets the TCR Category associated with this TCR.
        /// Implements TCRCategory in ITCR
        /// </summary>
        public ITCRCategory TCRCategory 
        {
            get { return this.tcrCategoryViewItem; } 
        }

        /// <summary>
        /// Gets the TCRCategoryViewItem of TCR Category associated with this TCR.
        /// </summary>
        public TCRCategoryViewItem TCRCategoryViewItem
        {
            get { return this.tcrCategoryViewItem; } 
        }

        /// <summary>
        /// Gets a list of ITCRTestCase objects representing the TCR Test cases associated with this TCR.
        /// Implements TCRTestCases in ITCR.
        /// </summary>
        public List<ITCRTestCase> TCRTestCases 
        {
            get { return this.TCRTestCaseViewItems.Cast<ITCRTestCase>().ToList(); } 
        }

        /// <summary>
        /// Gets a list of TCRTestCaseViewItem objects representing the TCR Test Cases associated with this TCR.
        /// </summary>
        public List<TCRTestCaseViewItem> TCRTestCaseViewItems
        {
            get
            {
                if (this.tcrTestCaseViewItems == null)
                {
                    this.tcrTestCaseViewItems = new List<TCRTestCaseViewItem>();
                    foreach (DataModel.TCRTestCase t in MainViewModel.DataModel.GetTCRTestCases(this.TCR))
                    {
                        TCRTestCaseViewItem tvi = new TCRTestCaseViewItem(t, this, this.mainViewModel);
                        this.tcrTestCaseViewItems.Add(tvi);
                    }
                }

                return this.tcrTestCaseViewItems;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this TreeView item is expended (actually ignored on TCRViewItem.  Only used on TCRCategoryViewItem)
        /// </summary>
        public bool IsExpanded { get; set; }

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
        /// Gets a value indicating whether or not there are any modules available within the context of this TCR
        /// </summary>
        public bool HasModules
        {
            get
            {
                bool result = false;
                List<TCRTestCaseViewItem> list = this.TCRTestCaseViewItems;
                foreach (TCRTestCaseViewItem tcrTestCaseViewItem in list)
                {
                    if (tcrTestCaseViewItem.HasModules == true)
                    {
                        result = true;
                        break;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the Id and Name of the TCR
        /// </summary>
        public string IdAndName 
        {
            get { return this.TCR.Number.ToString() + " - " + this.TCR.Name; } 
        }

        /// <summary>
        /// Gets a FlowDocument representing the HTML contents of this TCR
        /// </summary>
        public FlowDocument FlowDocumentContents
        {
            get
            {
                string sourceFlowDocument = @"
<FlowDocument FontFamily='Verdana' FontSize='12' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
    <Paragraph>
        <TextBlock FontWeight='Bold' FontSize='18'>
            TCR # {0} - {1}
        </TextBlock>
    </Paragraph>
    <Table >
        <Table.Columns>
            <TableColumn Width='120' />
            <TableColumn />
        </Table.Columns>
        <TableRowGroup>
            <TableRow>
                <TableCell>
                    <Paragraph>
                        <LineBreak />
                        <Run Text='Requirements:' FontWeight='Bold' />
                    </Paragraph>
                </TableCell>
                <TableCell>
                    {2}
                </TableCell>
            </TableRow>
            <TableRow>
                <TableCell>
                    <Paragraph>
                        <LineBreak />
                        <Run Text='Intent:' FontWeight='Bold' />
                    </Paragraph>
                </TableCell>
                <TableCell>
                    {3}
                </TableCell>
            </TableRow>
            <TableRow>
                <TableCell>
                    <Paragraph>
                        <LineBreak />
                        <Run Text='Remarks:' FontWeight='Bold' />
                    </Paragraph>
                </TableCell>
                <TableCell>
                    {4}
                </TableCell>
            </TableRow>
            <TableRow>
                <TableCell>
                    <Paragraph>
                        <LineBreak />
                        <Run Text='Exemptions:' FontWeight='Bold' />
                    </Paragraph>
                </TableCell>
                <TableCell>
                    {5}
                </TableCell>
            </TableRow>                      
        </TableRowGroup>
    </Table>
</FlowDocument>
";
                string result = string.Format(
                    sourceFlowDocument,
                    this.Id,
                    this.Name,
                    HtmlToXamlConvert.HtmlToXamlConverter.ConvertHtmlToXaml(this.Requirements, false),
                    HtmlToXamlConvert.HtmlToXamlConverter.ConvertHtmlToXaml(this.Intent, false),
                    HtmlToXamlConvert.HtmlToXamlConverter.ConvertHtmlToXaml(this.Remarks, false),
                    HtmlToXamlConvert.HtmlToXamlConverter.ConvertHtmlToXaml(this.Exemptions, false));

                return (FlowDocument)XamlReader.Parse(result);
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
