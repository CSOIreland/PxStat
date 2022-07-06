using API;
using PxStat.Template;

namespace PxStat.Data
{
    internal class GeoMap_BSO_ReadCollection : BaseTemplate_Read<GeoMap_DTO_ReadCollection, GeoMap_VLD_ReadCollection>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal GeoMap_BSO_ReadCollection(JSONRPC_API request) : base(request, new GeoMap_VLD_ReadCollection())
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

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            GeoMap_ADO gAdo = new GeoMap_ADO(Ado);
            var response = gAdo.ReadCollection(DTO.GmpCode);
            if (response.hasData)
                Response.data = response.data;
            return true;
        }
    }
}
