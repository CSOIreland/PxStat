using API;
using PxStat.Template;
using System;
using PxStat.Resources;

namespace PxStat.Security
{
    /// <summary>
    /// Read the current user
    /// </summary>
    internal class Account_BSO_ReadCurrentAccess : BaseTemplate_Read<Account_DTO_Read, Account_VLD_ReadCurrent>
    {
        /// <summary>
        /// This method reads only the current logged in user without the Group data
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_ReadCurrentAccess(JSONRPC_API request) : base(request, new Account_VLD_ReadCurrent())
        {
        }

        /// <summary>
        /// Test authentication
        /// </summary>
        /// <returns></returns>
        override protected bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        /// Test Privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        /// <summary>
        /// Anonymous Login identified
        /// </summary>
        override protected void OnAuthenticationFailed()
        {
            //Anonymous Login identified
            Response.data = null;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            if (!IsUserAuthenticated())
            {
                return true;
            }

            //Accounts are returned as an ADO result
            DTO.CcnUsername = SamAccountName;

            //The cache key is created to be unique to a given CcnUsername.
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Security", "Account_API", "ReadCurrentAccesss", DTO.CcnUsername);
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }

            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoAccount = new Account_ADO();

            ADO_readerOutput result = adoAccount.Read(Ado, DTO);
            if (result.hasData)
            {

                // Set the cache based on the data returned
                MemCacheD.Store_BSO<dynamic>("PxStat.Security", "Account_API", "ReadCurrentAccesss", DTO.CcnUsername, result.data, new DateTime());

                Response.data = result.data;
                return true;
            }

            Log.Instance.Debug("No Account data found");
            return false;
        }
    }
}
