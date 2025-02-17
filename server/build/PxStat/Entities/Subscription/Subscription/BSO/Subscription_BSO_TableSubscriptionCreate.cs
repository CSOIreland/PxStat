using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_TableSubscriptionCreate : BaseTemplate_Create<Subscription_DTO_TableSubscriptionCreate, Subscription_VLD_TableSubscriptionCreate>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_TableSubscriptionCreate(JSONRPC_API request) : base(request, new Subscription_VLD_TableSubscriptionCreate())
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

                if (!AppServicesHelper.Firebase.Authenticate(DTO.Uid, DTO.AccessToken, ApiServicesHelper.ApiConfiguration.Settings, Log.Instance))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
            }

            Subscription_ADO ado = new Subscription_ADO(Ado);

            if (ado.TableCreate(DTO.TsbTable, DTO.Uid, SamAccountName))
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
