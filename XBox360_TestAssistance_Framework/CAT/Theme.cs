// -----------------------------------------------------------------------
// <copyright file="Theme.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace CAT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Media;
    using System.Xml;

    /// <summary>
    /// Class containing a CAT theme
    /// </summary>
    public class Theme : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Theme" /> class.
        /// </summary>
        /// <param name="themeName">Name of the theme</param>
        public Theme(string themeName)
        {
            this.Name = themeName;

            string filePath = Path.Combine("Themes", themeName + ".cattheme");

            XmlDocument configFile = new XmlDocument();
            configFile.Load(filePath);
            if (configFile.DocumentElement.Name == "CATTheme")
            {
                XmlNode catThemeNode = configFile.DocumentElement;
                if (catThemeNode.ChildNodes.Count > 0)
                {
                    foreach (XmlNode n in catThemeNode.ChildNodes)
                    {
                        switch (n.Name)
                        {
                            case "ButtonBorderThickness":
                                {
                                    string valueString = string.Empty;
                                    if (n.Attributes["Value"] != null)
                                    {
                                        valueString = n.Attributes["Value"].InnerText.Trim();
                                    }

                                    uint valueInt;
                                    if (uint.TryParse(valueString, out valueInt))
                                    {
                                        this.ButtonBorderThickness = valueInt;
                                    }
                                }

                                break;
                            case "ButtonPadding":
                                {
                                    string valueString = string.Empty;
                                    if (n.Attributes["Value"] != null)
                                    {
                                        valueString = n.Attributes["Value"].InnerText.Trim();
                                    }

                                    uint valueInt;
                                    if (uint.TryParse(valueString, out valueInt))
                                    {
                                        this.ButtonPadding = valueInt;
                                    }
                                }

                                break;
                            case "ButtonCornerRadius":
                                {
                                    string valueString = string.Empty;
                                    if (n.Attributes["Value"] != null)
                                    {
                                        valueString = n.Attributes["Value"].InnerText.Trim();
                                    }

                                    uint valueInt;
                                    if (uint.TryParse(valueString, out valueInt))
                                    {
                                        this.ButtonCornerRadius = valueInt;
                                    }
                                }

                                break;
                            case "ButtonBorderThickness2":
                                {
                                    string valueString = string.Empty;
                                    if (n.Attributes["Value"] != null)
                                    {
                                        valueString = n.Attributes["Value"].InnerText.Trim();
                                    }

                                    uint valueInt;
                                    if (uint.TryParse(valueString, out valueInt))
                                    {
                                        this.ButtonBorderThickness2 = valueInt;
                                    }
                                }

                                break;
                            case "ButtonPadding2":
                                {
                                    string valueString = string.Empty;
                                    if (n.Attributes["Value"] != null)
                                    {
                                        valueString = n.Attributes["Value"].InnerText.Trim();
                                    }

                                    uint valueInt;
                                    if (uint.TryParse(valueString, out valueInt))
                                    {
                                        this.ButtonPadding2 = valueInt;
                                    }
                                }

                                break;
                            case "ButtonCornerRadius2":
                                {
                                    string valueString = string.Empty;
                                    if (n.Attributes["Value"] != null)
                                    {
                                        valueString = n.Attributes["Value"].InnerText.Trim();
                                    }

                                    uint valueInt;
                                    if (uint.TryParse(valueString, out valueInt))
                                    {
                                        this.ButtonCornerRadius2 = valueInt;
                                    }
                                }

                                break;
                            case "ToggleButtonBorderThickness":
                                {
                                    string valueString = string.Empty;
                                    if (n.Attributes["Value"] != null)
                                    {
                                        valueString = n.Attributes["Value"].InnerText.Trim();
                                    }

                                    uint valueInt;
                                    if (uint.TryParse(valueString, out valueInt))
                                    {
                                        this.ToggleButtonBorderThickness = valueInt;
                                    }
                                }

                                break;
                            case "ToggleButtonPadding":
                                {
                                    string valueString = string.Empty;
                                    if (n.Attributes["Value"] != null)
                                    {
                                        valueString = n.Attributes["Value"].InnerText.Trim();
                                    }

                                    uint valueInt;
                                    if (uint.TryParse(valueString, out valueInt))
                                    {
                                        this.ToggleButtonPadding = valueInt;
                                    }
                                }

                                break;
                            case "ToggleButtonCornerRadius":
                                {
                                    string valueString = string.Empty;
                                    if (n.Attributes["Value"] != null)
                                    {
                                        valueString = n.Attributes["Value"].InnerText.Trim();
                                    }

                                    uint valueInt;
                                    if (uint.TryParse(valueString, out valueInt))
                                    {
                                        this.ToggleButtonCornerRadius = valueInt;
                                    }
                                }

                                break;
                            default:
                                {
                                    string valueString = "White";
                                    if (n.Attributes["Value"] != null)
                                    {
                                        valueString = n.Attributes["Value"].InnerText.Trim();
                                    }

                                    string brushType = string.Empty;
                                    if (n.Attributes["Type"] != null)
                                    {
                                        brushType = n.Attributes["Type"].InnerText.Trim();
                                    }

                                    string valueString2 = "White";
                                    if (n.Attributes["Value2"] != null)
                                    {
                                        valueString2 = n.Attributes["Value2"].InnerText.Trim();
                                    }

                                    string gradientAngleString = "0";
                                    if (n.Attributes["Angle"] != null)
                                    {
                                        gradientAngleString = n.Attributes["Angle"].InnerText.Trim();
                                    }

                                    string gradientStartString = "0,0";
                                    if (n.Attributes["GradientStart"] != null)
                                    {
                                        gradientStartString = n.Attributes["GradientStart"].InnerText.Trim();
                                    }

                                    string gradientEndString = "0,0";
                                    if (n.Attributes["GradientEnd"] != null)
                                    {
                                        gradientEndString = n.Attributes["GradientEnd"].InnerText.Trim();
                                    }

                                    double gradientAngle = 0;
                                    Point gradientStart = new Point(0, 0);
                                    Point gradientEnd = new Point(0, 0);
                                    double.TryParse(gradientAngleString, out gradientAngle);
                                    try
                                    {
                                        gradientStart = Point.Parse(gradientStartString);
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    try
                                    {
                                        gradientEnd = Point.Parse(gradientEndString);
                                    }
                                    catch (Exception)
                                    {
                                    }

                                    Color brushColor;
                                    try
                                    {
                                        brushColor = (Color)ColorConverter.ConvertFromString(valueString);
                                    }
                                    catch (Exception)
                                    {
                                        brushColor = Colors.Black;
                                    }

                                    Color brushColor2;
                                    try
                                    {
                                        brushColor2 = (Color)ColorConverter.ConvertFromString(valueString2);
                                    }
                                    catch (Exception)
                                    {
                                        brushColor2 = brushColor;
                                    }

                                    Brush brush;
                                    if (brushType == "LinearGradient")
                                    {
                                        if (gradientStart == gradientEnd)
                                        {
                                            brush = new LinearGradientBrush(brushColor, brushColor2, gradientAngle);
                                        }
                                        else
                                        {
                                            brush = new LinearGradientBrush(brushColor, brushColor2, gradientStart, gradientEnd);
                                        }
                                    }
                                    else if (brushType == "RadialGradient")
                                    {
                                        brush = new RadialGradientBrush(brushColor, brushColor2);
                                    }
                                    else
                                    {
                                        // Default to brushType == "Solid"
                                        brush = new SolidColorBrush(brushColor);
                                    }

                                    switch (n.Name)
                                    {
                                        case "Foreground1":
                                            this.Foreground1 = brush;
                                            break;
                                        case "Foreground2":
                                            this.Foreground2 = brush;
                                            break;
                                        case "Foreground3":
                                            this.Foreground3 = brush;
                                            break;
                                        case "Foreground4":
                                            this.Foreground4 = brush;
                                            break;
                                        case "Background1":
                                            this.Background1 = brush;
                                            break;
                                        case "Background2":
                                            this.Background2 = brush;
                                            break;
                                        case "Background3":
                                            this.Background3 = brush;
                                            break;
                                        case "Background4":
                                            this.Background4 = brush;
                                            break;
                                        case "ButtonForeground":
                                            this.ButtonForeground = brush;
                                            break;
                                        case "ButtonForegroundDisabled":
                                            this.ButtonForegroundDisabled = brush;
                                            break;
                                        case "ButtonForegroundPressed":
                                            this.ButtonForegroundPressed = brush;
                                            break;
                                        case "ButtonForegroundMouseOver":
                                            this.ButtonForegroundMouseOver = brush;
                                            break;
                                        case "ButtonBackground":
                                            this.ButtonBackground = brush;
                                            break;
                                        case "ButtonBackgroundDisabled":
                                            this.ButtonBackgroundDisabled = brush;
                                            break;
                                        case "ButtonBackgroundPressed":
                                            this.ButtonBackgroundPressed = brush;
                                            break;
                                        case "ButtonBackgroundMouseOver":
                                            this.ButtonBackgroundMouseOver = brush;
                                            break;
                                        case "ButtonForeground2":
                                            this.ButtonForeground2 = brush;
                                            break;
                                        case "ButtonForegroundDisabled2":
                                            this.ButtonForegroundDisabled2 = brush;
                                            break;
                                        case "ButtonForegroundPressed2":
                                            this.ButtonForegroundPressed2 = brush;
                                            break;
                                        case "ButtonForegroundMouseOver2":
                                            this.ButtonForegroundMouseOver2 = brush;
                                            break;
                                        case "ButtonBackground2":
                                            this.ButtonBackground2 = brush;
                                            break;
                                        case "ButtonBackgroundDisabled2":
                                            this.ButtonBackgroundDisabled2 = brush;
                                            break;
                                        case "ButtonBackgroundPressed2":
                                            this.ButtonBackgroundPressed2 = brush;
                                            break;
                                        case "ButtonBackgroundMouseOver2":
                                            this.ButtonBackgroundMouseOver2 = brush;
                                            break;
                                        case "ToggleButtonForeground":
                                            this.ToggleButtonForeground = brush;
                                            break;
                                        case "ToggleButtonForegroundDisabled":
                                            this.ToggleButtonForegroundDisabled = brush;
                                            break;
                                        case "ToggleButtonForegroundPressed":
                                            this.ToggleButtonForegroundPressed = brush;
                                            break;
                                        case "ToggleButtonForegroundMouseOver":
                                            this.ToggleButtonForegroundMouseOver = brush;
                                            break;
                                        case "ToggleButtonForegroundChecked":
                                            this.ToggleButtonForegroundChecked = brush;
                                            break;
                                        case "ToggleButtonBackground":
                                            this.ToggleButtonBackground = brush;
                                            break;
                                        case "ToggleButtonBackgroundDisabled":
                                            this.ToggleButtonBackgroundDisabled = brush;
                                            break;
                                        case "ToggleButtonBackgroundPressed":
                                            this.ToggleButtonBackgroundPressed = brush;
                                            break;
                                        case "ToggleButtonBackgroundMouseOver":
                                            this.ToggleButtonBackgroundMouseOver = brush;
                                            break;
                                        case "ToggleButtonBackgroundChecked":
                                            this.ToggleButtonBackgroundChecked = brush;
                                            break;
                                        case "ComboBoxForeground":
                                            this.ComboBoxForeground = brush;
                                            break;
                                        case "ComboBoxForegroundDisabled":
                                            this.ComboBoxForegroundDisabled = brush;
                                            break;
                                        case "ComboBoxForegroundMouseOver":
                                            this.ComboBoxForegroundMouseOver = brush;
                                            break;
                                        case "ComboBoxForegroundPressed":
                                            this.ComboBoxForegroundPressed = brush;
                                            break;
                                        case "ComboBoxBackground":
                                            this.ComboBoxBackground = brush;
                                            break;
                                        case "ComboBoxBackgroundDisabled":
                                            this.ComboBoxBackgroundDisabled = brush;
                                            break;
                                        case "ComboBoxBackgroundMouseOver":
                                            this.ComboBoxBackgroundMouseOver = brush;
                                            break;
                                        case "ComboBoxBackgroundPressed":
                                            this.ComboBoxBackgroundPressed = brush;
                                            break;
                                        case "TextBoxForeground":
                                            this.TextBoxForeground = brush;
                                            break;
                                        case "TextBoxBackground":
                                            this.TextBoxBackground = brush;
                                            break;
                                        case "ComboBoxItemForeground":
                                            this.ComboBoxItemForeground = brush;
                                            break;
                                        case "ComboBoxItemBackground":
                                            this.ComboBoxItemBackground = brush;
                                            break;
                                        case "TabControlForeground":
                                            this.TabControlForeground = brush;
                                            break;
                                        case "TabControlBackground":
                                            this.TabControlBackground = brush;
                                            break;
                                        case "TabControlBorderColor":
                                            this.TabControlBorderColor = brush;
                                            break;
                                        case "TabItemForeground":
                                            this.TabItemForeground = brush;
                                            break;
                                        case "TabItemBackground":
                                            this.TabItemBackground = brush;
                                            break;
                                        case "TabItemBorderColor":
                                            this.TabItemBorderColor = brush;
                                            break;
                                        case "TabItemForegroundDeselected":
                                            this.TabItemForegroundDeselected = brush;
                                            break;
                                        case "TabItemBackgroundDeselected":
                                            this.TabItemBackgroundDeselected = brush;
                                            break;
                                        case "TabItemBorderColorDeselected":
                                            this.TabItemBorderColorDeselected = brush;
                                            break;
                                        default:
                                            break;
                                    }
                                }

                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// PropertyChanged event used by NotifyPropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a list of all themes
        /// </summary>
        public static List<string> ThemeNames
        {
            get
            {
                List<string> themeNames = new List<string>();
                DirectoryInfo dirInfo = new DirectoryInfo("Themes");
                FileInfo[] files = dirInfo.GetFiles("*.cattheme", SearchOption.TopDirectoryOnly);
                foreach (FileInfo fileInfo in files)
                {
                    themeNames.Add(Path.GetFileNameWithoutExtension(fileInfo.Name));
                }

                return themeNames;
            }
        }

        /// <summary>
        /// Gets or sets the name of the theme
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Foreground1 color in the theme
        /// </summary>
        public Brush Foreground1 { get; set; }

        /// <summary>
        /// Gets or sets the Foreground2 color in the theme
        /// </summary>
        public Brush Foreground2 { get; set; }

        /// <summary>
        /// Gets or sets the Foreground3 color in the theme
        /// </summary>
        public Brush Foreground3 { get; set; }

        /// <summary>
        /// Gets or sets the Foreground3 color in the theme
        /// </summary>
        public Brush Foreground4 { get; set; }

        /// <summary>
        /// Gets or sets the Background1 color in the theme
        /// </summary>
        public Brush Background1 { get; set; }

        /// <summary>
        /// Gets or sets the Background2 color in the theme
        /// </summary>
        public Brush Background2 { get; set; }

        /// <summary>
        /// Gets or sets the Background3 color in the theme
        /// </summary>
        public Brush Background3 { get; set; }

        /// <summary>
        /// Gets or sets the Background4 color in the theme
        /// </summary>
        public Brush Background4 { get; set; }

        /// <summary>
        /// Gets or sets the ButtonForeground color in the theme
        /// </summary>
        public Brush ButtonForeground { get; set; }

        /// <summary>
        /// Gets or sets the ButtonForegroundDisabled color in the theme
        /// </summary>
        public Brush ButtonForegroundDisabled { get; set; }

        /// <summary>
        /// Gets or sets the ButtonForegroundPressed color in the theme
        /// </summary>
        public Brush ButtonForegroundPressed { get; set; }

        /// <summary>
        /// Gets or sets the ButtonForegroundMouseOver color in the theme
        /// </summary>
        public Brush ButtonForegroundMouseOver { get; set; }

        /// <summary>
        /// Gets or sets the ButtonBackground color in the theme
        /// </summary>
        public Brush ButtonBackground { get; set; }

        /// <summary>
        /// Gets or sets the ButtonBackgroundDisabled color in the theme
        /// </summary>
        public Brush ButtonBackgroundDisabled { get; set; }

        /// <summary>
        /// Gets or sets the ButtonBackgroundPressed color in the theme
        /// </summary>
        public Brush ButtonBackgroundPressed { get; set; }

        /// <summary>
        /// Gets or sets the ButtonBackgroundMouseOver color in the theme
        /// </summary>
        public Brush ButtonBackgroundMouseOver { get; set; }

        /// <summary>
        /// Gets or sets the ButtonBorderThickness color in the theme
        /// </summary>
        public uint ButtonBorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the ButtonPadding color in the theme
        /// </summary>
        public uint ButtonPadding { get; set; }

        /// <summary>
        /// Gets or sets the ButtonCornerRadius color in the theme
        /// </summary>
        public uint ButtonCornerRadius { get; set; }

        /// <summary>
        /// Gets or sets the ButtonForeground2 color in the theme
        /// </summary>
        public Brush ButtonForeground2 { get; set; }

        /// <summary>
        /// Gets or sets the ButtonForegroundDisabled2 color in the theme
        /// </summary>
        public Brush ButtonForegroundDisabled2 { get; set; }

        /// <summary>
        /// Gets or sets the ButtonForegroundPressed2 color in the theme
        /// </summary>
        public Brush ButtonForegroundPressed2 { get; set; }

        /// <summary>
        /// Gets or sets the ButtonForegroundMouseOver2 color in the theme
        /// </summary>
        public Brush ButtonForegroundMouseOver2 { get; set; }

        /// <summary>
        /// Gets or sets the ButtonBackground2 color in the theme
        /// </summary>
        public Brush ButtonBackground2 { get; set; }

        /// <summary>
        /// Gets or sets the ButtonBackgroundDisabled2 color in the theme
        /// </summary>
        public Brush ButtonBackgroundDisabled2 { get; set; }

        /// <summary>
        /// Gets or sets the ButtonBackgroundPressed2 color in the theme
        /// </summary>
        public Brush ButtonBackgroundPressed2 { get; set; }

        /// <summary>
        /// Gets or sets the ButtonBackgroundMouseOver2 color in the theme
        /// </summary>
        public Brush ButtonBackgroundMouseOver2 { get; set; }

        /// <summary>
        /// Gets or sets the ButtonBorderThickness2 color in the theme
        /// </summary>
        public uint ButtonBorderThickness2 { get; set; }
        
        /// <summary>
        /// Gets or sets the ButtonPadding2 color in the theme
        /// </summary>
        public uint ButtonPadding2 { get; set; }

        /// <summary>
        /// Gets or sets the ButtonCornerRadius2 color in the theme
        /// </summary>
        public uint ButtonCornerRadius2 { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonForeground color in the theme
        /// </summary>
        public Brush ToggleButtonForeground { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonForegroundDisabled color in the theme
        /// </summary>
        public Brush ToggleButtonForegroundDisabled { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonForegroundPressed color in the theme
        /// </summary>
        public Brush ToggleButtonForegroundPressed { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonForegroundMouseOver color in the theme
        /// </summary>
        public Brush ToggleButtonForegroundMouseOver { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonForegroundChecked color in the theme
        /// </summary>
        public Brush ToggleButtonForegroundChecked { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonBackground color in the theme
        /// </summary>
        public Brush ToggleButtonBackground { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonBackgroundDisabled color in the theme
        /// </summary>
        public Brush ToggleButtonBackgroundDisabled { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonBackgroundPressed color in the theme
        /// </summary>
        public Brush ToggleButtonBackgroundPressed { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonBackgroundMouseOver color in the theme
        /// </summary>
        public Brush ToggleButtonBackgroundMouseOver { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonBackgroundChecked color in the theme
        /// </summary>
        public Brush ToggleButtonBackgroundChecked { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonBorderThickness color in the theme
        /// </summary>
        public uint ToggleButtonBorderThickness { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonPadding color in the theme
        /// </summary>
        public uint ToggleButtonPadding { get; set; }

        /// <summary>
        /// Gets or sets the ToggleButtonCornerRadius color in the theme
        /// </summary>
        public uint ToggleButtonCornerRadius { get; set; }

        /// <summary>
        /// Gets or sets the TextBoxForeground color in the theme
        /// </summary>
        public Brush TextBoxForeground { get; set; }

        /// <summary>
        /// Gets or sets the TextBoxBackground color in the theme
        /// </summary>
        public Brush TextBoxBackground { get; set; }

        /// <summary>
        /// Gets or sets the ComboBoxForeground color in the theme
        /// </summary>
        public Brush ComboBoxForeground { get; set; }

        /// <summary>
        /// Gets or sets the ComboBoxForegroundDisabled color in the theme
        /// </summary>
        public Brush ComboBoxForegroundDisabled { get; set; }

        /// <summary>
        /// Gets or sets the ComboBoxForegroundMouseOver color in the theme
        /// </summary>
        public Brush ComboBoxForegroundMouseOver { get; set; }

        /// <summary>
        /// Gets or sets the ComboBoxForegroundPressed color in the theme
        /// </summary>
        public Brush ComboBoxForegroundPressed { get; set; }

        /// <summary>
        /// Gets or sets the ComboBoxBackground color in the theme
        /// </summary>
        public Brush ComboBoxBackground { get; set; }

        /// <summary>
        /// Gets or sets the ComboBoxBackgroundDisabled color in the theme
        /// </summary>
        public Brush ComboBoxBackgroundDisabled { get; set; }

        /// <summary>
        /// Gets or sets the ComboBoxBackgroundMouseOver color in the theme
        /// </summary>
        public Brush ComboBoxBackgroundMouseOver { get; set; }

        /// <summary>
        /// Gets or sets the ComboBoxBackgroundPressed color in the theme
        /// </summary>
        public Brush ComboBoxBackgroundPressed { get; set; }

        /// <summary>
        /// Gets or sets the ComboBoxItemForeground color in the theme
        /// </summary>
        public Brush ComboBoxItemForeground { get; set; }

        /// <summary>
        /// Gets or sets the ComboBoxItemBackground color in the theme
        /// </summary>
        public Brush ComboBoxItemBackground { get; set; }

        /// <summary>
        /// Gets or sets the TabControlForeground color in the theme
        /// </summary>
        public Brush TabControlForeground { get; set; }

        /// <summary>
        /// Gets or sets the TabControlBackground color in the theme
        /// </summary>
        public Brush TabControlBackground { get; set; }

        /// <summary>
        /// Gets or sets the TabControlBorderColor color in the theme
        /// </summary>
        public Brush TabControlBorderColor { get; set; }

        /// <summary>
        /// Gets or sets the TabItemForeground color in the theme
        /// </summary>
        public Brush TabItemForeground { get; set; }

        /// <summary>
        /// Gets or sets the TabItemBackground color in the theme
        /// </summary>
        public Brush TabItemBackground { get; set; }

        /// <summary>
        /// Gets or sets the TabItemBorderColor color in the theme
        /// </summary>
        public Brush TabItemBorderColor { get; set; }

        /// <summary>
        /// Gets or sets the TabItemForegroundDeselected color in the theme
        /// </summary>
        public Brush TabItemForegroundDeselected { get; set; }

        /// <summary>
        /// Gets or sets the TabItemBackgroundDeselected color in the theme
        /// </summary>
        public Brush TabItemBackgroundDeselected { get; set; }

        /// <summary>
        /// Gets or sets the TabItemBorderColorDeselected color in the theme
        /// </summary>
        public Brush TabItemBorderColorDeselected { get; set; }

        /// <summary>
        /// Copies the properties of one Theme to another
        /// </summary>
        /// <param name="toCopy">Theme to copy</param>
        public void Copy(Theme toCopy)
        {
            this.Name = toCopy.Name;

            this.Foreground1 = toCopy.Foreground1;
            this.Foreground2 = toCopy.Foreground2;
            this.Foreground3 = toCopy.Foreground3;
            this.Foreground4 = toCopy.Foreground4;

            this.Background1 = toCopy.Background1;
            this.Background2 = toCopy.Background2;
            this.Background3 = toCopy.Background3;
            this.Background4 = toCopy.Background4;

            this.ButtonForeground = toCopy.ButtonForeground;
            this.ButtonForegroundDisabled = toCopy.ButtonForegroundDisabled;
            this.ButtonForegroundPressed = toCopy.ButtonForegroundPressed;
            this.ButtonForegroundMouseOver = toCopy.ButtonForegroundMouseOver;

            this.ButtonBackground = toCopy.ButtonBackground;
            this.ButtonBackgroundDisabled = toCopy.ButtonBackgroundDisabled;
            this.ButtonBackgroundPressed = toCopy.ButtonBackgroundPressed;
            this.ButtonBackgroundMouseOver = toCopy.ButtonBackgroundMouseOver;

            this.ButtonBorderThickness = toCopy.ButtonBorderThickness;
            this.ButtonPadding = toCopy.ButtonPadding;
            this.ButtonCornerRadius = toCopy.ButtonCornerRadius;

            this.ButtonForeground2 = toCopy.ButtonForeground2;
            this.ButtonForegroundDisabled2 = toCopy.ButtonForegroundDisabled2;
            this.ButtonForegroundPressed2 = toCopy.ButtonForegroundPressed2;
            this.ButtonForegroundMouseOver2 = toCopy.ButtonForegroundMouseOver2;

            this.ButtonBackground2 = toCopy.ButtonBackground2;
            this.ButtonBackgroundDisabled2 = toCopy.ButtonBackgroundDisabled2;
            this.ButtonBackgroundPressed2 = toCopy.ButtonBackgroundPressed2;
            this.ButtonBackgroundMouseOver2 = toCopy.ButtonBackgroundMouseOver2;

            this.ButtonBorderThickness2 = toCopy.ButtonBorderThickness2;
            this.ButtonPadding2 = toCopy.ButtonPadding2;
            this.ButtonCornerRadius2 = toCopy.ButtonCornerRadius2;

            this.ToggleButtonForeground = toCopy.ToggleButtonForeground;
            this.ToggleButtonForegroundDisabled = toCopy.ToggleButtonForegroundDisabled;
            this.ToggleButtonForegroundPressed = toCopy.ToggleButtonForegroundPressed;
            this.ToggleButtonForegroundMouseOver = toCopy.ToggleButtonForegroundMouseOver;
            this.ToggleButtonForegroundChecked = toCopy.ToggleButtonForegroundChecked;

            this.ToggleButtonBackground = toCopy.ToggleButtonBackground;
            this.ToggleButtonBackgroundDisabled = toCopy.ToggleButtonBackgroundDisabled;
            this.ToggleButtonBackgroundPressed = toCopy.ToggleButtonBackgroundPressed;
            this.ToggleButtonBackgroundMouseOver = toCopy.ToggleButtonBackgroundMouseOver;
            this.ToggleButtonBackgroundChecked = toCopy.ToggleButtonBackgroundChecked;

            this.ToggleButtonBorderThickness = toCopy.ToggleButtonBorderThickness;
            this.ToggleButtonPadding = toCopy.ToggleButtonPadding;
            this.ToggleButtonCornerRadius = toCopy.ToggleButtonCornerRadius;

            this.TextBoxForeground = toCopy.TextBoxForeground;
            this.TextBoxBackground = toCopy.TextBoxBackground;

            this.ComboBoxForeground = toCopy.ComboBoxForeground;
            this.ComboBoxForegroundDisabled = toCopy.ComboBoxForegroundDisabled;
            this.ComboBoxForegroundMouseOver = toCopy.ComboBoxForegroundMouseOver;
            this.ComboBoxForegroundPressed = toCopy.ComboBoxForegroundPressed;
            this.ComboBoxBackground = toCopy.ComboBoxBackground;
            this.ComboBoxBackgroundDisabled = toCopy.ComboBoxBackgroundDisabled;
            this.ComboBoxBackgroundMouseOver = toCopy.ComboBoxBackgroundMouseOver;
            this.ComboBoxBackgroundPressed = toCopy.ComboBoxBackgroundPressed;

            this.ComboBoxItemForeground = toCopy.ComboBoxItemForeground;
            this.ComboBoxItemBackground = toCopy.ComboBoxItemBackground;

            this.TabControlForeground = toCopy.TabControlForeground;
            this.TabControlBackground = toCopy.TabControlBackground;
            this.TabControlBorderColor = toCopy.TabControlBorderColor;

            this.TabItemForeground = toCopy.TabItemForeground;
            this.TabItemBackground = toCopy.TabItemBackground;
            this.TabItemBorderColor = toCopy.TabItemBorderColor;

            this.TabItemForegroundDeselected = toCopy.TabItemForegroundDeselected;
            this.TabItemBackgroundDeselected = toCopy.TabItemBackgroundDeselected;
            this.TabItemBorderColorDeselected = toCopy.TabItemBorderColorDeselected;

            this.RefreshAll();
        }

        /// <summary>
        /// Notify property changes for all properties
        /// </summary>
        public void RefreshAll()
        {
            this.NotifyPropertyChanged("Name");

            this.NotifyPropertyChanged("Foreground1");
            this.NotifyPropertyChanged("Foreground2");
            this.NotifyPropertyChanged("Foreground3");
            this.NotifyPropertyChanged("Foreground4");

            this.NotifyPropertyChanged("Background1");
            this.NotifyPropertyChanged("Background2");
            this.NotifyPropertyChanged("Background3");
            this.NotifyPropertyChanged("Background4");

            this.NotifyPropertyChanged("ButtonForeground");
            this.NotifyPropertyChanged("ButtonForegroundDisabled");
            this.NotifyPropertyChanged("ButtonForegroundPressed");
            this.NotifyPropertyChanged("ButtonForegroundMouseOver");

            this.NotifyPropertyChanged("ButtonBackground");
            this.NotifyPropertyChanged("ButtonBackgroundDisabled");
            this.NotifyPropertyChanged("ButtonBackgroundPressed");
            this.NotifyPropertyChanged("ButtonBackgroundMouseOver");

            this.NotifyPropertyChanged("ButtonBorderThickness");
            this.NotifyPropertyChanged("ButtonPadding");
            this.NotifyPropertyChanged("ButtonCornerRadius");

            this.NotifyPropertyChanged("ButtonForeground2");
            this.NotifyPropertyChanged("ButtonForegroundDisabled2");
            this.NotifyPropertyChanged("ButtonForegroundPressed2");
            this.NotifyPropertyChanged("ButtonForegroundMouseOver2");

            this.NotifyPropertyChanged("ButtonBackground2");
            this.NotifyPropertyChanged("ButtonBackgroundDisabled2");
            this.NotifyPropertyChanged("ButtonBackgroundPressed2");
            this.NotifyPropertyChanged("ButtonBackgroundMouseOver2");

            this.NotifyPropertyChanged("ButtonBorderThickness2");
            this.NotifyPropertyChanged("ButtonPadding2");
            this.NotifyPropertyChanged("ButtonCornerRadius2");

            this.NotifyPropertyChanged("ToggleButtonForeground");
            this.NotifyPropertyChanged("ToggleButtonForegroundDisabled");
            this.NotifyPropertyChanged("ToggleButtonForegroundPressed");
            this.NotifyPropertyChanged("ToggleButtonForegroundMouseOver");
            this.NotifyPropertyChanged("ToggleButtonForegroundChecked");

            this.NotifyPropertyChanged("ToggleButtonBackground");
            this.NotifyPropertyChanged("ToggleButtonBackgroundDisabled");
            this.NotifyPropertyChanged("ToggleButtonBackgroundPressed");
            this.NotifyPropertyChanged("ToggleButtonBackgroundMouseOver");
            this.NotifyPropertyChanged("ToggleButtonBackgroundChecked");

            this.NotifyPropertyChanged("ToggleButtonBorderThickness");
            this.NotifyPropertyChanged("ToggleButtonPadding");
            this.NotifyPropertyChanged("ToggleButtonCornerRadius");

            this.NotifyPropertyChanged("TextBoxForeground");
            this.NotifyPropertyChanged("TextBoxBackground");
            this.NotifyPropertyChanged("TextBoxBrushColor");

            this.NotifyPropertyChanged("ComboBoxForeground");
            this.NotifyPropertyChanged("ComboBoxForegroundDisabled");
            this.NotifyPropertyChanged("ComboBoxForegroundMouseOver");
            this.NotifyPropertyChanged("ComboBoxForegroundPressed");

            this.NotifyPropertyChanged("ComboBoxBackground");
            this.NotifyPropertyChanged("ComboBoxBackgroundDisabled");
            this.NotifyPropertyChanged("ComboBoxBackgroundMouseOver");
            this.NotifyPropertyChanged("ComboBoxBackgroundPressed");

            this.NotifyPropertyChanged("ComboBoxItemForeground");
            this.NotifyPropertyChanged("ComboBoxItemBackground");

            this.NotifyPropertyChanged("TabControlForeground");
            this.NotifyPropertyChanged("TabControlBackground");
            this.NotifyPropertyChanged("TabControlBorderColor");

            this.NotifyPropertyChanged("TabItemForeground");
            this.NotifyPropertyChanged("TabItemBackground");
            this.NotifyPropertyChanged("TabItemBorderColor");

            this.NotifyPropertyChanged("TabItemForegroundDeselected");
            this.NotifyPropertyChanged("TabItemBackgroundDeselected");
            this.NotifyPropertyChanged("TabItemBorderColorDeselected");
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