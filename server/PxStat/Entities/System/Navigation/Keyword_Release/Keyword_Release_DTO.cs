using PxStat.Security;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// DTO for Keyword Release
    /// </summary>
    internal class Keyword_Release_DTO
    {
        #region Properties
        /// <summary>
        /// Keyword Release Value
        /// </summary>
        public string KrlValue { get; set; }

        /// <summary>
        /// Keyword Release Mandatory flag
        /// </summary>
        public bool KrlMandatoryFlag { get; set; }

        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Keyword Release Code
        /// </summary>
        public int KrlCode { get; set; }

        /// <summary>
        /// Iso Language code. Sent if this keyword is to be singularised.
        /// </summary>
        public string LngIsoCode { get; set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Keyword_Release_DTO(dynamic parameters)
        {
            if (parameters.KrlValue != null)
                KrlValue = parameters.KrlValue;
            if (parameters.KrlMandatoryFlag != null)
                KrlMandatoryFlag = parameters.KrlMandatoryFlag;
            if (parameters.RlsCode != null)
                this.RlsCode = parameters.RlsCode;
            if (parameters.KrlCode != null)
                this.KrlCode = parameters.KrlCode;
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public Keyword_Release_DTO() { }
    }
}
