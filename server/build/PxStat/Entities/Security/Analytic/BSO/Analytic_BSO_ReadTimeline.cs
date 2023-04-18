using API;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace PxStat.Security
{
    /// <summary>
    /// Reads summary information from Analytics
    /// </summary>
    internal class Analytic_BSO_ReadTimeline : BaseTemplate_Read<Analytic_DTO_Read, Analytic_VLD_ReadTimeline>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Analytic_BSO_ReadTimeline(JSONRPC_API request) : base(request, new Analytic_VLD_ReadTimeline())
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
            ADO_readerOutput outputSummary = ado.ReadTimeline(DTO, SamAccountName);
            if (outputSummary.hasData)
            {
                List<dynamic> displayData = new List<dynamic>();
                List<nltTimeline> timeLineList = new List<nltTimeline>();
                List<int> groupList = new List<int>();

                foreach (var item in outputSummary.data[0])
                {
                    timeLineList.Add(new nltTimeline
                    {
                        date = item.date,
                        GrpId = item.GrpId.Equals(DBNull.Value) ? 0 : item.GrpId,
                        NltBot = item.NltBot.Equals(DBNull.Value) ? 0 : item.NltBot,
                        NltM2m = item.NltM2m.Equals(DBNull.Value) ? 0 : item.NltM2m,
                        NltWidget = item.NltWidget.Equals(DBNull.Value) ? 0 : item.NltWidget,
                        NltUser = item.NltUser.Equals(DBNull.Value) ? 0 : item.NltUser,
                        total = item.total.Equals(DBNull.Value) ? 0 : item.total
                    });
                }
                foreach (var item in outputSummary.data[1])
                {
                    groupList.Add(item.GrpId.Equals(DBNull.Value) ? 0 : item.GrpId);
                }

                var restrictedGroupQuery = (from b in timeLineList
                                            join g in groupList on b.GrpId equals g
                                            select b).ToList();

                var result = from e in restrictedGroupQuery
                             group e by e.date into g
                             select new
                             {
                                 date = g.Key,
                                 NltBot = g.Sum(x => x.NltBot.Equals(DBNull.Value) ? 0 : x.NltBot),
                                 NltM2m = g.Sum(x => x.NltM2m.Equals(DBNull.Value) ? 0 : x.NltM2m),
                                 NltWidget = g.Sum(x => x.NltWidget.Equals(DBNull.Value) ? 0 : x.NltWidget),
                                 NltUser = g.Sum(x => x.NltUser.Equals(DBNull.Value) ? 0 : x.NltUser),
                                 total = g.Sum(x => x.total.Equals(DBNull.Value) ? 0 : x.total)
                             };



                Response.data = result;
                return true;
            }
            return false;
        }
    }
    class nltTimeline
    {
        internal DateTime date { get; set; }
        internal int GrpId { get; set; }
        internal int NltBot { get; set; }
        internal int NltM2m { get; set; }
        internal int NltWidget { get; set; }
        internal int NltUser { get; set; }
        internal int total { get; set; }
    }
}
