
namespace PxStat.System.Settings
{
    /// <summary>
    /// DTO for Copyright Read
    /// </summary>
    public class Copyright_DTO_Read : ICopyright
    {
        #region Properties
        /// <summary>
        /// Copyright Code
        /// </summary>
        [UpperCase]
        public string CprCode { get; set; }
        #endregion

        public string CprValue { get; set; }

        public string CprUrl { get; set; }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public Copyright_DTO_Read() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Copyright_DTO_Read(dynamic parameters)
        {
            if (parameters.CprCode != null)
                this.CprCode = parameters.CprCode;
            if (parameters.CprValue != null)
                this.CprValue = parameters.CprValue;
        }
    }

    /// <summary>
    /// DTO for Copyright Create
    /// </summary>
    public class Copyright_DTO_Create : ICopyright
    {
        #region Properties
        /// <summary>
        /// Copyright Code
        /// </summary>
        [UpperCase]
        public string CprCode { get; set; }

        /// <summary>
        /// Copyright Value
        /// </summary>
        public string CprValue { get; set; }

        /// <summary>
        /// Copyright url
        /// </summary>
        public string CprUrl { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Copyright_DTO_Create(dynamic parameters)
        {
            if (parameters.CprCode != null)
                this.CprCode = parameters.CprCode;
            if (parameters.CprValue != null)
                this.CprValue = parameters.CprValue;
            if (parameters.CprUrl != null)
                this.CprUrl = parameters.CprUrl;
        }

        public Copyright_DTO_Create()
        {
        }
    }

    /// <summary>
    /// DTO for copyright update
    /// </summary>
    internal class Copyright_DTO_Update
    {
        #region Properties
        /// <summary>
        /// Old Copyright code - this is to enable copyright code to be updated
        /// </summary>
        [UpperCase]
        public string CprCodeOld { get; set; }

        /// <summary>
        /// New Copyright code - this is to enable copyright code to be updated
        /// </summary>
        [UpperCase]
        public string CprCodeNew { get; set; }

        /// <summary>
        /// Copyright value
        /// </summary>
        public string CprValue { get; set; }

        /// <summary>
        /// Copyright url
        /// </summary>
        public string CprUrl { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Copyright_DTO_Update(dynamic parameters)
        {
            if (parameters.CprCodeOld != null)
                this.CprCodeOld = parameters.CprCodeOld;
            if (parameters.CprCodeNew != null)
                this.CprCodeNew = parameters.CprCodeNew;
            if (parameters.CprValue != null)
                this.CprValue = parameters.CprValue;
            if (parameters.CprUrl != null)
                this.CprUrl = parameters.CprUrl;
        }
    }

    /// <summary>
    /// DTO for Copyright delete
    /// </summary>
    internal class Copyright_DTO_Delete
    {
        /// <summary>
        /// Copyright Code
        /// </summary>
        [UpperCase]
        public string CprCode { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Copyright_DTO_Delete(dynamic parameters)
        {
            if (parameters.CprCode != null)
                this.CprCode = parameters.CprCode;

        }
    }

    public interface ICopyright
    {
        string CprCode { get; set; }

        string CprValue { get; set; }

        string CprUrl { get; set; }
    }
}
