using API;
using PxStat.Template;

namespace PxStat.Subscription
{
    internal class Subscriber_BSO_Delete : BaseTemplate_Delete<Subscriber_DTO_Delete, Subscriber_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subscriber_BSO_Delete(JSONRPC_API request) : base(request, new Subscriber_VLD_Delete())
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

            //A local/ad user can't delete themselves
            //However an admin user can delete a subscriber
            if (SamAccountName != null)
            {
                if (IsAdministrator() && DTO.Uid != null)
                    subscriberUserId = DTO.Uid;
                else
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
            }

            else
            {
                //The subscriber is attempting to delete themselves

                if (!API.Firebase.Authenticate(DTO.Uid, DTO.AccessToken))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
                subscriberUserId = DTO.Uid;
            }

            Subscriber_BSO bso = new Subscriber_BSO();



            if (bso.Delete(Ado, subscriberUserId))
            {
                //Delete the user also from Firebase
                API.Firebase.DeleteUser(subscriberUserId);

                //Refresh the cache of subscriber keys (for throttling)
                new Subscriber_BSO().RefreshSubscriberKeyCache(Ado);

                //Refresh the cache of subscribers (for reading)
                bso.GetSubscribers(Ado, null, false);
                Response.data = JSONRPC.success;
                return true;
            }
            Log.Instance.Debug("Can't delete Subscriber");
            Response.error = Label.Get("error.delete");
            return false;

        }

    }

}
