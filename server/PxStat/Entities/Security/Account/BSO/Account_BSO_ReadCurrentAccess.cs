using API;
using PxStat.Template;

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
            DTO.CcnUsername = SamAccountName;
            if (!IsUserAuthenticated())
            {
                return true;
            }
            JSONRPC_Output response = new JSONRPC_Output();
            //The cache key is created to be unique to a given CcnUsername.
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Security", "Account_API", "ReadCurrentAccesss", DTO.CcnUsername);
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }


            Account_BSO bso = new Account_BSO();
            ADO_readerOutput output = bso.ReadCurrentAccess(Ado, DTO.CcnUsername);
            if (!output.hasData)
            {
                Log.Instance.Debug("No Account data found");
                return false;
            }
            Response.data = output.data;
            return true;
        }

    }
}
