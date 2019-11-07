
using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Create an account
    /// </summary>
    internal class Account_BSO_Create : BaseTemplate_Create<Account_DTO_Create, Account_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Account_BSO_Create(JSONRPC_API request) : base(request, new Account_VLD_Create())
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
            //A power user may not create an Administrator
            if (IsPowerUser() && DTO.PrvCode.Equals(Resources.Constants.C_SECURITY_PRIVILEGE_ADMINISTRATOR))
            {
                Log.Instance.Debug("A power user may not create an Administrator");
                Response.error = Label.Get("error.privilege");
                return false;
            }

            //We need to check if the requested user is in Active Directory, otherwise we refuse the request.
            ActiveDirectory_ADO adAdo = new ActiveDirectory_ADO();

            ActiveDirectory_DTO adDto = adAdo.GetUser(Ado, DTO);
            if (adDto.CcnUsername == null)
            {
                Log.Instance.Debug("AD user not found");
                Response.error = Label.Get("error.create");
                return false;
            }


            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoAccount = new Account_ADO();

            //First we must check if the Account exists already (we can't have duplicates)
            if (adoAccount.Exists(Ado, DTO.CcnUsername))
            {
                //This Account exists already, we can't proceed
                Log.Instance.Debug("Account exists already");
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //Create the Account - and retrieve the newly created Id
            int newId = adoAccount.Create(Ado, DTO, SamAccountName);
            if (newId == 0)
            {
                Log.Instance.Debug("adoAccount.Create - can't crete Account");
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}

