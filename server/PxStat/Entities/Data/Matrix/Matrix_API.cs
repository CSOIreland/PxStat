using API;
using System;

namespace PxStat.Data
{
    /// <summary>
    /// These APIs are used for uploading, validating and reading px file data.
    /// </summary>
    public class Matrix_API
    {
        /// <summary>
        /// Uploads the input source and create a new matrix and release if there are no duplicates, or if the overwrite param is yes.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns>The <seealso cref="JSONRPC_Output"/> object with a list of errors or the data object in case of success. </returns>
        /// 
        //CacheFlush internally executed
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {

            var response = new Matrix_BSO_Create(jsonrpcRequest).Create().Response;

            if (response.error == null)
            {
                runDeletes();
            }

            return response;
        }

        /// <summary>
        /// Run a cleanup function to hard delete unwanted 
        /// </summary>
        private static void runDeletes()
        {
            ADO defaultADO = new ADO("defaultConnection");
            ADO adoBatch = new ADO("msdbConnection");

            try

            {

                Matrix_ADO defaultMatrixAdo = new Matrix_ADO(defaultADO);

                string processName = "DataMatrixDeleteEntities_" + defaultMatrixAdo.getDbName();

                if (!defaultMatrixAdo.IsProcessRunning(processName))
                {

                    Matrix_ADO mAdo = new Matrix_ADO(adoBatch);


                    mAdo.DeleteEntities(processName);
                    adoBatch.CloseConnection();
                    adoBatch.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                defaultADO.CloseConnection();
                defaultADO.Dispose();
                adoBatch.CloseConnection();
                adoBatch.Dispose();
            }

        }

        /// <summary>
        /// Only validates the input source
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns>The <seealso cref="JSONRPC_Output"/> object with a list of errors or the data object in case of success. </returns>
        public static dynamic Validate(JSONRPC_API jsonrpcRequest)
        {
            return new Matrix_BSO_Validate(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Returns the list of matrices base on the user profile
        /// </summary>
        /// <param name="jsonrpcRequest">The request param</param>
        /// <returns>TODO</returns>
        public static dynamic ReadCodeList(JSONRPC_API jsonrpcRequest)
        {
            return new Matrix_BSO_ReadCodeList(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Get a list of Matrix codes based on usage of products
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadByProduct(JSONRPC_API jsonrpcRequest)
        {
            return new Matrix_BSO_ReadByProduct(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Returns the Matrix for a given release code and language
        /// </summary>
        /// <param name="jsonrpcRequest">The request containing the matrix code</param>
        /// <returns>TODO</returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Matrix_BSO_Read(jsonrpcRequest).Read().Response;
        }


        /// <summary>
        /// Returns the history of uploads for a given time period
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadHistory(JSONRPC_API jsonrpcRequest)
        {
            return new Matrix_BSO_ReadHistory(jsonrpcRequest).Read().Response;
        }
    }
}
