namespace Px5Migrator
{
    public class PxUpload_DTO : IUpload_DTO
    {

        public string MtrInput { get; set; }
        public bool Overwrite { get; set; }
        public string GrpCode { get; set; }
        public string Signature { get; set; }
        public string CprCode { get; set; }
        public string FrqValueTimeval { get; set; }
        public string FrqCodeTimeval { get; set; }
        public string LngIsoCode { get; set; }
    }
}
