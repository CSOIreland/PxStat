using PxStat.Security;

namespace PxStat.Subscription
{
    internal class Subscription_DTO_ChannelSubscriptionCreate
    {
        public string ChnCode { get; set; }
        public string Uid { get; set; }
        public string AccessToken { get; set; }

        public Subscription_DTO_ChannelSubscriptionCreate(dynamic parameters)
        {
            if (parameters.ChnCode != null)
                ChnCode = parameters.ChnCode;
            if (parameters.Uid != null)
                Uid = parameters.Uid;
            if (parameters.AccessToken != null)
                AccessToken = parameters.AccessToken;
        }
    }

    internal class Subscription_DTO_ChannelSubscriptionDelete
    {
        public string ChnCode { get; set; }
        public string Uid { get; set; }
        public string AccessToken { get; set; }

        public Subscription_DTO_ChannelSubscriptionDelete(dynamic parameters)
        {
            if (parameters.ChnCode != null)
                ChnCode = parameters.ChnCode;
            if (parameters.Uid != null)
                Uid = parameters.Uid;
            if (parameters.AccessToken != null)
                AccessToken = parameters.AccessToken;
        }
    }

    internal class Subscription_DTO_ChannelSubscriptionReadCurrent
    {
        public string Uid { get; set; }
        public string AccessToken { get; set; }
        public string LngIsoCode { get; set; }
        public Subscription_DTO_ChannelSubscriptionReadCurrent(dynamic parameters)
        {
            if (parameters.Uid != null)
                Uid = parameters.Uid;
            if (parameters.AccessToken != null)
                AccessToken = parameters.AccessToken;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
        }
    }

    internal class Subscription_DTO_TableSubscriptionCreate
    {
        public string TsbTable { get; set; }
        public string Uid { get; set; }
        public string AccessToken { get; set; }

        public Subscription_DTO_TableSubscriptionCreate(dynamic parameters)
        {
            if (parameters.TsbTable != null)
                TsbTable = parameters.TsbTable;
            if (parameters.Uid != null)
                Uid = parameters.Uid;
            if (parameters.AccessToken != null)
                AccessToken = parameters.AccessToken;
        }
    }

    internal class Subscription_DTO_TableSubscriptionDelete
    {
        public string TsbTable { get; set; }
        public string Uid { get; set; }
        public string AccessToken { get; set; }

        public Subscription_DTO_TableSubscriptionDelete(dynamic parameters)
        {
            if (parameters.TsbTable != null)
                TsbTable = parameters.TsbTable;
            if (parameters.Uid != null)
                Uid = parameters.Uid;
            if (parameters.AccessToken != null)
                AccessToken = parameters.AccessToken;
        }
    }

    internal class Subscription_DTO_TableSubscriptionReadCurrent
    {
        public string Uid { get; set; }
        public string AccessToken { get; set; }
        public Subscription_DTO_TableSubscriptionReadCurrent(dynamic parameters)
        {
            if (parameters.Uid != null)
                Uid = parameters.Uid;
            if (parameters.AccessToken != null)
                AccessToken = parameters.AccessToken;
        }
    }

    internal class Subscription_DTO_TableSubscriptionRead
    {
        public string TsbTable { get; set; }
        public Subscription_DTO_TableSubscriptionRead(dynamic parameters)
        {
            if (parameters.TsbTable != null)
                TsbTable = parameters.TsbTable;
        }
    }

    internal class Subscription_DTO_ChannelSubscriptionRead
    {
        public string ChnCode { get; set; }
        public string SbrUserId { get; set; }

        public string CcnUsername { get; set; }
        public string LngIsoCode { get; set; }

        public Subscription_DTO_ChannelSubscriptionRead(dynamic parameters)
        {
            if (parameters.SbrUserId != null)
                SbrUserId = parameters.SbrUserId;
            if (parameters.CcnUsername != null)
                CcnUsername = parameters.CcnUsername;
            if (parameters.ChnCode != null)
                ChnCode = parameters.ChnCode;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

        }
    }
}
