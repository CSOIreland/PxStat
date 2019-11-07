using API;
using PxStat.Template;


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
        internal Analytic_BSO_ReadTimeline(JSONRPC_API request) : base(request, new Analytic_VLD_ReadBrowser())
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
            ADO_readerOutput outputSummary = ado.ReadTimeline(DTO);
            if (outputSummary.hasData)
            {
                Response.data = outputSummary.data;
                return true;
            }
            return false;
        }
    }
}