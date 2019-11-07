namespace PxStat.Security
{
    /// <summary>
    /// DTO class for Account Read
    /// </summary>
    internal class Account_DTO_Read
    {
        #region Properties
        /// <summary>
        /// Account user name
        /// </summary>
        public string CcnUsername { get; set; }

        /// <summary>
        /// Privilege Code
        /// </summary>
        public string PrvCode { get; set; }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public Account_DTO_Read() { }
        #endregion

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="parameters"></param>
        public Account_DTO_Read(dynamic parameters)
        {
            if (parameters.CcnUsername != null)
                this.CcnUsername = parameters.CcnUsername;
            if (parameters.PrvCode != null)
                this.PrvCode = parameters.PrvCode;
        }

    }

    /// <summary>
    /// DTO for Account Create
    /// </summary>
    internal class Account_DTO_Create
    {
        #region Properties
        /// <summary>
        /// Account username
        /// </summary>
        public string CcnUsername { get; set; }

        /// <summary>
        ///  Privilege code
        /// </summary>
        public string PrvCode { get; set; }
        /// <summary>
        /// Notification flag (allows group communications from the application)
        /// </summary>
        public bool CcnNotificationFlag { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Account_DTO_Create(dynamic parameters)
        {
            if (parameters.CcnUsername != null)
                this.CcnUsername = parameters.CcnUsername;
            if (parameters.PrvCode != null)
                this.PrvCode = parameters.PrvCode;
            if (parameters.CcnNotificationFlag != null)
                this.CcnNotificationFlag = parameters.CcnNotificationFlag;
            else
                this.CcnNotificationFlag = true;
        }
    }

    /// <summary>
    /// DTO class for account delete
    /// </summary>
    internal class Account_DTO_Delete
    {
        /// <summary>
        /// Account username
        /// </summary>
        public string CcnUsername { get; set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Account_DTO_Delete(dynamic parameters)
        {
            if (parameters.CcnUsername != null)
                this.CcnUsername = parameters.CcnUsername;
        }
    }

    /// <summary>
    /// DTO class for Account update
    /// </summary>
    internal class Account_DTO_Update
    {
        /// <summary>
        /// Account username
        /// </summary>
        public string CcnUsername { get; set; }
        /// <summary>
        /// Privilege Code
        /// </summary>
        public string PrvCode { get; set; }
        /// <summary>
        /// Notification flag (allows group communications from the application)
        /// </summary>
        public bool CcnNotificationFlag { get; set; }



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Account_DTO_Update(dynamic parameters)
        {
            if (parameters.CcnUsername != null)
                this.CcnUsername = parameters.CcnUsername;
            if (parameters.PrvCode != null)
                this.PrvCode = parameters.PrvCode;
            if (parameters.CcnNotificationFlag != null)
                this.CcnNotificationFlag = parameters.CcnNotificationFlag;
            else
                this.CcnNotificationFlag = true;
        }

        public Account_DTO_Update()
        {
        }
    }



    /// <summary>
    /// DTO class for ReadIsApprover
    /// </summary>
    internal class Account_DTO_ReadIsApprover
    {
        /// <summary>
        /// Account username
        /// </summary>
        public string CcnUsername { get; internal set; }
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; internal set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Account_DTO_ReadIsApprover(dynamic parameters)
        {
            if (parameters.CcnUsername != null)
                CcnUsername = parameters.CcnUsername;
            if (parameters.RlsCode != null)
                RlsCode = parameters.RlsCode;
        }
    }
}