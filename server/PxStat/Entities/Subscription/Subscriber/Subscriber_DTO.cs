using PxStat.Security;

namespace PxStat.Subscription
{
    internal class Subscriber_DTO_Create
    {
        public string SbrPreference { get; set; }
        public string LngIsoCode { get; set; }
        public string Uid { get; set; }
        public string AccessToken { get; set; }
        public Subscriber_DTO_Create(dynamic parameters)
        {
            if (parameters.SbrPreference != null)
                SbrPreference = parameters.SbrPreference;

            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

            if (parameters.Uid != null)
                Uid = parameters.Uid;
            if (parameters.AccessToken != null)
                AccessToken = parameters.AccessToken;
        }
    }

    internal class Subscriber_DTO_ReadCurrent
    {
        public string Uid { get; set; }
        public string AccessToken { get; set; }

        public Subscriber_DTO_ReadCurrent(dynamic parameters)
        {
            if (parameters.Uid != null)
                Uid = parameters.Uid;
            if (parameters.AccessToken != null)
                AccessToken = parameters.AccessToken;
        }
    }

    internal class Subscriber_DTO_Read
    {
        public string Uid { get; set; }
        public Subscriber_DTO_Read(dynamic parameters)
        {
            if (parameters.Uid != null)
                Uid = parameters.Uid;
        }
    }

    internal class Subscriber_DTO_Update
    {
        public string SbrPreference { get; set; }
        public string LngIsoCode { get; set; }

        public string AccessToken { get; set; }
        public string Uid { get; set; }
        public Subscriber_DTO_Update(dynamic parameters)
        {
            if (parameters.Uid != null)
                Uid = parameters.Uid;
            if (parameters.AccessToken != null)
                AccessToken = parameters.AccessToken;

            if (parameters.SbrPreference != null)
                SbrPreference = parameters.SbrPreference;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;

        }
    }

    internal class Subscriber_DTO_UpdateKey
    {
        public string LngIsoCode { get; set; }
        public string AccessToken { get; set; }
        public string Uid { get; set; }
        public Subscriber_DTO_UpdateKey(dynamic parameters)
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

    internal class Subscriber_DTO_Delete
    {
        public string AccessToken { get; set; }
        public string Uid { get; set; }
        public Subscriber_DTO_Delete(dynamic parameters)
        {
            if (parameters.Uid != null)
                Uid = parameters.Uid;
            if (parameters.AccessToken != null)
                AccessToken = parameters.AccessToken;
        }
    }
}
