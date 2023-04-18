using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Updates a GroupAccount
    /// </summary>
    internal class GroupAccount_BSO_Update : BaseTemplate_Create<GroupAccount_DTO_Update, GroupAccount_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal GroupAccount_BSO_Update(JSONRPC_API request) : base(request, new GroupAccount_VLD_Update())
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
            //Even though this is an Update request, it would qualify as suspicious behaviour
            if (IsPowerUser(DTO.CcnUsername) || IsAdministrator(DTO.CcnUsername))
            {
                Log.Instance.Debug("Power users or Administrators may not be group members");
                Response.error = Label.Get("error.update");
                return false;
            }

            //Create the GroupAccount - and retrieve the newly created Id
            int newId = adoGroupAccount.Update(Ado, DTO, SamAccountName);
            if (newId == 0)
            {
                Log.Instance.Debug("Can't update Group Account");
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}