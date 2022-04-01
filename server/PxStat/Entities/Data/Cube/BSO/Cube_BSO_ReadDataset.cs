using API;
using Newtonsoft.Json.Linq;
using PxStat.JsonStatSchema;
using PxStat.Security;
using PxStat.Template;
using System;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PxStat.Data
{
    /// <summary>
    /// Reads a dataset for a live release
    /// </summary>
    internal class Cube_BSO_ReadDataset : BaseTemplate_Read<CubeQuery_DTO, JsonStatQueryLive_VLD>
    {
        internal bool defaultPivot = false;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadDataset(JSONRPC_API request, bool defaultRequestPivot = false) : base(request, new JsonStatQueryLive_VLD())
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

            if (DTO.jStatQueryExtension.extension.Pivot != null)
            {
                if (DTO.jStatQueryExtension.extension.Format.Type != "CSV" && DTO.jStatQueryExtension.extension.Format.Type != "XLSX")
                    DTO.jStatQueryExtension.extension.Pivot = null;
            }
            //if the role details haven't been supplied then look it up from the metadata in the database
            if (DTO.Role == null)
                DTO.Role = new Cube_BSO().UpdateRoleFromMetadata(Ado, DTO);

            //The Language of the received data may be different from the request - so we make sure it corresponds to the language of the dataset (???)
            var items = new Release_ADO(Ado).ReadLiveNow(DTO.jStatQueryExtension.extension.Matrix, DTO.jStatQueryExtension.extension.Language.Code);

            ////See if this request has cached data
            MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", DTO);

            if (cache.hasData)
            {
                Response.data = cache.data;
                return true;
            }


            if (Throttle_BSO.IsThrottled(Ado, HttpContext.Current.Request, Request, SamAccountName))
            {
                Log.Instance.Debug("Request throttled");
                Response.error = Label.Get("error.throttled");
            }

            var result = Release_ADO.GetReleaseDTO(items);
            if (result == null)
            {
                Response.data = null;
                return true;
            }

            var data = ExecuteReadDataset(Ado, DTO, result, Response, DTO.jStatQueryExtension.extension.Language.Code, DTO.jStatQueryExtension.extension.Language.Culture, defaultPivot);
            return data;

        }

        internal static bool ExecuteReadDataset(ADO theAdo, CubeQuery_DTO theDto, Release_DTO releaseDto, JSONRPC_Output theResponse, string requestLanguage, string culture = null, bool defaultPivot = false)
        {

            var theMatrix = new Matrix(theAdo, releaseDto, theDto.jStatQueryExtension.extension.Language.Code).ApplySearchCriteria(theDto);
            if (theMatrix == null)
            {
                theResponse.data = null;
                return true;
            }

            if (theDto.jStatQueryExtension.extension.Format.Type.Equals(Resources.Constants.C_SYSTEM_XLSX_NAME))
            {
                int dataSize = theMatrix.MainSpec.GetDataSize();
                int maxSize = Configuration_BSO.GetCustomConfig(ConfigType.global, "dataset.download.threshold.xlsx");
                if (dataSize > maxSize)
                {
                    theResponse.error = String.Format(Label.Get("error.dataset.limit", requestLanguage), dataSize, Resources.Constants.C_SYSTEM_XLSX_NAME, maxSize);
                    return false;
                }
            }
            else if (theDto.jStatQueryExtension.extension.Format.Type.Equals(Resources.Constants.C_SYSTEM_CSV_NAME))
            {
                int dataSize = theMatrix.MainSpec.GetDataSize();
                int maxSize = Configuration_BSO.GetCustomConfig(ConfigType.global, "dataset.download.threshold.csv");
                if (dataSize > maxSize)
                {
                    theResponse.error = String.Format(Label.Get("error.dataset.limit", requestLanguage), dataSize, Resources.Constants.C_SYSTEM_CSV_NAME, maxSize);
                    return false;
                }
            }


            var matrix = new Cube_ADO(theAdo).ReadCubeData(theMatrix);
            if (matrix == null)
            {
                theResponse.data = null;
                return true;
            }


            if (!string.IsNullOrEmpty(theDto.jStatQueryExtension.extension.Pivot))
            {
                if (theMatrix.MainSpec.Classification.Where(x => x.Code == theDto.jStatQueryExtension.extension.Pivot).Count() == 0
                && Utility.GetCustomConfig("APP_CSV_STATISTIC") != theDto.jStatQueryExtension.extension.Pivot
                    && theMatrix.MainSpec.Frequency.Code != theDto.jStatQueryExtension.extension.Pivot)
                {
                    theResponse.error = Label.Get("error.validation", theDto.jStatQueryExtension.extension.Language.Code);
                    return false;
                }

            }
            else theDto.jStatQueryExtension.extension.Pivot = null;

            CultureInfo readCulture;

            if (culture == null)
                readCulture = CultureInfo.CurrentCulture;
            else
                readCulture = CultureInfo.CreateSpecificCulture(culture); ;

            if (theDto.jStatQueryExtension.extension.Pivot == null && defaultPivot && theDto.jStatQueryExtension.extension.Format.Type == DatasetFormat.Csv)
            {
                theDto.jStatQueryExtension.extension.Pivot = matrix.MainSpec.Frequency.Code;
            }


            switch (theDto.jStatQueryExtension.extension.Format.Type)
            {
                case DatasetFormat.JsonStat:
                    matrix.FormatType = theDto.jStatQueryExtension.extension.Format.Type;
                    matrix.FormatVersion = theDto.jStatQueryExtension.extension.Format.Version;

                    if (theDto.jStatQueryExtension.extension.Format.Version == "1.0")
                    {
                        var jsonStat = matrix.GetJsonStatV1Object(true, null);
                        theResponse.data = new JRaw(SerializeJsonStatV1.ToJson(jsonStat));
                    }
                    else if (theDto.jStatQueryExtension.extension.Format.Version == "1.1")
                    {
                        var jsonStat = matrix.GetJsonStatV1_1Object(false, true, null, readCulture);
                        theResponse.data = new JRaw(SerializeJsonStatV1_1.ToJson(jsonStat));

                    }
                    else
                    {
                        var jsonStat = matrix.GetJsonStatObject(false, true, null, readCulture);
                        theResponse.data = new JRaw(Serialize.ToJson(jsonStat));
                    }
                    break;
                case DatasetFormat.Csv:
                    theResponse.data = matrix.GetCsvObject(requestLanguage, false, readCulture, theDto.jStatQueryExtension.extension.Pivot, theDto.jStatQueryExtension.extension.Codes == null ? true : (bool)theDto.jStatQueryExtension.extension.Codes);
                    break;
                case DatasetFormat.Px:
                    theResponse.data = matrix.GetPxObject();
                    break;
                case DatasetFormat.Xlsx:
                    theResponse.data = matrix.GetXlsxObject(requestLanguage, CultureInfo.CurrentCulture, theDto.jStatQueryExtension.extension.Pivot, theDto.jStatQueryExtension.extension.Codes == null ? true : (bool)theDto.jStatQueryExtension.extension.Codes);
                    break;

                case DatasetFormat.Sdmx:
                    theResponse.data = matrix.GetSdmx(theDto.jStatQueryExtension.extension.Language.Code);
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
