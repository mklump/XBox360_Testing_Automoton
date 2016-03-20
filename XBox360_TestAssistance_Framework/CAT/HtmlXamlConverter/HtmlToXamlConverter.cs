// -----------------------------------------------------------------------
// <copyright file="HtmlToXamlConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace HtmlToXamlConvert
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Documents;
    using System.Xml;

    // This code was taken from MSDN as an example of converting HTML to XAML.
    //
    // I've combined all the classes together and made some spelling corrections.
    //
    // Copyright (C) Microsoft Corporation.  All rights reserved.
    //
    // Description: Prototype for XAML - Html conversion

    /// <summary>
    /// types of lexical tokens for HTML to XAML converter
    /// </summary>
    internal enum HtmlTokenType
    {
        /// <summary>
        /// Start of opening tag
        /// </summary>
        OpeningTagStart,

        /// <summary>
        /// Start of closing tag
        /// </summary>
        ClosingTagStart,

        /// <summary>
        /// End of tag
        /// </summary>
        TagEnd,

        /// <summary>
        /// End of empty tag
        /// </summary>
        EmptyTagEnd,

        /// <summary>
        /// Equal sign token
        /// </summary>
        EqualSign,

        /// <summary>
        /// Name token
        /// </summary>
        Name,

        /// <summary>
        /// Token for any attribute value not in quotes
        /// </summary>
        Atom,

        /// <summary>
        /// text content when accepting text
        /// </summary>
        Text,

        /// <summary>
        /// Comment token
        /// </summary>
        Comment,

        /// <summary>
        /// EOF Token
        /// </summary>
        EOF,
    }

    /// <summary>
    /// A static class that takes an HTML string
    /// and converts it into XAML
    /// </summary>
    public static class HtmlToXamlConverter
    {
        //// ----------------------------------------------------------------
        ////
        //// Internal Constants
        ////
        //// ----------------------------------------------------------------
        //// These constants represent all XAML names used in a conversion

        /// <summary>
        /// FlowDocument string
        /// </summary>
        public const string XamlFlowDocument = "FlowDocument";

        /// <summary>
        /// Run string
        /// </summary>
        public const string XamlRun = "Run";

        /// <summary>
        /// Span string
        /// </summary>
        public const string XamlSpan = "Span";

        /// <summary>
        /// Hyperlink string
        /// </summary>
        public const string XamlHyperlink = "Hyperlink";

        /// <summary>
        /// NavigateUri string
        /// </summary>
        public const string XamlHyperlinkNavigateUri = "NavigateUri";

        /// <summary>
        /// TargetName string
        /// </summary>
        public const string XamlHyperlinkTargetName = "TargetName";

        /// <summary>
        /// Section string
        /// </summary>
        public const string XamlSection = "Section";

        /// <summary>
        /// List string
        /// </summary>
        public const string XamlList = "List";

        /// <summary>
        /// List MarkerStyle string
        /// </summary>
        public const string XamlListMarkerStyle = "MarkerStyle";

        /// <summary>
        /// List MarkerStyle None string
        /// </summary>
        public const string XamlListMarkerStyleNone = "None";

        /// <summary>
        /// List MarkerStyle Decimal string
        /// </summary>
        public const string XamlListMarkerStyleDecimal = "Decimal";

        /// <summary>
        /// List MarkerStyle Disc string
        /// </summary>
        public const string XamlListMarkerStyleDisc = "Disc";

        /// <summary>
        /// List MarkerStyle Circle string
        /// </summary>
        public const string XamlListMarkerStyleCircle = "Circle";

        /// <summary>
        /// List MarkerStyle Square string
        /// </summary>
        public const string XamlListMarkerStyleSquare = "Square";

        /// <summary>
        /// List MarkerStyle Box string
        /// </summary>
        public const string XamlListMarkerStyleBox = "Box";

        /// <summary>
        /// List MarkerStyle LowerLatin string
        /// </summary>
        public const string XamlListMarkerStyleLowerLatin = "LowerLatin";

        /// <summary>
        /// List MarkerStyle UpperLatin string
        /// </summary>
        public const string XamlListMarkerStyleUpperLatin = "UpperLatin";

        /// <summary>
        /// List MarkerStyle LowerRoman string
        /// </summary>
        public const string XamlListMarkerStyleLowerRoman = "LowerRoman";

        /// <summary>
        /// List MarkerStyle UpperRoman string
        /// </summary>
        public const string XamlListMarkerStyleUpperRoman = "UpperRoman";

        /// <summary>
        /// ListItem string
        /// </summary>
        public const string XamlListItem = "ListItem";

        /// <summary>
        /// LineBreak string
        /// </summary>
        public const string XamlLineBreak = "LineBreak";

        /// <summary>
        /// Paragraph string
        /// </summary>
        public const string XamlParagraph = "Paragraph";

        /// <summary>
        /// Margin string
        /// </summary>
        public const string XamlMargin = "Margin";

        /// <summary>
        /// Padding string
        /// </summary>
        public const string XamlPadding = "Padding";

        /// <summary>
        /// BorderBrush string
        /// </summary>
        public const string XamlBorderBrush = "BorderBrush";

        /// <summary>
        /// BorderThickness string
        /// </summary>
        public const string XamlBorderThickness = "BorderThickness";

        /// <summary>
        /// Table string
        /// </summary>
        public const string XamlTable = "Table";

        /// <summary>
        /// TableColumn string
        /// </summary>
        public const string XamlTableColumn = "TableColumn";

        /// <summary>
        /// TableRowGroup string
        /// </summary>
        public const string XamlTableRowGroup = "TableRowGroup";

        /// <summary>
        /// TableRow string
        /// </summary>
        public const string XamlTableRow = "TableRow";

        /// <summary>
        /// TableCell string
        /// </summary>
        public const string XamlTableCell = "TableCell";

        /// <summary>
        /// TableCell BorderThickness string
        /// </summary>
        public const string XamlTableCellBorderThickness = "BorderThickness";

        /// <summary>
        /// TableCell BorderBrush string
        /// </summary>
        public const string XamlTableCellBorderBrush = "BorderBrush";

        /// <summary>
        /// TableCell ColumnSpan string
        /// </summary>
        public const string XamlTableCellColumnSpan = "ColumnSpan";

        /// <summary>
        /// TableCell RowSpan string
        /// </summary>
        public const string XamlTableCellRowSpan = "RowSpan";

        /// <summary>
        /// Width string
        /// </summary>
        public const string XamlWidth = "Width";

        /// <summary>
        /// Black string
        /// </summary>
        public const string XamlBrushesBlack = "Black";

        /// <summary>
        /// FontFamily string
        /// </summary>
        public const string XamlFontFamily = "FontFamily";

        /// <summary>
        /// FontSize string
        /// </summary>
        public const string XamlFontSize = "FontSize";

        /// <summary>
        /// FontSize XXLarge string
        /// </summary>
        public const string XamlFontSizeXXLarge = "22pt"; // "XXLarge";

        /// <summary>
        /// FontSize XLarge string
        /// </summary>
        public const string XamlFontSizeXLarge = "20pt"; // "XLarge";

        /// <summary>
        /// FontSize Large string
        /// </summary>
        public const string XamlFontSizeLarge = "18pt"; // "Large";

        /// <summary>
        /// FontSize Medium string
        /// </summary>
        public const string XamlFontSizeMedium = "16pt"; // "Medium";

        /// <summary>
        /// FontSize Small string
        /// </summary>
        public const string XamlFontSizeSmall = "12pt"; // "Small";

        /// <summary>
        /// FontSize XSmall string
        /// </summary>
        public const string XamlFontSizeXSmall = "10pt"; // "XSmall";

        /// <summary>
        /// FontSize Medium XXSmall string
        /// </summary>
        public const string XamlFontSizeXXSmall = "8pt"; // "XXSmall";

        /// <summary>
        /// FontWeight string
        /// </summary>
        public const string XamlFontWeight = "FontWeight";

        /// <summary>
        /// FontWeight Bold string
        /// </summary>
        public const string XamlFontWeightBold = "Bold";

        /// <summary>
        /// FontStyle string
        /// </summary>
        public const string XamlFontStyle = "FontStyle";

        /// <summary>
        /// Foreground string
        /// </summary>
        public const string XamlForeground = "Foreground";

        /// <summary>
        /// Background string
        /// </summary>
        public const string XamlBackground = "Background";

        /// <summary>
        /// TextDecorations string
        /// </summary>
        public const string XamlTextDecorations = "TextDecorations";

        /// <summary>
        /// TextDecorations Underline string
        /// </summary>
        public const string XamlTextDecorationsUnderline = "Underline";

        /// <summary>
        /// TextIndent string
        /// </summary>
        public const string XamlTextIndent = "TextIndent";

        /// <summary>
        /// TextAlignment string
        /// </summary>
        public const string XamlTextAlignment = "TextAlignment";

        // ---------------------------------------------------------------------
        //
        // Private Fields
        //
        // ---------------------------------------------------------------------
        #region Private Fields

        /// <summary>
        /// Stores a parent XAML element for the case when selected fragment is inline.
        /// </summary>
        private static XmlElement inlineFragmentParentElement;

        /// <summary>
        /// XAML namespace
        /// </summary>
        private static string xamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        #endregion Private Fields

        // ---------------------------------------------------------------------
        //
        // Internal Methods
        //
        // ---------------------------------------------------------------------
        #region Internal Methods

        /// <summary>
        /// Converts an HTML string into XAML string.
        /// </summary>
        /// <param name="htmlString">
        /// Input html which may be badly formatted XML.
        /// </param>
        /// <param name="asFlowDocument">
        /// true indicates that we need a FlowDocument as a root element;
        /// false means that Section or Span elements will be used
        /// depending on StartFragment/EndFragment comments locations.
        /// </param>
        /// <returns>
        /// Well-formed XML representing XAML equivalent for the input HTML string.
        /// </returns>
        public static string ConvertHtmlToXaml(string htmlString, bool asFlowDocument)
        {
            // Create well-formed Xml from Html string
            XmlElement htmlElement = HtmlParser.ParseHtml(htmlString);

            // Decide what name to use as a root
            string rootElementName = asFlowDocument ? HtmlToXamlConverter.XamlFlowDocument : HtmlToXamlConverter.XamlSection;

            // Create an XmlDocument for generated XAML
            XmlDocument xamlTree = new XmlDocument();
            XmlElement xamlFlowDocumentElement = xamlTree.CreateElement(null, rootElementName, xamlNamespace);

            // Extract style definitions from all STYLE elements in the document
            CssStylesheet stylesheet = new CssStylesheet(htmlElement);

            // Source context is a stack of all elements - ancestors of a parentElement
            List<XmlElement> sourceContext = new List<XmlElement>(10);

            // Clear fragment parent
            inlineFragmentParentElement = null;

            // convert root html element
            AddBlock(xamlFlowDocumentElement, htmlElement, new Hashtable(), stylesheet, sourceContext);

            // In case if the selected fragment is inline, extract it into a separate Span wrapper
            if (!asFlowDocument)
            {
                xamlFlowDocumentElement = ExtractInlineFragment(xamlFlowDocumentElement);
            }

            // Return a string representing resulting XAML
            xamlFlowDocumentElement.SetAttribute("xml:space", "preserve");
            string xaml = xamlFlowDocumentElement.OuterXml;

            return xaml;
        }

        /// <summary>
        /// Returns a value for an attribute by its name (ignoring casing)
        /// </summary>
        /// <param name="element">
        /// XmlElement in which we are trying to find the specified attribute
        /// </param>
        /// <param name="attributeName">
        /// String representing the attribute name to be searched for
        /// </param>
        /// <returns>a value for an attribute by its name (ignoring casing)</returns>
        public static string GetAttribute(XmlElement element, string attributeName)
        {
            attributeName = attributeName.ToLower();

            for (int i = 0; i < element.Attributes.Count; i++)
            {
                if (element.Attributes[i].Name.ToLower() == attributeName)
                {
                    return element.Attributes[i].Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns string extracted from quotation marks
        /// </summary>
        /// <param name="value">
        /// String representing value enclosed in quotation marks
        /// </param>
        /// <returns>string extracted from quotation marks</returns>
        internal static string UnQuote(string value)
        {
            if ((value.StartsWith("\"") && value.EndsWith("\"")) || (value.StartsWith("'") && value.EndsWith("'")))
            {
                value = value.Substring(1, value.Length - 2).Trim();
            }

            return value;
        }

        #endregion Internal Methods

        // ---------------------------------------------------------------------
        //
        // Private Methods
        //
        // ---------------------------------------------------------------------
        #region Private Methods

        /// <summary>
        /// Analyzes the given htmlElement expecting it to be converted
        /// into some of XAML Block elements and adds the converted block
        /// to the children collection of XAML parent elements.
        /// Analyzes the given XmlElement htmlElement, recognizes it as some HTML element
        /// and adds it as a child to a XAML parent element.
        /// In some cases several following siblings of the given htmlElement
        /// will be consumed too (e.g. LIs encountered without wrapping UL/OL,
        /// which must be collected together and wrapped into one implicit List element).
        /// </summary>
        /// <param name="xamlParentElement">
        /// Parent XAML element, to which new converted element will be added
        /// </param>
        /// <param name="htmlNode">
        /// Source html element subject to convert to XAML.
        /// </param>
        /// <param name="inheritedProperties">
        /// Properties inherited from an outer context.
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        /// <returns>
        /// Last processed html node. Normally it should be the same htmlElement
        /// as was passed as a parameter, but in some irregular cases
        /// it could one of its following siblings.
        /// The caller must use this node to get to next sibling from it.
        /// </returns>
        private static XmlNode AddBlock(XmlElement xamlParentElement, XmlNode htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            if (htmlNode is XmlComment)
            {
                DefineInlineFragmentParent((XmlComment)htmlNode, /*xamlParentElement:*/null);
            }
            else if (htmlNode is XmlText)
            {
                htmlNode = AddImplicitParagraph(xamlParentElement, htmlNode, inheritedProperties, stylesheet, sourceContext);
            }
            else if (htmlNode is XmlElement)
            {
                // Identify element name
                XmlElement htmlElement = (XmlElement)htmlNode;

                string htmlElementName = htmlElement.LocalName; // Keep the name case-sensitive to check xml names
                string htmlElementNamespace = htmlElement.NamespaceURI;

                if (htmlElementNamespace != HtmlParser.XhtmlNamespace)
                {
                    // Non-html element. skip it
                    // Isn't it too agressive? What if this is just an error in html tag name?
                    // TODO: Consider skipping just a wparrer in recursing into the element tree,
                    // which may produce some garbage though coming from xml fragments.
                    return htmlElement;
                }

                // Put source element to the stack
                sourceContext.Add(htmlElement);

                // Convert the name to lowercase, because html elements are case-insensitive
                htmlElementName = htmlElementName.ToLower();

                // Switch to an appropriate kind of processing depending on html element name
                switch (htmlElementName)
                {
                    // Sections:
                    case "html":
                    case "body":
                    case "div":
                    case "form": // not a block according to xhtml spec
                    case "pre": // Renders text in a fixed-width font
                    case "blockquote":
                    case "caption":
                    case "center":
                    case "cite":
                        AddSection(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;

                    // Paragraphs:
                    case "p":
                    case "h1":
                    case "h2":
                    case "h3":
                    case "h4":
                    case "h5":
                    case "h6":
                    case "nsrtitle":
                    case "textarea":
                    case "dd": // ???
                    case "dl": // ???
                    case "dt": // ???
                    case "tt": // ???
                        AddParagraph(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;

                    case "ol":
                    case "ul":
                    case "dir": // treat as UL element
                    case "menu": // treat as UL element
                        // List element conversion
                        AddList(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;
                    case "li":
                        // LI outside of OL/UL
                        // Collect all sibling LIs, wrap them into a List and then proceed with the element following the last of LIs
                        htmlNode = AddOrphanListItems(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;

                    case "img":
                        // TODO: Add image processing
                        AddImage(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;

                    case "table":
                        // hand off to table parsing function which will perform special table syntax checks
                        AddTable(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;

                    case "tbody":
                    case "tfoot":
                    case "thead":
                    case "tr":
                    case "td":
                    case "th":
                        // Table stuff without table wrapper
                        // TODO: add special-case processing here for elements that should be within tables when the
                        // parent element is NOT a table. If the parent element is a table they can be processed normally.
                        // we need to compare against the parent element here, we can't just break on a switch
                        goto default; // Thus we will skip this element as unknown, but still recurse into it.

                    case "style": // We already pre-processed all style elements. Ignore it now
                    case "meta":
                    case "head":
                    case "title":
                    case "script":
                        // Ignore these elements
                        break;

                    default:
                        // Wrap a sequence of inlines into an implicit paragraph
                        htmlNode = AddImplicitParagraph(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;
                }

                // Remove the element from the stack
                Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlElement, "HtmlToXamlConverter");
                sourceContext.RemoveAt(sourceContext.Count - 1);
            }

            // Return last processed node
            return htmlNode;
        }

        // .............................................................
        //
        // Line Breaks
        //
        // .............................................................

        /// <summary>
        /// Add break
        /// </summary>
        /// <param name="xamlParentElement">XAML parent element</param>
        /// <param name="htmlElementName">HTML element name</param>
        private static void AddBreak(XmlElement xamlParentElement, string htmlElementName)
        {
            // Create new XAML element corresponding to this html element
            XmlElement xamlLineBreak = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.XamlLineBreak, xamlNamespace);
            xamlParentElement.AppendChild(xamlLineBreak);
            if (htmlElementName == "hr")
            {
                XmlText xamlHorizontalLine = xamlParentElement.OwnerDocument.CreateTextNode("----------------------");
                xamlParentElement.AppendChild(xamlHorizontalLine);
                xamlLineBreak = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.XamlLineBreak, xamlNamespace);
                xamlParentElement.AppendChild(xamlLineBreak);
            }
        }

        // .............................................................
        //
        // Text Flow Elements
        //
        // .............................................................

        /// <summary>
        /// Generates Section or Paragraph element from DIV depending whether it contains any block elements or not
        /// </summary>
        /// <param name="xamlParentElement">
        /// XmlElement representing XAML parent to which the converted element should be added
        /// </param>
        /// <param name="htmlElement">
        /// XmlElement representing Html element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddSection(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Analyze the content of htmlElement to decide what XAML element to choose - Section or Paragraph.
            // If this Div has at least one block child then we need to use Section, otherwise use Paragraph
            bool htmlElementContainsBlocks = false;
            for (XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
            {
                if (htmlChildNode is XmlElement)
                {
                    string htmlChildName = ((XmlElement)htmlChildNode).LocalName.ToLower();
                    if (HtmlSchema.IsBlockElement(htmlChildName))
                    {
                        htmlElementContainsBlocks = true;
                        break;
                    }
                }
            }

            if (!htmlElementContainsBlocks)
            {
                // The Div does not contain any block elements, so we can treat it as a Paragraph
                AddParagraph(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
            }
            else
            {
                // The Div has some nested blocks, so we treat it as a Section

                // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
                Hashtable localProperties;
                Hashtable currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

                // Create a XAML element corresponding to this html element
                XmlElement xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.XamlSection, xamlNamespace);
                ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/true);

                // Decide whether we can unwrap this element as not having any formatting significance.
                if (!xamlElement.HasAttributes)
                {
                    // This elements is a group of block elements whitout any additional formatting.
                    // We can add blocks directly to xamlParentElement and avoid
                    // creating unnecessary Sections nesting.
                    xamlElement = xamlParentElement;
                }

                // Recurse into element subtree
                for (XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode != null ? htmlChildNode.NextSibling : null)
                {
                    htmlChildNode = AddBlock(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
                }

                // Add the new element to the parent.
                if (xamlElement != xamlParentElement)
                {
                    xamlParentElement.AppendChild(xamlElement);
                }
            }
        }

        /// <summary>
        /// Generates Paragraph element from P, H1-H7, Center etc.
        /// </summary>
        /// <param name="xamlParentElement">
        /// XmlElement representing XAML parent to which the converted element should be added
        /// </param>
        /// <param name="htmlElement">
        /// XmlElement representing Html element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddParagraph(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            // Create a XAML element corresponding to this html element
            XmlElement xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.XamlParagraph, xamlNamespace);
            ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/true);

            // Recurse into element subtree
            for (XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
            {
                AddInline(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
            }

            // Add the new element to the parent.
            xamlParentElement.AppendChild(xamlElement);
        }

        /// <summary>
        /// Creates a Paragraph element and adds all nodes starting from htmlNode
        /// converted to appropriate inline elements.
        /// </summary>
        /// <param name="xamlParentElement">
        /// XmlElement representing XAML parent to which the converted element should be added
        /// </param>
        /// <param name="htmlNode">
        /// XmlNode starting a collection of implicitly wrapped inline elements.
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        /// <returns>
        /// The last htmlNode added to the implicit paragraph
        /// </returns>
        private static XmlNode AddImplicitParagraph(XmlElement xamlParentElement, XmlNode htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Collect all non-block elements and wrap them into implicit Paragraph
            XmlElement xamlParagraph = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.XamlParagraph, xamlNamespace);
            XmlNode lastNodeProcessed = null;
            while (htmlNode != null)
            {
                if (htmlNode is XmlComment)
                {
                    DefineInlineFragmentParent((XmlComment)htmlNode, /*xamlParentElement:*/null);
                }
                else if (htmlNode is XmlText)
                {
                    if (htmlNode.Value.Trim().Length > 0)
                    {
                        AddTextRun(xamlParagraph, htmlNode.Value);
                    }
                }
                else if (htmlNode is XmlElement)
                {
                    string htmlChildName = ((XmlElement)htmlNode).LocalName.ToLower();
                    if (HtmlSchema.IsBlockElement(htmlChildName))
                    {
                        // The sequence of non-blocked inlines ended. Stop implicit loop here.
                        break;
                    }
                    else
                    {
                        AddInline(xamlParagraph, (XmlElement)htmlNode, inheritedProperties, stylesheet, sourceContext);
                    }
                }

                // Store last processed node to return it at the end
                lastNodeProcessed = htmlNode;
                htmlNode = htmlNode.NextSibling;
            }

            // Add the Paragraph to the parent
            // If only whitespaces and commens have been encountered,
            // then we have nothing to add in implicit paragraph; forget it.
            if (xamlParagraph.FirstChild != null)
            {
                xamlParentElement.AppendChild(xamlParagraph);
            }

            // Need to return last processed node
            return lastNodeProcessed;
        }

        // .............................................................
        //
        // Inline Elements
        //
        // .............................................................

        /// <summary>
        /// Add Inline
        /// </summary>
        /// <param name="xamlParentElement">XAML parent element</param>
        /// <param name="htmlNode">HTML node</param>
        /// <param name="inheritedProperties">Inherited properties</param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddInline(XmlElement xamlParentElement, XmlNode htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            if (htmlNode is XmlComment)
            {
                DefineInlineFragmentParent((XmlComment)htmlNode, xamlParentElement);
            }
            else if (htmlNode is XmlText)
            {
                AddTextRun(xamlParentElement, htmlNode.Value);
            }
            else if (htmlNode is XmlElement)
            {
                XmlElement htmlElement = (XmlElement)htmlNode;

                // Check whether this is an html element
                if (htmlElement.NamespaceURI != HtmlParser.XhtmlNamespace)
                {
                    return; // Skip non-html elements
                }

                // Identify element name
                string htmlElementName = htmlElement.LocalName.ToLower();

                // Put source element to the stack
                sourceContext.Add(htmlElement);

                switch (htmlElementName)
                {
                    case "a":
                        AddHyperlink(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;
                    case "img":
                        AddImage(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        break;
                    case "br":
                    case "hr":
                        AddBreak(xamlParentElement, htmlElementName);
                        break;
                    default:
                        if (HtmlSchema.IsInlineElement(htmlElementName) || HtmlSchema.IsBlockElement(htmlElementName))
                        {
                            // Note: actually we do not expect block elements here,
                            // but if it happens to be here, we will treat it as a Span.
                            AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
                        }

                        break;
                }

                // Ignore all other elements non-(block/inline/image)
                // Remove the element from the stack
                Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlElement, "HtmlToXamlConverter");
                sourceContext.RemoveAt(sourceContext.Count - 1);
            }
        }

        /// <summary>
        /// Add span or run
        /// </summary>
        /// <param name="xamlParentElement">XAML parent element</param>
        /// <param name="htmlElement">HTML element</param>
        /// <param name="inheritedProperties">Inherited properties</param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddSpanOrRun(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Decide what XAML element to use for this inline element.
            // Check whether it contains any nested inlines
            bool elementHasChildren = false;
            for (XmlNode htmlNode = htmlElement.FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling)
            {
                if (htmlNode is XmlElement)
                {
                    string htmlChildName = ((XmlElement)htmlNode).LocalName.ToLower();
                    if (HtmlSchema.IsInlineElement(htmlChildName) || HtmlSchema.IsBlockElement(htmlChildName) ||
                        htmlChildName == "img" || htmlChildName == "br" || htmlChildName == "hr")
                    {
                        elementHasChildren = true;
                        break;
                    }
                }
            }

            string xamlElementName = elementHasChildren ? HtmlToXamlConverter.XamlSpan : HtmlToXamlConverter.XamlRun;

            // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            // Create a XAML element corresponding to this html element
            XmlElement xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/xamlElementName, xamlNamespace);
            ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/false);

            // Recurse into element subtree
            for (XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
            {
                AddInline(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
            }

            // Add the new element to the parent.
            xamlParentElement.AppendChild(xamlElement);
        }

        /// <summary>
        /// Adds a text run to a XAML tree
        /// </summary>
        /// <param name="xamlElement">XAML element</param>
        /// <param name="textData">Text data</param>
        private static void AddTextRun(XmlElement xamlElement, string textData)
        {
            // Remove control characters
            for (int i = 0; i < textData.Length; i++)
            {
                if (char.IsControl(textData[i]))
                {
                    textData = textData.Remove(i--, 1);  // decrement i to compensate for character removal
                }
            }

            // Replace No-Breaks by spaces (160 is a code of &nbsp; entity in html)
            //  This is a work around since WPF/XAML does not support &nbsp.
            textData = textData.Replace((char)160, ' ');

            if (textData.Length > 0)
            {
                xamlElement.AppendChild(xamlElement.OwnerDocument.CreateTextNode(textData));
            }
        }

        /// <summary>
        /// Add Hyperlink
        /// </summary>
        /// <param name="xamlParentElement">XAML parent element</param>
        /// <param name="htmlElement">HTML element</param>
        /// <param name="inheritedProperties">Inherited properties</param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddHyperlink(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Convert href attribute into NavigateUri and TargetName
            string href = GetAttribute(htmlElement, "href");
            if (href == null)
            {
                // When href attribute is missing - ignore the hyperlink
                AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
            }
            else
            {
                // Create currentProperties as a compilation of local and inheritedProperties, set localProperties
                Hashtable localProperties;
                Hashtable currentProperties = GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

                // Create a XAML element corresponding to this html element
                XmlElement xamlElement = xamlParentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.XamlHyperlink, xamlNamespace);
                ApplyLocalProperties(xamlElement, localProperties, /*isBlock:*/false);

                string[] hrefParts = href.Split(new char[] { '#' });
                if (hrefParts.Length > 0 && hrefParts[0].Trim().Length > 0)
                {
                    xamlElement.SetAttribute(HtmlToXamlConverter.XamlHyperlinkNavigateUri, hrefParts[0].Trim());
                }

                if (hrefParts.Length == 2 && hrefParts[1].Trim().Length > 0)
                {
                    xamlElement.SetAttribute(HtmlToXamlConverter.XamlHyperlinkTargetName, hrefParts[1].Trim());
                }

                // Recurse into element subtree
                for (XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
                {
                    AddInline(xamlElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
                }

                // Add the new element to the parent.
                xamlParentElement.AppendChild(xamlElement);
            }
        }

        /// <summary>
        /// Called when html comment is encountered to store a parent element
        /// for the case when the fragment is inline - to extract it to a separate
        /// Span wrapper after the conversion.
        /// </summary>
        /// <param name="htmlComment">HTML comment</param>
        /// <param name="xamlParentElement">XAML parent element</param>
        private static void DefineInlineFragmentParent(XmlComment htmlComment, XmlElement xamlParentElement)
        {
            if (htmlComment.Value == "StartFragment")
            {
                inlineFragmentParentElement = xamlParentElement;
            }
            else if (htmlComment.Value == "EndFragment")
            {
                if (inlineFragmentParentElement == null && xamlParentElement != null)
                {
                    // Normally this cannot happen if comments produced by correct copying code
                    // in Word or IE, but when it is produced manually then fragment boundary
                    // markers can be inconsistent. In this case StartFragment takes precedence,
                    // but if it is not set, then we get the value from EndFragment marker.
                    inlineFragmentParentElement = xamlParentElement;
                }
            }
        }

        /// <summary>
        /// Extracts a content of an element stored as inlineFragmentParentElement
        /// into a separate Span wrapper.
        /// Note: when selected content does not cross paragraph boundaries,
        /// the fragment is marked within
        /// </summary>
        /// <param name="xamlFlowDocumentElement">XAML FlowDocument element</param>
        /// <returns>a content of an element stored as inlineFragmentParentElement into a separate Span wrapper</returns>
        private static XmlElement ExtractInlineFragment(XmlElement xamlFlowDocumentElement)
        {
            if (inlineFragmentParentElement != null)
            {
                if (inlineFragmentParentElement.LocalName == HtmlToXamlConverter.XamlSpan)
                {
                    xamlFlowDocumentElement = inlineFragmentParentElement;
                }
                else
                {
                    xamlFlowDocumentElement = xamlFlowDocumentElement.OwnerDocument.CreateElement(/*prefix:*/null, /*localName:*/HtmlToXamlConverter.XamlSpan, xamlNamespace);
                    while (inlineFragmentParentElement.FirstChild != null)
                    {
                        XmlNode copyNode = inlineFragmentParentElement.FirstChild;
                        inlineFragmentParentElement.RemoveChild(copyNode);
                        xamlFlowDocumentElement.AppendChild(copyNode);
                    }
                }
            }

            return xamlFlowDocumentElement;
        }

        // .............................................................
        //
        // Images
        //
        // .............................................................

        /// <summary>
        /// Add Image
        /// </summary>
        /// <param name="xamlParentElement">XAML parent element</param>
        /// <param name="htmlElement">HTML element</param>
        /// <param name="inheritedProperties">Inherited properties</param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddImage(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Implement images
        }

        // .............................................................
        //
        // Lists
        //
        // .............................................................

        /// <summary>
        /// Converts HTML UL or OL element into XAML list element. During conversion if the UL/OL element has any children
        /// that are not LI elements, they are ignored and not added to the list element
        /// </summary>
        /// <param name="xamlParentElement">
        /// XmlElement representing XAML parent to which the converted element should be added
        /// </param>
        /// <param name="htmlListElement">
        /// XmlElement representing Html UL/OL element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddList(XmlElement xamlParentElement, XmlElement htmlListElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            string htmlListElementName = htmlListElement.LocalName.ToLower();

            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlListElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            // Create XAML List element
            XmlElement xamlListElement = xamlParentElement.OwnerDocument.CreateElement(null, XamlList, xamlNamespace);

            // Set default list markers
            if (htmlListElementName == "ol")
            {
                // Ordered list
                xamlListElement.SetAttribute(HtmlToXamlConverter.XamlListMarkerStyle, XamlListMarkerStyleDecimal);
            }
            else
            {
                // Unordered list - all elements other than OL treated as unordered lists
                xamlListElement.SetAttribute(HtmlToXamlConverter.XamlListMarkerStyle, XamlListMarkerStyleDisc);
            }

            // Apply local properties to list to set marker attribute if specified
            // TODO: Should we have separate list attribute processing function?
            ApplyLocalProperties(xamlListElement, localProperties, /*isBlock:*/true);

            // Recurse into list subtree
            for (XmlNode htmlChildNode = htmlListElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
            {
                if (htmlChildNode is XmlElement && htmlChildNode.LocalName.ToLower() == "li")
                {
                    sourceContext.Add((XmlElement)htmlChildNode);
                    AddListItem(xamlListElement, (XmlElement)htmlChildNode, currentProperties, stylesheet, sourceContext);
                    Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlChildNode, "HtmlToXamlConverter");
                    sourceContext.RemoveAt(sourceContext.Count - 1);
                }
                else
                {
                    // Not an LI element. Add it to previous ListBoxItem
                    //  We need to append the content to the end
                    // of a previous list item.
                }
            }

            // Add the List element to XAML tree - if it is not empty
            if (xamlListElement.HasChildNodes)
            {
                xamlParentElement.AppendChild(xamlListElement);
            }
        }

        /// <summary>
        /// If LI items are found without a parent UL/OL element in Html string, creates XAML list element as their parent and adds
        /// them to it. If the previously added node to the same XAML parent element was a List, adds the elements to that list.
        /// Otherwise, we create a new XAML list element and add them to it. Elements are added as long as LI elements appear sequentially.
        /// The first non-LI or text node stops the addition.
        /// </summary>
        /// <param name="xamlParentElement">
        /// Parent element for the list
        /// </param>
        /// <param name="htmlLIElement">
        /// Start Html LI element without parent list
        /// </param>
        /// <param name="inheritedProperties">
        /// Properties inherited from parent context
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        /// <returns>
        /// XmlNode representing the first non-LI node in the input after one or more LI's have been processed.
        /// </returns>
        private static XmlElement AddOrphanListItems(XmlElement xamlParentElement, XmlElement htmlLIElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            Debug.Assert(htmlLIElement.LocalName.ToLower() == "li", "HtmlToXamlConverter");

            XmlElement lastProcessedListItemElement = null;

            // Find out the last element attached to the xamlParentElement, which is the previous sibling of this node
            XmlNode xamlListItemElementPreviousSibling = xamlParentElement.LastChild;
            XmlElement xamlListElement;
            if (xamlListItemElementPreviousSibling != null && xamlListItemElementPreviousSibling.LocalName == XamlList)
            {
                // Previously added XAML element was a list. We will add the new li to it
                xamlListElement = (XmlElement)xamlListItemElementPreviousSibling;
            }
            else
            {
                // No list element near. Create our own.
                xamlListElement = xamlParentElement.OwnerDocument.CreateElement(null, XamlList, xamlNamespace);
                xamlParentElement.AppendChild(xamlListElement);
            }

            XmlNode htmlChildNode = htmlLIElement;
            string htmlChildNodeName = htmlChildNode == null ? null : htmlChildNode.LocalName.ToLower();

            // Current element properties missed here.
            // currentProperties = GetElementProperties(htmlLIElement, inheritedProperties, out localProperties, stylesheet);

            // Add li elements to the parent xamlListElement we created as long as they appear sequentially
            // Use properties inherited from xamlParentElement for context
            while (htmlChildNode != null && htmlChildNodeName == "li")
            {
                AddListItem(xamlListElement, (XmlElement)htmlChildNode, inheritedProperties, stylesheet, sourceContext);
                lastProcessedListItemElement = (XmlElement)htmlChildNode;
                htmlChildNode = htmlChildNode.NextSibling;
                htmlChildNodeName = htmlChildNode == null ? null : htmlChildNode.LocalName.ToLower();
            }

            return lastProcessedListItemElement;
        }

        /// <summary>
        /// Converts htmlLIElement into XAML ListItem element, and appends it to the parent XAML list element
        /// </summary>
        /// <param name="xamlListElement">
        /// XmlElement representing XAML list element to which the converted TD/TH should be added
        /// </param>
        /// <param name="htmlLIElement">
        /// XmlElement representing HTML LI element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// Properties inherited from parent context
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddListItem(XmlElement xamlListElement, XmlElement htmlLIElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Parameter validation
            Debug.Assert(xamlListElement != null, "HtmlToXamlConverter");
            Debug.Assert(xamlListElement.LocalName == XamlList, "HtmlToXamlConverter");
            Debug.Assert(htmlLIElement != null, "HtmlToXamlConverter");
            Debug.Assert(htmlLIElement.LocalName.ToLower() == "li", "HtmlToXamlConverter");
            Debug.Assert(inheritedProperties != null, "HtmlToXamlConverter");

            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlLIElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            XmlElement xamlListItemElement = xamlListElement.OwnerDocument.CreateElement(null, XamlListItem, xamlNamespace);

            // TODO: process local properties for li element

            // Process children of the ListItem
            for (XmlNode htmlChildNode = htmlLIElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode != null ? htmlChildNode.NextSibling : null)
            {
                htmlChildNode = AddBlock(xamlListItemElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
            }

            // Add resulting ListBoxItem to a XAML parent
            xamlListElement.AppendChild(xamlListItemElement);
        }

        // .............................................................
        //
        // Tables
        //
        // .............................................................

        /// <summary>
        /// Converts htmlTableElement to a XAML Table element. Adds TBODY elements if they are missing so
        /// that a resulting XAML Table element is properly formed.
        /// </summary>
        /// <param name="xamlParentElement">
        /// Parent XAML element to which a converted table must be added.
        /// </param>
        /// <param name="htmlTableElement">
        /// XmlElement representing the HTML table element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// Hash table representing properties inherited from parent context.
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddTable(XmlElement xamlParentElement, XmlElement htmlTableElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Parameter validation
            Debug.Assert(htmlTableElement.LocalName.ToLower() == "table", "HtmlToXamlConverter");
            Debug.Assert(xamlParentElement != null, "HtmlToXamlConverter");
            Debug.Assert(inheritedProperties != null, "HtmlToXamlConverter");

            // Create current properties to be used by children as inherited properties, set local properties
            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlTableElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            // TODO: process localProperties for tables to override defaults, decide cell spacing defaults

            // Check if the table contains only one cell - we want to take only its content
            XmlElement singleCell = GetCellFromSingleCellTable(htmlTableElement);

            if (singleCell != null)
            {
                // Need to push skipped table elements onto sourceContext
                sourceContext.Add(singleCell);

                // Add the cell's content directly to parent
                for (XmlNode htmlChildNode = singleCell.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode != null ? htmlChildNode.NextSibling : null)
                {
                    htmlChildNode = AddBlock(xamlParentElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
                }

                Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == singleCell, "HtmlToXamlConverter");
                sourceContext.RemoveAt(sourceContext.Count - 1);
            }
            else
            {
                // Create xamlTableElement
                XmlElement xamlTableElement = xamlParentElement.OwnerDocument.CreateElement(null, XamlTable, xamlNamespace);

                // Analyze table structure for column widths and ROWSPAN attributes
                ArrayList columnStarts = AnalyzeTableStructure(htmlTableElement, stylesheet);

                // Process COLGROUP & COL elements
                AddColumnInformation(htmlTableElement, xamlTableElement, columnStarts, currentProperties, stylesheet, sourceContext);

                // Process table body - TBODY and TR elements
                XmlNode htmlChildNode = htmlTableElement.FirstChild;

                while (htmlChildNode != null)
                {
                    string htmlChildName = htmlChildNode.LocalName.ToLower();

                    // Process the element
                    if (htmlChildName == "tbody" || htmlChildName == "thead" || htmlChildName == "tfoot")
                    {
                        // Add more special processing for TableHeader and TableFooter
                        XmlElement xamlTableBodyElement = xamlTableElement.OwnerDocument.CreateElement(null, XamlTableRowGroup, xamlNamespace);
                        xamlTableElement.AppendChild(xamlTableBodyElement);

                        sourceContext.Add((XmlElement)htmlChildNode);

                        // Get properties of Html TBODY element
                        Hashtable tbodyElementLocalProperties;
                        Hashtable tbodyElementCurrentProperties = GetElementProperties((XmlElement)htmlChildNode, currentProperties, out tbodyElementLocalProperties, stylesheet, sourceContext);
                        //// TODO: apply local properties for TBODY

                        // Process children of htmlChildNode, which is TBODY, for TR elements
                        AddTableRowsToTableBody(xamlTableBodyElement, htmlChildNode.FirstChild, tbodyElementCurrentProperties, columnStarts, stylesheet, sourceContext);
                        if (xamlTableBodyElement.HasChildNodes)
                        {
                            xamlTableElement.AppendChild(xamlTableBodyElement);
                            //// else: if there is no TRs in this TBODY, we simply ignore it
                        }

                        Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlChildNode, "HtmlToXamlConverter");
                        sourceContext.RemoveAt(sourceContext.Count - 1);

                        htmlChildNode = htmlChildNode.NextSibling;
                    }
                    else if (htmlChildName == "tr")
                    {
                        // TBODY is not present, but TR element is present. Tr is wrapped in TBODY
                        XmlElement xamlTableBodyElement = xamlTableElement.OwnerDocument.CreateElement(null, XamlTableRowGroup, xamlNamespace);

                        // We use currentProperties of xamlTableElement when adding rows since the TBODY element is artificially created and has
                        // no properties of its own
                        htmlChildNode = AddTableRowsToTableBody(xamlTableBodyElement, htmlChildNode, currentProperties, columnStarts, stylesheet, sourceContext);
                        if (xamlTableBodyElement.HasChildNodes)
                        {
                            xamlTableElement.AppendChild(xamlTableBodyElement);
                        }
                    }
                    else
                    {
                        // Element is not TBODY or TR. Ignore it.
                        // TODO: add processing for thead, tfoot elements and recovery for TD elements
                        htmlChildNode = htmlChildNode.NextSibling;
                    }
                }

                if (xamlTableElement.HasChildNodes)
                {
                    xamlParentElement.AppendChild(xamlTableElement);
                }
            }
        }

        /// <summary>
        /// Get Cell From Single Cell Table
        /// </summary>
        /// <param name="htmlTableElement">HTML table element</param>
        /// <returns>XML element</returns>
        private static XmlElement GetCellFromSingleCellTable(XmlElement htmlTableElement)
        {
            XmlElement singleCell = null;

            for (XmlNode tableChild = htmlTableElement.FirstChild; tableChild != null; tableChild = tableChild.NextSibling)
            {
                string elementName = tableChild.LocalName.ToLower();
                if (elementName == "tbody" || elementName == "thead" || elementName == "tfoot")
                {
                    if (singleCell != null)
                    {
                        return null;
                    }

                    for (XmlNode tbodyChild = tableChild.FirstChild; tbodyChild != null; tbodyChild = tbodyChild.NextSibling)
                    {
                        if (tbodyChild.LocalName.ToLower() == "tr")
                        {
                            if (singleCell != null)
                            {
                                return null;
                            }

                            for (XmlNode child = tbodyChild.FirstChild; child != null; child = child.NextSibling)
                            {
                                string cellName = child.LocalName.ToLower();
                                if (cellName == "td" || cellName == "th")
                                {
                                    if (singleCell != null)
                                    {
                                        return null;
                                    }

                                    singleCell = (XmlElement)child;
                                }
                            }
                        }
                    }
                }
                else if (tableChild.LocalName.ToLower() == "tr")
                {
                    if (singleCell != null)
                    {
                        return null;
                    }

                    for (XmlNode child = tableChild.FirstChild; child != null; child = child.NextSibling)
                    {
                        string cellName = child.LocalName.ToLower();
                        if (cellName == "td" || cellName == "th")
                        {
                            if (singleCell != null)
                            {
                                return null;
                            }

                            singleCell = (XmlElement)child;
                        }
                    }
                }
            }

            return singleCell;
        }

        /// <summary>
        /// Processes the information about table columns - COLGROUP and COL html elements.
        /// </summary>
        /// <param name="htmlTableElement">
        /// XmlElement representing a source html table.
        /// </param>
        /// <param name="xamlTableElement">
        /// XmlElement representing a resulting XAML table.
        /// </param>
        /// <param name="columnStartsAllRows">
        /// Array of doubles - column start coordinates.
        /// Can be null, which means that column size information is not available
        /// and we must use source COLGROUP/COL information.
        /// In case when it's not null, we will ignore source COLGROUP/COL information.
        /// </param>
        /// <param name="currentProperties">Current properties</param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddColumnInformation(XmlElement htmlTableElement, XmlElement xamlTableElement, ArrayList columnStartsAllRows, Hashtable currentProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Add column information
            if (columnStartsAllRows != null)
            {
                // We have consistent information derived from table cells; use it
                // The last element in columnStarts represents the end of the table
                for (int columnIndex = 0; columnIndex < columnStartsAllRows.Count - 1; columnIndex++)
                {
                    XmlElement xamlColumnElement;

                    xamlColumnElement = xamlTableElement.OwnerDocument.CreateElement(null, XamlTableColumn, xamlNamespace);
                    xamlColumnElement.SetAttribute(XamlWidth, ((double)columnStartsAllRows[columnIndex + 1] - (double)columnStartsAllRows[columnIndex]).ToString());
                    xamlTableElement.AppendChild(xamlColumnElement);
                }
            }
            else
            {
                // We do not have consistent information from table cells;
                // Translate blindly COLGROUPs from html.
                for (XmlNode htmlChildNode = htmlTableElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
                {
                    if (htmlChildNode.LocalName.ToLower() == "colgroup")
                    {
                        // TODO: add column width information to this function as a parameter and process it
                        AddTableColumnGroup(xamlTableElement, (XmlElement)htmlChildNode, currentProperties, stylesheet, sourceContext);
                    }
                    else if (htmlChildNode.LocalName.ToLower() == "col")
                    {
                        AddTableColumn(xamlTableElement, (XmlElement)htmlChildNode, currentProperties, stylesheet, sourceContext);
                    }
                    else if (htmlChildNode is XmlElement)
                    {
                        // Some element which belongs to table body. Stop column loop.
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Converts HTML COLGROUP element into XAML TableColumnGroup element, and appends it to the parent
        /// XAML Table Element
        /// </summary>
        /// <param name="xamlTableElement">
        /// XmlElement representing XAML Table element to which the converted column group should be added
        /// </param>
        /// <param name="htmlColgroupElement">
        /// XmlElement representing Html COLGROUP element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// Properties inherited from parent context
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddTableColumnGroup(XmlElement xamlTableElement, XmlElement htmlColgroupElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlColgroupElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            // TODO: process local properties for COLGROUP

            // Process children of COLGROUP. Colgroup may contain only COL elements.
            for (XmlNode htmlNode = htmlColgroupElement.FirstChild; htmlNode != null; htmlNode = htmlNode.NextSibling)
            {
                if (htmlNode is XmlElement && htmlNode.LocalName.ToLower() == "col")
                {
                    AddTableColumn(xamlTableElement, (XmlElement)htmlNode, currentProperties, stylesheet, sourceContext);
                }
            }
        }

        /// <summary>
        /// Converts HTML COL Element into XAML TableColumn element, and appends it to the parent
        /// XAML Table Column Group Element
        /// </summary>
        /// <param name="xamlTableElement">XAML table element</param>
        /// <param name="htmlColElement">
        /// XmlElement representing Html COL element to be converted
        /// </param>
        /// <param name="inheritedProperties">
        /// properties inherited from parent context
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddTableColumn(XmlElement xamlTableElement, XmlElement htmlColElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            Hashtable localProperties;
            Hashtable currentProperties = GetElementProperties(htmlColElement, inheritedProperties, out localProperties, stylesheet, sourceContext);

            XmlElement xamlTableColumnElement = xamlTableElement.OwnerDocument.CreateElement(null, XamlTableColumn, xamlNamespace);

            //// TODO: process local properties for TableColumn element

            // Col is an empty element, with no subtree
            xamlTableElement.AppendChild(xamlTableColumnElement);
        }

        /// <summary>
        /// Adds TableRow elements to XAML Table Body Element. The rows are converted from Html TR elements that
        /// may be the children of an Html TBODY element or an Html table element with TBODY missing
        /// </summary>
        /// <param name="xamlTableBodyElement">
        /// XmlElement representing XAML TableRowGroup element to which the converted rows should be added
        /// </param>
        /// <param name="htmlTRStartNode">
        /// XmlElement representing the first TR child of the TBODY element to be read
        /// </param>
        /// <param name="currentProperties">
        /// Hash table representing current properties of the TBODY element that are generated and applied in the
        /// AddTable function; to be used as inheritedProperties when adding TR elements
        /// </param>
        /// <param name="columnStarts">Column starts</param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        /// <returns>
        /// XmlNode representing the current position of the iterator among TR elements
        /// </returns>
        private static XmlNode AddTableRowsToTableBody(XmlElement xamlTableBodyElement, XmlNode htmlTRStartNode, Hashtable currentProperties, ArrayList columnStarts, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Parameter validation
            Debug.Assert(xamlTableBodyElement.LocalName == XamlTableRowGroup, "HtmlToXamlConverter");
            Debug.Assert(currentProperties != null, "HtmlToXamlConverter");

            // Initialize child node for iteratimg through children to the first TR element
            XmlNode htmlChildNode = htmlTRStartNode;
            ArrayList activeRowSpans = null;
            if (columnStarts != null)
            {
                activeRowSpans = new ArrayList();
                InitializeActiveRowSpans(activeRowSpans, columnStarts.Count);
            }

            while (htmlChildNode != null && htmlChildNode.LocalName.ToLower() != "tbody")
            {
                if (htmlChildNode.LocalName.ToLower() == "tr")
                {
                    XmlElement xamlTableRowElement = xamlTableBodyElement.OwnerDocument.CreateElement(null, XamlTableRow, xamlNamespace);

                    sourceContext.Add((XmlElement)htmlChildNode);

                    // Get TR element properties
                    Hashtable elementLocalProperties;
                    Hashtable elementCurrentProperties = GetElementProperties((XmlElement)htmlChildNode, currentProperties, out elementLocalProperties, stylesheet, sourceContext);
                    
                    //// TODO: apply local properties to TR element

                    AddTableCellsToTableRow(xamlTableRowElement, htmlChildNode.FirstChild, elementCurrentProperties, columnStarts, activeRowSpans, stylesheet, sourceContext);
                    if (xamlTableRowElement.HasChildNodes)
                    {
                        xamlTableBodyElement.AppendChild(xamlTableRowElement);
                    }

                    Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlChildNode, "HtmlToXamlConverter");
                    sourceContext.RemoveAt(sourceContext.Count - 1);

                    // Advance
                    htmlChildNode = htmlChildNode.NextSibling;
                }
                else if (htmlChildNode.LocalName.ToLower() == "td")
                {
                    // Tr element is not present. We create one and add TD elements to it
                    XmlElement xamlTableRowElement = xamlTableBodyElement.OwnerDocument.CreateElement(null, XamlTableRow, xamlNamespace);

                    // This is incorrect formatting and the column starts should not be set in this case
                    Debug.Assert(columnStarts == null, "HtmlToXamlConverter");

                    htmlChildNode = AddTableCellsToTableRow(xamlTableRowElement, htmlChildNode, currentProperties, columnStarts, activeRowSpans, stylesheet, sourceContext);
                    if (xamlTableRowElement.HasChildNodes)
                    {
                        xamlTableBodyElement.AppendChild(xamlTableRowElement);
                    }
                }
                else
                {
                    // Not a TR or TD  element. Ignore it.
                    //// TODO: consider better recovery here
                    htmlChildNode = htmlChildNode.NextSibling;
                }
            }

            return htmlChildNode;
        }

        /// <summary>
        /// Adds TableCell elements to XAML Table Row Element.
        /// </summary>
        /// <param name="xamlTableRowElement">
        /// XmlElement representing XAML TableRow element to which the converted cells should be added
        /// </param>
        /// <param name="htmlTDStartNode">
        /// XmlElement representing the child of TR or TBODY element from which we should start adding TD elements
        /// </param>
        /// <param name="currentProperties">
        /// properties of the current html TR element to which cells are to be added
        /// </param>
        /// <param name="columnStarts">Column starts</param>
        /// <param name="activeRowSpans">Active Row Spans</param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        /// <returns>
        /// XmlElement representing the current position of the iterator among the children of the parent Html TBODY/TR element
        /// </returns>
        private static XmlNode AddTableCellsToTableRow(XmlElement xamlTableRowElement, XmlNode htmlTDStartNode, Hashtable currentProperties, ArrayList columnStarts, ArrayList activeRowSpans, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // parameter validation
            Debug.Assert(xamlTableRowElement.LocalName == XamlTableRow, "HtmlToXamlConverter");
            Debug.Assert(currentProperties != null, "HtmlToXamlConverter");
            if (columnStarts != null)
            {
                Debug.Assert(activeRowSpans.Count == columnStarts.Count, "HtmlToXamlConverter");
            }

            XmlNode htmlChildNode = htmlTDStartNode;
            double columnStart = 0;
            double columnWidth = 0;
            int columnIndex = 0;
            int columnSpan = 0;

            while (htmlChildNode != null && htmlChildNode.LocalName.ToLower() != "tr" && htmlChildNode.LocalName.ToLower() != "tbody" && htmlChildNode.LocalName.ToLower() != "thead" && htmlChildNode.LocalName.ToLower() != "tfoot")
            {
                if (htmlChildNode.LocalName.ToLower() == "td" || htmlChildNode.LocalName.ToLower() == "th")
                {
                    XmlElement xamlTableCellElement = xamlTableRowElement.OwnerDocument.CreateElement(null, XamlTableCell, xamlNamespace);

                    sourceContext.Add((XmlElement)htmlChildNode);

                    Hashtable elementLocalProperties;
                    Hashtable elementCurrentProperties = GetElementProperties((XmlElement)htmlChildNode, currentProperties, out elementLocalProperties, stylesheet, sourceContext);

                    // TODO: determine if localProperties can be used instead of htmlChildNode in this call, and if they can,
                    // make necessary changes and use them instead.
                    ApplyPropertiesToTableCellElement((XmlElement)htmlChildNode, xamlTableCellElement);

                    if (columnStarts != null)
                    {
                        Debug.Assert(columnIndex < columnStarts.Count - 1, "HtmlToXamlConverter");
                        while (columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex] > 0)
                        {
                            activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex] - 1;
                            Debug.Assert((int)activeRowSpans[columnIndex] >= 0, "HtmlToXamlConverter");
                            columnIndex++;
                        }

                        Debug.Assert(columnIndex < columnStarts.Count - 1, "HtmlToXamlConverter");
                        columnStart = (double)columnStarts[columnIndex];
                        columnWidth = GetColumnWidth((XmlElement)htmlChildNode);
                        columnSpan = CalculateColumnSpan(columnIndex, columnWidth, columnStarts);
                        int rowSpan = GetRowSpan((XmlElement)htmlChildNode);

                        // Column cannot have no span
                        Debug.Assert(columnSpan > 0, "HtmlToXamlConverter");
                        Debug.Assert(columnIndex + columnSpan < columnStarts.Count, "HtmlToXamlConverter");

                        xamlTableCellElement.SetAttribute(XamlTableCellColumnSpan, columnSpan.ToString());

                        // Apply row span
                        for (int spannedColumnIndex = columnIndex; spannedColumnIndex < columnIndex + columnSpan; spannedColumnIndex++)
                        {
                            Debug.Assert(spannedColumnIndex < activeRowSpans.Count, "HtmlToXamlConverter");
                            activeRowSpans[spannedColumnIndex] = rowSpan - 1;
                            Debug.Assert((int)activeRowSpans[spannedColumnIndex] >= 0, "HtmlToXamlConverter");
                        }

                        columnIndex = columnIndex + columnSpan;
                    }

                    AddDataToTableCell(xamlTableCellElement, htmlChildNode.FirstChild, elementCurrentProperties, stylesheet, sourceContext);
                    if (xamlTableCellElement.HasChildNodes)
                    {
                        xamlTableRowElement.AppendChild(xamlTableCellElement);
                    }

                    Debug.Assert(sourceContext.Count > 0 && sourceContext[sourceContext.Count - 1] == htmlChildNode, "HtmlToXamlConverter");
                    sourceContext.RemoveAt(sourceContext.Count - 1);

                    htmlChildNode = htmlChildNode.NextSibling;
                }
                else
                {
                    // Not TD element. Ignore it.
                    // TODO: Consider better recovery
                    htmlChildNode = htmlChildNode.NextSibling;
                }
            }

            return htmlChildNode;
        }

        /// <summary>
        /// adds table cell data to XAML Table Cell Element
        /// </summary>
        /// <param name="xamlTableCellElement">
        /// XmlElement representing XAML TableCell element to which the converted data should be added
        /// </param>
        /// <param name="htmlDataStartNode">
        /// XmlElement representing the start element of data to be added to XAML Table Cell Element
        /// </param>
        /// <param name="currentProperties">
        /// Current properties for the html TD/TH element corresponding to XAML Table Cell Element
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        private static void AddDataToTableCell(XmlElement xamlTableCellElement, XmlNode htmlDataStartNode, Hashtable currentProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Parameter validation
            Debug.Assert(xamlTableCellElement.LocalName == XamlTableCell, "HtmlToXamlConverter");
            Debug.Assert(currentProperties != null, "HtmlToXamlConverter");

            for (XmlNode htmlChildNode = htmlDataStartNode; htmlChildNode != null; htmlChildNode = htmlChildNode != null ? htmlChildNode.NextSibling : null)
            {
                // Process a new html element and add it to the TD element
                htmlChildNode = AddBlock(xamlTableCellElement, htmlChildNode, currentProperties, stylesheet, sourceContext);
            }
        }

        /// <summary>
        /// Performs a parsing pass over a table to read information about column width and ROWSPAN attributes. This information
        /// is used to determine the starting point of each column.
        /// </summary>
        /// <param name="htmlTableElement">
        /// XmlElement representing Html table whose structure is to be analyzed
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <returns>
        /// ArrayList of type double which contains the function output. If analysis is successful, this ArrayList contains
        /// all the points which are the starting position of any column in the table, ordered from left to right.
        /// In case if analysis was impossible we return null.
        /// </returns>
        private static ArrayList AnalyzeTableStructure(XmlElement htmlTableElement, CssStylesheet stylesheet)
        {
            // Parameter validation
            Debug.Assert(htmlTableElement.LocalName.ToLower() == "table", "HtmlToXamlConverter");
            if (!htmlTableElement.HasChildNodes)
            {
                return null;
            }

            bool columnWidthsAvailable = true;

            ArrayList columnStarts = new ArrayList();
            ArrayList activeRowSpans = new ArrayList();
            Debug.Assert(columnStarts.Count == activeRowSpans.Count, "HtmlToXamlConverter");

            XmlNode htmlChildNode = htmlTableElement.FirstChild;
            double tableWidth = 0;  // Keep track of table width which is the width of its widest row

            // Analyze TBODY and TR elements
            while (htmlChildNode != null && columnWidthsAvailable)
            {
                Debug.Assert(columnStarts.Count == activeRowSpans.Count, "HtmlToXamlConverter");

                switch (htmlChildNode.LocalName.ToLower())
                {
                    case "tbody":
                        // TBODY element, we should analyze its children for trows
                        double tbodyWidth = AnalyzeTbodyStructure((XmlElement)htmlChildNode, columnStarts, activeRowSpans, tableWidth, stylesheet);
                        if (tbodyWidth > tableWidth)
                        {
                            // Table width must be increased to supported newly added wide row
                            tableWidth = tbodyWidth;
                        }
                        else if (tbodyWidth == 0)
                        {
                            // TBODY analysis may return 0, probably due to unprocessable format.
                            // We should also fail.
                            columnWidthsAvailable = false; // interrupt the analysis
                        }

                        break;
                    case "tr":
                        // Table row. Analyze column structure within row directly
                        double width = AnalyzeTRStructure((XmlElement)htmlChildNode, columnStarts, activeRowSpans, tableWidth, stylesheet);
                        if (width > tableWidth)
                        {
                            tableWidth = width;
                        }
                        else if (width == 0)
                        {
                            columnWidthsAvailable = false; // interrupt the analysis
                        }

                        break;
                    case "td":
                        // Incorrect formatting, too deep to analyze at this level. Return null.
                        // TODO: implement analysis at this level, possibly by creating a new TR
                        columnWidthsAvailable = false; // interrupt the analysis
                        break;
                    default:
                        // Element should not occur directly in table. Ignore it.
                        break;
                }

                htmlChildNode = htmlChildNode.NextSibling;
            }

            if (columnWidthsAvailable)
            {
                // Add an item for whole table width
                columnStarts.Add(tableWidth);
                VerifyColumnStartsAscendingOrder(columnStarts);
            }
            else
            {
                columnStarts = null;
            }

            return columnStarts;
        }

        /// <summary>
        /// Performs a parsing pass over a TBODY to read information about column width and ROWSPAN attributes. Information read about width
        /// attributes is stored in the reference ArrayList parameter columnStarts, which contains a list of all starting
        /// positions of all columns in the table, ordered from left to right. Row spans are taken into consideration when
        /// computing column starts
        /// </summary>
        /// <param name="htmlTbodyElement">
        /// XmlElement representing Html TBODY whose structure is to be analyzed
        /// </param>
        /// <param name="columnStarts">
        /// ArrayList of type double which contains the function output. If analysis fails, this parameter is set to null
        /// </param>
        /// <param name="activeRowSpans">Active row spans</param>
        /// <param name="tableWidth">
        /// Current width of the table. This is used to determine if a new column when added to the end of table should
        /// come after the last column in the table or is actually splitting the last column in two. If it is only splitting
        /// the last column it should inherit row span for that column
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <returns>
        /// Calculated width of a TBODY.
        /// In case of non-analyzable column width structure return 0;
        /// </returns>
        private static double AnalyzeTbodyStructure(XmlElement htmlTbodyElement, ArrayList columnStarts, ArrayList activeRowSpans, double tableWidth, CssStylesheet stylesheet)
        {
            // Parameter validation
            Debug.Assert(htmlTbodyElement.LocalName.ToLower() == "tbody", "HtmlToXamlConverter");
            Debug.Assert(columnStarts != null, "HtmlToXamlConverter");

            double tbodyWidth = 0;
            bool columnWidthsAvailable = true;

            if (!htmlTbodyElement.HasChildNodes)
            {
                return tbodyWidth;
            }

            // Set active row spans to 0 - thus ignoring row spans crossing TBODY boundaries
            ClearActiveRowSpans(activeRowSpans);

            XmlNode htmlChildNode = htmlTbodyElement.FirstChild;

            // Analyze TR elements
            while (htmlChildNode != null && columnWidthsAvailable)
            {
                switch (htmlChildNode.LocalName.ToLower())
                {
                    case "tr":
                        double width = AnalyzeTRStructure((XmlElement)htmlChildNode, columnStarts, activeRowSpans, tbodyWidth, stylesheet);
                        if (width > tbodyWidth)
                        {
                            tbodyWidth = width;
                        }

                        break;
                    case "td":
                        columnWidthsAvailable = false; // interrupt the analysis
                        break;
                    default:
                        break;
                }

                htmlChildNode = htmlChildNode.NextSibling;
            }

            // Set active row spans to 0 - thus ignoring row spans crossing TBODY boundaries
            ClearActiveRowSpans(activeRowSpans);

            return columnWidthsAvailable ? tbodyWidth : 0;
        }

        /// <summary>
        /// Performs a parsing pass over a TR element to read information about column width and ROWSPAN attributes.
        /// </summary>
        /// <param name="htmlTRElement">
        /// XmlElement representing Html TR element whose structure is to be analyzed
        /// </param>
        /// <param name="columnStarts">
        /// ArrayList of type double which contains the function output. If analysis is successful, this ArrayList contains
        /// all the points which are the starting position of any column in the TR, ordered from left to right. If analysis fails,
        /// the ArrayList is set to null
        /// </param>
        /// <param name="activeRowSpans">
        /// ArrayList representing all columns currently spanned by an earlier row span attribute. These columns should
        /// not be used for data in this row. The ArrayList actually contains notation for all columns in the table, if the
        /// active row span is set to 0 that column is not presently spanned but if it is > 0 the column is presently spanned
        /// </param>
        /// <param name="tableWidth">
        /// Double value representing the current width of the table.
        /// Return 0 if analysis was unsuccessful.
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <returns>TR row width</returns>
        private static double AnalyzeTRStructure(XmlElement htmlTRElement, ArrayList columnStarts, ArrayList activeRowSpans, double tableWidth, CssStylesheet stylesheet)
        {
            double columnWidth;

            // Parameter validation
            Debug.Assert(htmlTRElement.LocalName.ToLower() == "tr", "HtmlToXamlConverter");
            Debug.Assert(columnStarts != null, "HtmlToXamlConverter");
            Debug.Assert(activeRowSpans != null, "HtmlToXamlConverter");
            Debug.Assert(columnStarts.Count == activeRowSpans.Count, "HtmlToXamlConverter");

            if (!htmlTRElement.HasChildNodes)
            {
                return 0;
            }

            bool columnWidthsAvailable = true;

            double columnStart = 0; // starting position of current column
            XmlNode htmlChildNode = htmlTRElement.FirstChild;
            int columnIndex = 0;
            double width = 0;

            // Skip spanned columns to get to real column start
            if (columnIndex < activeRowSpans.Count)
            {
                Debug.Assert((double)columnStarts[columnIndex] >= columnStart, "HtmlToXamlConverter");
                if ((double)columnStarts[columnIndex] == columnStart)
                {
                    // The new column may be in a spanned area
                    while (columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex] > 0)
                    {
                        activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex] - 1;
                        Debug.Assert((int)activeRowSpans[columnIndex] >= 0, "HtmlToXamlConverter");
                        columnIndex++;
                        columnStart = (double)columnStarts[columnIndex];
                    }
                }
            }

            while (htmlChildNode != null && columnWidthsAvailable)
            {
                Debug.Assert(columnStarts.Count == activeRowSpans.Count, "HtmlToXamlConverter");

                VerifyColumnStartsAscendingOrder(columnStarts);

                switch (htmlChildNode.LocalName.ToLower())
                {
                    case "td":
                        Debug.Assert(columnIndex <= columnStarts.Count, "HtmlToXamlConverter");
                        if (columnIndex < columnStarts.Count)
                        {
                            Debug.Assert(columnStart <= (double)columnStarts[columnIndex], "HtmlToXamlConverter");
                            if (columnStart < (double)columnStarts[columnIndex])
                            {
                                columnStarts.Insert(columnIndex, columnStart);

                                // There can be no row spans now - the column data will appear here
                                // Row spans may appear only during the column analysis
                                activeRowSpans.Insert(columnIndex, 0);
                            }
                        }
                        else
                        {
                            // Column start is greater than all previous starts. Row span must still be 0 because
                            // we are either adding after another column of the same row, in which case it should not inherit
                            // the previous column's span. Otherwise we are adding after the last column of some previous
                            // row, and assuming the table widths line up, we should not be spanned by it. If there is
                            // an incorrect tbale structure where a columns starts in the middle of a row span, we do not
                            // guarantee correct output
                            columnStarts.Add(columnStart);
                            activeRowSpans.Add(0);
                        }

                        columnWidth = GetColumnWidth((XmlElement)htmlChildNode);
                        if (columnWidth != -1)
                        {
                            int nextColumnIndex;
                            int rowSpan = GetRowSpan((XmlElement)htmlChildNode);

                            nextColumnIndex = GetNextColumnIndex(columnIndex, columnWidth, columnStarts, activeRowSpans);
                            if (nextColumnIndex != -1)
                            {
                                // Entire column width can be processed without hitting conflicting row span. This means that
                                // column widths line up and we can process them
                                Debug.Assert(nextColumnIndex <= columnStarts.Count, "HtmlToXamlConverter");

                                // Apply row span to affected columns
                                for (int spannedColumnIndex = columnIndex; spannedColumnIndex < nextColumnIndex; spannedColumnIndex++)
                                {
                                    activeRowSpans[spannedColumnIndex] = rowSpan - 1;
                                    Debug.Assert((int)activeRowSpans[spannedColumnIndex] >= 0, "HtmlToXamlConverter");
                                }

                                columnIndex = nextColumnIndex;

                                // Calculate columnsStart for the next cell
                                columnStart = columnStart + columnWidth;

                                if (columnIndex < activeRowSpans.Count)
                                {
                                    Debug.Assert((double)columnStarts[columnIndex] >= columnStart, "HtmlToXamlConverter");
                                    if ((double)columnStarts[columnIndex] == columnStart)
                                    {
                                        // The new column may be in a spanned area
                                        while (columnIndex < activeRowSpans.Count && (int)activeRowSpans[columnIndex] > 0)
                                        {
                                            activeRowSpans[columnIndex] = (int)activeRowSpans[columnIndex] - 1;
                                            Debug.Assert((int)activeRowSpans[columnIndex] >= 0, "HtmlToXamlConverter");
                                            columnIndex++;
                                            columnStart = (double)columnStarts[columnIndex];
                                        }
                                    }
                                    //// else: the new column does not start at the same time as a pre existing column
                                    //// so we don't have to check it for active row spans, it starts in the middle
                                    //// of another column which has been checked already by the GetNextColumnIndex function
                                }
                            }
                            else
                            {
                                // Full column width cannot be processed without a pre existing row span.
                                // We cannot analyze widths
                                columnWidthsAvailable = false;
                            }
                        }
                        else
                        {
                            // Incorrect column width, stop processing
                            columnWidthsAvailable = false;
                        }

                        break;
                    default:
                        break;
                }

                htmlChildNode = htmlChildNode.NextSibling;
            }

            // The width of the TR element is the position at which it's last TD element ends, which is calculated in
            // the columnStart value after each TD element is processed
            if (columnWidthsAvailable)
            {
                width = columnStart;
            }
            else
            {
                width = 0;
            }

            return width;
        }

        /// <summary>
        /// Gets row span attribute from htmlTDElement. Returns an integer representing the value of the ROWSPAN attribute.
        /// Default value if attribute is not specified or if it is invalid is 1
        /// </summary>
        /// <param name="htmlTDElement">
        /// Html TD element to be searched for ROWSPAN attribute
        /// </param>
        /// <returns>Row span</returns>
        private static int GetRowSpan(XmlElement htmlTDElement)
        {
            string rowSpanAsString;
            int rowSpan;

            rowSpanAsString = GetAttribute((XmlElement)htmlTDElement, "rowspan");
            if (rowSpanAsString != null)
            {
                if (!int.TryParse(rowSpanAsString, out rowSpan))
                {
                    // Ignore invalid value of ROWSPAN; treat it as 1
                    rowSpan = 1;
                }
            }
            else
            {
                // No row span, default is 1
                rowSpan = 1;
            }

            return rowSpan;
        }

        /// <summary>
        /// Gets index at which a column should be inserted into the columnStarts ArrayList. This is
        /// decided by the value columnStart. The columnStarts ArrayList is ordered in ascending order.
        /// Returns an integer representing the index at which the column should be inserted
        /// </summary>
        /// <param name="columnIndex">
        /// Integer representing the current column index. This acts as a clue while finding the insertion index.
        /// If the value of columnStarts at columnIndex is the same as columnStart, then this position already exists
        /// in the array and we can just return columnIndex.
        /// </param>
        /// <param name="columnWidth">Column width</param>
        /// <param name="columnStarts">Array list representing starting coordinates of all columns in the table</param>
        /// <param name="activeRowSpans">Active row spans</param>
        /// <returns>Spanned column index</returns>
        private static int GetNextColumnIndex(int columnIndex, double columnWidth, ArrayList columnStarts, ArrayList activeRowSpans)
        {
            double columnStart;
            int spannedColumnIndex;

            // Parameter validation
            Debug.Assert(columnStarts != null, "HtmlToXamlConverter");
            Debug.Assert(0 <= columnIndex && columnIndex <= columnStarts.Count, "HtmlToXamlConverter");
            Debug.Assert(columnWidth > 0, "HtmlToXamlConverter");

            columnStart = (double)columnStarts[columnIndex];
            spannedColumnIndex = columnIndex + 1;

            while (spannedColumnIndex < columnStarts.Count && (double)columnStarts[spannedColumnIndex] < columnStart + columnWidth && spannedColumnIndex != -1)
            {
                if ((int)activeRowSpans[spannedColumnIndex] > 0)
                {
                    // The current column should span this area, but something else is already spanning it
                    // Not analyzable
                    spannedColumnIndex = -1;
                }
                else
                {
                    spannedColumnIndex++;
                }
            }

            return spannedColumnIndex;
        }

        /// <summary>
        /// Used for clearing activeRowSpans array in the beginning/end of each TBODY
        /// </summary>
        /// <param name="activeRowSpans">
        /// ArrayList representing currently active row spans
        /// </param>
        private static void ClearActiveRowSpans(ArrayList activeRowSpans)
        {
            for (int columnIndex = 0; columnIndex < activeRowSpans.Count; columnIndex++)
            {
                activeRowSpans[columnIndex] = 0;
            }
        }

        /// <summary>
        /// Used for initializing activeRowSpans array in the before adding rows to TBODY element
        /// </summary>
        /// <param name="activeRowSpans">
        /// ArrayList representing currently active row spans
        /// </param>
        /// <param name="count">
        /// Size to be give to array list
        /// </param>
        private static void InitializeActiveRowSpans(ArrayList activeRowSpans, int count)
        {
            for (int columnIndex = 0; columnIndex < count; columnIndex++)
            {
                activeRowSpans.Add(0);
            }
        }

        /// <summary>
        /// Calculates width of next TD element based on starting position of current element and it's width, which
        /// is calculated by the function
        /// </summary>
        /// <param name="htmlTDElement">
        /// XmlElement representing Html TD element whose width is to be read
        /// </param>
        /// <param name="columnStart">
        /// Starting position of current column
        /// </param>
        /// <returns>Next column start</returns>
        private static double GetNextColumnStart(XmlElement htmlTDElement, double columnStart)
        {
            double columnWidth;
            double nextColumnStart;

            // Parameter validation
            Debug.Assert(htmlTDElement.LocalName.ToLower() == "td" || htmlTDElement.LocalName.ToLower() == "th", "HtmlToXamlConverter");
            Debug.Assert(columnStart >= 0, "HtmlToXamlConverter");

            nextColumnStart = -1;  // -1 indicates inability to calculate columnStart width

            columnWidth = GetColumnWidth(htmlTDElement);

            if (columnWidth == -1)
            {
                nextColumnStart = -1;
            }
            else
            {
                nextColumnStart = columnStart + columnWidth;
            }

            return nextColumnStart;
        }

        /// <summary>
        /// Get column width
        /// </summary>
        /// <param name="htmlTDElement">HTML TD element</param>
        /// <returns>Column width</returns>
        private static double GetColumnWidth(XmlElement htmlTDElement)
        {
            string columnWidthAsString;
            double columnWidth;

            columnWidthAsString = null;
            columnWidth = -1;

            // Get string valkue for the width
            columnWidthAsString = GetAttribute(htmlTDElement, "width");
            if (columnWidthAsString == null)
            {
                columnWidthAsString = GetCssAttribute(GetAttribute(htmlTDElement, "style"), "width");
            }

            // We do not allow column width to be 0, if specified as 0 we will fail to record it
            if (!TryGetLengthValue(columnWidthAsString, out columnWidth) || columnWidth == 0)
            {
                columnWidth = -1;
            }

            return columnWidth;
        }

        /// <summary>
        /// Calculates column span based the column width and the widths of all other columns. Returns an integer representing
        /// the column span
        /// </summary>
        /// <param name="columnIndex">
        /// Index of the current column
        /// </param>
        /// <param name="columnWidth">
        /// Width of the current column
        /// </param>
        /// <param name="columnStarts">
        /// ArrayList representing starting coordinates of all columns
        /// </param>
        /// <returns>Column span</returns>
        private static int CalculateColumnSpan(int columnIndex, double columnWidth, ArrayList columnStarts)
        {
            // Current status of column width. Indicates the amount of width that has been scanned already
            double columnSpanningValue;
            int columnSpanningIndex;
            int columnSpan;
            double subColumnWidth; // Width of the smallest-grain columns in the table

            Debug.Assert(columnStarts != null, "HtmlToXamlConverter");
            Debug.Assert(columnIndex < columnStarts.Count - 1, "HtmlToXamlConverter");
            Debug.Assert((double)columnStarts[columnIndex] >= 0, "HtmlToXamlConverter");
            Debug.Assert(columnWidth > 0, "HtmlToXamlConverter");

            columnSpanningIndex = columnIndex;
            columnSpanningValue = 0;
            columnSpan = 0;
            subColumnWidth = 0;

            while (columnSpanningValue < columnWidth && columnSpanningIndex < columnStarts.Count - 1)
            {
                subColumnWidth = (double)columnStarts[columnSpanningIndex + 1] - (double)columnStarts[columnSpanningIndex];
                Debug.Assert(subColumnWidth > 0, "HtmlToXamlConverter");
                columnSpanningValue += subColumnWidth;
                columnSpanningIndex++;
            }

            // Now, we have either covered the width we needed to cover or reached the end of the table, in which
            // case the column spans all the columns until the end
            columnSpan = columnSpanningIndex - columnIndex;
            Debug.Assert(columnSpan > 0, "HtmlToXamlConverter");

            return columnSpan;
        }

        /// <summary>
        /// Verifies that values in columnStart, which represent starting coordinates of all columns, are arranged
        /// in ascending order
        /// </summary>
        /// <param name="columnStarts">
        /// ArrayList representing starting coordinates of all columns
        /// </param>
        private static void VerifyColumnStartsAscendingOrder(ArrayList columnStarts)
        {
            Debug.Assert(columnStarts != null, "HtmlToXamlConverter");

            double columnStart;

            columnStart = -0.01;

            for (int columnIndex = 0; columnIndex < columnStarts.Count; columnIndex++)
            {
                Debug.Assert(columnStart < (double)columnStarts[columnIndex], "HtmlToXamlConverter");
                columnStart = (double)columnStarts[columnIndex];
            }
        }

        // .............................................................
        //
        // Attributes and Properties
        //
        // .............................................................

        /// <summary>
        /// Analyzes local properties of Html element, converts them into XAML equivalents, and applies them to XAML element
        /// </summary>
        /// <param name="xamlElement">
        /// XmlElement representing XAML element to which properties are to be applied
        /// </param>
        /// <param name="localProperties">
        /// Hash table representing local properties of Html element that is converted into XAML element
        /// </param>
        /// <param name="isBlock">if is a block</param>
        private static void ApplyLocalProperties(XmlElement xamlElement, Hashtable localProperties, bool isBlock)
        {
            bool marginSet = false;
            string marginTop = "0";
            string marginBottom = "0";
            string marginLeft = "0";
            string marginRight = "0";

            bool paddingSet = false;
            string paddingTop = "0";
            string paddingBottom = "0";
            string paddingLeft = "0";
            string paddingRight = "0";

            string borderColor = null;

            bool borderThicknessSet = false;
            string borderThicknessTop = "0";
            string borderThicknessBottom = "0";
            string borderThicknessLeft = "0";
            string borderThicknessRight = "0";

            IDictionaryEnumerator propertyEnumerator = localProperties.GetEnumerator();
            while (propertyEnumerator.MoveNext())
            {
                switch ((string)propertyEnumerator.Key)
                {
                    case "font-family":
                        // Convert from font-family value list into XAML FontFamily value
                        xamlElement.SetAttribute(XamlFontFamily, (string)propertyEnumerator.Value);
                        break;
                    case "font-style":
                        xamlElement.SetAttribute(XamlFontStyle, (string)propertyEnumerator.Value);
                        break;
                    case "font-variant":
                        // Convert from font-variant into XAML property
                        break;
                    case "font-weight":
                        xamlElement.SetAttribute(XamlFontWeight, (string)propertyEnumerator.Value);
                        break;
                    case "font-size":
                        // Convert from CSS size into FontSize
                        xamlElement.SetAttribute(XamlFontSize, (string)propertyEnumerator.Value);
                        break;
                    case "color":
                        SetPropertyValue(xamlElement, TextElement.ForegroundProperty, (string)propertyEnumerator.Value);
                        break;
                    case "background-color":
                        SetPropertyValue(xamlElement, TextElement.BackgroundProperty, (string)propertyEnumerator.Value);
                        break;
                    case "text-decoration-underline":
                        if (!isBlock)
                        {
                            if ((string)propertyEnumerator.Value == "true")
                            {
                                xamlElement.SetAttribute(XamlTextDecorations, XamlTextDecorationsUnderline);
                            }
                        }

                        break;
                    case "text-decoration-none":
                    case "text-decoration-overline":
                    case "text-decoration-line-through":
                    case "text-decoration-blink":
                        // Convert from all other text-decorations values
                        if (!isBlock)
                        {
                        }

                        break;
                    case "text-transform":
                        // Convert from text-transform into XAML property
                        break;

                    case "text-indent":
                        if (isBlock)
                        {
                            xamlElement.SetAttribute(XamlTextIndent, (string)propertyEnumerator.Value);
                        }

                        break;

                    case "text-align":
                        if (isBlock)
                        {
                            xamlElement.SetAttribute(XamlTextAlignment, (string)propertyEnumerator.Value);
                        }

                        break;

                    case "width":
                    case "height":
                        // Decide what to do with width and height propeties
                        break;

                    case "margin-top":
                        marginSet = true;
                        marginTop = (string)propertyEnumerator.Value;
                        break;
                    case "margin-right":
                        marginSet = true;
                        marginRight = (string)propertyEnumerator.Value;
                        break;
                    case "margin-bottom":
                        marginSet = true;
                        marginBottom = (string)propertyEnumerator.Value;
                        break;
                    case "margin-left":
                        marginSet = true;
                        marginLeft = (string)propertyEnumerator.Value;
                        break;
                    case "padding-top":
                        paddingSet = true;
                        paddingTop = (string)propertyEnumerator.Value;
                        break;
                    case "padding-right":
                        paddingSet = true;
                        paddingRight = (string)propertyEnumerator.Value;
                        break;
                    case "padding-bottom":
                        paddingSet = true;
                        paddingBottom = (string)propertyEnumerator.Value;
                        break;
                    case "padding-left":
                        paddingSet = true;
                        paddingLeft = (string)propertyEnumerator.Value;
                        break;

                    // NOTE: CSS names for elementary border styles have side indications in the middle (top/bottom/left/right)
                    // In our internal notation we intentionally put them at the end - to unify processing in ParseCssRectangleProperty method
                    case "border-color-top":
                        borderColor = (string)propertyEnumerator.Value;
                        break;
                    case "border-color-right":
                        borderColor = (string)propertyEnumerator.Value;
                        break;
                    case "border-color-bottom":
                        borderColor = (string)propertyEnumerator.Value;
                        break;
                    case "border-color-left":
                        borderColor = (string)propertyEnumerator.Value;
                        break;
                    case "border-style-top":
                    case "border-style-right":
                    case "border-style-bottom":
                    case "border-style-left":
                        // Implement conversion from border style
                        break;
                    case "border-width-top":
                        borderThicknessSet = true;
                        borderThicknessTop = (string)propertyEnumerator.Value;
                        break;
                    case "border-width-right":
                        borderThicknessSet = true;
                        borderThicknessRight = (string)propertyEnumerator.Value;
                        break;
                    case "border-width-bottom":
                        borderThicknessSet = true;
                        borderThicknessBottom = (string)propertyEnumerator.Value;
                        break;
                    case "border-width-left":
                        borderThicknessSet = true;
                        borderThicknessLeft = (string)propertyEnumerator.Value;
                        break;

                    case "list-style-type":
                        if (xamlElement.LocalName == XamlList)
                        {
                            string markerStyle;
                            switch (((string)propertyEnumerator.Value).ToLower())
                            {
                                case "disc":
                                    markerStyle = HtmlToXamlConverter.XamlListMarkerStyleDisc;
                                    break;
                                case "circle":
                                    markerStyle = HtmlToXamlConverter.XamlListMarkerStyleCircle;
                                    break;
                                case "none":
                                    markerStyle = HtmlToXamlConverter.XamlListMarkerStyleNone;
                                    break;
                                case "square":
                                    markerStyle = HtmlToXamlConverter.XamlListMarkerStyleSquare;
                                    break;
                                case "box":
                                    markerStyle = HtmlToXamlConverter.XamlListMarkerStyleBox;
                                    break;
                                case "lower-latin":
                                    markerStyle = HtmlToXamlConverter.XamlListMarkerStyleLowerLatin;
                                    break;
                                case "upper-latin":
                                    markerStyle = HtmlToXamlConverter.XamlListMarkerStyleUpperLatin;
                                    break;
                                case "lower-roman":
                                    markerStyle = HtmlToXamlConverter.XamlListMarkerStyleLowerRoman;
                                    break;
                                case "upper-roman":
                                    markerStyle = HtmlToXamlConverter.XamlListMarkerStyleUpperRoman;
                                    break;
                                case "decimal":
                                    markerStyle = HtmlToXamlConverter.XamlListMarkerStyleDecimal;
                                    break;
                                default:
                                    markerStyle = HtmlToXamlConverter.XamlListMarkerStyleDisc;
                                    break;
                            }

                            xamlElement.SetAttribute(HtmlToXamlConverter.XamlListMarkerStyle, markerStyle);
                        }

                        break;
                    case "float":
                    case "clear":
                        if (isBlock)
                        {
                            // Convert float and clear properties
                        }

                        break;

                    case "display":
                        break;
                }
            }

            if (isBlock)
            {
                if (marginSet)
                {
                    ComposeThicknessProperty(xamlElement, XamlMargin, marginLeft, marginRight, marginTop, marginBottom);
                }

                if (paddingSet)
                {
                    ComposeThicknessProperty(xamlElement, XamlPadding, paddingLeft, paddingRight, paddingTop, paddingBottom);
                }

                if (borderColor != null)
                {
                    // We currently ignore possible difference in brush colors on different border sides. Use the last colored side mentioned
                    xamlElement.SetAttribute(XamlBorderBrush, borderColor);
                }

                if (borderThicknessSet)
                {
                    ComposeThicknessProperty(xamlElement, XamlBorderThickness, borderThicknessLeft, borderThicknessRight, borderThicknessTop, borderThicknessBottom);
                }
            }
        }

        /// <summary>
        /// Create syntactically optimized four-value Thickness
        /// </summary>
        /// <param name="xamlElement">XAML element</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="left">Left string</param>
        /// <param name="right">Right string</param>
        /// <param name="top">Top string</param>
        /// <param name="bottom">Bottom string</param>
        private static void ComposeThicknessProperty(XmlElement xamlElement, string propertyName, string left, string right, string top, string bottom)
        {
            // XAML syntax:
            // We have a reasonable interpreation for one value (all four edges), two values (horizontal, vertical),
            // and four values (left, top, right, bottom).
            //  switch (i) {
            //    case 1: return new Thickness(lengths[0]);
            //    case 2: return new Thickness(lengths[0], lengths[1], lengths[0], lengths[1]);
            //    case 4: return new Thickness(lengths[0], lengths[1], lengths[2], lengths[3]);
            //  }
            string thickness;

            // We do not accept negative margins
            if (left[0] == '0' || left[0] == '-')
            {
                left = "0";
            }

            if (right[0] == '0' || right[0] == '-')
            {
                right = "0";
            }

            if (top[0] == '0' || top[0] == '-')
            {
                top = "0";
            }

            if (bottom[0] == '0' || bottom[0] == '-')
            {
                bottom = "0";
            }

            if (left == right && top == bottom)
            {
                if (left == top)
                {
                    thickness = left;
                }
                else
                {
                    thickness = left + "," + top;
                }
            }
            else
            {
                thickness = left + "," + top + "," + right + "," + bottom;
            }

            // Need safer processing for a thickness value
            xamlElement.SetAttribute(propertyName, thickness);
        }

        /// <summary>
        /// Set property value
        /// </summary>
        /// <param name="xamlElement">XAML element</param>
        /// <param name="property">Dependency property</param>
        /// <param name="stringValue">String value</param>
        private static void SetPropertyValue(XmlElement xamlElement, DependencyProperty property, string stringValue)
        {
            TypeConverter typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
            try
            {
                object convertedValue = typeConverter.ConvertFromInvariantString(stringValue);
                if (convertedValue != null)
                {
                    xamlElement.SetAttribute(property.Name, stringValue);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Analyzes the tag of the htmlElement and infers its associated formatted properties.
        /// After that parses style attribute and adds all inline CSS styles.
        /// The resulting style attributes are collected in output parameter localProperties.
        /// </summary>
        /// <param name="htmlElement">HTML element</param>
        /// <param name="inheritedProperties">
        /// set of properties inherited from ancestor elements. Currently not used in the code. Reserved for the future development.
        /// </param>
        /// <param name="localProperties">
        /// returns all formatting properties defined by this element - implied by its tag, its attributes, or its CSS inline style
        /// </param>
        /// <param name="stylesheet">Style sheet</param>
        /// <param name="sourceContext">Source context</param>
        /// <returns>
        /// returns a combination of previous context with local set of properties.
        /// This value is not used in the current code - intended for the future development.
        /// </returns>
        private static Hashtable GetElementProperties(XmlElement htmlElement, Hashtable inheritedProperties, out Hashtable localProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
        {
            // Start with context formatting properties
            Hashtable currentProperties = new Hashtable();
            IDictionaryEnumerator propertyEnumerator = inheritedProperties.GetEnumerator();
            while (propertyEnumerator.MoveNext())
            {
                currentProperties[propertyEnumerator.Key] = propertyEnumerator.Value;
            }

            // Identify element name
            string elementName = htmlElement.LocalName.ToLower();
            string elementNamespace = htmlElement.NamespaceURI;

            // update current formatting properties depending on element tag
            localProperties = new Hashtable();
            switch (elementName)
            {
                // Character formatting
                case "i":
                case "italic":
                case "em":
                    localProperties["font-style"] = "italic";
                    break;
                case "b":
                case "bold":
                case "strong":
                case "dfn":
                    localProperties["font-weight"] = "bold";
                    break;
                case "u":
                case "underline":
                    localProperties["text-decoration-underline"] = "true";
                    break;
                case "font":
                    string attributeValue = GetAttribute(htmlElement, "face");
                    if (attributeValue != null)
                    {
                        localProperties["font-family"] = attributeValue;
                    }

                    attributeValue = GetAttribute(htmlElement, "size");
                    if (attributeValue != null)
                    {
                        double fontSize = double.Parse(attributeValue) * (12.0 / 3.0);
                        if (fontSize < 1.0)
                        {
                            fontSize = 1.0;
                        }
                        else if (fontSize > 1000.0)
                        {
                            fontSize = 1000.0;
                        }

                        localProperties["font-size"] = fontSize.ToString();
                    }

                    attributeValue = GetAttribute(htmlElement, "color");
                    if (attributeValue != null)
                    {
                        localProperties["color"] = attributeValue;
                    }

                    break;
                case "samp":
                    localProperties["font-family"] = "Courier New"; // code sample
                    localProperties["font-size"] = XamlFontSizeXXSmall;
                    localProperties["text-align"] = "Left";
                    break;
                case "sub":
                    break;
                case "sup":
                    break;

                // Hyperlinks
                case "a": // href, hreflang, urn, methods, rel, rev, title
                    //  Set default hyperlink properties
                    break;
                case "acronym":
                    break;

                // Paragraph formatting:
                case "p":
                    // Set default paragraph properties
                    break;
                case "div":
                    // Set default div properties
                    break;
                case "pre":
                    localProperties["font-family"] = "Courier New"; // renders text in a fixed-width font
                    localProperties["font-size"] = XamlFontSizeXXSmall;
                    localProperties["text-align"] = "Left";
                    break;
                case "blockquote":
                    localProperties["margin-left"] = "16";
                    break;

                case "h1":
                    localProperties["font-size"] = XamlFontSizeXXLarge;
                    break;
                case "h2":
                    localProperties["font-size"] = XamlFontSizeXLarge;
                    break;
                case "h3":
                    localProperties["font-size"] = XamlFontSizeLarge;
                    break;
                case "h4":
                    localProperties["font-size"] = XamlFontSizeMedium;
                    break;
                case "h5":
                    localProperties["font-size"] = XamlFontSizeSmall;
                    break;
                case "h6":
                    localProperties["font-size"] = XamlFontSizeXSmall;
                    break;

                // List properties
                case "ul":
                    localProperties["list-style-type"] = "disc";
                    break;
                case "ol":
                    localProperties["list-style-type"] = "decimal";
                    break;

                case "table":
                case "body":
                case "html":
                    break;
            }

            // Override html defaults by CSS attributes - from stylesheets and inline settings
            HtmlCssParser.GetElementPropertiesFromCssAttributes(htmlElement, elementName, stylesheet, localProperties, sourceContext);

            // Combine local properties with context to create new current properties
            propertyEnumerator = localProperties.GetEnumerator();
            while (propertyEnumerator.MoveNext())
            {
                currentProperties[propertyEnumerator.Key] = propertyEnumerator.Value;
            }

            return currentProperties;
        }

        /// <summary>
        /// Extracts a value of CSS attribute from CSS style definition.
        /// </summary>
        /// <param name="cssStyle">
        /// Source CSS style definition
        /// </param>
        /// <param name="attributeName">
        /// A name of CSS attribute to extract
        /// </param>
        /// <returns>
        /// A string representation of an attribute value if found;
        /// null if there is no such attribute in a given string.
        /// </returns>
        private static string GetCssAttribute(string cssStyle, string attributeName)
        {
            // This is poor man's attribute parsing. Replace it by real CSS parsing
            if (cssStyle != null)
            {
                string[] styleValues;

                attributeName = attributeName.ToLower();

                // Check for width specification in style string
                styleValues = cssStyle.Split(';');

                for (int styleValueIndex = 0; styleValueIndex < styleValues.Length; styleValueIndex++)
                {
                    string[] styleNameValue;

                    styleNameValue = styleValues[styleValueIndex].Split(':');
                    if (styleNameValue.Length == 2)
                    {
                        if (styleNameValue[0].Trim().ToLower() == attributeName)
                        {
                            return styleNameValue[1].Trim();
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Converts a length value from string representation to a double.
        /// </summary>
        /// <param name="lengthAsString">
        /// Source string value of a length.
        /// </param>
        /// <param name="length">Receives length</param>
        /// <returns>True is result is a valid number</returns>
        private static bool TryGetLengthValue(string lengthAsString, out double length)
        {
            length = double.NaN;

            if (lengthAsString != null)
            {
                lengthAsString = lengthAsString.Trim().ToLower();

                // We try to convert currentColumnWidthAsString into a double. This will eliminate widths of type "50%", etc.
                if (lengthAsString.EndsWith("pt"))
                {
                    lengthAsString = lengthAsString.Substring(0, lengthAsString.Length - 2);
                    if (double.TryParse(lengthAsString, out length))
                    {
                        length = (length * 96.0) / 72.0; // convert from points to pixels
                    }
                    else
                    {
                        length = double.NaN;
                    }
                }
                else if (lengthAsString.EndsWith("px"))
                {
                    lengthAsString = lengthAsString.Substring(0, lengthAsString.Length - 2);
                    if (!double.TryParse(lengthAsString, out length))
                    {
                        length = double.NaN;
                    }
                }
                else
                {
                    // Assuming pixels
                    if (!double.TryParse(lengthAsString, out length)) 
                    {
                        length = double.NaN;
                    }
                }
            }

            return !double.IsNaN(length);
        }

        // .................................................................
        //
        // Parsing Color Attribute
        //
        // .................................................................

        /// <summary>
        /// Get Color Value
        /// </summary>
        /// <param name="colorValue">Color value</param>
        /// <returns>The color value</returns>
        private static string GetColorValue(string colorValue)
        {
            // TODO: Implement color conversion
            return colorValue;
        }

        /// <summary>
        /// Applies properties to XAML Table Cell Element based on the html TD element it is converted from.
        /// </summary>
        /// <param name="htmlChildNode">
        /// Html TD/TH element to be converted to XAML
        /// </param>
        /// <param name="xamlTableCellElement">
        /// XmlElement representing XAML element for which properties are to be processed
        /// </param>
        /// <remarks>
        /// TODO: Use the processed properties for htmlChildNode instead of using the node itself
        /// </remarks>
        private static void ApplyPropertiesToTableCellElement(XmlElement htmlChildNode, XmlElement xamlTableCellElement)
        {
            // Parameter validation
            Debug.Assert(htmlChildNode.LocalName.ToLower() == "td" || htmlChildNode.LocalName.ToLower() == "th", "HtmlToXamlConverter");
            Debug.Assert(xamlTableCellElement.LocalName == XamlTableCell, "HtmlToXamlConverter");

            // set default border thickness for xamlTableCellElement to enable gridlines
            xamlTableCellElement.SetAttribute(XamlTableCellBorderThickness, "1,1,1,1");
            xamlTableCellElement.SetAttribute(XamlTableCellBorderBrush, XamlBrushesBlack);
            string rowSpanString = GetAttribute((XmlElement)htmlChildNode, "rowspan");
            if (rowSpanString != null)
            {
                xamlTableCellElement.SetAttribute(XamlTableCellRowSpan, rowSpanString);
            }
        }

        #endregion Private Methods
    }
}