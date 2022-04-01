using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_ChannelCreate : BaseTemplate_Create<Subscription_DTO_ChannelCreate, Subscription_VLD_ChannelCreate>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_ChannelCreate(JSONRPC_API request) : base(request, new Subscription_VLD_ChannelCreate())
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

            if (ado.ChannelCreate(DTO.ChnName, API.Entities.Firebase.Uid, SamAccountName))
            {
                Response.data = JSONRPC.success;
                return true;
            }
            Log.Instance.Debug("Can't create Subscription");
            Response.error = Label.Get("error.create");
            return false;
        }
    }

}
