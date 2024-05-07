using API;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.Security
{
    /// <summary>
    /// Read referrer data from Analytics
    /// </summary>
    internal class Analytic_BSO_ReadReferrer : BaseTemplate_Read<Analytic_DTO_Read, Analytic_VLD_ReadReferrer>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Analytic_BSO_ReadReferrer(JSONRPC_API request) : base(request, new Analytic_VLD_ReadReferrer())
        { }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            if (IsPowerUser())
                return true;

            if (IsModerator() && !String.IsNullOrEmpty(DTO.MtrCode))
                return true;

            return false;
        }


        protected override bool Execute()
        {
            MemCachedD_Value cache = ApiServicesHelper.CacheD.Get_BSO("PxStat.Security", "Analytic", "ReadReferrer", DTO);
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }
            Analytic_ADO ado = new Analytic_ADO(Ado);
            List<dynamic> outputSummary = ado.ReadReferer(DTO);
            if (outputSummary != null)
            {
                Response.data = FormatData(outputSummary);
                ApiServicesHelper.CacheD.Store_BSO("PxStat.Security", "Analytic", "ReadReferrer", DTO, Response.data, default(DateTime));
                return true;
            }
            return false;
        }

        /// <summary>
        /// Format data for output
        /// </summary>
        /// <param name="readData"></param>
        /// <returns></returns>
        private dynamic FormatData(dynamic readData)
        {
            dynamic output = new ExpandoObject();
            var itemDict = output as IDictionary<string, object>;
            int limit = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "analytic.read-referrer-item-limit");
            int counter = 1;
            int otherSum = 0;

            string unknown = Label.Get("analytic.unknown");
            string others = Label.Get("analytic.others");
            foreach (dynamic item in readData)
            {

                if (counter < limit)
                {
                    itemDict.Add(item.NltReferer.ToString() == "-" ? unknown : item.NltReferer.ToString(), item.NltCount);
                }
                else otherSum = otherSum + item.NltCount;
                counter++;
            }

            if (otherSum > 0) itemDict.Add(others, otherSum);

            return output;
        }
    }
    class nltReferer
    {
        internal string NltReferer { get; set; }      
        internal int GrpId { get; set; }
        internal int nltCount { get; set; }
    }
}
