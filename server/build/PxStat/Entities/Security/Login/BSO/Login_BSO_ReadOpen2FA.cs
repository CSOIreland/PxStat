using API;
using PxStat.Template;

namespace PxStat.Security
{
    internal class Login_BSO_ReadOpen2FA : BaseTemplate_Read<Login_DTO_ReadOpen2FA, Login_VLD_ReadOpen2FA>
    {
        internal Login_BSO_ReadOpen2FA(JSONRPC_API request) : base(request, new Login_VLD_ReadOpen2FA())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
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
            Login_BSO lBso = new Login_BSO(Ado);

            Account_ADO aAdo = new Account_ADO();

            ADO_readerOutput response = aAdo.Read(Ado, DTO.CcnEmail);



            if (!response.hasData)
            {

                ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();
                //adAdo.MergeAdToUsers(ref result);

                var adResult = adAdo.GetAdSpecificDataForEmail(DTO.CcnEmail);


                ////Email address not in the login table, try to get the username from the email address via AD
                //PrincipalContext context = new PrincipalContext(ContextType.Domain);
                //user = UserPrincipal.FindByIdentity(context, DTO.CcnEmail).Name;

                if (adResult == null)
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }


                DTO.CcnUsername = adResult.CcnUsername;
            }
            else
                DTO.CcnUsername = response.data[0].CcnUsername;

            Login_ADO lAdo = new Login_ADO(Ado);

            if (lAdo.ReadOpen2Fa(DTO.CcnUsername))
            {
                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;
            }
            return false;
        }

    }
}
