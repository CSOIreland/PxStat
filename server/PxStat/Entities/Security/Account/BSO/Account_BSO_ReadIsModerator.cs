using API;
using PxStat.Template;


namespace PxStat.Security
{
    /// <summary>
    /// Tests if a user is a moderator
    /// </summary>
    internal class Account_BSO_ReadIsModerator : BaseTemplate_Read<Account_DTO_Read, Account_VLD_Read>
    {
        /// <summary>
        /// Returns true or false to indicate whether or not the user is a moderator
        /// If a CcnUsername is supplied, the method returns the indicator for that user
        /// If no CcnUsername is supplied the test is done on the user principal
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_ReadIsModerator(JSONRPC_API request) : base(request, new Account_VLD_Read())
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

            if (string.IsNullOrEmpty(DTO.CcnUsername))
            {

                Response.data = IsModerator(SamAccountName);
            }
            else
                Response.data = IsModerator(DTO.CcnUsername);


            return Response.data != null;

        }

    }
}