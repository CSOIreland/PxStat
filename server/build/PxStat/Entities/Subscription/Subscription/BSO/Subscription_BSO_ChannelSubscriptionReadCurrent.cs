using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_ChannelSubscriptionReadCurrent : BaseTemplate_Read<Subscription_DTO_ChannelSubscriptionReadCurrent, Subscription_VLD_ChannelSubscriptionReadCurrent>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_ChannelSubscriptionReadCurrent(JSONRPC_API request) : base(request, new Subscription_VLD_ChannelSubscriptionReadCurrent())
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

            Subscription_ADO sAdo = new Subscription_ADO(Ado);
            var response = sAdo.ChannelReadCurrent(DTO.LngIsoCode, DTO.Uid, SamAccountName);

            if (response.hasData)
            {
                Response.data = response.data;
                return true;
            }
            return false;

        }

    }

}
