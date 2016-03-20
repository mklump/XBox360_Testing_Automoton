// -----------------------------------------------------------------------
// <copyright file="HtmlCssParser.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace HtmlToXamlConvert
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Xml;

    // This code was taken from MSDN as an example of converting HTML to XAML.
    //
    // I've combined all the classes together and made some spelling corrections.
    //
    // Copyright (C) Microsoft Corporation.  All rights reserved.
    //
    // Description: Prototype for XAML - Html conversion

    /// <summary>
    /// HTML CSS Parser
    /// </summary>
    internal static class HtmlCssParser
    {
        /// <summary>
        /// Valid colors
        /// </summary>
        private static readonly string[] Colors = new string[]
            {
                "aliceblue", "antiquewhite", "aqua", "aquamarine", "azure", "beige", "bisque", "black", "blanchedalmond",
                "blue", "blueviolet", "brown", "burlywood", "cadetblue", "chartreuse", "chocolate", "coral",
                "cornflowerblue", "cornsilk", "crimson", "cyan", "darkblue", "darkcyan", "darkgoldenrod", "darkgray",
                "darkgreen", "darkkhaki", "darkmagenta", "darkolivegreen", "darkorange", "darkorchid", "darkred",
                "darksalmon", "darkseagreen", "darkslateblue", "darkslategray", "darkturquoise", "darkviolet", "deeppink",
                "deepskyblue", "dimgray", "dodgerblue", "firebrick", "floralwhite", "forestgreen", "fuchsia", "gainsboro",
                "ghostwhite", "gold", "goldenrod", "gray", "green", "greenyellow", "honeydew", "hotpink", "indianred",
                "indigo", "ivory", "khaki", "lavender", "lavenderblush", "lawngreen", "lemonchiffon", "lightblue", "lightcoral",
                "lightcyan", "lightgoldenrodyellow", "lightgreen", "lightgrey", "lightpink", "lightsalmon", "lightseagreen",
                "lightskyblue", "lightslategray", "lightsteelblue", "lightyellow", "lime", "limegreen", "linen", "magenta",
                "maroon", "mediumaquamarine", "mediumblue", "mediumorchid", "mediumpurple", "mediumseagreen", "mediumslateblue",
                "mediumspringgreen", "mediumturquoise", "mediumvioletred", "midnightblue", "mintcream", "mistyrose", "moccasin",
                "navajowhite", "navy", "oldlace", "olive", "olivedrab", "orange", "orangered", "orchid", "palegoldenrod",
                "palegreen", "paleturquoise", "palevioletred", "papayawhip", "peachpuff", "peru", "pink", "plum", "powderblue",
                "purple", "red", "rosybrown", "royalblue", "saddlebrown", "salmon", "sandybrown", "seagreen", "seashell",
                "sienna", "silver", "skyblue", "slateblue", "slategray", "snow", "springgreen", "steelblue", "tan", "teal",
                "thistle", "tomato", "turquoise", "violet", "wheat", "white", "whitesmoke", "yellow", "yellowgreen",
            };

        /// <summary>
        /// System colors
        /// </summary>
        private static readonly string[] SystemColors = new string[]
            {
                "activeborder", "activecaption", "appworkspace", "background", "buttonface", "buttonhighlight", "buttonshadow",
                "buttontext", "captiontext", "graytext", "highlight", "highlighttext", "inactiveborder", "inactivecaption",
                "inactivecaptiontext", "infobackground", "infotext", "menu", "menutext", "scrollbar", "threeddarkshadow",
                "threedface", "threedhighlight", "threedlightshadow", "threedshadow", "window", "windowframe", "windowtext",
            };

        // CSS has five font properties: font-family, font-style, font-variant, font-weight, font-size.
        // An aggregated "font" property lets you specify in one action all the five in combination
        // with additional line-height property.
        //
        // font-family: [<family-name>,]* [<family-name> | <generic-family>]
        //    generic-family: serif | sans-serif | monospace | cursive | fantasy
        //       The list of families sets priorities to choose fonts;
        //       Quotes not allowed around generic-family names
        // font-style: normal | italic | oblique
        // font-variant: normal | small-caps
        // font-weight: normal | bold | bolder | lighter | 100 ... 900 |
        //    Default is "normal", normal==400
        // font-size: <absolute-size> | <relative-size> | <length> | <percentage>
        //    absolute-size: xx-small | x-small | small | medium | large | x-large | xx-large
        //    relative-size: larger | smaller
        //    length: <point> | <pica> | <ex> | <em> | <points> | <millimeters> | <centimeters> | <inches>
        //    Default: medium
        // font: [ <font-style> || <font-variant> || <font-weight ]? <font-size> [ / <line-height> ]? <font-family>

        /// <summary>
        /// Constant list of all generic families
        /// </summary>
        private static readonly string[] FontGenericFamilies = new string[] { "serif", "sans-serif", "monospace", "cursive", "fantasy" };

        /// <summary>
        /// Constant list of all font styles
        /// </summary>
        private static readonly string[] FontStyles = new string[] { "normal", "italic", "oblique" };

        /// <summary>
        /// Constant list of all font variants
        /// </summary>
        private static readonly string[] FontVariants = new string[] { "normal", "small-caps" };

        /// <summary>
        /// Constant list of all font weights
        /// </summary>
        private static readonly string[] FontWeights = new string[] { "normal", "bold", "bolder", "lighter", "100", "200", "300", "400", "500", "600", "700", "800", "900" };

        /// <summary>
        /// Constant list of all font absolute sizes
        /// </summary>
        private static readonly string[] FontAbsoluteSizes = new string[] { "xx-small", "x-small", "small", "medium", "large", "x-large", "xx-large" };

        /// <summary>
        /// Constant list of all font relative sizes
        /// </summary>
        private static readonly string[] FontRelativeSizes = new string[] { "larger", "smaller" };

        /// <summary>
        /// Constant list of all size units
        /// </summary>
        private static readonly string[] FontSizeUnits = new string[] { "px", "mm", "cm", "in", "pt", "pc", "em", "ex", "%" };

        /// <summary>
        /// Constant list of all list style types
        /// </summary>
        private static readonly string[] ListStyleTypes = new string[] { "disc", "circle", "square", "decimal", "lower-roman", "upper-roman", "lower-alpha", "upper-alpha", "none" };

        /// <summary>
        /// Constant list of all list style positions
        /// </summary>
        private static readonly string[] ListStylePositions = new string[] { "inside", "outside" };

        /// <summary>
        /// Constant list of all text decorations
        /// </summary>
        private static readonly string[] TextDecorations = new string[] { "none", "underline", "overline", "line-through", "blink" };

        /// <summary>
        /// Constant list of all text transforms
        /// </summary>
        private static readonly string[] TextTransforms = new string[] { "none", "capitalize", "uppercase", "lowercase" };

        /// <summary>
        /// Constant list of all text alignments
        /// </summary>
        private static readonly string[] TextAligns = new string[] { "left", "right", "center", "justify" };

        /// <summary>
        /// Constant list of all vertical alignments
        /// </summary>
        private static readonly string[] VerticalAligns = new string[] { "baseline", "sub", "super", "top", "text-top", "middle", "bottom", "text-bottom" };

        /// <summary>
        /// Constant list of all floats
        /// </summary>
        private static readonly string[] Floats = new string[] { "left", "right", "none" };

        /// <summary>
        /// Constant list of all clears
        /// </summary>
        private static readonly string[] Clears = new string[] { "none", "left", "right", "both" };

        /// <summary>
        /// Constant list of all border styles
        /// </summary>
        private static readonly string[] BorderStyles = new string[] { "none", "dotted", "dashed", "solid", "double", "groove", "ridge", "inset", "outset" };

        /// <summary>
        /// Constant list of all blocks
        /// </summary>
        private static string[] blocks = new string[] { "block", "inline", "list-item", "none" };

        /// <summary>
        /// Process CSS Attributes
        /// </summary>
        /// <param name="htmlElement">HTML element</param>
        /// <param name="elementName">Name of element</param>
        /// <param name="stylesheet">CSS style sheet</param>
        /// <param name="localProperties">Local properties</param>
        /// <param name="sourceContext">Source context</param>
        internal static void GetElementPropertiesFromCssAttributes(XmlElement htmlElement, string elementName, CssStylesheet stylesheet, Hashtable localProperties, List<XmlElement> sourceContext)
        {
            string styleFromStylesheet = stylesheet.GetStyle(elementName, sourceContext);

            string styleInline = HtmlToXamlConverter.GetAttribute(htmlElement, "style");

            // Combine styles from stylesheet and from inline attribute.
            // The order is important - the latter styles will override the former.
            string style = styleFromStylesheet != null ? styleFromStylesheet : null;
            if (styleInline != null)
            {
                style = style == null ? styleInline : (style + ";" + styleInline);
            }

            // Apply local style to current formatting properties
            if (style != null)
            {
                string[] styleValues = style.Split(';');
                for (int i = 0; i < styleValues.Length; i++)
                {
                    string[] styleNameValue;

                    styleNameValue = styleValues[i].Split(':');
                    if (styleNameValue.Length == 2)
                    {
                        string styleName = styleNameValue[0].Trim().ToLower();
                        string styleValue = HtmlToXamlConverter.UnQuote(styleNameValue[1].Trim()).ToLower();
                        int nextIndex = 0;

                        switch (styleName)
                        {
                            case "font":
                                ParseCssFont(styleValue, localProperties);
                                break;
                            case "font-family":
                                ParseCssFontFamily(styleValue, ref nextIndex, localProperties);
                                break;
                            case "font-size":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, "font-size", /*mustBeNonNegative:*/true);
                                break;
                            case "font-style":
                                ParseCssFontStyle(styleValue, ref nextIndex, localProperties);
                                break;
                            case "font-weight":
                                ParseCssFontWeight(styleValue, ref nextIndex, localProperties);
                                break;
                            case "font-variant":
                                ParseCssFontVariant(styleValue, ref nextIndex, localProperties);
                                break;
                            case "line-height":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, "line-height", /*mustBeNonNegative:*/true);
                                break;
                            case "word-spacing":
                                // Implement word-spacing conversion
                                break;
                            case "letter-spacing":
                                // Implement letter-spacing conversion
                                break;
                            case "color":
                                ParseCssColor(styleValue, ref nextIndex, localProperties, "color");
                                break;

                            case "text-decoration":
                                ParseCssTextDecoration(styleValue, ref nextIndex, localProperties);
                                break;

                            case "text-transform":
                                ParseCssTextTransform(styleValue, ref nextIndex, localProperties);
                                break;

                            case "background-color":
                                ParseCssColor(styleValue, ref nextIndex, localProperties, "background-color");
                                break;
                            case "background":
                                // TODO: need to parse composite background property
                                ParseCssBackground(styleValue, ref nextIndex, localProperties);
                                break;

                            case "text-align":
                                ParseCssTextAlign(styleValue, ref nextIndex, localProperties);
                                break;
                            case "vertical-align":
                                ParseCssVerticalAlign(styleValue, ref nextIndex, localProperties);
                                break;
                            case "text-indent":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, "text-indent", /*mustBeNonNegative:*/false);
                                break;

                            case "width":
                            case "height":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, styleName, /*mustBeNonNegative:*/true);
                                break;

                            case "margin": // top/right/bottom/left
                                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, styleName);
                                break;
                            case "margin-top":
                            case "margin-right":
                            case "margin-bottom":
                            case "margin-left":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, styleName, /*mustBeNonNegative:*/true);
                                break;

                            case "padding":
                                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, styleName);
                                break;
                            case "padding-top":
                            case "padding-right":
                            case "padding-bottom":
                            case "padding-left":
                                ParseCssSize(styleValue, ref nextIndex, localProperties, styleName, /*mustBeNonNegative:*/true);
                                break;

                            case "border":
                                ParseCssBorder(styleValue, ref nextIndex, localProperties);
                                break;
                            case "border-style":
                            case "border-width":
                            case "border-color":
                                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, styleName);
                                break;
                            case "border-top":
                            case "border-right":
                            case "border-left":
                            case "border-bottom":
                                // Parse CSS border style
                                break;

                            // NOTE: CSS names for elementary border styles have side indications in the middle (top/bottom/left/right)
                            // In our internal notation we intentionally put them at the end - to unify processing in ParseCssRectangleProperty method
                            case "border-top-style":
                            case "border-right-style":
                            case "border-left-style":
                            case "border-bottom-style":
                            case "border-top-color":
                            case "border-right-color":
                            case "border-left-color":
                            case "border-bottom-color":
                            case "border-top-width":
                            case "border-right-width":
                            case "border-left-width":
                            case "border-bottom-width":
                                // Parse CSS border style
                                break;

                            case "display":
                                // Implement display style conversion
                                break;

                            case "float":
                                ParseCssFloat(styleValue, ref nextIndex, localProperties);
                                break;
                            case "clear":
                                ParseCssClear(styleValue, ref nextIndex, localProperties);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        // .................................................................
        //
        // Parsing CSS - Lexical Helpers
        //
        // .................................................................

        /// <summary>
        /// Skips whitespaces in style values
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        private static void ParseWhiteSpace(string styleValue, ref int nextIndex)
        {
            while (nextIndex < styleValue.Length && char.IsWhiteSpace(styleValue[nextIndex]))
            {
                nextIndex++;
            }
        }

        /// <summary>
        /// Checks if the following character matches to a given word and advances nextIndex
        /// by the word's length in case of success.
        /// Otherwise leaves nextIndex in place (except for possible whitespaces).
        /// Returns true or false depending on success or failure of matching.
        /// </summary>
        /// <param name="word">Word to match</param>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">next Index</param>
        /// <returns>True if successful, false on failure</returns>
        private static bool ParseWord(string word, string styleValue, ref int nextIndex)
        {
            ParseWhiteSpace(styleValue, ref nextIndex);

            for (int i = 0; i < word.Length; i++)
            {
                if (!(nextIndex + i < styleValue.Length && word[i] == styleValue[nextIndex + i]))
                {
                    return false;
                }
            }

            if (nextIndex + word.Length < styleValue.Length && char.IsLetterOrDigit(styleValue[nextIndex + word.Length]))
            {
                return false;
            }

            nextIndex += word.Length;
            return true;
        }

        /// <summary>
        /// Checks whether the following character sequence matches one of the given words,
        /// and advances the nextIndex to matched word length.
        /// Returns null in case if there is no match or the word matched.
        /// </summary>
        /// <param name="words">Words to match</param>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <returns>Words matched, or null if not matched</returns>
        private static string ParseWordEnumeration(string[] words, string styleValue, ref int nextIndex)
        {
            for (int i = 0; i < words.Length; i++)
            {
                if (ParseWord(words[i], styleValue, ref nextIndex))
                {
                    return words[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Parse word enumeration
        /// </summary>
        /// <param name="words">Words to search through</param>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        /// <param name="attributeName">Attribute name</param>
        private static void ParseWordEnumeration(string[] words, string styleValue, ref int nextIndex, Hashtable localProperties, string attributeName)
        {
            string attributeValue = ParseWordEnumeration(words, styleValue, ref nextIndex);
            if (attributeValue != null)
            {
                localProperties[attributeName] = attributeValue;
            }
        }

        /// <summary>
        /// Parse CSS Size
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="mustBeNonNegative">Whether or not the value must be non-negative</param>
        /// <returns>string containing the result</returns>
        private static string ParseCssSize(string styleValue, ref int nextIndex, bool mustBeNonNegative)
        {
            ParseWhiteSpace(styleValue, ref nextIndex);

            int startIndex = nextIndex;

            // Parse optional munis sign
            if (nextIndex < styleValue.Length && styleValue[nextIndex] == '-')
            {
                nextIndex++;
            }

            if (nextIndex < styleValue.Length && char.IsDigit(styleValue[nextIndex]))
            {
                while (nextIndex < styleValue.Length && (char.IsDigit(styleValue[nextIndex]) || styleValue[nextIndex] == '.'))
                {
                    nextIndex++;
                }

                string number = styleValue.Substring(startIndex, nextIndex - startIndex);

                string unit = ParseWordEnumeration(FontSizeUnits, styleValue, ref nextIndex);
                if (unit == null)
                {
                    unit = "px"; // Assuming pixels by default
                }

                if (mustBeNonNegative && styleValue[startIndex] == '-')
                {
                    return "0";
                }
                else
                {
                    return number + unit;
                }
            }

            return null;
        }

        /// <summary>
        /// Parse CSS Size
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localValues">Local values</param>
        /// <param name="propertyName">Property name</param>
        /// <param name="mustBeNonNegative">Whether or not the value must be non-negative</param>
        private static void ParseCssSize(string styleValue, ref int nextIndex, Hashtable localValues, string propertyName, bool mustBeNonNegative)
        {
            string length = ParseCssSize(styleValue, ref nextIndex, mustBeNonNegative);
            if (length != null)
            {
                localValues[propertyName] = length;
            }
        }

        /// <summary>
        /// Parse CSS Color
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <returns>A string specifying the color</returns>
        private static string ParseCssColor(string styleValue, ref int nextIndex)
        {
            // Implement color parsing
            // rgb(100%,53.5%,10%)
            // rgb(255,91,26)
            // #FF5B1A
            // black | silver | gray | ... | aqua
            // transparent - for background-color
            ParseWhiteSpace(styleValue, ref nextIndex);

            string color = null;

            if (nextIndex < styleValue.Length)
            {
                int startIndex = nextIndex;
                char character = styleValue[nextIndex];

                if (character == '#')
                {
                    nextIndex++;
                    while (nextIndex < styleValue.Length)
                    {
                        character = char.ToUpper(styleValue[nextIndex]);
                        if (!((('0' <= character) && (character <= '9')) || (('A' <= character) && (character <= 'F'))))
                        {
                            break;
                        }

                        nextIndex++;
                    }

                    if (nextIndex > startIndex + 1)
                    {
                        color = styleValue.Substring(startIndex, nextIndex - startIndex);
                    }
                }
                else if (styleValue.Substring(nextIndex, 3).ToLower() == "rbg")
                {
                    // Implement real rgb() color parsing
                    while (nextIndex < styleValue.Length && styleValue[nextIndex] != ')')
                    {
                        nextIndex++;
                    }

                    if (nextIndex < styleValue.Length)
                    {
                        nextIndex++; // to skip ')'
                    }

                    color = "gray"; // return bogus color
                }
                else if (char.IsLetter(character))
                {
                    color = ParseWordEnumeration(Colors, styleValue, ref nextIndex);
                    if (color == null)
                    {
                        color = ParseWordEnumeration(SystemColors, styleValue, ref nextIndex);
                        if (color != null)
                        {
                            // Implement smarter system color converions into real colors
                            color = "black";
                        }
                    }
                }
            }

            return color;
        }

        /// <summary>
        /// Parse CSS color
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localValues">Local values</param>
        /// <param name="propertyName">Property name</param>
        private static void ParseCssColor(string styleValue, ref int nextIndex, Hashtable localValues, string propertyName)
        {
            string color = ParseCssColor(styleValue, ref nextIndex);
            if (color != null)
            {
                localValues[propertyName] = color;
            }
        }

        // .................................................................
        //
        // Pasring CSS font Property
        //
        // .................................................................

        /// <summary>
        /// Parses CSS string fontStyle representing a value for CSS font attribute
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssFont(string styleValue, Hashtable localProperties)
        {
            int nextIndex = 0;

            ParseCssFontStyle(styleValue, ref nextIndex, localProperties);
            ParseCssFontVariant(styleValue, ref nextIndex, localProperties);
            ParseCssFontWeight(styleValue, ref nextIndex, localProperties);

            ParseCssSize(styleValue, ref nextIndex, localProperties, "font-size", /*mustBeNonNegative:*/true);

            ParseWhiteSpace(styleValue, ref nextIndex);
            if (nextIndex < styleValue.Length && styleValue[nextIndex] == '/')
            {
                nextIndex++;
                ParseCssSize(styleValue, ref nextIndex, localProperties, "line-height", /*mustBeNonNegative:*/true);
            }

            ParseCssFontFamily(styleValue, ref nextIndex, localProperties);
        }

        /// <summary>
        /// Parse CSS Font Style
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssFontStyle(string styleValue, ref int nextIndex, Hashtable localProperties)
        {
            ParseWordEnumeration(FontStyles, styleValue, ref nextIndex, localProperties, "font-style");
        }

        /// <summary>
        /// Parse CSS Font Variant
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssFontVariant(string styleValue, ref int nextIndex, Hashtable localProperties)
        {
            ParseWordEnumeration(FontVariants, styleValue, ref nextIndex, localProperties, "font-variant");
        }

        /// <summary>
        /// Parse CSS Font Weight
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssFontWeight(string styleValue, ref int nextIndex, Hashtable localProperties)
        {
            ParseWordEnumeration(FontWeights, styleValue, ref nextIndex, localProperties, "font-weight");
        }

        /// <summary>
        /// Parse CSS Font Family
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssFontFamily(string styleValue, ref int nextIndex, Hashtable localProperties)
        {
            string fontFamilyList = null;

            while (nextIndex < styleValue.Length)
            {
                // Try generic-family
                string fontFamily = ParseWordEnumeration(FontGenericFamilies, styleValue, ref nextIndex);

                if (fontFamily == null)
                {
                    // Try quoted font family name
                    if (nextIndex < styleValue.Length && (styleValue[nextIndex] == '"' || styleValue[nextIndex] == '\''))
                    {
                        char quote = styleValue[nextIndex];

                        nextIndex++;

                        int startIndex = nextIndex;

                        while (nextIndex < styleValue.Length && styleValue[nextIndex] != quote)
                        {
                            nextIndex++;
                        }

                        fontFamily = '"' + styleValue.Substring(startIndex, nextIndex - startIndex) + '"';
                    }

                    if (fontFamily == null)
                    {
                        // Try unquoted font family name
                        int startIndex = nextIndex;
                        while (nextIndex < styleValue.Length && styleValue[nextIndex] != ',' && styleValue[nextIndex] != ';')
                        {
                            nextIndex++;
                        }

                        if (nextIndex > startIndex)
                        {
                            fontFamily = styleValue.Substring(startIndex, nextIndex - startIndex).Trim();
                            if (fontFamily.Length == 0)
                            {
                                fontFamily = null;
                            }
                        }
                    }
                }

                ParseWhiteSpace(styleValue, ref nextIndex);
                if (nextIndex < styleValue.Length && styleValue[nextIndex] == ',')
                {
                    nextIndex++;
                }

                if (fontFamily != null)
                {
                    // CSS font-family can contein a list of names. We only consider the first name from the list. Need a decision what to do with remaining names
                    // fontFamilyList = (fontFamilyList == null) ? fontFamily : fontFamilyList + "," + fontFamily;
                    if (fontFamilyList == null && fontFamily.Length > 0)
                    {
                        if (fontFamily[0] == '"' || fontFamily[0] == '\'')
                        {
                            // Unquote the font family name
                            fontFamily = fontFamily.Substring(1, fontFamily.Length - 2);
                        }
                        else
                        {
                            // Convert generic CSS family name
                        }

                        fontFamilyList = fontFamily;
                    }
                }
                else
                {
                    break;
                }
            }

            if (fontFamilyList != null)
            {
                localProperties["font-family"] = fontFamilyList;
            }
        }

        // .................................................................
        //
        // Pasring CSS list-style Property
        //
        // .................................................................

        // list-style: [ <list-style-type> || <list-style-position> || <list-style-image> ]

        /// <summary>
        /// Parse CSS List Style
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssListStyle(string styleValue, Hashtable localProperties)
        {
            int nextIndex = 0;

            while (nextIndex < styleValue.Length)
            {
                string listStyleType = ParseCssListStyleType(styleValue, ref nextIndex);
                if (listStyleType != null)
                {
                    localProperties["list-style-type"] = listStyleType;
                }
                else
                {
                    string listStylePosition = ParseCssListStylePosition(styleValue, ref nextIndex);
                    if (listStylePosition != null)
                    {
                        localProperties["list-style-position"] = listStylePosition;
                    }
                    else
                    {
                        string listStyleImage = ParseCssListStyleImage(styleValue, ref nextIndex);
                        if (listStyleImage != null)
                        {
                            localProperties["list-style-image"] = listStyleImage;
                        }
                        else
                        {
                            // TODO: Process unrecognized list style value
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parse CSS List Style
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <returns>Matching word, if found</returns>
        private static string ParseCssListStyleType(string styleValue, ref int nextIndex)
        {
            return ParseWordEnumeration(ListStyleTypes, styleValue, ref nextIndex);
        }

        /// <summary>
        /// Parse CSS List Style Position
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <returns>Matching word, if found</returns>
        private static string ParseCssListStylePosition(string styleValue, ref int nextIndex)
        {
            return ParseWordEnumeration(ListStylePositions, styleValue, ref nextIndex);
        }

        /// <summary>
        /// Parse CSS List Style Image - Not yet implemented
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <returns>Always returns null</returns>
        private static string ParseCssListStyleImage(string styleValue, ref int nextIndex)
        {
            // TODO: Implement URL parsing for images
            return null;
        }

        // .................................................................
        //
        // Pasring CSS text-decorations Property
        //
        // .................................................................

        /// <summary>
        /// Parse CSS Text Decoration
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssTextDecoration(string styleValue, ref int nextIndex, Hashtable localProperties)
        {
            // Set default text-decorations:none;
            for (int i = 1; i < TextDecorations.Length; i++)
            {
                localProperties["text-decoration-" + TextDecorations[i]] = "false";
            }

            // Parse list of decorations values
            while (nextIndex < styleValue.Length)
            {
                string decoration = ParseWordEnumeration(TextDecorations, styleValue, ref nextIndex);
                if (decoration == null || decoration == "none")
                {
                    break;
                }

                localProperties["text-decoration-" + decoration] = "true";
            }
        }

        // .................................................................
        //
        // Pasring CSS text-transform Property
        //
        // .................................................................

        /// <summary>
        /// Parse CSS Text Transform
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssTextTransform(string styleValue, ref int nextIndex, Hashtable localProperties)
        {
            ParseWordEnumeration(TextTransforms, styleValue, ref nextIndex, localProperties, "text-transform");
        }

        // .................................................................
        //
        // Pasring CSS text-align Property
        //
        // .................................................................

        /// <summary>
        /// Parse CSS Text Align
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssTextAlign(string styleValue, ref int nextIndex, Hashtable localProperties)
        {
            ParseWordEnumeration(TextAligns, styleValue, ref nextIndex, localProperties, "text-align");
        }

        // .................................................................
        //
        // Pasring CSS vertical-align Property
        //
        // .................................................................

        /// <summary>
        /// Parse CSS Vertical Align
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssVerticalAlign(string styleValue, ref int nextIndex, Hashtable localProperties)
        {
            // Parse percentage value for vertical-align style
            ParseWordEnumeration(VerticalAligns, styleValue, ref nextIndex, localProperties, "vertical-align");
        }

        // .................................................................
        //
        // Pasring CSS float Property
        //
        // .................................................................

        /// <summary>
        /// Parse CSS Float
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssFloat(string styleValue, ref int nextIndex, Hashtable localProperties)
        {
            ParseWordEnumeration(Floats, styleValue, ref nextIndex, localProperties, "float");
        }

        // .................................................................
        //
        // Pasring CSS clear Property
        //
        // .................................................................

        /// <summary>
        /// Parse CSS Clear
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssClear(string styleValue, ref int nextIndex, Hashtable localProperties)
        {
            ParseWordEnumeration(Clears, styleValue, ref nextIndex, localProperties, "clear");
        }

        // .................................................................
        //
        // Pasring CSS margin and padding Properties
        //
        // .................................................................

        /// <summary>
        /// Generic method for parsing any of four-values properties, such as margin, padding, border-width, border-style, border-color
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>true on success, false on failure</returns>
        private static bool ParseCssRectangleProperty(string styleValue, ref int nextIndex, Hashtable localProperties, string propertyName)
        {
            // CSS Spec:
            // If only one value is set, then the value applies to all four sides;
            // If two or three values are set, then missinng value(s) are taken fromm the opposite side(s).
            // The order they are applied is: top/right/bottom/left
            Debug.Assert(propertyName == "margin" || propertyName == "padding" || propertyName == "border-width" || propertyName == "border-style" || propertyName == "border-color", "Unexpected property name");
            string value = propertyName == "border-color" ? ParseCssColor(styleValue, ref nextIndex) : propertyName == "border-style" ? ParseCssBorderStyle(styleValue, ref nextIndex) : ParseCssSize(styleValue, ref nextIndex, /*mustBeNonNegative:*/true);
            if (value != null)
            {
                localProperties[propertyName + "-top"] = value;
                localProperties[propertyName + "-bottom"] = value;
                localProperties[propertyName + "-right"] = value;
                localProperties[propertyName + "-left"] = value;
                value = propertyName == "border-color" ? ParseCssColor(styleValue, ref nextIndex) : propertyName == "border-style" ? ParseCssBorderStyle(styleValue, ref nextIndex) : ParseCssSize(styleValue, ref nextIndex, /*mustBeNonNegative:*/true);
                if (value != null)
                {
                    localProperties[propertyName + "-right"] = value;
                    localProperties[propertyName + "-left"] = value;
                    value = propertyName == "border-color" ? ParseCssColor(styleValue, ref nextIndex) : propertyName == "border-style" ? ParseCssBorderStyle(styleValue, ref nextIndex) : ParseCssSize(styleValue, ref nextIndex, /*mustBeNonNegative:*/true);
                    if (value != null)
                    {
                        localProperties[propertyName + "-bottom"] = value;
                        value = propertyName == "border-color" ? ParseCssColor(styleValue, ref nextIndex) : propertyName == "border-style" ? ParseCssBorderStyle(styleValue, ref nextIndex) : ParseCssSize(styleValue, ref nextIndex, /*mustBeNonNegative:*/true);
                        if (value != null)
                        {
                            localProperties[propertyName + "-left"] = value;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        // .................................................................
        //
        // Pasring CSS border Properties
        //
        // .................................................................

        // border: [ <border-width> || <border-style> || <border-color> ]

        /// <summary>
        /// Parse CSS Border
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localProperties">Local properties</param>
        private static void ParseCssBorder(string styleValue, ref int nextIndex, Hashtable localProperties)
        {
            while (
                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-width") ||
                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-style") ||
                ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-color"))
            {
            }
        }

        // .................................................................
        //
        // Pasring CSS border-style Propertie
        //
        // .................................................................

        /// <summary>
        /// Parse CSS Border Style
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <returns>Match if found</returns>
        private static string ParseCssBorderStyle(string styleValue, ref int nextIndex)
        {
            return ParseWordEnumeration(BorderStyles, styleValue, ref nextIndex);
        }

        // .................................................................
        //
        //  What are these definitions doing here:
        //
        // .................................................................

        // .................................................................
        //
        // Pasring CSS Background Properties
        //
        // .................................................................

        /// <summary>
        /// Parse CSS Background
        /// </summary>
        /// <param name="styleValue">Style value</param>
        /// <param name="nextIndex">Next index</param>
        /// <param name="localValues">Local values</param>
        private static void ParseCssBackground(string styleValue, ref int nextIndex, Hashtable localValues)
        {
            // Implement parsing background attribute
        }
    }
}