using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Deletes a Product
    /// </summary>
    class Product_BSO_Delete : BaseTemplate_Delete<Product_DTO, Product_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Product_BSO_Delete(JSONRPC_API request) : base(request, new Product_VLD_Delete())
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
            var adoProduct = new Product_ADO(Ado);

            //attempting to delete. The number of entities deleted are passed to the entitiesDeleted variable (this is 1 for a successful delete)
            int nDeleted = adoProduct.Delete(DTO, SamAccountName);
            Log.Instance.Debug("Delete operation finished in ADO");

            if (nDeleted == 0)
            {
                Log.Instance.Debug("No record found for delete request");
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

