using API;
using System.Collections.Generic;

namespace PxStat.System.Navigation
{
    public class Theme_ADO
    {
        /// <summary>
        /// ADO class parameter
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Theme_ADO(ADO ado)
        {
            this.ado = ado;
        }

        internal int Update(Theme_DTO_Update dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@ThmCode",value= dto.ThmCode},
                    new ADO_inputParams() {name ="@ThmValue",value= dto.ThmValue},
                     new ADO_inputParams() {name ="@userName",value= userName}
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            var outParam = new ADO_outputParam() { name = "@ThmId", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Theme_Update", inputParams, ref returnParam, ref outParam);

            return (int)outParam.value;
        }

        internal bool UpdateExists(Theme_DTO_Read dto)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@ThmValue",value= dto.ThmValue},
                     new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode  }
                };
            var reader = ado.ExecuteReaderProcedure("System_Navigation_Theme_Read", inputParams);
            return reader.data.Find(e => e.ThmValue == dto.ThmValue && e.ThmCode != dto.ThmCode) != null;
        }

        internal int Delete(Theme_DTO_Delete dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@ThmCode",value= dto.ThmCode},
                     new ADO_inputParams() {name ="@userName",value= userName},
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Theme_Delete", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        /// <summary>
        /// Reads one or more theme
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal List<dynamic> Read(Theme_DTO_Read dto)
        {
            var inputParams = dto.ThmCode == 0
                ? new List<ADO_inputParams>()
                : new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@ThmCode",value= dto.ThmCode }
                };


            if (dto.LngIsoCode != null)
                inputParams.Add(new ADO_inputParams() { name = "@LngIsoCode", value = dto.LngIsoCode });

            if (dto.ThmValue != null)
                inputParams.Add(new ADO_inputParams() { name = "@ThmValue", value = dto.ThmValue });

            var reader = ado.ExecuteReaderProcedure("System_Navigation_Theme_Read", inputParams);

            return reader.data;
        }

        internal bool Exists(Theme_DTO_Read dto)
        {
            return this.Read(dto).Count > 0;

        }

        internal int Create(Theme_DTO_Create dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@ThmValue",value= dto.ThmValue},
                     new ADO_inputParams() {name ="@userName",value= userName},
                     new ADO_inputParams() {name ="@LngIsoCode",value=dto.LngIsoCode  }
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("System_Navigation_Theme_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }
    }
}
