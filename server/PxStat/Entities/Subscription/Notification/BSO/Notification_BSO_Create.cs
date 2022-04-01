using API;
using PxStat.Template;
using System.Linq;

namespace PxStat.Subscription
{
    internal class Notification_BSO_Create : BaseTemplate_Create<Notification_DTO_Create, Notification_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Notification_BSO_Create(JSONRPC_API request) : base(request, new Notification_VLD_Create())
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
            return IsPowerUser();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            if (Common.FirebaseId == null && SamAccountName == null)
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            using (eMail email = new eMail())
            {
                string Body = "";
                string Subject = "";

                Body = Label.Get("email.body.subscription-notification", DTO.LngIsoCode);
                Subject = Label.Get("email.subject.subscription-notification", DTO.LngIsoCode);
                email.Body = Body;
                email.Subject = Subject;

                // Get user from FirebaseId
                Subscriber_BSO sbso = new Subscriber_BSO();
                var user = sbso.GetSubscribers(Ado, Common.FirebaseId);

                if (user.FirstOrDefault() == null)
                {
                    Log.Instance.Debug("Cannot send notification because email address is null");
                    Response.error = Label.Get("error.notification");
                    return false;
                }

                // Get email address from user
                string emailAddress = user.FirstOrDefault().CcnEmail;
                email.To.Add(emailAddress);
                Log.Instance.Debug($"Send notification to {emailAddress}");
                email.Send();
            }
            return true;
        }
    }
}
