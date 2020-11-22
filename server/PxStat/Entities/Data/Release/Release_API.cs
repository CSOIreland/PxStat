using API;

namespace PxStat.Data
{
    /// <summary>
    /// These APIs are concerned with reading and amending Releases.
    /// </summary>
    public class Release_API
    {
        /// <summary>
        /// Returns the complete information for a given release based on profile.
        /// </summary>
        /// <param name="jsonrpcRequest">The request containing the release code</param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Release_BSO_Read(jsonrpcRequest).Read().Response;
        }
        /// <summary>
        /// Reads the previous live Release
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadPrevious(JSONRPC_API jsonrpcRequest)
        {
            return new Release_BSO_ReadPrevious(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Returns the list of releases for a given matrix code based on the user profile
        /// </summary>
        /// <param name="jsonrpcRequest">The request containing the matrix code</param>
        /// <returns>TODO</returns>
        public static dynamic ReadList(JSONRPC_API jsonrpcRequest)
        {
            return new Release_BSO_ReadList(jsonrpcRequest).Read().Response;
        }

        public static dynamic ReadHasWipForLive(JSONRPC_API jsonrpcRequest)
        {
            return new Release_BSO_ReadWipForLive(jsonrpcRequest).Read().Response;
        }


        /// <summary>
        /// Updates the value of the Release Analytical flag
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST =
            Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + "/MtrCode,"
            + Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET + "/RlsCode,"
            + Resources.Constants.C_CAS_DATA_CUBE_READ_METADATA + "/MtrCode,"
            + Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA + "/RlsCode")]
        public static dynamic UpdateAnalyticalFlag(JSONRPC_API jsonrpcRequest)
        {
            return new Release_BSO_UpdateAnalyticalFlag(jsonrpcRequest).Update().Response;
        }



        /// <summary>
        /// Updates the product for a Release
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_NAVIGATION_READ + ","
            + Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET + "/RlsCode,"
            + Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA + "/RlsCode"
            )] // internal cache flush for live read (matrix code not available in the DTO at this point)
        public static dynamic UpdateProduct(JSONRPC_API jsonrpcRequest)
        {
            return new Release_BSO_UpdateProduct(jsonrpcRequest).Update().Response;
        }

        /// <summary>
        /// Updates a release comment
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET + "/RlsCode")]// internal cache flush for live read (matrix code not available in the DTO at this point)
        public static dynamic UpdateComment(JSONRPC_API jsonrpcRequest)
        {
            return new Release_BSO_UpdateComment(jsonrpcRequest).Update().Response;
        }

        /// <summary>
        /// Deletes a release comment
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET + "/RlsCode")]// internal cache flush for live read (matrix code not available in the DTO at this point)
        public static dynamic DeleteComment(JSONRPC_API jsonrpcRequest)
        {
            return new Release_BSO_DeleteComment(jsonrpcRequest).Delete().Response;
        }

    }
}
