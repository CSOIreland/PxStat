using System;

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
        public int RlsCode { get; internal set; }
        /// <summary>
        /// Release Analytical Flag
        /// </summary>
        public bool? RlsAnalyticalFlag { get; internal set; }

        /// <summary>
        /// Release Dependency Flag
        /// </summary>
        public bool? RlsDependencyFlag { get; internal set; }

        /// <summary>
        /// Product Code
        /// </summary>
        public string PrcCode { get; internal set; }

        /// <summary>
        /// Comment Value
        /// </summary>
        public string CmmValue { get; internal set; }

        /// <summary>
        /// Matrix Code
        /// </summary>
        public string MtrCode { get; internal set; }

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
            if (parameters.RlsAnalyticalFlag != null)
                RlsAnalyticalFlag = parameters.RlsAnalyticalFlag;
            if (parameters.RlsDependencyFlag != null)
                RlsDependencyFlag = parameters.RlsDependencyFlag;
            if (parameters.PrcCode != null)
                PrcCode = parameters.PrcCode;
            if (parameters.CmmValue != null)
                CmmValue = parameters.CmmValue;

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
        public int RlsCode { get; internal set; }


        /// <summary>
        /// Release Analytical Flag
        /// </summary>
        public bool RlsAnalyticalFlag { get; internal set; }

        /// <summary>
        /// Release Archive Flag
        /// </summary>
        public bool RlsArchiveFlag { get; internal set; }

        /// <summary>
        ///  End date and time of release
        /// </summary>
        public DateTime RlsLiveDatetimeTo { get; internal set; }

        /// <summary>
        ///Release Reservation Flag
        /// </summary>
        public bool RlsReservationFlag { get; internal set; }

        /// <summary>
        /// Release Revision number
        /// </summary>
        public int RlsRevision { get; internal set; }

        /// <summary>
        /// Release Version number
        /// </summary>
        public int RlsVersion { get; internal set; }

        /// <summary>
        /// Release Dependency Flag
        /// </summary>
        public bool RlsDependencyFlag { get; internal set; }

        /// <summary>
        ///  Release Emergency Flag
        /// </summary>
        public bool RlsEmergencyFlag { get; internal set; }

        /// <summary>
        /// Start date and time of Release
        /// </summary>
        public DateTime RlsLiveDatetimeFrom { get; internal set; }


        /// <summary>
        /// Group Code
        /// </summary>
        public string GrpCode { get; internal set; }

        /// <summary>
        /// Matrix Code
        /// </summary>
        public string MtrCode { get; internal set; }

        /// <summary>
        /// Subject Value
        /// </summary>
        public string SbjValue { get; internal set; }

        /// <summary>
        /// Product value
        /// </summary>
        public string PrcValue { get; internal set; }

        /// <summary>
        /// Product Code
        /// </summary>
        public string PrcCode { get; internal set; }

        /// <summary>
        /// Comment Code
        /// </summary>
        public int CmmCode { get; internal set; }

        /// <summary>
        /// Release Live Flag
        /// </summary>
        public bool RlsLiveFlag { get; internal set; }

        /// <summary>
        /// Group Name
        /// </summary>
        public string GrpName { get; internal set; }

        /// <summary>
        /// Comment Value
        /// </summary>
        public string CmmValue { get; internal set; }

        /// <summary>
        /// Subject Code
        /// </summary>
        public int SbjCode { get; internal set; }

        /// <summary>
        /// Group Contact Name
        /// </summary>
        public string GrpContactName { get; internal set; }

        /// <summary>
        /// Group Contact Phone
        /// </summary>
        public string GrpContactPhone { get; internal set; }

        /// <summary>
        /// Group Contact Email
        /// </summary>
        public string GrpContactEmail { get; internal set; }

        #endregion
    }
}
