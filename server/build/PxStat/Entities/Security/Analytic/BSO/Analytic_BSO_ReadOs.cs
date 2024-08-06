using API;
using DocumentFormat.OpenXml.Office2016.Excel;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.Security
{
    /// <summary>
    /// Reads Operating system from Analytics
    /// </summary>
    internal class Analytic_BSO_ReadOs : BaseTemplate_Read<Analytic_DTO_Read, Analytic_VLD_ReadOs>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Analytic_BSO_ReadOs(JSONRPC_API request) : base(request, new Analytic_VLD_ReadOs())
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


        protected override bool Execute()
        {
            if (IsModerator() && String.IsNullOrEmpty(DTO.MtrCode))
            {
                Response.error = Label.Get("error.privilege");
                return false;
            }

            MemCachedD_Value cache = ApiServicesHelper.CacheD.Get_BSO("PxStat.Security", "Analytic", "ReadOs", DTO);
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }
            Analytic_ADO ado = new Analytic_ADO(Ado);
            List<dynamic> outputSummary = ado.ReadOs(DTO);
            if (outputSummary != null)
            {
                Response.data = FormatData(outputSummary);
                ApiServicesHelper.CacheD.Store_BSO("PxStat.Security", "Analytic", "ReadOs", DTO, Response.data, default(DateTime));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Format returned data for output
        /// </summary>
        /// <param name="readData"></param>
        /// <returns></returns>
        private dynamic FormatData(dynamic readData)
        {
            dynamic output = new ExpandoObject();
            var itemDict = output as IDictionary<string, object>;
            int limit = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "analytic.read-os-item-limit");
            int counter = 1;
            int otherSum = 0;

            foreach (dynamic item in readData)
            {

                if (counter < limit)
                {
                    itemDict.Add(item.NltOs.ToString() == "-" ? Label.Get("analytic.unknown", DTO.LngIsoCode) : item.NltOs.ToString(), item.NltCount);
                }
                else otherSum = otherSum + item.NltCount;
                counter++;
            }

            if (otherSum > 0) itemDict.Add(Label.Get("analytic.others", DTO.LngIsoCode), otherSum);
            return output;
        }
    }
    class nltOs
    {
        public string NltOs { get; set; }
        public int GrpId { get; set; }
        public int nltCount { get; set; }
    }
}
