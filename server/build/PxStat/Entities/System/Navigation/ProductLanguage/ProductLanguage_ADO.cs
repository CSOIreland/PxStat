using System.Collections.Generic;
using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// ADO methods of ProductLanguage
    /// </summary>
    public class ProductLanguage_ADO
    {
        /// <summary>
        /// ADO class parameter
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        internal ProductLanguage_ADO(ADO ado)
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