using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// BSO for ProductLanguage - for maintaining other language versions of product names
    /// </summary>
    internal class ProductLangauge_BSO
    {
        /// <summary>
        /// Create or update a product language
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="Ado"></param>
        /// <returns></returns>
        internal int CreateOrUpdate(Product_DTO dto, ADO Ado)
        {
            ProductLanguage_ADO adoProductLanguage = new ProductLanguage_ADO(Ado);
            ProductLanguage_DTO dtoProductLanguage = new ProductLanguage_DTO();
            dtoProductLanguage.PrcCode = dto.PrcCode;
            dtoProductLanguage.LngIsoCode = dto.LngIsoCode;
            dtoProductLanguage.PlgValue = dto.PrcValue;
            //If the productID is already represented in this language then an update is performed
            //Otherwise do an insert
            return adoProductLanguage.CreateOrUpdate(dtoProductLanguage);
        }

    }
}