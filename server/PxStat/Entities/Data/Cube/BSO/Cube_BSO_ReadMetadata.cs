using API;
using Newtonsoft.Json.Linq;
using PxStat.DataStore;
using PxStat.Security;
using PxStat.Template;
using System;
using System.Net;
using System.Web;
using static PxStat.System.Settings.Format_DTO_Read;

namespace PxStat.Data
{
    /// <summary>
    /// Read the Cube metadata for a live release
    /// </summary>
    internal class Cube_BSO_ReadMetadata : BaseTemplate_Read<Cube_DTO_ReadMetadata, Cube_VLD_ReadMetadata>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadMetadata(IRequest request) : base(request, new Cube_VLD_ReadMetadata())
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

            // Get whitelist
            string[] whitelist = Configuration_BSO.GetCustomConfig(ConfigType.server, "whitelist");

            // Get host 
            var hRequest = HttpContext.Current.Request;
            string host = hRequest.Url.Host;

            // Check if host is not in the whitelist and the request is throttled
            if (Throttle_BSO.IsNotInTheWhitelist(whitelist, host) && Throttle_BSO.IsThrottled(Ado, HttpContext.Current.Request, Request, SamAccountName))
            {
                Log.Instance.Debug("Request throttled");
                Response.error = Label.Get("error.throttled");
            }

            var items = new Release_ADO(Ado).ReadLiveNow(DTO.matrix, DTO.language);

            var result = Release_ADO.GetReleaseDTO(items);
            if (result == null)
            {
                Response.statusCode = HttpStatusCode.NotFound;
                Response.data = null;
                return false;
            }
            //The Language of the received data may be different from the request - so we make sure it corresponds to the language of the metadata
            DTO.language = items.LngIsoCode;
            return ExecuteReadMetadata(Ado, DTO, result, Response, SamAccountName);
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="theAdo"></param>
        /// <param name="theCubeDTO"></param>
        /// <param name="theReleaseDto"></param>
        /// <param name="theResponse"></param>
        /// <returns></returns>
        static internal bool ExecuteReadMetadata(IADO theAdo, ICube_DTO_Read theCubeDTO, Release_DTO theReleaseDto, IResponseOutput theResponse, string ccnUsername, bool isLive = true)
        {
            // The matrix constructor will load all the metadata from the db when instances specification


            DataReader dr = new DataReader();
            IDmatrix matrix = null;
            if (isLive)
                matrix = dr.ReadLiveDataset(theCubeDTO.matrix, theCubeDTO.language, theReleaseDto, true);
            else
                matrix = dr.ReadNonLiveDataset(theCubeDTO.language, theReleaseDto, ccnUsername, true);


            matrix.FormatType = theCubeDTO.Format.FrmType;
            matrix.FormatVersion = theCubeDTO.Format.FrmVersion;
            matrix.Language = theCubeDTO.language;

            if (matrix == null)
            {
                theResponse.data = null;
                theResponse.statusCode = HttpStatusCode.NotFound;
                return false;
            }


            JsonStatBuilder2_0 jxb = new JsonStatBuilder2_0();
            var jsonStat = jxb.Create(matrix, matrix.Language, false);
            theResponse.data = new JRaw(Serialize.ToJson(jsonStat));

            if (isLive)
            {
                if (theReleaseDto.RlsLiveDatetimeTo != default)
                    MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadMetadata", theCubeDTO, theResponse.data, theReleaseDto.RlsLiveDatetimeTo, Resources.Constants.C_CAS_DATA_CUBE_READ_METADATA + matrix.Code);
                else
                    MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadMetadata", theCubeDTO, theResponse.data, new DateTime(), Resources.Constants.C_CAS_DATA_CUBE_READ_METADATA + matrix.Code);
            }
            else
            {
                if (theReleaseDto.RlsLiveDatetimeFrom > DateTime.Now)
                {
                    MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadPreMetadata", theCubeDTO, theResponse.data, theReleaseDto.RlsLiveDatetimeFrom, Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA + theReleaseDto.RlsCode);
                }
                else
                {
                    MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadPreMetadata", theCubeDTO, theResponse.data, default(DateTime), Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA + theReleaseDto.RlsCode);
                }
            }
            return true;
        }
    }
}
