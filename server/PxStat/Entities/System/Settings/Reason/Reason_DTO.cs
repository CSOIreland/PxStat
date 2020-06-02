using PxStat.Security;

namespace PxStat.System.Settings
{
    /// <summary>
    /// DTO for Reason Read
    /// </summary>
    internal class Reason_DTO_Read
    {
        /// <summary>
        /// Release Code
        /// </summary>
        [UpperCase]
        public int RlsCode { get; set; }

        /// <summary>
        /// Reason Code
        /// </summary>
        public string RsnCode { get; set; }
        /// <summary>
        /// Language Code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Reason_DTO_Read(dynamic parameters)
        {
            if (parameters.RlsCode != null)
                this.RlsCode = parameters.RlsCode;
            if (parameters.RsnCode != null)
                this.RsnCode = parameters.RsnCode;
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Configuration_BSO.GetCustomConfig("language.iso.code");
        }
    }

    /// <summary>
    /// DTO for Reason Create
    /// </summary>
    internal class Reason_DTO_Create
    {
        /// <summary>
        /// Reason Code
        /// </summary>
        [UpperCase]
        public string RsnCode { get; set; }

        /// <summary>
        /// Reason Value for Internal users
        /// </summary>
        public string RsnValueInternal { get; set; }

        /// <summary>
        /// Reason Value for External users
        /// </summary>
        public string RsnValueExternal { get; set; }
        /// <summary>
        /// Language Code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Reason_DTO_Create(dynamic parameters)
        {
            if (parameters.RsnCode != null)
                this.RsnCode = parameters.RsnCode;
            if (parameters.RsnValueInternal != null)
                this.RsnValueInternal = parameters.RsnValueInternal;
            if (parameters.RsnValueExternal != null)
                this.RsnValueExternal = parameters.RsnValueExternal;

            this.LngIsoCode = Configuration_BSO.GetCustomConfig("language.iso.code");
        }
    }

    /// <summary>
    /// DTO for Reason Update
    /// </summary>
    internal class Reason_DTO_Update
    {
        /// <summary>
        /// Reason Code
        /// </summary>
        [UpperCase]
        public string RsnCode { get; set; }

        /// <summary>
        /// Reason Value Internal
        /// </summary>
        public string RsnValueInternal { get; set; }

        /// <summary>
        /// Reason Value External
        /// </summary>
        public string RsnValueExternal { get; set; }
        /// <summary>
        /// Language Code
        /// </summary>
        public string LngIsoCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Reason_DTO_Update(dynamic parameters)
        {
            if (parameters.RsnCode != null)
                this.RsnCode = parameters.RsnCode;
            if (parameters.RsnValueInternal != null)
                this.RsnValueInternal = parameters.RsnValueInternal;
            if (parameters.RsnValueExternal != null)
                this.RsnValueExternal = parameters.RsnValueExternal;
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Configuration_BSO.GetCustomConfig("language.iso.code");
        }
    }

    /// <summary>
    /// DTO for Reason Delete
    /// </summary>
    internal class Reason_DTO_Delete
    {
        /// <summary>
        /// Reason Code
        /// </summary>
        [UpperCase]
        public string RsnCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Reason_DTO_Delete(dynamic parameters)
        {
            if (parameters.RsnCode != null)
                this.RsnCode = parameters.RsnCode;
        }
    }
}
