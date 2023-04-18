using API;
using PxStat.Template;

namespace PxStat.Security
{
    internal class Login_BSO_ReadByToken : BaseTemplate_Read<Login_DTO_Create2FA, Login_VLD_Create2FA>
    {
        internal Login_BSO_ReadByToken(JSONRPC_API request) : base(request, new Login_VLD_Create2FA())
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
            using (Login_BSO lBso = new Login_BSO())
            {
                var response = lBso.ReadByToken2Fa(DTO.LgnToken2Fa, DTO.CcnUsername);
                if (response.hasData)
                {
                    Response.data = response.data;
                    return true;
                }
                return false;
            }
        }

    }
}
