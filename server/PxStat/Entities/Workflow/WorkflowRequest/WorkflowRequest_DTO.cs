using System;

namespace PxStat.Workflow
{
    /// <summary>
    /// DTO for Workflow Request
    /// </summary>
    internal class WorkflowRequest_DTO
    {
        #region Properties
        /// <summary>
        /// Request Code
        /// </summary>
        [UpperCase]
        public string RqsCode { get; set; }
        /// <summary>
        /// Request Value
        /// </summary>
        public string RqsValue { get; set; }

        /// <summary>
        /// Comment Code
        /// </summary>
        public int CmmCode { get; set; }

        /// <summary>
        /// Request Datetime
        /// </summary>
        public DateTime WrqDatetime { get; set; }

        /// <summary>
        /// Exceptional flag
        /// </summary>
        public bool? WrqExceptionalFlag { get; set; }

        /// <summary>
        /// Reservation flag
        /// </summary>
        public bool? WrqReservationFlag { get; set; }

        /// <summary>
        /// Archive flag
        /// </summary>
        public bool? WrqArchiveFlag { get; set; }



        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Comment value
        /// </summary>
        public string CmmValue { get; set; }

        /// <summary>
        /// Current flag
        /// </summary>
        public bool WrqCurrentFlag { get; set; }

        public Security.ActiveDirectory_DTO RequestAccount { get; set; }

        #endregion

        /// <summary>
        /// Blank constructor
        /// </summary>
        public WorkflowRequest_DTO() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public WorkflowRequest_DTO(dynamic parameters)
        {
            if (parameters.RqsCode != null)
                this.RqsCode = parameters.RqsCode;

            if (parameters.WrqExceptionalFlag != null)
                this.WrqExceptionalFlag = parameters.WrqExceptionalFlag;

            if (parameters.WrqReservationFlag != null)
                this.WrqReservationFlag = parameters.WrqReservationFlag;

            if (parameters.WrqArchiveFlag != null)
                this.WrqArchiveFlag = parameters.WrqArchiveFlag;


            if (parameters.WrqDatetime != null)
            {
                DateTime result;
                if (DateTime.TryParse(parameters.WrqDatetime.ToString(), out result))
                    this.WrqDatetime = Convert.ToDateTime(parameters.WrqDatetime.ToString());
            }

            if (parameters.RlsCode != null)
                this.RlsCode = parameters.RlsCode;

            this.CmmValue = parameters.CmmValue == null ? "" : parameters.CmmValue;
        }

    }

    /// <summary>
    /// DTO for Workflow Request update
    /// </summary>
    internal class WorkflowRequest_DTO_Update
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }
        /// <summary>
        /// Current flag
        /// </summary>
        public bool WrqCurrentFlag { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public WorkflowRequest_DTO_Update(dynamic parameters)
        {
            this.RlsCode = parameters.RlsCode;
            if (!string.IsNullOrEmpty(parameters.WrqCurrentFlag))
                this.WrqCurrentFlag = parameters.WrqCurrentFlag;
        }
    }
}
