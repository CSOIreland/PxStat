using API;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;

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
            ADO_readerOutput outputSummary = ado.ReadTimeline(DTO,SamAccountName );
            if (outputSummary.hasData)
            {
                List<dynamic> displayData = new List<dynamic>();
                foreach (dynamic item in outputSummary.data)
                {
                    dynamic obj = new ExpandoObject();
                    Dictionary<string, object> items = new Dictionary<string, object>();

                    IDictionary<string, object> dbitem = item;
                    foreach (var d in dbitem)
                    {
                        if (d.Key == "date")
                            items.Add(Label.Get("analytic.date", DTO.LngIsoCode), d.Value);
                        else if (d.Key == "total")
                            items.Add(Label.Get("analytic.total", DTO.LngIsoCode), d.Value);
                        else
                            items.Add(d.Key, d.Value);
                    }
                    displayData.Add(items);
                }

                Response.data = displayData;
                return true;
            }
            return false;
        }
    }
}
