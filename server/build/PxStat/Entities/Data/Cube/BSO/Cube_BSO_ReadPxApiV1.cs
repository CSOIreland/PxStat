using API;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxStat.Data.PxApiV1;
using PxStat.JsonQuery;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Navigation;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;

namespace PxStat.Data
{

    public class Cube_BSO_ReadPxApiV1
    {
        public IResponseOutput Read(IRequest request)
        {
            
            var lng = LanguageManager.Instance;


            var parameters = ((List<string>)request.parameters).Where(x => !string.IsNullOrWhiteSpace(x));
            int pcount = parameters.Count() - 1;
            PxApiMetadata meta = new PxApiMetadata();
            IResponseOutput rspSbj = null;
            switch (pcount)
            {

                case Constants.C_DATA_PXAPIV1_SUBJECT_QUERY - 1:
                    //Get a list of all subjects - but no language was specified
                    //In that case we add the default language to the url and continue as a subject query

                    request.parameters = new List<string>() { Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") };
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
                    if(rspPrd.data==null)
                    {
                        rspPrd.statusCode = HttpStatusCode.NotFound;
                        return rspPrd;
                    }
                    if(rspPrd.data.Count==0)
                    {
                        RESTful_Output output = new();
                        output.statusCode = HttpStatusCode.NotFound;
                        output.data = null;
                        return output;
                    }
                    rspPrd.response = Utility.JsonSerialize_IgnoreLoopingReference(meta.ReadProductsAsObjectList(rspPrd));
                    return rspPrd;

                case Constants.C_DATA_PXAPIV1_COLLECTION_QUERY:
                    //Get the collection
                    var rspCol = new Cube_BSO_ReadCollectionSummary(request, false).Read().Response;
                    if(rspCol.data==null)
                    {
                        rspCol.statusCode = HttpStatusCode.NotFound;
                        return rspCol;
                    }
                   
                    rspCol.response = Utility.JsonSerialize_IgnoreLoopingReference(meta.ReadCollectionAsObjectList(rspCol));
                    return rspCol;
                default:
                    
                    //validate the request
                    if (pcount < Constants.C_DATA_PXAPIV1_METADATA_QUERY)
                    {
                        throw new Exception(Label.Get("error.validation"));
                    }
                    //If this is a HEAD request, we just check the existence of the matrix
                    if (!request.requestType.Equals("HEAD"))
                    {
                        //Either get the metadata for the table or, if a query has been supplied, get the table data based on the query
                        //string rq = (string)(string.IsNullOrWhiteSpace(request.httpPOST) ? request.httpGET[Constants.C_JSON_STAT_QUERY_CLASS] : request.httpPOST);

                        //This is a PxApiV1 request
                        string rq=null;
                        if (!String.IsNullOrEmpty(request.httpPOST))
                        {
                            rq = request.httpPOST;
                        }
                        else if (request.httpGET.Count>0)
                        {
                            rq = request.httpGET[0];
                        }
                        else
                        {
                            if (!request.parameters[Constants.C_DATA_PXAPIV1_METADATA_QUERY].Contains("?query="))
                            {
                                var rspMeta = new Cube_BSO_ReadMetadataMatrix(request, false).Read().Response;
                                rspMeta.response =rspMeta.data==null? null: Utility.JsonSerialize_IgnoreLoopingReference(rspMeta.data);
                                
                                return rspMeta;
                                
                            }

                            string pxApiParameters = request.parameters[Constants.C_DATA_PXAPIV1_METADATA_QUERY];


                            string[] queryParameters = pxApiParameters.Split("?query=");
                            if (queryParameters.Length < 2)
                            {
                                throw new Exception(Label.Get("error.validation"));
                            }

                            request.parameters[Constants.C_DATA_PXAPIV1_METADATA_QUERY] = queryParameters[0];


                            request.parameters.Add(queryParameters[1]);


                            string coreString = request.parameters[Constants.C_DATA_PXAPIV1_DATA_QUERY].Replace("\"", "");

                            rq = HttpUtility.UrlDecode(coreString);
                        }
                        

                        if (String.IsNullOrEmpty(rq))
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


            using (Cube_BSO cBso = new Cube_BSO(AppServicesHelper.StaticADO))
            {
                collectionPrdSbjdata = cBso.ReadCollection(request.parameters[Constants.C_DATA_PXAPIV1_SUBJECT_QUERY], request.parameters[Constants.C_DATA_PXAPIV1_PRODUCT_QUERY], request.parameters[Constants.C_DATA_PXAPIV1_COLLECTION_QUERY]);
            }

            if (collectionPrdSbjmeta == null)
            {
                var empty = new RESTful_Output();
                empty.statusCode = HttpStatusCode.NotFound;
                return empty;
            }

  

            string queryString = string.IsNullOrWhiteSpace(request.httpPOST) ? (string)request.httpGET[Constants.C_JSON_STAT_QUERY_CLASS] : request.httpPOST;



            dynamic queryParameters = new ExpandoObject();
            queryParameters.queryString = queryString; ;
            queryParameters.LngIsoCode = request.parameters[Constants.C_DATA_PXAPIV1_SUBJECT_QUERY];
            queryParameters.MtrCode = request.parameters[Constants.C_DATA_PXAPIV1_METADATA_QUERY];


            request.parameters = queryParameters;
            IResponseOutput rsp = new Cube_BSO_ReadPxApiV1Bso(request).Read().Response;


            if (rsp.error == Label.Get("error.validation"))
            {
                rsp.response = Label.Get("error.validation");
                rsp.statusCode = HttpStatusCode.BadRequest;
                return rsp;
            }

            if (rsp.error == Label.Get("error.exception"))
            {
                rsp.statusCode = HttpStatusCode.InternalServerError;
                rsp.response = Label.Get("error.exception");
                return rsp;
            }
            if (rsp.data == null)
            {
                rsp.statusCode = HttpStatusCode.NotFound;
                return rsp;
            }

            return rsp;
        }

    }
}
