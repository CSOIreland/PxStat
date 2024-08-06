using API;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.Security
{
    internal class Analytic_BSO_ReadFormat : BaseTemplate_Read<Analytic_DTO_Read, Analytic_VLD_ReadFormat>
    {
        internal Analytic_BSO_ReadFormat(JSONRPC_API request) : base(request, new Analytic_VLD_ReadFormat())
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

            MemCachedD_Value cache = ApiServicesHelper.CacheD.Get_BSO("PxStat.Security", "Analytic", "ReadFormat", DTO);
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }
            Analytic_ADO ado = new Analytic_ADO(Ado);
            List<dynamic> outputSummary = ado.ReadFormat(DTO);
            if (outputSummary != null)
            {
                Response.data = FormatData(outputSummary);
                ApiServicesHelper.CacheD.Store_BSO("PxStat.Security", "Analytic", "ReadFormat", DTO, Response.data, default(DateTime));
                return true;
            }
            return false;
        }

        private dynamic FormatData(dynamic readData)
        {
            dynamic output = new ExpandoObject();
            var itemDict = output as IDictionary<string, object>;

            foreach (dynamic item in readData)
            {

                itemDict.Add(item.FrmTypeVersion.ToString(), item.NltCount);


            }


            return output;
        }

    }

     class nltFormat
    {
        internal string FrmTypeVersion { get; set; }
        internal int GrpId { get; set; }
        internal int NltCount { get; set; }
    }
}
