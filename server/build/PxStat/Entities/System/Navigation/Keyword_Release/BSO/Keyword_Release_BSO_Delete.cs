using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Delete a Keyword Release
    /// </summary>
    internal class Keyword_Release_BSO_Delete : BaseTemplate_Delete<Keyword_Release_DTO, Keyword_Release_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Keyword_Release_BSO_Delete(JSONRPC_API request) : base(request, new Keyword_Release_VLD_Delete())
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
            Keyword_Release_ADO keyWordAdo = new Keyword_Release_ADO();

            int deleteCount = keyWordAdo.Delete(Ado, null, DTO.KrlCode, false);

            if (deleteCount == 0)
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