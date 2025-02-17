using API;
using CSO.Recaptcha;
using PxStat.System.Notification;
using PxStat.Template;
using System;
using System.Collections.Generic;

namespace PxStat.Security
{
    internal class Login_BSO_Create1FA : BaseTemplate_Create<Login_DTO_Create1FA, Login_VLD_Create1FA>
    {
        bool sendMail;
        bool send2FA;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Login_BSO_Create1FA(JSONRPC_API request, bool SendMail, bool Send2FA = true) : base(request, new Login_VLD_Create1FA())
        {
            sendMail = SendMail;
            send2FA = Send2FA;
        }

        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        override protected bool HasPrivilege()
        {
            return true;
        }

        protected override bool Execute()
        {
            //Validate against ReCAPTCHA

            if (!ReCAPTCHA.Validate(DTO.Captcha, ApiServicesHelper.ApiConfiguration.Settings, Log.Instance))
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            //get the user from the token while checking the token is still valid
            // generate a new token and new timeout
            //update TD_LOGIN with the hashed password, the new token and the new timeout
            bool success = false;

            Login_BSO lBso = new Login_BSO(Ado);

            var userdata = lBso.ReadByToken1Fa(DTO.LgnToken1Fa, DTO.CcnUsername);
            if (!userdata.hasData)
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            
            string newToken = Utility.GetSHA256(new Random().Next() + userdata.data[0].CcnId.ToString() + DateTime.Now.Millisecond);

            DTO.CcnEmail = userdata.data[0].CcnEmail;
            DTO.CcnUsername = userdata.data[0].CcnUsername;

            //Not allowed for AD users
            ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();
            ActiveDirectory_DTO adDto = adAdo.GetUser(Ado, DTO);
            if (adDto.CcnDisplayName != null)
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            if (lBso.Update1FA(DTO, newToken))
            {
                DTO.LgnToken1Fa = newToken;

                lBso.UpdateInvitationToken2Fa(DTO.CcnUsername, newToken);

                if (sendMail)
                {
                    SendEmail(new Login_DTO_Create() { CcnUsername = DTO.CcnUsername, LngIsoCode = DTO.LngIsoCode, CcnEmail = DTO.CcnEmail, CcnDisplayname = userdata.data[0].CcnDisplayName }, newToken, "PxStat.Security.Login_API.Create2FA");
                }
                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                success = true;
            }
            else
            {
                Response.error = Label.Get("error.create");
                success = false;

            }

            return success;

        }
        private void SendEmail(Login_DTO_Create lDto, string token, string nextMethod)
        {

            string url = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "url.application") + "?method=" + nextMethod + "&email=" + lDto.CcnEmail + '&' + "name=" + Uri.EscapeUriString(lDto.CcnDisplayname) + '&' + "token=" + token;
            string link = "<a href = " + url + ">" + Label.Get("email.body.header.anchor-text", lDto.LngIsoCode) + "</a>";
            string subject = string.Format(Label.Get("email.subject.setup-2fa", lDto.LngIsoCode), Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "title"));
            string to = lDto.CcnEmail;
            string header = string.Format(Label.Get("email.body.header.setup-2fa", lDto.LngIsoCode), lDto.CcnDisplayname, Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "title"));
            string subHeader = string.Format(Label.Get("email.body.sub-header.setup-2fa"), link);
            string footer = string.Format(Label.Get("email.body.footer", lDto.LngIsoCode), lDto.CcnDisplayname);

            Email_BSO.SendLoginTemplateEmail(subject, new List<string>() { to }, header, url, footer, subHeader, lDto.LngIsoCode);
        }
    }
}
