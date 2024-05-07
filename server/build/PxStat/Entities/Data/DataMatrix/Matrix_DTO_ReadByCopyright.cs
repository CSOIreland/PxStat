using PxStat.Security;

namespace PxStat.Data
{
    internal class Matrix_DTO_ReadByCopyright
    {
        public string CprCode { get; internal set; }
        public string LngIsoCode { get; internal set; }


        public Matrix_DTO_ReadByCopyright(dynamic parameters)
        {
            if (parameters.CprCode != null)
                CprCode = parameters.CprCode;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }

    }
}
