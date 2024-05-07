using API;
using PxStat.DBuild;
using PxStat.Security;

namespace PxStat.Entities.DBuild
{


    public class DBuild_DTO_ReadTemplate
    {
        [NoTrim]
        [NoHtmlStrip]
        [DefaultSanitizer]
        public string MtrInput { get; set; }
        public string LngIsoCode { get; set; }
        public string FrqValueTimeval { get; set; }
        public string FrqCodeTimeval { get; set; }
        public string Signature { get; set; }

        public DBuild_DTO_ReadTemplate(dynamic parameters)
        {
            if (parameters.MtrInput != null)
                MtrInput = Utility.DecodeBase64ToUTF8((string)parameters["MtrInput"]);
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            if (parameters.FrqCodeTimeval != null)
                FrqCodeTimeval = parameters.FrqCodeTimeval;
            if (parameters.FrqValueTimeval != null)
                FrqValueTimeval = parameters.FrqValueTimeval;
            if (parameters.Signature != null)
                Signature = parameters.Signature;
        }
        internal Signature_DTO GetSignatureDTO()
        {
            // PxUpdate_DTO dto = Utility.JsonDeserialize_IgnoreLoopingReference<PxUpdate_DTO>(Utility.JsonSerialize_IgnoreLoopingReference(this));
            return new Signature_DTO
            {
                MtrInput = this.MtrInput,
                FrqCodeTimeval = this.FrqCodeTimeval,
                FrqValueTimeval = this.FrqValueTimeval
            };


        }

    }
}
