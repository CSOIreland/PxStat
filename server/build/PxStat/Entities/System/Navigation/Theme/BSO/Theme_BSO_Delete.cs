using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Delete a subject
    /// </summary>
    class Theme_BSO_Delete : BaseTemplate_Delete<Theme_DTO_Delete, Theme_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Theme_BSO_Delete(JSONRPC_API request) : base(request, new Theme_VLD_Delete())
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
            var adoTheme = new Theme_ADO(Ado);

            //attempting to delete. The number of entities deleted are passed to the entitiesDeleted variable (this is 1 for a successful delete)
            int nDeleted = adoTheme.Delete(DTO, SamAccountName);
            Log.Instance.Debug("Delete operation finished in IADO");

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
