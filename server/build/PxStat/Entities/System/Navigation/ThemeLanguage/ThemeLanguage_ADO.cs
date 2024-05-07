using API;
using System.Collections.Generic;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// IADO class for SubjectLanguage
    /// </summary>
    internal class ThemeLanguage_ADO
    {
        /// <summary>
        /// IADO class parameter
        /// </summary>
        private IADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        internal ThemeLanguage_ADO(IADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Create or update a Theme Language
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal int CreateOrUpdate(ThemeLanguage_DTO dto)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@ThmCode",value= dto.ThmCode },
                    new ADO_inputParams() {name ="@TlgValue",value= dto.ThmValue  },
                    new ADO_inputParams() {name="@ThmIsoCode",value=dto.LngIsoCode  }
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_ThemeLanguage_CreateOrUpdate", inputParams, ref returnParam);

            return (int)returnParam.value;
        }
    }
}
