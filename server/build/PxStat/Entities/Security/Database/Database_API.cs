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
    }
}
