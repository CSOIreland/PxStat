using PxStat.Security;

namespace PxStat.Data
{
    internal class Matrix_DTO_ReadByGeoMap
    {
        public string GmpCode { get; internal set; }
        public string LngIsoCode { get; internal set; }



        public Matrix_DTO_ReadByGeoMap(dynamic parameters)
        {


            if (parameters.GmpCode != null)
                GmpCode = parameters.GmpCode;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }
    }
}
