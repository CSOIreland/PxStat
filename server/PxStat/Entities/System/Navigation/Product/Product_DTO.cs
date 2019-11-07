
using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// DTO for Product
    /// </summary>
    internal class Product_DTO
    {
        #region Properties
        /// <summary>
        /// Product Code
        /// </summary>
        public string PrcCode { get; internal set; }

        /// <summary>
        /// The new product code when a product code is being updated
        /// </summary>
        public string PrcCodeNew { get; internal set; }

        /// <summary>
        /// Subject Code
        /// </summary>
        public int SbjCode { get; private set; }

        /// <summary>
        /// Product Value
        /// </summary>
        public string PrcValue { get; set; }

        /// <summary>
        /// ISO Language Code
        /// </summary>
        public string LngIsoCode { get; set; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parameters"></param>
        public Product_DTO(dynamic parameters)
        {
            if (parameters.PrcCode != null)
                this.PrcCode = parameters.PrcCode;

            if (parameters.PrcCodeNew != null)
                this.PrcCodeNew = parameters.PrcCodeNew;

            if (parameters.SbjCode != null)
                this.SbjCode = parameters.SbjCode;

            if (parameters.PrcValue != null)
                this.PrcValue = parameters.PrcValue;

            if (parameters.LngIsoCode != null)
                this.LngIsoCode = parameters.LngIsoCode;
            else
                this.LngIsoCode = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE");
        }
    }
}
