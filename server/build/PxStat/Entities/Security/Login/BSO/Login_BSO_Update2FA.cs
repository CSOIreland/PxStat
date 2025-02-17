using API;
using CSO.Recaptcha;
using PxStat.Template;
using System;

namespace PxStat.Security
{
    internal class Login_BSO_Update2FA : BaseTemplate_Update<Login_DTO_Update2FA, Login_VLD_Update2FA>
    {
        internal Login_BSO_Update2FA(JSONRPC_API request) : base(request, new Login_VLD_Update2FA())
        { }

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
            if (!ReCAPTCHA.Validate(DTO.Captcha, ApiServicesHelper.ApiConfiguration.Settings, Log.Instance))
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();
            ActiveDirectory_DTO adDto = adAdo.GetUser(Ado, DTO);

            dynamic adUser = adAdo.GetAdSpecificDataForEmail(DTO.CcnEmail);

            if (adUser?.CcnEmail != null)
            {
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
            Login_BSO lBso = new Login_BSO(Ado);
            ADO_readerOutput output = lBso.ReadByToken2Fa(DTO.LgnToken2Fa, DTO.CcnUsername);
            if (!output.hasData)
            {
                return false;
            }
            //create a 2fa, save it to the database, unlock the account and send the 2fa back to the client to be displayed as a QRCode

            string token = lBso.Update2FA(new Login_DTO_Create2FA() { LgnToken2Fa = DTO.LgnToken2Fa, CcnUsername = DTO.CcnUsername });

            Response.data = token;
            return true;
        }

    }
}
