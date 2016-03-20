// -----------------------------------------------------------------------
// <copyright file="CssStylesheet.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace HtmlToXamlConvert
{
    using System.Collections.Generic;
    using System.Diagnostics;
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
    /// CSS Style sheet
    /// </summary>
    internal class CssStylesheet
    {
        /// <summary>
        /// Style definitions
        /// </summary>
        private List<StyleDefinition> styleDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CssStylesheet" /> class.
        /// </summary>
        /// <param name="htmlElement">HTML element</param>
        public CssStylesheet(XmlElement htmlElement)
        {
            if (htmlElement != null)
            {
                this.DiscoverStyleDefinitions(htmlElement);
            }
        }

        /// <summary>
        /// Recursively traverses an html tree, discovers STYLE elements and creates a style definition table
        /// for further cascading style application
        /// </summary>
        /// <param name="htmlElement">HTML element</param>
        public void DiscoverStyleDefinitions(XmlElement htmlElement)
        {
            if (htmlElement.LocalName.ToLower() == "link")
            {
                // Add LINK elements processing for included stylesheets
                // <LINK href="http://sc.msn.com/global/css/ptnr/orange.css" type=text/css \r\nrel=stylesheet>
                return;
            }

            if (htmlElement.LocalName.ToLower() != "style")
            {
                // This is not a STYLE element. Recurse into it
                for (XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
                {
                    if (htmlChildNode is XmlElement)
                    {
                        this.DiscoverStyleDefinitions((XmlElement)htmlChildNode);
                    }
                }

                return;
            }

            // Add style definitions from this style.
            // Collect all text from this style definition
            StringBuilder stylesheetBuffer = new StringBuilder();
            for (XmlNode htmlChildNode = htmlElement.FirstChild; htmlChildNode != null; htmlChildNode = htmlChildNode.NextSibling)
            {
                if (htmlChildNode is XmlText || htmlChildNode is XmlComment)
                {
                    stylesheetBuffer.Append(this.RemoveComments(htmlChildNode.Value));
                }
            }

            // CssStylesheet has the following syntactical structure:
            //     @import declaration;
            //     selector { definition }
            // where "selector" is one of: ".classname", "tagname"
            // It can contain comments in the following form: /*...*/
            int nextCharacterIndex = 0;
            while (nextCharacterIndex < stylesheetBuffer.Length)
            {
                // Extract selector
                int selectorStart = nextCharacterIndex;
                while (nextCharacterIndex < stylesheetBuffer.Length && stylesheetBuffer[nextCharacterIndex] != '{')
                {
                    // Skip declaration directive starting from @
                    if (stylesheetBuffer[nextCharacterIndex] == '@')
                    {
                        while (nextCharacterIndex < stylesheetBuffer.Length && stylesheetBuffer[nextCharacterIndex] != ';')
                        {
                            nextCharacterIndex++;
                        }

                        selectorStart = nextCharacterIndex + 1;
                    }

                    nextCharacterIndex++;
                }

                if (nextCharacterIndex < stylesheetBuffer.Length)
                {
                    // Extract definition
                    int definitionStart = nextCharacterIndex;
                    while (nextCharacterIndex < stylesheetBuffer.Length && stylesheetBuffer[nextCharacterIndex] != '}')
                    {
                        nextCharacterIndex++;
                    }

                    // Define a style
                    if (nextCharacterIndex - definitionStart > 2)
                    {
                        this.AddStyleDefinition(
                            stylesheetBuffer.ToString(selectorStart, definitionStart - selectorStart),
                            stylesheetBuffer.ToString(definitionStart + 1, nextCharacterIndex - definitionStart - 2));
                    }

                    // Skip closing brace
                    if (nextCharacterIndex < stylesheetBuffer.Length)
                    {
                        Debug.Assert(stylesheetBuffer[nextCharacterIndex] == '}', "Unexpected character");
                        nextCharacterIndex++;
                    }
                }
            }
        }

        /// <summary>
        /// Add Style Definition
        /// </summary>
        /// <param name="selector">Selector to use</param>
        /// <param name="definition">Definition of style to add</param>
        public void AddStyleDefinition(string selector, string definition)
        {
            // Notrmalize parameter values
            selector = selector.Trim().ToLower();
            definition = definition.Trim().ToLower();
            if (selector.Length == 0 || definition.Length == 0)
            {
                return;
            }

            if (this.styleDefinitions == null)
            {
                this.styleDefinitions = new List<StyleDefinition>();
            }

            string[] simpleSelectors = selector.Split(',');

            for (int i = 0; i < simpleSelectors.Length; i++)
            {
                string simpleSelector = simpleSelectors[i].Trim();
                if (simpleSelector.Length > 0)
                {
                    this.styleDefinitions.Add(new StyleDefinition(simpleSelector, definition));
                }
            }
        }

        /// <summary>
        /// Get style
        /// </summary>
        /// <param name="elementName">Element name</param>
        /// <param name="sourceContext">Source context</param>
        /// <returns>A string indicating the style</returns>
        public string GetStyle(string elementName, List<XmlElement> sourceContext)
        {
            Debug.Assert(sourceContext.Count > 0, "Unexpected end of data");
            Debug.Assert(elementName == sourceContext[sourceContext.Count - 1].LocalName, "Unexpected element name");

            // Add id processing for style selectors
            if (this.styleDefinitions != null)
            {
                for (int i = this.styleDefinitions.Count - 1; i >= 0; i--)
                {
                    string selector = this.styleDefinitions[i].Selector;

                    string[] selectorLevels = selector.Split(' ');

                    int indexInSelector = selectorLevels.Length - 1;
                    int indexInContext = sourceContext.Count - 1;
                    string selectorLevel = selectorLevels[indexInSelector].Trim();

                    if (this.MatchSelectorLevel(selectorLevel, sourceContext[sourceContext.Count - 1]))
                    {
                        return this.styleDefinitions[i].Definition;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a string with all c-style comments replaced by spaces
        /// </summary>
        /// <param name="text">Text to remove comments from</param>
        /// <returns>Text with comments removed</returns>
        private string RemoveComments(string text)
        {
            int commentStart = text.IndexOf("/*");
            if (commentStart < 0)
            {
                return text;
            }

            int commentEnd = text.IndexOf("*/", commentStart + 2);
            if (commentEnd < 0)
            {
                return text.Substring(0, commentStart);
            }

            return text.Substring(0, commentStart) + " " + this.RemoveComments(text.Substring(commentEnd + 2));
        }

        /// <summary>
        /// Match Selector Level
        /// </summary>
        /// <param name="selectorLevel">Selector level to match</param>
        /// <param name="xmlElement">XML element</param>
        /// <returns>true if matched, false if not</returns>
        private bool MatchSelectorLevel(string selectorLevel, XmlElement xmlElement)
        {
            if (selectorLevel.Length == 0)
            {
                return false;
            }

            int indexOfDot = selectorLevel.IndexOf('.');
            int indexOfPound = selectorLevel.IndexOf('#');

            string selectorClass = null;
            string selectorId = null;
            string selectorTag = null;
            if (indexOfDot >= 0)
            {
                if (indexOfDot > 0)
                {
                    selectorTag = selectorLevel.Substring(0, indexOfDot);
                }

                selectorClass = selectorLevel.Substring(indexOfDot + 1);
            }
            else if (indexOfPound >= 0)
            {
                if (indexOfPound > 0)
                {
                    selectorTag = selectorLevel.Substring(0, indexOfPound);
                }

                selectorId = selectorLevel.Substring(indexOfPound + 1);
            }
            else
            {
                selectorTag = selectorLevel;
            }

            if (selectorTag != null && selectorTag != xmlElement.LocalName)
            {
                return false;
            }

            if (selectorId != null && HtmlToXamlConverter.GetAttribute(xmlElement, "id") != selectorId)
            {
                return false;
            }

            if (selectorClass != null && HtmlToXamlConverter.GetAttribute(xmlElement, "class") != selectorClass)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Style Definition
        /// </summary>
        private class StyleDefinition
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="StyleDefinition" /> class
            /// </summary>
            /// <param name="selector">A selector</param>
            /// <param name="definition">A definition</param>
            public StyleDefinition(string selector, string definition)
            {
                this.Selector = selector;
                this.Definition = definition;
            }

            /// <summary>
            /// Gets or sets the selector
            /// </summary>
            public string Selector { get; set; }

            /// <summary>
            /// Gets or sets the definition
            /// </summary>
            public string Definition { get; set; }
        }
    }
}