using API;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

namespace PxStat.Security
{
    /// <summary>
    /// Reads Browser Analytic data
    /// </summary>
    internal class Analytic_BSO_ReadBrowser : BaseTemplate_Read<Analytic_DTO_Read, Analytic_VLD_ReadBrowser>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Analytic_BSO_ReadBrowser(JSONRPC_API request) : base(request, new Analytic_VLD_ReadBrowser())
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

            Stopwatch sw= Stopwatch.StartNew();
            MemCachedD_Value cache = ApiServicesHelper.CacheD.Get_BSO("PxStat.Security", "Analytic", "ReadBrowser", DTO);
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }
            Analytic_ADO ado = new Analytic_ADO(Ado);
            List<dynamic> outputSummary = ado.ReadBrowser(DTO);
            if (outputSummary != null)
            {
                Response.data =  FormatData(outputSummary);
                ApiServicesHelper.CacheD.Store_BSO("PxStat.Security", "Analytic", "ReadBrowser", DTO, Response.data, default(DateTime));
                sw.Stop();
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
            int limit = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "analytic.read-browser-item-limit");
            int counter = 1;
            int otherSum = 0;
            foreach (dynamic item in readData)
            {
                if (counter < limit)
                {
                    itemDict.Add(item.NltBrowser.ToString() == "-" ? Label.Get("analytic.unknown", DTO.LngIsoCode) : item.NltBrowser.ToString(), item.NltCount);
                }
                else otherSum = otherSum + item.NltCount;
                counter++;
            }

            if (otherSum > 0) itemDict.Add(Label.Get("analytic.others", DTO.LngIsoCode), otherSum);
            return output;
        }
    }

     class nltBrowser
    {
        internal string NltBrowser { get; set; }
        internal int GrpId { get; set; }
        internal int Counter { get; set; }
    }
}
