using System.Collections.Generic;
using System.Globalization;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// This interface defines what a keyword extractor should do. Because of wide variation among languages as to how nouns vary
    /// (inflections can vary depending of plurals and noun cases), it is necessary to rewrite the extractor for each language 
    /// for which keywords are stored.
    /// </summary>
    internal interface IKeywordExtractor
    {
        /// <summary>
        /// ISO Language Code
        /// </summary>
        string LngIsoCode { get; set; }

        /// <summary>
        /// cultureInfo object
        /// </summary>
        CultureInfo cultureInfo { get; set; }




        string Singularize(string words);
    }
}
