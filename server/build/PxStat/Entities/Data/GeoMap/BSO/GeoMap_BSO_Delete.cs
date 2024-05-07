using API;
using PxStat.Template;

namespace PxStat.Data
{
    internal class GeoMap_BSO_Delete : BaseTemplate_Delete<GeoMap_DTO_Delete, GeoMap_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal GeoMap_BSO_Delete(JSONRPC_API request) : base(request, new GeoMap_VLD_Delete())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            GeoMap_ADO gAdo = new GeoMap_ADO(Ado);


            if (gAdo.Delete(DTO, SamAccountName) == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }
           AppServicesHelper.CacheD.Remove_BSO<dynamic>("PxStat.Data", "GeoMap_BSO_Read", "Read", DTO.GmpCode);
            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}
