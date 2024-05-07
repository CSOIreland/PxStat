using API;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Deletes a language
    /// </summary>
    internal class Language_BSO_Delete : BaseTemplate_Delete<Language_DTO_Delete, Language_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Language_BSO_Delete(JSONRPC_API request) : base(request, new Language_VLD_Delete())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            var adoLanguage = new Language_ADO(Ado);

            //Check if the language is used in a related entity. If so, we can't proceed with the delete request
            if (adoLanguage.IsInUse(DTO.LngIsoCode))
            {
                //The language is in use by at least one related entity, we cannot proceed with the delete request
                Log.Instance.Debug("The ISO Code '" + DTO.LngIsoCode + "' is currently is use and cannot be deleted.");
                Response.error = Label.Get("error.delete");
                return false;
            }

            //attempting to delete. The number of entities deleted are passed to the entitiesDeleted variable (this is 1 for a successful delete)
            int nDeleted = adoLanguage.Delete(DTO, SamAccountName);
            if (nDeleted == 0)
            {
                Log.Instance.Debug("Can't delete Language");
                Response.error = Label.Get("error.delete");
                return false;
            }
            return true;
        }

        public override bool PostExecute()
        {
            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}

