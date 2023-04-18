using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Channel_BSO_Read : BaseTemplate_Read<Channel_DTO_Read, Subscription_VLD_ChannelRead>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Channel_BSO_Read(JSONRPC_API request) : base(request, new Subscription_VLD_ChannelRead())
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
            Channel_BSO cBso = new Channel_BSO();


            var response = cBso.Read(Ado, DTO.LngIsoCode, DTO.ChnCode);

            if (response.hasData)
            {
                Response.data = response.data;
                return true;
            }

            return false;

        }

    }

}
