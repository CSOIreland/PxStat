using API;
using PxStat.Resources;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Updates a product
    /// </summary>
    class Product_BSO_Update : BaseTemplate_Update<Product_DTO, Product_VLD_Update>
    {
        private Product_ADO adoProduct;
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
            adoProduct = new Product_ADO(Ado);

            int nUpdatedProductID = 0;

            //Check if the product exists

            if (!adoProduct.ExistsCode(DTO.PrcCode))
            {
                Response.error = Label.Get("error.update");
                return false;

            }

            //Duplicate product names aren't allowed, so we check first
            if (adoProduct.Exists(DTO.PrcValue, DTO.SbjCode)) //|| )
            {
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            if (DTO.PrcCodeNew != null)
            {
                if (adoProduct.ExistsCode(DTO.PrcCodeNew) && DTO.PrcCode != DTO.PrcCodeNew)
                {
                    Response.error = Label.Get("error.duplicate");
                    return false;
                }
            }

            //We must  delete all of the mandatory product keywords for the product 
            Keyword_Product_BSO_Mandatory kpBso = new Keyword_Product_BSO_Mandatory();
            int nchanged = kpBso.Delete(Ado, DTO, true);
            if (nchanged == 0)
            {
                Log.Instance.Debug("Delete of keywords failed");

            }

            //Update and retrieve the number of updated rows
            if (DTO.LngIsoCode != Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code"))
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

            bool IsDefault = DTO.LngIsoCode == Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            nUpdatedProductID = adoProduct.Update(DTO, IsDefault, SamAccountName);

            if (nUpdatedProductID == 0)
            {
                Log.Instance.Debug("Update of Product failed");
                Response.error = Label.Get("error.update");
                return false;
            }



            //Finally we must recreate the mandatory keywords in line with the updated product
            kpBso.Create(Ado, DTO, nUpdatedProductID);

            return true;
        }

        public override bool PostExecute()
        {
            //Flush the cache for search - it's now out of date
            Cas.RunCasFlush(Resources.Constants.C_CAS_NAVIGATION_SEARCH);
            Cas.RunCasFlush(Resources.Constants.C_CAS_NAVIGATION_READ);

            //Flush caches for each MtrCode affected by this change
            var mtrResult = adoProduct.GetMtrCodeListForProduct(DTO);
            if (mtrResult.hasData)
            {
                foreach (var mtrCode in mtrResult.data)
                {
                    Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + mtrCode.MtrCode);
                    Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_METADATA + mtrCode.MtrCode);
                }
            }

            //Flush caches for each RlsCode affected by this change
            var rlsResult = adoProduct.GetRlsCodeListForProduct(DTO);
            if (rlsResult.hasData)

            {
                foreach (var rlsCode in rlsResult.data)
                {
                    Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET + rlsCode.RlsCode);
                    Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA + rlsCode.RlsCode);
                }
            }

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];

            return true;
        }
    }
}
