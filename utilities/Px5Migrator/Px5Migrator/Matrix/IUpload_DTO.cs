namespace Px5Migrator
{
    public interface IUpload_DTO
    {
        string CprCode { get; set; }
        string FrqCodeTimeval { get; set; }
        string FrqValueTimeval { get; set; }
        string GrpCode { get; set; }
        string LngIsoCode { get; set; }
        string MtrInput { get; set; }
        bool Overwrite { get; set; }
        string Signature { get; set; }
    }
}
