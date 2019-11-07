using API;
using PxStat.System.Navigation;
using System.Collections.Generic;


namespace PxStat.Entities.System.Navigation.AlertLanguage
{
    /// <summary>
    /// ADO functions for other language versions of alerts
    /// </summary>
    internal class AlertLanguage_ADO
    {
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        internal AlertLanguage_ADO(ADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Creates a new language version or updates an existing language version of an alert
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal int CreateOrUpdate(Alert_DTO dto)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@LrtCode",value= dto.LrtCode  },
                    new ADO_inputParams() {name ="@LlnMessage",value= dto.LrtMessage   },
                    new ADO_inputParams() {name="@LngIsoCode",value=dto.LngIsoCode  }
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Alert_Language_CreateOrUpdate", inputParams, ref returnParam);

            return (int)returnParam.value;
        }
    }
}