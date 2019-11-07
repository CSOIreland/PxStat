using System;

namespace PxStat.Security
{
    /// <summary>
    /// DTO class for GroupAccount
    /// </summary>
    internal class GroupAccount_DTO
    {
        #region Properties
        /// <summary>
        /// Group Code
        /// </summary>
        public string GrpCode { get; set; }

        /// <summary>
        /// Account username
        /// </summary>
        internal string CcnUsername { get; set; }

        /// <summary>
        /// Group name
        /// </summary>
        public string GrpName { get; set; }

        /// <summary>
        /// Flag to indicate the user is an approver
        /// </summary>
        public bool GccApproveFlag { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public GroupAccount_DTO(dynamic parameters)
        {
            if (parameters.GrpCode != null)
                this.GrpCode = parameters.GrpCode;
            if (parameters.CcnUsername != null)
                this.CcnUsername = parameters.CcnUsername;
            if (parameters.GccApproveFlag != null)
                this.GccApproveFlag = Convert.ToBoolean(parameters.GccApproveFlag);

        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public GroupAccount_DTO() { }
    }

    /// <summary>
    /// DTO class for GroupAccount Read
    /// </summary>
    internal class GroupAccount_DTO_Read
    {
        /// <summary>
        /// Group Code
        /// </summary>
        public string GrpCode { get; set; }

        /// <summary>
        /// Account username
        /// </summary>
        internal string CcnUsername { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public GroupAccount_DTO_Read(dynamic parameters)
        {

            if (parameters.GrpCode != null)
                this.GrpCode = parameters.GrpCode;
            if (parameters.CcnUsername != null)
                this.CcnUsername = parameters.CcnUsername;

        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public GroupAccount_DTO_Read() { }

    }

    /// <summary>
    /// DTO for GroupAccount Create
    /// </summary>
    internal class GroupAccount_DTO_Create
    {
        /// <summary>
        /// Group Code
        /// </summary>
        public string GrpCode { get; set; }

        /// <summary>
        /// Account username
        /// </summary>
        internal string CcnUsername { get; set; }

        /// <summary>
        /// Flag to indicate the user is an approver
        /// </summary>
        public bool? GccApproveFlag { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public GroupAccount_DTO_Create(dynamic parameters)
        {

            if (parameters.GrpCode != null)
                this.GrpCode = parameters.GrpCode;
            if (parameters.CcnUsername != null)
                this.CcnUsername = parameters.CcnUsername;
            if (parameters.GccApproveFlag != null)
                this.GccApproveFlag = Convert.ToBoolean(parameters.GccApproveFlag);
        }
    }

    /// <summary>
    /// DTO for GroupAccount Update
    /// </summary>
    internal class GroupAccount_DTO_Update
    {
        /// <summary>
        /// Group Code
        /// </summary>
        public string GrpCode { get; set; }

        /// <summary>
        /// Account username
        /// </summary>
        internal string CcnUsername { get; set; }

        /// <summary>
        /// Group Name
        /// </summary>
        public string GrpName { get; set; }

        /// <summary>
        /// Flag to indicate the user is an approver
        /// </summary>
        public bool? GccApproveFlag { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public GroupAccount_DTO_Update(dynamic parameters)
        {
            if (parameters.GrpCode != null)
                this.GrpCode = parameters.GrpCode;
            if (parameters.CcnUsername != null)
                this.CcnUsername = parameters.CcnUsername;
            if (parameters.GccApproveFlag != null)
                this.GccApproveFlag = Convert.ToBoolean(parameters.GccApproveFlag);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class GroupAccount_DTO_Delete
    {
        /// <summary>
        /// Group Code
        /// </summary>
        public string GrpCode { get; set; }

        /// <summary>
        /// Account username
        /// </summary>
        internal string CcnUsername { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public GroupAccount_DTO_Delete(dynamic parameters)
        {

            if (parameters.GrpCode != null)
                this.GrpCode = parameters.GrpCode;
            if (parameters.CcnUsername != null)
                this.CcnUsername = parameters.CcnUsername;
        }
        /// <summary>
        /// Blank Constructor
        /// </summary>
        public GroupAccount_DTO_Delete() { }
    }
}