using API;
namespace PxStat.Security
{
    internal class Cache_API
    {

        public static dynamic FlushAll(JSONRPC_API requestApi)
        {
            return new Cache_BSO_FlushAll(requestApi).Update().Response;
        }


        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Cache_BSO_Read(requestApi).Read().Response;
        }
    }
}
