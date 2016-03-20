// -----------------------------------------------------------------------
// <copyright file="HtmlSchema.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace HtmlToXamlConvert
{
    using System.Collections;
    using System.Diagnostics;

    // This code was taken from MSDN as an example of converting HTML to XAML.
    //
    // I've combined all the classes together and made some spelling corrections.
    //
    // Copyright (C) Microsoft Corporation.  All rights reserved.
    //
    // Description: Prototype for XAML - Html conversion

    /// <summary>
    /// HtmlSchema class
    /// maintains static information about HTML structure
    /// can be used by HtmlParser to check conditions under which an element starts or ends, etc.
    /// </summary>
    internal class HtmlSchema
    {
        // ---------------------------------------------------------------------
        //
        // Private Fields
        //
        // ---------------------------------------------------------------------
        #region Private Fields

        // html element names
        // this is an array list now, but we may want to make it a hash table later for better performance

        /// <summary>
        /// inline elements
        /// </summary>
        private static ArrayList htmlInlineElements;

        /// <summary>
        /// block elements
        /// </summary>
        private static ArrayList htmlBlockElements;

        /// <summary>
        /// Open elements that can be opened
        /// </summary>
        private static ArrayList htmlOtherOpenableElements;

        /// <summary>
        /// list of html empty element names
        /// </summary>
        private static ArrayList htmlEmptyElements;

        /// <summary>
        /// names of html elements for which closing tags are optional, and close when the outer nested element closes
        /// </summary>
        private static ArrayList htmlElementsClosingOnParentElementEnd;

        // names of elements that close certain optional closing tag elements when they start

        /// <summary>
        /// names of elements closing the COLGROUP element
        /// </summary>
        private static ArrayList htmlElementsClosingColgroup;

        /// <summary>
        /// names of elements closing the DD element
        /// </summary>
        private static ArrayList htmlElementsClosingDD;

        /// <summary>
        /// names of elements closing the DT element
        /// </summary>
        private static ArrayList htmlElementsClosingDT;

        /// <summary>
        /// names of elements closing the LI element
        /// </summary>
        private static ArrayList htmlElementsClosingLI;

        /// <summary>
        /// names of elements closing the TBODY element
        /// </summary>
        private static ArrayList htmlElementsClosingTbody;

        /// <summary>
        /// names of elements closing the TD element
        /// </summary>
        private static ArrayList htmlElementsClosingTD;

        /// <summary>
        /// names of elements closing the TFOOT element
        /// </summary>
        private static ArrayList htmlElementsClosingTfoot;

        /// <summary>
        /// names of elements closing the THEAD element
        /// </summary>
        private static ArrayList htmlElementsClosingThead;

        /// <summary>
        /// names of elements closing the TH element
        /// </summary>
        private static ArrayList htmlElementsClosingTH;

        /// <summary>
        /// names of elements closing the TR element
        /// </summary>
        private static ArrayList htmlElementsClosingTR;

        /// <summary>
        /// html character entities hash table
        /// </summary>
        private static Hashtable htmlCharacterEntities;

        #endregion Private Fields

        // ---------------------------------------------------------------------
        //
        // Constructors
        //
        // ---------------------------------------------------------------------
        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="HtmlSchema" /> class.
        /// static constructor, initializes the ArrayLists
        /// that hold the elements in various sub-components of the schema
        /// e.g htmlEmptyElements, etc.
        /// </summary>
        static HtmlSchema()
        {
            // initializes the list of all html elements
            InitializeInlineElements();

            InitializeBlockElements();

            InitializeOtherOpenableElements();

            // initialize empty elements list
            InitializeEmptyElements();

            // initialize list of elements closing on the outer element end
            InitializeElementsClosingOnParentElementEnd();

            // initalize list of elements that close when a new element starts
            InitializeElementsClosingOnNewElementStart();

            // Initialize character entities
            InitializeHtmlCharacterEntities();
        }

        #endregion Constructors;

        // ---------------------------------------------------------------------
        //
        // Internal Methods
        //
        // ---------------------------------------------------------------------
        #region Internal Methods

        /// <summary>
        /// returns true when xmlElementName corresponds to empty element
        /// </summary>
        /// <param name="xmlElementName">
        /// string representing name to test
        /// </param>
        /// <returns>True if the xml element name is an empty element, false otherwise</returns>
        internal static bool IsEmptyElement(string xmlElementName)
        {
            // convert to lowercase before we check
            // because element names are not case sensitive
            return htmlEmptyElements.Contains(xmlElementName.ToLower());
        }

        /// <summary>
        /// returns true if xmlElementName represents a block formatting element.
        /// It used in an algorithm of transferring inline elements over block elements
        /// in HtmlParser
        /// </summary>
        /// <param name="xmlElementName">XML element name</param>
        /// <returns>true if the xml element name is a block, false otherwise</returns>
        internal static bool IsBlockElement(string xmlElementName)
        {
            return htmlBlockElements.Contains(xmlElementName);
        }

        /// <summary>
        /// returns true if the xmlElementName represents an inline formatting element
        /// </summary>
        /// <param name="xmlElementName">XML element name</param>
        /// <returns>true if the xml element name is an inline element, false otherwise</returns>
        internal static bool IsInlineElement(string xmlElementName)
        {
            return htmlInlineElements.Contains(xmlElementName);
        }

        /// <summary>
        /// It is a list of known html elements which we
        /// want to allow to produce HTML parser,
        /// but don't want to act as inline, block or no-scope.
        /// Presence in this list will allow to open
        /// elements during html parsing, and adding the
        /// to a tree produced by html parser.
        /// </summary>
        /// <param name="xmlElementName">XML element name</param>
        /// <returns>true if the xml element name is another element that can be opened, false otherwise</returns>
        internal static bool IsKnownOpenableElement(string xmlElementName)
        {
            return htmlOtherOpenableElements.Contains(xmlElementName);
        }

        /// <summary>
        /// returns true when xmlElementName closes when the outer element closes
        /// this is true of elements with optional start tags
        /// </summary>
        /// <param name="xmlElementName">
        /// string representing name to test
        /// </param>
        /// <returns>true if the xml element closes on parent element end, false otherwise</returns>
        internal static bool ClosesOnParentElementEnd(string xmlElementName)
        {
            // convert to lowercase when testing
            return htmlElementsClosingOnParentElementEnd.Contains(xmlElementName.ToLower());
        }

        /// <summary>
        /// returns true if the current element closes when the new element, whose name has just been read, starts
        /// </summary>
        /// <param name="currentElementName">string representing current element name</param>
        /// <param name="nextElementName">string representing name of the next element that will start</param>
        /// <returns>true if the current element closes when the new element, whose name has just been read, starts</returns>
        internal static bool ClosesOnNextElementStart(string currentElementName, string nextElementName)
        {
            Debug.Assert(currentElementName == currentElementName.ToLower(), "HtmlToXamlConverter");
            switch (currentElementName)
            {
                case "colgroup":
                    return htmlElementsClosingColgroup.Contains(nextElementName) && HtmlSchema.IsBlockElement(nextElementName);
                case "dd":
                    return htmlElementsClosingDD.Contains(nextElementName) && HtmlSchema.IsBlockElement(nextElementName);
                case "dt":
                    return htmlElementsClosingDT.Contains(nextElementName) && HtmlSchema.IsBlockElement(nextElementName);
                case "li":
                    return htmlElementsClosingLI.Contains(nextElementName);
                case "p":
                    return HtmlSchema.IsBlockElement(nextElementName);
                case "tbody":
                    return htmlElementsClosingTbody.Contains(nextElementName);
                case "tfoot":
                    return htmlElementsClosingTfoot.Contains(nextElementName);
                case "thead":
                    return htmlElementsClosingThead.Contains(nextElementName);
                case "tr":
                    return htmlElementsClosingTR.Contains(nextElementName);
                case "td":
                    return htmlElementsClosingTD.Contains(nextElementName);
                case "th":
                    return htmlElementsClosingTH.Contains(nextElementName);
            }

            return false;
        }

        /// <summary>
        /// returns true if the string passed as argument is an Html entity name
        /// </summary>
        /// <param name="entityName">string to be tested for Html entity name</param>
        /// <returns>true if the string passed as argument is an Html entity name</returns>
        internal static bool IsEntity(string entityName)
        {
            // we do not convert entity strings to lowercase because these names are case-sensitive
            if (htmlCharacterEntities.Contains(entityName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// returns the character represented by the entity name string which is passed as an argument, if the string is an entity name
        /// as specified in htmlCharacterEntities, returns the character value of 0 otherwise
        /// </summary>
        /// <param name="entityName">
        /// string representing entity name whose character value is desired
        /// </param>
        /// <returns>the character represented by the entity name string which is passed as an argument</returns>
        internal static char EntityCharacterValue(string entityName)
        {
            if (htmlCharacterEntities.Contains(entityName))
            {
                return (char)htmlCharacterEntities[entityName];
            }
            else
            {
                return (char)0;
            }
        }

        #endregion Internal Methods

        // ---------------------------------------------------------------------
        //
        //  Internal Properties
        //
        // ---------------------------------------------------------------------
        #region Internal Properties

        #endregion Internal Indexers

        // ---------------------------------------------------------------------
        //
        // Private Methods
        //
        // ---------------------------------------------------------------------
        #region Private Methods

        /// <summary>
        /// Initialize inline elements
        /// </summary>
        private static void InitializeInlineElements()
        {
            htmlInlineElements = new ArrayList();
            htmlInlineElements.Add("a");
            htmlInlineElements.Add("abbr");
            htmlInlineElements.Add("acronym");
            htmlInlineElements.Add("address");
            htmlInlineElements.Add("b");
            htmlInlineElements.Add("bdo"); // ???
            htmlInlineElements.Add("big");
            htmlInlineElements.Add("button");
            htmlInlineElements.Add("code");
            htmlInlineElements.Add("del"); // deleted text
            htmlInlineElements.Add("dfn");
            htmlInlineElements.Add("em");
            htmlInlineElements.Add("font");
            htmlInlineElements.Add("i");
            htmlInlineElements.Add("ins"); // inserted text
            htmlInlineElements.Add("kbd"); // text to entered by a user
            htmlInlineElements.Add("label");
            htmlInlineElements.Add("legend"); // ???
            htmlInlineElements.Add("q"); // short inline quotation
            htmlInlineElements.Add("s"); // strike-through text style
            htmlInlineElements.Add("samp"); // Specifies a code sample
            htmlInlineElements.Add("small");
            htmlInlineElements.Add("span");
            htmlInlineElements.Add("strike");
            htmlInlineElements.Add("strong");
            htmlInlineElements.Add("sub");
            htmlInlineElements.Add("sup");
            htmlInlineElements.Add("u");
            htmlInlineElements.Add("var"); // indicates an instance of a program variable
        }

        /// <summary>
        /// Initialize block elements
        /// </summary>
        private static void InitializeBlockElements()
        {
            htmlBlockElements = new ArrayList();

            htmlBlockElements.Add("blockquote");
            htmlBlockElements.Add("body");
            htmlBlockElements.Add("caption");
            htmlBlockElements.Add("center");
            htmlBlockElements.Add("cite");
            htmlBlockElements.Add("dd");
            htmlBlockElements.Add("dir"); // treat as UL element
            htmlBlockElements.Add("div");
            htmlBlockElements.Add("dl");
            htmlBlockElements.Add("dt");
            htmlBlockElements.Add("form"); // Not a block according to XHTML spec
            htmlBlockElements.Add("h1");
            htmlBlockElements.Add("h2");
            htmlBlockElements.Add("h3");
            htmlBlockElements.Add("h4");
            htmlBlockElements.Add("h5");
            htmlBlockElements.Add("h6");
            htmlBlockElements.Add("html");
            htmlBlockElements.Add("li");
            htmlBlockElements.Add("menu"); // treat as UL element
            htmlBlockElements.Add("ol");
            htmlBlockElements.Add("p");
            htmlBlockElements.Add("pre"); // Renders text in a fixed-width font
            htmlBlockElements.Add("table");
            htmlBlockElements.Add("tbody");
            htmlBlockElements.Add("td");
            htmlBlockElements.Add("textarea");
            htmlBlockElements.Add("tfoot");
            htmlBlockElements.Add("th");
            htmlBlockElements.Add("thead");
            htmlBlockElements.Add("tr");
            htmlBlockElements.Add("tt");
            htmlBlockElements.Add("ul");
        }

        /// <summary>
        /// initializes htmlEmptyElements with empty elements in HTML 4 spec at
        /// http://www.w3.org/TR/REC-html40/index/elements.html
        /// </summary>
        private static void InitializeEmptyElements()
        {
            // Build a list of empty (no-scope) elements
            // (element not requiring closing tags, and not accepting any content)
            htmlEmptyElements = new ArrayList();
            htmlEmptyElements.Add("area");
            htmlEmptyElements.Add("base");
            htmlEmptyElements.Add("basefont");
            htmlEmptyElements.Add("br");
            htmlEmptyElements.Add("col");
            htmlEmptyElements.Add("frame");
            htmlEmptyElements.Add("hr");
            htmlEmptyElements.Add("img");
            htmlEmptyElements.Add("input");
            htmlEmptyElements.Add("isindex");
            htmlEmptyElements.Add("link");
            htmlEmptyElements.Add("meta");
            htmlEmptyElements.Add("param");
        }

        /// <summary>
        /// Initialize other elements that can be opened
        /// </summary>
        private static void InitializeOtherOpenableElements()
        {
            // It is a list of known html elements which we
            // want to allow to produce HTML parser,
            // but don't want to act as inline, block or no-scope.
            // Presence in this list will allow to open
            // elements during html parsing, and adding the
            // to a tree produced by html parser.
            htmlOtherOpenableElements = new ArrayList();
            htmlOtherOpenableElements.Add("applet");
            htmlOtherOpenableElements.Add("base");
            htmlOtherOpenableElements.Add("basefont");
            htmlOtherOpenableElements.Add("colgroup");
            htmlOtherOpenableElements.Add("fieldset");
            ////htmlOtherOpenableElements.Add("form"); --> treated as block
            htmlOtherOpenableElements.Add("frameset");
            htmlOtherOpenableElements.Add("head");
            htmlOtherOpenableElements.Add("iframe");
            htmlOtherOpenableElements.Add("map");
            htmlOtherOpenableElements.Add("noframes");
            htmlOtherOpenableElements.Add("noscript");
            htmlOtherOpenableElements.Add("object");
            htmlOtherOpenableElements.Add("optgroup");
            htmlOtherOpenableElements.Add("option");
            htmlOtherOpenableElements.Add("script");
            htmlOtherOpenableElements.Add("select");
            htmlOtherOpenableElements.Add("style");
            htmlOtherOpenableElements.Add("title");
        }

        /// <summary>
        /// initializes htmlElementsClosingOnParentElementEnd with the list of HTML 4 elements for which closing tags are optional
        /// we assume that for any element for which closing tags are optional, the element closes when it's outer element
        /// (in which it is nested) does
        /// </summary>
        private static void InitializeElementsClosingOnParentElementEnd()
        {
            htmlElementsClosingOnParentElementEnd = new ArrayList();
            htmlElementsClosingOnParentElementEnd.Add("body");
            htmlElementsClosingOnParentElementEnd.Add("colgroup");
            htmlElementsClosingOnParentElementEnd.Add("dd");
            htmlElementsClosingOnParentElementEnd.Add("dt");
            htmlElementsClosingOnParentElementEnd.Add("head");
            htmlElementsClosingOnParentElementEnd.Add("html");
            htmlElementsClosingOnParentElementEnd.Add("li");
            htmlElementsClosingOnParentElementEnd.Add("p");
            htmlElementsClosingOnParentElementEnd.Add("tbody");
            htmlElementsClosingOnParentElementEnd.Add("td");
            htmlElementsClosingOnParentElementEnd.Add("tfoot");
            htmlElementsClosingOnParentElementEnd.Add("thead");
            htmlElementsClosingOnParentElementEnd.Add("th");
            htmlElementsClosingOnParentElementEnd.Add("tr");
        }

        /// <summary>
        /// Initialize elements closing on new element start
        /// </summary>
        private static void InitializeElementsClosingOnNewElementStart()
        {
            htmlElementsClosingColgroup = new ArrayList();
            htmlElementsClosingColgroup.Add("colgroup");
            htmlElementsClosingColgroup.Add("tr");
            htmlElementsClosingColgroup.Add("thead");
            htmlElementsClosingColgroup.Add("tfoot");
            htmlElementsClosingColgroup.Add("tbody");

            htmlElementsClosingDD = new ArrayList();
            htmlElementsClosingDD.Add("dd");
            htmlElementsClosingDD.Add("dt");
            //// TODO: dd may end in other cases as well - if a new "p" starts, etc.
            //// TODO: these are the basic "legal" cases but there may be more recovery

            htmlElementsClosingDT = new ArrayList();
            htmlElementsClosingDD.Add("dd");
            htmlElementsClosingDD.Add("dt");
            //// TODO: dd may end in other cases as well - if a new "p" starts, etc.
            //// TODO: these are the basic "legal" cases but there may be more recovery

            htmlElementsClosingLI = new ArrayList();
            htmlElementsClosingLI.Add("li");
            //// TODO: more complex recovery

            htmlElementsClosingTbody = new ArrayList();
            htmlElementsClosingTbody.Add("tbody");
            htmlElementsClosingTbody.Add("thead");
            htmlElementsClosingTbody.Add("tfoot");
            //// TODO: more complex recovery

            htmlElementsClosingTR = new ArrayList();

            // NOTE: TR should not really close on a new THEAD
            // because if there are rows before a THEAD, it is assumed to be in TBODY, whose start tag is optional
            // and THEAD can't come after TBODY
            // however, if we do encounter this, it's probably best to end the row and ignore the THEAD or treat
            // it as part of the table
            htmlElementsClosingTR.Add("thead");
            htmlElementsClosingTR.Add("tfoot");
            htmlElementsClosingTR.Add("tbody");
            htmlElementsClosingTR.Add("tr");
            //// TODO: more complex recovery

            htmlElementsClosingTD = new ArrayList();
            htmlElementsClosingTD.Add("td");
            htmlElementsClosingTD.Add("th");
            htmlElementsClosingTD.Add("tr");
            htmlElementsClosingTD.Add("tbody");
            htmlElementsClosingTD.Add("tfoot");
            htmlElementsClosingTD.Add("thead");
            //// TODO: more complex recovery

            htmlElementsClosingTH = new ArrayList();
            htmlElementsClosingTH.Add("td");
            htmlElementsClosingTH.Add("th");
            htmlElementsClosingTH.Add("tr");
            htmlElementsClosingTH.Add("tbody");
            htmlElementsClosingTH.Add("tfoot");
            htmlElementsClosingTH.Add("thead");
            //// TODO: more complex recovery

            htmlElementsClosingThead = new ArrayList();
            htmlElementsClosingThead.Add("tbody");
            htmlElementsClosingThead.Add("tfoot");
            //// TODO: more complex recovery

            htmlElementsClosingTfoot = new ArrayList();
            htmlElementsClosingTfoot.Add("tbody");

            // although THEAD comes before tfoot, we add it because if it is found the tfoot should close
            // and some recovery processing be done on the THEAD
            htmlElementsClosingTfoot.Add("thead");
            //// TODO: more complex recovery
        }

        /// <summary>
        /// initializes htmlCharacterEntities hash table with the character corresponding to entity names
        /// </summary>
        private static void InitializeHtmlCharacterEntities()
        {
            htmlCharacterEntities = new Hashtable();
            htmlCharacterEntities["Aacute"] = (char)193;
            htmlCharacterEntities["aacute"] = (char)225;
            htmlCharacterEntities["Acirc"] = (char)194;
            htmlCharacterEntities["acirc"] = (char)226;
            htmlCharacterEntities["acute"] = (char)180;
            htmlCharacterEntities["AElig"] = (char)198;
            htmlCharacterEntities["aelig"] = (char)230;
            htmlCharacterEntities["Agrave"] = (char)192;
            htmlCharacterEntities["agrave"] = (char)224;
            htmlCharacterEntities["alefsym"] = (char)8501;
            htmlCharacterEntities["Alpha"] = (char)913;
            htmlCharacterEntities["alpha"] = (char)945;
            htmlCharacterEntities["amp"] = (char)38;
            htmlCharacterEntities["and"] = (char)8743;
            htmlCharacterEntities["ang"] = (char)8736;
            htmlCharacterEntities["Aring"] = (char)197;
            htmlCharacterEntities["aring"] = (char)229;
            htmlCharacterEntities["asymp"] = (char)8776;
            htmlCharacterEntities["Atilde"] = (char)195;
            htmlCharacterEntities["atilde"] = (char)227;
            htmlCharacterEntities["Auml"] = (char)196;
            htmlCharacterEntities["auml"] = (char)228;
            htmlCharacterEntities["bdquo"] = (char)8222;
            htmlCharacterEntities["Beta"] = (char)914;
            htmlCharacterEntities["beta"] = (char)946;
            htmlCharacterEntities["brvbar"] = (char)166;
            htmlCharacterEntities["bull"] = (char)8226;
            htmlCharacterEntities["cap"] = (char)8745;
            htmlCharacterEntities["Ccedil"] = (char)199;
            htmlCharacterEntities["ccedil"] = (char)231;
            htmlCharacterEntities["cent"] = (char)162;
            htmlCharacterEntities["Chi"] = (char)935;
            htmlCharacterEntities["chi"] = (char)967;
            htmlCharacterEntities["circ"] = (char)710;
            htmlCharacterEntities["clubs"] = (char)9827;
            htmlCharacterEntities["cong"] = (char)8773;
            htmlCharacterEntities["copy"] = (char)169;
            htmlCharacterEntities["crarr"] = (char)8629;
            htmlCharacterEntities["cup"] = (char)8746;
            htmlCharacterEntities["curren"] = (char)164;
            htmlCharacterEntities["dagger"] = (char)8224;
            htmlCharacterEntities["Dagger"] = (char)8225;
            htmlCharacterEntities["darr"] = (char)8595;
            htmlCharacterEntities["dArr"] = (char)8659;
            htmlCharacterEntities["deg"] = (char)176;
            htmlCharacterEntities["Delta"] = (char)916;
            htmlCharacterEntities["delta"] = (char)948;
            htmlCharacterEntities["diams"] = (char)9830;
            htmlCharacterEntities["divide"] = (char)247;
            htmlCharacterEntities["Eacute"] = (char)201;
            htmlCharacterEntities["eacute"] = (char)233;
            htmlCharacterEntities["Ecirc"] = (char)202;
            htmlCharacterEntities["ecirc"] = (char)234;
            htmlCharacterEntities["Egrave"] = (char)200;
            htmlCharacterEntities["egrave"] = (char)232;
            htmlCharacterEntities["empty"] = (char)8709;
            htmlCharacterEntities["emsp"] = (char)8195;
            htmlCharacterEntities["ensp"] = (char)8194;
            htmlCharacterEntities["Epsilon"] = (char)917;
            htmlCharacterEntities["epsilon"] = (char)949;
            htmlCharacterEntities["equiv"] = (char)8801;
            htmlCharacterEntities["Eta"] = (char)919;
            htmlCharacterEntities["eta"] = (char)951;
            htmlCharacterEntities["ETH"] = (char)208;
            htmlCharacterEntities["eth"] = (char)240;
            htmlCharacterEntities["Euml"] = (char)203;
            htmlCharacterEntities["euml"] = (char)235;
            htmlCharacterEntities["euro"] = (char)8364;
            htmlCharacterEntities["exist"] = (char)8707;
            htmlCharacterEntities["fnof"] = (char)402;
            htmlCharacterEntities["forall"] = (char)8704;
            htmlCharacterEntities["frac12"] = (char)189;
            htmlCharacterEntities["frac14"] = (char)188;
            htmlCharacterEntities["frac34"] = (char)190;
            htmlCharacterEntities["frasl"] = (char)8260;
            htmlCharacterEntities["Gamma"] = (char)915;
            htmlCharacterEntities["gamma"] = (char)947;
            htmlCharacterEntities["ge"] = (char)8805;
            htmlCharacterEntities["gt"] = (char)62;
            htmlCharacterEntities["harr"] = (char)8596;
            htmlCharacterEntities["hArr"] = (char)8660;
            htmlCharacterEntities["hearts"] = (char)9829;
            htmlCharacterEntities["hellip"] = (char)8230;
            htmlCharacterEntities["Iacute"] = (char)205;
            htmlCharacterEntities["iacute"] = (char)237;
            htmlCharacterEntities["Icirc"] = (char)206;
            htmlCharacterEntities["icirc"] = (char)238;
            htmlCharacterEntities["iexcl"] = (char)161;
            htmlCharacterEntities["Igrave"] = (char)204;
            htmlCharacterEntities["igrave"] = (char)236;
            htmlCharacterEntities["image"] = (char)8465;
            htmlCharacterEntities["infin"] = (char)8734;
            htmlCharacterEntities["int"] = (char)8747;
            htmlCharacterEntities["Iota"] = (char)921;
            htmlCharacterEntities["iota"] = (char)953;
            htmlCharacterEntities["iquest"] = (char)191;
            htmlCharacterEntities["isin"] = (char)8712;
            htmlCharacterEntities["Iuml"] = (char)207;
            htmlCharacterEntities["iuml"] = (char)239;
            htmlCharacterEntities["Kappa"] = (char)922;
            htmlCharacterEntities["kappa"] = (char)954;
            htmlCharacterEntities["Lambda"] = (char)923;
            htmlCharacterEntities["lambda"] = (char)955;
            htmlCharacterEntities["lang"] = (char)9001;
            htmlCharacterEntities["laquo"] = (char)171;
            htmlCharacterEntities["larr"] = (char)8592;
            htmlCharacterEntities["lArr"] = (char)8656;
            htmlCharacterEntities["lceil"] = (char)8968;
            htmlCharacterEntities["ldquo"] = (char)8220;
            htmlCharacterEntities["le"] = (char)8804;
            htmlCharacterEntities["lfloor"] = (char)8970;
            htmlCharacterEntities["lowast"] = (char)8727;
            htmlCharacterEntities["loz"] = (char)9674;
            htmlCharacterEntities["lrm"] = (char)8206;
            htmlCharacterEntities["lsaquo"] = (char)8249;
            htmlCharacterEntities["lsquo"] = (char)8216;
            htmlCharacterEntities["lt"] = (char)60;
            htmlCharacterEntities["macr"] = (char)175;
            htmlCharacterEntities["mdash"] = (char)8212;
            htmlCharacterEntities["micro"] = (char)181;
            htmlCharacterEntities["middot"] = (char)183;
            htmlCharacterEntities["minus"] = (char)8722;
            htmlCharacterEntities["Mu"] = (char)924;
            htmlCharacterEntities["mu"] = (char)956;
            htmlCharacterEntities["nabla"] = (char)8711;
            htmlCharacterEntities["nbsp"] = (char)160;
            htmlCharacterEntities["ndash"] = (char)8211;
            htmlCharacterEntities["ne"] = (char)8800;
            htmlCharacterEntities["ni"] = (char)8715;
            htmlCharacterEntities["not"] = (char)172;
            htmlCharacterEntities["notin"] = (char)8713;
            htmlCharacterEntities["nsub"] = (char)8836;
            htmlCharacterEntities["Ntilde"] = (char)209;
            htmlCharacterEntities["ntilde"] = (char)241;
            htmlCharacterEntities["Nu"] = (char)925;
            htmlCharacterEntities["nu"] = (char)957;
            htmlCharacterEntities["Oacute"] = (char)211;
            htmlCharacterEntities["ocirc"] = (char)244;
            htmlCharacterEntities["OElig"] = (char)338;
            htmlCharacterEntities["oelig"] = (char)339;
            htmlCharacterEntities["Ograve"] = (char)210;
            htmlCharacterEntities["ograve"] = (char)242;
            htmlCharacterEntities["oline"] = (char)8254;
            htmlCharacterEntities["Omega"] = (char)937;
            htmlCharacterEntities["omega"] = (char)969;
            htmlCharacterEntities["Omicron"] = (char)927;
            htmlCharacterEntities["omicron"] = (char)959;
            htmlCharacterEntities["oplus"] = (char)8853;
            htmlCharacterEntities["or"] = (char)8744;
            htmlCharacterEntities["ordf"] = (char)170;
            htmlCharacterEntities["ordm"] = (char)186;
            htmlCharacterEntities["Oslash"] = (char)216;
            htmlCharacterEntities["oslash"] = (char)248;
            htmlCharacterEntities["Otilde"] = (char)213;
            htmlCharacterEntities["otilde"] = (char)245;
            htmlCharacterEntities["otimes"] = (char)8855;
            htmlCharacterEntities["Ouml"] = (char)214;
            htmlCharacterEntities["ouml"] = (char)246;
            htmlCharacterEntities["para"] = (char)182;
            htmlCharacterEntities["part"] = (char)8706;
            htmlCharacterEntities["permil"] = (char)8240;
            htmlCharacterEntities["perp"] = (char)8869;
            htmlCharacterEntities["Phi"] = (char)934;
            htmlCharacterEntities["phi"] = (char)966;
            htmlCharacterEntities["pi"] = (char)960;
            htmlCharacterEntities["piv"] = (char)982;
            htmlCharacterEntities["plusmn"] = (char)177;
            htmlCharacterEntities["pound"] = (char)163;
            htmlCharacterEntities["prime"] = (char)8242;
            htmlCharacterEntities["Prime"] = (char)8243;
            htmlCharacterEntities["prod"] = (char)8719;
            htmlCharacterEntities["prop"] = (char)8733;
            htmlCharacterEntities["Psi"] = (char)936;
            htmlCharacterEntities["psi"] = (char)968;
            htmlCharacterEntities["quot"] = (char)34;
            htmlCharacterEntities["radic"] = (char)8730;
            htmlCharacterEntities["rang"] = (char)9002;
            htmlCharacterEntities["raquo"] = (char)187;
            htmlCharacterEntities["rarr"] = (char)8594;
            htmlCharacterEntities["rArr"] = (char)8658;
            htmlCharacterEntities["rceil"] = (char)8969;
            htmlCharacterEntities["rdquo"] = (char)8221;
            htmlCharacterEntities["real"] = (char)8476;
            htmlCharacterEntities["reg"] = (char)174;
            htmlCharacterEntities["rfloor"] = (char)8971;
            htmlCharacterEntities["Rho"] = (char)929;
            htmlCharacterEntities["rho"] = (char)961;
            htmlCharacterEntities["rlm"] = (char)8207;
            htmlCharacterEntities["rsaquo"] = (char)8250;
            htmlCharacterEntities["rsquo"] = (char)8217;
            htmlCharacterEntities["sbquo"] = (char)8218;
            htmlCharacterEntities["Scaron"] = (char)352;
            htmlCharacterEntities["scaron"] = (char)353;
            htmlCharacterEntities["sdot"] = (char)8901;
            htmlCharacterEntities["sect"] = (char)167;
            htmlCharacterEntities["shy"] = (char)173;
            htmlCharacterEntities["Sigma"] = (char)931;
            htmlCharacterEntities["sigma"] = (char)963;
            htmlCharacterEntities["sigmaf"] = (char)962;
            htmlCharacterEntities["sim"] = (char)8764;
            htmlCharacterEntities["spades"] = (char)9824;
            htmlCharacterEntities["sub"] = (char)8834;
            htmlCharacterEntities["sube"] = (char)8838;
            htmlCharacterEntities["sum"] = (char)8721;
            htmlCharacterEntities["sup"] = (char)8835;
            htmlCharacterEntities["sup1"] = (char)185;
            htmlCharacterEntities["sup2"] = (char)178;
            htmlCharacterEntities["sup3"] = (char)179;
            htmlCharacterEntities["supe"] = (char)8839;
            htmlCharacterEntities["szlig"] = (char)223;
            htmlCharacterEntities["Tau"] = (char)932;
            htmlCharacterEntities["tau"] = (char)964;
            htmlCharacterEntities["there4"] = (char)8756;
            htmlCharacterEntities["Theta"] = (char)920;
            htmlCharacterEntities["theta"] = (char)952;
            htmlCharacterEntities["thetasym"] = (char)977;
            htmlCharacterEntities["thinsp"] = (char)8201;
            htmlCharacterEntities["THORN"] = (char)222;
            htmlCharacterEntities["thorn"] = (char)254;
            htmlCharacterEntities["tilde"] = (char)732;
            htmlCharacterEntities["times"] = (char)215;
            htmlCharacterEntities["trade"] = (char)8482;
            htmlCharacterEntities["Uacute"] = (char)218;
            htmlCharacterEntities["uacute"] = (char)250;
            htmlCharacterEntities["uarr"] = (char)8593;
            htmlCharacterEntities["uArr"] = (char)8657;
            htmlCharacterEntities["Ucirc"] = (char)219;
            htmlCharacterEntities["ucirc"] = (char)251;
            htmlCharacterEntities["Ugrave"] = (char)217;
            htmlCharacterEntities["ugrave"] = (char)249;
            htmlCharacterEntities["uml"] = (char)168;
            htmlCharacterEntities["upsih"] = (char)978;
            htmlCharacterEntities["Upsilon"] = (char)933;
            htmlCharacterEntities["upsilon"] = (char)965;
            htmlCharacterEntities["Uuml"] = (char)220;
            htmlCharacterEntities["uuml"] = (char)252;
            htmlCharacterEntities["weierp"] = (char)8472;
            htmlCharacterEntities["Xi"] = (char)926;
            htmlCharacterEntities["xi"] = (char)958;
            htmlCharacterEntities["Yacute"] = (char)221;
            htmlCharacterEntities["yacute"] = (char)253;
            htmlCharacterEntities["yen"] = (char)165;
            htmlCharacterEntities["Yuml"] = (char)376;
            htmlCharacterEntities["yuml"] = (char)255;
            htmlCharacterEntities["Zeta"] = (char)918;
            htmlCharacterEntities["zeta"] = (char)950;
            htmlCharacterEntities["zwj"] = (char)8205;
            htmlCharacterEntities["zwnj"] = (char)8204;
        }

        #endregion Private Methods
    }
}