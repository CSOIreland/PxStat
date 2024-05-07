using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscriber_BSO_UpdateKey : BaseTemplate_Update<Subscriber_DTO_UpdateKey, Subscriber_VLD_UpdateKey>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscriber_BSO_UpdateKey(JSONRPC_API request) : base(request, new Subscriber_VLD_UpdateKey())
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

                if (!AppServicesHelper.Firebase.Authenticate(DTO.Uid, DTO.AccessToken))
                {
                    Response.error = Label.Get("error.authentication", DTO.LngIsoCode);
                    return false;
                }
            }

            Subscriber_ADO ado = new Subscriber_ADO(Ado);
            Subscriber_BSO sBso = new Subscriber_BSO();

            string newKey = sBso.GetSubscriberKey(DTO.Uid);
            if (ado.UpdateKey(DTO.Uid, newKey))
            {
                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;
            }
            Log.Instance.Debug("Can't update Subscriber Key");
            Response.error = Label.Get("error.update", DTO.LngIsoCode);
            return false;

        }

    }

}
