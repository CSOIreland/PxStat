using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Deletes a Group Account
    /// </summary>
    internal class GroupAccount_BSO_Delete : BaseTemplate_Delete<GroupAccount_DTO_Delete, GroupAccount_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal GroupAccount_BSO_Delete(JSONRPC_API request) : base(request, new GroupAccount_VLD_Delete()) { }

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
            var adoGroupAccount = new GroupAccount_ADO();

            //attempting to delete. The number of entities deleted are passed to the entitiesDeleted variable (this is 1 for a successful delete)
            int nDeleted = adoGroupAccount.Delete(Ado, DTO, SamAccountName);
            if (nDeleted == 0)
            {
                Log.Instance.Debug("Can't delete Group Account");
                Response.error = Label.Get("error.delete");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}