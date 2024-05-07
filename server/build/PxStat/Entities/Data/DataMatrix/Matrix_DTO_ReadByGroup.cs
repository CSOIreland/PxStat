using PxStat.Security;

namespace PxStat.Data
{
    internal class Matrix_DTO_ReadByGroup
    {
        public string GrpCode { get; internal set; }
        public string LngIsoCode { get; internal set; }


        public Matrix_DTO_ReadByGroup(dynamic parameters)
        {
            if (parameters.GrpCode != null)
                GrpCode = parameters.GrpCode;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }

    }
}
