using PxStat.Security;

namespace PxStat.Subscription
{
    internal class Notification_DTO_Create
    {
        public string LngIsoCode { get; set; }
        public string ChnCode { get; set; }
        public string EmailBody { get; set; }
        public string EmailSubject { get; set; }
        public bool singleLanguage { get; set; }
        public Notification_DTO_Create(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
            {
                LngIsoCode = parameters.LngIsoCode;
                singleLanguage = true;
            }
            else
            {
                LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
                singleLanguage = false;
            }

            if (parameters.ChnCode != null)
            {
                ChnCode = parameters.ChnCode;
            }

            if (parameters.EmailBody != null)
                EmailBody = parameters.EmailBody;

            if (parameters.EmailSubject != null)
                EmailSubject = parameters.EmailSubject;
        }

        public Notification_DTO_Create()
        {
        }
    }
}
