using API;
using PxStat.System.Navigation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxStat.Resources
{
    /// <summary>
    /// Implementation of IKeywordExtractor for Gaeilge
    /// </summary>
    public class Keyword_ga : IKeywordExtractor
    {
        readonly Keyword keyword;
        readonly Dictionary<string, string> nounDictionary;
        public List<Synonym> SynonymList { get; set; }

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
        public Keyword_ga()
        {

                LngIsoCode = Utility.GetCustomConfig("APP_INTERNATIONALISATION_IRISH");
            keyword = new Keyword(LngIsoCode);

            nounDictionary = Keyword_BSO_ResourceFactory.GetMorphology(LngIsoCode);

            this.SynonymList = Keyword_BSO_ResourceFactory.GetSynonyms(LngIsoCode);


        }

        public string Sanitize(string words)
        {
            string s = keyword.Get("excluded.regex");
            return Regex.Replace(words, keyword.Get("excluded.regex"), " ");
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
            throw new NotImplementedException();
        }



        //
        /// <summary>
        /// If 'h' is the second letter of the word, remove it (in certain cases!)
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string RemoveAspiration(string word)
        {
            string outWord = word;
            List<string> aspirationList = keyword.GetStringList("inflection.aspiration");
            foreach (string s in aspirationList)
            {
                if (word.StartsWith(s) && s.Length >= 2 && word.Length >= 3)
                {
                    outWord = word.Substring(0, 1) + word.Substring(2, word.Length - 2);
                }
            }

            return outWord;
        }

        /// <summary>
        /// Remove the eclipsis (e.g. bh, dt, etc) from the start of a word
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private string RemoveEclipsis(string word)
        {
            string outWord = word;
            List<string> eclipsisList = keyword.GetStringList("inflection.eclipsis");
            foreach (string s in eclipsisList)
            {
                if (word.StartsWith(s) && s.Length == 2)
                {
                    outWord = word.Substring(s.Length - (s.Length - 1), word.Length - (s.Length - (s.Length - 1)));
                }
                else if (word.StartsWith(s) && s.Length == 3)
                    outWord = word.Substring(s.Length - (s.Length - 2), word.Length - (s.Length - (s.Length - 2)));
            }

            return outWord;
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

            word = RemoveEclipsis(word.ToLower());
            word = RemoveAspiration(word);

            return nounDictionary.ContainsKey(word) ? nounDictionary[word] : word;
        }
    }
}
