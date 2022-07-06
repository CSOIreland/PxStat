namespace PxStat.Data
{

    public class MetaData : IMetaData
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

        public string GetConfigServerJSONPath()
        {
            return API.Utility.GetCustomConfig("APP_CONFIG_SERVER_JSON_PATH");
        }

        public string GetResourcesMapPath()
        {
            return API.Utility.GetCustomConfig("APP_CONFIG_RESOURCES_MAP_PATH");
        }

        public string GetConfigGlobalJSONPath()
        {
            return API.Utility.GetCustomConfig("APP_CONFIG_GLOBAL_JSON_PATH");
        }

        public string GetPxDataTimeFormat()
        {
            return API.Utility.GetCustomConfig("APP_PX_DATE_TIME_FORMAT");
        }
        public string GetPxDefaultCharSet()
        {
            return API.Utility.GetCustomConfig("APP_PX_DEFAULT_CHARSET");
        }
        public string GetPxDefaultAxisVersion()
        {
            return API.Utility.GetCustomConfig("APP_PX_DEFAULT_AXIS_VERSION");
        }

        public string GetPxTrue()
        {
            return API.Utility.GetCustomConfig("APP_PX_TRUE");
        }

        public string GetPxFalse()
        {
            return API.Utility.GetCustomConfig("APP_PX_FALSE");
        }

        public string GetPxMultilineCharLimit()
        {
            return API.Utility.GetCustomConfig("APP_PX_MULTILINE_CHAR_LIMIT");
        }

        public string GetCsvStatistic()
        {
            return API.Utility.GetCustomConfig("APP_CSV_STATISTIC");
        }

        public string GetConfidentialValue()
        {
            return API.Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE");
        }
    }
}
