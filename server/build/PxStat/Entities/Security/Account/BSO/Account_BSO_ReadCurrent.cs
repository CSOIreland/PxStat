using API;
using PxStat.Template;
namespace PxStat.Security
{
    /// <summary>
    /// Reads account details for the current user
    /// </summary>
    internal class Acccount_BSO_ReadCurrent : BaseTemplate_Read<Account_DTO_Read, Account_VLD_ReadCurrent>
    {
        /// <summary>
        /// This method reads only the current logged in user
        /// </summary>
        /// <param name="request"></param>
        internal Acccount_BSO_ReadCurrent(JSONRPC_API request) : base(request, new Account_VLD_ReadCurrent())
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
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoAccount = new Account_ADO();

            //Accounts are returned as an IADO result
            if (DTO.CcnUsername == null)
                DTO.CcnUsername = SamAccountName;
            ADO_readerOutput result = adoAccount.Read(Ado, DTO);

            //Merge the data with Active Directory data
            if (result.hasData)
            {
                ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();
                adAdo.MergeAdToUsers(ref result);

                if (!string.IsNullOrEmpty(DTO.CcnUsername))
                {
                    adAdo.MergeGroupsToUsers(Ado, ref result);
                }

                Response.data = result.data[0];
                return true;
            }

            Log.Instance.Debug("No Account data found");
            return false;
        }
    }
}
