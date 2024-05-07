using PxStat.Security;

namespace PxStat.Data
{
    /// <summary>
    /// class
    /// </summary>
    internal class Matrix_DTO_Read
    {
        public string MtrCode { get; set; }

        public int RlsCode { get; set; }

        public string LngIsoCode { get; set; }

        public Matrix_DTO_Read(dynamic parameters)
        {

            if (parameters.MtrCode != null)
                this.MtrCode = parameters.MtrCode;

            if (parameters.RlsCode != null)
                this.RlsCode = parameters.RlsCode;

            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

        }
    }
}
