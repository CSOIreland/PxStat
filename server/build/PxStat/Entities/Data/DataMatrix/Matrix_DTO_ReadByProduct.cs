using PxStat.Security;

namespace PxStat.Data
{
    internal class Matrix_DTO_ReadByProduct
    {
        public string PrcCode { get; internal set; }
        public string LngIsoCode { get; internal set; }
        public bool AssociatedOnly { get; internal set; }

        public Matrix_DTO_ReadByProduct(dynamic parameters)
        {
            if (parameters.PrcCode != null)
            {
                PrcCode = parameters.PrcCode;
            }               
            if (parameters.LngIsoCode != null)
            {
                LngIsoCode = parameters.LngIsoCode;
            }
            else
            {
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            }
            if (parameters.AssociatedOnly != null)
            {
                AssociatedOnly = parameters.AssociatedOnly;
            }
            else
            {
                AssociatedOnly = false;
            }
        }
    }
}
