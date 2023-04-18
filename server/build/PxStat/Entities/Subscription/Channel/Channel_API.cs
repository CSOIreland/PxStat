using API;

namespace PxStat.Subscription
{
    [AllowAPICall]
    public class Channel_API
    {
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Channel_BSO_Read(requestApi).Read().Response;
        }
    }
}
