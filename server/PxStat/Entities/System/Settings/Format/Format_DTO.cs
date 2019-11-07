
namespace PxStat.System.Settings
{
    /// <summary>
    /// DTO for Format Read
    /// </summary>
    internal class Format_DTO_Read
    {
        /// <summary>
        /// Format Type
        /// </summary>
        public string FrmType { get; set; }

        /// <summary>
        /// Format Version
        /// </summary>
        public string FrmVersion { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Format_DTO_Read(dynamic parameters)
        {
            this.FrmType = parameters.FrmType;
            this.FrmVersion = parameters.FrmVersion;
        }

        public Format_DTO_Read()
        {
        }
    }
}