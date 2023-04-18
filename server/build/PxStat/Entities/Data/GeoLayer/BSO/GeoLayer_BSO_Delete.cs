using API;
using PxStat.Template;

namespace PxStat.Data
{
    internal class GeoLayer_BSO_Delete : BaseTemplate_Delete<GeoLayer_DTO_Delete, GeoLayer_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public GeoLayer_BSO_Delete(JSONRPC_API request) : base(request, new GeoLayer_VLD_Delete())
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

            var read = bso.Read(Ado, DTO.GlrCode);
            if (read?.Count == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }

            //We can't delete a record that is already associated with a map
            if (read[0].GmpCount > 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }

            if (gAdo.Delete(DTO, SamAccountName) == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }
            Response.data = JSONRPC.success;
            return true;
        }
    }

}
