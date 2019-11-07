
namespace PxStat.System.Settings
{
    /// <summary>
    /// The Language_DTO_Read class is used to deal specifically with the input parameters for the Read API
    /// </summary>
    internal class Language_DTO_Read
    {
        public Language_DTO_Read()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Language_DTO_Read(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
        }

        /// <summary>
        /// Iso Language Code
        /// </summary>
        [LowerCase]
        public string LngIsoCode { get; set; }
    }

    /// <summary>
    /// DTO for reading a Language
    /// </summary>
    internal class Language_DTO_ReadList
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Language_DTO_ReadList(dynamic parameters)
        {
            if (parameters.RlsCode != null)
                this.RlsCode = parameters.RlsCode;

        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public int RlsCode { get; set; }
    }

    /// <summary>
    /// DTO for Language Create
    /// </summary>
    internal class Language_DTO_Create
    {
        /// <summary>
        /// ISO Language Code
        /// </summary>
        [LowerCase]
        public string LngIsoCode { get; set; }

        /// <summary>
        /// ISO Language Name
        /// </summary>
        public string LngIsoName { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Language_DTO_Create(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            if (parameters.LngIsoName != null)
                this.LngIsoName = parameters.LngIsoName;
        }

        public Language_DTO_Create()
        {
        }
    }

    /// <summary>
    /// DTO for Language Update
    /// </summary>
    internal class Language_DTO_Update
    {
        /// <summary>
        /// ISO Language Code
        /// </summary>
        [LowerCase]
        public string LngIsoCode { get; set; }
        /// <summary>
        /// ISO Language Name
        /// </summary>
        public string LngIsoName { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Language_DTO_Update(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            if (parameters.LngIsoName != null)
                this.LngIsoName = parameters.LngIsoName;
        }
    }

    /// <summary>
    /// DTO for Language Delete
    /// </summary>
    internal class Language_DTO_Delete
    {
        /// <summary>
        /// ISO Language Code
        /// </summary>
        [LowerCase]
        public string LngIsoCode { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Language_DTO_Delete(dynamic parameters)
        {
            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
        }

    }
}