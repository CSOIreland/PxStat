using API;
using PxStat.System.Notification;
using PxStat.Template;
using System;
using System.Collections.Generic;

namespace PxStat.Security
{
    internal class Login_BSO_InitiateUpdate1FA_Current : BaseTemplate_Update<Login_DTO_Update1FA_Session, Login_VLD_Update1FA_Session>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Login_BSO_InitiateUpdate1FA_Current(JSONRPC_API request) : base(request, new Login_VLD_Update1FA_Session())
        {
        }


        override protected bool HasPrivilege()
        {
            return true;
        }

        protected override bool HasUserToBeAuthenticated()
        {
            return true;
        }

        protected override bool Execute()
        {


            if (Request.sessionCookie == null)
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }
            Login_BSO lBso = new Login_BSO(Ado);

            var userResponse = lBso.ReadBySession(Request.sessionCookie.Value);
            if (userResponse.hasData)
            {

                string user = userResponse.data[0].CcnUsername;
                //This should not be allowed for an AD user
                DTO.CcnUsername = user;
                ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();
                ActiveDirectory_DTO adDto = adAdo.GetUser(Ado, DTO);
                if (adDto.CcnDisplayName != null)
                {
                    Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                    return true;
                }

                
                string token = Utility.GetSHA256(new Random().Next() + userResponse.data[0].CcnId.ToString() + DateTime.Now.Millisecond);

                if (lBso.Update1FaTokenForUser(userResponse.data[0].CcnUsername, token) != null)
                {
                    SendEmail(new Login_DTO_Create() { CcnUsername = userResponse.data[0].CcnUsername, LngIsoCode = DTO.LngIsoCode, CcnEmail = userResponse.data[0].CcnEmail, CcnDisplayname = userResponse.data[0].CcnDisplayName }, token, "PxStat.Security.Login_API.Update1FA");
                    Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                    return true;
                }



                return false;
            }
            Response.error = Label.Get("error.authentication");
            return false;

        }

        private void SendEmail(Login_DTO_Create lDto, string token, string nextMethod)
        {

            string url = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.application") + "?method=" + nextMethod + "&email=" + lDto.CcnEmail + '&' + "name=" + Uri.EscapeUriString(lDto.CcnDisplayname) + '&' + "token=" + token;
            string link = "<a href = " + url + ">" + Label.Get("email.body.header.anchor-text", lDto.LngIsoCode) + "</a>";
            string subject = string.Format(Label.Get("email.subject.update-1fa", lDto.LngIsoCode), Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "title"));
            string to = lDto.CcnEmail;
            string header = string.Format(Label.Get("email.body.header.update-1fa", lDto.LngIsoCode), lDto.CcnDisplayname, Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "title"));
            string subHeader = string.Format(Label.Get("email.body.sub-header.update-1fa"), link);
            string footer = string.Format(Label.Get("email.body.footer", lDto.LngIsoCode), lDto.CcnDisplayname);

            Email_BSO.SendLoginTemplateEmail(subject, new List<string>() { to }, header, url, footer, subHeader, lDto.LngIsoCode);
        }

    }
}
