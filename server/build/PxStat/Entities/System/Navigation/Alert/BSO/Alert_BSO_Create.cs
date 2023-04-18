using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Create an alert
    /// </summary>
    internal class Alert_BSO_Create : BaseTemplate_Create<Alert_DTO, Alert_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Alert_BSO_Create(JSONRPC_API request) : base(request, new Alert_VLD_Create())
        {
        }

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

            int created = adoAlert.Create(Ado, DTO, SamAccountName);

            if (created == 0)
            {
                Log.Instance.Debug("Error creating Alert");
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}