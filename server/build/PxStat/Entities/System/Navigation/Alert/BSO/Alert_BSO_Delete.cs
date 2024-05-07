using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Delete an Alert
    /// </summary>
    internal class Alert_BSO_Delete : BaseTemplate_Delete<Alert_DTO, Alert_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Alert_BSO_Delete(JSONRPC_API request) : base(request, new Alert_VLD_Delete())
        {
        }

        /// <summary>
        /// Test Privilege
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

            int deleted = adoAlert.Delete(Ado, DTO, SamAccountName);

            if (deleted == 0)
            {
                Log.Instance.Debug("Error deleting Alert");
                Response.error = Label.Get("error.delete");
                return false;
            }

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}