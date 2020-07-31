using API;
using PxStat.Security;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.System.Notification
{
    internal class Email_BSO_GroupMessage_Create : BaseTemplate_Read<Email_DTO_GroupMessage, Email_VLD>
    {
        internal Email_BSO_GroupMessage_Create(JSONRPC_API request) : base(request, new Email_VLD())
        { }

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
            //Translate from bbCode to html
            Resources.BBCode bbc = new Resources.BBCode();
            DTO.Body = bbc.Transform(DTO.Body);

            List<string> groupCodes = new List<string>();

            GroupAccount_ADO gAdo = new GroupAccount_ADO();

            foreach (var code in DTO.GroupCodes)
            {
                groupCodes.Add(code.GrpCode);
            }

            //Get accounts associated with the Group(s)
            ADO_readerOutput readGroupAccounts = gAdo.ReadMultiple(Ado, groupCodes);

            Account_ADO accAdo = new Account_ADO();
            Account_DTO_Read dtoRead = new Account_DTO_Read();
            dtoRead.PrvCode = Resources.Constants.C_SECURITY_PRIVILEGE_POWER_USER;
            ADO_readerOutput readPowerUsers = accAdo.Read(Ado, dtoRead);

            //Get the AD data associated with the users, specifically 
            ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();
            adAdo.MergeAdToUsers(ref readGroupAccounts);
            adAdo.MergeAdToUsers(ref readPowerUsers);

            eMail email = new eMail();

            if (!readGroupAccounts.hasData && !readPowerUsers.hasData)
            {
                Response.data = JSONRPC.success;
                Log.Instance.Debug("No email addresses found");
                return true;
            }

            foreach (var user in readGroupAccounts.data)
            {
                email.Bcc.Add(user.CcnEmail.ToString());
            }

            foreach (var user in readPowerUsers.data)
            {
                email.Bcc.Add(user.CcnEmail.ToString());
            }


            email.Subject = DTO.Subject;
            email.Body = DTO.Body;

            sendMail(email, Configuration_BSO.GetCustomConfig("title"), DTO.Subject, DTO.Body);

            Response.data = JSONRPC.success;
            return true;
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
            var listToParse = new List<eMail_KeyValuePair>();

            List<string> grpCodes = new List<string>();
            grpCodes = DTO.GroupCodes.Select(s => (string)s.GrpCode).ToList();

            listToParse.Add(new eMail_KeyValuePair() { key = "{title}", value = title });
            listToParse.Add(new eMail_KeyValuePair() { key = "{subject}", value = subject });
            listToParse.Add(new eMail_KeyValuePair() { key = "{body}", value = body });
            listToParse.Add(new eMail_KeyValuePair() { key = "{grouplist}", value = grpCodes.Any() ? String.Join(", ", grpCodes) : Label.Get("static.all-groups") });
            listToParse.Add(new eMail_KeyValuePair() { key = "{website_name}", value = Configuration_BSO.GetCustomConfig("title") });
            listToParse.Add(new eMail_KeyValuePair() { key = "{website_url}", value = Configuration_BSO.GetCustomConfig("url.application") });

            email.Subject = subject;
            email.Body = email.ParseTemplate(Properties.Resources.template_GroupMessage, listToParse);
            email.Send();
        }

    }
}
