using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Read the current user
    /// </summary>
    internal class Account_BSO_ReadCurrentAccess : BaseTemplate_Read<Account_DTO_Read, Account_VLD_ReadCurrent>
    {
        AuthenticationType authenticationType;
        /// <summary>
        /// This method reads only the current logged in user without the Group data
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_ReadCurrentAccess(JSONRPC_API request, AuthenticationType authType) : base(request, new Account_VLD_ReadCurrent())
        {
            authenticationType = authType;
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

            if (IsUserAuthenticated())
            {
                if (AuthenticationType != authenticationType)
                {
                    return false;
                }
            }

            
            DTO.CcnUsername = SamAccountName;

            Log.Instance.Debug("ReadCurrentAccess SamAccountName:" + DTO.CcnUsername);

            if (DTO.CcnUsername == null)
            {
                Response.data = null;
                return true;
            }



            Account_BSO bso = new Account_BSO(Ado);
            ADO_readerOutput output = bso.ReadCurrentAccess(Ado, DTO.CcnUsername);
            if (!output.hasData)
            {
                Log.Instance.Debug("No Account data found");
                return false;
            }

            Response.data = output.data[0];
            return true;
        }

    }
}
