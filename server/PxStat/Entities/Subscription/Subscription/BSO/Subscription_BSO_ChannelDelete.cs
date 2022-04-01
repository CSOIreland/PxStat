using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_ChannelDelete : BaseTemplate_Delete<Subscription_DTO_ChannelDelete, Subscription_VLD_ChannelDelete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_ChannelDelete(JSONRPC_API request) : base(request, new Subscription_VLD_ChannelDelete())
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

            if (ado.ChannelDelete(DTO.ChnName, API.Entities.Firebase.Uid, SamAccountName))
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
