
namespace PxStat.Workflow
{
    /// <summary>
    /// 
    /// </summary>
    internal class Comment_DTO
    {
        /// <summary>
        /// 
        /// </summary>
        public string CmmValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int CmmCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public Comment_DTO(dynamic parameters)
        {
            this.CmmValue = parameters.CmmValue == null ? "" : parameters.CmmValue;
            if (parameters.CmmCode != null)
                this.CmmCode = parameters.CmmCode;
        }
    }
}