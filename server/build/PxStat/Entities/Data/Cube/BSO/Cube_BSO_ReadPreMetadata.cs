using API;
using PxStat.Template;
using static PxStat.System.Settings.Format_DTO_Read;

namespace PxStat.Data
{
    /// <summary>
    /// Read the cube metadata for a pending release
    /// </summary>
    internal class Cube_BSO_ReadPreMetadata : BaseTemplate_Read<Cube_DTO_ReadMetadata, Cube_VLD_ReadPreDataset>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadPreMetadata(JSONRPC_API request) : base(request, new Cube_VLD_ReadPreMetadata())
        {

        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            if (DTO.Format.FrmDirection != FormatDirection.DOWNLOAD.ToString())
            {
                return false;
            }

            ////See if this request has cached data
            MemCachedD_Value cache =AppServicesHelper.CacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadPreMetadata", DTO);

            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }

            var item = new Matrix_ADO(Ado).Read(DTO.release, DTO.language, SamAccountName);
            var result = Release_ADO.GetReleaseDTO(item);
            if (result == null)
            {
                Response.data = null;
                return true;
            }

            DTO.language = item.LngIsoCode;
            return Cube_BSO_ReadMetadata.ExecuteReadMetadata(Ado, DTO, result, Response, SamAccountName, false);
        }
    }
}
