using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Creates a GroupAccount
    /// </summary>
    internal class GroupAccount_BSO_Create : BaseTemplate_Create<GroupAccount_DTO_Create, GroupAccount_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal GroupAccount_BSO_Create(JSONRPC_API request) : base(request, new GroupAccount_VLD_Create())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoGroupAccount = new GroupAccount_ADO();

            //Power users or Administrators may not be group members
            if (IsPowerUser(DTO.CcnUsername) || IsAdministrator(DTO.CcnUsername))
            {
                Log.Instance.Debug("Power users or Administrators may not be group members");
                Response.error = Label.Get("error.create");
                return false;
            }

            //Check if the user is in Active Directory
            if (!ActiveDirectory_BSO_Read.IsInActiveDirectory(Ado, DTO.CcnUsername))
            {
                Log.Instance.Debug("User is not in Active Directory");
                Response.error = Label.Get("error.create");
                return false;
            }

            //First we must check if the GroupAccount exists already (we can't have duplicates)
            if (adoGroupAccount.Exists(Ado, DTO.CcnUsername, DTO.GrpCode))
            {
                //This GroupAccount exists already, we can't proceed
                Log.Instance.Debug("GroupAccount exists already - create request refused");
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //Create the GroupAccount - and retrieve the newly created Id
            int newId = adoGroupAccount.Create(Ado, DTO, SamAccountName);
            if (newId == 0)
            {
                Log.Instance.Debug("Can't create Group Account");
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}

