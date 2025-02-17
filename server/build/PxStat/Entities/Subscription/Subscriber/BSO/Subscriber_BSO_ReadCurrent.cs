using API;
using PxStat.Template;
using System.Linq;

namespace PxStat.Subscription
{
    internal class Subscriber_BSO_ReadCurrent : BaseTemplate_Read<Subscriber_DTO_ReadCurrent, Subscriber_VLD_ReadCurrent>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscriber_BSO_ReadCurrent(JSONRPC_API request) : base(request, new Subscriber_VLD_ReadCurrent())
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
            //if (SamAccountName != null)
            //{
            //    Response.error = Label.Get("error.authentication");
            //    return false;
            //}


                if (!AppServicesHelper.Firebase.Authenticate(DTO.Uid, DTO.AccessToken, ApiServicesHelper.ApiConfiguration.Settings, Log.Instance))
                {
                Response.error = Label.Get("error.authentication");
                return false;
            }


            Subscriber_BSO sbso = new Subscriber_BSO();
            var responseSubscriber = sbso.GetSubscribers(Ado, DTO.Uid);



            if (responseSubscriber.Count > 0)
            {

                Response.data = responseSubscriber.FirstOrDefault();
                return true;
            }
            return false;



        }

    }

}
