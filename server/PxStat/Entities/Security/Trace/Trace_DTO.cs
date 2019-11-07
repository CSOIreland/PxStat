using System;

namespace PxStat.Security
{
    /// <summary>
    /// DTO for Trace Read
    /// </summary>
    internal class Trace_DTO_Read
    {
        #region Properties
        /// <summary>
        /// Start Date of Trace search
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// End date of Trace search
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Restrict search to this user name
        /// </summary>
        public string TrcUsername { get; set; }

        /// <summary>
        /// Restrict search to this ip address
        /// </summary>
        public string TrcIp { get; set; }

        /// <summary>
        /// Restrict search to this authentication type
        /// </summary>
        [UpperCase]
        public string AuthenticationType { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Trace_DTO_Read(dynamic parameters)
        {
            if (parameters.StartDate != null && parameters.EndDate != null)
            {
                DateTime result;
                if (DateTime.TryParse(parameters.StartDate.ToString(), out result))
                    this.StartDate = Convert.ToDateTime(parameters.StartDate.ToString());

                if (DateTime.TryParse(parameters.EndDate.ToString(), out result))
                    this.EndDate = Convert.ToDateTime(parameters.EndDate.ToString());
            }
            if (parameters.AuthenticationType != null)
                this.AuthenticationType = parameters.AuthenticationType;
            if (parameters.TrcUsername != null)
                this.TrcUsername = parameters.TrcUsername;
            if (parameters.TrcIp != null)
                this.TrcIp = parameters.TrcIp;
        }
    }

    /// <summary>
    /// DTO for Trace Create
    /// </summary>
    internal class Trace_DTO_Create
    {
        #region Properties
        /// <summary>
        /// Trace Method
        /// </summary>
        public string TrcMethod { get; set; }

        /// <summary>
        /// Trace parameters
        /// </summary>
        public string TrcParams { get; set; }

        /// <summary>
        /// Trace ip address
        /// </summary>
        public string TrcIp { get; set; }

        /// <summary>
        /// Trace useragent string
        /// </summary>
        public string TrcUseragent { get; set; }

        /// <summary>
        /// account username
        /// </summary>
        public string CcnUsername { get; set; }
        #endregion
    }

    /// <summary>
    /// DTO for reading authentication type
    /// </summary>
    internal class Trace_DTO_ReadType
    {
        #region Properties
        /// <summary>
        /// Authentication type
        /// </summary>
        public string AuthenticationType { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authenticationType"></param>
        public Trace_DTO_ReadType(string authenticationType)
        {
            AuthenticationType = authenticationType;
        }
    }
}