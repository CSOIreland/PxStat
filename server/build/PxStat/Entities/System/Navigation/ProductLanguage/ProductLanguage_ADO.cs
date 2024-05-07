using System.Collections.Generic;
using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// IADO methods of ProductLanguage
    /// </summary>
    public class ProductLanguage_ADO
    {
        /// <summary>
        /// IADO class parameter
        /// </summary>
        private IADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        internal ProductLanguage_ADO(IADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Create or update a product language
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal int CreateOrUpdate(ProductLanguage_DTO dto)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@PlgValue",value= dto.PlgValue},
                    new ADO_inputParams() {name ="@PlgIsoCode",value= dto.LngIsoCode },
                    new ADO_inputParams() {name="@PlgPrcCode",value=dto.PrcCode }
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_ProductLanguage_CreateOrUpdate", inputParams, ref returnParam);

            return (int)returnParam.value;
        }
    }
}