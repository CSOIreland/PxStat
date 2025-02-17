using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscriber_BSO_Update : BaseTemplate_Update<Subscriber_DTO_Update, Subscriber_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscriber_BSO_Update(JSONRPC_API request) : base(request, new Subscriber_VLD_Update())
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

            string subscriberUserId;
            if (SamAccountName != null)
            {
                Response.error = Label.Get("error.firebase.alreadySubscribed", DTO.LngIsoCode);
                return false;
            }

            else
            {
                
                if (!AppServicesHelper.Firebase.Authenticate(DTO.Uid, DTO.AccessToken, ApiServicesHelper.ApiConfiguration.Settings, Log.Instance))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
                subscriberUserId = DTO.Uid;
            }

            Subscriber_ADO ado = new Subscriber_ADO(Ado);

            if (ado.Update(subscriberUserId, DTO.LngIsoCode, DTO.SbrPreference))
            {
                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;
            }
            Log.Instance.Debug("Can't update Subscriber");
            Response.error = Label.Get("error.update");
            return false;

        }

    }

}
