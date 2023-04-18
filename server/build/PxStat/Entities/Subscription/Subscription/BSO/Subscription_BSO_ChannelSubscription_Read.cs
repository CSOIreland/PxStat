using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_ChannelSubscription_Read : BaseTemplate_Read<Subscription_DTO_ChannelSubscriptionRead, Subscription_VLD_ChannelSubscriptionRead>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_ChannelSubscription_Read(JSONRPC_API request) : base(request, new Subscription_VLD_ChannelSubscriptionRead())
        {
        }


        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsAdministrator();
        }
        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            if (SamAccountName == null)
            {

                Response.error = Label.Get("error.authentication");
                return false;

            }

            Subscription_ADO sAdo = new Subscription_ADO(Ado);
            var response = sAdo.ChannelReadCurrent(DTO.LngIsoCode, DTO.SbrUserId, DTO.CcnUsername);


            Response.data = response.data;
            return true;

        }
    }
}
