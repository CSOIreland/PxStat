using PxStat.System.Settings;
using System;

namespace PxStat.Security
{
    /// <summary>
    /// Analytic DTO
    /// </summary>
    internal class Analytic_DTO
    {
        #region Properties
        /// <summary>
        /// matrix code
        /// </summary>
        public string matrix { get; internal set; }

        /// <summary>
        /// Masked IP address
        /// </summary>
        public string NltMaskedIp { get; internal set; }

        /// <summary>
        /// Operating System
        /// </summary>
        public string NltOs { get; internal set; }

        /// <summary>
        /// Browser
        /// </summary>
        public string NltBrowser { get; internal set; }

        /// <summary>
        /// Flag to indicate if the request was from a Bot
        /// </summary>
        public bool NltBotFlag { get; internal set; }

        /// <summary>
        /// url of Referring site
        /// </summary>
        public string NltReferer { get; internal set; }

        /// <summary>
        /// Flag to indicate machine to machine request
        /// </summary>
        public bool? NltM2m { get; internal set; }

        /// <summary>
        /// Flag to indicate that this is a widget based call
        /// </summary>
        public bool NltWidget { get; internal set; }

        public bool NltUser { get; internal set; }

        /// <summary>
        /// Date and time of the request
        /// </summary>
        public DateTime NltDate { get; internal set; }
        public string FrmType { get; set; }
        public string FrmVersion { get; set; }
        public string EnvironmentLngIsoCode { get; set; }
        public string LngIsoCode { get; set; }

        #endregion
    }
    /// <summary>
    /// DTO class for Analytic
    /// </summary>
    internal class Analytic_DTO_Read
    {
        #region Properties
        /// <summary>
        /// Start date and time of request
        /// </summary>
        public DateTime DateFrom { get; internal set; }

        /// <summary>
        /// End date and time of request
        /// </summary>
        public DateTime DateTo { get; internal set; }

        /// <summary>
        /// Subject code
        /// </summary>
        public int SbjCode { get; internal set; }

        /// <summary>
        /// Product code
        /// </summary>
        public string PrcCode { get; internal set; }

        /// <summary>
        /// Flag to exclude requests from within a defined domain
        /// </summary>
        public bool ExcludeInternal { get; internal set; }

        /// <summary>
        /// Matrix code
        /// </summary>
        public string MtrCode { get; internal set; }

        /// <summary>
        /// Nlt mask to define internal domain
        /// </summary>
        public string NltInternalNetworkMask { get; set; }

        /// <summary>
        /// Language ISO code
        /// </summary>
        public string LngIsoCode { get; set; }

        public string FrmType { get; internal set; }
        public string FrmVersion { get; internal set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Analytic_DTO_Read(dynamic parameters)
        {
            if (parameters.DateFrom != null)
            {
                DateFrom = parameters.DateFrom;
                DateFrom = DateFrom.Date;
            }
            if (parameters.DateTo != null)
            {
                DateTo = parameters.DateTo;
                DateTo = DateTo.Date;               
            }
            if (parameters.SbjCode != null)
                SbjCode = parameters.SbjCode;
            if (parameters.PrcCode != null)
                PrcCode = parameters.PrcCode;
            if (parameters.ExcludeInternal != null)
                ExcludeInternal = parameters.ExcludeInternal;
            if (parameters.NltInternalNetworkMask!=null)
                NltInternalNetworkMask = parameters.NltInternalNetworkMask;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            

            if (parameters.FrmType != null)
                FrmType = parameters.FrmType;
            if (parameters.FrmVersion != null)
                FrmVersion = parameters.FrmVersion;

            if(parameters.MtrCode!=null)
                MtrCode = parameters.MtrCode;   
        }
    }
}
