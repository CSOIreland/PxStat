using API;

namespace PxStat.Data
{
    /// <summary>
    /// The Compare APIs allow similar releases to be compared and differences in the releases to be viewed.
    /// </summary>
    [AllowAPICall]
    public class Compare_API
    {
        /// <summary>
        /// Reads data to the supplied Release Code. Data include "Corrected flag" ("status") for each record.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        /// 
        [CacheRead(CAS_REPOSITORY = Resources.Constants.C_CAS_DATA_COMPARE_READ_AMENDMENT, DOMAIN = "RlsCode")]
        public static dynamic ReadAmendment(JSONRPC_API jsonrpcRequest)
        {
            return new Compare_BSO_ReadAmendment(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Reads data to the supplied Release Code. Data include "Deleted flag" ("status") for each record.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheRead(CAS_REPOSITORY = Resources.Constants.C_CAS_DATA_COMPARE_READ_DELETION, DOMAIN = "RlsCode")]
        public static dynamic ReadDeletion(JSONRPC_API jsonrpcRequest)
        {
            return new Compare_BSO_ReadDeletion(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Reads data to the supplied Release Code. Data include "Added flag" ("status") for each record.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheRead(CAS_REPOSITORY = Resources.Constants.C_CAS_DATA_COMPARE_READ_ADDITION, DOMAIN = "RlsCode")]
        public static dynamic ReadAddition(JSONRPC_API jsonrpcRequest)
        {
            return new Compare_BSO_ReadAddition(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Reads the Previous Release Code to the supplied Release code.
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadPreviousRelease(JSONRPC_API jsonrpcRequest)
        {
            return new Compare_BSO_ReadPreviousRelease(jsonrpcRequest).Read().Response;
        }
    }
}
