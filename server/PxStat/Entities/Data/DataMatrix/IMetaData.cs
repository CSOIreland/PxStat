using PxStat.System.Settings;

namespace PxStat.Data
{
    public interface IMetaData
    {
        string GetFormatType();
        string GetIsOfficialStatistic();
        string GetFrequencyCodes();
        string GetConfigServerJSONPath();
        string GetResourcesMapPath();
        string GetConfigGlobalJSONPath();
        string GetPxDataTimeFormat();
        string GetPxDefaultCharSet();
        string GetPxDefaultAxisVersion();
        string GetPxTrue();
        string GetPxFalse();
        string GetPxMultilineCharLimit();
        string GetCsvStatistic();
        string GetConfidentialValue();
        string GetRegexNoWhitespace();
        string GetBuildRegexForbiddenChars();
        string GetAppCsvValue();
        string GetAppCsvUnit();
        string GetAppRegexAlphaNumeric();
        ICopyright GetCopyright();
        bool IsTest();
        string GetTitleBy();
    }
}
