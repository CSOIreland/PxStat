using System;

namespace PxStat.Security.Logging
{
    /// <summary>
    /// DTO for logging read
    /// </summary>
    internal class Logging_DTO
    {
        /// <summary>
        /// Date and time of first log entry
        /// </summary>
        public DateTime LggDatetimeStart { get; set; }

        /// <summary>
        /// Date and time of last log entry
        /// </summary>
        public DateTime LggDatetimeEnd { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Logging_DTO(dynamic parameters)
        {
            if (parameters.LggDatetimeStart != null)
                LggDatetimeStart = parameters.LggDatetimeStart;
            if (parameters.LggDatetimeEnd != null)
                LggDatetimeEnd = parameters.LggDatetimeEnd;
        }
    }
}