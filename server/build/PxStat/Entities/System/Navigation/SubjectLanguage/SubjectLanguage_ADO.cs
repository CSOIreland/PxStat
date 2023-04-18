using System.Collections.Generic;
using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// ADO class for SubjectLanguage
    /// </summary>
    internal class SubjectLanguage_ADO
    {
        /// <summary>
        /// ADO class parameter
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        internal SubjectLanguage_ADO(ADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Create or update a Subject Language
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal int CreateOrUpdate(SubjectLanguage_DTO dto)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@SbjCode",value= dto.SbjCode },
                    new ADO_inputParams() {name ="@SlgValue",value= dto.SlgValue  },
                    new ADO_inputParams() {name="@SlgIsoCode",value=dto.LngIsoCode  }
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_SubjectLanguage_CreateOrUpdate", inputParams, ref returnParam);

            return (int)returnParam.value;
        }
    }
}