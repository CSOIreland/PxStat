using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_ChannelReadCurrent : BaseTemplate_Read<Subscription_DTO_ChannelReadCurrent, Subscription_VLD_ChannelReadCurrent>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_ChannelReadCurrent(JSONRPC_API request) : base(request, new Subscription_VLD_ChannelReadCurrent())
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

            Subscription_ADO sAdo = new Subscription_ADO(Ado);
            var response = sAdo.ChannelReadCurrent(API.Entities.Firebase.Uid, SamAccountName);

            if (response.hasData)
            {
                Response.data = response.data;
                return true;
            }
            return false;

        }

    }

}
