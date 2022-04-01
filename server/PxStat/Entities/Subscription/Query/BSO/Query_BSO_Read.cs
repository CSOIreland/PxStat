using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Query_BSO_Read : BaseTemplate_Read<Query_DTO_Read, Query_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Query_BSO_Read(JSONRPC_API request) : base(request, new Query_VLD_Read())
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

                if (!API.Firebase.Authenticate(DTO.Uid, DTO.AccessToken))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
            }

            Query_ADO ado = new Query_ADO(Ado);
            var response = ado.Read(DTO.UserQueryId, DTO.Uid, SamAccountName);
            if (response.hasData)
            {
                Response.data = response.data;
            }
            return true;
        }
    }
}
