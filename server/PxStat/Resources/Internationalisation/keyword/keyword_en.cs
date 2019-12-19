using API;
using PxStat.System.Navigation;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PxStat.Resources
{



    /// <summary>
    /// This class creates the necessary keyword extraction and modification methods for the English language
    /// </summary>
    internal class Keyword_en : IKeywordExtractor
    {
        public List<Synonym> SynonymList { get; set; }

        readonly Keyword keyword;
        /// <summary>
        /// ISO language code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// cultureInfo
        /// </summary>
        public CultureInfo cultureInfo { get; set; }

        /// <summary>
        /// English language PluralizationService
        /// </summary>
        private PluralizationService pluralService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lngIsoCode"></param>
        public Keyword_en()
        {
            //this will be "en" for english
            this.LngIsoCode = Utility.GetCustomConfig("APP_INTERNATIONALISATION_ENGLISH");
            keyword = new Keyword(LngIsoCode);
            this.cultureInfo = new CultureInfo(LngIsoCode);
            //The PluralizationService contains a number of ready-made english language operations
            pluralService =
              PluralizationService.CreateService(cultureInfo);

            this.SynonymList = Keyword_BSO_ResourceFactory.GetSynonyms(LngIsoCode);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="words"></param>
        /// <returns></returns>
        public string Sanitize(string words)
        {
            return Regex.Replace(words, keyword.Get("excluded.regex"), " ");
        }



        /// <summary>
        /// Returns the singular form of a noun if the noun is already plural
        /// otherwise just return the word unchanged.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public string Singularize(string word)
        {
            if (pluralService.IsPlural(word))
            {
                return pluralService.Singularize(word);
            }
            else return word;
        }

        /// <summary>
        /// Returns the plural form of a noun if the noun is already singular
        /// otherwise just return the word unchanged
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public string GetPlural(string word)
        {
            if (pluralService.IsSingular(word))
            {
                return pluralService.Pluralize(word);
            }
            else return word;
        }

        internal Dictionary<string, string> GetSynonyms()
        {
            Dictionary<string, string> synonyms = new Dictionary<string, string>();

            return synonyms;
        }

    }
}
