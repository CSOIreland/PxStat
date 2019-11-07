using API;
using PxStat.Resources;
using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// This class returns an instance of IKeywordExtractor depending on the Language Iso Code (LngIsoCode)
    /// supplied to it.
    /// If the LngIsoCode is not recognised, a default implementation will be returned.
    /// In order to extend this application to deal with keywords in another language, you will need to do the following:
    ///  - Write an implementation of IKeywordExtractor for the required language
    ///  - Add to the case statement in GetExtractor to call the implementation when the corresponding LngIsoCode is supplied.
    /// </summary>
    internal class Keyword_BSO_Extract : PluralizationService
    {
        internal IKeywordExtractor extractor { get; }
        string LngIsoCode;
        Keyword keyword;

        internal Keyword_BSO_Extract(string lngIsoCode)
        {
            LngIsoCode = lngIsoCode;
            keyword = new PxStat.Keyword(lngIsoCode);
            switch (lngIsoCode)
            {
                case "en":
                    this.extractor = new Keyword_en();
                    break;
                case "ga":
                    this.extractor = new Keyword_ga();
                    break;
                default:
                    //We will default to an extractor based on the default language of the system
                    string extractorClassName = "PxStat.Resources.Keyword_" + Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");

                    Type extractorType = Type.GetType(extractorClassName);
                    this.extractor = (IKeywordExtractor)Assembly.GetAssembly(extractorType).CreateInstance(extractorType.FullName);
                    break;
            }
        }

        /// <summary>
        /// Indicates if a word is one of a specific list of number that are not to be processed. These words will not be added to the output.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        internal bool IsExcluded(string word)
        {


            List<string> wordList = keyword.GetStringList("excluded.article");

            wordList.AddRange(keyword.GetStringList("excluded.preposition"));


            if (wordList.Contains(word.ToLower()))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if a word is not to be singularized or pluralized. These words will be unchanged in the output.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        internal bool IsDoNotAmend(string word)
        {
            //time words, some statistical terms
            List<string> wordList = keyword.Get("static.month").Split(',').ToList();
            wordList.AddRange(keyword.Get("static.statistical").Split(',').ToList());
            if (wordList.Contains(word.ToLower()))
                return true;
            else
                return false;
        }

        /// <summary>
        /// This function returns a list of words derived from splitting an input phrase. The words are not singularized.
        /// The words are sanitized and are also subject to exclusions
        /// </summary>
        /// <param name="readString"></param>
        /// <returns></returns>
        internal List<string> ExtractSplit(string readString)
        {

            readString = Sanitize(readString);

            // convert the sentence to a list of words
            List<string> wordListInput = readString.Split(' ').ToList<string>();

            //create an output list
            List<string> wordList = new List<string>();

            foreach (string word in wordListInput)
            {
                //trim white spaces
                string trimWord = Regex.Replace(word, @"^\s+", "");
                trimWord = Regex.Replace(trimWord, @"\s+$", "");

                if (trimWord.Length > 0)
                {

                    //if the word is not in our list of excluded words
                    if (!IsExcluded(trimWord))
                    {

                        if (!wordList.Contains(trimWord))
                            wordList.Add(trimWord);
                    }

                }
            }

            return wordList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        internal string Sanitize(string words)
        {
            //replace non alpha characters with spaces
            //Note this Regex is specific to the english language. Use a language appropriate string for any other language
            return Regex.Replace(words, keyword.Get("excluded.regex"), " ");
        }

        /// <summary>
        /// This function returns a list of nominative singular words from a string of space separated words (e.g. a sentance).
        /// For a non-inflected language like English, it suffices to return the word if singular and the singular version of the word
        /// if plural.
        /// </summary>
        /// <param name="readString"></param>
        /// <returns></returns>
        internal List<string> ExtractSplitSingular(string readString)
        {
            readString = Sanitize(readString);

            // convert the sentance to a list of words
            List<string> wordListInput = readString.Split(' ').ToList<string>();

            //create an output list
            List<string> wordList = new List<string>();

            foreach (string word in wordListInput)
            {
                //trim white spaces
                string trimWord = Regex.Replace(word, @"^\s+", "");
                trimWord = Regex.Replace(trimWord, @"\s+$", "");

                if (trimWord.Length > 0)
                {

                    //if the word is not in our list of excluded words
                    if (!IsExcluded(trimWord))
                    {

                        //if the word may be changed from singular to plural
                        if (!IsDoNotAmend(trimWord))
                        {
                            //get the singular version if it's singular
                            string wordRead = Singularize(trimWord);
                            if (!wordList.Contains(wordRead))
                                wordList.Add(wordRead);
                        }
                        else
                        {
                            //add the word to the output list, but not if it's in the list already
                            if (!wordList.Contains(trimWord))
                                wordList.Add(trimWord);
                        }
                    }

                }
            }
            return wordList;
        }

        public override bool IsPlural(string word)
        {
            throw new NotImplementedException();
        }

        public override bool IsSingular(string word)
        {
            throw new NotImplementedException();
        }

        public override string Pluralize(string word)
        {
            throw new NotImplementedException();
        }

        public override string Singularize(string word)
        {
            //First we check in case this is in the list of irregular endings...
            Keyword kw = new Keyword(LngIsoCode);
            string lookup = "irregular.plural." + word;
            string result = kw.Get(lookup);
            if (!(result).Equals(lookup))
                return result;
            else
                return extractor.Singularize(word);
        }
    }
}
