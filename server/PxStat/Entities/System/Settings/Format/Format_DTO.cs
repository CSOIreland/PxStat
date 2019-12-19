
using PxStat.Resources;
using System.ComponentModel;

namespace PxStat.System.Settings
{
    /// <summary>
    /// DTO for Format Read
    /// </summary>
    public class Format_DTO_Read
    {
        /// <summary>
        /// Format Type
        /// </summary>
        public string FrmType { get; set; }

        /// <summary>
        /// Format Version
        /// </summary>
        public string FrmVersion { get; set; }

        public string FrmDirection { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Format_DTO_Read(dynamic parameters)
        {
            if (parameters.FrmType != null)
                this.FrmType = parameters.FrmType;
            if (parameters.FrmVersion != null)
                this.FrmVersion = parameters.FrmVersion;
            if (parameters.FrmDirection != null)
                this.FrmDirection = parameters.FrmDirection;
        }

        public Format_DTO_Read()
        {
        }

        /// <summary>
        /// Enums for format type etc
        /// </summary>

        internal enum FormatDirection { UPLOAD, DOWNLOAD };
        internal enum FormatType
        {
            [Description(Constants.C_SYSTEM_PX_NAME)]
            PX,
            [Description(Constants.C_SYSTEM_JSON_STAT_NAME)]
            JSONstat,
            [Description(Constants.C_SYSTEM_CSV_NAME)]
            CSV
        };



    }


}
