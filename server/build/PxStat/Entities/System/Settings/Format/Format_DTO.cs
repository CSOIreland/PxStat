
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

            switch (pxApiValue)
            {
                case Constants.C_PXAPIV1_PX:
                    this.FrmType = Constants.C_SYSTEM_PX_NAME;
                    this.FrmVersion = Constants.C_SYSTEM_PX_VERSION;
                    break;

                case Constants.C_PXAPIV1_JSON_STAT_1X:
                    this.FrmType = Constants.C_SYSTEM_JSON_STAT_NAME;
                    this.FrmVersion = Constants.C_SYSTEM_JSON_STAT_1X_VERSION;
                    break;
                case Constants.C_PXAPIV1_JSON_STAT_2X:
                    this.FrmType = Constants.C_SYSTEM_JSON_STAT_NAME;
                    this.FrmVersion = Constants.C_SYSTEM_JSON_STAT_2X_VERSION;
                    break;
                case Constants.C_PXAPIV1_CSV:
                    this.FrmType = Constants.C_SYSTEM_CSV_NAME;
                    this.FrmVersion = Constants.C_SYSTEM_CSV_VERSION;
                    break;
                case Constants.C_PXAPIV1_XLSX:
                    this.FrmType = Constants.C_SYSTEM_XLSX_NAME;
                    this.FrmVersion = Constants.C_SYSTEM_XLSX_VERSION;
                    break;
                default:

                    break;


            }



            string mtype = "";
            using (Format_BSO fbso = new Format_BSO(AppServicesHelper.StaticADO))
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
