using API;

namespace PxStat.Subscription
{
    [AllowAPICall]
    public class Query_API
    {
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new Query_BSO_Create(requestApi).Create().Response;
        }

        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new Query_BSO_Delete(requestApi).Delete().Response;
        }

        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Query_BSO_Read(requestApi).Read().Response;
        }
        public static dynamic ReadAll(JSONRPC_API requestApi)
        {
            return new Query_BSO_ReadAll(requestApi).Read().Response;
        }
    }
}
