using API;
using Newtonsoft.Json.Linq;
using PxStat.JsonStatSchema;
using PxStat.Template;
using System;
using System.Diagnostics;
using System.Globalization;

namespace PxStat.Data
{
    /// <summary>
    /// Reads a dataset for a live release
    /// </summary>
    internal class Cube_BSO_ReadDataset : BaseTemplate_Read<CubeQuery_DTO, JsonStatQueryLive_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadDataset(JSONRPC_API request) : base(request, new JsonStatQueryLive_VLD())
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

            //if the role details haven't been supplied then look it up from the metadata in the database
            if (DTO.Role == null)
                DTO.Role = new Cube_BSO().UpdateRoleFromMetadata(Ado, DTO);

            //The Language of the received data may be different from the request - so we make sure it corresponds to the language of the dataset (???)
            var items = new Release_ADO(Ado).ReadLiveNow(DTO.jStatQueryExtension.extension.Matrix, DTO.jStatQueryExtension.extension.Language.Code);
            /* string requestLanguage = DTO.language;
             DTO.language = items.LngIsoCode; */


            ////See if this request has cached data
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", DTO);

            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }


            var result = Release_ADO.GetReleaseDTO(items);
            if (result == null)
            {
                Response.data = null;
                return true;
            }

            var data = ExecuteReadDataset(Ado, DTO, result, Response, DTO.jStatQueryExtension.extension.Language.Code, DTO.jStatQueryExtension.extension.Language.Culture);
            return data;

        }

        internal static bool ExecuteReadDataset(ADO theAdo, CubeQuery_DTO theDto, Release_DTO releaseDto, JSONRPC_Output theResponse, string requestLanguage, string culture = null)
        {


            var theMatrix = new Matrix(theAdo, releaseDto, theDto.jStatQueryExtension.extension.Language.Code).ApplySearchCriteria(theDto);
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

            CultureInfo readCulture;

            if (culture == null)
                readCulture = CultureInfo.CurrentCulture;
            else
                readCulture = CultureInfo.CreateSpecificCulture(culture); ;


            switch (theDto.jStatQueryExtension.extension.Format.Type)
            {
                case DatasetFormat.JsonStat:
                    var jsonStat = matrix.GetJsonStatObject(false, true, null, readCulture);
                    theResponse.data = new JRaw(Serialize.ToJson(jsonStat));
                    break;
                case DatasetFormat.Csv:
                    theResponse.data = matrix.GetCsvObject(requestLanguage, false, readCulture);
                    break;
                case DatasetFormat.Px:
                    theResponse.data = matrix.GetPxObject().ToString();
                    break;
                case DatasetFormat.Xlsx:
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    theResponse.data = matrix.GetXlsxObject(requestLanguage, CultureInfo.CurrentCulture);
                    sw.Stop();
                    long l = sw.ElapsedMilliseconds;
                    break;
            }

            if (releaseDto.RlsLiveDatetimeTo != default(DateTime))
                MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", theDto, theResponse.data, releaseDto.RlsLiveDatetimeTo, Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + matrix.Code);
            else
                MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", theDto, theResponse.data, new DateTime(), Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + matrix.Code);
            return true;

        }

        /// <summary>
        /// Run the detailed processes for this Execute
        /// </summary>
        /// <param name="theAdo"></param>
        /// <param name="theDto"></param>
        /// <param name="releaseDto"></param>
        /// <param name="theResponse"></param>
        /// <returns></returns>
        internal static bool ExecuteReadDataset(ADO theAdo, Cube_DTO_Read theDto, Release_DTO releaseDto, JSONRPC_Output theResponse, string requestLanguage)
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



            switch (theDto.Format.FrmType)
            {
                case DatasetFormat.JsonStat:
                    var jsonStat = matrix.GetJsonStatObject();
                    theResponse.data = new JRaw(Serialize.ToJson(jsonStat));
                    break;
                case DatasetFormat.Csv:
                    theResponse.data = matrix.GetCsvObject(requestLanguage);
                    break;
                case DatasetFormat.Px:
                    theResponse.data = matrix.GetPxObject().ToString();
                    break;
                case DatasetFormat.Xlsx:
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    theResponse.data = matrix.GetXlsxObject(requestLanguage);
                    sw.Stop();
                    long l = sw.ElapsedMilliseconds;
                    break;
            }

            if (releaseDto.RlsLiveDatetimeTo != default(DateTime))
                MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", theDto, theResponse.data, releaseDto.RlsLiveDatetimeTo, Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + matrix.Code);
            else
                MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", theDto, theResponse.data, new DateTime(), Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + matrix.Code);
            return true;

        }

    }
}
