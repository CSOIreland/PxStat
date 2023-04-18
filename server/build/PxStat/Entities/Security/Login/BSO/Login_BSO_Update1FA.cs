using API;
using PxStat.Template;

namespace PxStat.Security
{
    internal class Login_BSO_Update1FA : BaseTemplate_Update<Login_DTO_Update1FA, Login_VLD_Update1FA>
    {

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Login_BSO_Update1FA(JSONRPC_API request) : base(request, new Login_VLD_Update1FA())
        {
        }

        protected override bool HasPrivilege()
        {
            return true;
        }

        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        protected override bool Execute()
        {
            ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();

            dynamic adDto = adAdo.GetAdSpecificDataForEmail(DTO.CcnEmail);

            if (adDto?.CcnUsername != null)
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            if (!ReCAPTCHA.Validate(DTO.Captcha))
            {
                Response.error = Label.Get("error.authentication");
                return false;
            }

            Login_BSO lBso = new Login_BSO(Ado);

            var userResponse = lBso.ReadByToken1Fa(DTO.LgnToken1Fa, DTO.CcnEmail);
            if (userResponse.hasData)
            {

                string user = userResponse.data[0].CcnUsername;

                DTO.CcnUsername = user;

                if (lBso.Update1FA(new Login_DTO_Create1FA() { LgnToken1Fa = DTO.LgnToken1Fa, Lgn1Fa = DTO.Lgn1Fa, CcnEmail = DTO.CcnEmail }, DTO.LgnToken1Fa))
                {
                    Response.data = JSONRPC.success;
                    return true;
                }


            }
            Response.error = Label.Get("error.authentication");
            return false;
        }


    }
}
