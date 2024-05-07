using API;
using System.Collections.Generic;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// IADO for Alerts
    /// </summary>
    internal class Alert_ADO
    {
        /// <summary>
        /// Read Alerts
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="readLiveOnly"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(IADO ado, Alert_DTO dto, bool readLiveOnly)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@ReadLiveOnly",value=readLiveOnly},
                new ADO_inputParams() {name="@LngIsoCode",value=dto.LngIsoCode  }
            };

            if (dto.LrtCode != default(int))
                inputParamList.Add(new ADO_inputParams() { name = "@LrtCode", value = dto.LrtCode });

            var reader = ado.ExecuteReaderProcedure("System_Navigation_Alert_Read", inputParamList);

            return reader;

        }

        /// <summary>
        /// Create Alert
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Create(IADO ado, Alert_DTO dto, string userName)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LrtMessage",value=dto.LrtMessage},
                new ADO_inputParams() {name= "@LrtDatetime",value=dto.LrtDatetime },
                new ADO_inputParams() {name= "@CcnUsername",value=userName }
            };

            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("System_Navigation_Alert_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Update Alert
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Update(IADO ado, Alert_DTO dto, string userName)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LrtCode",value=dto.LrtCode},
                new ADO_inputParams() {name= "@LrtMessage",value=dto.LrtMessage},
                new ADO_inputParams() {name= "@LrtDatetime",value=dto.LrtDatetime },
                new ADO_inputParams() {name= "@CcnUsername",value=userName }
            };

            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("System_Navigation_Alert_Update", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Delete Alert
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Delete(IADO ado, Alert_DTO dto, string userName)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LrtCode",value=dto.LrtCode},
                new ADO_inputParams() {name= "@CcnUsername",value=userName }
            };

            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("System_Navigation_Alert_Delete", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }
    }
}