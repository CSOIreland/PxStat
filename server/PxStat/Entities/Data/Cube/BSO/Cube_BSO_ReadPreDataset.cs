using API;
using PxStat.JsonStatSchema;
using PxStat.Template;
using System;

namespace PxStat.Data
{
    /// <summary>
    /// Read the dataset for a cube that has not yet been released
    /// </summary>
    internal class Cube_BSO_ReadPreDataset : BaseTemplate_Read<CubeQuery_DTO, JsonStatPreQuery_VLD>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadPreDataset(JSONRPC_API request) : base(request, new JsonStatPreQuery_VLD())
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
            var release = Release_ADO.GetReleaseDTO(item);
            if (release == null)
            {
                Response.data = null;
                return true;
            }
            //if the role details haven't been supplied then look it up from the metadata in the database
            if (DTO.Role == null)
                DTO.Role = new Cube_BSO().UpdateRoleFromMetadata(Ado, DTO);

            DTO.jStatQueryExtension.extension.Matrix = release.MtrCode;

            ////See if this request has cached data
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadPreDataset", DTO);

            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }

            ////See if this request has cached data





            //The returned data may not be in the same language as the preferred language, so we must change the DTO.language property
            // DTO.language = item.LngIsoCode;
            //return Cube_BSO_ReadDataset.ExecuteReadDataset(Ado, DTO, release, Response, DTO.jStatQueryExtension.extension.Language.Code, DTO.jStatQueryExtension.extension.Language.Culture);

            IMetaData metaData = new MetaData();
            var hasRun = Cube_BSO_ReadDataset.ExecuteReadDatasetDmatrix(Ado, metaData, DTO, release, Response, DTO.jStatQueryExtension.extension.Language.Code, DTO.jStatQueryExtension.extension.Language.Culture, false, false);



            MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadPreDataset", DTO, Response.data, new DateTime(), Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET + DTO.jStatQueryExtension.extension.RlsCode);

            return hasRun;
        }
    }
}
