using PxStat.Security;

namespace PxStat.Subscription
{
    internal class Channel_DTO_Read
    {
        public string ChnCode { get; set; }
        public string LngIsoCode { get; set; }

        public Channel_DTO_Read(dynamic parameters)
        {
            if (parameters.ChnCode != null)
                ChnCode = parameters.ChnCode;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }
    }
}
