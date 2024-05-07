using PxStat.Security;
using PxStat.System.Settings;

namespace PxStat.Data
{

    public class MetaData : IMetaData
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

        public string GetConfigServerJSONPath()
        {
            return Configuration_BSO.GetStaticConfig("APP_CONFIG_SERVER_JSON_PATH");
        }

        public string GetResourcesMapPath()
        {
            return Configuration_BSO.GetStaticConfig("APP_CONFIG_RESOURCES_MAP_PATH");
        }

        public string GetConfigGlobalJSONPath()
        {
            return Configuration_BSO.GetStaticConfig("APP_CONFIG_GLOBAL_JSON_PATH");
        }

        public string GetPxDataTimeFormat()
        {
            return Configuration_BSO.GetStaticConfig("APP_PX_DATE_TIME_FORMAT");
        }
        public string GetPxDefaultCharSet()
        {
            return Configuration_BSO.GetStaticConfig("APP_PX_DEFAULT_CHARSET");
        }
        public string GetPxDefaultAxisVersion()
        {
            return Configuration_BSO.GetStaticConfig("APP_PX_DEFAULT_AXIS_VERSION");
        }

        public string GetPxTrue()
        {
            return Configuration_BSO.GetStaticConfig("APP_PX_TRUE");
        }

        public string GetPxFalse()
        {
            return Configuration_BSO.GetStaticConfig("APP_PX_FALSE");
        }

        public string GetPxMultilineCharLimit()
        {
            return Configuration_BSO.GetStaticConfig("APP_PX_MULTILINE_CHAR_LIMIT");
        }

        public string GetCsvStatistic()
        {
            return Configuration_BSO.GetStaticConfig("APP_CSV_STATISTIC");
        }

        public string GetConfidentialValue()
        {
            return Configuration_BSO.GetStaticConfig("APP_PX_CONFIDENTIAL_VALUE");
        }
        public string GetRegexNoWhitespace()
        {
            return Configuration_BSO.GetStaticConfig("APP_REGEX_NO_WHITESPACE");
        }
        public string GetBuildRegexForbiddenChars()
        {
            return Configuration_BSO.GetStaticConfig("APP_BUILD_REGEX_FORBIDDEN_CHARS");
        }

        public bool IsTest()
        {
            return false;
        }

        public string GetAppCsvValue()
        {
            return Configuration_BSO.GetStaticConfig("APP_CSV_VALUE");
        }

        public string GetAppCsvUnit()
        {
            return Configuration_BSO.GetStaticConfig("APP_CSV_UNIT");
        }

        public string GetAppRegexAlphaNumeric()
        {
            return Configuration_BSO.GetStaticConfig("APP_REGEX_ALPHA_NUMERIC");
        }

        public ICopyright GetCopyright()
        {
            return new Copyright_DTO_Read() { CprCode = "DHA", CprValue = "Department of Health and Children", CprUrl = "https://www.gov.ie/en/organisation/department-of-health/" };
        }

        public string GetTitleBy()
        {
            return Configuration_BSO.GetStaticConfig("APP_PX_TITLE_BY");
        }

        public string GetDefaultCodingLanguage()
        {
            return Configuration_BSO.GetStaticConfig("APP_DEFAULT_CODING_LANGUAGE");
        }
    }
}
