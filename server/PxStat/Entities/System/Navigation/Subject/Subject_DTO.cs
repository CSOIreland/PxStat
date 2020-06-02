using PxStat.Security;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// DTO class for Subjects
    /// </summary>
    internal class Subject_DTO
    {
        /// <summary>
        /// Subject Code
        /// </summary>
        public int SbjCode { get; set; }

        /// <summary>
        /// Subject Value
        /// </summary>
        public string SbjValue { get; set; }

        /// <summary>
        /// ISO Language Code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Subject_DTO(dynamic parameters)
        {
            if (parameters.SbjValue != null)
                this.SbjValue = parameters.SbjValue;

            if (parameters.SbjCode != null)
                this.SbjCode = parameters.SbjCode;

            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Configuration_BSO.GetCustomConfig("language.iso.code");

        }
        /// <summary>
        /// Blank constructor
        /// </summary>
        public Subject_DTO() { }
    }
}
