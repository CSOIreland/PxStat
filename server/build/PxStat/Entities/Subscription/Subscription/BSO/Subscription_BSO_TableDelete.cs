using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_TableDelete : BaseTemplate_Delete<Subscription_DTO_TableDelete, Subscription_VLD_TableDelete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_TableDelete(JSONRPC_API request) : base(request, new Subscription_VLD_TableDelete())
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
                API.Entities.Firebase.Uid = DTO.Uid;
                API.Entities.Firebase.AccessToken = DTO.AccessToken;

                if (!API.Entities.Firebase.Authenticate())
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
            }


            Subscription_ADO ado = new Subscription_ADO(Ado);

            if (ado.TableDelete(DTO.TsbTable, API.Entities.Firebase.Uid, SamAccountName))
            {
                Response.data = JSONRPC.success;
                return true;
            }
            Log.Instance.Debug("Can't delete Subscription");
            Response.error = Label.Get("error.delete");
            return false;
        }
    }

}
