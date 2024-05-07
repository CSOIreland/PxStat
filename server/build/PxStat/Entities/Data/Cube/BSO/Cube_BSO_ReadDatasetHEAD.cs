using API;
using PxStat.DataStore;
using PxStat.JsonStatSchema;
using PxStat.System.Settings;
using PxStat.Template;
using System.Diagnostics;
using System.Net;

namespace PxStat.Data
{
    /// <summary>
    /// Reads a dataset for a live release
    /// </summary>
    internal class Cube_BSO_ReadDatasetHEAD : BaseTemplate_Read<CubeQuery_DTO, JsonStatQueryLiveHEAD_VLD>
    {
        internal bool defaultPivot = false;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadDatasetHEAD(IRequest request, bool defaultRequestPivot = false) : base(request, new JsonStatQueryLiveHEAD_VLD())
        {
            defaultPivot = defaultRequestPivot;
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
            if (DTO.jStatQueryExtension.extension.Format != null)
            {
                Format_DTO_Read format = new Format_DTO_Read() { FrmType = DTO.jStatQueryExtension.extension.Format.Type, FrmVersion = DTO.jStatQueryExtension.extension.Format.Version, FrmDirection = Format_DTO_Read.FormatDirection.DOWNLOAD.ToString() };
                using (IADO fAdo = AppServicesHelper.StaticADO)
                {
                    Format_ADO fa = new Format_ADO();
                    if (!fa.Read(fAdo, format).hasData)
                    {
                        Response.statusCode = HttpStatusCode.NotFound;
                        Response.mimeType = null;
                        return false;
                    }
                    using (Format_BSO fbso = new Format_BSO(AppServicesHelper.StaticADO))
                    {
                        Response.mimeType = fbso.GetMimetypeForFormat(format);
                    };
                }
            }

            using (IADO ado = AppServicesHelper.StaticADO)
            {
                DataStore_ADO dAdo = new DataStore_ADO();
                if (!(dAdo.DataMatrixReadLive(ado, DTO.jStatQueryExtension.extension.Matrix, DTO.jStatQueryExtension.extension.Language.Code).hasData))
                {
                    Response.statusCode = HttpStatusCode.NotFound;
                    Response.mimeType = null;
                    return false;
                }
            }


            Response.statusCode = HttpStatusCode.OK;
            Response.response = "";
            return true;
        }

    }
}
