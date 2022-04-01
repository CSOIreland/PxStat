using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscription_BSO_ChannelRead : BaseTemplate_Read<Subscription_DTO_ChannelRead, Subscription_VLD_ChannelRead>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscription_BSO_ChannelRead(JSONRPC_API request) : base(request, new Subscription_VLD_ChannelRead())
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
            var response = sAdo.ChannelRead(DTO.ChnName);

            if (response.hasData)
            {
                Response.data = response.data;
                return true;
            }
            return false;

        }
    }
}
