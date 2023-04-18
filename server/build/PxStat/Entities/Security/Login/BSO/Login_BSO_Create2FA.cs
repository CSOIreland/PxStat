using API;
using PxStat.Template;

namespace PxStat.Security
{
    internal class Login_BSO_Create2FA : BaseTemplate_Create<Login_DTO_Create2FA, Login_VLD_Create2FA>
    {
        internal Login_BSO_Create2FA(JSONRPC_API request) : base(request, new Login_VLD_Create2FA())
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
            if (!ReCAPTCHA.Validate(DTO.Captcha))
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }
            Login_BSO lBso = new Login_BSO(Ado);
            Account_ADO aAdo = new Account_ADO();


            ADO_readerOutput responseUser = aAdo.Read(Ado, DTO.CcnEmail);

            //If this is an AD user using their email as an identifier then we must get their details from AD 
            if (!responseUser.hasData)
            {

                ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();

                var adResult = adAdo.GetAdSpecificDataForEmail(DTO.CcnEmail);


                if (adResult == null)
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
                //Check if AD local access is allowed
                if (!Configuration_BSO.GetCustomConfig(ConfigType.global, "security.adOpenAccess") && adResult != null)
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }

                DTO.CcnUsername = adResult.CcnUsername;
            }
            else
                DTO.CcnUsername = responseUser.data[0].CcnUsername;


            var response = lBso.Update2FA(DTO);
            if (response != null)
            {
                Response.data = response;

                return true;
            }
            Response.error = Label.Get("error.authentication");
            return false;

        }
    }
}
