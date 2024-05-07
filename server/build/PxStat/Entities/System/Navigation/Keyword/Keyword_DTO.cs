using PxStat.Security;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// DTO for Keyword s
    /// </summary>
    internal class Keyword_DTO_Search
    {
        /// <summary>
        /// Search term
        /// </summary>
        public string SearchTerm { get; set; }

        /// <summary>
        /// ISO Language code
        /// </summary>
        [LowerCase]
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Keyword_DTO_Search(dynamic parameters)
        {
            if (parameters.SearchTerm != null)
                SearchTerm = parameters.SearchTerm;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }
    }

    /// <summary>
    /// No constructor used - values are filled from AutoMapper
    /// </summary>
    public class Keyword_DTO_ReadSynonym
    {
        private string _lng;
        [LowerCase]
        public string KrlValue { get; set; }

        public string LngIsoCode { get { return _lng; } set { _lng = value ?? Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code"); } }

    }
    
}
