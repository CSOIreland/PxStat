using System;
using API;
using Newtonsoft.Json.Linq;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Reads a dataset for a live release
    /// </summary>
    internal class Cube_BSO_ReadDataset : BaseTemplate_Read<Cube_DTO_Read, Cube_VLD_ReadDataset>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadDataset(JSONRPC_API request) : base(request, new Cube_VLD_ReadDataset())
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

            ////See if this request has cached data
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", DTO);
            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }

            var items = new Release_ADO(Ado).ReadLiveNow(DTO.matrix, DTO.language);
            var result = Release_ADO.GetReleaseDTO(items);
            if (result == null)
            {
                Response.data = null;
                return true;
            }
            //The Language of the received data may be different from the request - so we make sure it corresponds to the language of the dataset
            DTO.language = items.LngIsoCode;
            var data = ExecuteReadDataset(Ado, DTO, result, Response);
            return data;
        }

        /// <summary>
        /// Run the detailed processes for this Execute
        /// </summary>
        /// <param name="theAdo"></param>
        /// <param name="theDto"></param>
        /// <param name="releaseDto"></param>
        /// <param name="theResponse"></param>
        /// <returns></returns>
        internal static bool ExecuteReadDataset(ADO theAdo, Cube_DTO_Read theDto, Release_DTO releaseDto, JSONRPC_Output theResponse)
        {
            var theMatrix = new Matrix(theAdo, releaseDto, theDto.language).ApplySearchCriteria(theDto);
            if (theMatrix == null)
            {
                theResponse.data = null;
                return true;
            }

            var matrix = new Cube_ADO(theAdo).ReadCubeData(theMatrix);
            if (matrix == null)
            {
                theResponse.data = null;
                return true;
            }

            switch (theDto.format)
            {
                case DatasetFormat.JsonStat:
                    var jsonStat = matrix.GetJsonStatObject();
                    theResponse.data = new JRaw(Serialize.ToJson(jsonStat));
                    break;
                case DatasetFormat.Csv:
                    theResponse.data = matrix.GetCSVObject().ToString();
                    break;
                case DatasetFormat.Px:
                    theResponse.data = matrix.GetPxObject().ToString();
                    break;
            }

            if (releaseDto.RlsLiveDatetimeTo != null)
                MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", theDto, theResponse.data, releaseDto.RlsLiveDatetimeTo, Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + matrix.Code);
            else
                MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", theDto, theResponse.data, new DateTime(), Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + matrix.Code);
            return true;
        }
    }
}
