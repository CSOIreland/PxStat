namespace PxStat.Security
{
    /// <summary>
    /// 
    /// </summary>
    internal class ActiveDirectory_DTO
    {
        #region Properties
        /// <summary>
        /// Account username
        /// </summary>
        public string CcnUsername { get; set; }

        /// <summary>
        /// Account user's name
        /// </summary>
        public string CcnName { get; set; }

        /// <summary>
        /// Account email address
        /// </summary>
        public string CcnEmail { get; set; }
        #endregion

        /// <summary>
        /// Constructor with parameters
        /// </summary>
        /// <param name="parameters"></param>
        public ActiveDirectory_DTO(dynamic parameters)
        {
            if (parameters.CcnUsername != null)
                this.CcnUsername = parameters.CcnUsername;
        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public ActiveDirectory_DTO() { }
    }
}