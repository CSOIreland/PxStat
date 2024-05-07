using PxStat.Security;

namespace PxStat.Data
{

    public class PxData : IPxData
    {
        public string GetFormatType()
        {
            return Configuration_BSO.GetStaticConfig("APP_PX_DEFAULT_FORMAT");
        }

        public string GetIsOfficialStatistic()
        {
            return Configuration_BSO.GetStaticConfig("APP_PX_TRUE");
        }

        public string GetFrequencyCodes()
        {
            return Configuration_BSO.GetStaticConfig("APP_PX_FREQUENCY_CODES");
        }
    }
}
