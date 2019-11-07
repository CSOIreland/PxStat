

namespace PxStat.Workflow
{
    /// <summary>
    /// DTO for Workflow Response
    /// </summary>
    internal class WorkflowResponse_DTO
    {
        /// <summary>
        /// Response Code
        /// </summary>
        public string RspCode { get; set; }

        /// <summary>
        /// Comment code
        /// </summary>
        public int CmmCode { get; set; }

        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Comment value
        /// </summary>
        public string CmmValue { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public WorkflowResponse_DTO(dynamic parameters)
        {
            this.RspCode = parameters.RspCode == null ? "" : parameters.RspCode;
            if (parameters.RlsCode != null)
                this.RlsCode = parameters.RlsCode;
            this.CmmValue = parameters.CmmValue == null ? "" : parameters.CmmValue;
        }

        public WorkflowResponse_DTO()
        {
        }
    }
}