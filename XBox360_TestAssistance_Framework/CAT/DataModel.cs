// -----------------------------------------------------------------------
// <copyright file="DataModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    
    /// <summary>
    /// The main database class
    /// The constructor for DataModel loads all data from storage into memory.
    /// </summary>
    public class DataModel
    {
        /// <summary>
        /// Name of folder containing modules 
        /// </summary>
        private const string ModulePath = "Modules";

        /// <summary>
        /// Name of folder containing TCR platform and and TCR version data
        /// </summary>
        private const string DataPath = "Platforms";

        /// <summary>
        /// Name of file containing TCR and CRC content
        /// </summary>
        private const string TCRDataBase = "CertData.xml";

        /// <summary>
        /// Name of file that maps TCR/CTC to modules
        /// </summary>
        private const string ModuleDataBase = "ModuleData.xml";

        /// <summary>
        /// Root of the CAT database
        /// </summary>
        private List<CATPlatformData> catDatabase;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataModel" /> class.
        /// </summary>
        public DataModel()
        {
            this.LoadCATDatabase();
            this.GameTitle = null;
        }

        /// <summary>
        /// Gets or sets the current game title
        /// </summary>
        public XboxTitle GameTitle { get; set; }

        // public methods to deliver parsed data

        /// <summary>
        /// Gets the list of platforms
        /// </summary>
        /// <returns>List of platforms</returns>
        public List<Platform> GetPlatforms()
        {
            List<Platform> list = new List<Platform>();
            foreach (CATPlatformData pdata in this.catDatabase)
            {
                list.Add(new Platform(pdata.Platform));
            }

            return list;
        }

        /// <summary>
        /// Gets a list of TCR Versions for the specified platform
        /// </summary>
        /// <param name="p">The platform to get TCR Versions for</param>
        /// <returns>A list of TCR Versions</returns>
        public List<TCRVersion> GetTCRVersions(Platform p)
        {
            List<TCRVersion> list = new List<TCRVersion>();
            foreach (CATPlatformData pdata in this.catDatabase)
            {
                if (pdata.Platform == p.Name)
                {
                    foreach (CATVersionData vdata in pdata.Version)
                    {
                        list.Add(new TCRVersion(vdata.Name, p));
                    }

                    break;
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a list of TCR Categories for the specified TCR Version
        /// </summary>
        /// <param name="version">TCR Version to get categories for</param>
        /// <returns>A list of TCR Categories</returns>
        public List<TCRCategory> GetCategories(TCRVersion version)
        {
            List<TCRCategory> list = new List<TCRCategory>();
            DataTable tables = this.GetCertTables(version.Parent, version, "TCRDefinition");
            var query = (from table in tables.AsEnumerable() select new { cat = this.SafeGetField(table, "category") }).Distinct();
            foreach (var thecat in query)
            {
                list.Add(
                    new TCRCategory(
                        thecat.cat,
                        this.IsRequired(version.Name, thecat.cat, 0),
                        version));
            }

            return list;
        }

        /// <summary>
        /// Gets a list of TCRs for the specified TCR Category
        /// </summary>
        /// <param name="category">Category to get a list of TCR for</param>
        /// <returns>A list of TCRs</returns>
        public List<TCR> GetTcrs(TCRCategory category)
        {
            List<TCR> list = new List<TCR>();
            DataTable tables = this.GetCertTables(category.Parent.Parent, category.Parent, "TCRDefinition");
            string cat;
            int id;
            foreach (var table in tables.AsEnumerable())
            {
                cat = this.SafeGetField(table, "category");
                id = this.SafeToInt32(this.SafeGetField(table, "tcr_id"));
                if (category.Name == cat)
                {
                    list.Add(
                        new TCR(
                            id,
                            this.SafeGetField(table, "short_description"),
                            this.SafeGetField(table, "requirements"),
                            this.SafeGetField(table, "intent"),
                            this.SafeGetField(table, "remarks"),
                            this.SafeGetField(table, "exemptions"),
                            this.IsRequired(category.Parent.Name, cat, id),
                            category));
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a list of TCR Test Cases for the specified TCR
        /// </summary>
        /// <param name="tcr">TCR to get test cases for</param>
        /// <returns>A list of TCR Test Cases</returns>
        public List<TCRTestCase> GetTCRTestCases(TCR tcr)
        {
            List<TCRTestCase> list = new List<TCRTestCase>();
            DataTable tables = this.GetCertTables(tcr.Parent.Parent.Parent, tcr.Parent.Parent, "CTC");
            foreach (var table in tables.AsEnumerable())
            {
                if (tcr.Number == this.SafeToInt32(this.SafeGetField(table, "tcr_id")))
                {
                    list.Add(
                        new TCRTestCase(
                            this.SafeGetField(table, "testcase_id").Trim(),
                            this.SafeGetField(table, "short_description"),
                            this.SafeGetField(table, "Requirements"),
                            this.SafeGetField(table, "Tools"),
                            this.SafeGetField(table, "Configuration"),
                            this.SafeGetField(table, "Definition"),
                            this.SafeGetField(table, "Steps"),
                            this.SafeGetField(table, "Documentation"),
                            this.SafeGetField(table, "Result"),
                            this.SafeGetField(table, "PassExamples"),
                            this.SafeGetField(table, "FailExamples"),
                            this.SafeGetField(table, "NAExamples"),
                            this.SafeGetField(table, "Analysis"),
                            this.SafeGetField(table, "Faq"),
                            this.SafeGetField(table, "Hardware"),
                            tcr));
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a list of module for the specified TCR Test Case
        /// </summary>
        /// <param name="testCase">Test case to get a list of modules for</param>
        /// <returns>A list of CAT Modules</returns>
        public List<CATModuleInfo> GetModules(TCRTestCase testCase)
        {
            List<CATModuleInfo> list = new List<CATModuleInfo>();

            DataTable tables = this.GetModuleTables(testCase.Parent.Parent.Parent.Parent, testCase.Parent.Parent.Parent);
            foreach (var table in tables.AsEnumerable())
            {
                if (testCase.Parent.Number == this.SafeToInt32(this.SafeGetField(table, "tcr")))
                {
                    if (testCase.Id == this.SafeGetField(table, "testcase"))
                    {
                        list.Add(
                            new CATModuleInfo(
                                this.SafeGetField(table, "assembly"),
                                ModulePath,
                                this.SafeGetField(table, "short_description"),
                                this.SafeGetField(table, "detail"),
                                this.SafeGetField(table, "class"),
                                testCase));
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// private methods used to parse the database
        /// </summary>
        /// <param name="version">Version to check</param>
        /// <param name="category">Category to check</param>
        /// <param name="tcr">TCR to check</param>
        /// <returns>A value indicating whether or not the TCR is required</returns>
        private bool IsRequired(string version, string category, int tcr)
        {
            // require all TCRs when no title has been specified
            if (this.GameTitle == null)
            {
                return true;
            }

            // otherwise, require the TCR based on the title description
            return this.GameTitle.IsXboxRequirement(version, category, tcr);
        }

        /// <summary>
        /// Loads the CAT Database
        /// </summary>
        private void LoadCATDatabase()
        {
            string xmlPath;
            DirectoryInfo di = new DirectoryInfo("Platforms");
            DirectoryInfo[] pdi = di.GetDirectories();

            // Load all known data into local memory

            // todo: mark existing data for removal/garbage collection if loading fresh
            this.catDatabase = new List<CATPlatformData>();
            foreach (DirectoryInfo platdir in pdi)
            {
                CATPlatformData platdata = new CATPlatformData();
                platdata.Platform = platdir.Name;
                DirectoryInfo[] vdi = platdir.GetDirectories();
                foreach (DirectoryInfo verdir in vdi)
                {
                    // create the version object - it has a name and two xml file resources
                    CATVersionData verdata = new CATVersionData();

                    // get the version name from the directory name
                    verdata.Name = verdir.Name;

                    // load data
                    xmlPath = Path.Combine(DataPath, platdir.Name, verdir.Name, TCRDataBase);
                    if (File.Exists(xmlPath))
                    {
                        try
                        {
                            verdata.Data.ReadXml(xmlPath);
                        }
                        catch (Exception e)
                        {
                            verdata.Data = new DataSet("There was an error loading data from the source at " + xmlPath + ".\n" + e.Message);
                        }
                    }

                    // load modules
                    xmlPath = Path.Combine(DataPath, platdir.Name, verdir.Name, ModuleDataBase);
                    if (File.Exists(xmlPath))
                    {
                        try
                        {
                            verdata.Modules.ReadXml(xmlPath);
                        }
                        catch (Exception e)
                        {
                            verdata.Data = new DataSet("There was an error loading data from the source at " + xmlPath + ".\n" + e.Message);
                        }
                    }

                    // add the version, cert descriptions and module descriptions to this platform
                    platdata.Version.Add(verdata);
                }

                // add this platform to the database
                this.catDatabase.Add(platdata);
            }
        }

        /// <summary>
        /// Get Cert Tables
        /// </summary>
        /// <param name="p">Platform to query</param>
        /// <param name="v">TCR Version to query</param>
        /// <param name="tableName">The name of the table to get</param>
        /// <returns>A cert table</returns>
        private DataTable GetCertTables(Platform p, TCRVersion v, string tableName)
        {
            foreach (CATPlatformData pdata in this.catDatabase)
            {
                if (pdata.Platform == p.Name)
                {
                    foreach (CATVersionData vdata in pdata.Version)
                    {
                        if (vdata.Name == v.Name)
                        {
                            if (vdata.Data.Tables.Contains(tableName))
                            {
                                return vdata.Data.Tables[tableName];
                            }
                        }
                    }
                }
            }

            return new DataTable(); // empty data table
        }

        /// <summary>
        /// Get Module Tables
        /// </summary>
        /// <param name="p">The platform to query</param>
        /// <param name="v">The TCR Version</param>
        /// <returns>Module tables</returns>
        private DataTable GetModuleTables(Platform p, TCRVersion v)
        {
            foreach (CATPlatformData pdata in this.catDatabase)
            {
                if (pdata.Platform == p.Name)
                {
                    foreach (CATVersionData vdata in pdata.Version)
                    {
                        if (vdata.Name == v.Name)
                        {
                            if (vdata.Modules.Tables.Contains("Module"))
                            {
                                return vdata.Modules.Tables["Module"];
                            }
                        }
                    }
                }
            }

            return new DataTable(); // empty data table
        }

        /// <summary>
        /// Get a field safely
        /// </summary>
        /// <param name="row">Row to get</param>
        /// <param name="columnName">Column to get</param>
        /// <returns>The string value of the field</returns>
        private string SafeGetField(DataRow row, string columnName)
        {
            string value = string.Empty;
            try
            {
                if (row.Table.Columns.Contains(columnName))
                {
                    value = row.Field<string>(columnName);

                    if (value == null)
                    {
                        value = "none";
                    }
                    else
                    {
                        value = value.Trim();
                    }
                }
            }
            catch (Exception e)
            {
                value = e.Message;
            }

            return value;
        }

        /// <summary>
        /// Get an integer safety
        /// </summary>
        /// <param name="number">String containing a number to convert to an integer</param>
        /// <returns>Integer converted from specified string</returns>
        private int SafeToInt32(string number)
        {
            int i = 0;
            int.TryParse(number, out i);
            return i;
        }

        /// <summary>
        /// public structures describing cert data items: platform, version, category, TCR, test case, test module binary
        /// </summary>
        public class Platform
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Platform" /> class.
            /// </summary>
            /// <param name="name">The name of this platform</param>
            public Platform(string name)
            {
                this.Name = name;
            }
            
            /// <summary>
            /// Gets or sets the name of this platform
            /// </summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// TCR Version
        /// </summary>
        public class TCRVersion
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TCRVersion" /> class.
            /// </summary>
            /// <param name="name">Name of this TCR version</param>
            /// <param name="platform">Platform associated with this TCR version</param>
            public TCRVersion(string name, Platform platform)
            {
                this.Name = name;
                this.Parent = platform;
            }

            /// <summary>
            /// Gets or sets the name of this TCR version
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the platform of this TCR version
            /// </summary>
            public Platform Parent { get; set; }
        }

        /// <summary>
        /// TCR Category
        /// </summary>
        public class TCRCategory
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TCRCategory" /> class.
            /// </summary>
            /// <param name="name">Name of this TCR Category</param>
            /// <param name="required">Whether or not this TCR Category matches the requirement filter</param>
            /// <param name="version">The TCR Version associated with this category</param>
            public TCRCategory(string name, bool required, TCRVersion version)
            {
                this.Name = name;
                this.IsRequired = required;
                this.Parent = version;
            }

            /// <summary>
            /// Gets or sets the name of this Category
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not this TCR category matches the requirement filter
            /// </summary>
            public bool IsRequired { get; set; }

            /// <summary>
            /// Gets or sets the TCR Version associated with this category
            /// </summary>
            public TCRVersion Parent { get; set; }
        }

        /// <summary>
        /// TCR class
        /// </summary>
        public class TCR
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TCR" /> class.
            /// </summary>
            /// <param name="number">TCR number field</param>
            /// <param name="name">TCR name field</param>
            /// <param name="requirements">TCR requirements field</param>
            /// <param name="intent">TCR intent field</param>
            /// <param name="remarks">TCR remarks field</param>
            /// <param name="exemptions">TCR exemptions field</param>
            /// <param name="isRequired">Whether or not this TCR matches the requirements filter</param>
            /// <param name="category">The TCR Category associated with this TCR</param>
            public TCR(
                int number, 
                string name, 
                string requirements,
                string intent, 
                string remarks,
                string exemptions, 
                bool isRequired,
                TCRCategory category)
            {
                this.Number = number;
                this.Name = name;
                this.Requirements = requirements;
                this.Intent = intent;
                this.Remarks = remarks;
                this.Exemptions = exemptions;
                this.IsRequired = isRequired;
                this.Parent = category;
            }

            /// <summary>
            /// Gets or sets the TCR number
            /// </summary>
            public int Number { get; set; }

            /// <summary>
            /// Gets or sets the name of the TCR
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the requirements field of the TCR
            /// </summary>
            public string Requirements { get; set; }

            /// <summary>
            /// Gets or sets the intents field of the TCR
            /// </summary>
            public string Intent { get; set; }

            /// <summary>
            /// Gets or sets the Remarks field of the TCR
            /// </summary>
            public string Remarks { get; set; }

            /// <summary>
            /// Gets or sets the Exemptions field of this TCR
            /// </summary>
            public string Exemptions { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not this TCR matches the requirements filter
            /// </summary>
            public bool IsRequired { get; set; }

            /// <summary>
            /// Gets or sets the TCR Category associated with this TCR
            /// </summary>
            public TCRCategory Parent { get; set; }
        }

        /// <summary>
        /// TCR Test Case class
        /// </summary>
        public class TCRTestCase
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TCRTestCase" /> class.
            /// </summary>
            /// <param name="id">TCR Test Case Id</param>
            /// <param name="name">TCR Test Case Name</param>
            /// <param name="requirements">TCR Test Case Requirements field</param>
            /// <param name="tools">TCR Test Case Tools</param>
            /// <param name="config">TCR Test Case Config field</param>
            /// <param name="definition">TCR Test Case Definition field</param>
            /// <param name="steps">TCR Test Case Steps field</param>
            /// <param name="documentation">TCR Test Case Documentation field</param>
            /// <param name="result">TCR Test Case Result field</param>
            /// <param name="passExamples">TCR Test Case Pass Examples field</param>
            /// <param name="failExamples">TCR Test Case Fail Examples field</param>
            /// <param name="notApplicableExamples">TCR Test Case N/A Examples field</param>
            /// <param name="analysis">TCR Test Case Analysis field</param>
            /// <param name="faq">TCR Test Case FAQ field</param>
            /// <param name="hardware">TCR Test Case Hardware field</param>
            /// <param name="tcr">The TCR associated with this TCR Test Case</param>
            public TCRTestCase(
                string id,
                string name, 
                string requirements,
                string tools, 
                string config,
                string definition, 
                string steps,
                string documentation,
                string result, 
                string passExamples,
                string failExamples,
                string notApplicableExamples,
                string analysis,
                string faq, 
                string hardware, 
                TCR tcr)
            {
                this.Id = id;
                this.Name = name;
                this.Requirements = requirements;
                this.Tools = tools;
                this.Configuration = config;
                this.Definition = definition;
                this.Steps = steps;
                this.Documentation = documentation;
                this.Result = result;
                this.PassExamples = passExamples;
                this.FailExamples = failExamples;
                this.NaExamples = notApplicableExamples;
                this.Analysis = analysis;
                this.Faq = faq;
                this.Hardware = hardware;
                this.Parent = tcr;
            }

            /// <summary>
            /// Gets or sets the Id of the TCR Test Case
            /// </summary>
            public string Id { get; set; }

            /// <summary>
            /// Gets or sets the name of the TCR Test Case
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the Requirements field of the TCR test case
            /// </summary>
            public string Requirements { get; set; }

            /// <summary>
            /// Gets or sets the Tools field of the TCR Test Case
            /// </summary>
            public string Tools { get; set; }

            /// <summary>
            /// Gets or sets the Configuration field of the TCR test case
            /// </summary>
            public string Configuration { get; set; }

            /// <summary>
            /// Gets or sets the Definitions field of the TCR test case
            /// </summary>
            public string Definition { get; set; }

            /// <summary>
            /// Gets or sets the Steps field of the TCR test case
            /// </summary>
            public string Steps { get; set; }
            
            /// <summary>
            /// Gets or sets the Documentation field of the TCR test case
            /// </summary>
            public string Documentation { get; set; }

            /// <summary>
            /// Gets or sets the Result field of the TCR Test Case
            /// </summary>
            public string Result { get; set; }

            /// <summary>
            /// Gets or sets the Pass Examples field of the TCR Test Case
            /// </summary>
            public string PassExamples { get; set; }

            /// <summary>
            /// Gets or sets the Fail Example field of the TCR test case
            /// </summary>
            public string FailExamples { get; set; }

            /// <summary>
            /// Gets or sets the N/A Examples field of the TCR Test Case
            /// </summary>
            public string NaExamples { get; set; }

            /// <summary>
            /// Gets or sets the Analysis field of the TCR Test case
            /// </summary>
            public string Analysis { get; set; }

            /// <summary>
            /// Gets or sets the FAQ field of the TCR Test Case
            /// </summary>
            public string Faq { get; set; }

            /// <summary>
            /// Gets or sets the Hardware field of the TCR Test Case
            /// </summary>
            public string Hardware { get; set; }

            /// <summary>
            /// Gets or sets the TCR associated with this TCR Test Case
            /// </summary>
            public TCR Parent { get; set; }
        }

        /// <summary>
        /// CAT Module Info
        /// </summary>
        public class CATModuleInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CATModuleInfo" /> class.
            /// </summary>
            /// <param name="name">Name of the CAT module</param>
            /// <param name="filePath">File path to the CAT module</param>
            /// <param name="description">Description of the CAT module</param>
            /// <param name="detail">Detail of the CAT module</param>
            /// <param name="className">ClassName of that CAT module</param>
            /// <param name="testCase">Test Case associated with this CAT module</param>
            public CATModuleInfo(string name, string filePath, string description, string detail, string className, TCRTestCase testCase)
            {
                this.Name = name;
                this.FilePath = filePath;
                this.Description = description;
                this.Detail = detail;
                this.ClassName = className;

                string modfile = Path.GetFullPath(Path.Combine(filePath, name));
                if (File.Exists(modfile))
                {
                    this.Assembly = Assembly.LoadFile(modfile);
                }
                else
                {
                    this.Assembly = null;
                }

                this.Parent = testCase;
            }

            /// <summary>
            /// Gets or sets the name of the CAT Module
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the file path to the CAT Module
            /// </summary>
            public string FilePath { get; set; }

            /// <summary>
            /// Gets or sets the description of the CAT Module
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Gets or sets the detail for the CAT Module
            /// </summary>
            public string Detail { get; set; }

            /// <summary>
            /// Gets or sets the ClassName of the CAT Module
            /// </summary>
            public string ClassName { get; set; }

            /// <summary>
            /// Gets or sets the CAT Module assembly
            /// </summary>
            public Assembly Assembly { get; set; }

            /// <summary>
            /// Gets or sets the TCR Test Case associated with this TCR
            /// </summary>
            public TCRTestCase Parent { get; set; }
        }

        /// <summary>
        /// public structures describing target data items: title-under-test, xbox device, gamer profile
        /// </summary>
        public class ITitleFilter
        {
        }

        /// <summary>
        /// Xbox Title Filter
        /// </summary>
        public class XboxTitleFilter : ITitleFilter
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="XboxTitleFilter" /> class.
            /// Sets most common properties as default. or most inclusive. or most exclusive. TBD
            /// </summary>
            public XboxTitleFilter()
            {
                // include most common TCRs
                this.IsArcade = true;
                this.IsMultiplayer = true;
                this.UsesKinect = true;
                this.HasDlc = true;
                this.HasInGameDlc = true;
                this.HasStoredGameContentFiles = true;
                this.UsesOnlineLiveStorage = true;
                this.HasPlayerContent = true;
                this.HasPlayerContentTransmission = true;
                this.HasAchievements = true;
                this.HasAvatarAwards = true;
                this.SendsLiveVideo = true;
                this.IsNoCostGame = false;
                this.IsStandAloneDemo = false;
                this.UsesVirtualKeyboard = true;
                this.UsesPlayReadyLicense = true;
                this.SupportsStereoscopic3d = true;
                this.UsesSystemLink = true;
                this.IsMultiplayerBySystemLinkOnly = false;
                this.UsesNetwork = true;
                this.UsesInternetService = true;
                this.UsesLiveService = true;
                this.UsesDataCenterHostedGamePlay = true;
                this.SharesWithSocialNetworks = true;
                this.AllowsPlayerCommunication = true;
                this.AllowsPlayerTextCommunication = true;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="XboxTitleFilter" /> class.
            /// </summary>
            /// <param name="toCopy">XboxTitleFilter to copy</param>
            public XboxTitleFilter(XboxTitleFilter toCopy)
            {
                // include most common TCRs
                this.IsArcade = toCopy.IsArcade;
                this.IsMultiplayer = toCopy.IsMultiplayer;
                this.UsesKinect = toCopy.UsesKinect;
                this.HasDlc = toCopy.HasDlc;
                this.HasInGameDlc = toCopy.HasInGameDlc; // same as hasDlc???
                this.HasStoredGameContentFiles = toCopy.HasStoredGameContentFiles;
                this.UsesOnlineLiveStorage = toCopy.UsesOnlineLiveStorage;
                this.HasPlayerContent = toCopy.HasPlayerContent;
                this.HasPlayerContentTransmission = toCopy.HasPlayerContentTransmission;
                this.HasAchievements = toCopy.HasAchievements;
                this.HasAvatarAwards = toCopy.HasAvatarAwards;
                this.SendsLiveVideo = toCopy.SendsLiveVideo;
                this.IsNoCostGame = toCopy.IsNoCostGame;
                this.IsStandAloneDemo = toCopy.IsStandAloneDemo;
                this.UsesVirtualKeyboard = toCopy.UsesVirtualKeyboard;
                this.UsesPlayReadyLicense = toCopy.UsesPlayReadyLicense;
                this.SupportsStereoscopic3d = toCopy.SupportsStereoscopic3d;
                this.UsesSystemLink = toCopy.UsesSystemLink;
                this.IsMultiplayerBySystemLinkOnly = toCopy.IsMultiplayerBySystemLinkOnly;
                this.UsesNetwork = toCopy.UsesNetwork;
                this.UsesInternetService = toCopy.UsesInternetService;
                this.UsesLiveService = toCopy.UsesLiveService;
                this.UsesDataCenterHostedGamePlay = toCopy.UsesDataCenterHostedGamePlay;
                this.SharesWithSocialNetworks = toCopy.SharesWithSocialNetworks;
                this.AllowsPlayerCommunication = toCopy.AllowsPlayerCommunication;
                this.AllowsPlayerTextCommunication = toCopy.AllowsPlayerTextCommunication;
            }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested is an arcade title
            /// includes TCR Category XLA
            /// </summary>
            public bool IsArcade { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested support multiple players
            /// includes TCR Category MPS, includes TCR 89, (turns on usesNetwork and allowsPlayerCommunication)
            /// </summary>
            public bool IsMultiplayer { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested uses Kinect
            /// includes TCR Category NUI
            /// includes TCR 70, TCR 96
            /// </summary>
            public bool UsesKinect { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested has DLC
            /// includes TCR 60, TCR 59
            /// </summary>
            public bool HasDlc { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested has In-Game DLC
            /// includes TCR 132
            /// </summary>
            public bool HasInGameDlc { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested has stored game content files
            /// includes Category STR
            /// </summary>
            public bool HasStoredGameContentFiles { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested uses online live storage
            /// includes TCR 63, TCR 62, TCR 55
            /// </summary>
            public bool UsesOnlineLiveStorage { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested has player content
            /// includes TCR 64, TCR 61, TCR 168
            /// </summary>
            public bool HasPlayerContent { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether has player content transmission
            /// includes TCR 65
            /// </summary>
            public bool HasPlayerContentTransmission { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested has achievements
            /// includes TCR 73, TCR 69
            /// </summary>
            public bool HasAchievements { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested has avatar awards
            /// includes TCR 143, TCR 142
            /// </summary>
            public bool HasAvatarAwards { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested sends live video
            /// includes TCR 125
            /// </summary>
            public bool SendsLiveVideo { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested is a no-cost game
            /// excludes TCR 126, TCR 127. includes TCR Category DEM
            /// </summary>
            public bool IsNoCostGame { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested is a standalone demo
            /// excludes TCR 110, TCR 111 (of DEM category)
            /// </summary>
            public bool IsStandAloneDemo { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested uses virtual keyboard
            /// includes TCR 43
            /// </summary>
            public bool UsesVirtualKeyboard { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested uses a PlayReady license
            /// includes TCR 185
            /// </summary>
            public bool UsesPlayReadyLicense { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested supports stereoscopic 3D
            /// includes TCR 167
            /// </summary>
            public bool SupportsStereoscopic3d { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested uses system link
            /// includes Category SL
            /// </summary>
            public bool UsesSystemLink { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested is multiplayer only using system link
            /// excludes TCR 71, excludes TCR 86 
            /// </summary>
            public bool IsMultiplayerBySystemLinkOnly { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested uses the network
            /// includes TCR 95
            /// </summary>
            public bool UsesNetwork { get; set; }

            // non-local network specific TCRs
            
            /// <summary>
            /// Gets or sets a value indicating whether the title being tested uses the internet
            /// (except Xbox service). includes TCR 99
            /// </summary>
            public bool UsesInternetService { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested uses live service
            /// includes TCR 98
            /// </summary>
            public bool UsesLiveService { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested uses data center hosted gameplay
            /// includes TCR 97
            /// </summary>
            public bool UsesDataCenterHostedGamePlay { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested shared with social networks
            /// (except Xbox service). includes TCR 188, TCR 169 (turns on hasPlayerContentTransmission and allowsPlayerCommunication)
            /// </summary>
            public bool SharesWithSocialNetworks { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested allows player communication
            /// includes TCR 94, TCR 93, TCR 91, TCR 90, TCR 54
            /// </summary>
            public bool AllowsPlayerCommunication { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the title being tested allows player text communication
            /// (except chat). includes TCR 92
            /// </summary>
            public bool AllowsPlayerTextCommunication { get; set; }
        }

        /// <summary>
        /// XboxTitle represents a game title
        /// </summary>
        public class XboxTitle : IXboxTitle
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="XboxTitle" /> class.
            /// </summary>
            public XboxTitle()
            {
                this.Name = string.Empty;
                this.SymbolsPath = string.Empty;
                this.GameConfigPath = string.Empty;
                this.ContentPackage = string.Empty;
                this.XdkRecoveryPath = string.Empty;
                this.GameInstallType = string.Empty;
                this.GameDirectory = string.Empty;
                this.RawGameDirectory = string.Empty;
                this.DiscImage = string.Empty;
                this.TitleUpdatePath = string.Empty;
                this.Filter = new XboxTitleFilter();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="XboxTitle" /> class.
            /// </summary>
            /// <param name="copyFrom">XboxTitle to copy</param>
            public XboxTitle(XboxTitle copyFrom)
            {
                this.Name = copyFrom.Name;
                this.SymbolsPath = copyFrom.SymbolsPath;
                this.GameConfigPath = copyFrom.GameConfigPath;
                this.UseDemo = copyFrom.UseDemo;
                this.DemoContentPackage = copyFrom.DemoContentPackage;
                this.ContentPackage = copyFrom.ContentPackage;
                this.XdkRecoveryPath = copyFrom.XdkRecoveryPath;
                this.GameInstallType = copyFrom.GameInstallType;
                this.GameDirectory = copyFrom.GameDirectory;
                this.RawGameDirectory = copyFrom.RawGameDirectory;
                this.DiscImage = copyFrom.DiscImage;
                this.TitleUpdatePath = copyFrom.TitleUpdatePath;
                this.Filter = new XboxTitleFilter(copyFrom.Filter);
            }

            /// <summary>
            /// Gets or sets name of the game title
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the game install type of this type.  "Content Package", "Disc Emulation", or "Raw"
            /// </summary>
            public string GameInstallType { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not to use the demo when launching a content package based title
            /// </summary>
            public bool UseDemo { get; set; }

            /// <summary>
            /// Gets or sets the path to symbol for this game title
            /// </summary>
            public string SymbolsPath { get; set; }

            /// <summary>
            /// Gets or sets the path to the game config file for this game title
            /// </summary>
            public string GameConfigPath { get; set; }

            /// <summary>
            /// Gets or sets the path to the content package for this game title, if content package based
            /// </summary>
            public string ContentPackage { get; set; }

            /// <summary>
            /// Gets or sets the path to the demo content package for this game title, if content package based
            /// </summary>
            public string DemoContentPackage { get; set; }

            /// <summary>
            /// Gets or sets the path to the XDK Recovery executable
            /// </summary>
            public string XdkRecoveryPath { get; set; }

            /// <summary>
            /// Gets or sets the path to the root game directory for this game title
            /// </summary>
            public string GameDirectory { get; set; }

            /// <summary>
            /// Gets or sets the path to the RAW game directory for this game title, if game install type is "Raw"
            /// </summary>
            public string RawGameDirectory { get; set; }

            /// <summary>
            /// Gets or sets the path to the disc image for this game title, if game install type is "Disc Emulation"
            /// </summary>
            public string DiscImage { get; set; }

            /// <summary>
            /// Gets or sets the path to a title update for this game title, if any is present
            /// </summary>
            public string TitleUpdatePath { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether or not to filter TCRs and TCR Categories to match this game title.  TBD
            /// </summary>
            /// <returns></returns>
            public bool IsFiltered { get; set; }

            /// <summary>
            /// Gets or sets the filter to use to match TCRs and TCR Categories.  TBD
            /// </summary>
            public XboxTitleFilter Filter { get; set; }

            /// <summary>
            /// Checks if a TCR, TCR Category and TCR Version match the requirements of this title.
            /// </summary>
            /// <param name="version">Version to check</param>
            /// <param name="category">Category to check</param>
            /// <param name="tcr">TCR to check</param>
            /// <returns>True it matching, false otherwise</returns>
            public bool IsXboxRequirement(string version, string category, int tcr)
            {
                // filter versions we know about
                if (version.Contains("3.0a"))
                {
                    return this.IsXbox30aRequirement(category, tcr);
                }
   
                // for all other versions, assume all TCRs are requirements
                return true;
            }

            /// <summary>
            /// Copies an instance of the <see cref="XboxTitle" /> class.
            /// </summary>
            /// <param name="copyFrom">XboxTitle to copy</param>
            public void Copy(XboxTitle copyFrom)
            {
                this.Name = copyFrom.Name;
                this.SymbolsPath = copyFrom.SymbolsPath;
                this.GameConfigPath = copyFrom.GameConfigPath;
                this.UseDemo = copyFrom.UseDemo;
                this.DemoContentPackage = copyFrom.DemoContentPackage;
                this.ContentPackage = copyFrom.ContentPackage;
                this.XdkRecoveryPath = copyFrom.XdkRecoveryPath;
                this.GameInstallType = copyFrom.GameInstallType;
                this.GameDirectory = copyFrom.GameDirectory;
                this.RawGameDirectory = copyFrom.RawGameDirectory;
                this.DiscImage = copyFrom.DiscImage;
                this.TitleUpdatePath = copyFrom.TitleUpdatePath;
                this.Filter = new XboxTitleFilter(copyFrom.Filter);
            }

            /// <summary>
            /// Checks if a TCR, and TCR Category match the requirements of this title.
            /// </summary>
            /// <param name="category">Category to check</param>
            /// <param name="tcr">TCR to check</param>
            /// <returns>True it matching, false otherwise</returns>
            public bool IsXbox30aRequirement(string category, int tcr)
            {
                // show all requirements if no filter has been applied
                if (this.Filter == null)
                {
                    return true;
                }

                // first filter by category
                if (category.Contains("DEM"))
                {
                    if (this.Filter.IsNoCostGame)
                    {
                        return true;
                    }
                }
                else if (category.Contains("MPS"))
                {
                    if (this.Filter.IsMultiplayer)
                    {
                        return true;
                    }
                }
                else if (category.Contains("NUI"))
                {
                    if (this.Filter.UsesKinect)
                    {
                        return true;
                    }
                }
                else if (category.Contains("SL"))
                {
                    if (this.Filter.UsesSystemLink)
                    {
                        return true;
                    }
                }
                else if (category.Contains("STR"))
                {
                    if (this.Filter.HasStoredGameContentFiles)
                    {
                        return true;
                    }
                }
                else if (category.Contains("XLA"))
                {
                    if (this.Filter.IsArcade)
                    {
                        return true;
                    }
                }

                // filter by individual TCR
                switch (tcr)
                {
                    case 89:
                        return this.Filter.IsMultiplayer;
                    case 70:
                        return this.Filter.IsMultiplayer;
                    case 96:
                        return this.Filter.IsMultiplayer;
                    case 60:
                        return this.Filter.HasDlc;
                    case 59:
                        return this.Filter.HasDlc;
                    case 132:
                        return this.Filter.HasInGameDlc;
                    case 63:
                        return this.Filter.UsesOnlineLiveStorage;
                    case 62:
                        return this.Filter.UsesOnlineLiveStorage;
                    case 55:
                        return this.Filter.UsesOnlineLiveStorage;
                    case 64:
                        return this.Filter.HasPlayerContent;
                    case 61:
                        return this.Filter.HasPlayerContent;
                    case 65:
                        return this.Filter.HasPlayerContentTransmission;
                    case 73:
                        return this.Filter.HasAchievements;
                    case 69:
                        return this.Filter.HasAchievements;
                    case 143:
                        return this.Filter.HasAvatarAwards;
                    case 142:
                        return this.Filter.HasAvatarAwards;
                    case 125:
                        return this.Filter.SendsLiveVideo;
                    case 43:
                        return this.Filter.UsesVirtualKeyboard;
                    case 185:
                        return this.Filter.UsesPlayReadyLicense;
                    case 167:
                        return this.Filter.SupportsStereoscopic3d;
                    case 95:
                        return this.Filter.UsesNetwork;
                    case 71:
                        return this.Filter.UsesNetwork || !this.Filter.IsMultiplayerBySystemLinkOnly;
                    case 99:
                        return this.Filter.UsesInternetService;
                    case 98:
                        return this.Filter.UsesLiveService;
                    case 97:
                        return this.Filter.UsesDataCenterHostedGamePlay;
                    case 188:
                        return this.Filter.SharesWithSocialNetworks;
                    case 169:
                        return this.Filter.SharesWithSocialNetworks;
                    case 94:
                        return this.Filter.AllowsPlayerCommunication;
                    case 93:
                        return this.Filter.AllowsPlayerCommunication;
                    case 91:
                        return this.Filter.AllowsPlayerCommunication;
                    case 90:
                        return this.Filter.AllowsPlayerCommunication;
                    case 54:
                        return this.Filter.AllowsPlayerCommunication;
                    case 92:
                        return this.Filter.AllowsPlayerTextCommunication;
                    
                    // category-wide -- Game Demos -- DEM
                    case 110:
                        return this.Filter.IsNoCostGame && !this.Filter.IsStandAloneDemo;
                    case 111:
                        return this.Filter.IsNoCostGame && !this.Filter.IsStandAloneDemo;
                    case 144:
                        return this.Filter.IsNoCostGame;
                    
                    // Multiplayer Sessions -- MPS
                    case 86:
                        return this.Filter.IsMultiplayer && this.Filter.IsMultiplayerBySystemLinkOnly;
                    case 82:
                    case 84:
                    case 87:
                    case 88:
                    case 115:
                    case 124:
                    case 138:
                    case 139:
                    case 140:
                    case 141:
                        return this.Filter.IsMultiplayer;

                    // Natural User Input -- NUI
                    case 146:
                    case 147:
                    case 148:
                    case 149:
                    case 150:
                    case 151:
                    case 152:
                    case 153:
                    case 154:
                    case 155:
                    case 156:
                    case 158:
                    case 160:
                    case 161:
                    case 164:
                    case 165:
                    case 166:
                        return this.Filter.UsesKinect;

                    // System Link -- SL
                    case 102:
                        return this.Filter.UsesSystemLink;
                    
                    // Storage -- STR
                    case 168:
                        return this.Filter.HasStoredGameContentFiles && this.Filter.HasPlayerContent;
                    case 45:
                    case 47:
                    case 49:
                    case 50:
                    case 51:
                    case 118:
                        return this.Filter.HasStoredGameContentFiles;

                    // Xbox LIVE Arcade -- XLA
                    case 126:
                        return this.Filter.IsArcade && this.Filter.IsNoCostGame;
                    case 127:
                        return this.Filter.IsArcade && this.Filter.IsNoCostGame;
                    case 130:
                        return this.Filter.IsArcade;
                }

                // show the requirement if we get here, we didn't know how to filter it
                return true; 
            }
        }

        /// <summary>
        /// CAT Platform data
        /// </summary>
        private class CATPlatformData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CATPlatformData" /> class.
            /// </summary>
            public CATPlatformData()
            {
                this.Version = new List<CATVersionData>();
            }

            /// <summary>
            /// Gets or sets the platform
            /// </summary>
            public string Platform { get; set; }

            /// <summary>
            /// Gets or sets CAT Version Data
            /// </summary>
            public List<CATVersionData> Version { get; set; }
        }

        /// <summary>
        /// CAT Version data
        /// </summary>
        private class CATVersionData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CATVersionData" /> class.
            /// </summary>
            public CATVersionData()
            {
                this.Data = new DataSet();
                this.Modules = new DataSet();
            }

            /// <summary>
            /// Gets or sets the name of the CAT Version Data
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets Data associated with this Version
            /// </summary>
            public DataSet Data { get; set; }

            /// <summary>
            /// Gets or sets Modules associated with this Version
            /// </summary>
            public DataSet Modules { get; set; }
        }
    }
}
