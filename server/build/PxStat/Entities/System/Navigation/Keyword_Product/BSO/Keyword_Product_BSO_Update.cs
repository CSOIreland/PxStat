using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Updates a KeywordProduct
    /// </summary>
    class Keyword_Product_BSO_Update : BaseTemplate_Update<Keyword_Product_DTO, Keyword_Product_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Keyword_Product_BSO_Update(JSONRPC_API request) : base(request, new Keyword_Product_VLD_Update())
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

            //Update and retrieve the number of updated rows
            int nUpdated = adoKeyword_Product.Update(DTO);

            if (nUpdated == 0)
            {
                Log.Instance.Debug("No record found for update request");
                Response.error = Label.Get("error.update");
                return false;
            }
            else if (nUpdated < 0)
            {
                Response.error = Label.Get("error.duplicate");
            }

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}

