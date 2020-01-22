using API;
using PxStat.System.Settings;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.System.Navigation
{
    internal class Keyword_BSO_ReadSynonym : BaseTemplate_Read<Keyword_DTO_ReadSynonym, Keyword_VLD_ReadSynonym>
    {
        internal Keyword_BSO_ReadSynonym(JSONRPC_API request) : base(request, new Keyword_VLD_ReadSynonym())
        {
        }

        override protected bool HasPrivilege()
        {
            return IsPowerUser();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {

            Dictionary<string, List<Synonym>> synonymSets = Keyword_BSO_ResourceFactory.GetAllSynonymSets();
            dynamic results = new List<ExpandoObject>();
            results.Add(GetListForLanguage(Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE"), synonymSets));

            Language_ADO lAdo = new Language_ADO(Ado);
            var languages = lAdo.Read(new Language_DTO_Read());

            //foreach (var s in synonymSets)
            //{
            foreach (var l in languages.data)
            {
                //  if (synonymSets.ContainsKey(l.LngIsoCode))
                // {
                if (l.LngIsoCode != Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE"))
                    results.Add(GetListForLanguage(l.LngIsoCode, synonymSets));
                //}
                //else
                // {

                // }

            }
            //}






            Response.data = results;
            return true;
        }

        private dynamic GetListForLanguage(string lngIsoCode, Dictionary<string, List<Synonym>> synonyms)
        {
            dynamic result = new ExpandoObject();
            result.LngIsoCode = lngIsoCode;
            result.LngIsoName = new Language_BSO().Read(lngIsoCode).LngIsoName;
            List<Synonym> languageSynonyms = new List<Synonym>();
            if (synonyms.ContainsKey(lngIsoCode))
            {
                languageSynonyms = synonyms[lngIsoCode].Where(x => x.match == DTO.KrlValue).ToList();
                result.Synonym = languageSynonyms.Select(x => x.lemma).ToList();
            }
            else result.Synonym = languageSynonyms;
            return result;
        }



    }
}
