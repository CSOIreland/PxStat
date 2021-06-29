using API;
using PxStat.Template;

namespace PxStat.Data
{
    internal class GeoLayer_BSO_Update : BaseTemplate_Update<GeoLayer_DTO_Update, GeoLayer_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public GeoLayer_BSO_Update(JSONRPC_API request) : base(request, new GeoLayer_VLD_Update())
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

            //Check if the GeoLayer exists
            if (bso.Read(Ado, DTO.GlrCode).Count == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }

            if (gAdo.Update(DTO, SamAccountName) == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }
            Response.data = JSONRPC.success;
            return true;
        }
    }
}
