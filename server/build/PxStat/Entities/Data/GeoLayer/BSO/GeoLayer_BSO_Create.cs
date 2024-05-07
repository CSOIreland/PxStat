using API;
using PxStat.Template;

namespace PxStat.Data
{
    internal class GeoLayer_BSO_Create : BaseTemplate_Create<GeoLayer_DTO_Create, GeoLayer_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal GeoLayer_BSO_Create(JSONRPC_API request) : base(request, new GeoLayer_VLD_Create())
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
            GeoLayer_ADO gAdo = new GeoLayer_ADO(Ado);

            GeoLayer_BSO bso = new GeoLayer_BSO();



            //We can't have duplicate GlrName
            if (bso.Read(Ado, null, DTO.GlrName).Count > 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }

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
