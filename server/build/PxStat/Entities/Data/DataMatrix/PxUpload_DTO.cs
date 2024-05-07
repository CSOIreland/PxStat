
using API;
using PxStat.Data;
using PxStat.Security;

namespace PxStat.Data
{
    public class PxUpload_DTO : IUpload_DTO
    {
        [NoTrim]
        [DefaultSanitizer]
        public string MtrInput { get; set; }
        public bool Overwrite { get; set; }
        public string GrpCode { get; set; }
        public string Signature { get; set; }
        public string CprCode { get; set; }
        public string FrqValueTimeval { get; set; }
        public string FrqCodeTimeval { get; set; }
        public string LngIsoCode { get; set; }


        internal Signature_DTO GetSignatureDTO()
        {
            return new Signature_DTO
            {
                MtrInput = this.MtrInput,
                FrqCodeTimeval = this.FrqCodeTimeval,
                FrqValueTimeval = this.FrqValueTimeval

            };



        }



        /// <summary>
        /// Used for serialization/deserialization
        /// </summary>
        public PxUpload_DTO() { }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="parameters"></param>
        public PxUpload_DTO(dynamic parameters)
        {
            GrpCode = parameters["GrpCode"];

            if (parameters["MtrInput"] != null)
            {
                MtrInput = Utility.DecodeBase64ToUTF8((string)parameters["MtrInput"]);
            }

            if (parameters.Overwrite != null)
                this.Overwrite = parameters.Overwrite;

            if (parameters.Signature != null)
                this.Signature = parameters.Signature;

            if (parameters.CprCode != null)
                this.CprCode = parameters.CprCode;
            else
                this.CprCode = Configuration_BSO.GetStaticConfig("APP_DEFAULT_SOURCE");

            if (parameters.FrqValueTimeval != null)
                this.FrqValueTimeval = parameters.FrqValueTimeval;

            if (parameters.FrqCodeTimeval != null)
                this.FrqCodeTimeval = parameters.FrqCodeTimeval;

            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");


        }
    }

}
