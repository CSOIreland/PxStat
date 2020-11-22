using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    ///  Get a list of Matrix codes based on assignments of Copyrights to tables
    /// </summary>
    internal class Matrix_BSO_ReadByCopyright : BaseTemplate_Read<Matrix_DTO_ReadByCopyright, Matrix_VLD_ReadByCopyright>
    {
        internal Matrix_BSO_ReadByCopyright(JSONRPC_API request) : base(request, new Matrix_VLD_ReadByCopyright())
        {
        }
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }


        protected override bool Execute()
        {
            var adoMatrix = new Matrix_ADO(Ado);

            var list = adoMatrix.ReadByCopyright(DTO.CprCode, DTO.LngIsoCode);
            if (list.hasData)
                Response.data = list.data;

            return true;
        }


    }

}
