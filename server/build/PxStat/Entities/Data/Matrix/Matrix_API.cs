using API;
using PxStat.DBuild;
using System;

namespace PxStat.Data
{
    /// <summary>
    /// These APIs are used for uploading, validating and reading px file data.
    /// </summary>
    [AllowAPICall]
    public class Matrix_API
    {
        /// <summary>
        /// Uploads the input source and create a new matrix and release if there are no duplicates, or if the overwrite param is yes.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns>The <seealso cref="JSONRPC_Output"/> object with a list of errors or the data object in case of success. </returns>
        /// 
        //CacheFlush internally executed
        [NoCleanseDto]
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {
            var response = new Matrix_BSO_Create(jsonrpcRequest).Create().Response;

            return response;
        }

        /// <summary>
        /// Only validates the input source
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns>The <seealso cref="JSONRPC_Output"/> object with a list of errors or the data object in case of success. </returns>
        /// 
        [NoCleanseDto]
        public static dynamic Validate(JSONRPC_API jsonrpcRequest)
        {
            //return new Matrix_BSO_Validate(jsonrpcRequest).Read().Response;
            return new DBuild_BSO_Validate(jsonrpcRequest).Read().Response;
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

        public static dynamic ReadByGeoMap(JSONRPC_API jsonrpcRequest)
        {
            return new Matrix_BSO_ReadByGeoMap(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Get a list of Matrix codes based on assignment of groups to tables
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadByGroup(JSONRPC_API jsonrpcRequest)
        {
            return new Matrix_BSO_ReadByGroup(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Get a list of Matrix codes based on language
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadByLanguage(JSONRPC_API jsonrpcRequest)
        {
            return new Matrix_BSO_ReadByLanguage(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Get a list of Matrix codes based on assignment of copyrights to tables
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadByCopyright(JSONRPC_API jsonrpcRequest)
        {
            return new Matrix_BSO_ReadByCopyright(jsonrpcRequest).Read().Response;
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
