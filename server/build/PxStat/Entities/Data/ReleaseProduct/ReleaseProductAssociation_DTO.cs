using System;

namespace PxStat.Data
{
    /// <summary>
    /// DTO class for ReleaseProduct Read
    /// </summary>
    internal class ReleaseProduct_DTO_Read
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Language code
        /// </summary>
        public string LngIsoCode { get; internal set; }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public ReleaseProduct_DTO_Read(dynamic parameters)
        {
            if (parameters.RlsCode != null)
            {
                this.RlsCode = parameters.RlsCode;
            }
            if (parameters.LngIsoCode != null)
            {
                this.LngIsoCode = parameters.LngIsoCode;
            }
        }

        /// <summary>
        /// Blank constructor
        /// </summary>
        public ReleaseProduct_DTO_Read() { }

    }

    /// <summary>
    /// DTO for ReleaseProduct Create
    /// </summary>
    internal class ReleaseProduct_DTO_Create
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Product Code
        /// </summary>
        public string PrcCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public ReleaseProduct_DTO_Create(dynamic parameters)
        {
            if (parameters.RlsCode != null)
            {
                this.RlsCode = parameters.RlsCode;
            }
            if (parameters.PrcCode != null)
            {
                this.PrcCode = parameters.PrcCode;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class ReleaseProductAssociation_DTO_Delete
    {
        /// <summary>
        /// Release Code
        /// </summary>
        public int RlsCode { get; set; }

        /// <summary>
        /// Product Code
        /// </summary>
        public string PrcCode { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public ReleaseProductAssociation_DTO_Delete(dynamic parameters)
        {
            if (parameters.RlsCode != null)
            {
                this.RlsCode = parameters.RlsCode;
            }
            if (parameters.PrcCode != null)
            {
                this.PrcCode = parameters.PrcCode;
            }
        }
        /// <summary>
        /// Blank Constructor
        /// </summary>
        public ReleaseProductAssociation_DTO_Delete() { }
    }
}