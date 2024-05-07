using API;
using PxStat.Security;
using System.Collections.Generic;

namespace PxStat.Data
{
    internal class GeoMap_ADO
    {
        /// <summary>
        /// IADO class parameter
        /// </summary>
        private IADO ado;

        internal GeoMap_ADO(IADO ado)
        {
            this.ado = ado;
        }

        internal int Create(GeoMap_DTO_Create dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@GmpCode",value= dto.GmpCode },
                    new ADO_inputParams() {name ="@GmpName",value=dto.GmpName  },
                    new ADO_inputParams() {name ="@GmpDescription",value=dto.GmpDescription },
                    new ADO_inputParams() {name ="@GmpGeoJson",value=dto.GmpGeoJson  },
                    new ADO_inputParams() {name ="@GmpFeatureCount",value=dto.GmpFeatureCount   },
                    new ADO_inputParams() {name ="@GlrCode",value=dto.GlrCode },
                     new ADO_inputParams() {name ="@CcnUsername",value= userName},

                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Geomap_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        internal int Update(GeoMap_DTO_Update dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@GmpCode",value= dto.GmpCode },
                    new ADO_inputParams() {name ="@GmpName",value=dto.GmpName  },
                    new ADO_inputParams() {name ="@GmpDescription",value=dto.GmpDescription },
                     new ADO_inputParams() {name ="@CcnUsername",value= userName},
                     new ADO_inputParams() {name="@GlrCode",value=dto.GlrCode}

                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Geomap_Update", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        internal ADO_readerOutput Read(string gmpCode)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@GmpCode",value=gmpCode}
            };



            var reader = ado.ExecuteReaderProcedure("Data_GeoMap_Read", inputParamList);

            return reader;

        }

        internal int Delete(GeoMap_DTO_Delete dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@GmpCode",value= dto.GmpCode },
                     new ADO_inputParams() {name ="@CcnUsername",value= userName},
                     new ADO_inputParams() {name ="@UrlStub",value= Configuration_BSO.GetApplicationConfigItem(ConfigType.global ,"url.api.static") + "/PxStat.Data.GeoMap_API.Read/"}
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_GeoMap_Delete", inputParams, ref returnParam);

            return (int)returnParam.value;
        }


        internal ADO_readerOutput ReadCollection(string gmpCode = null, string glrCode = null)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@UrlStub",value=Configuration_BSO.GetApplicationConfigItem(ConfigType.global ,"url.api.static") + "/PxStat.Data.GeoMap_API.Read/"}
            };

            if (gmpCode != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@GmpCode", value = gmpCode });
            }
            if (glrCode != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@GlrCode", value = glrCode });
            }
            var reader = ado.ExecuteReaderProcedure("Data_GeoMap_ReadCollection", inputParamList);

            return reader;

        }
    }
}
