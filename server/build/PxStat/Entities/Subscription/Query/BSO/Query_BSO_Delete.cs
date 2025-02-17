using API;
using PxStat.Template;
using CSO.Firebase;

namespace PxStat.Subscription
{
    internal class Query_BSO_Delete : BaseTemplate_Delete<Query_DTO_Delete, Query_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Query_BSO_Delete(JSONRPC_API request) : base(request, new Query_VLD_Delete())
        {
        }

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
            if (SamAccountName == null)
            {
              
                if (!AppServicesHelper.Firebase.Authenticate(DTO.Uid, DTO.AccessToken, ApiServicesHelper.ApiConfiguration.Settings, Log.Instance))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
            }

            Query_ADO ado = new Query_ADO(Ado);
            var response = ado.Delete(DTO.UserQueryId, DTO.Uid, SamAccountName);
            if (response)
            {
                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;
            }
            else
            {
                Log.Instance.Debug("No record found for delete request");
                Response.error = Label.Get("error.delete");
                return false;
            }
        }
    }
}
