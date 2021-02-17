using API;
using PxStat.System.Notification;
using PxStat.Template;
using System;
using System.Collections.Generic;

namespace PxStat.Security
{
    internal class Login_BSO_InitiateUpdate1FA : BaseTemplate_Update<Login_DTO_InitiateUpdate1Fa, Login_VLD_InitiateUpdate1Fa>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Login_BSO_InitiateUpdate1FA(JSONRPC_API request) : base(request, new Login_VLD_InitiateUpdate1Fa())
        {
        }


        override protected bool HasPrivilege()
        {
            return true;
        }

        protected override bool Execute()
        {

            ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();

            ActiveDirectory_DTO adDto = adAdo.GetAdSpecificDataForEmail(DTO.CcnEmail);

            if (adDto?.CcnUsername != null)
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            Login_BSO lBso = new Login_BSO(Ado);

            Account_ADO aado = new Account_ADO();
            var user = aado.Read(Ado, DTO.CcnEmail);
            if (!user.hasData)
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }
            string token = Utility.GetRandomSHA256(user.data[0].CcnId.ToString());

            if (lBso.Update1FaTokenForUser(user.data[0].CcnUsername, token) != null)
            {
                SendEmail(new Login_DTO_Create() { CcnUsername = user.data[0].CcnUsername, LngIsoCode = DTO.LngIsoCode, CcnEmail = user.data[0].CcnEmail, CcnDisplayname = user.data[0].CcnDisplayName }, token, "PxStat.Security.Login_API.Update1FA");
                Response.data = JSONRPC.success;
                return true;
            }

            Response.error = Label.Get("error.authentication");
            return false;
        }

        private void SendEmail(Login_DTO_Create lDto, string token, string nextMethod)
        {

            string url = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.application") + "?method=" + nextMethod + "&email=" + lDto.CcnEmail + '&' + "name=" + Uri.EscapeUriString(lDto.CcnDisplayname) + '&' + "token=" + token;
            string link = "<a href = " + url + ">" + Label.Get("email.body.header.anchor-text", lDto.LngIsoCode) + "</a>";
            string subject = string.Format(Label.Get("email.subject.update-1fa", lDto.LngIsoCode), Configuration_BSO.GetCustomConfig(ConfigType.global, "title"));
            string to = lDto.CcnEmail;
            string header = string.Format(Label.Get("email.body.header.update-1fa", lDto.LngIsoCode), lDto.CcnDisplayname, Configuration_BSO.GetCustomConfig(ConfigType.global, "title"));
            string subHeader = string.Format(Label.Get("email.body.sub-header.update-1fa"), link);
            string footer = string.Format(Label.Get("email.body.footer", lDto.LngIsoCode), lDto.CcnDisplayname);

            Email_BSO.SendLoginTemplateEmail(subject, new List<string>() { to }, header, url, footer, subHeader, lDto.LngIsoCode);
        }

    }
}
