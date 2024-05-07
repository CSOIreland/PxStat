using PxStat.Security;
using System;
using System.Collections.Generic;

namespace PxStat.Data
{
    /// <summary>
    /// DTO class for Release Delete
    /// </summary>
    internal class Release_DTO_Delete
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }
    }

    public class Release_DTO_ReadWipForLive
    {
        public int RlsCode { get; set; }

        public Release_DTO_ReadWipForLive(dynamic parameters)
        {
            if (parameters.RlsCode != null)
                RlsCode = parameters.RlsCode;
        }
    }

    /// <summary>
    /// DTO class for Release Create
    /// </summary>
    internal class Release_DTO_Create
    {
        /// <summary>
        /// Matrix Code
        /// </summary>
        public string MtrCode { get; set; }
        /// <summary>
        /// Group Code
        /// </summary>
        public string GrpCode { get; set; }
    }

    /// <summary>
    /// DTO class for Release Read
    /// </summary>
    internal class Release_DTO_Read
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Matrix Code
        /// </summary>
        public string MtrCode { get; set; }

        /// <summary>
        /// Product Code
        /// </summary>
        public string PrcCode { get; set; }

        public string LngIsoCode { get; set; }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public Release_DTO_Read() { }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="parameters"></param>
        public Release_DTO_Read(dynamic parameters)
        {
            if (parameters.RlsCode != null)
                this.RlsCode = parameters.RlsCode;
            if (parameters.MtrCode != null)
                this.MtrCode = parameters.MtrCode;
            if (parameters.PrcCode != null)
                this.PrcCode = parameters.PrcCode;
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
        }
    }

    /// <summary>
    /// DTO class for Release Update
    /// </summary>
    internal class Release_DTO_Update
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }
        /// <summary>
        /// Release Analytical Flag
        /// </summary>
        public bool? RlsAnalyticalFlag { get; set; }



        /// <summary>
        /// Product Code
        /// </summary>
        public string PrcCode { get; set; }

        /// <summary>
        /// Comment Value
        /// </summary>
        public string CmmValue { get; set; }

        /// <summary>
        /// Matrix Code
        /// </summary>
        public string MtrCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Release_DTO_Update(dynamic parameters)
        {
            if (parameters.RlsCode != null)
            {
                RlsCode = parameters.RlsCode;
            }
            if (parameters.PrcCode != null)
                PrcCode = parameters.PrcCode;
            if (parameters.CmmValue != null)
                CmmValue = parameters.CmmValue;
            if (parameters.RlsAnalyticalFlag != null)
                RlsAnalyticalFlag = parameters.RlsAnalyticalFlag;

        }

    }

    /// <summary>
    /// General DTO class for Release - used as a container class
    /// </summary>
    public class Release_DTO
    {
        #region Properties
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }


        /// <summary>
        /// Release Analytical Flag
        /// </summary>
        public bool RlsAnalyticalFlag { get; set; }

        /// <summary>
        /// Release Archive Flag
        /// </summary>
        public bool RlsArchiveFlag { get; set; }

        /// <summary>
        /// Release Experimental Flag
        /// </summary>
        public bool RlsExperimentalFlag { get; set; }

        /// <summary>
        ///  End date and time of release
        /// </summary>
        public DateTime RlsLiveDatetimeTo { get; set; }

        /// <summary>
        ///Release Reservation Flag
        /// </summary>
        public bool RlsReservationFlag { get; set; }

        /// <summary>
        /// Release Revision number
        /// </summary>
        public int RlsRevision { get; set; }

        /// <summary>
        /// Release Version number
        /// </summary>
        public int RlsVersion { get; set; }


        /// <summary>
        ///  Release Exceptional Flag
        /// </summary>
        public bool RlsExceptionalFlag { get; set; }

        /// <summary>
        /// Start date and time of Release
        /// </summary>
        public DateTime RlsLiveDatetimeFrom { get; set; }


        /// <summary>
        /// Group Code
        /// </summary>
        public string GrpCode { get; set; }

        /// <summary>
        /// Matrix Code
        /// </summary>
        public string MtrCode { get; set; }

        /// <summary>
        /// Subject Value
        /// </summary>
        public string SbjValue { get; set; }

        /// <summary>
        /// Product value
        /// </summary>
        public string PrcValue { get; set; }

        /// <summary>
        /// Product Code
        /// </summary>
        public string PrcCode { get; set; }

        /// <summary>
        /// Comment Code
        /// </summary>
        public int CmmCode { get; set; }

        /// <summary>
        /// Release Live Flag
        /// </summary>
        public bool RlsLiveFlag { get; set; }

        /// <summary>
        /// Group Name
        /// </summary>
        public string GrpName { get; set; }

        /// <summary>
        /// Comment Value
        /// </summary>
        public string CmmValue { get; set; }

        /// <summary>
        /// Subject Code
        /// </summary>
        public int SbjCode { get; set; }

        /// <summary>
        /// Group Contact Name
        /// </summary>
        public string GrpContactName { get; set; }

        /// <summary>
        /// Group Contact Phone
        /// </summary>
        public string GrpContactPhone { get; set; }

        /// <summary>
        /// Group Contact Email
        /// </summary>
        public string GrpContactEmail { get; set; }

        public List<string> Reasons { get; set; }

        public int MtrId { get; set; }
        #endregion
    }
}
