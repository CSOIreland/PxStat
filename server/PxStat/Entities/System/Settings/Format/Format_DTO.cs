
using API;
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

        public string FrmMimetype { get; set; }

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
        /// Used to create a valid Format based on the PxApiV1 model
        /// </summary>
        /// <param name="pxApiValue"></param>
        public Format_DTO_Read(string pxApiValue)
        {
            try
            {
                this.FrmType = Utility.GetCustomConfig("APP_FORMAT_PXAPI_TYPE_" + pxApiValue);
                this.FrmVersion = Utility.GetCustomConfig("APP_FORMAT_PXAPI_VERSION_" + pxApiValue);
            }
            catch { }

            string mtype = "";
            using (Format_BSO fbso = new Format_BSO(new ADO("defaultConnection")))
            {
                mtype = fbso.GetMimetypeForFormat(this);
            };
            this.FrmMimetype = mtype;
            if (this.FrmVersion == null) this.FrmVersion = pxApiValue;
            if (this.FrmType == null) this.FrmType = pxApiValue;

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
            CSV,
            [Description(Constants.C_SYSTEM_XLSX_NAME)]
            XLSX,
            [Description(Constants.C_SYSTEM_SDMX_NAME)]
            SDMX

        };



    }


}
