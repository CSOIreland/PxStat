using API;
using DocumentFormat.OpenXml.Spreadsheet;
using PxStat.System.Settings;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Security
{
    /// <summary>
    /// Reads analytic entries
    /// </summary>
    internal class Analytic_BSO_Read : BaseTemplate_Read<Analytic_DTO_Read, Analytic_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Analytic_BSO_Read(JSONRPC_API request) : base(request, new Analytic_VLD_Read())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        protected override bool Execute()
        {
            if (IsModerator() && String.IsNullOrEmpty(DTO.MtrCode))
            {
                Response.error = Label.Get("error.privilege", DTO.LngIsoCode);
                return false;
            }
                if (new Language_BSO().Read(DTO.LngIsoCode.ToString(), Ado) == null)
                DTO.LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

            MemCachedD_Value cache = ApiServicesHelper.CacheD.Get_BSO("PxStat.Security", "Analytic", "Read", DTO);
            
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }



            Analytic_ADO ado = new Analytic_ADO(Ado);
            List<dynamic> outputSummary = ado.Read(DTO);

            if (outputSummary != null)
                ApiServicesHelper.CacheD.Store_BSO("PxStat.Security", "Analytic", "Read", DTO, outputSummary, default(DateTime));


            Response.data = outputSummary;
            return true;
        }

    }
}