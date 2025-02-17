using API;
using PxStat.Template;

namespace PxStat.Subscription
{
   
    internal class Subscriber_BSO_Create : BaseTemplate_Create<Subscriber_DTO_Create, Subscriber_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscriber_BSO_Create(JSONRPC_API request) : base(request, new Subscriber_VLD_Create())
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

            if (!AppServicesHelper.Firebase.Authenticate(DTO.Uid, DTO.AccessToken, ApiServicesHelper.ApiConfiguration.Settings, Log.Instance))
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            Subscriber_BSO bso = new Subscriber_BSO();


            if (bso.Create(Ado, DTO.SbrPreference, DTO.Uid, DTO.LngIsoCode))
            {
                //Calling GetSubscribers with readCache=false will cause the cache to be refreshed to include the new user
                bso.GetSubscribers(Ado, null, false);
                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;
            }
            Log.Instance.Debug("Can't create Subscription");
            Response.error = Label.Get("error.create");
            return false;
        }
    }

}
