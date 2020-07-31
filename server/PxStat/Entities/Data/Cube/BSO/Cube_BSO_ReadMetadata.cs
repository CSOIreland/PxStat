using API;
using Newtonsoft.Json.Linq;
using PxStat.Template;
using System;
using static PxStat.System.Settings.Format_DTO_Read;

namespace PxStat.Data
{
    /// <summary>
    /// Read the Cube metadata for a live release
    /// </summary>
    internal class Cube_BSO_ReadMetadata : BaseTemplate_Read<Cube_DTO_Read, Cube_VLD_ReadMetadata>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadMetadata(JSONRPC_API request) : base(request, new Cube_VLD_ReadMetadata())
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

            if (DTO.Format.FrmDirection != FormatDirection.DOWNLOAD.ToString())
            {
                return false;
            }

            ////See if this request has cached data
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadMetadata", DTO);
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
            //The Language of the received data may be different from the request - so we make sure it corresponds to the language of the metadata
            DTO.language = items.LngIsoCode;
            return ExecuteReadMetadata(Ado, DTO, result, Response);
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="theAdo"></param>
        /// <param name="theCubeDTO"></param>
        /// <param name="theReleaseDto"></param>
        /// <param name="theResponse"></param>
        /// <returns></returns>
        static internal bool ExecuteReadMetadata(ADO theAdo, Cube_DTO_Read theCubeDTO, Release_DTO theReleaseDto, JSONRPC_Output theResponse)
        {
            var ado = new Cube_ADO(theAdo);

            // The matrix constructor will load all the metadata from the db when instances specification
            var theMatrix = new Matrix(theAdo, theReleaseDto, theCubeDTO.language).ApplySearchCriteria(theCubeDTO);
            theMatrix.FormatType = theCubeDTO.Format.FrmType;
            theMatrix.FormatVersion = theCubeDTO.Format.FrmVersion;
            if (theMatrix == null)
            {
                theResponse.data = null;
                return true;
            }

            var jsonStat = theMatrix.GetJsonStatObject();
            theResponse.data = new JRaw(Serialize.ToJson(jsonStat));

            if (theReleaseDto.RlsLiveDatetimeTo != null)
                MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadMetadata", theCubeDTO, theResponse.data, theReleaseDto.RlsLiveDatetimeTo, Resources.Constants.C_CAS_DATA_CUBE_READ_METADATA + theMatrix.Code);
            else
                MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadMetadata", theCubeDTO, theResponse.data, new DateTime(), Resources.Constants.C_CAS_DATA_CUBE_READ_METADATA + theMatrix.Code);

            return true;
        }
    }
}
