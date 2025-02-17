using API;
using CSO.Recaptcha;
using PxStat.System.Notification;
using PxStat.Template;
using System;
using System.Collections.Generic;

namespace PxStat.Security
{
    internal class Login_BSO_InitiateForgotten1FA : BaseTemplate_Update<Login_DTO_InitiateForgotten1Fa, Login_VLD_Login_InitiateForgotten1Fa>
    {
        /// <param name="request"></param>
        internal Login_BSO_InitiateForgotten1FA(JSONRPC_API request) : base(request, new Login_VLD_Login_InitiateForgotten1Fa())
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
            if (!ReCAPTCHA.Validate(DTO.Captcha, ApiServicesHelper.ApiConfiguration.Settings, Log.Instance))
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            if (DTO.CcnUsername == null) DTO.CcnUsername = DTO.CcnEmail;

            //Not allowed for AD users
            ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();
            ActiveDirectory_DTO adDto = adAdo.GetUser(Ado, DTO);



            if (adDto.CcnDisplayName != null)
            {
                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;
            }

            Account_ADO ccnAdo = new Account_ADO();
            var user = ccnAdo.Read(Ado, new Account_DTO_Read() { CcnUsername = DTO.CcnEmail });
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

            DTO.CcnEmail = user.data[0].CcnEmail;

            Login_BSO lBso = new Login_BSO(Ado);
            
            string loginToken = Utility.GetSHA256(new Random().Next() + user.data[0].CcnId.ToString() + DateTime.Now.Millisecond);

            Login_DTO_Create ldto = new Login_DTO_Create() { CcnUsername = DTO.CcnEmail, LngIsoCode = DTO.LngIsoCode, CcnEmail = DTO.CcnEmail, CcnDisplayname = user.data[0].CcnDisplayName };

            if (lBso.Update1FaTokenForUser(DTO.CcnEmail, loginToken) != null)
            {

                SendEmail(new Login_DTO_Create() { CcnUsername = user.data[0].CcnUsername, LngIsoCode = DTO.LngIsoCode, CcnEmail = user.data[0].CcnEmail, CcnDisplayname = user.data[0].CcnDisplayName }, loginToken, "PxStat.Security.Login_API.Update1FA");

                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;
            }
            else
            {
                Response.error = Label.Get("error.create");
                return false;
            }


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
