using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Read an account
    /// </summary>
    public class Account_BSO_Read : BaseTemplate_Read<Account_DTO_Read, Account_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Account_BSO_Read(JSONRPC_API request) : base(request, new Account_VLD_Read())
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
            if(IsModerator() && !DTO.CcnUsername.Equals(SamAccountName))
            {
                Response.error = Label.Get("error.privilege");
                return false;
            }
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoAccount = new Account_ADO();

            //Accounts are returned as an IADO result
            ADO_readerOutput result = adoAccount.Read(Ado, DTO);


            //We need to merge the account data with the data from Active Directory
            if (result.hasData)
            {

                ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();
                adAdo.MergeAdToUsers(ref result);


                if (!string.IsNullOrEmpty(DTO.CcnUsername))
                {

                    adAdo.MergeGroupsToUsers(Ado, ref result);
                }

                Response.data = result.data;
                return true;
            }

            Log.Instance.Debug("Account not found");

            return false;
        }



    }
}
