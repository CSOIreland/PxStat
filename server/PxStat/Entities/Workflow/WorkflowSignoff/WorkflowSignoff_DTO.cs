namespace PxStat.Workflow
{
    /// <summary>
    /// DTO for WorkflowSignoff
    /// </summary>
    internal class WorkflowSignoff_DTO
    {
        /// <summary>
        /// Signoff Code
        /// </summary>
        public string SgnCode { get; set; }

        /// <summary>
        /// Comment Code
        /// </summary>
        public int CmmCode { get; set; }

        /// <summary>
        /// Comment Value
        /// </summary>
        public string CmmValue { get; set; }

        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Matrix Code
        /// </summary>
        public string MtrCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public WorkflowSignoff_DTO(dynamic parameters)
        {
            SgnCode = parameters.SgnCode == null ? "" : parameters.SgnCode;
            CmmValue = parameters.CmmValue == null ? "" : parameters.CmmValue;

            if (parameters.RlsCode != null)
                RlsCode = parameters.RlsCode;
        }

        public WorkflowSignoff_DTO()
        {
        }
    }
}