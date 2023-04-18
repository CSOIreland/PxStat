namespace PxStat.Data
{

    public class PxData : IPxData
    {
        public string GetFormatType()
        {
            return API.Utility.GetCustomConfig("APP_PX_DEFAULT_FORMAT");
        }

        public string GetIsOfficialStatistic()
        {
            return API.Utility.GetCustomConfig("APP_PX_TRUE");
        }

        public string GetFrequencyCodes()
        {
            return API.Utility.GetCustomConfig("APP_PX_FREQUENCY_CODES");
        }
    }
}
