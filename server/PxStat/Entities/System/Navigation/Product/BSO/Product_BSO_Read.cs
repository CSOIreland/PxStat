using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Reads Products
    /// </summary>
    internal class Product_BSO_Read : BaseTemplate_Read<Product_DTO, Product_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Product_BSO_Read(JSONRPC_API request) : base(request, new Product_VLD_Read())
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
        ///  Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            if (DTO.LngIsoCode == null)
                DTO.LngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");

            var adoProduct = new Product_ADO(Ado);
            var list = adoProduct.Read(DTO);
            Response.data = list;

            return true;
        }
    }
}

