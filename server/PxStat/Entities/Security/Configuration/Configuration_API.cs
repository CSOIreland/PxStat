using API;
namespace PxStat.Security
{
    internal class Configuration_API
    {

        public static dynamic Refresh(JSONRPC_API requestApi)
        {
            return new Configuration_BSO_Refresh(requestApi).Update().Response;
        }
    }
}
