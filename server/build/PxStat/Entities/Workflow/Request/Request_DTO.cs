using System;

namespace PxStat.Workflow
{
    /// <summary>
    /// DTO for Request
    /// </summary>
    internal class Request_DTO
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Request Code
        /// </summary>
        public string RqsCode { get; set; }

        /// <summary>
        /// Request Value
        /// </summary>
        public string RqsValue { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>

        public Request_DTO(dynamic parameters)
        {

            if (parameters.RlsCode != null)
            {
                int param;
                if (Int32.TryParse((string)parameters.RlsCode, out param))
                    this.RlsCode = parameters.RlsCode;

            }

        }

        /// <summary>
        /// Blank constuctor
        /// </summary>
        public Request_DTO() { }
    }
}