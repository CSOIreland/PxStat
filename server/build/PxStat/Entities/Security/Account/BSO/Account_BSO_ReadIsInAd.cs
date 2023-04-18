using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Tests if a user is in Active Directory
    /// </summary>
    internal class Account_BSO_ReadIsInAD : BaseTemplate_Read<Account_DTO_Read, Account_VLD_Read>
    {
        /// <summary>
        /// Returns true or false to indicate whether or not the user is on the domain Active Directory
        /// If a CcnUsername is supplied, the method returns the indicator for that user
        /// If no CcnUsername is supplied the test is done on the user principal
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_ReadIsInAD(JSONRPC_API request) : base(request, new Account_VLD_Read())
        {
        }

        /// <summary>
        /// Tests privilege
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

            if (string.IsNullOrEmpty(DTO.CcnUsername))
            {
                Response.data = ActiveDirectory_BSO_Read.IsInActiveDirectory(Ado, SamAccountName);
            }
            else
                Response.data = ActiveDirectory_BSO_Read.IsInActiveDirectory(Ado, DTO.CcnUsername);


            return Response.data != null;

        }
    }
}