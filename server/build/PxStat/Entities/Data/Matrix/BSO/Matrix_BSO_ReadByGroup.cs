using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    ///  Get a list of Matrix codes based on assignments of groups to tables
    /// </summary>
    internal class Matrix_BSO_ReadByGroup : BaseTemplate_Read<Matrix_DTO_ReadByGroup, Matrix_VLD_ReadByGroup>
    {
        internal Matrix_BSO_ReadByGroup(JSONRPC_API request) : base(request, new Matrix_VLD_ReadByGroup())
        {
        }
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }


        protected override bool Execute()
        {
            var adoMatrix = new Matrix_ADO(Ado);

            var list = adoMatrix.ReadByGroup(DTO.GrpCode, DTO.LngIsoCode);
            if (list.hasData)
                Response.data = list.data;

            return true;
        }


    }

}
