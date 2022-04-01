using API;

namespace PxStat.Subscription
{
    public class Subscription_API
    {
        public static dynamic TableSubscriptionCreate(JSONRPC_API requestApi)
        {
            return new Subscription_BSO_TableSubscriptionCreate(requestApi).Create().Response;
        }

        public static dynamic TableSubscriptionDelete(JSONRPC_API requestApi)
        {
            return new Subscription_BSO_TableSubscription_Delete(requestApi).Delete().Response;
        }

        public static dynamic TableSubscriptionReadCurrent(JSONRPC_API requestApi)
        {
            return new Subscription_BSO_TableSubscription_ReadCurrent(requestApi).Read().Response;
        }

        public static dynamic TableSubscriptionRead(JSONRPC_API requestApi)
        {
            return new Subscription_BSO_TableSubscription_Read(requestApi).Read().Response;
        }



        public static dynamic ChannelSubscriptionCreate(JSONRPC_API requestApi)
        {
            return new Subscription_BSO_ChannelSubscription_Create(requestApi).Create().Response;
        }

        public static dynamic ChannelSubscriptionDelete(JSONRPC_API requestApi)
        {
            return new Subscription_BSO_ChannelSubscription_Delete(requestApi).Delete().Response;
        }

        public static dynamic ChannelSubscriptionReadCurrent(JSONRPC_API requestApi)
        {
            return new Subscription_BSO_ChannelSubscriptionReadCurrent(requestApi).Read().Response;
        }

        public static dynamic ChannelSubscriptionRead(JSONRPC_API requestApi)
        {
            return new Subscription_BSO_ChannelSubscription_Read(requestApi).Read().Response;
        }
    }
}
