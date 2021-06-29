using API;
using PxStat.System.Notification;
using PxStat.Template;
using System;
using System.Collections.Generic;

namespace PxStat.Security
{
    internal class Account_BSO_CreateLocal : BaseTemplate_Create<Account_DTO_CreateLocal, Account_VLD_CreateLocal>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_CreateLocal(JSONRPC_API request) : base(request, new Account_VLD_CreateLocal())
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

            //We need to check if the requested user is NOT in Active Directory, otherwise we refuse the request.
            ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();

            ActiveDirectory_DTO adDto = adAdo.GetUser(Ado, DTO);

            if (adDto.CcnUsername != null)
            {
                Log.Instance.Debug("Account exists already");
                Response.error = Label.Get("error.create");
                return false;
            }

            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoAccount = new Account_ADO();

            //First we must check if the Account exists already (we can't have duplicates)
            if (adoAccount.Exists(Ado, DTO.CcnEmail))
            {
                //This Account exists already, we can't proceed
                Log.Instance.Debug("Account exists already");
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //Next check if the email exists
            if (adoAccount.ExistsByEmail(Ado, DTO.CcnEmail))
            {
                //This Account exists already, we can't proceed
                Log.Instance.Debug("Account exists already");
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //make sure this email isn't an AD email - they should not become local users
            var aduser = adAdo.GetAdSpecificDataForEmail(DTO.CcnEmail);
            if(aduser!=null)
            {
                //This Account exists in AD, we can't proceed
                Log.Instance.Debug("Account exists in AD");
                Response.error = Label.Get("error.create");
                return false;
            }


            //Create the Account - and retrieve the newly created Id
            int newId = adoAccount.Create(Ado, new Account_DTO_Create() { CcnUsername = DTO.CcnUsername, CcnNotificationFlag = DTO.CcnNotificationFlag, LngIsoCode = DTO.LngIsoCode, PrvCode = DTO.PrvCode, CcnDisplayName = DTO.CcnDisplayName, CcnEmail = DTO.CcnEmail }, SamAccountName, false);
            if (newId == 0)
            {
                Log.Instance.Debug("adoAccount.Create - can't create Account");
                Response.error = Label.Get("error.create");
                return false;
            }

            Login_DTO_Create lDto = new Login_DTO_Create() { CcnUsername = DTO.CcnEmail, LngIsoCode = DTO.LngIsoCode, CcnEmail = DTO.CcnEmail, CcnDisplayname = DTO.CcnDisplayName };

            Login_BSO lBso = new Login_BSO(Ado);

            string token = Utility.GetRandomSHA256(newId.ToString());


            if (lBso.CreateLogin(lDto, SamAccountName, token))
                SendEmail(lDto, token, "PxStat.Security.Login_API.Create1FA");
            else
            {
                Response.error = Label.Get("error.create");
                return false;
            }


            Response.data = JSONRPC.success;
            return true;

        }


        private void SendEmail(Login_DTO_Create lDto, string token, string nextMethod)
        {

            string url = Configuration_BSO.GetCustomConfig(ConfigType.global, "url.application") + "?method=" + nextMethod + "&email=" + lDto.CcnEmail + '&' + "name=" + Uri.EscapeUriString(lDto.CcnDisplayname) + '&' + "token=" + token;
            string link = "<a href = " + url + ">" + Label.Get("email.body.header.anchor-text", lDto.LngIsoCode) + "</a>";
            string subject = string.Format(Label.Get("email.subject.setup-1fa", lDto.LngIsoCode), Configuration_BSO.GetCustomConfig(ConfigType.global, "title"));
            string to = lDto.CcnEmail;
            string header = string.Format(Label.Get("email.body.header.setup-1fa", lDto.LngIsoCode), lDto.CcnDisplayname, Configuration_BSO.GetCustomConfig(ConfigType.global, "title"));
            string subHeader = string.Format(Label.Get("email.body.sub-header.setup-1fa"), link);
            string footer = string.Format(Label.Get("email.body.footer", lDto.LngIsoCode), lDto.CcnDisplayname);

            Email_BSO.SendLoginTemplateEmail(subject, new List<string>() { to }, header, url, footer, subHeader, lDto.LngIsoCode);
        }
    }
}
