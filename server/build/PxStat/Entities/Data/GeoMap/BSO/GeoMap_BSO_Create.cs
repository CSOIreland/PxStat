using API;
using PxStat.Template;

namespace PxStat.Data
{
    internal class GeoMap_BSO_Create : BaseTemplate_Create<GeoMap_DTO_Create, GeoMap_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal GeoMap_BSO_Create(JSONRPC_API request) : base(request, new GeoMap_VLD_Create())
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
            int records = gAdo.Create(DTO, SamAccountName);
            if (records == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}
