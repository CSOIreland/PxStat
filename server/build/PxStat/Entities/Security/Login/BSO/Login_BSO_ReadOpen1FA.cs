using API;
using PxStat.Template;

namespace PxStat.Security
{
    internal class Login_BSO_ReadOpen1FA : BaseTemplate_Read<Login_DTO_ReadOpen1FA, Login_VLD_ReadOpen1FA>
    {
        internal Login_BSO_ReadOpen1FA(JSONRPC_API request) : base(request, new Login_VLD_ReadOpen1FA())
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
            Login_ADO lAdo = new Login_ADO(Ado);
            if (lAdo.ReadOpen1Fa(DTO.CcnEmail))
            {
                Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
                return true;
            }
            return false;
        }

    }
}
