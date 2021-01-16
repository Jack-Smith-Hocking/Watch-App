using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Text Formatting + Strings
namespace Utility.String
{
    [System.Serializable]
    public class FormattedParagraph
    {
        public FormattedText m_heading;
        public FormattedText m_body;

        public string GetFormatted()
        {
            return m_heading.GetFormatted() + m_body.GetFormatted();
        }
    }
    [System.Serializable]
    public class FormattedText
    {
        [TextArea] [Tooltip("Main body of text")] public string m_text;
        [Space]
        [Min(0)]
        [Tooltip("Number of new lines after the text")] public int m_newLinesAfterText = 1;

        [Header("Size Details")]
        [Tooltip("Size of the text")] public int m_textSize = 10;
        [Tooltip("Whether to use the default text size")] public bool m_useDefaultSize = false;

        [Header("Colour Details")]
        [Tooltip("Colour to set the text as")] public Color m_textColour = Color.red;

        [Header("Style Details")]
        [Tooltip("Whether to make the text bold")] public bool m_boldText = false;
        [Tooltip("Whether to make the text italic")] public bool m_italicText = false;
        [Tooltip("Whether to make the text strike-through")] public bool m_strikeThrough = false;

        /// <summary>
        /// Format a string of text based on the setting of this instance
        /// </summary>
        /// <param name="text">The text to format</param>
        /// <returns>The origanl text formatted correctly</returns>
        public string FormatText(string text)
        {
            string _formattedText = text;

            if (m_boldText)
            {
                _formattedText = StringUtil.MakeBold(_formattedText);
            }
            if (m_italicText)
            {
                _formattedText = StringUtil.MakeItalic(_formattedText);
            }
            if (m_strikeThrough)
            {
                _formattedText = StringUtil.MakeStrikethrough(_formattedText);
            }
            if (!m_useDefaultSize)
            {
                _formattedText = StringUtil.SetSize(_formattedText, m_textSize);
            }

            _formattedText = StringUtil.SetColour(_formattedText, m_textColour);
            _formattedText = StringUtil.AddNewLines(_formattedText, m_newLinesAfterText);

            return _formattedText;
        }

        /// <summary>
        /// Formats the text associated with this instance
        /// </summary>
        /// <returns></returns>
        public string GetFormatted()
        {
            return FormatText(m_text);
        }
    }

    public static class StringUtil
    {
        /// <summary>
        /// Remove a string from another string
        /// </summary>
        /// <param name="original">The string to remove from</param>
        /// <param name="toExclude">The string to remove</param>
        /// <returns>The original string after toExclude is removed</returns>
        public static string Exclude(string original, string toExclude)
        {
            return original.Replace(toExclude, "");
        }
        /// <summary>
        /// Remove a list of strings from a list
        /// </summary>
        /// <param name="original">The string to remove from</param>
        /// <param name="toExclude">The list of strings to remove</param>
        /// <returns>The original string after all exclusions are done</returns>
        public static string MultiExclude(string original, List<string> toExclude)
        {
            // Go through each element in toExclude and remove it from the original string
            foreach (var s in toExclude)
            {
                original = Exclude(original, s);
            }

            return original;
        }
        /// <summary>
        /// Separate a string by upper cases (place a space before every upper case)
        /// </summary>
        /// <param name="text">The text to change</param>
        /// <returns>The text with spaces before every upper case</returns>
        public static string SeparateByUpperCase(string text)
        {
            string _newText = "";

            for (int i = 0; i < text.Length; i++)
            {
                // Check if the character is upper case
                // Add a space if it is
                if (char.IsUpper(text[i]) && i > 0)
                {
                    _newText += " ";
                }

                // Add the character to the new string
                _newText += text[i];
            }

            return _newText;
        }

        #region RichTextFormatting
        /// <summary>
        /// Prepend a string to a string, then append a string to the end of that result
        /// </summary>
        /// <param name="text">The string to prepend/append to</param>
        /// <param name="prependText">The string to add to the start of 'text'</param>
        /// <param name="appendText">The string to add to the end of 'text'</param>
        /// <returns>'text' with 'prependText' at the start and 'appendText' at the end</returns>
        public static string Prepend_Append(string text, string prependText, string appendText)
        {
            text = text.Insert(0, prependText);
            text = text.Insert(text.Length, appendText);

            return text;
        }
        /// <summary>
        /// Using mark-up make a string bold
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MakeBold(string text)
        {
            return Prepend_Append(text, "<b>", "</b>");
        }
        /// <summary>
        /// Using mark-up make a string italic
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string MakeItalic(string text)
        {
            return Prepend_Append(text, "<i>", "</i>");
        }
        public static string MakeStrikethrough(string text)
        {
            return Prepend_Append(text, "<s>", "</s>");
        }
        /// <summary>
        /// Using mark-up make a string a certain colour
        /// </summary>
        /// <param name="text"></param>
        /// <param name="textColour"></param>
        /// <returns></returns>
        public static string SetColour(string text, Color textColour)
        {
            string hex = "#" + ColorUtility.ToHtmlStringRGBA(textColour);

            return Prepend_Append(text, $"<color={hex}>", "</color>");
        }
        /// <summary>
        /// Using mark-up make a string a certain font size
        /// </summary>
        /// <param name="text"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string SetSize(string text, int size)
        {
            return Prepend_Append(text, $"<size={size}>", "</size>");
        }
        /// <summary>
        /// Add a mewline character to the end of a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string AddNewLine(string text)
        {
            text = text.Insert(text.Length, "\n");

            return text;
        }
        /// <summary>
        /// Add a number of newline characters to the end of a string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="numLines"></param>
        /// <returns></returns>
        public static string AddNewLines(string text, int numLines)
        {
            for (int i = 0; i < numLines; i++)
            {
                text = AddNewLine(text);
            }

            return text;
        }
        #endregion
    }
}