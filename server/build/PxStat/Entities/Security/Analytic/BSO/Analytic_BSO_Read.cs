using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Reads analytic entries
    /// </summary>
    internal class Analytic_BSO_Read : BaseTemplate_Read<Analytic_DTO_Read, Analytic_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Analytic_BSO_Read(JSONRPC_API request) : base(request, new Analytic_VLD_Read())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() ;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Analytic_ADO ado = new Analytic_ADO(Ado);

            ADO_readerOutput outputDetails = ado.Read(DTO);

            Response.data = outputDetails.data;

            if (outputDetails.hasData)
            {
                return true;
            }

            return false;
        }
    }
}