using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Tests if a user is a registered user on the system
    /// </summary>
    internal class Account_BSO_ReadIsRegistered : BaseTemplate_Read<Account_DTO_Read, Account_VLD_Read>
    {
        /// <summary>
        /// Returns true or false to indicate whether or not the user is registered on the application
        /// If a CcnUsername is supplied, the method returns the indicator for that user
        /// If no CcnUsername is supplied the test is done on the user principal
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_ReadIsRegistered(JSONRPC_API request) : base(request, new Account_VLD_Read())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Account_BSO acBso = new Account_BSO();
            if (string.IsNullOrEmpty(DTO.CcnUsername))
            {
                Response.data = acBso.IsRegistered(Ado, SamAccountName);
            }
            else
                Response.data = acBso.IsRegistered(Ado, DTO.CcnUsername);


            return Response.data != null;

        }
    }
}
