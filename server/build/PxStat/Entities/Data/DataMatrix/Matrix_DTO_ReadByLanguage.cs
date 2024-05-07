namespace PxStat.Data
{
    internal class Matrix_DTO_ReadByLanguage
    {
        public string LngIsoCode { get; internal set; }


        public Matrix_DTO_ReadByLanguage(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
        }

    }
}
