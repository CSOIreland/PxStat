using API;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Newtonsoft.Json.Linq;
using PxStat.DataStore;
using PxStat.JsonStatSchema;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;


namespace PxStat.Data
{
    /// <summary>
    /// Reads a dataset for a live release
    /// </summary>
    internal class Cube_BSO_ReadDataset : BaseTemplate_Read<CubeQuery_DTO, JsonStatQueryLive_VLD>
    {
        static Stopwatch _sw;
        internal bool defaultPivot = false;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Cube_BSO_ReadDataset(IRequest request, bool defaultRequestPivot = false) : base(request, new JsonStatQueryLive_VLD())
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
          

            _sw = new Stopwatch();
            _sw.Start();
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
            
            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", DTO);

            if (cache.hasData)
            {
                //Modify mimetype etc
                AddFormatAndMimeType();
                Response.data = cache.data;
                return true;
            }

            // Get whitelist
            string[] whitelist = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "whitelist");


            // Get host 
            string host = "";
            if(Request.requestHeaders.ContainsKey("Host"))
            {
                host = Request.requestHeaders["Host"].ToString();
            }

           


            // Check if host is not in the whitelist and the request is throttled
            if (Throttle_BSO.IsNotInTheWhitelist(whitelist, host) && Throttle_BSO.IsThrottled(Ado , Request, SamAccountName))
            {
                Log.Instance.Debug("Request throttled");
                Response.statusCode = HttpStatusCode.Forbidden;
                Response.error = Label.Get("error.throttled");



                //Further information if this is an m2m request
                if ((bool)DTO.m2m)
                {
                    Response.error = Response.error + " " + Label.Get("error.throttled-m2m");
                }
                return false;
            }



            var releaseDto = Release_ADO.GetReleaseDTO(items);
            if (releaseDto == null)
            {
                Response.statusCode = HttpStatusCode.NotFound;
                Response.data = null;
                return false;
            }
            
            var hasRun = ExecuteReadDatasetDmatrix(Ado,  DTO, releaseDto, Response, DTO.jStatQueryExtension.extension.Language.Code, DTO.jStatQueryExtension.extension.Language.Culture);

            //Modify mimetype etc
            AddFormatAndMimeType();

            if (hasRun)
            {
                if (releaseDto.RlsLiveDatetimeTo != default(DateTime))
                   AppServicesHelper.CacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", DTO, Response.data, releaseDto.RlsLiveDatetimeTo, Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + DTO.jStatQueryExtension.extension.Matrix);
                else
                   AppServicesHelper.CacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadDataset", DTO, Response.data, new DateTime(), Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + DTO.jStatQueryExtension.extension.Matrix);
            }
            return hasRun;

        }




        private void AddFormatAndMimeType()
        {
            Format_DTO_Read format = null;

            if (DTO.jStatQueryExtension?.extension?.Format != null)
            {
                format = new Format_DTO_Read()
                {
                    FrmType = DTO.jStatQueryExtension.extension.Format.Type,
                    FrmVersion = DTO.jStatQueryExtension.extension.Format.Version

                };
            }
            else
                format = new Format_DTO_Read() { FrmType = Request.parameters[Constants.C_DATA_RESTFUL_FORMAT_TYPE], FrmVersion = Request.parameters[Constants.C_DATA_RESTFUL_FORMAT_VERSION] };
            string mtype = null;
            using (Format_BSO fbso = new Format_BSO(AppServicesHelper.StaticADO))
            {
                mtype = fbso.GetMimetypeForFormat(format);
            };

            string suffix;
            using (Format_BSO bso = new Format_BSO(AppServicesHelper.StaticADO))
            {
                suffix = bso.GetFileSuffixForFormat(format);
            };
            Response.mimeType = mtype;
            if (Request.GetType().Equals(typeof(JSONRPC_API)))
                Response.fileName = Request.parameters.extension.matrix + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + suffix;
            else
                Response.fileName = Request.parameters[Constants.C_DATA_RESTFUL_MATRIX] + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + suffix;
            Response.response = Response.data;
        }

        internal static bool ExecuteReadDatasetDmatrix(IADO theAdo,  CubeQuery_DTO theDto, Release_DTO releaseDto, IResponseOutput theResponse, string requestLanguage, string culture = null, bool defaultPivot = false, bool isLive = true)
        {


            DataReader dr = new DataReader();

            DataStore_ADO dAdo = new DataStore_ADO();

            IDmatrix matrix = null;

            //If we are reading px we will bring all languages down. We start with the default language.
            //Therefore we set the request language to the default and pick up the other language(s) later
            if (theDto.jStatQueryExtension.extension.Format.Type.Equals(DatasetFormat.Px))
            {
                requestLanguage = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            }

            if (isLive)
            {
                matrix = dr.GetLiveData(theAdo, theDto.jStatQueryExtension.extension.Matrix, requestLanguage, releaseDto);
            }
            else
            {
                
                matrix = dr.GetNonLiveData(theAdo,  requestLanguage, releaseDto);
            }

            

            if (matrix == null)
            {
                theResponse.data = null;
                theResponse.statusCode = HttpStatusCode.NotFound;
                return false;
            }



            DMatrix_VLD vld = new DMatrix_VLD( theAdo);
            var validation = vld.Validate(matrix);

            if (!validation.IsValid)
            {
                foreach (var error in validation.Errors)
                {
                    Log.Instance.Error(error.ErrorMessage);
                    Log.Instance.Error($"Validation failure for matrix {theDto.jStatQueryExtension.extension.Matrix}");
                    theResponse.error = Label.Get("error.validation", theDto.jStatQueryExtension.extension.Language.Code);
                    theResponse.statusCode = HttpStatusCode.InternalServerError;
                    return false;
                }
            }

            //Validate the query against the matrix??
            
            matrix = dr.QueryDataset(theDto, matrix);

            if(matrix==null)
            {
                theResponse.data = null;
                theResponse.statusCode = HttpStatusCode.BadRequest;
                return false;
            }
            //public IDmatrix RunFractalQueryMetadata(IDmatrix matrix, CubeQuery_DTO query)

            //If this is a px file request we gather up the other language specs one by one
            if (theDto.jStatQueryExtension.extension.Format.Type.Equals(DatasetFormat.Px))
            {
                //See what languages exist for this release:
                var languages = new Compare_ADO(theAdo).ReadLanguagesForRelease(matrix.Release.RlsCode);
                var nonCoreLanguages = languages.Where(x => x != requestLanguage).ToList();
                foreach (var lng in nonCoreLanguages)
                {
                    IDmatrix lMatrix;
                    if (isLive)
                    {
                        lMatrix = dr.GetLiveData(theAdo,  theDto.jStatQueryExtension.extension.Matrix, lng, releaseDto);
                        
                    }
                    else
                    {
                        lMatrix = dr.GetNonLiveData(theAdo,  lng, releaseDto);
                    }

                    //Apply the query to the other language version of the matrix
                    //Metadata only - we already have the data
                    DataReader datareader = new DataReader();
                    lMatrix = datareader.RunFractalQueryMetadata(lMatrix, theDto);

                    //Add the spec of the other language matrix to the main output matrix
                    matrix.Dspecs.Add(lng, lMatrix.Dspecs[lng]);
                    matrix.Languages.Add(lng);
                }
            }


            if (theDto.jStatQueryExtension.extension.Format.Type.Equals(Resources.Constants.C_SYSTEM_XLSX_NAME))
            {
                int dataSize = matrix.Cells.Count;
                int maxSize = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "dataset.download.threshold.xlsx");
                if (dataSize > maxSize)
                {
                    theResponse.error = String.Format(Label.Get("error.dataset.limit", requestLanguage), dataSize, Resources.Constants.C_SYSTEM_XLSX_NAME, maxSize);
                    theResponse.statusCode = HttpStatusCode.Forbidden;
                    return false;
                }
            }
            else if (theDto.jStatQueryExtension.extension.Format.Type.Equals(Resources.Constants.C_SYSTEM_CSV_NAME))
            {
                int dataSize = matrix.Cells.Count;
                int maxSize = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "dataset.download.threshold.csv");
                if (dataSize > maxSize)
                {
                    theResponse.error = String.Format(Label.Get("error.dataset.limit", requestLanguage), dataSize, Resources.Constants.C_SYSTEM_CSV_NAME, maxSize);
                    theResponse.statusCode = HttpStatusCode.Forbidden;
                    return false;
                }
            }

            //Ensure the requested pivot actually exists among the dimensions
            if (!string.IsNullOrEmpty(theDto.jStatQueryExtension.extension.Pivot))
            {
                bool pivotOk = false;
                foreach (var dim in matrix.Dspecs[matrix.Language].Dimensions)
                {
                    if (dim.Code.Equals(theDto.jStatQueryExtension.extension.Pivot)) pivotOk = true;
                }
                if (!pivotOk)
                {
                    theResponse.error = Label.Get("error.validation", theDto.jStatQueryExtension.extension.Language.Code);
                    theResponse.statusCode = HttpStatusCode.NotFound;
                    return false;
                }

            }
            else theDto.jStatQueryExtension.extension.Pivot = null;


            //Set culture
            CultureInfo readCulture;

            if (culture == null)
                readCulture = CultureInfo.CurrentCulture;
            else
                readCulture = CultureInfo.CreateSpecificCulture(culture);


            //Set a default pivot of the time dimension if necessary
            if (theDto.jStatQueryExtension.extension.Pivot == null && defaultPivot && theDto.jStatQueryExtension.extension.Format.Type == DatasetFormat.Csv)
            {
                theDto.jStatQueryExtension.extension.Pivot = matrix.Dspecs[matrix.Language].Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_TIME).FirstOrDefault().Code;
            }

            switch (theDto.jStatQueryExtension.extension.Format.Type)
            {
                case DatasetFormat.JsonStat:

                    if (theDto.jStatQueryExtension.extension.Format.Version == "1.0")
                    {
                        JsonStatBuilder1_0 jxb = new JsonStatBuilder1_0();
                        var jsonStat = jxb.Create(matrix, matrix.Language, true);
                        theResponse.data = new JRaw(SerializeJsonStatV1.ToJson(jsonStat));
                    }
                    else if (theDto.jStatQueryExtension.extension.Format.Version == "1.1")
                    {
                        JsonStatBuilder1_1 jxb = new JsonStatBuilder1_1();
                        var jsonStat = jxb.Create(matrix, matrix.Language, true);
                        theResponse.data = new JRaw(SerializeJsonStatV1_1.ToJson(jsonStat));
                    }
                    else
                    {
                        JsonStatBuilder2_0 jxb = new JsonStatBuilder2_0();
                        var jsonStat = jxb.Create(matrix, matrix.Language, true);
                        theResponse.data = new JRaw(Serialize.ToJson(jsonStat));
                    }
                    break;
                case DatasetFormat.Csv:
                    {
                        FlatTableBuilder ftb = new FlatTableBuilder();
                        var pivot = theDto.jStatQueryExtension.extension.Pivot;
                        DataTable dt;
                        if (pivot == null)
                            dt = ftb.GetMatrixDataTableCodesAndLabels(matrix, requestLanguage, false, 0, theDto.jStatQueryExtension.extension.Codes == null ? false : (bool)theDto.jStatQueryExtension.extension.Codes);
                        else
                            dt = ftb.GetMatrixDataTablePivot(matrix, requestLanguage, pivot, theDto.jStatQueryExtension.extension.Codes == null ? false : (bool)theDto.jStatQueryExtension.extension.Codes);

                        theResponse.data = ftb.GetCsv(dt, "\"", readCulture, false);
                        break;
                    }
                case DatasetFormat.Px:
                    PxFileBuilder pxb = new PxFileBuilder();
                    theResponse.data = pxb.Create(matrix,  requestLanguage);
                    break;
                case DatasetFormat.Xlsx:
                    {
                        FlatTableBuilder ftb = new FlatTableBuilder();
                        theResponse.data = ftb.GetXlsxObject(matrix, requestLanguage, CultureInfo.CurrentCulture, theDto.jStatQueryExtension.extension.Pivot, theDto.jStatQueryExtension.extension.Codes == null ? true : (bool)theDto.jStatQueryExtension.extension.Codes);
                        break;
                    }

            }

            return true;
        }



    }
}
