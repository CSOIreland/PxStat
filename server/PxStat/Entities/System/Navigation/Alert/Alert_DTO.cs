using PxStat.Security;
using System;


namespace PxStat.System.Navigation
{
    /// <summary>
    /// DTO for Alert
    /// </summary>
    internal class Alert_DTO
    {
        /// <summary>
        /// Alert Code
        /// </summary>
        public int LrtCode { get; set; }

        /// <summary>
        /// Iso Language Code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Alert Message
        /// </summary>
        public string LrtMessage { get; set; }

        /// <summary>
        /// Alert date and time
        /// </summary>
        public DateTime LrtDatetime { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Alert_DTO(dynamic parameters)
        {
            if (parameters.LrtCode != null)
                LrtCode = parameters.LrtCode;
            if (parameters.LrtMessage != null)
                LrtMessage = parameters.LrtMessage;
            if (parameters.LrtDatetime != null)
                LrtDatetime = parameters.LrtDatetime;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetCustomConfig("language.iso.code");


        }
    }
}
