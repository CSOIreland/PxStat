using API;
using PxStat.DataStore;
using PxStat.Security;
using PxStat.Template;
using System.Net;

namespace PxStat.Data
{
    /// <summary>
    /// Read the Cube metadata for a live release
    /// </summary>
    internal class Cube_BSO_ReadMetadataHEAD : BaseTemplate_Read<Cube_DTO_ReadMatrixMetadata, Cube_VLD_ReadMetadataHEAD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadMetadataHEAD(IRequest request) : base(request, new Cube_VLD_ReadMetadataHEAD())
        {

        }

        /// <summary>
        /// Test authentication
        /// </summary>
        /// <returns></returns>
        protected override bool HasUserToBeAuthenticated()
        {
            return false;
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

            using (IADO ado = AppServicesHelper.StaticADO)
            {
                DataStore_ADO dAdo = new DataStore_ADO();
                if (!(dAdo.DataMatrixReadLive(ado, DTO.matrix, DTO.language).hasData))
                {
                    Response.statusCode = HttpStatusCode.NotFound;
                    Response.mimeType = null;
                    return false;
                }
            }


            Response.mimeType = Configuration_BSO.GetStaticConfig("APP_JSON_MIMETYPE");
            Response.statusCode = HttpStatusCode.OK;
            return true;
        }
    }
}
