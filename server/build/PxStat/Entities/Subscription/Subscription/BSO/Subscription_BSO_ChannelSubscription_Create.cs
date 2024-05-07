using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_ChannelSubscription_Create : BaseTemplate_Create<Subscription_DTO_ChannelSubscriptionCreate, Subscription_VLD_ChannelSubscriptionCreate>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_ChannelSubscription_Create(JSONRPC_API request) : base(request, new Subscription_VLD_ChannelSubscriptionCreate())
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

                if (!AppServicesHelper.Firebase.Authenticate(DTO.Uid, DTO.AccessToken))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
            }

            Subscription_ADO ado = new Subscription_ADO(Ado);

            if (ado.ChannelCreate(DTO.ChnCode, DTO.Uid, SamAccountName))
            {
                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;
            }
            Log.Instance.Debug("Can't create Subscription");
            Response.error = Label.Get("error.create");
            return false;
        }
    }

}
