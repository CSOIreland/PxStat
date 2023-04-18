using API;
using PxStat.Resources;
using PxStat.Security;
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

            Dictionary<string, List<Synonym>> synonymSets = new Dictionary<string, List<Synonym>>();
            dynamic results = new List<ExpandoObject>();

            foreach(ILanguagePlugin  language in Resources.LanguageManager.Languages.Values.Where(x=>x.IsLive))
            {
                dynamic result = new ExpandoObject();
                result.LngIsoCode = language.LngIsoCode ;
                result.LngIsoName = new Language_BSO().Read(language.LngIsoCode).LngIsoName;
                result.Synonym = language.GetSynonyms(DTO.KrlValue);
                results.Add(result);
            }
            Response.data = results;
            return true;
        }


    }
}
