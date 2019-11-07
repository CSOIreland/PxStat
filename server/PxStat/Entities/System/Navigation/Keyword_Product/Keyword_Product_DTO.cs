namespace PxStat.System.Navigation
{
    /// <summary>
    /// DTO for Keyword Product
    /// </summary>
    internal class Keyword_Product_DTO
    {
        #region Properties
        /// <summary>
        /// Product Code
        /// </summary>
        public string PrcCode { get; set; }

        /// <summary>
        /// Subject Code
        /// </summary>
        public int SbjCode { get; set; }

        /// <summary>
        /// Keyword Product Code
        /// </summary>
        public int KprCode { get; set; }

        /// <summary>
        /// Keyword Product value
        /// </summary>
        public string KprValue { get; set; }

        /// <summary>
        /// Keyword Product Mandatory flag
        /// </summary>
        public bool? KprMandatoryFlag { get; set; }

        public string LngIsoCode { get; set; }
        #endregion

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Keyword_Product_DTO(dynamic parameters)
        {
            if (parameters.PrcCode != null)
                this.PrcCode = parameters.PrcCode;

            if (parameters.KprCode != null)
                this.KprCode = parameters.KprCode;

            if (parameters.KprValue != null)
                this.KprValue = parameters.KprValue;

            if (parameters.KprMandatoryFlag != null)
                this.KprMandatoryFlag = parameters.KprMandatoryFlag;

            if (parameters.SbjCode != null)
                this.SbjCode = parameters.SbjCode;

            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;

        }

        /// <summary>
        /// blank constructor
        /// </summary>
        public Keyword_Product_DTO() { }
    }
}
