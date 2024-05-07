using API;
using PxStat.System.Notification;
using PxStat.Template;
using System;
using System.Collections.Generic;

namespace PxStat.Security
{
    /// <summary>
    /// Create an account
    /// </summary>
    internal class Account_BSO_CreateAD : BaseTemplate_Create<Account_DTO_Create, Account_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_CreateAD(JSONRPC_API request) : base(request, new Account_VLD_Create())
        {
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
            //A power user may not create an Administrator
            if (IsPowerUser() && DTO.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_ADMINISTRATOR))
            {
                Log.Instance.Debug("A power user may not create an Administrator");
                Response.error = Label.Get("error.privilege");
                return false;
            }

            //We need to check if the requested user is in Active Directory, otherwise we refuse the request.
            ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();

            ActiveDirectory_DTO adDto = adAdo.GetUser(Ado, DTO);

            if (adDto.CcnUsername == null)
            {
                Log.Instance.Debug("AD user not found");
                Response.error = Label.Get("error.create");
                return false;
            }

            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoAccount = new Account_ADO();

            //First we must check if the Account exists already (we can't have duplicates)
            if (adoAccount.Exists(Ado, DTO.CcnUsername))
            {
                //This Account exists already, we can't proceed
                Log.Instance.Debug("Account exists already");
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //Create the Account - and retrieve the newly created Id
            int newId = adoAccount.Create(Ado, DTO, SamAccountName, true);
            if (newId == 0)
            {
                Log.Instance.Debug("adoAccount.Create - can't create Account");
                Response.error = Label.Get("error.create");
                return false;
            }
            string token = Utility.GetSHA256(new Random().Next() + newId.ToString() + DateTime.Now.Millisecond);
            Login_BSO lBso = new Login_BSO(Ado);
            lBso.CreateLogin(new Login_DTO_Create() { CcnUsername = DTO.CcnUsername }, SamAccountName, null);

            //Check if local access is available for AD users
            if (Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "security.adOpenAccess"))
            {
                lBso.UpdateInvitationToken2Fa(DTO.CcnUsername, token);

                SendEmail(new Login_DTO_Create() { CcnDisplayname = adDto.CcnDisplayName, CcnEmail = adDto.CcnEmail, CcnUsername = DTO.CcnUsername, LngIsoCode = DTO.LngIsoCode }, token, "PxStat.Security.Login_API.Create2FA");
            }

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
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
