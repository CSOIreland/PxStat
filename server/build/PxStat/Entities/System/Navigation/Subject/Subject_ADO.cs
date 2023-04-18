using API;
using PxStat.Security;
using System.Collections.Generic;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// ADO class for Subject
    /// </summary>
    internal class Subject_ADO
    {
        /// <summary>
        /// ADO class parameter
        /// </summary>
        private IADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Subject_ADO(IADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Create a Subject
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Create(Subject_DTO dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@SbjValue",value= dto.SbjValue},
                     new ADO_inputParams() {name ="@userName",value= userName},
                     new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode  },
                     new ADO_inputParams(){name="@ThmCode",value=dto.ThmCode }
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Subject_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        /// <summary>
        /// Test if a subject is in place
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal bool Exists(Subject_DTO dto)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@SbjValue",value= dto.SbjValue},
                     new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode  }
                };
            var reader = ado.ExecuteReaderProcedure("System_Navigation_Subject_Read", inputParams);
            return reader.hasData;
        }

        /// <summary>
        /// A version of Exists more suitable for update. It tests whether the SbjValue exists on a subject but
        /// ignores the subject we're trying to update.
        /// </summary>
        /// <param name="prcValue"></param>
        /// <param name="prcCode"></param>
        /// <returns></returns>
        internal bool UpdateExists(Subject_DTO dto)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@SbjValue",value= dto.SbjValue},
                     new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode  }
                };
            var reader = ado.ExecuteReaderProcedure("System_Navigation_Subject_Read", inputParams);
            return reader.data.Find(e => e.SbjValue == dto.SbjValue && e.SbjCode != dto.SbjCode) != null;
        }

        /// <summary>
        /// Reads one or more subject
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal List<dynamic> Read(Subject_DTO dto)
        {
            var inputParams = dto.SbjCode == 0
                ? new List<ADO_inputParams>()
                : new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@SbjCode",value= dto.SbjCode}
                };

            if (dto.LngIsoCode != null)
                inputParams.Add(new ADO_inputParams() { name = "@LngIsoCode", value = dto.LngIsoCode });
            else
                inputParams.Add(new ADO_inputParams() { name = "@LngIsoCode", value = Configuration_BSO.GetCustomConfig(ConfigType.global,"language.iso.code")});

            var reader = ado.ExecuteReaderProcedure("System_Navigation_Subject_Read", inputParams);

            return reader.data;
        }

        /// <summary>
        /// Updates a subject
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Update(Subject_DTO dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@SbjCode",value= dto.SbjCode},
                    new ADO_inputParams() {name ="@SbjValue",value= dto.SbjValue},
                     new ADO_inputParams() {name ="@userName",value= userName},
                     new ADO_inputParams(){ name ="@ThmCode",value=dto.ThmCode }
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            var outParam = new ADO_outputParam() { name = "@SbjId", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Subject_Update", inputParams, ref returnParam, ref outParam);

            return (int)outParam.value;
        }

        /// <summary>
        /// Deletes a subject
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Delete(Subject_DTO dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@SbjCode",value= dto.SbjCode},
                     new ADO_inputParams() {name ="@userName",value= userName},
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Subject_Delete", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

    }
}
