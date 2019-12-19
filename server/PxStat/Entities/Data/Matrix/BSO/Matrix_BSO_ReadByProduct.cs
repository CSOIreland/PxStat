using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    ///  Get a list of Matrix codes based on usage of products
    /// </summary>
    internal class Matrix_BSO_ReadByProduct : BaseTemplate_Read<Matrix_DTO_ReadByProduct, Matrix_VLD_ReadByProduct>
    {
        internal Matrix_BSO_ReadByProduct(JSONRPC_API request) : base(request, new Matrix_VLD_ReadByProduct())
        {
        }
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }


        protected override bool Execute()
        {
            var adoMatrix = new Matrix_ADO(Ado);

            var list = adoMatrix.ReadByProduct(DTO.PrcCode, DTO.LngIsoCode);
            if (list.hasData)
                Response.data = list.data;

            return true;
        }


    }

}
