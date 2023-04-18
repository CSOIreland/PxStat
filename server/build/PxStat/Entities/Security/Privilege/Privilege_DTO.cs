
namespace PxStat.Security
{
    /// <summary>
    /// DTO class for Privilege Read
    /// </summary>
    internal class Privilege_DTO
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Privilege_DTO(dynamic parameters)
        {
            this.PrvCode = parameters.PrvCode;
        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public string PrvCode { get; set; }
    }
}