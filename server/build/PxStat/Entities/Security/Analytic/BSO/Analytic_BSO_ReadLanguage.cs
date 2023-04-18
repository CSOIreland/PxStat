using API;
using PxStat.Template;
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
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Analytic_ADO ado = new Analytic_ADO(Ado);
            ADO_readerOutput outputSummary = ado.ReadLanguage(DTO,SamAccountName );
            List<nltLanguage> languageList = new List<nltLanguage>();
            List<int> groupList = new List<int>();
            if (outputSummary.hasData)
            {
                foreach (var item in outputSummary.data[0])
                {
                    languageList.Add(new nltLanguage { LngIsoName = item.LngIsoName, GrpId = item.GrpId, lngCount = item.lngCount });
                }
                foreach (var item in outputSummary.data[1])
                {
                    groupList.Add(item.GrpId);
                }

                var restrictedGroupQuery = (from b in languageList
                                            join g in groupList on b.GrpId equals g
                                            select b).ToList();

                var result = from e in restrictedGroupQuery
                             group e by e.LngIsoName into g
                             select new { LngIsoName = g.Key, lngCount = g.Sum(x => x.lngCount) };


                Response.data = FormatData(result);
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
                itemDict.Add(item.LngIsoName, item.lngCount);
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
