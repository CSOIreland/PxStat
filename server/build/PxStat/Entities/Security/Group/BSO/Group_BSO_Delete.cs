using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Deletes a user group
    /// </summary>
    internal class Group_BSO_Delete : BaseTemplate_Delete<Group_DTO_Delete, Group_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Group_BSO_Delete(JSONRPC_API request) : base(request, new Group_VLD_Delete())
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
            var adoGroup = new Group_ADO();

            //Check if the Group is used in a related entity. If so, we can't proceed with the delete request
            if (adoGroup.IsInUse(Ado, DTO.GrpCode))
            {
                //The Group is in use by at least one related entity, we cannot proceed with the delete request
                Log.Instance.Debug("Delete request for Group Code: " + DTO.GrpCode + " refused because it is in use by at least one related entity");
                Response.error = Label.Get("error.delete");
                return false;
            }

            //attempting to delete. The number of entities deleted are passed to the entitiesDeleted variable (this is 1 for a successful delete)
            int nDeleted = adoGroup.Delete(Ado, DTO, SamAccountName);
            if (nDeleted == 0)
            {
                Log.Instance.Debug("Can't delete Group");
                Response.error = Label.Get("error.delete");
                return false;
            }
            Response.data = JSONRPC.success;
            return true;
        }
    }
}
