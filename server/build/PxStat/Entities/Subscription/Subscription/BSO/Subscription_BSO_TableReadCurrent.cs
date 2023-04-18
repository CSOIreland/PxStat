using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_TableReadCurrent : BaseTemplate_Read<Subscription_DTO_TableReadCurrent, Subscription_VLD_TableReadCurrent>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_TableReadCurrent(JSONRPC_API request) : base(request, new Subscription_VLD_TableReadCurrent())
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
            var response = sAdo.TableReadCurrent(API.Entities.Firebase.Uid, SamAccountName);

            if (response.hasData)
            {
                Response.data = response.data;
                return true;
            }
            return false;

        }

    }

}
