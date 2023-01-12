using System.Linq;
using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscriber_BSO_Read : BaseTemplate_Read<Subscriber_DTO_Read, Subscriber_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscriber_BSO_Read(JSONRPC_API request) : base(request, new Subscriber_VLD_Read())
        {
        }


        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator() || IsPowerUser();
        }
        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Subscriber_BSO sbso = new Subscriber_BSO();
            var response = sbso.GetSubscribers(Ado, DTO.Uid);

            // Remove d from response if the email and display name are null 
            foreach (var d in response.ToList())
            {
                if (d.CcnEmail == null && d.DisplayName == null)
                {
                    response.Remove(d);
                }
            }

            if (response.Count > 0)
            {
                Response.data = response;
                return true;
            }
            return false;

        }

    }

}
