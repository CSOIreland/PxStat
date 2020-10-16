using PxStat.Security;

namespace PxStat.Data
{
    /// <summary>
    /// Class
    /// </summary>
    internal class Compare_DTO_Read
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Language ISO Code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="parameters"></param>
        public Compare_DTO_Read(dynamic parameters)
        {
            if (parameters.RlsCode != null)
                this.RlsCode = parameters.RlsCode;

            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

        }
        /// <summary>
        /// Blank constructor
        /// </summary>
        public Compare_DTO_Read()
        {
        }
    }

    /// <summary>
    /// Class
    /// </summary>
    internal class Compare_DTO_ReadPrevious
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Compare_DTO_ReadPrevious(dynamic parameters)
        {
            if (parameters.RlsCode != null)
                this.RlsCode = parameters.RlsCode;
        }
    }


}
