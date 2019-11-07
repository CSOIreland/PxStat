using API;
using PxStat.System.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PxStat.Resources
{
    /// <summary>
    /// Implementation of IKeywordExtractor for Gaeilge
    /// </summary>
    public class Keyword_ga : IKeywordExtractor
    {
        Keyword keyword;

        /// <summary>
        /// Language ISO Code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// cultureInfo
        /// </summary>
        public CultureInfo cultureInfo { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lngIsoCode"></param>
        internal Keyword_ga()
        {
            this.LngIsoCode = "ga";
        }





        /// <summary>
        /// Gets a list from a comma separated string
        /// </summary>
        /// <param name="readString"></param>
        /// <returns></returns>
        public List<string> ExtractSplit(string readString)
        {
            return readString.Split(' ').ToList<string>();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool IsDoNotAmend(string word)
        {
            return false;
        }

        /// <summary>
        /// Most plurals from the 2nd to 5th declensions
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        //
        public string Pluralize(string word)
        {
            string outWord = word;
            List<string> wordList = keyword.GetStringList("inflection.plural");
            int last = 0;
            foreach (string s in wordList)
            {
                if (word.EndsWith(s))
                {
                    if (s.Length > last && word.Length > s.Length)
                    {
                        outWord = word.Substring(0, word.Length - s.Length);
                        last = s.Length;
                    }
                }
            }
            return outWord;
        }

        //Remove the final 'i' from the word
        /// <summary>
        /// Plurals from the first declension
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string PluralFirstDeclension(string word)
        {
            List<char> charList = word.ToCharArray().ToList<char>();
            List<char> charListOut = new List<char>();
            charList.Reverse();
            if (charList.Count > 2)
            {
                if (charList[1] != 'i')
                    charListOut = charList;
                else
                {
                    for (int i = 0; i < charList.Count; i++)
                    {
                        if (i != 1) charListOut.Add(charList[i]);
                    }
                }

            }
            charListOut.Reverse();
            string outstring = "";
            foreach (char c in charListOut) { outstring = outstring + c; }
            return outstring;
        }

        //
        /// <summary>
        /// If 'h' is the second letter of the word, remove it
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string Aspiration(string word)
        {
            //e.g. dative case
            if (word.Length < 3) return word;
            if (word.Substring(0, 1).Equals("h"))
            {
                if (IsSlender(word.Substring(1, 1).ToCharArray()[0]))
                {
                    word = word.Substring(1, word.Length - 1);

                }

            }
            //e.g. genitive case
            if (word.Length > 2)
            {
                if (word.Substring(1, 1).Equals("h"))
                {
                    word = word.Substring(0, 1) + word.Substring(2, word.Length - 2);
                }
            }

            return word;
        }
        //Remove the lenition from a word
        private string Lenition(string word)
        {
            string outWord = word;
            List<string> wordList = keyword.GetStringList("inflection.eclipsis");
            foreach (string s in wordList)
            {
                if (word.StartsWith(s))
                {
                    if (s.Equals("bhf"))
                    {
                        outWord = word.Substring(s.Length - 1, word.Length - (s.Length - 1));
                    }
                    else
                        outWord = word.Substring(s.Length, word.Length - s.Length);
                }
            }

            return outWord;
        }

        /// <summary>
        /// get a list of slender vowels
        /// </summary>
        /// <param name="inChar"></param>
        /// <returns></returns>
        private bool IsSlender(char inChar)
        {
            List<char> slenders = new List<char>() { 'a', 'e', 'i', 'o', 'u', 'á', 'é', 'í', 'ó', 'ú' };
            if (slenders.Contains(inChar))
                return true;
            else return false;
        }

        public bool IsPlural(string word)
        {
            throw new NotImplementedException();
        }

        public bool IsSingular(string word)
        {
            throw new NotImplementedException();
        }

        public string Singularize(string word)
        {
            string outWord = word;
            List<string> wordList = Utility.GetCustomConfig("APP_KEYWORD_GA_INFLECTED_PLURAL").Split(',').ToList<string>();
            int last = 0;
            foreach (string s in wordList)
            {
                if (word.EndsWith(s))
                {
                    if (s.Length > last && word.Length > s.Length)
                    {
                        outWord = word.Substring(0, word.Length - s.Length);
                        last = s.Length;
                    }
                }
            }
            return outWord;
        }
    }
}
