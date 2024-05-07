using API;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Creates a Product
    /// </summary>
    internal class Product_BSO_Create : BaseTemplate_Create<Product_DTO, Product_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Product_BSO_Create(JSONRPC_API request) : base(request, new Product_VLD_Create())
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
            var adoProduct = new Product_ADO(Ado);

            //You can only create a product in the default Language
            DTO.LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

            //Duplicate product names aren't allowed, so we check first
            if (adoProduct.Exists(DTO.PrcValue, DTO.SbjCode) || adoProduct.ExistsCode(DTO.PrcCode))
            {
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //Create the Product - and retrieve the newly created Id
            int newId = adoProduct.Create(DTO, SamAccountName);
            if (newId == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }
            //We must create the search keywords for this new product
            Keyword_Product_BSO_Mandatory keyBso = new Keyword_Product_BSO_Mandatory();
            keyBso.Create(Ado, DTO, newId);

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];

            return true;
        }
    }
}
