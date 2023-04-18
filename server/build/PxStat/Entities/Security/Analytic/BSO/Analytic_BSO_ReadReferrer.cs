using API;
using PxStat.Template;
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
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Analytic_ADO ado = new Analytic_ADO(Ado);
            ADO_readerOutput outputSummary = ado.ReadReferrer(DTO, SamAccountName );
            List<nltReferer> refList = new List<nltReferer>();
            List<int> groupList = new List<int>();
            if (outputSummary.hasData)
            {
                foreach (var item in outputSummary.data[0])
                {
                    refList.Add(new nltReferer { NltReferer = item.NltReferer, GrpId = item.GrpId, nltCount = item.nltCount });
                }
                foreach (var item in outputSummary.data[1])
                {
                    groupList.Add(item.GrpId);
                }

                var restrictedGroupQuery = (from b in refList
                                            join g in groupList on b.GrpId equals g
                                            select b).ToList();

                var result = from e in restrictedGroupQuery
                             group e by e.NltReferer into g
                             select new { NltReferer = g.Key, NltCount = g.Sum(x => x.nltCount) };


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
            int limit = Configuration_BSO.GetCustomConfig(ConfigType.server, "analytic.read-referrer-item-limit");
            int counter = 1;
            int otherSum = 0;

            foreach (dynamic item in readData)
            {

                if (counter < limit)
                {
                    itemDict.Add(item.NltReferer == "-" ? Label.Get("analytic.unknown", DTO.LngIsoCode) : item.NltReferer, item.NltCount);
                }
                else otherSum = otherSum + item.NltCount;
                counter++;
            }

            if (otherSum > 0) itemDict.Add(Label.Get("analytic.unknown", DTO.LngIsoCode), otherSum);

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
