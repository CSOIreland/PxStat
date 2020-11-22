using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    ///  Get a list of Matrix codes based on assignments of Languages to tables
    /// </summary>
    internal class Matrix_BSO_ReadByLanguage : BaseTemplate_Read<Matrix_DTO_ReadByLanguage, Matrix_VLD_ReadByLanguage>
    {
        internal Matrix_BSO_ReadByLanguage(JSONRPC_API request) : base(request, new Matrix_VLD_ReadByLanguage())
        {
        }
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }


        protected override bool Execute()
        {
            var adoMatrix = new Matrix_ADO(Ado);

            var list = adoMatrix.ReadByLanguage(DTO.LngIsoCode);
            if (list.hasData)
                Response.data = list.data;

            return true;
        }


    }

}
