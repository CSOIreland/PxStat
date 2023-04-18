

namespace PxStat.Security
{
    /// <summary>
    /// DTO class for Group Read
    /// </summary>
    internal class Group_DTO_Read
    {
        #region Properties
        /// <summary>
        /// Group Code
        /// </summary>
        [UpperCase]
        public string GrpCode { get; set; }

        /// <summary>
        /// Account username
        /// </summary>
        public string CcnUsername { get; set; }
        #endregion

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="parameters"></param>
        public Group_DTO_Read(dynamic parameters)
        {
            if (parameters.GrpCode != null)
                this.GrpCode = parameters.GrpCode;

            if (parameters.CcnUsername != null)
                this.CcnUsername = parameters.CcnUsername;

        }

        /// <summary>
        /// Blank constructor 
        /// </summary>
        public Group_DTO_Read()
        {
        }
    }

    /// <summary>
    /// DTO class for Group Create
    /// </summary>
    internal class Group_DTO_Create
    {
        #region Properties
        /// <summary>
        /// Group Code
        /// </summary>
        [UpperCase]
        public string GrpCode { get; set; }

        /// <summary>
        /// Group Name
        /// </summary>
        public string GrpName { get; set; }

        /// <summary>
        /// Group Contact Name
        /// </summary>
        public string GrpContactName { get; set; }

        /// <summary>
        /// Group Contact Name
        /// </summary>
        public string GrpContactPhone { get; set; }

        /// <summary>
        /// Group Contact Email
        /// </summary>
        public string GrpContactEmail { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Group_DTO_Create(dynamic parameters)
        {
            if (parameters.GrpCode != null)
                this.GrpCode = parameters.GrpCode;
            if (parameters.GrpName != null)
                this.GrpName = parameters.GrpName;
            if (parameters.GrpContactName != null)
                this.GrpContactName = parameters.GrpContactName;
            if (parameters.GrpContactPhone != null)
                this.GrpContactPhone = parameters.GrpContactPhone;
            if (parameters.GrpContactEmail != null)
                this.GrpContactEmail = parameters.GrpContactEmail;
        }

        /// <summary>
        /// Blank Constructor
        /// </summary>
        public Group_DTO_Create()
        {
        }
    }

    /// <summary>
    /// DTO class for Group Update
    /// </summary>
    internal class Group_DTO_Update
    {
        #region Properties
        /// <summary>
        /// Old Group Code - in case the Group Code is being changed
        /// </summary>
        [UpperCase]
        public string GrpCodeOld { get; set; }

        /// <summary>
        /// New Group Code - in case the Group Code is being changed
        /// </summary>
        [UpperCase]
        public string GrpCodeNew { get; set; }

        /// <summary>
        /// Group Name
        /// </summary>
        public string GrpName { get; set; }

        /// <summary>
        /// Group Contact Name
        /// </summary>
        public string GrpContactName { get; set; }

        /// <summary>
        /// Group Contact Phone
        /// </summary>
        public string GrpContactPhone { get; set; }

        /// <summary>
        /// Group Contact Email
        /// </summary>
        public string GrpContactEmail { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Group_DTO_Update(dynamic parameters)
        {
            if (parameters.GrpCodeOld != null)
                this.GrpCodeOld = parameters.GrpCodeOld;
            if (parameters.GrpCodeNew != null)
                this.GrpCodeNew = parameters.GrpCodeNew;
            if (parameters.GrpName != null)
                this.GrpName = parameters.GrpName;
            if (parameters.GrpContactName != null)
                this.GrpContactName = parameters.GrpContactName;
            if (parameters.GrpContactPhone != null)
                this.GrpContactPhone = parameters.GrpContactPhone;
            if (parameters.GrpContactEmail != null)
                this.GrpContactEmail = parameters.GrpContactEmail;
        }

        /// <summary>
        /// Blank Constructor
        /// </summary>
        public Group_DTO_Update()
        {
        }
    }

    /// <summary>
    /// DTO class for Group Delete
    /// </summary>
    internal class Group_DTO_Delete
    {
        /// <summary>
        /// Group Code
        /// </summary>
        public string GrpCode { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Group_DTO_Delete(dynamic parameters)
        {
            if (parameters.GrpCode != null)
                this.GrpCode = parameters.GrpCode;
        }
    }
}