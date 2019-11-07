using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Reads one or more Alerts, including alerts set in the future
    /// </summary>
    internal class Alert_BSO_Read : BaseTemplate_Read<Alert_DTO, Alert_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Alert_BSO_Read(JSONRPC_API request) : base(request, new Alert_VLD_Read())
        { }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Alert_ADO adoAlert = new Alert_ADO();

            ADO_readerOutput result;

            result = adoAlert.Read(Ado, DTO, false);
            if (!result.hasData)
            {
                return false;
            }

            Response.data = result.data;

            return true;
        }


    }
}