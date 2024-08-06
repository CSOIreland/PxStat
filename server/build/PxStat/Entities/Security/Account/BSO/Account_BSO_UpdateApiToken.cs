using API;
using PxStat.Template;
using System;
using System.Configuration;

namespace PxStat.Security
{
    internal class Account_BSO_UpdateApiToken : BaseTemplate_Update<Account_DTO_UpdateToken, Account_VLD_UpdateApiToken>
    {
        AuthenticationType authenticationType;
        /// <summary>
        /// This method reads only the current logged in user without the Group data
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_UpdateApiToken(JSONRPC_API request) : base(request, new Account_VLD_UpdateApiToken())
        {
          
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


        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            string newToken= Utility.GetSHA256(new Random().Next() + Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "salsa.firebase") + DateTime.Now.Millisecond);
            Account_ADO account_ADO = new();
            int count = account_ADO.UpdateCcnToken(Ado, newToken, SamAccountName);
            if(count == 0)
            {
                return false;
            }
            Response.data = newToken;
            return true;
            
        }

    }


}
