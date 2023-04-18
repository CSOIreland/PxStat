using API;

namespace PxStat.Data
{
    [AllowAPICall]
    public class GeoLayer_API
    {
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {
            return new GeoLayer_BSO_Create(jsonrpcRequest).Create().Response;
        }

        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new GeoLayer_BSO_Read(jsonrpcRequest).Read().Response;
        }

        public static dynamic Update(JSONRPC_API jsonrpcRequest)
        {
            return new GeoLayer_BSO_Update(jsonrpcRequest).Update().Response;
        }

        public static dynamic Delete(JSONRPC_API jsonrpcRequest)
        {
            return new GeoLayer_BSO_Delete(jsonrpcRequest).Delete().Response;
        }
    }
}
