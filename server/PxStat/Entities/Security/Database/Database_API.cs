using API;

namespace PxStat.Security
{    /// <summary>
     /// API for reading Database Fragmentation Statistics
     /// </summary>
    [AllowAPICall]
    public class Database_API
    {
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Database_BSO_Read(jsonrpcRequest).Read().Response;
        }
        public static dynamic Update(JSONRPC_API requestApi)
        {
            return new Database_BSO_Update(requestApi).Read().Response;
        }
    }
}
