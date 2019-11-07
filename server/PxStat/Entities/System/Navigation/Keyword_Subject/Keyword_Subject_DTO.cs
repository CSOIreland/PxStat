namespace PxStat.System.Navigation
{
    /// <summary>
    /// DTO for Keyword Subject
    /// </summary>
    internal class Keyword_Subject_DTO
    {
        /// <summary>
        /// Subject Code
        /// </summary>
        public int SbjCode { get; set; }

        /// <summary>
        /// Keyword Subject Code
        /// </summary>
        public int KsbCode { get; set; }

        /// <summary>
        /// Keyword Subject Value
        /// </summary>
        public string KsbValue { get; set; }

        /// <summary>
        /// Keyword Subject Mandatory Flag
        /// </summary>
        public bool? KsbMandatoryFlag { get; set; }

        public string LngIsoCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Keyword_Subject_DTO(dynamic parameters)
        {
            if (parameters.SbjCode != null)
                this.SbjCode = parameters.SbjCode;

            if (parameters.KsbCode != null)
                this.KsbCode = parameters.KsbCode;

            if (parameters.KsbValue != null)
                this.KsbValue = parameters.KsbValue;

            if (parameters.KsbMandatoryFlag != null)
                this.KsbMandatoryFlag = parameters.KsbMandatoryFlag;
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;

        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public Keyword_Subject_DTO() { }
    }
}
