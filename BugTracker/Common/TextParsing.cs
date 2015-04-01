using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Blog
{
    public class TextParsing
    {
        public static StringBuilder GetWordsFromHTML_EqualToCharAmmount(string _html, int _chars)
        {
            return GetWordsToAMaxOf_Characters(
                StripAllHtmlTags_ReturnOnlyText(_html).ToString(), _chars
                );
        }

        /// <summary>
        /// It uses regex to eliminate all the tags and a manual method to eliminate
        /// special characters like "&nbsp;"
        /// </summary>
        /// <param name="_html"></param>
        /// <param name="_chars"></param>
        /// <returns></returns>
        public static StringBuilder GetWordsFromHTML_EqualToCharAmmount_Regex(string _html, int _chars)
        {
            Regex stripHtml = new Regex("<[^>]*>");

            string result = stripHtml.Replace(_html, "");
            result = HtmlTagRemover.HtmlStripTags(result, true, true);
            return GetWordsToAMaxOf_Characters(
                result, _chars
                );
        }

        public static StringBuilder GetWordsFromHTML_EqualToCharAmmount_Manually(string _html, int _chars)
        {
            return GetWordsToAMaxOf_Characters(
                HtmlTagRemover.HtmlStripTags(_html,true,true), _chars
                );
        }


        /// <summary>
        /// Kesps only the text buy removing the Html code/tags
        /// </summary>
        /// <param name="_html"></param>
        /// <returns></returns>
        public static StringBuilder StripAllHtmlTags_ReturnOnlyText(string _html)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            doc.LoadHtml(_html);
            if (doc == null) return new StringBuilder();

            StringBuilder output = new StringBuilder();
            foreach (var node in doc.DocumentNode.ChildNodes)
            {
                output.Append(node.InnerText);
            }

            return output.Replace("&nbsp;","");
        }

        /// <summary>
        /// Tries to give an ammount of wods that their characters and spaces together
        /// equals "_maxCharacters"
        /// </summary>
        /// <param name="_maxCharacters"></param>
        /// <returns></returns>
        public static StringBuilder GetWordsToAMaxOf_Characters(string text, int _maxCharacters)
        {
            string[] words = text.Split();
            //string[] words = item.Body.Split(' ');
            StringBuilder result = new StringBuilder();
            int charTotal = 0;
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Trim() != "")
                {
                    if (charTotal + words[i].Length + 1 > _maxCharacters) { break; }

                    result.Append(words[i] + ' ');
                    charTotal += words[i].Length + 1;
                }
            }

            return result;
        }
    }
}