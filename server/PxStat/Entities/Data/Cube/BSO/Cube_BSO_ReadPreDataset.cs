using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Read the dataset for a cube that has not yet been released
    /// </summary>
    internal class Cube_BSO_ReadPreDataset : BaseTemplate_Read<Cube_DTO_Read, Cube_VLD_ReadPreDataset>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadPreDataset(JSONRPC_API request) : base(request, new Cube_VLD_ReadPreDataset())
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
            var item = new Matrix_ADO(Ado).Read(DTO.release, DTO.language, SamAccountName);
            var result = Release_ADO.GetReleaseDTO(item);
            if (result == null)
            {
                Response.data = null;
                return true;
            }
            //The returned data may not be in the same language as the preferred language, so we must change the DTO.language property
            DTO.language = item.LngIsoCode;
            return Cube_BSO_ReadDataset.ExecuteReadDataset(Ado, DTO, result, Response);
        }
    }
}

