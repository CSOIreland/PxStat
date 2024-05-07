using API;
using PxStat.Template;


namespace PxStat.Security
{
    /// <summary>
    /// Allows the current user to update their own account. Currently only the NotificationFlag can be updated.
    /// </summary>
    internal class Account_BSO_UpdateCurrent : BaseTemplate_Update<Account_DTO_Update, Account_VLD_UpdateCurrent>
    {
        internal Account_BSO_UpdateCurrent(JSONRPC_API request) : base(request, new Account_VLD_UpdateCurrent())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            DTO.CcnUsername = SamAccountName;
            Account_ADO accountAdo = new Account_ADO();

            Account_DTO_Read dtoRead = new Account_DTO_Read();
            dtoRead.CcnUsername = SamAccountName;

            var readUser = accountAdo.Read(Ado, dtoRead);

            if (readUser.hasData)
            {
                Account_DTO_Update account = accountAdo.ReadAccounts(readUser)[0];
                DTO.CcnLockedFlag = account.CcnLockedFlag;
                if ((bool)DTO.CcnLockedFlag)
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }

                DTO.PrvCode = account.PrvCode;
                int nUpdated = accountAdo.Update(Ado, DTO, SamAccountName);

                if (nUpdated == 0)
                {
                    Log.Instance.Debug("Failed to update Account");
                    Response.error = Label.Get("error.update");
                    return false;
                }
            }
            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }

    }
}
