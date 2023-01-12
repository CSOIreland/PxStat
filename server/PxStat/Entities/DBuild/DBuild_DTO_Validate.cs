using API;

namespace PxStat.DBuild
{
    public class DBuild_DTO_Validate
    {
        [NoTrim]
        [NoHtmlStrip]
        [DefaultSanitizer]
        public string MtrInput { get; set; }
        public string FrqCodeTimeval { get; set; }
        public string FrqValueTimeval { get; set; }
        public string Signature { get; set; }

        public DBuild_DTO_Validate(dynamic parameters)
        {
            if (parameters.MtrInput != null)
                this.MtrInput = Utility.DecodeBase64ToUTF8((string)parameters["MtrInput"]);
            if (parameters.FrqCodeTimeval != null)
                FrqCodeTimeval = parameters.FrqCodeTimeval;
            if (parameters.FrqValueTimeval != null)
                FrqValueTimeval = parameters.FrqValueTimeval;
            if (parameters.Signature != null)
                Signature = parameters.Signature;
        }
        internal Signature_DTO GetSignatureDTO()
        {
            return new Signature_DTO
            {
                MtrInput = this.MtrInput,
                FrqCodeTimeval = this.FrqCodeTimeval,
                FrqValueTimeval = this.FrqValueTimeval
            };
        }
    }
}
