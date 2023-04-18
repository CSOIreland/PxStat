using API;
using PxStat.JsonQuery;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Navigation;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PxStat.Data
{

    public class Cube_BSO_ReadPxApiV1
    {
        public IResponseOutput Read(IRequest request)
        {
            IMetaData metaData = new MetaData();
            IPathProvider servicePathProvider = new ServerPathProvider();
            Configuration_BSO.GetInstance(metaData, servicePathProvider);
            Configuration_BSO.SetConfigFromFiles();

            var parameters = ((List<string>)request.parameters).Where(x => !string.IsNullOrWhiteSpace(x));
            int pcount = parameters.Count() - 1;
            PxApiMetadata meta = new PxApiMetadata();
            IResponseOutput rspSbj = null;
            switch (pcount)
            {

                case Constants.C_DATA_PXAPIV1_SUBJECT_QUERY - 1:
                    //Get a list of all subjects - but no language was specified
                    //In that case we add the default language to the url and continue as a subject query

                    request.parameters = new List<string>() { Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code") };
                    rspSbj = new Subject_BSO_Read(request).Read().Response;
                    rspSbj.response = Utility.JsonSerialize_IgnoreLoopingReference(meta.ReadSubjectsAsObjectList(rspSbj));
                    return rspSbj;

                case Constants.C_DATA_PXAPIV1_SUBJECT_QUERY:
                    //Get a list of all subjects

                    request.parameters = new List<string>() { parameters.ElementAt(Constants.C_DATA_PXAPIV1_SUBJECT_QUERY) };
                    rspSbj = new Subject_BSO_Read(request).Read().Response;
                    rspSbj.response = Utility.JsonSerialize_IgnoreLoopingReference(meta.ReadSubjectsAsObjectList(rspSbj));
                    return rspSbj;

                case Constants.C_DATA_PXAPIV1_PRODUCT_QUERY:
                    //Get a list of products for a subject
                    var rspPrd = new Product_BSO_Read(request).Read().Response;
                    rspPrd.response = Utility.JsonSerialize_IgnoreLoopingReference(meta.ReadProductsAsObjectList(rspPrd));
                    return rspPrd;

                case Constants.C_DATA_PXAPIV1_COLLECTION_QUERY:
                    //Get the collection
                    var rspCol = new Cube_BSO_ReadCollectionSummary(request, false).Read().Response;
                    rspCol.response = Utility.JsonSerialize_IgnoreLoopingReference(meta.ReadCollectionAsObjectList(rspCol));
                    return rspCol;
                default:

                    //validate the request
                    if (pcount < Constants.C_DATA_PXAPIV1_METADATA_QUERY)
                    {
                        throw new Exception(Label.Get("error.validation"));
                    }
                    //If this is a HEAD request, we just check the existence of the matrix
                    if (!request.GetType().Equals(typeof(Head_API)))
                    {
                        //Either get the metadata for the table or, if a query has been supplied, get the table data based on the query
                        string rq = string.IsNullOrWhiteSpace(request.httpPOST) ? request.httpGET[Constants.C_JSON_STAT_QUERY_CLASS] : request.httpPOST;

                        if (rq == null)
                        {
                            //This is a request for metadata only
                            var rspMeta = new Cube_BSO_ReadMetadataMatrix(request, false).Read().Response;
                            rspMeta.response = Utility.JsonSerialize_IgnoreLoopingReference(rspMeta.data);
                            return rspMeta;
                        }
                        else
                        {
                            //This is a query and requires data to be returned
                            return ReadPxApiV1data(request);
                        }
                    }
                    else
                    {
                        return new Cube_BSO_ReadMetadataHEAD(request).Read().Response;

                    }

            }



        }

        private IResponseOutput ReadPxApiV1data(IRequest request)
        {
            //For conventional RESTful patterns, don't return metadata unless the requested MtrCode is contained in the collection
            //                //for the subject and product parameters
            List<dynamic> collectionPrdSbjdata = new List<dynamic>();
            List<dynamic> collectionPrdSbjmeta = new List<dynamic>();

            using (Cube_BSO cBso = new Cube_BSO(new ADO("defaultConnection")))
            {
                collectionPrdSbjdata = cBso.ReadCollection(request.parameters[Constants.C_DATA_PXAPIV1_SUBJECT_QUERY], request.parameters[Constants.C_DATA_PXAPIV1_PRODUCT_QUERY], request.parameters[Constants.C_DATA_PXAPIV1_COLLECTION_QUERY]);
            }

            if (collectionPrdSbjmeta == null)
            {
                var empty = new RESTful_Output();
                empty.statusCode = HttpStatusCode.NotFound;
                return empty;
            }

            if (collectionPrdSbjdata.Where(x => x.MtrCode == request.parameters[Constants.C_DATA_PXAPIV1_METADATA_QUERY]).ToList().Count == 0)
            {
                var empty = new RESTful_Output();
                empty.statusCode = HttpStatusCode.NotFound;
                return empty;
            }


            string queryString = string.IsNullOrWhiteSpace(request.httpPOST) ? request.httpGET[Constants.C_JSON_STAT_QUERY_CLASS] : request.httpPOST;

            PxApiV1Query pxapiQuery = Utility.JsonDeserialize_IgnoreLoopingReference<PxApiV1Query>(queryString);


            Format_DTO_Read format = new Format_DTO_Read(pxapiQuery.Response.Format);
            string pivot = pxapiQuery.Response.Pivot;


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

            jsQuery.Extension.Add("matrix", request.parameters[Constants.C_DATA_PXAPIV1_METADATA_QUERY]);
            jsQuery.Extension.Add("language", new Language() { Code = request.parameters[Constants.C_DATA_PXAPIV1_SUBJECT_QUERY], Culture = null });
            jsQuery.Extension.Add("codes", false);
            jsQuery.Extension.Add("format", new JsonQuery.Format() { Type = format.FrmType, Version = format.FrmVersion });
            if (pivot != null)
            {
                jsQuery.Extension.Add("pivot", pivot);
            }
            jsQuery.Version = Constants.C_JSON_STAT_QUERY_VERSION;


            request.parameters = Utility.JsonSerialize_IgnoreLoopingReference(jsQuery);

            //Run the request as a Json Rpc call
            IResponseOutput rsp = new Cube_BSO_ReadDataset(request, true).Read().Response;


            string suffix;
            using (Format_BSO bso = new Format_BSO(new ADO("defaultConnection")))
            {
                suffix = bso.GetFileSuffixForFormat(format);
            };
            if (suffix != null)
                rsp.fileName = jsQuery.Extension["matrix"] + "." + DateTime.Now.ToString("yyyyMMddHHmmss") + suffix;
            return rsp;
        }

    }
}
