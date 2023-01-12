using API;

namespace PxStat.Subscription
{
    [AllowAPICall]
    public class Subscriber_API
    {
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new Subscriber_BSO_Create(requestApi).Create().Response;
        }

        public static dynamic ReadCurrent(JSONRPC_API requestApi)
        {
            return new Subscriber_BSO_ReadCurrent(requestApi).Read().Response;
        }

        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Subscriber_BSO_Read(requestApi).Read().Response;
        }

        public static dynamic Update(JSONRPC_API requestApi)
        {
            return new Subscriber_BSO_Update(requestApi).Update().Response;
        }

        public static dynamic UpdateKey(JSONRPC_API requestApi)
        {
            return new Subscriber_BSO_UpdateKey(requestApi).Update().Response;
        }

        public static dynamic Logout(JSONRPC_API requestApi)
        {
            return new Subscriber_BSO_Logout(requestApi).Update().Response;
        }

        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new Subscriber_BSO_Delete(requestApi).Delete().Response;
        }
    }
}
