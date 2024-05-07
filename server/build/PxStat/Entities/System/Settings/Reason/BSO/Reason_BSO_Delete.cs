using API;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Delete a Reason
    /// </summary>
    internal class Reason_BSO_Delete : BaseTemplate_Delete<Reason_DTO_Delete, Reason_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Reason_BSO_Delete(JSONRPC_API request) : base(request, new Reason_VLD_Delete())
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
            var adoReason = new Reason_ADO();

            //attempting to delete. The number of entities deleted are passed to the entitiesDeleted variable (this is 1 for a successful delete)
            int nDeleted = adoReason.Delete(Ado, DTO, SamAccountName);
            if (nDeleted == 0)
            {
                Log.Instance.Debug("Can't delete Reason");

                Response.error = Label.Get("error.delete");
                return false;
            }

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}