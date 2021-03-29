namespace PxStat.Security
{
    internal class Database_DTO_Read
    {
        public string TableName { get; set; }
        /// <summary>
        /// Blank constructor
        /// </summary>
        public Database_DTO_Read() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Database_DTO_Read(dynamic parameters)
        {
            if (parameters.TableName != null)
                this.TableName = parameters.TableName;

        }
    }
    internal class Database_DTO_Update
    {

        /// <summary>
        /// Blank constructor
        /// </summary>
        public Database_DTO_Update() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Database_DTO_Update(dynamic parameters)
        {


        }
    }
}
