using API;
using System.Net;

namespace PxStat.Data
{
    [AllowAPICall]
    public class GeoMap_API
    {
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {
            return new GeoMap_BSO_Create(jsonrpcRequest).Create().Response;
        }

        public static dynamic Update(JSONRPC_API jsonrpcRequest)
        {
            return new GeoMap_BSO_Update(jsonrpcRequest).Update().Response;
        }


        public static dynamic Delete(JSONRPC_API jsonrpcRequest)
        {
            return new GeoMap_BSO_Delete(jsonrpcRequest).Delete().Response;
        }

        public static dynamic Read(Static_API staticRequest)
        {

            GeoMap_BSO_Read map = new GeoMap_BSO_Read();
            if (staticRequest.parameters.Count < 2)
            {
                Static_Output output = new Static_Output() { };
                output.statusCode = HttpStatusCode.BadRequest;
                return output;
            }

            return map.Read(staticRequest);

        }

        public static dynamic ReadCollection(JSONRPC_API jsonrpcRequest)
        {
            return new GeoMap_BSO_ReadCollection(jsonrpcRequest).Read().Response;
        }

        public static dynamic Validate(JSONRPC_API jsonrpcRequest)
        {
            return new GeoMap_BSO_Validate(jsonrpcRequest).Read().Response;
        }
    }
}
