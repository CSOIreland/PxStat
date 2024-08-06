using API;
using PxStat.DBuild;
using PxStat.Security;

namespace PxStat.Entities.DBuild
{


    public class DBuild_DTO_ReadTemplateByRelease
    {
        [NoTrim]
        [NoHtmlStrip]
        [DefaultSanitizer]
        public int RlsCode { get; set; }
        public string LngIsoCode { get; set; }
        public string FrqValueTimeval { get; set; }
        public string FrqCodeTimeval { get; set; }

        public DBuild_DTO_ReadTemplateByRelease(dynamic parameters)
        {
            if (parameters.RlsCode != null)
                RlsCode = parameters.RlsCode;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            if (parameters.FrqCodeTimeval != null)
                FrqCodeTimeval = parameters.FrqCodeTimeval;
            if (parameters.FrqValueTimeval != null)
                FrqValueTimeval = parameters.FrqValueTimeval;
        }
    }
}
