
using API;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json.Linq;
using PxStat.DataStore;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PxStat.Data
{
    internal class Cube_BSO_ReadPxApiV1Bso : BaseTemplate_Read<PxApiV1_DTO, PxApiV1_VLD>
    {
        public Cube_BSO_ReadPxApiV1Bso(IRequest request) : base(request, new PxApiV1_VLD())
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

            MemCachedD_Value cache = AppServicesHelper.CacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadPxApiV1Bso", DTO);


            if (cache.hasData)
            {
                AddFormatAndMimeType();
                Response.data = cache.data;
                return true;
            }

            DataReader dr =new DataReader();
            var qmatrix = dr.ReadLiveDataset(DTO.MtrCode, DTO.LngIsoCode, null);

            //Effectively, nothing found
            if(qmatrix.Dspecs.Count==0)
            {
                Response.statusCode = HttpStatusCode.NotFound;
                return false;
            }

            //Only run a query if there's a query in the request!
            var matrix = DTO.jsonStatQueryPxApiV1.Query.Count == 0 ? qmatrix : dr.RunFractalQuery(qmatrix, DTO);

            //If this is a px file request we gather up the other language specs one by one
            if (DTO.jsonStatQueryPxApiV1.Response.Format.Equals(DatasetFormat.Px.ToUpper()))
            {
                //See what languages exist for this release:
                var languages = new Compare_ADO(Ado).ReadLanguagesForRelease(matrix.Release.RlsCode);
                var nonCoreLanguages = languages.Where(x => x != DTO.LngIsoCode).ToList();
                foreach (var lng in nonCoreLanguages)
                {
                    DTO.LngIsoCode = lng;
                    IDmatrix lMatrix;

                    lMatrix = dr.ReadLiveDataset(DTO.MtrCode, lng, null);


                    //Apply the query to the other language version of the matrix
                    //Metadata only - we already have the data
                    DataReader datareader = new DataReader();
                    lMatrix = datareader.RunFractalQueryMetadata(lMatrix, DTO);

                    //Add the spec of the other language matrix to the main output matrix
                    matrix.Dspecs.Add(lng, lMatrix.Dspecs[lng]);
                    matrix.Languages.Add(lng);
                }
                DTO.LngIsoCode = matrix.Language;
            }


            if (DTO.jsonStatQueryPxApiV1.Response.Format.Equals(Resources.Constants.C_SYSTEM_XLSX_NAME))
            {
                int dataSize = matrix.Cells.Count;
                int maxSize = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "dataset.download.threshold.xlsx");
                if (dataSize > maxSize)
                {
                    Response.error = String.Format(Label.Get("error.dataset.limit", DTO.LngIsoCode), dataSize, Resources.Constants.C_SYSTEM_XLSX_NAME, maxSize);
                    Response.statusCode = HttpStatusCode.Forbidden;
                    return false;
                }
            }
            else if (DTO.jsonStatQueryPxApiV1.Response.Format.Equals(Resources.Constants.C_SYSTEM_CSV_NAME))
            {
                int dataSize = matrix.Cells.Count;
                int maxSize = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "dataset.download.threshold.csv");
                if (dataSize > maxSize)
                {
                    Response.error = String.Format(Label.Get("error.dataset.limit", DTO.LngIsoCode ), dataSize, Resources.Constants.C_SYSTEM_CSV_NAME, maxSize);
                    Response.statusCode = HttpStatusCode.Forbidden;
                    return false;
                }
            }

            //Ensure the requested pivot actually exists among the dimensions
            if (!string.IsNullOrEmpty(DTO.jsonStatQueryPxApiV1.Response.Pivot))
            {
                bool pivotOk = false;
                foreach (var dim in matrix.Dspecs[matrix.Language].Dimensions)
                {
                    if (dim.Code.Equals(DTO.jsonStatQueryPxApiV1.Response.Pivot)) pivotOk = true;
                }
                if (!pivotOk)
                {
                    Response.error = Label.Get("error.validation", DTO.LngIsoCode);
                    Response.statusCode = HttpStatusCode.BadRequest;
                    return false;
                }

            }
            


            //Set culture
            CultureInfo readCulture = CultureInfo.CurrentCulture; ;


            


            //Set a default pivot of the time dimension if necessary
            if (DTO.jsonStatQueryPxApiV1.Response.Pivot == null  && DTO.jsonStatQueryPxApiV1.Response.Format == DatasetFormat.Csv)
            {
                DTO.jsonStatQueryPxApiV1.Response.Pivot = matrix.Dspecs[matrix.Language].Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_TIME).FirstOrDefault().Code;
            }

            bool codes = DTO.jsonStatQueryPxApiV1.Response.Codes;

            switch (DTO.jsonStatQueryPxApiV1.Response.Format)
            {
                case "json-stat":
                    
                    
                        JsonStatBuilder1_0 jxb = new JsonStatBuilder1_0();
                        var jsonStat = jxb.Create(matrix, matrix.Language, true);
                        Response.data = new JRaw(SerializeJsonStatV1.ToJson(jsonStat));
                    break;

                case "json-stat2":
                    
                        JsonStatBuilder2_0 jxb2 = new JsonStatBuilder2_0();
                        var jsonStat2 = jxb2.Create(matrix, matrix.Language, true);
                        Response.data = new JRaw(Serialize.ToJson(jsonStat2));
                    break;
                    

                case "csv":
                    
                        FlatTableBuilder ftb = new FlatTableBuilder();
                        var pivot = DTO.jsonStatQueryPxApiV1.Response.Pivot;
                        DataTable dt;
                    if (pivot == null)
                        dt = ftb.GetMatrixDataTableCodesAndLabels(matrix, DTO.LngIsoCode, false,0,codes);//, 0, theDto.jStatQueryExtension.extension.Codes == null ? false : (bool)theDto.jStatQueryExtension.extension.Codes);
                    else
                        dt = ftb.GetMatrixDataTablePivot(matrix, DTO.LngIsoCode, pivot,codes);//, theDto.jStatQueryExtension.extension.Codes == null ? false : (bool)theDto.jStatQueryExtension.extension.Codes);

                        Response.data = ftb.GetCsv(dt, "\"", readCulture, false);
                        break;
                    
                case "px":
                    PxFileBuilder pxb = new PxFileBuilder();
                    Response.data = pxb.Create(matrix, DTO.LngIsoCode);
                    break;
                case "xlsx":
                    {
                        FlatTableBuilder ftbXls = new FlatTableBuilder();
                        Response.data = ftbXls.GetXlsxObject(matrix, DTO.LngIsoCode, CultureInfo.CurrentCulture, DTO.jsonStatQueryPxApiV1.Response.Pivot,codes);
                        break;
                    }

            }

            AddFormatAndMimeType();


            //The Language of the received data may be different from the request - so we make sure it corresponds to the language of the dataset (???)
            var items = new Release_ADO(Ado).ReadLiveNow(matrix.Code, matrix.Language);
            
            var releaseDto = Release_ADO.GetReleaseDTO(items);


            if (releaseDto.RlsLiveDatetimeTo != default(DateTime))
                AppServicesHelper.CacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadPxApiV1Bso", DTO, Response.data, releaseDto.RlsLiveDatetimeTo, Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + matrix.Code);
            else
                AppServicesHelper.CacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadPxApiV1Bso", DTO, Response.data, new DateTime(), Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + matrix.Code);
           

            return true;
        }

        private void AddFormatAndMimeType()
        {
            string formatName=null;
            string formatVersion=null;

            switch (DTO.jsonStatQueryPxApiV1?.Response?.Format)
            {
                case Constants.C_PXAPIV1_PX :
                    {
                        formatName = Constants.C_SYSTEM_PX_NAME;
                        formatVersion=Constants.C_SYSTEM_PX_VERSION;
                        break;
                    }
                case Constants.C_PXAPIV1_XLSX:
                    {
                        formatName = Constants.C_SYSTEM_XLSX_NAME;
                        formatVersion = Constants.C_SYSTEM_XLSX_VERSION;
                        break;
                    }
                case Constants.C_PXAPIV1_JSON_STAT_1X:
                    {
                        formatName = Constants.C_SYSTEM_JSON_STAT_NAME;
                        formatVersion = Constants.C_SYSTEM_JSON_STAT_1X_VERSION;
                        break;
                    }
                case Constants.C_PXAPIV1_JSON_STAT_2X:
                    {
                        formatName = Constants.C_SYSTEM_JSON_STAT_NAME;
                        formatVersion = Constants.C_SYSTEM_JSON_STAT_2X_VERSION;
                        break;
                    }
                case Constants.C_PXAPIV1_CSV:
                    {
                        formatName = Constants.C_SYSTEM_CSV_NAME;
                        formatVersion=Constants.C_SYSTEM_CSV_VERSION;
                        break;
                    }
                
            }

            Format_DTO_Read format = null;

            if (DTO.jsonStatQueryPxApiV1?.Response?.Format != null)
            {
                format = new Format_DTO_Read()
                {
                    FrmType = formatName, FrmVersion= formatVersion

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
            //if (Request.GetType().Equals(typeof(JSONRPC_API)))
            //    Response.fileName = Request.parameters.extension.matrix + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + suffix;
            //else
                Response.fileName = DTO.MtrCode + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + suffix;
            if (Response.data != null)
            {
                Response.response = Response.data;
            }
        }
    }
}