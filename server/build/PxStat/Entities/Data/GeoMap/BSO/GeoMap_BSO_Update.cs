using API;
using PxStat.Template;

namespace PxStat.Data
{
    internal class GeoMap_BSO_Update : BaseTemplate_Update<GeoMap_DTO_Update, GeoMap_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal GeoMap_BSO_Update(JSONRPC_API request) : base(request, new GeoMap_VLD_Update())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator () ;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {

            GeoMap_ADO gAdo = new GeoMap_ADO(Ado);
            int records = gAdo.Update(DTO, SamAccountName);
            if (records == 0)
            {
                Response.error = Label.Get("error.update");
                return false;
            }

           AppServicesHelper.CacheD.Remove_BSO<dynamic>("PxStat.Data", "GeoMap_RESTful", "Read", DTO.GmpCode);
;           Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}
