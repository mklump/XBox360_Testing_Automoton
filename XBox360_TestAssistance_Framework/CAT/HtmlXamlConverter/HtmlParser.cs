// -----------------------------------------------------------------------
// <copyright file="HtmlParser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace HtmlToXamlConvert
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml;

    // This code was taken from MSDN as an example of converting HTML to XAML.
    //
    // I've combined all the classes together and made some spelling corrections.
    //
    // Copyright (C) Microsoft Corporation.  All rights reserved.
    //
    // Description: Prototype for XAML - Html conversion

    /// <summary>
    /// HtmlParser class accepts a string of possibly badly formed Html, parses it and returns a string
    /// of well-formed Html that is as close to the original string in content as possible
    /// </summary>
    internal class HtmlParser : IDisposable
    {
        /// <summary>
        /// HTML header
        /// </summary>
        internal const string HtmlHeader = "Version:1.0\r\nStartHTML:{0:D10}\r\nEndHTML:{1:D10}\r\nStartFragment:{2:D10}\r\nEndFragment:{3:D10}\r\nStartSelection:{4:D10}\r\nEndSelection:{5:D10}\r\n";

        /// <summary>
        /// HTML Start fragment comment
        /// </summary>
        internal const string HtmlStartFragmentComment = "<!--StartFragment-->";

        /// <summary>
        /// HTML End fragment comment
        /// </summary>
        internal const string HtmlEndFragmentComment = "<!--EndFragment-->";

        // ---------------------------------------------------------------------
        //
        // Private Fields
        //
        // ---------------------------------------------------------------------
        #region Private Fields

        /// <summary>
        /// XHTML namespace
        /// </summary>
        internal const string XhtmlNamespace = "http://www.w3.org/1999/xhtml";

        /// <summary>
        /// HTML lexical analyzer
        /// </summary>
        private HtmlLexicalAnalyzer htmlLexicalAnalyzer;

        /// <summary>
        /// document from which all elements are created
        /// </summary>
        private XmlDocument document;

        /// <summary>
        /// stack for open elements
        /// </summary>
        private Stack<XmlElement> openedElements;

        /// <summary>
        /// Stack for pending inline elements
        /// </summary>
        private Stack<XmlElement> pendingInlineElements;

        #endregion Private Fields

        // ---------------------------------------------------------------------
        //
        // Constructors
        //
        // ---------------------------------------------------------------------
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlParser" /> class.
        /// Initializes the htmlLexicalAnalyzer element with the given input string
        /// </summary>
        /// <param name="inputString">
        /// string to parsed into well-formed Html
        /// </param>
        private HtmlParser(string inputString)
        {
            // Create an output xml document
            this.document = new XmlDocument();

            // initialize open tag stack
            this.openedElements = new Stack<XmlElement>();

            this.pendingInlineElements = new Stack<XmlElement>();

            // initialize lexical analyzer
            this.htmlLexicalAnalyzer = new HtmlLexicalAnalyzer(inputString);

            // get first token from input, expecting text
            this.htmlLexicalAnalyzer.GetNextContentToken();
        }

        #endregion Constructors

        /// <summary>
        /// Implements IDisposable.Dispose()
        /// </summary>
        public void Dispose()
        {
            if (this.htmlLexicalAnalyzer != null)
            {
                this.htmlLexicalAnalyzer.Dispose();
            }
        }

        // ---------------------------------------------------------------------
        //
        // Internal Methods
        //
        // ---------------------------------------------------------------------
        #region Internal Methods

        /// <summary>
        /// Instantiates an HtmlParser element and calls the parsing function on the given input string
        /// </summary>
        /// <param name="htmlString">
        /// Input string of possibly badly-formed Html to be parsed into well-formed Html
        /// </param>
        /// <returns>
        /// XmlElement rep
        /// </returns>
        internal static XmlElement ParseHtml(string htmlString)
        {
            XmlElement htmlRootElement;
            using (HtmlParser htmlParser = new HtmlParser(htmlString))
            {
                htmlRootElement = htmlParser.ParseHtmlContent();
            }

            return htmlRootElement;
        }

        // .....................................................................
        //
        // Html Header on Clipboard
        //
        // .....................................................................

        // Html header structure.
        //      Version:1.0
        //      StartHTML:000000000
        //      EndHTML:000000000
        //      StartFragment:000000000
        //      EndFragment:000000000
        //      StartSelection:000000000
        //      EndSelection:000000000

        /// <summary>
        /// Extracts Html string from clipboard data by parsing header information in htmlDataString
        /// </summary>
        /// <param name="htmlDataString">
        /// String representing Html clipboard data. This includes Html header
        /// </param>
        /// <returns>
        /// String containing only the Html data part of htmlDataString, without header
        /// </returns>
        internal static string ExtractHtmlFromClipboardData(string htmlDataString)
        {
            int startHtmlIndex = htmlDataString.IndexOf("StartHTML:");
            if (startHtmlIndex < 0)
            {
                return "ERROR: Urecognized html header";
            }

            //// TODO: We assume that indices represented by strictly 10 zeros ("0123456789".Length),
            //// which could be wrong assumption. We need to implement more flrxible parsing here
            startHtmlIndex = int.Parse(htmlDataString.Substring(startHtmlIndex + "StartHTML:".Length, "0123456789".Length));
            if (startHtmlIndex < 0 || startHtmlIndex > htmlDataString.Length)
            {
                return "ERROR: Urecognized html header";
            }

            int endHtmlIndex = htmlDataString.IndexOf("EndHTML:");
            if (endHtmlIndex < 0)
            {
                return "ERROR: Urecognized html header";
            }

            //// TODO: We assume that indices represented by strictly 10 zeros ("0123456789".Length),
            //// which could be wrong assumption. We need to implement more flrxible parsing here
            endHtmlIndex = int.Parse(htmlDataString.Substring(endHtmlIndex + "EndHTML:".Length, "0123456789".Length));
            if (endHtmlIndex > htmlDataString.Length)
            {
                endHtmlIndex = htmlDataString.Length;
            }

            return htmlDataString.Substring(startHtmlIndex, endHtmlIndex - startHtmlIndex);
        }

        /// <summary>
        /// Adds XHTML header information to Html data string so that it can be placed on clipboard
        /// </summary>
        /// <param name="htmlString">
        /// Html string to be placed on clipboard with appropriate header
        /// </param>
        /// <returns>
        /// String wrapping htmlString with appropriate Html header
        /// </returns>
        internal static string AddHtmlClipboardHeader(string htmlString)
        {
            StringBuilder stringBuilder = new StringBuilder();

            // each of 6 numbers is represented by "{0:D10}" in the format string
            // must actually occupy 10 digit positions ("0123456789")
            int startHTML = HtmlHeader.Length + (6 * ("0123456789".Length - "{0:D10}".Length));
            int endHTML = startHTML + htmlString.Length;
            int startFragment = htmlString.IndexOf(HtmlStartFragmentComment, 0);
            if (startFragment >= 0)
            {
                startFragment = startHTML + startFragment + HtmlStartFragmentComment.Length;
            }
            else
            {
                startFragment = startHTML;
            }

            int endFragment = htmlString.IndexOf(HtmlEndFragmentComment, 0);
            if (endFragment >= 0)
            {
                endFragment = startHTML + endFragment;
            }
            else
            {
                endFragment = endHTML;
            }

            // Create HTML clipboard header string
            stringBuilder.AppendFormat(HtmlHeader, startHTML, endHTML, startFragment, endFragment, startFragment, endFragment);

            // Append HTML body.
            stringBuilder.Append(htmlString);

            return stringBuilder.ToString();
        }

        #endregion Internal Methods

        // ---------------------------------------------------------------------
        //
        // Private methods
        //
        // ---------------------------------------------------------------------
        #region Private Methods

        /// <summary>
        /// Invariant assert.
        /// Throws an exception if the condition is false
        /// </summary>
        /// <param name="condition">Condition that must be true</param>
        /// <param name="message">Message to include in the exception</param>
        private void InvariantAssert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception("Assertion error: " + message);
            }
        }

        /// <summary>
        /// Parses the stream of html tokens starting
        /// from the name of top-level element.
        /// Returns XmlElement representing the top-level
        /// html element
        /// </summary>
        /// <returns>XML element</returns>
        private XmlElement ParseHtmlContent()
        {
            // Create artificial root elelemt to be able to group multiple top-level elements
            // We create "html" element which may be a duplicate of real HTML element, which is ok, as HtmlConverter will swallow it painlessly..
            XmlElement htmlRootElement = this.document.CreateElement("html", XhtmlNamespace);
            this.OpenStructuringElement(htmlRootElement);

            while (this.htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EOF)
            {
                if (this.htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.OpeningTagStart)
                {
                    this.htmlLexicalAnalyzer.GetNextTagToken();
                    if (this.htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
                    {
                        string htmlElementName = this.htmlLexicalAnalyzer.NextToken.ToLower();
                        this.htmlLexicalAnalyzer.GetNextTagToken();

                        // Create an element
                        XmlElement htmlElement = this.document.CreateElement(htmlElementName, XhtmlNamespace);

                        // Parse element attributes
                        this.ParseAttributes(htmlElement);

                        if (this.htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.EmptyTagEnd || HtmlSchema.IsEmptyElement(htmlElementName))
                        {
                            // It is an element without content (because of explicit slash or based on implicit knowledge aboout html)
                            this.AddEmptyElement(htmlElement);
                        }
                        else if (HtmlSchema.IsInlineElement(htmlElementName))
                        {
                            // Elements known as formatting are pushed to some special
                            // pending stack, which allows them to be transferred
                            // over block tags - by doing this we convert
                            // overlapping tags into normal heirarchical element structure.
                            this.OpenInlineElement(htmlElement);
                        }
                        else if (HtmlSchema.IsBlockElement(htmlElementName) || HtmlSchema.IsKnownOpenableElement(htmlElementName))
                        {
                            // This includes no-scope elements
                            this.OpenStructuringElement(htmlElement);
                        }
                        else
                        {
                            // Do nothing. Skip the whole opening tag.
                            // Ignoring all unknown elements on their start tags.
                            // Thus we will ignore them on closinng tag as well.
                            // Anyway we don't know what to do withthem on conversion to XAML.
                        }
                    }
                    else
                    {
                        // Note that the token following opening angle bracket must be a name - lexical analyzer must guarantee that.
                        // Otherwise - we skip the angle bracket and continue parsing the content as if it is just text.
                        //  Add the following asserion here, right? or output "<" as a text run instead?:
                        // InvariantAssert(false, "Angle bracket without a following name is not expected");
                    }
                }
                else if (this.htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.ClosingTagStart)
                {
                    this.htmlLexicalAnalyzer.GetNextTagToken();
                    if (this.htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
                    {
                        string htmlElementName = this.htmlLexicalAnalyzer.NextToken.ToLower();

                        // Skip the name token. Assume that the following token is end of tag,
                        // but do not check this. If it is not true, we simply ignore one token
                        // - this is our recovery from bad xml in this case.
                        this.htmlLexicalAnalyzer.GetNextTagToken();

                        this.CloseElement(htmlElementName);
                    }
                }
                else if (this.htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Text)
                {
                    this.AddTextContent(this.htmlLexicalAnalyzer.NextToken);
                }
                else if (this.htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Comment)
                {
                    this.AddComment(this.htmlLexicalAnalyzer.NextToken);
                }

                this.htmlLexicalAnalyzer.GetNextContentToken();
            }

            // Get rid of the artificial root element
            if (htmlRootElement.FirstChild is XmlElement &&
                htmlRootElement.FirstChild == htmlRootElement.LastChild &&
                htmlRootElement.FirstChild.LocalName.ToLower() == "html")
            {
                htmlRootElement = (XmlElement)htmlRootElement.FirstChild;
            }

            return htmlRootElement;
        }

        /// <summary>
        /// Create element copy
        /// </summary>
        /// <param name="htmlElement">HTML element</param>
        /// <returns>XML element</returns>
        private XmlElement CreateElementCopy(XmlElement htmlElement)
        {
            XmlElement htmlElementCopy = this.document.CreateElement(htmlElement.LocalName, XhtmlNamespace);
            for (int i = 0; i < htmlElement.Attributes.Count; i++)
            {
                XmlAttribute attribute = htmlElement.Attributes[i];
                htmlElementCopy.SetAttribute(attribute.Name, attribute.Value);
            }

            return htmlElementCopy;
        }

        /// <summary>
        /// Add empty element
        /// </summary>
        /// <param name="htmlEmptyElement">HTML empty element</param>
        private void AddEmptyElement(XmlElement htmlEmptyElement)
        {
            this.InvariantAssert(this.openedElements.Count > 0, "AddEmptyElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");
            XmlElement htmlParent = this.openedElements.Peek();
            htmlParent.AppendChild(htmlEmptyElement);
        }

        /// <summary>
        /// Open Inline elements
        /// </summary>
        /// <param name="htmlInlineElement">HTML inline element</param>
        private void OpenInlineElement(XmlElement htmlInlineElement)
        {
            this.pendingInlineElements.Push(htmlInlineElement);
        }

        /// <summary>
        /// Opens structure element such as DIV or TABLE etc.
        /// </summary>
        /// <param name="htmlElement">HTML element</param>
        private void OpenStructuringElement(XmlElement htmlElement)
        {
            // Close all pending inline elements
            // All block elements are considered as delimiters for inline elements
            // which forces all inline elements to be closed and re-opened in the following
            // structural element (if any).
            // By doing that we guarantee that all inline elements appear only within most nested blocks
            if (HtmlSchema.IsBlockElement(htmlElement.LocalName))
            {
                while (this.openedElements.Count > 0 && HtmlSchema.IsInlineElement(this.openedElements.Peek().LocalName))
                {
                    XmlElement htmlInlineElement = this.openedElements.Pop();
                    this.InvariantAssert(this.openedElements.Count > 0, "OpenStructuringElement: stack of opened elements cannot become empty here");

                    this.pendingInlineElements.Push(this.CreateElementCopy(htmlInlineElement));
                }
            }

            // Add this block element to its parent
            if (this.openedElements.Count > 0)
            {
                XmlElement htmlParent = this.openedElements.Peek();

                // Check some known block elements for auto-closing (LI and P)
                if (HtmlSchema.ClosesOnNextElementStart(htmlParent.LocalName, htmlElement.LocalName))
                {
                    this.openedElements.Pop();
                    htmlParent = this.openedElements.Count > 0 ? this.openedElements.Peek() : null;
                }

                if (htmlParent != null)
                {
                    // NOTE:
                    // Actually we never expect null - it would mean two top-level P or LI (without a parent).
                    // In such weird case we will loose all paragraphs except the first one...
                    htmlParent.AppendChild(htmlElement);
                }
            }

            // Push it onto a stack
            this.openedElements.Push(htmlElement);
        }

        /// <summary>
        /// Is element opened
        /// </summary>
        /// <param name="htmlElementName">HTML element name</param>
        /// <returns>true if found, false if not</returns>
        private bool IsElementOpened(string htmlElementName)
        {
            foreach (XmlElement openedElement in this.openedElements)
            {
                if (openedElement.LocalName == htmlElementName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Close element
        /// </summary>
        /// <param name="htmlElementName">HTML element name</param>
        private void CloseElement(string htmlElementName)
        {
            // Check if the element is opened and already added to the parent
            this.InvariantAssert(this.openedElements.Count > 0, "CloseElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");

            // Check if the element is opened and still waiting to be added to the parent
            if (this.pendingInlineElements.Count > 0 && this.pendingInlineElements.Peek().LocalName == htmlElementName)
            {
                // Closing an empty inline element.
                // Note that HtmlConverter will skip empty inlines, but for completeness we keep them here on parser level.
                XmlElement htmlInlineElement = this.pendingInlineElements.Pop();
                this.InvariantAssert(this.openedElements.Count > 0, "CloseElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");
                XmlElement htmlParent = this.openedElements.Peek();
                htmlParent.AppendChild(htmlInlineElement);
                return;
            }
            else if (this.IsElementOpened(htmlElementName))
            {
                // we never pop the last element - the artificial root
                while (this.openedElements.Count > 1) 
                {
                    // Close all unbalanced elements.
                    XmlElement htmlOpenedElement = this.openedElements.Pop();

                    if (htmlOpenedElement.LocalName == htmlElementName)
                    {
                        return;
                    }

                    if (HtmlSchema.IsInlineElement(htmlOpenedElement.LocalName))
                    {
                        // Unbalances Inlines will be transfered to the next element content
                        this.pendingInlineElements.Push(this.CreateElementCopy(htmlOpenedElement));
                    }
                }
            }

            // If element was not opened, we simply ignore the unbalanced closing tag
            return;
        }

        /// <summary>
        /// Add text content
        /// </summary>
        /// <param name="textContent">Text content</param>
        private void AddTextContent(string textContent)
        {
            this.OpenPendingInlineElements();

            this.InvariantAssert(this.openedElements.Count > 0, "AddTextContent: Stack of opened elements cannot be empty, as we have at least one artificial root element");

            XmlElement htmlParent = this.openedElements.Peek();
            XmlText textNode = this.document.CreateTextNode(textContent);
            htmlParent.AppendChild(textNode);
        }

        /// <summary>
        /// Add comment
        /// </summary>
        /// <param name="comment">comment to add</param>
        private void AddComment(string comment)
        {
            this.OpenPendingInlineElements();

            this.InvariantAssert(this.openedElements.Count > 0, "AddComment: Stack of opened elements cannot be empty, as we have at least one artificial root element");

            XmlElement htmlParent = this.openedElements.Peek();
            XmlComment xmlComment = this.document.CreateComment(comment);
            htmlParent.AppendChild(xmlComment);
        }

        /// <summary>
        /// Moves all inline elements pending for opening to actual document
        /// and adds them to current open stack.
        /// </summary>
        private void OpenPendingInlineElements()
        {
            if (this.pendingInlineElements.Count > 0)
            {
                XmlElement htmlInlineElement = this.pendingInlineElements.Pop();

                this.OpenPendingInlineElements();

                this.InvariantAssert(this.openedElements.Count > 0, "OpenPendingInlineElements: Stack of opened elements cannot be empty, as we have at least one artificial root element");

                XmlElement htmlParent = this.openedElements.Peek();
                htmlParent.AppendChild(htmlInlineElement);
                this.openedElements.Push(htmlInlineElement);
            }
        }

        /// <summary>
        /// Parse attributes
        /// </summary>
        /// <param name="xmlElement">XML element</param>
        private void ParseAttributes(XmlElement xmlElement)
        {
            while (this.htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EOF &&
                this.htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.TagEnd &&
                this.htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EmptyTagEnd)
            {
                // read next attribute (name=value)
                if (this.htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
                {
                    string attributeName = this.htmlLexicalAnalyzer.NextToken;
                    this.htmlLexicalAnalyzer.GetNextEqualSignToken();

                    this.htmlLexicalAnalyzer.GetNextAtomToken();

                    string attributeValue = this.htmlLexicalAnalyzer.NextToken;
                    xmlElement.SetAttribute(attributeName, attributeValue);
                }

                this.htmlLexicalAnalyzer.GetNextTagToken();
            }
        }

        #endregion Private Methods
    }
}