using API;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Data;

namespace PxStat.Data
{
    /// <summary>
    /// IADO functions for Cube
    /// </summary>
    internal class Cube_ADO
    {
        /// <summary>
        /// IADO class variable
        /// </summary>
        private IADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Cube_ADO(IADO ado)
        {
            this.ado = ado;
        }

        internal Dictionary<string, string> ReadDimensionRoles(string contvariable, string matrix = null, int rlsCode = 0, string lngIsoCode = null)
        {
            var inputParams = new List<ADO_inputParams>();

            Dictionary<string, string> dimDictionary = new Dictionary<string, string>();

            inputParams.Add(new ADO_inputParams { name = "@ContentVariable", value = contvariable });

            if (matrix != null)
            {
                inputParams.Add(new ADO_inputParams { name = "@MtrCode", value = matrix });
            }
            else
            {
                inputParams.Add(new ADO_inputParams { name = "@RlsCode", value = rlsCode });

            }

            if (lngIsoCode != null)
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCode", value = lngIsoCode });
            }
            else
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCode", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") });

            var output = ado.ExecuteReaderProcedure("Data_Matrix_ReadDimensionRole", inputParams);

            foreach (var role in output.data)
            {

                if (!DBNull.Value.Equals(role.ROLE))
                {
                    if (!dimDictionary.ContainsKey(role.ROLE))
                        dimDictionary.Add(role.ROLE, role.CODE);
                }
            }

            return dimDictionary;


        }

        internal ADO_readerOutput ReadLiveAll()
        {
            var inputParams = new List<ADO_inputParams>();
            return ado.ExecuteReaderProcedure("Data_Matrix_Read_LiveAll", inputParams);
        }


        /// <summary>
        /// Reads the latest releases
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="DateFrom"></param>
        /// <returns></returns>
        internal List<dynamic> ReadListLive(string languageCode, DateTime DateFrom, int SbjCode, string PrcCode = null)
        {
            var inputParams = new List<ADO_inputParams>();

            inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") });

            if (!string.IsNullOrEmpty(languageCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeRead", value = languageCode });
            }

            if (DateFrom != default)
            {
                inputParams.Add(new ADO_inputParams { name = "@DateFrom", value = DateFrom });
            }


            inputParams.Add(new ADO_inputParams { name = "@SbjCode", value = SbjCode });
            inputParams.Add(new ADO_inputParams { name = "@PrcCode", value = PrcCode });


            var output = ado.ExecuteReaderProcedure("Data_Release_ReadListLive", inputParams);

            if (output.hasData)
            {
                return output.data;
            }

            return null;
        }

        /// <summary>
        /// Update title to contain dimensions and time range
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        internal List<dynamic> ReadTitleUpdate(DataTable dataTable)
        {
            var inputParams = new List<ADO_inputParams>();
            

            inputParams.Add(new ADO_inputParams { name = "@Placeholder", value = Configuration_BSO.GetStaticConfig("APP_PX_TITLE_BY") });
            var param = new ADO_inputParams
            {
                name = "@MatrixMap",
                value = dataTable,
                typeName = "KeyValueVarcharAttribute"
            };
            inputParams.Add(param);

            var output = ado.ExecuteReaderProcedure("Data_Read_Title_Update", inputParams);

            if (output.hasData)
            {
                return output.data;
            }
            return null;
        }
        internal List<dynamic> ReadCollection(string languageCode, DateTime DateFrom, string PrcCode = null, bool meta = true)
        {
            var inputParams = new List<ADO_inputParams>();

            inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code") });

            if (!string.IsNullOrEmpty(languageCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeRead", value = languageCode });
            }

            if (DateFrom != default(DateTime))
            {
                inputParams.Add(new ADO_inputParams { name = "@DateFrom", value = DateFrom });
            }

            if (PrcCode != null)
            {
                inputParams.Add(new ADO_inputParams { name = "@PrcCode", value = PrcCode });
            }

            ADO_readerOutput output;
            if (meta)
                output = ado.ExecuteReaderProcedure("Data_Release_ReadMetaCollection", inputParams);
            else
                output = ado.ExecuteReaderProcedure("Data_Release_ReadCollection", inputParams);

            if (output.hasData)
            {
                return output.data;
            }

            return null;
        }


    }
}
