
using PxStat.Resources;

namespace PxStat.Config
{
    /// <summary>
    /// The Config_DTO_Read class is used to deal specifically with the input parameters for the Read API
    /// </summary>
    internal class Config_DTO_Read
    {
        public string version { get; set; }

        public string type { get; set; }

        public string name { get; set; }

        public Config_DTO_Read()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Config_DTO_Read(dynamic parameters)
        {
            if (Cleanser.TryParseJson<dynamic>(parameters.ToString(), out dynamic canParse))
            {
                if (parameters.version != null)
                {
                    this.version = parameters.version;
                }
                if (parameters.type != null)
                {
                    this.type = parameters.type;
                }
                else
                {
                    // APP is the default type
                    this.type = "APP";
                }
                if (parameters.name != null)
                {
                    this.name = parameters.name;
                }
            }
            else
            {
                if(parameters.Count>=2)
                    this.name= parameters[1];
                this.type = "APP";
            }
        }
    }

    internal class Config_DTO_ReadSchema
    {
        public string name { get; set; }

        public Config_DTO_ReadSchema()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Config_DTO_ReadSchema(dynamic parameters)
        {
            if (parameters.name != null)
            {
                this.name = parameters.name;
            }
        }
    }

    /// <summary>
    /// DTO for Configuration Create
    /// </summary>
    internal class Config_DTO_Create
    {
        [DefaultSanitizer]
        public string version { get; set; }

        public string type { get; set; }

        public string name { get; set; }

        [NoHtmlStrip]
        public string value { get; set; }

        public string description { get; set; }

        public bool sensitive { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Config_DTO_Create(dynamic parameters)
        {
            if (parameters.version != null)
            {
                this.version = parameters.version;
            }
            if (parameters.type != null)
            {
                this.type = parameters.type;
            }
            if (parameters.name != null)
            {
                this.name = parameters.name;
            }
            if (parameters.value != null)
            {
                this.value = parameters.value.ToString();
            }
            if (parameters.description != null)
            {
                this.description = parameters.description;
            }
            if (parameters.sensitive != null)
            {
                this.sensitive = parameters.sensitive;
            }
            else
            {
                this.sensitive = false;
            }
        }

        public Config_DTO_Create()
        {
        }
    }

    /// <summary>
    /// DTO for Configuration Update
    /// </summary>
    internal class Config_DTO_Update
    {
        [DefaultSanitizer]
        public string version { get; set; }

        public string type { get; set; }

        public string name { get; set; }

        [NoHtmlStrip]
        public string value { get; set; }

        public string description { get; set; }

        public bool sensitive { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Config_DTO_Update(dynamic parameters)
        {
            if (parameters.version != null)
            {
                this.version = parameters.version;
            }
            if (parameters.type != null)
            {
                this.type = parameters.type;
            }
            if (parameters.name != null)
            {
                this.name = parameters.name;
            }
            if (parameters.value != null)
            {
                this.value = parameters.value.ToString();
            }
            if (parameters.description != null)
            {
                this.description = parameters.description;
            }
            if (parameters.sensitive != null)
            {
                this.sensitive = parameters.sensitive;
            }
            else
            {
                this.sensitive = false;
            }
        }

        public Config_DTO_Update()
        {
        }
    }
    
    public class Config_DTO_Ping
    {
        public Config_DTO_Ping(dynamic parameters) { }
    }

    public class Config_DTO_ReadVersions
    {
        public string type { get; set; }

        public Config_DTO_ReadVersions(dynamic parameters) {
            if (parameters.type != null)
            {
                this.type = parameters.type;
            }
        }
    }
}