﻿using API;
using CSO.Email;
using PxStat.Security;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            DTO.Body = bbc.Transform(DTO.Body, true);

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
                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                Log.Instance.Debug("No email addresses found");
                return true;
            }
            string emailRegex = Configuration_BSO.GetStaticConfig("APP_REGEX_EMAIL");

            foreach (var user in readGroupAccounts.data)
            {
                if (user.CcnEmail != null)
                {
                    string address = user.CcnEmail.ToString();
                    if (Regex.IsMatch(address, emailRegex))
                        email.Bcc.Add(address);
                }
            }

            foreach (var user in readPowerUsers.data)
            {
                if (user.CcnEmail != null)
                {
                    string address = user.CcnEmail.ToString();
                    if (Regex.IsMatch(address, emailRegex))
                        email.Bcc.Add(address);
                }
            }


            email.Subject = DTO.Subject;
            email.Body = DTO.Body;

            sendMail(email, Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "title"), DTO.Subject, DTO.Body);

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
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

            string grouplist = grpCodes.Any() ? String.Join(", ", grpCodes) : Label.Get("static.all-groups");

            listToParse.Add(new eMail_KeyValuePair() { key = "{title}", value = title });
            listToParse.Add(new eMail_KeyValuePair() { key = "{subject}", value = subject });
            listToParse.Add(new eMail_KeyValuePair() { key = "{body}", value = body });
            listToParse.Add(new eMail_KeyValuePair() { key = "{received-by}", value = string.Format(Label.Get("label.timezone", Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code")), grouplist) });
            listToParse.Add(new eMail_KeyValuePair() { key = "{website_name}", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "title") });
            listToParse.Add(new eMail_KeyValuePair() { key = "{website_url}", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.application") });
            listToParse.Add(new eMail_KeyValuePair() { key = "{image_source}", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.logo") });
            listToParse.Add(new eMail_KeyValuePair() { key = "{datetime_label}", value = Label.Get("label.date-time", Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code")) });
            listToParse.Add(new eMail_KeyValuePair() { key = "{date_format}", value = Label.Get("label.date-format", Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code")) });
            listToParse.Add(new eMail_KeyValuePair() { key = "{timezone}", value = Label.Get("label.timezone", Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code")) });


            email.Subject = subject;
            email.Body = email.ParseTemplate(Properties.Resources.template_GroupMessage, listToParse,Log.Instance);
            email.Send(ApiServicesHelper.ApiConfiguration.Settings, Log.Instance);
        }

    }
}
