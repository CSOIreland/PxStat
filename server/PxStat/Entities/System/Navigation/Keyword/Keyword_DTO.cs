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
            else LngIsoCode = Configuration_BSO.GetCustomConfig("language.iso.code");
        }
    }

    public class Keyword_DTO_ReadSynonym
    {
        [LowerCase]
        public string KrlValue { get; set; }
        public string LngIsoCode { get; set; }

        public Keyword_DTO_ReadSynonym(dynamic parameters)
        {
            if (parameters.KrlValue != null)
                this.KrlValue = parameters.KrlValue;
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
        }
    }
}
