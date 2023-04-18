using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{/// <summary>
/// Reads Alerts only if alert date is in the pase
/// </summary>
    internal class Alert_BSO_ReadLive : BaseTemplate_Read<Alert_DTO, Alert_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Alert_BSO_ReadLive(JSONRPC_API request) : base(request, new Alert_VLD_Read())
        { }

        /// <summary>
        /// Test authentication
        /// </summary>
        /// <returns></returns>
        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Alert_ADO adoAlert = new Alert_ADO();

            ADO_readerOutput result;

            result = adoAlert.Read(Ado, DTO, true);
            if (!result.hasData)
            {
                return false;
            }

            Response.data = result.data;

            return true;
        }


    }
}