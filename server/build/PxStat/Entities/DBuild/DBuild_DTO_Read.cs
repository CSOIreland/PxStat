using API;

namespace PxStat.DBuild
{
    public class DBuild_DTO_Read
    {
        /// <summary>
        /// 
        /// </summary>
        /// 
        [NoTrim]
        [NoHtmlStrip]
        [DefaultSanitizer]
        public string MtrInput { get; internal set; }
        public string FrqCodeTimeval { get; internal set; }
        public string FrqValueTimeval { get; internal set; }
        public string Signature { get; internal set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public DBuild_DTO_Read(dynamic parameters)
        {
            if (parameters.MtrInput != null)
                MtrInput = Utility.DecodeBase64ToUTF8((string)parameters["MtrInput"]);
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
