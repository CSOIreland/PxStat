using API;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Deletes a Copyright
    /// </summary>
    internal class Copyright_BSO_Delete : BaseTemplate_Delete<Copyright_DTO_Delete, Copyright_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Copyright_BSO_Delete(JSONRPC_API request) : base(request, new Copyright_VLD_Delete())
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
            Copyright_ADO adoCopyright = new Copyright_ADO();
            if (adoCopyright.IsInUse(Ado, DTO.CprCode))
            {
                Log.Instance.Debug("Delete request for Copyright Code: " + DTO.CprCode + " refused because it is in use by at least one related entity");
                Response.error = Label.Get("error.delete");
                return false;
            }

            //attempting to delete. The number of entities deleted are passed to the entitiesDeleted variable (this is 1 for a successful delete)
            int nDeleted = adoCopyright.Delete(Ado, DTO, SamAccountName);

            if (nDeleted == 0)
            {
                Log.Instance.Debug("No record found for delete request");
                Response.error = Label.Get("error.delete");
                return false;
            }

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}

