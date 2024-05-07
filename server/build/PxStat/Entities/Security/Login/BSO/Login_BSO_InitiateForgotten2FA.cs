using API;
using PxStat.System.Notification;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Security
{
    internal class Login_BSO_InitiateForgotten2FA : BaseTemplate_Update<Login_DTO_InitiateForgotten2FA, Login_VLD_InitiateForgotten2FA>
    {
        /// <param name="request"></param>
        internal Login_BSO_InitiateForgotten2FA(JSONRPC_API request) : base(request, new Login_VLD_InitiateForgotten2FA())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {


            Login_BSO lBso = new Login_BSO(Ado);

            ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();

            dynamic adUser = adAdo.GetAdSpecificDataForEmail(DTO.CcnEmail);

            if (adUser?.CcnEmail != null)
            {
                //Check if local access is available for AD users
                if (!Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "security.adOpenAccess"))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
                DTO.CcnEmail = adUser.CcnEmail;
                DTO.CcnDisplayname = adUser.CcnDisplayName;
                DTO.CcnUsername = adUser.CcnUsername;
            }
            else
            {
                Account_ADO aAdo = new Account_ADO();
                var user = aAdo.Read(Ado, new Account_DTO_Read() { CcnUsername = DTO.CcnEmail });
                if (!user.hasData)
                {
                    Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                    return true;
                }

                if (user.data[0].CcnEmail.Equals(DBNull.Value) || user.data[0].CcnDisplayName.Equals(DBNull.Value))
                {
                    Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                    return true;
                }

                DTO.CcnDisplayname = user.data[0].CcnDisplayName;
                DTO.CcnEmail = user.data[0].CcnEmail;
                DTO.CcnUsername = DTO.CcnEmail;

            }

            string token = Utility.GetSHA256(new Random().Next() + DTO.CcnUsername + DateTime.Now.Millisecond);

            lBso.UpdateInvitationToken2Fa(DTO.CcnUsername, token);

            if (token != null)
            {
                SendEmail(new Login_DTO_Create() { CcnUsername = DTO.CcnUsername, CcnEmail = DTO.CcnEmail, LngIsoCode = DTO.LngIsoCode, CcnDisplayname = DTO.CcnDisplayname }, token, "PxStat.Security.Login_API.Update2FA");
                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;
            }

            Response.error = Label.Get("error.create");
            return false;

        }

        private void SendEmail(Login_DTO_Create lDto, string token, string nextMethod)
        {

            string url = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.application") + "?method=" + nextMethod + "&email=" + lDto.CcnEmail + '&' + "name=" + Uri.EscapeUriString(lDto.CcnDisplayname) + '&' + "token=" + token;
            string link = "<a href = " + url + ">" + Label.Get("email.body.header.anchor-text", lDto.LngIsoCode) + "</a>";
            string subject = string.Format(Label.Get("email.subject.update-2fa", lDto.LngIsoCode), Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "title"));
            string to = lDto.CcnEmail;
            string header = string.Format(Label.Get("email.body.header.update-2fa", lDto.LngIsoCode), lDto.CcnDisplayname, Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "title"));
            string subHeader = string.Format(Label.Get("email.body.sub-header.update-2fa"), link);
            string footer = string.Format(Label.Get("email.body.footer", lDto.LngIsoCode), lDto.CcnDisplayname);
            List<string> list = (string.Format(Label.Get("email.body.sub-header.list-2fa", lDto.LngIsoCode)).Split('~')).ToList<string>();

            Email_BSO.SendLoginTemplateBulletPointEmail(subject, new List<string>() { to }, header, url, footer, list, subHeader, lDto.LngIsoCode);
        }
    }
}
