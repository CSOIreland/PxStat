using API;

namespace PxStat.Data
{
    /// <summary>
    /// Cube APIs are used for reading data and metadata.
    /// </summary>
    public class Cube_API
    {

        /// <summary>
        /// Reads a live dataset based on specific criteria. 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [Analytic]
        //Internal cache management
        public static dynamic ReadDataset(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadDataset(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Reads the metadata for a live dataset based on specific criteria.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>

        public static dynamic ReadMetadata(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadMetadata(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Reads any Dataset (including pre-release data) based on Release Code and other criteria. 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheRead(CAS_REPOSITORY =
            Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET, DOMAIN = "release")]
        public static dynamic ReadPreDataset(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadPreDataset(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Reads the metadata for a non live dataset based on Release code and other criteria. 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheRead(CAS_REPOSITORY = Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA, DOMAIN = "release")]
        public static dynamic ReadPreMetadata(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadPreMetadata(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Returns a Collection of JsonStat items.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadCollection(JSONRPC_API jsonrpcRequest)
        {
            return new Cube_BSO_ReadCollection(jsonrpcRequest).Read().Response;
        }
    }
}
