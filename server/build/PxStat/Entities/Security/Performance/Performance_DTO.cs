using System;

namespace PxStat.Security
{
    /// <summary>
    /// DTO for performance read
    /// </summary>
    internal class Performance_DTO_Read
    {
        /// <summary>
        /// Date and time of first performance entry
        /// </summary>
        public DateTime PrfDatetimeStart { get; set; }

        /// <summary>
        /// Date and time of last performance entry
        /// </summary>
        public DateTime PrfDatetimeEnd { get; set; }
        /// <summary>
        /// ISO Language Code
        /// </summary>
        public string LngIsoCode { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Performance_DTO_Read(dynamic parameters)
        {
            if (parameters.PrfDatetimeStart != null)
                PrfDatetimeStart = parameters.PrfDatetimeStart;
            if (parameters.PrfDatetimeEnd != null)
                PrfDatetimeEnd = parameters.PrfDatetimeEnd;
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
        }
    }
    internal class Performance_DTO_Delete
    {
        /// <summary>
        /// ISO Language Code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Performance_DTO_Delete(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

        }
    }
}
