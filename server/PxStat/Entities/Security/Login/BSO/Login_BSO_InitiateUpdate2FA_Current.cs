using API;
using PxStat.System.Notification;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Security
{
    internal class Login_BSO_InitiateUpdate2FA_Current : BaseTemplate_Update<Login_DTO_Update2FACurrent, Login_VLD_Update2FACurrent>
    {
        /// <param name="request"></param>
        internal Login_BSO_InitiateUpdate2FA_Current(JSONRPC_API request) : base(request, new Login_VLD_Update2FACurrent())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true; ;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {

            Login_BSO lBso = new Login_BSO(Ado);


            ADO_readerOutput user;
            string displayName = null;
            string email = null;
            string ccnUsername = null;



            if (SamAccountName != null)
            {
                ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();
                ActiveDirectory_DTO adDto = adAdo.GetUser(Ado, new Account_DTO_Create() { CcnUsername = SamAccountName });
                displayName = adDto.CcnDisplayName;
                email = adDto.CcnEmail;
                ccnUsername = adDto.CcnUsername;
            }

            if (ccnUsername == null)
            {
                if (Request.sessionCookie == null)
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
                user = lBso.ReadBySession(Request.sessionCookie.Value);
                if (user.hasData)
                {
                    if (user.data[0].CcnEmail.Equals(DBNull.Value) || user.data[0].CcnDisplayName.Equals(DBNull.Value))
                    {
                        Response.data = JSONRPC.success;
                        return true;
                    }
                    displayName = user.data[0].CcnDisplayName;
                    email = user.data[0].CcnEmail;
                    ccnUsername = user.data[0].CcnUsername;
                }
            }

            if (ccnUsername == null)
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }


            string token = Utility.GetRandomSHA256(ccnUsername);

            lBso.UpdateInvitationToken2Fa(ccnUsername, token);

            if (token != null)
            {
                SendEmail(new Login_DTO_Create() { CcnUsername = ccnUsername, LngIsoCode = DTO.LngIsoCode, CcnEmail = email, CcnDisplayname = displayName }, token, "PxStat.Security.Login_API.Update2FA");
                Response.data = JSONRPC.success;
                return true;
            }


            Response.error = Label.Get("error.create");
            return false;

        }

        private void SendEmail(Login_DTO_Create lDto, string token, string nextMethod)
        {

            string url = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.application") + "?method=" + nextMethod + "&email=" + lDto.CcnEmail + '&' + "name=" + Uri.EscapeUriString(lDto.CcnDisplayname) + '&' + "token=" + token;
            string link = "<a href = " + url + ">" + Label.Get("email.body.header.anchor-text", lDto.LngIsoCode) + "</a>";
            string subject = string.Format(Label.Get("email.subject.update-2fa", lDto.LngIsoCode), Configuration_BSO.GetCustomConfig(ConfigType.global, "title"));
            string to = lDto.CcnEmail;
            string header = string.Format(Label.Get("email.body.header.update-2fa", lDto.LngIsoCode), lDto.CcnDisplayname, Configuration_BSO.GetCustomConfig(ConfigType.global, "title"));
            string subHeader = string.Format(Label.Get("email.body.sub-header.update-2fa"), link);
            string footer = string.Format(Label.Get("email.body.footer", lDto.LngIsoCode), lDto.CcnDisplayname);
            List<string> list = (string.Format(Label.Get("email.body.sub-header.list-2fa", lDto.LngIsoCode)).Split('~')).ToList<string>();

            Email_BSO.SendLoginTemplateBulletPointEmail(subject, new List<string>() { to }, header, url, footer, list, subHeader, lDto.LngIsoCode);
        }

    }
}
