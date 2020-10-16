using PxStat.Security;
using System;

namespace PxStat.Data
{
    /// <summary>
    /// Reason Release DTO class
    /// </summary>
    internal class ReasonRelease_DTO_Read
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Reason Code
        /// </summary>
        public string RsnCode { get; set; }

        public string LngIsoCode { get; set; }

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="parameters"></param>
        public ReasonRelease_DTO_Read(dynamic parameters)
        {
            if (parameters.RlsCode != null)
            {
                int param;
                if (Int32.TryParse((string)parameters.RlsCode, out param))
                    this.RlsCode = parameters.RlsCode;

            }
            if (parameters.RsnCode != null)
                this.RsnCode = parameters.RsnCode;

            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public ReasonRelease_DTO_Read() { }
    }

    /// <summary>
    /// DTO class for ReasonRelease Create
    /// </summary>
    internal class ReasonRelease_DTO_Create
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Reason Code
        /// </summary>
        public string RsnCode { get; set; }

        /// <summary>
        /// Comment string
        /// </summary>
        public string CmmValue { get; set; }

        /// <summary>
        /// Comment string
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public ReasonRelease_DTO_Create(dynamic parameters)
        {
            if (parameters.RlsCode != null)
            {
                int param;
                if (Int32.TryParse((string)parameters.RlsCode, out param))
                    this.RlsCode = parameters.RlsCode;

            }

            if (parameters.RsnCode != null)
                this.RsnCode = parameters.RsnCode;

            if (parameters.CmmValue != null)
                this.CmmValue = parameters.CmmValue;

            if (parameters.LngIsocode != null)
                this.LngIsoCode = parameters.LngIsocode;
            else
                this.LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

        }

    }

    /// <summary>
    /// DTO class for ReasonRelease update
    /// </summary>
    internal class ReasonRelease_DTO_Update
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }
        /// <summary>
        /// Reason Code
        /// </summary>
        public string RsnCode { get; set; }
        /// <summary>
        /// Comment string
        /// </summary>
        public string CmmValue { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public ReasonRelease_DTO_Update(dynamic parameters)
        {
            if (parameters.RlsCode != null)
            {
                int param;
                if (Int32.TryParse((string)parameters.RlsCode, out param))
                    this.RlsCode = parameters.RlsCode;


            }

            if (parameters.RsnCode != null)
                this.RsnCode = parameters.RsnCode;

            if (parameters.CmmValue != null)
                this.CmmValue = parameters.CmmValue;
        }

    }

    /// <summary>
    /// DTO class for ReasonRelease Delete
    /// </summary>
    internal class ReasonRelease_DTO_Delete
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Reason Code
        /// </summary>
        public string RsnCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public ReasonRelease_DTO_Delete(dynamic parameters)
        {
            if (parameters.RlsCode != null)
            {
                int param;
                if (Int32.TryParse((string)parameters.RlsCode, out param))
                    this.RlsCode = parameters.RlsCode;

            }

            if (parameters.RsnCode != null)
                this.RsnCode = parameters.RsnCode;

        }
    }


}
