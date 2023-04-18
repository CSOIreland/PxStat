using API;
using PxStat.Data;
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
        internal ADO_readerOutput Read(ADO ado, Database_DTO_Read databaseParam)
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
        internal void UpdateIndexes(ADO ado)
        {


            ADO adoBatch = new ADO("msdbConnection");

            Matrix_ADO m = new Matrix_ADO(ado);
            string dbName = m.getDbName();

            string processName = "DataMatrixUpdateIndexes_" + dbName;

            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@job_name",value=processName }
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };
            if (!m.IsProcessRunning(processName))
            {

                try
                {
                    adoBatch.ExecuteNonQueryProcedure("sp_start_job", inputParams, ref returnParam);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    adoBatch.CloseConnection();
                    adoBatch.Dispose();
                }
            }



        }

    }
}
