using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxStat.Resources
{
    public interface ILanguagePlugin
    {
        string LngIsoCode { get; set; }
        bool IsLive { get; set; }
        dynamic GetLabelValues();
        string Sanitize(string words);
        string Singularize(string word);
        List<string> GetSynonyms(string word);
        IEnumerable<string> GetExcludedTerms();
        IEnumerable<string> GetDoNotAmend();
    }
}