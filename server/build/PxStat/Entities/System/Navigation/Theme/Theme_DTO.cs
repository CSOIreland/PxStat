using PxStat.Security;

namespace PxStat.System.Navigation
{
    public class Theme_DTO_Delete
    {
        public int ThmCode { get; set; }

        public Theme_DTO_Delete(dynamic parameters)
        {
            if (parameters.ThmCode != null)
                ThmCode = parameters.ThmCode;
        }
    }
    public class Theme_DTO_Create
    {
        public string LngIsoCode { get; set; }
        public string ThmValue { get; set; }

        public Theme_DTO_Create(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            if (parameters.ThmValue != null)
                ThmValue = parameters.ThmValue;
        }
    }

    public class Theme_DTO_Read
    {
        public string LngIsoCode { get; set; }
        public int ThmCode { get; set; }

        public string ThmValue { get; set; }

        public Theme_DTO_Read(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            if (parameters.ThmCode != null)
                ThmCode = parameters.ThmCode;
            if (parameters.ThmValue != null)
                ThmValue = parameters.ThmValue;
        }

        public Theme_DTO_Read()
        {
        }
    }

    public class Theme_DTO_Update
    {
        public string LngIsoCode { get; set; }
        public int ThmCode { get; set; }

        public string ThmValue { get; set; }

        public Theme_DTO_Update(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            if (parameters.ThmCode != null)
                ThmCode = parameters.ThmCode;
            if (parameters.ThmValue != null)
                ThmValue = parameters.ThmValue;
        }

    }
}
