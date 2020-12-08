using API;
using FluentValidation;
using FluentValidation.Results;
using Newtonsoft.Json.Linq;
using PxStat.JsonQuery;
using PxStat.Resources;
using PxStat.System.Navigation;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace PxStat.Data
{
    /// <summary>
    /// Cube APIs are used for reading data and metadata.
    /// </summary>

    public class Cube_API
    {
        [Analytic]
        public static dynamic PxAPIv1(RESTful_API restfulRequest)
        {
            try
            {
                var parameters = ((List<string>)restfulRequest.parameters).Where(x => !string.IsNullOrWhiteSpace(x));
                int pcount = parameters.Count() - 1;
                JSONRPC_API rpc = Map.RESTful2JSONRPC_API(restfulRequest);

                Cube_MAP map = new Cube_MAP();
                JObject rpcParams = new JObject();
                PxApiMetadata meta = new PxApiMetadata();

                var context = HttpContext.Current.Request;

                switch (pcount)
                {
                    case Constants.C_DATA_PXAPIV1_SUBJECT_QUERY:
                        //Get a list of all subjects

                        rpcParams.Add("LngIsoCode", parameters.ElementAt(Constants.C_DATA_PXAPIV1_SUBJECT_QUERY));
                        rpc.parameters = rpcParams;

                        var rspSbj = new Subject_BSO_Read(rpc).Read().Response;

                        if (rspSbj.data == null)
                            return new RESTful_Output() { statusCode = HttpStatusCode.NotFound };

                        rspSbj.data = Utility.JsonSerialize_IgnoreLoopingReference(meta.ReadSubjectsAsObjectList(rspSbj));



                        var responseSbj = Map.JSONRPC2RESTful_Output(rspSbj, null, rspSbj.data == null ? HttpStatusCode.NotFound : HttpStatusCode.OK);

                        return responseSbj;

                    case Constants.C_DATA_PXAPIV1_PRODUCT_QUERY:
                        //Get a list of products for a subject
                        rpcParams.Add("LngIsoCode", parameters.ElementAt(Constants.C_DATA_PXAPIV1_SUBJECT_QUERY));

                        rpcParams.Add("SbjCode", parameters.ElementAt(Constants.C_DATA_PXAPIV1_PRODUCT_QUERY));
                        rpc.parameters = rpcParams;

                        var rspPrd = new Product_BSO_Read(rpc).Read().Response;

                        if (rspPrd.data == null)
                            return new RESTful_Output() { statusCode = HttpStatusCode.NotFound };

                        meta = new PxApiMetadata();

                        if (rspPrd.data.Count == 0)
                        {
                            rspPrd = new JSONRPC_Output();
                            rspPrd.data = null;
                        }
                        else
                        {
                            rspPrd.data = Utility.JsonSerialize_IgnoreLoopingReference(meta.ReadProductsAsObjectList(rspPrd));

                        }

                        var responsePrd = Map.JSONRPC2RESTful_Output(rspPrd, null, rspPrd.data == null ? HttpStatusCode.NotFound : HttpStatusCode.OK);

                        return responsePrd;

                    case Constants.C_DATA_PXAPIV1_COLLECTION_QUERY:
                        //Get a list of live tables for a product
                        rpcParams.Add("LngIsoCode", parameters.ElementAt(Constants.C_DATA_PXAPIV1_SUBJECT_QUERY));
                        rpcParams.Add("product", parameters.ElementAt(Constants.C_DATA_PXAPIV1_COLLECTION_QUERY));

                        if (!Int32.TryParse(parameters.ElementAt(Constants.C_DATA_PXAPIV1_PRODUCT_QUERY), out int sbjInt))
                            rpcParams.Add("SbjCode", "0");
                        else
                            rpcParams.Add("SbjCode", sbjInt.ToString());


                        rpc.parameters = rpcParams;

                        List<dynamic> collection = new List<dynamic>();

                        RESTful_Output output = new RESTful_Output();
                        DateTime nextReleaseDateForProduct = default;

                        using (Cube_BSO cBso = new Cube_BSO(new ADO("defaultConnection")))
                        {
                            nextReleaseDateForProduct = cBso.GetNextReleaseDate(parameters.ElementAt(Constants.C_DATA_PXAPIV1_COLLECTION_QUERY));
                        }

                        //See if this request has cached data
                        MemCachedD_Value cache = MemCacheD.Get_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadCollectionPxApi", rpcParams);

                        if (cache.hasData && (nextReleaseDateForProduct == default || nextReleaseDateForProduct > DateTime.Now))
                        {
                            output.response = cache.data;
                            output.statusCode = HttpStatusCode.OK;
                        }

                        else
                        {
                            using (Cube_BSO cBso = new Cube_BSO(new ADO("defaultConnection")))
                            {
                                collection = cBso.ReadCollection(parameters.ElementAt(Constants.C_DATA_PXAPIV1_SUBJECT_QUERY), parameters.ElementAt(Constants.C_DATA_PXAPIV1_PRODUCT_QUERY), parameters.ElementAt(Constants.C_DATA_PXAPIV1_COLLECTION_QUERY));
                            }

                            meta = new PxApiMetadata();

                            if (collection != null)
                            {
                                if (collection.Count > 0)
                                {
                                    output.response = Utility.JsonSerialize_IgnoreLoopingReference(meta.ReadCollectionAsObjectList(new JSONRPC_Output() { data = collection }));
                                    output.statusCode = HttpStatusCode.OK;
                                }
                                else
                                    output.statusCode = HttpStatusCode.NotFound;
                            }
                            else
                                output.statusCode = HttpStatusCode.NotFound;
                        }


                        MemCacheD.Store_BSO<dynamic>("PxStat.Data", "Cube_API", "ReadCollectionPxApi", rpcParams, output.response, nextReleaseDateForProduct, Constants.C_CAS_DATA_CUBE_READ_COLLECTION_PXAPI);

                        return output;

                    default:

                        if (pcount < Constants.C_DATA_PXAPIV1_METADATA_QUERY)
                        {
                            throw new Exception(Label.Get("error.validation"));
                        }

                        //Either get the metadata for the table or, if a query has been supplied, get the table data based on the query
                        string request = string.IsNullOrWhiteSpace(restfulRequest.httpPOST) ? restfulRequest.httpGET[Constants.C_JSON_STAT_QUERY_CLASS] : restfulRequest.httpPOST;

                        //For conventional RESTful patterns, don't return metadata unless the requested MtrCode is contained in the collection
                        //for the subject and product parameters
                        List<dynamic> collectionPrdSbjmeta = new List<dynamic>();
                        using (Cube_BSO cBso = new Cube_BSO(new ADO("defaultConnection")))
                        {
                            collectionPrdSbjmeta = cBso.ReadCollection(parameters.ElementAt(Constants.C_DATA_PXAPIV1_SUBJECT_QUERY), parameters.ElementAt(Constants.C_DATA_PXAPIV1_PRODUCT_QUERY), parameters.ElementAt(Constants.C_DATA_PXAPIV1_COLLECTION_QUERY));
                        }

                        if (collectionPrdSbjmeta == null)
                        {
                            var empty = new RESTful_Output();
                            empty.statusCode = HttpStatusCode.NotFound;
                            return empty;
                        }

                        if (collectionPrdSbjmeta.Where(x => x.MtrCode == parameters.ElementAt(Constants.C_DATA_PXAPIV1_METADATA_QUERY)).ToList().Count == 0)
                        {
                            var empty = new RESTful_Output();
                            empty.statusCode = HttpStatusCode.NotFound;
                            return empty;
                        }

                        if (request == null)
                        {

                            rpcParams.Add("LngIsoCode", parameters.ElementAt(Constants.C_DATA_PXAPIV1_SUBJECT_QUERY));

                            rpcParams.Add("matrix", parameters.ElementAt(Constants.C_DATA_PXAPIV1_METADATA_QUERY));

                            rpcParams.Add("format", JToken.FromObject(new { type = Constants.C_SYSTEM_JSON_STAT_NAME, version = Constants.C_SYSTEM_JSON_STAT_2X_VERSION }));

                            Matrix theMatrix;
                            using (Cube_BSO cBso = new Cube_BSO(new ADO("defaultConnection")))
                            {
                                theMatrix = cBso.GetMetadataMatrix(parameters.ElementAt(Constants.C_DATA_PXAPIV1_SUBJECT_QUERY), parameters.ElementAt(Constants.C_DATA_PXAPIV1_METADATA_QUERY));

                            }
                            var rspMeta = new RESTful_Output();// JSONRPC_Output();

                            if (theMatrix != null)
                            {
                                var json = theMatrix.GetApiMetadata(parameters.ElementAt(Constants.C_DATA_PXAPIV1_SUBJECT_QUERY));

                                rpc.parameters = rpcParams;

                                rspMeta.response = Utility.JsonSerialize_IgnoreLoopingReference(json);
                                rspMeta.statusCode = HttpStatusCode.OK;
                            }
                            else rspMeta.statusCode = HttpStatusCode.NotFound;


                            return rspMeta;

                        }
                        else
                        {
                            //This is a request for data - we need to use the query

                            //For conventional RESTful patterns, don't return metadata unless the requested MtrCode is contained in the collection
                            //for the subject and product parameters
                            List<dynamic> collectionPrdSbjdata = new List<dynamic>();
                            using (Cube_BSO cBso = new Cube_BSO(new ADO("defaultConnection")))
                            {
                                collectionPrdSbjdata = cBso.ReadCollection(parameters.ElementAt(Constants.C_DATA_PXAPIV1_SUBJECT_QUERY), parameters.ElementAt(Constants.C_DATA_PXAPIV1_PRODUCT_QUERY), parameters.ElementAt(Constants.C_DATA_PXAPIV1_COLLECTION_QUERY));
                            }

                            if (collectionPrdSbjmeta == null)
                            {
                                var empty = new RESTful_Output();
                                empty.statusCode = HttpStatusCode.NotFound;
                                return empty;
                            }

                            if (collectionPrdSbjdata.Where(x => x.MtrCode == parameters.ElementAt(Constants.C_DATA_PXAPIV1_METADATA_QUERY)).ToList().Count == 0)
                            {
                                var empty = new RESTful_Output();
                                empty.statusCode = HttpStatusCode.NotFound;
                                return empty;
                            }


                            string queryString = string.IsNullOrWhiteSpace(restfulRequest.httpPOST) ? restfulRequest.httpGET[Constants.C_JSON_STAT_QUERY_CLASS] : restfulRequest.httpPOST;

                            PxApiV1Query pxapiQuery = Utility.JsonDeserialize_IgnoreLoopingReference<PxApiV1Query>(queryString);


                            Format_DTO_Read format = new Format_DTO_Read(pxapiQuery.Response.Format);

                            
                            JsonStatQuery jsQuery = new JsonStatQuery();
                            jsQuery.Class = Constants.C_JSON_STAT_QUERY_CLASS;
                            jsQuery.Id = new List<string>();
                            jsQuery.Dimensions = new Dictionary<string, JsonQuery.Dimension>();
                            foreach (var q in pxapiQuery.Query)
                            {
                                jsQuery.Id.Add(q.Code);
                                jsQuery.Dimensions.Add(q.Code, new JsonQuery.Dimension() { Category = new JsonQuery.Category() { Index = q.Selection.Values.ToList<string>() } });
                            }
                            jsQuery.Extension = new Dictionary<string, object>();

                            jsQuery.Extension.Add("matrix", parameters.ElementAt(Constants.C_DATA_PXAPIV1_METADATA_QUERY));
                            jsQuery.Extension.Add("language", new Language() { Code = parameters.ElementAt(Constants.C_DATA_PXAPIV1_SUBJECT_QUERY), Culture = null });
                            jsQuery.Extension.Add("codes", false);
                            jsQuery.Extension.Add("format", new JsonQuery.Format() { Type = format.FrmType, Version = format.FrmVersion });
                            if (pcount >= Constants.C_DATA_PXAPIV1_DATA_QUERY)
                            {
                                jsQuery.Extension.Add("pivot", parameters.ElementAt(Constants.C_DATA_PXAPIV1_DATA_QUERY));
                            }
                            jsQuery.Version = Constants.C_JSON_STAT_QUERY_VERSION;


                            JSONRPC_API jsonRpcRequest = Map.RESTful2JSONRPC_API(restfulRequest);

                            jsonRpcRequest.parameters = Utility.JsonSerialize_IgnoreLoopingReference(jsQuery);

                            //Run the request as a Json Rpc call
                            JSONRPC_Output rsp = new Cube_BSO_ReadDataset(jsonRpcRequest, true).Read().Response;
                            // Convert the JsonRpc output to RESTful output
                            var response = Map.JSONRPC2RESTful_Output(rsp, format.FrmMimetype, rsp.data == null ? HttpStatusCode.NotFound : HttpStatusCode.OK);

                            string suffix;
                            using (Format_BSO bso = new Format_BSO(new ADO("defaultConnection")))
                            {
                                suffix = bso.GetFileSuffixForFormat(format);
                            };


                            response.fileName = parameters.ElementAt(Constants.C_DATA_PXAPIV1_METADATA_QUERY) + suffix;
                            return response;

                        }

                }
            }
            catch (Exception ex)

            {
                RESTful_Output error = new RESTful_Output
                {
                    statusCode = HttpStatusCode.InternalServerError
                };

                Log.Instance.Debug(ex.Message);
                return error;
            }


        }

        private static dynamic RunQuery()
        {
            JsonStatQuery jq = new JsonStatQuery();
            jq.Class = Constants.C_JSON_STAT_QUERY_CLASS;

            return null;
        }

        /// <summary>
        /// Reads a live dataset based on specific criteria. 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [Analytic]
        //Internal cache management
        public static dynamic ReadDataset(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadDataset(jsonrpcRequest).Read().Response;
        }
        /// <summary>
        /// Reads a live dataset based on specific criteria. RESTful version
        /// </summary>
        /// <param name="restfulRequest"></param>
        /// <returns></returns>
        /// 
        [Analytic]
        public static dynamic ReadDataset(RESTful_API restfulRequest)
        {

            //A pre-validation. If a validation problem is found then an output containing the error will be created and returned immediately
            var vldOutput = ValidateRest<Cube_VLD_REST_ReadDataset>(restfulRequest, new Cube_VLD_REST_ReadDataset());
            if (vldOutput != null) return vldOutput;

            //Map the RESTful request to an equivalent Json Rpc request
            JSONRPC_API jsonRpcRequest = Map.RESTful2JSONRPC_API(restfulRequest);

            //Map the parameters - this is specific to the function
            Cube_MAP map = new Cube_MAP();
            //jsonRpcRequest.parameters = map.ReadDataset_MapParameters(jsonRpcRequest.parameters);

            JsonStatQuery jq = map.ReadMetadata_MapParametersToQuery(restfulRequest.parameters);


            jsonRpcRequest.parameters = Utility.JsonSerialize_IgnoreLoopingReference(jq);


            //Run the request as a Json Rpc call
            JSONRPC_Output rsp = new Cube_BSO_ReadDataset(jsonRpcRequest).Read().Response;

            //Convert the JsonRpc output to RESTful output
            var response = Map.JSONRPC2RESTful_Output(rsp, map.MimeType, rsp.data == null ? HttpStatusCode.NotFound : HttpStatusCode.NoContent);

            response.fileName = map.FileName;
            return response;

        }

        /// <summary>
        /// Creates a pre-validation of the RESTful request
        /// You must supply a generic validator and a RESTful_API
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="restfulRequest"></param>
        /// <param name="Validation"></param>
        /// <returns>Returns a RESTful_Output with error details if validation fails. This can be returned to the user. Othwerwise returns null</returns>
        internal static RESTful_Output ValidateRest<V>(RESTful_API restfulRequest, V Validation) where V : AbstractValidator<RESTful_API>
        {
            ValidationResult result = Validation.Validate(restfulRequest);
            if (!result.IsValid)
            {
                foreach (var e in result.Errors)
                {
                    Log.Instance.Debug(e.ErrorMessage);
                }
                RESTful_Output output = new RESTful_Output();
                output.statusCode = HttpStatusCode.BadRequest;
                output.response = Label.Get("error.invalid");
                return output;
            }
            return null;
        }

        /// <summary>
        /// Reads the metadata for a live dataset based on specific criteria.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>

        public static dynamic ReadMetadata(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadMetadata(jsonrpcRequest).Read().Response;
        }
        /// <summary>
        ///  Reads the metadata for a live dataset based on specific criteria. RESTful version
        /// </summary>
        /// <param name="restfulRequest"></param>
        /// <returns></returns>
        public static dynamic ReadMetadata(RESTful_API restfulRequest)
        {
            //A pre-validation. If a validation problem is found then an output containing the error will be created and returned immediately
            var vldOutput = ValidateRest<Cube_VLD_REST_ReadMetadata>(restfulRequest, new Cube_VLD_REST_ReadMetadata());
            if (vldOutput != null) return vldOutput;

            //Map the RESTful request to an equivalent Json Rpc request
            JSONRPC_API jsonRpcRequest = Map.RESTful2JSONRPC_API(restfulRequest);

            //Map the parameters - this is specific to the function
            Cube_MAP map = new Cube_MAP();
            jsonRpcRequest.parameters = map.ReadMetadata_MapParameters(jsonRpcRequest.parameters);

            //Run the request as a Json Rpc call
            JSONRPC_Output rsp = new Cube_BSO_ReadMetadata(jsonRpcRequest).Read().Response;

            //Convert the JsonRpc output to RESTful output
            return Map.JSONRPC2RESTful_Output(rsp, map.MimeType, rsp.data == null ? HttpStatusCode.NotFound : HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Reads any Dataset (including pre-release data) based on Release Code and other criteria. 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheRead(CAS_REPOSITORY =
            Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET, DOMAIN = "release")]
        public static dynamic ReadPreDataset(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadPreDataset(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Reads the metadata for a non live dataset based on Release code and other criteria. 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheRead(CAS_REPOSITORY = Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA, DOMAIN = "release")]
        public static dynamic ReadPreMetadata(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadPreMetadata(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Returns a Collection of JsonStat items.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadCollection(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadCollection(jsonrpcRequest, false).Read().Response;
        }

        public static dynamic ReadMetaCollection(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadCollection(jsonrpcRequest, true).Read().Response;
        }

        /// <summary>
        ///  Returns a Collection of JsonStat items. RESTful version
        /// </summary>
        /// <param name="restfulRequest"></param>
        /// <returns></returns>
        public static dynamic ReadCollection(RESTful_API restfulRequest)
        {
            //A pre-validation. If a validation problem is found then an output containing the error will be created and returned immediately
            var vldOutput = ValidateRest<Cube_VLD_REST_ReadCollection>(restfulRequest, new Cube_VLD_REST_ReadCollection());
            if (vldOutput != null) return vldOutput;

            //Map the RESTful request to an equivalent Json Rpc request
            JSONRPC_API jsonRpcRequest = Map.RESTful2JSONRPC_API(restfulRequest);

            //Map the parameters - this is specific to the function
            Cube_MAP map = new Cube_MAP();
            jsonRpcRequest.parameters = map.ReadCollection_MapParameters(jsonRpcRequest.parameters);

            //Run the request as a Json Rpc call
            JSONRPC_Output rsp = new Cube_BSO_ReadCollection(jsonRpcRequest, false).Read().Response;

            string mimeType;
            using (Format_BSO fbso = new Format_BSO(new ADO("defaultConnection")))
            {
                mimeType = fbso.GetMimetypeForFormat(new Format_DTO_Read() { FrmType = Constants.C_SYSTEM_JSON_STAT_NAME });
            };

            //Convert the JsonRpc output to RESTful output
            var response = Map.JSONRPC2RESTful_Output(rsp, mimeType);
            if (rsp.data == null) response.statusCode = HttpStatusCode.NoContent;
            response.fileName = map.FileName;
            return response;
        }
    }

}
