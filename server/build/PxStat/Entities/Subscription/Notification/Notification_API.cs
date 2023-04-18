using API;

namespace PxStat.Subscription
{
    [AllowAPICall]
    public class NotificationChannel_API
    {
        public static dynamic Send(JSONRPC_API requestApi)
        {
            return new NotificationChannel_BSO_Create(requestApi).Create().Response;
        }

    }
}
