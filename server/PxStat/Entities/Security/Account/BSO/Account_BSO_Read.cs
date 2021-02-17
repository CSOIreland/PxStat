using API;
using PxStat.Template;
using System.Linq;

namespace PxStat.Security
{
    /// <summary>
    /// Read an account
    /// </summary>
    internal class Account_BSO_Read : BaseTemplate_Read<Account_DTO_Read, Account_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_Read(JSONRPC_API request) : base(request, new Account_VLD_Read())
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

            //Accounts are returned as an ADO result
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

        /// <summary>
        /// Checks if the user is registered on the system
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>


        internal static bool IsRegistered(ADO ado, string ccnUsername)
        {
            Account_ADO accountAdo = new Account_ADO();
            Account_DTO_Read dto = new Account_DTO_Read();
            dto.CcnUsername = ccnUsername;
            ADO_readerOutput output = accountAdo.Read(ado, dto);
            return output.hasData;
        }



        /// <summary>
        /// Checks if the user is an administrator - based on the ccnUsername parameter
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal static bool IsAdministrator(ADO ado, string ccnUsername)
        {
            if (ccnUsername == null) return false;
            Account_ADO accountAdo = new Account_ADO();
            Account_DTO_Read dto = new Account_DTO_Read();
            dto.CcnUsername = ccnUsername;
            ADO_readerOutput output = accountAdo.Read(ado, dto);


            if (!output.hasData)
                return false;
            else
            {
                dynamic account = output.data.First();
                return (account.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_ADMINISTRATOR));
            }
        }

        /// <summary>
        /// Checks if the user is a power user - based on the username parameter
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal static bool IsPowerUser(ADO ado, string ccnUsername)
        {
            Account_ADO accountAdo = new Account_ADO();
            Account_DTO_Read dto = new Account_DTO_Read();
            dto.CcnUsername = ccnUsername;
            ADO_readerOutput output = accountAdo.Read(ado, dto);


            if (!output.hasData) return false;
            else
            {
                dynamic account = output.data.First();
                return (account.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_POWER_USER));
            }
        }
        /// <summary>
        /// Checks if the user is a Moderator - based on the username parameter
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal static bool IsModerator(ADO ado, string ccnUsername)
        {
            Account_ADO accountAdo = new Account_ADO();
            Account_DTO_Read dto = new Account_DTO_Read();
            dto.CcnUsername = ccnUsername;
            ADO_readerOutput output = accountAdo.Read(ado, dto);


            if (!output.hasData) return false;
            else
            {
                dynamic account = output.data.First();
                return (account.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_MODERATOR));
            }
        }
    }
}
