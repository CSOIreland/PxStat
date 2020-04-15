using API;
using PxStat.JsonStatSchema;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Read the dataset for a cube that has not yet been released
    /// </summary>
    internal class Cube_BSO_ReadPreDataset : BaseTemplate_Read<CubeQuery_DTO, JsonStatQueryWip_VLD>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadPreDataset(JSONRPC_API request) : base(request, new JsonStatQueryWip_VLD())
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {


            var item = new Matrix_ADO(Ado).Read(DTO.jStatQueryExtension.extension.RlsCode, DTO.jStatQueryExtension.extension.Language.Code, SamAccountName);
            var result = Release_ADO.GetReleaseDTO(item);
            if (result == null)
            {
                Response.data = null;
                return true;
            }

            DTO.jStatQueryExtension.extension.Matrix = result.MtrCode;
            //if the role details haven't been supplied then look it up from the metadata in the database
            if (DTO.Role == null)
                DTO.Role = new Cube_BSO().UpdateRoleFromMetadata(Ado, DTO);

            //The returned data may not be in the same language as the preferred language, so we must change the DTO.language property
            // DTO.language = item.LngIsoCode;
            return Cube_BSO_ReadDataset.ExecuteReadDataset(Ado, DTO, result, Response, DTO.jStatQueryExtension.extension.Language.Code, DTO.jStatQueryExtension.extension.Language.Culture);
        }
    }
}
