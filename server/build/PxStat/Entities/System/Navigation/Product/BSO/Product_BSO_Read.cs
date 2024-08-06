using API;
using PxStat.Security;
using PxStat.Template;
using System.Linq;

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
        internal Product_BSO_Read(IRequest request) : base(request, new Product_VLD_Read())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        ///  Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            if (DTO.LngIsoCode == null)
                DTO.LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

            var adoProduct = new Product_ADO(Ado);
            var list = adoProduct.Read(DTO);
            Response.data = list.ToList().OrderBy(x=>x.PrcValue);

            return true;
        }
    }
}
