using Newtonsoft.Json;
using PxStat.Security;
using System;
using System.Collections.Generic;
using API;

namespace PxStat.Report
{
    /// <summary>
    /// DTO for TableAudit read
    /// </summary>
    public class TableAudit_DTO_Read
    {
        /// <summary>
        /// Date and time of first performance entry
        /// </summary>
        public DateTime DateFrom { get; set; }

        /// <summary>
        /// Date and time of last performance entry
        /// </summary>
        public DateTime DateTo { get; set; }
        /// <summary>
        /// Reason Code
        /// </summary>
        public List<string> GrpCode { get; set; }
        /// <summary>
        /// Group Code
        /// </summary>
        public List<string> RsnCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public TableAudit_DTO_Read(dynamic parameters)
        {
            if (parameters.DateFrom != null)
            {
                this.DateFrom = parameters.DateFrom;
            }

            if (parameters.DateTo != null)
            {
                this.DateTo = parameters.DateTo;
            }

            if (parameters.GrpCode != null)
            {
                try
                {
                    this.GrpCode = JsonConvert.DeserializeObject<List<string>>(parameters.GrpCode.ToString());
                }
                catch (Exception e)
                {
                    Log.Instance.Error(string.Format("Parameter value {0} cannot be deserialized", parameters.GrpCode));
                    Log.Instance.Error(e.Message);
                    throw;
                }
            }

            if (parameters.RsnCode != null)
            {
                try
                {
                    this.RsnCode = JsonConvert.DeserializeObject<List<string>>(parameters.RsnCode.ToString());
                }
                catch (Exception e)
                {
                    Log.Instance.Error(string.Format("Parameter value {0} cannot be deserialized", parameters.RsnCode));
                    Log.Instance.Error(e.Message);
                    throw;
                }
            }
        }
    }
}
