using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Deletes a ReleaseProduct
    /// </summary>
    internal class ReleaseProductAssociation_BSO_Delete : BaseTemplate_Delete<ReleaseProductAssociation_DTO_Delete, ReleaseProductAssociation_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal ReleaseProductAssociation_BSO_Delete(JSONRPC_API request) : base(request, new ReleaseProductAssociation_VLD_Delete()) { }

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
            var adoReleaseProduct = new ReleaseProductAssocaition_ADO();

            // The number of entities deleted are passed to the entitiesDeleted variable (this is 1 for a successful delete)
            int nDeleted = adoReleaseProduct.Delete(Ado, DTO, SamAccountName);
            if (nDeleted == 0)
            {
                Log.Instance.Debug("Cannot delete Release Product association");
                Response.error = Label.Get("error.delete");
                return false;
            }

            //Flush the cache for search - it's now out of date
            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_NAVIGATION_SEARCH);

            Response.data = JSONRPC.success;
            return true;
        }
    }
}