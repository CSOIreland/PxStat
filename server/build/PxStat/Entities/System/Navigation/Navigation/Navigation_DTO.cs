using PxStat.Security;
using System.Data;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// DTO for Navigation Read
    /// </summary>
    internal class Navigation_DTO_Read
    {
        /// <summary>
        /// ISO Language Code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Navigation_DTO_Read(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }
    }

    /// <summary>
    /// DTO for Search
    /// </summary>
    internal class Navigation_DTO_Search
    {
        #region Properties
        /// <summary>
        /// ISO Language Code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Matrix Code
        /// </summary>
        public string MtrCode { get; set; }//*** Not used by the Front End yet **

        /// <summary>
        /// Official flag
        /// </summary>
        public bool? MtrOfficialFlag { get; set; }//*** Not used by the Front End yet **

        /// <summary>
        /// Subject Code
        /// </summary>
        public int SbjCode { get; set; }

        /// <summary>
        /// Product Code
        /// </summary>
        public string PrcCode { get; set; }

        /// <summary>
        /// Copyright Code
        /// </summary>
        public string CprCode { get; set; }//*** Not used by the Front End yet **

        /// <summary>
        /// Exceptional Flag
        /// </summary>
        public bool? RlsExceptionalFlag { get; set; }//*** Not used by the Front End yet **

        /// <summary>
        /// Reservation flag
        /// </summary>
        public bool? RlsReservationFlag { get; set; }//*** Not used by the Front End yet **

        /// <summary>
        /// Archive Flag
        /// </summary>
        public bool? RlsArchiveFlag { get; set; }//*** Not used by the Front End yet **

        /// <summary>
        /// Experimental Flag
        /// </summary>
        public bool? RlsExperimentalFlag { get; set; }//*** Not used by the Front End yet **

        /// <summary>
        /// Analytical Flag
        /// </summary>
        public bool? RlsAnalyticalFlag { get; set; }//*** Not used by the Front End yet **

        /// <summary>
        /// Search term
        /// </summary>
        [LowerCase]
        public string Search { get; set; }
        #endregion

        public DataTable SearchTerms { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Navigation_DTO_Search(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

            if (parameters.MtrCode != null)
                MtrCode = parameters.MtrCode;

            if (parameters.MtrOfficialFlag != null)
                MtrOfficialFlag = parameters.MtrOfficialFlag;

            if (parameters.SbjCode != null)
                SbjCode = parameters.SbjCode;

            if (parameters.PrcCode != null)
                PrcCode = parameters.PrcCode;

            if (parameters.CprCode != null)
                CprCode = parameters.CprCode;

            if (parameters.RlsExceptionalFlag != null)
                RlsExceptionalFlag = parameters.RlsExceptionalFlag;

            if (parameters.RlsReservationFlag != null)
                RlsReservationFlag = parameters.RlsReservationFlag;

            if (parameters.RlsArchiveFlag != null)
                RlsArchiveFlag = parameters.RlsArchiveFlag;

            if (parameters.RlsExperimentalFlag != null)
                RlsExperimentalFlag = parameters.RlsExperimentalFlag;

            if (parameters.RlsAnalyticalFlag != null)
                RlsAnalyticalFlag = parameters.RlsAnalyticalFlag;

            if (parameters.Search != null)
            {
                Search = parameters.Search;
                if (string.IsNullOrEmpty(Search) || string.IsNullOrWhiteSpace(Search)) Search = null;
            }


        }
    }
}
