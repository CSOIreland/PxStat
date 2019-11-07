
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using PxStat.System.Navigation;

namespace PxStat.Resources
{



    /// <summary>
    /// This class creates the necessary keyword extraction and modification methods for the English language
    /// </summary>
    internal class Keyword_en : IKeywordExtractor
    {
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
            this.LngIsoCode = "en";
            this.cultureInfo = new CultureInfo(LngIsoCode);
            //The PluralizationService contains a number of ready-made english language operations
            pluralService =
              PluralizationService.CreateService(cultureInfo);
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



    }
}
