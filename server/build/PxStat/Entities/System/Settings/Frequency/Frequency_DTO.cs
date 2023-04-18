
using PxStat.Data;

using System.Collections.Generic;

namespace PxStat.System.Settings
{
    /// <summary>
    /// DTO for Frequency
    /// </summary>
    internal class Frequency_DTO
    {
        internal string FrqCode { get; set; }
        internal string FrqValue { get; set; }

        internal List<PeriodRecordDTO_Create> Period { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Frequency_DTO(dynamic parameters) { }

        public Frequency_DTO()
        {
        }
    }
}