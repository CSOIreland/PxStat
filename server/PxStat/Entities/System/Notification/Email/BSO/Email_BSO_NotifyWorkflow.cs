using API;
using PxStat.Data;
using PxStat.Resources;
using PxStat.Security;
using PxStat.Workflow;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace PxStat.System.Notification
{
    internal class Email_BSO_NotifyWorkflow
    {
        /// <summary>
        /// Send email notifications after a WorkflowRequest has been created.
        /// Emails sent to the Group of the release
        /// </summary>
        /// <param name="requestDTO"></param>
        /// <param name="releaseDTO"></param>
        internal void EmailRequest(WorkflowRequest_DTO requestDTO, Release_DTO releaseDTO)
        {
            eMail email = new eMail();

            var v = getReleaseUrl(releaseDTO);

            Account_BSO aBso = new Account_BSO();
            ADO_readerOutput approvers = aBso.getReleaseUsers(releaseDTO.RlsCode, true);

            if (!approvers.hasData) return;

            List<string> emails = getEmailAddresses(approvers);


            if (emails.Count == 0) return;

            foreach (string person in emails)
            {
                email.Bcc.Add(person);
            }


            string releaseUrl = getReleaseUrl(releaseDTO);
            String subject = string.Format(Label.Get("email.subject.request-create"), releaseDTO.MtrCode, releaseDTO.RlsVersion, releaseDTO.RlsRevision);
            String body = string.Format(Label.Get("email.body.request-create"), requestDTO.RqsValue, releaseDTO.MtrCode, releaseDTO.RlsVersion, releaseDTO.RlsRevision, releaseUrl);

            sendMail(email, ConfigurationManager.AppSettings["APP_NAME"], subject, body);

            email.Dispose();
        }

        /// <summary>
        /// Send email notification after Response
        /// Emails sent to Group  of the release and all Power Users
        /// </summary>
        /// <param name="rspDTO"></param>
        /// <param name="releaseDTO"></param>
        internal void EmailResponse(WorkflowRequest_DTO requestDTO, WorkflowResponse_DTO rspDTO, Release_DTO releaseDTO)
        {
            eMail email = new eMail();

            Account_BSO aBso = new Account_BSO();

            ADO_readerOutput moderators = new ADO_readerOutput();
            ADO_readerOutput powerUsers = new ADO_readerOutput();

            moderators = aBso.getReleaseUsers(releaseDTO.RlsCode, false);



            if (rspDTO.RspCode == Constants.C_WORKFLOW_STATUS_APPROVE)
                powerUsers = aBso.getUsersOfPrivilege(Resources.Constants.C_SECURITY_PRIVILEGE_POWER_USER);

            List<string> allEmails = new List<string>();


            allEmails.AddRange(getEmailAddresses(powerUsers));

            allEmails.AddRange(getEmailAddresses(moderators));

            if (allEmails.Count == 0) return;

            foreach (string person in allEmails)
            {
                email.Bcc.Add(person);
            }


            string subject = "";
            string body = "";
            string releaseUrl = getReleaseUrl(releaseDTO);
            switch (rspDTO.RspCode)
            {


                case Constants.C_WORKFLOW_STATUS_APPROVE:

                    subject = string.Format(Label.Get("email.subject.response-approve"), releaseDTO.MtrCode, releaseDTO.RlsVersion, releaseDTO.RlsRevision);
                    body = string.Format(Label.Get("email.body.response-approve"), requestDTO.RqsValue, releaseDTO.MtrCode, releaseDTO.RlsVersion, releaseDTO.RlsRevision, releaseUrl);

                    break;

                case Constants.C_WORKFLOW_STATUS_REJECT:

                    subject = string.Format(Label.Get("email.subject.response-reject"), releaseDTO.MtrCode, releaseDTO.RlsVersion, releaseDTO.RlsRevision);
                    body = string.Format(Label.Get("email.body.response-reject"), requestDTO.RqsValue, releaseDTO.MtrCode, releaseDTO.RlsVersion, releaseDTO.RlsRevision, releaseUrl);

                    break;
            }



            sendMail(email, ConfigurationManager.AppSettings["APP_NAME"], subject, body);
            email.Dispose();
        }


        /// <summary>
        /// Send emails for a Signoff
        /// </summary>
        /// <param name="sgnDTO"></param>
        internal void EmailSignoff(WorkflowRequest_DTO requestDTO, WorkflowSignoff_DTO sgnDTO, Release_DTO releaseDTO, ADO_readerOutput moderators, ADO_readerOutput powerUsers)
        {
            eMail email = new eMail();

            List<string> emailsAll = new List<string>();

            emailsAll.AddRange(getEmailAddresses(powerUsers));
            emailsAll.AddRange(getEmailAddresses(moderators));

            if (emailsAll.Count == 0) return;

            foreach (string person in emailsAll)
            {
                email.Bcc.Add(person);
            }


            string subject = "";
            string body = "";
            string releaseUrl = getReleaseUrl(releaseDTO);
            switch (sgnDTO.SgnCode)
            {
                case Constants.C_WORKFLOW_STATUS_APPROVE:
                    subject = string.Format(Label.Get("email.subject.signoff-approve"), releaseDTO.MtrCode, releaseDTO.RlsVersion, releaseDTO.RlsRevision);
                    body = string.Format(Label.Get("email.body.signoff-approve"), requestDTO.RqsValue, releaseDTO.MtrCode, releaseDTO.RlsVersion, releaseDTO.RlsRevision, releaseUrl);


                    break;

                case Constants.C_WORKFLOW_STATUS_REJECT:

                    subject = string.Format(Label.Get("email.subject.signoff-reject"), releaseDTO.MtrCode, releaseDTO.RlsVersion, releaseDTO.RlsRevision);
                    body = string.Format(Label.Get("email.body.signoff-reject"), requestDTO.RqsValue, releaseDTO.MtrCode, releaseDTO.RlsVersion, releaseDTO.RlsRevision, releaseUrl);



                    break;
            }
            sendMail(email, ConfigurationManager.AppSettings["APP_NAME"], subject, body);
            email.Dispose();
        }

        /// <summary>
        /// Send mails for a workflow
        /// </summary>
        /// <param name="email"></param>
        /// <param name="title"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        private void sendMail(eMail email, string title, string subject, string body)
        {
            BBCode bbc = new BBCode();
            body = bbc.Transform(body);
            var listToParse = new List<eMail_KeyValuePair>();

            listToParse.Add(new eMail_KeyValuePair() { key = "{title}", value = title });
            listToParse.Add(new eMail_KeyValuePair() { key = "{subject}", value = subject });
            listToParse.Add(new eMail_KeyValuePair() { key = "{website_name}", value = ConfigurationManager.AppSettings["APP_NAME"] });
            listToParse.Add(new eMail_KeyValuePair() { key = "{website_url}", value = ConfigurationManager.AppSettings["APP_URL"] });
            listToParse.Add(new eMail_KeyValuePair() { key = "{body}", value = body });

            email.Subject = subject;
            email.Body = email.ParseTemplate(Properties.Resources.template_NotifyWorkflow, listToParse);
            email.Send();
        }

        /// <summary>
        /// For a list of users, get their emails from ActiveDirectory
        /// </summary>
        /// <param name="releaseDTO"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        private List<string> getEmailAddresses(ADO_readerOutput users)
        {

            List<string> emails = new List<string>();
            ADO ado = new ADO("defaultConnection");
            try

            {
                ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();
                adAdo.MergeAdToUsers(ref users);


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ado.Dispose();
            }

            foreach (var v in users.data)
            {
                //Only send mails if the user notification flag is set in their profile
                if ((bool)v.CcnNotificationFlag)
                    emails.Add(v.CcnEmail.ToString());
            }
            return emails;
        }

        /// <summary>
        /// Get a bbCode url for insertion into an email. Allows a hyperlink to be hidden in a hrefText
        /// </summary>
        /// <param name="dto">The Release DTO</param>
        /// <returns></returns>
        private string getReleaseUrl(Release_DTO dto)
        {
            return "[url=" + ConfigurationManager.AppSettings["APP_URL"] + "/" + Utility.GetCustomConfig("APP_COOKIELINK_RELEASE") + '/' + dto.RlsCode.ToString() + "]" + Label.Get("static.release") + " " + dto.RlsVersion.ToString() + "." + dto.RlsRevision.ToString() + "[/url]";
        }

    }
}
