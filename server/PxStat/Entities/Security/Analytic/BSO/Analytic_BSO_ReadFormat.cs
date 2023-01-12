using API;
using PxStat.Template;
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
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Analytic_ADO ado = new Analytic_ADO(Ado);
            ADO_readerOutput outputSummary = ado.ReadFormat(DTO,SamAccountName );
            List<nltFormat> formatList = new List<nltFormat>();
            List<int> groupList = new List<int>();

            if (outputSummary.hasData)
            {
                foreach (var item in outputSummary.data[0])
                {
                    formatList.Add(new nltFormat {FrmTypeVersion=item.FrmTypeVersion, GrpId=item.GrpId, NltCount=item.NltCount });
                }
                foreach (var item in outputSummary.data[1])
                {
                    groupList.Add(item.GrpId);
                }

                var restrictedGroupQuery = (from b in formatList
                                            join g in groupList on b.GrpId equals g
                                            select b).ToList();

                var result = from e in restrictedGroupQuery
                             group e by e.FrmTypeVersion into g
                             select new { FrmTypeVersion = g.Key, NltCount = g.Sum(x => x.NltCount) };


                Response.data = FormatData(result);

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

                itemDict.Add(item.FrmTypeVersion, item.NltCount);


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
