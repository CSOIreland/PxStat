using API;
using PxStat.Data;
using PxStat.DataStore;
using System;
using System.Collections.Generic;

namespace PxStat.Security
{
    /// <summary>
    /// ADO methods for Database reading
    /// </summary>
    internal class Database_ADO
    {

        /// <summary>
        /// Read indices from database
        /// </summary>
        /// <param name="ado"></param>

        /// <returns></returns>
        internal ADO_readerOutput Read(IADO ado, Database_DTO_Read databaseParam)
        {


            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();

            if (databaseParam.TableName != null)
            {
                inputParamList.Add(new ADO_inputParams() { name = "@TableName", value = databaseParam.TableName });
            }
            var reader = ado.ExecuteReaderProcedure("Security_Database_ReadStatistic", inputParamList);

            return reader;

        }

        internal ADO_readerOutput ReadTableList(ADO ado, Database_DTO_Update databaseParam)
        {


            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {

            };

            var reader = ado.ExecuteReaderProcedure("Security_Database_ReadTableList", inputParamList);

            return reader;

        }
    }
}
