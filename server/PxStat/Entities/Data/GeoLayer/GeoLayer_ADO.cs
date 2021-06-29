using API;
using System.Collections.Generic;

namespace PxStat.Data
{
    internal class GeoLayer_ADO
    {
        /// <summary>
        /// ADO class parameter
        /// </summary>
        private ADO ado;
        internal GeoLayer_ADO(ADO ado)
        {
            this.ado = ado;
        }

        internal int Create(GeoLayer_DTO_Create dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@GlrName",value=dto.GlrName  },
                     new ADO_inputParams() {name ="@CcnUsername",value= userName},

                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_GeoLayer_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }
        /// <summary>
        /// Reads one or more GeoLayer
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal List<dynamic> Read(GeoLayer_DTO_Read dto)
        {
            var inputParams = new List<ADO_inputParams>();



            if (dto.GlrCode != null)
                inputParams.Add(new ADO_inputParams() { name = "@GlrCode", value = dto.GlrCode });

            if (dto.GlrName != null)
                inputParams.Add(new ADO_inputParams() { name = "@GlrName", value = dto.GlrName });


            return ado.ExecuteReaderProcedure("Data_GeoLayer_Read", inputParams).data;


        }

        internal int Update(GeoLayer_DTO_Update dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@GlrCode",value= dto.GlrCode },
                    new ADO_inputParams() {name ="@GlrName",value=dto.GlrName  },
                     new ADO_inputParams() {name ="@CcnUsername",value= userName},

                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_GeoLayer_Update", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        internal int Delete(GeoLayer_DTO_Delete dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@GlrCode",value= dto.GlrCode },
                     new ADO_inputParams() {name ="@CcnUsername",value= userName}

                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_GeoLayer_Delete", inputParams, ref returnParam);

            return (int)returnParam.value;
        }
    }
}
