using API;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace PxStat.System.Navigation
{

    internal class Keyword_BSO_Extract
    {
        readonly ILanguagePlugin language;
        internal Keyword_BSO_Extract(string lngIsoCode)
        {
            language=LanguageManager.GetLanguage(lngIsoCode);
        }

        internal bool IsExcluded(string word)
        {
            return language.GetExcludedTerms().Contains(word);          
        }

        internal bool IsDoNotAmend(string word)
        {
            return language.GetDoNotAmend().Contains(word);
        }

        internal List<string> ExtractSplit(string readString, bool sanitize = true)
        {
            if (sanitize)
                readString = language.Sanitize(readString);

            //We need to treat spaces and non-breaking spaces the same, so we replace any space with a standard space
            Regex rx = new Regex("[\\s]");
            readString = rx.Replace(readString, " ");
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

        internal List<string> ExtractSplitSingular(string readString)
        {

            readString = language.Sanitize(readString);

            //We need to treat spaces and non-breaking spaces the same, so we replace any space with a standard space
            Regex rx = new Regex("[\\s]");
            readString = rx.Replace(readString, " ");

            // convert the sentance to a list of words
            List<string> wordListInput = (readString.Split(' ')).Where(x=>x.Length>0).ToList();

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

        public string Singularize(string word)
        {
            return language.Singularize(word);
        }
    }

   

    public class Synonym :IEquatable <Synonym>  
    {
        public string match { get; set; }
        public string lemma { get; set; }


        public Synonym() { }

        public  bool Equals(Synonym other)
        {
            return match == other.match && lemma == other.lemma;    
        }
        public override int GetHashCode()
        {
            return (match != null ? match.GetHashCode() : 0) ^ lemma.GetHashCode();
        }
    }
}
