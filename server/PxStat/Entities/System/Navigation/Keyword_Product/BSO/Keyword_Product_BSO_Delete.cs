using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Deletes a Keyword Product
    /// </summary>
    class Keyword_Product_BSO_Delete : BaseTemplate_Delete<Keyword_Product_DTO, Keyword_Product_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Keyword_Product_BSO_Delete(JSONRPC_API request) : base(request, new Keyword_Product_VLD_Delete())
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
            var adoKeyword_Product = new Keyword_Product_ADO(Ado);

            //attempting to delete. The number of entities deleted are passed to the entitiesDeleted variable (this is 1 for a successful delete)
            int nDeleted = adoKeyword_Product.Delete(DTO, false);
            Log.Instance.Debug("Delete operation finished in ADO");

            if (nDeleted == 0)
            {
                Log.Instance.Debug("No record found for delete request");
                Response.error = Label.Get("error.delete");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}
