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

    public class Account_DTO_Lock
    {
        public string CcnUsername { get; set; }
        public bool CcnLockedFlag { get; set; }

        public Account_DTO_Lock(dynamic parameters)
        {
            if (parameters.CcnUsername != null)
                CcnUsername = parameters.CcnUsername;
            if (parameters.CcnLockedFlag != null)
                CcnLockedFlag = parameters.CcnLockedFlag;
        }
    }

    internal class Account_DTO_CreateLocal
    {
        public string CcnEmail { get; set; }
        public string CcnDisplayName { get; set; }
        public string PrvCode { get; set; }
        public bool CcnNotificationFlag { get; set; }
        public string LngIsoCode { get; set; }
        public string CcnUsername { get; set; }

        public Account_DTO_CreateLocal(dynamic parameters)
        {
            if (parameters.CcnEmail != null)
            {
                this.CcnEmail = parameters.CcnEmail;
                this.CcnUsername = parameters.CcnEmail;
            }
            if (parameters.CcnDisplayName != null)
                this.CcnDisplayName = parameters.CcnDisplayName;
            if (parameters.PrvCode != null)
                this.PrvCode = parameters.PrvCode;
            if (parameters.CcnNotificationFlag != null)
                this.CcnNotificationFlag = parameters.CcnNotificationFlag;
            else
                this.CcnNotificationFlag = true;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");
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

        public string LngIsoCode { get; set; }

        public string CcnDisplayName { get; set; }

        public string CcnEmail { get; set; }

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
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;
            else
                LngIsoCode = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code");

        }

        public Account_DTO_Create()
        {
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

        public bool? CcnLockedFlag { get; set; }

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
            if (parameters.CcnLockedFlag != null)
                CcnLockedFlag = parameters.CcnLockedFlag;
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

    public class Account_DTO_Confirm
    {
        public string Email { get; set; }
        public string InvitationToken { get; set; }

        public Account_DTO_Confirm(dynamic parameters)
        {
            if (parameters.Email != null)
                this.Email = parameters.Email;
            if (parameters.Token != null)
                this.InvitationToken = parameters.Token;
        }
    }

    public class Account_DTO_Login
    {
        public string Email { get; set; }
        public string TwofactorCode { get; set; }
        public string Password { get; internal set; }


        public Account_DTO_Login()
        {

        }
    }
}
