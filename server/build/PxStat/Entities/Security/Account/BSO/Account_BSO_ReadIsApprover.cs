using API;
using PxStat.Template;


namespace PxStat.Security
{
    /// <summary>
    /// Tests if a user is an approver
    /// </summary>
    internal class Account_BSO_ReadIsApprover : BaseTemplate_Read<Account_DTO_ReadIsApprover, Account_VLD_ReadIsApprover>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_ReadIsApprover(JSONRPC_API request) : base(request, new Account_VLD_ReadIsApprover())
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
            if (DTO.CcnUsername == null)
                DTO.CcnUsername = SamAccountName;

            Account_ADO adoAccount = new Account_ADO();
            ADO_readerOutput result = adoAccount.ReadReleaseApprovers(Ado, DTO);

            Response.data = result.hasData;

            return true;

        }
    }
}