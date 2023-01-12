using API;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using PxStat.Template;
using System.Collections.Generic;
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
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Analytic_ADO ado = new Analytic_ADO(Ado);
            ADO_readerOutput outputSummary = ado.ReadBrowser(DTO,SamAccountName );
            List<int> groupList = new List<int>();
            List<nltBrowser> browserList = new List<nltBrowser>();
            if (outputSummary.hasData)
            {
                
                foreach(var item in outputSummary.data[0])
                {
                    browserList.Add(new nltBrowser { NltBrowser=item.NltBrowser, GrpId=item.GrpId,Counter=item.Counter });
                }
                foreach (var item in outputSummary.data[1]) 
                {
                    groupList.Add(item.GrpId);
                }

                var restrictedGroupQuery = (from b in browserList
                        join g in groupList on b.GrpId equals g
                        select b).ToList();

               var result= from e in restrictedGroupQuery
                group e by e.NltBrowser into g
                select new { NltBrowser=g.Key,NltCount=g.Sum(x=>x.Counter)};
                

                Response.data = FormatData(result);
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
            int limit = Configuration_BSO.GetCustomConfig(ConfigType.server, "analytic.read-browser-item-limit");
            int counter = 1;
            int otherSum = 0;
            foreach (dynamic item in readData)
            {
                if (counter < limit)
                {
                    itemDict.Add(item.NltBrowser == "-" ? Label.Get("analytic.unknown", DTO.LngIsoCode) : item.NltBrowser, item.NltCount);
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
