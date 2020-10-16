using API;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;

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
            ADO_readerOutput outputSummary = ado.ReadBrowser(DTO);
            if (outputSummary.hasData)
            {
                Response.data = FormatData(outputSummary.data);
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
}
