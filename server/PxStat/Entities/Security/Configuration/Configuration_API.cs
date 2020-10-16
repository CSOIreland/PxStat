using API;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;

namespace PxStat.Security
{
    internal class Configuration_API
    {

        public static dynamic Refresh(JSONRPC_API requestApi)
        {
            return new Configuration_BSO_Refresh(requestApi).Update().Response;
        }

        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Configuration_BSO_Read(requestApi).Read().Response;
        }

        public static dynamic Read(RESTful_API restfulRequestApi)
        {
            //Map the RESTful request to an equivalent Json Rpc request
            JSONRPC_API jsonRpcRequest = Map.RESTful2JSONRPC_API(restfulRequestApi);
            //Run the request as a Json Rpc call

            dynamic obj = new ExpandoObject();
            var dict = (IDictionary<string, object>)obj;

            jsonRpcRequest.parameters = Utility.JsonSerialize_IgnoreLoopingReference(dict);
            JSONRPC_Output rsp = new Configuration_BSO_Read(jsonRpcRequest).Read().Response;


            RESTful_Output resp = Map.JSONRPC2RESTful_Output(rsp, null, rsp.data == null ? HttpStatusCode.NotFound : HttpStatusCode.OK);

            resp.response = Utility.JsonSerialize_IgnoreLoopingReference(rsp.data);

            return resp;
        }
    }
}
