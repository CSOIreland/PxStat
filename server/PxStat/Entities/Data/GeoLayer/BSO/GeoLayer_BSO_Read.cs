using API;
using PxStat.Template;

namespace PxStat.Data
{
    internal class GeoLayer_BSO_Read : BaseTemplate_Read<GeoLayer_DTO_Read, GeoLayer_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal GeoLayer_BSO_Read(JSONRPC_API request) : base(request, new GeoLayer_VLD_Read())
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

            GeoLayer_BSO bso = new GeoLayer_BSO();
            var list = bso.Read(Ado, DTO);
            Response.data = list;

            return true;
        }
    }
}
