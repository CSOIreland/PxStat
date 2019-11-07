using API;

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
            else LngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");
        }
    }
}