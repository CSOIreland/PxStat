using API;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Updates a product
    /// </summary>
    class Product_BSO_Update : BaseTemplate_Update<Product_DTO, Product_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Product_BSO_Update(JSONRPC_API request) : base(request, new Product_VLD_Update())
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

            int nUpdatedProductID = 0;

            //Check if the product exists

            if (!adoProduct.ExistsCode(DTO.PrcCode))
            {
                Response.error = Label.Get("error.update");
                return false;

            }

            //Duplicate product names aren't allowed, so we check first
            if (adoProduct.Exists(DTO.PrcValue, DTO.SbjCode) || (adoProduct.ExistsCode(DTO.PrcCodeNew) && DTO.PrcCode != DTO.PrcCodeNew))
            {
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //We must  delete all of the mandatory product keywords for the product 
            Keyword_Product_BSO_Mandatory kpBso = new Keyword_Product_BSO_Mandatory();
            int nchanged = kpBso.Delete(Ado, DTO, true);
            if (nchanged == 0)
            {
                Log.Instance.Debug("Delete of keywords failed");

            }

            //Update and retrieve the number of updated rows
            if (DTO.LngIsoCode != Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code"))
            {
                ProductLangauge_BSO productLanguageBso = new ProductLangauge_BSO();
                nUpdatedProductID = productLanguageBso.CreateOrUpdate(DTO, Ado);

                if (nUpdatedProductID == 0)
                {
                    Log.Instance.Debug("Update of ProductLanguage failed");
                    Response.error = Label.Get("error.update");
                    return false;
                }
            }



            if (DTO.PrcCodeNew == null) DTO.PrcCodeNew = DTO.PrcCode;

            bool IsDefault = DTO.LngIsoCode == Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
            nUpdatedProductID = adoProduct.Update(DTO, IsDefault, SamAccountName);

            if (nUpdatedProductID == 0)
            {
                Log.Instance.Debug("Update of Product failed");
                Response.error = Label.Get("error.update");
                return false;
            }



            //Finally we must recreate the mandatory keywords in line with the updated product
            kpBso.Create(Ado, DTO, nUpdatedProductID);

            Response.data = JSONRPC.success;

            return true;
        }
    }
}
