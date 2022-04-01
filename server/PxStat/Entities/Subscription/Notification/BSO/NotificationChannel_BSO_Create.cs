using API;
using PxStat.Security;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Subscription
{
    internal class NotificationChannel_BSO_Create : BaseTemplate_Create<Notification_DTO_Create, Notification_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal NotificationChannel_BSO_Create(JSONRPC_API request) : base(request, new Notification_VLD_Create())
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
            return IsPowerUser() || IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //If we don't know who you are then we won't allow the method to execute
            if (Common.FirebaseId == null && SamAccountName == null)
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            //Get a list of subscribers to the channel
            //If DTO.singleLangage is asserted then we only want users who prefer the associated langauge
            Subscription_BSO subBso = new Subscription_BSO();
            var chnSubs = subBso.ChannelRead(Ado, DTO.LngIsoCode, DTO.ChnCode, DTO.singleLanguage);

            //Get the name of the requested channel
            var channel = new Channel_BSO().Read(Ado, DTO.LngIsoCode, DTO.ChnCode).data.FirstOrDefault();
            string cName = "";
            if (channel != null)
                cName = channel.ChnName;

            int attemptCounter = 0;
            List<string> emailsNotSent = new List<string>();
            foreach (var user in chnSubs)
            {
                if (String.IsNullOrEmpty(user.CcnEmail))
                    continue;
                attemptCounter++;
                using (eMail email = new eMail())
                {
                    try
                    {
                        email.Body = DTO.EmailBody ?? Label.Get("email.subscription.notification-body", DTO.LngIsoCode);
                        email.Subject = DTO.EmailSubject ?? Label.Get("email.subscription.notification-body", DTO.LngIsoCode);
                        string salutation = String.Format(Label.Get("email.salutation-informal", DTO.LngIsoCode), user.FullName);

                        email.To.Add(user.CcnEmail);
                        Log.Instance.Debug($"Send notification to {user.CcnEmail}");
                        if (!sendMail(email, String.Format(Label.Get("email.subscription.notification-title", DTO.LngIsoCode), cName), email.Subject, email.Body, salutation))
                            emailsNotSent.Add(user.CcnEmail);
                    }
                    catch (Exception ex)
                    {
                        Log.Instance.Error("Error sending email " + ex.Message);
                        emailsNotSent.Add(user.CcnEmail);
                    }
                }
            }
            Response.data = emailsNotSent;
            return true;
        }


        /// <summary>
        /// Builds an email based on the template
        /// </summary>
        /// <param name="email"></param>
        /// <param name="title"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="salutationRecipient"></param>
        private bool sendMail(eMail email, string title, string subject, string body, string salutationRecipient)
        {
            var listToParse = new List<eMail_KeyValuePair>();


            listToParse.Add(new eMail_KeyValuePair() { key = "{title}", value = title });
            listToParse.Add(new eMail_KeyValuePair() { key = "{subject}", value = subject });
            listToParse.Add(new eMail_KeyValuePair() { key = "{salutation}", value = salutationRecipient });
            listToParse.Add(new eMail_KeyValuePair() { key = "{body}", value = body });
            listToParse.Add(new eMail_KeyValuePair() { key = "{website_name}", value = Configuration_BSO.GetCustomConfig(ConfigType.global, "title") });
            listToParse.Add(new eMail_KeyValuePair() { key = "{website_url}", value = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.application") });
            listToParse.Add(new eMail_KeyValuePair() { key = "{image_source}", value = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.logo") });
            listToParse.Add(new eMail_KeyValuePair() { key = "{datetime_label}", value = Label.Get("label.date-time", Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code")) });
            listToParse.Add(new eMail_KeyValuePair() { key = "{date_format}", value = Label.Get("label.date-format", Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code")) });
            listToParse.Add(new eMail_KeyValuePair() { key = "{timezone}", value = Label.Get("label.timezone", Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code")) });


            email.Subject = subject;
            email.Body = email.ParseTemplate(Properties.Resources.template_NotifyChannelSubscription, listToParse);
            return email.Send();
        }
    }
}
