using API;
using PxStat.Template;
using System.Dynamic;

namespace PxStat.Data
{
    internal class Matrix_BSO_ReadByGeoMap : BaseTemplate_Read<Matrix_DTO_ReadByGeoMap, Matrix_VLD_ReadByGeoMap>
    {
        internal Matrix_BSO_ReadByGeoMap(JSONRPC_API request) : base(request, new Matrix_VLD_ReadByGeoMap())
        {
        }
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsAdministrator();
        }


        protected override bool Execute()
        {
            var adoMatrix = new Matrix_ADO(Ado);

            var list = adoMatrix.ReadByGeoMap(DTO.GmpCode, DTO.LngIsoCode);
            if (list.hasData)
                Response.data = list.data;

            return true;
        }


    }

}
