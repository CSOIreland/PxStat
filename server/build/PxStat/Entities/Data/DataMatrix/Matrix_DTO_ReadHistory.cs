using System;

namespace PxStat.Data
{
    /// <summary>
    /// class
    /// </summary>
    public class Matrix_DTO_ReadHistory
    {
        public DateTime DateFrom { get; internal set; }
        public DateTime DateTo { get; internal set; }
        public string LngIsoCode { get; internal set; }

        public Matrix_DTO_ReadHistory(dynamic parameters)
        {
            if (parameters.DateFrom != null)
                DateFrom = parameters.DateFrom;
            if (parameters.DateTo != null)
                DateTo = parameters.DateTo;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
        }
    }
}
