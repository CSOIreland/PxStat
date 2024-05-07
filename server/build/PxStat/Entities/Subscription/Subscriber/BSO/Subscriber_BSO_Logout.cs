using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscriber_BSO_Logout : BaseTemplate_Update<Subscriber_DTO_Logout, Subscriber_VLD_Logout>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscriber_BSO_Logout(JSONRPC_API request) : base(request, new Subscriber_VLD_Logout())
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
            if (SamAccountName != null)
            {
                Response.error = Label.Get("error.firebase.alreadySubscribed", DTO.LngIsoCode);
                return false;
            }

            else
            {

                if (!AppServicesHelper.Firebase.Logout(DTO.Uid, DTO.AccessToken))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
                Response.data = Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;
            }



        }

    }

}
