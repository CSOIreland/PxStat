

namespace PxStat.Data
{
    /// <summary>
    /// Class
    /// </summary>
    internal class Classification_DTO_Search
    {
        /// <summary>
        /// class variable
        /// </summary>
        public string Search { get; set; }

        public string LngIsoCode { get; set; }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Classification_DTO_Search(dynamic parameters)
        {
            if (parameters.Search != null)
                Search = parameters.Search;
            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;

        }


    }

    /// <summary>
    /// Class
    /// </summary>
    internal class Classification_DTO_Read
    {
        public int ClsID { get; set; }
        public Classification_DTO_Read(dynamic parameters)
        {
            if (parameters.ClsID != null)
                ClsID = parameters.ClsID;
        }


    }

}