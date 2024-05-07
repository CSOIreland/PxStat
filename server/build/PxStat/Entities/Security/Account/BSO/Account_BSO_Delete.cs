using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Delete an account
    /// </summary>
    public class Account_BSO_Delete : BaseTemplate_Delete<Account_DTO_Delete, Account_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Account_BSO_Delete(JSONRPC_API request) : base(request, new Account_VLD_Delete())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //A power user may not delete an Administrator
            if (IsPowerUser() && IsAdministrator(DTO.CcnUsername))
            {
                Log.Instance.Debug("A power user may not delete an Administrator");
                Response.error = Label.Get("error.privilege");
                return false;
            }

            //You can't delete yourself
            if (DTO.CcnUsername.Equals(SamAccountName))
            {
                Log.Instance.Debug("A user may not delete themselves");
                Response.error = Label.Get("error.delete");
                return false;
            }

            var adoAccount = new Account_ADO();

            //There must always be at least one administrator in the system. If this delete would leave no administrator then the request must be refused.
            if (IsAdministrator(DTO.CcnUsername))
            {
                if (!adoAccount.EnoughPrivilegesInAccounts(Ado, Resources.Constants.C_SECURITY_PRIVILEGE_ADMINISTRATOR))
                {
                    Log.Instance.Debug("There are insufficient Administrators in the Account table to proceed with this delete.");
                    Response.error = Label.Get("error.delete");
                    return false;
                }
            }

            //We also need to delete user membership of any groups
            GroupAccount_ADO gaAdo = new GroupAccount_ADO();
            GroupAccount_DTO_Read gaDto = new GroupAccount_DTO_Read();
            gaDto.CcnUsername = DTO.CcnUsername;
            ADO_readerOutput groupAccountList = gaAdo.Read(Ado, gaDto);
            if (groupAccountList.hasData)
            {
                foreach (dynamic res in groupAccountList.data)
                {
                    GroupAccount_DTO_Delete dtoDelete = new GroupAccount_DTO_Delete();
                    dtoDelete.CcnUsername = DTO.CcnUsername;
                    dtoDelete.GrpCode = res.GrpCode;
                    gaAdo.Delete(Ado, dtoDelete, SamAccountName);
                }
            }

            //attempting to delete. The number of entities deleted are passed to the entitiesDeleted variable (this is 1 for a successful delete)
            int nDeleted = adoAccount.Delete(Ado, DTO, SamAccountName);
            if (nDeleted == 0)
            {
                Log.Instance.Debug("adoAccount.Delete - can't delete Account");
                Response.error = Label.Get("error.delete");
                return false;
            }

            //If this user is cached then we must remove the cache entry as well
           AppServicesHelper.CacheD.Remove_BSO<dynamic>("PxStat.Security", "Account_API", "ReadCurrentAccesss", DTO.CcnUsername);

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}

