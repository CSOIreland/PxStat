
using API;
using PxStat.Template;
using System.Collections.Generic;

namespace PxStat.Security
{
    /// <summary>
    /// Updates an account
    /// </summary>
    internal class Account_BSO_Update : BaseTemplate_Update<Account_DTO_Update, Account_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_Update(JSONRPC_API request) : base(request, new Account_VLD_Update())
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
            //A power user may not update a user to become an Administrator
            if (IsPowerUser() && DTO.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_ADMINISTRATOR))
            {
                Log.Instance.Debug("A power user may not update a user to become an Administrator");
                Response.error = Label.Get("error.privilege");
                return false;
            }

            //A power user may not downgrade an administrator 
            if (IsPowerUser() && IsAdministrator(DTO.CcnUsername) && !DTO.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_ADMINISTRATOR))
            {
                Log.Instance.Debug("A power user may not downgrade an administrator");
                Response.error = Label.Get("error.privilege");
                return false;
            }

            Account_ADO adoAccount = new Account_ADO();

            //There must always be at least one administrator in the system. If this delete would leave no administrator then the request must be refused.
            if (IsAdministrator(DTO.CcnUsername))
            {
                if (!adoAccount.EnoughPrivilegesInAccounts(Ado, Resources.Constants.C_SECURITY_PRIVILEGE_ADMINISTRATOR))
                {
                    Log.Instance.Debug("There are insufficient Administrators in the Account table to proceed with this update.");
                    Response.error = Label.Get("error.update");
                    return false;
                }
            }

            //Update and retrieve the number of updated rows
            int nUpdated = adoAccount.Update(Ado, DTO, SamAccountName);

            if (nUpdated == 0)
            {

                Log.Instance.Debug("Failed to update Account");
                Response.error = Label.Get("error.update");
                return false;
            }

            if (DTO.PrvCode != null)
            {
                //An administrator or power user may not be a member of a group. Therefore we will remove any group memberships for the updated user
                // We run the check based on the proposed PrvCode, not on the existing privilege
                if (DTO.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_ADMINISTRATOR) || DTO.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_POWER_USER))
                {
                    List<GroupAccount_DTO> groupAccountList = getGroupMembership(DTO.CcnUsername);

                    foreach (GroupAccount_DTO groupAccount in groupAccountList)
                    {
                        GroupAccount_ADO gaAdo = new GroupAccount_ADO();
                        GroupAccount_DTO_Delete gaDto = new GroupAccount_DTO_Delete();
                        gaDto.CcnUsername = groupAccount.CcnUsername;
                        gaDto.GrpCode = groupAccount.GrpCode;
                        int deleted = gaAdo.Delete(Ado, gaDto, SamAccountName);
                        if (deleted == 0)
                        {
                            Log.Instance.Debug("Failed to delete account group membership");
                            Response.error = Label.Get("error.update");
                            return false;
                        }
                    }
                }
            }
            //If this user is cached then we must remove it because the data is now out of date
            MemCacheD.Remove_BSO<dynamic>("PxStat.Security", "Account_API", "ReadCurrentAccesss", DTO.CcnUsername);
            Response.data = JSONRPC.success;
            return true;
        }


        /// <summary>
        /// Get a list of GroupAccounts for a CcnUsername
        /// </summary>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        private List<GroupAccount_DTO> getGroupMembership(string ccnUsername)
        {
            GroupAccount_ADO gaAdo = new GroupAccount_ADO();
            GroupAccount_DTO_Read dtoRead = new GroupAccount_DTO_Read();
            dtoRead.CcnUsername = ccnUsername;
            dynamic result = gaAdo.Read(Ado, dtoRead);
            List<GroupAccount_DTO> groupAccountList = new List<GroupAccount_DTO>();
            foreach (dynamic groupAccount in result.data)
            {
                GroupAccount_DTO resultDTO = new GroupAccount_DTO(groupAccount);
                groupAccountList.Add(resultDTO);
            }

            return groupAccountList;
        }
    }
}
