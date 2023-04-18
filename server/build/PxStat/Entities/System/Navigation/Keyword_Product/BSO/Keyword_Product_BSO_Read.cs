using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Reads a Keyword Product
    /// </summary>
    internal class Keyword_Product_BSO_Read : BaseTemplate_Read<Keyword_Product_DTO, Keyword_Product_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Keyword_Product_BSO_Read(JSONRPC_API request) : base(request, new Keyword_Product_VLD_Read())
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
            var list = adoKeyword_Product.Read(DTO);
            Response.data = list;

            return true;
        }
    }
}

