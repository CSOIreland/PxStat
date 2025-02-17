using API;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.Security
{
    /// <summary>
    /// Reads language data from Analytics
    /// </summary>
    internal class Analytic_BSO_ReadLanguage : BaseTemplate_Read<Analytic_DTO_Read, Analytic_VLD_ReadLanguage>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Analytic_BSO_ReadLanguage(JSONRPC_API request) : base(request, new Analytic_VLD_ReadLanguage())
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

            MemCachedD_Value cache = ApiServicesHelper.CacheD.Get_BSO("PxStat.Security", "Analytic", "ReadLanguage", DTO);
     
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }
            Analytic_ADO ado = new Analytic_ADO(Ado);
            List<dynamic> outputSummary = ado.ReadLanguage(DTO);

            if (outputSummary != null)
            {
               // outputSummary.ToList().OrderBy(x=>x.lngCount).Reverse();

                Response.data = FormatData(outputSummary);
                ApiServicesHelper.CacheD.Store_BSO("PxStat.Security", "Analytic", "ReadLanguage", DTO, Response.data, default(DateTime));
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


            foreach (dynamic item in readData)
            {
                itemDict.Add(item.mtrLngName.ToString(), item.lngCount);
            }
            return output;
        }

    }
    class nltLanguage
    {
        internal string LngIsoName { get; set; }
        internal int GrpId { get; set; }
        internal int lngCount { get; set; }
    }
}
