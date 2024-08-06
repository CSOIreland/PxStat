using API;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.Security
{/// <summary>
/// Reads the main environment language of the API user
/// </summary>
    internal class Analytic_BSO_ReadEnvironmentLanguage : BaseTemplate_Read<Analytic_DTO_Read, Analytic_VLD_ReadEnvironmentLanguage>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Analytic_BSO_ReadEnvironmentLanguage(JSONRPC_API request) : base(request, new Analytic_VLD_ReadEnvironmentLanguage())
        { }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            if (IsPowerUser() || IsModerator())
                return true;


            return false;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            if (IsModerator() && String.IsNullOrEmpty(DTO.MtrCode))
            {
                Response.error = Label.Get("error.privilege");
                return false;
            }

            MemCachedD_Value cache = ApiServicesHelper.CacheD.Get_BSO("PxStat.Security", "Analytic", "ReadEnvironmentLanguage", DTO);
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }
            Analytic_ADO ado = new Analytic_ADO(Ado);
            List<dynamic> outputSummary = ado.ReadEnvironmentLanguage(DTO);
            if (outputSummary != null)
            {
                Response.data = FormatData(outputSummary);
                ApiServicesHelper.CacheD.Store_BSO("PxStat.Security", "Analytic", "ReadEnvironmentLanguage", DTO, Response.data, default(DateTime));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Formats the returned data
        /// </summary>
        /// <param name="readData"></param>
        /// <returns></returns>
        private dynamic FormatData(dynamic readData)
        {
            dynamic output = new ExpandoObject();
            var itemDict = output as IDictionary<string, object>;


            foreach (dynamic item in readData)
            {
                itemDict.Add(item.nltLngIsoCode.ToString(), item.lngCount);
            }
            return output;
        }
    }
}
