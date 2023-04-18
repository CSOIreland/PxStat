using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_TableSubscription_Read : BaseTemplate_Read<Subscription_DTO_TableSubscriptionRead, Subscription_VLD_TableSubscriptionRead>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_TableSubscription_Read(JSONRPC_API request) : base(request, new Subscription_VLD_TableSubscriptionRead())
        {
        }


        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }
        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {

            Subscription_BSO sbso = new Subscription_BSO();
            var response = sbso.TableRead(Ado, DTO.TsbTable);

            if (response.Count > 0)
            {
                Response.data = response;
                return true;
            }
            return false;

        }

    }

}
