using API;
using PxStat.Template;
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
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Analytic_ADO ado = new Analytic_ADO(Ado);
            ADO_readerOutput outputSummary = ado.ReadOs(DTO, SamAccountName);
            List<nltOs> osList = new List<nltOs>();
            List<int> groupList = new List<int>();
            if (outputSummary.hasData)
            {
                foreach (var item in outputSummary.data[0])
                {
                    osList.Add(new nltOs { NltOs = item.NltOs, GrpId = item.GrpId, nltCount = item.nltCount });
                }
                foreach (var item in outputSummary.data[1])
                {
                    groupList.Add(item.GrpId);
                }

                var restrictedGroupQuery = (from b in osList
                                            join g in groupList on b.GrpId equals g
                                            select b).ToList();

                var result = from e in restrictedGroupQuery
                             group e by e.NltOs into g
                             select new { NltOs = g.Key, NltCount = g.Sum(x => x.nltCount) };


                Response.data = FormatData(result);
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
            int limit = Configuration_BSO.GetCustomConfig(ConfigType.server, "analytic.read-os-item-limit");
            int counter = 1;
            int otherSum = 0;

            foreach (dynamic item in readData)
            {

                if (counter < limit)
                {
                    itemDict.Add(item.NltOs == "-" ? Label.Get("analytic.unknown", DTO.LngIsoCode) : item.NltOs, item.NltCount);
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
