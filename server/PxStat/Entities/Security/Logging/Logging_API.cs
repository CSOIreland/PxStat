using API;

namespace PxStat.Security.Logging
{
    /// <summary>
    /// API for reading logs
    /// </summary>
    [AllowAPICall]
    public class Logging_API
    {
        /// <summary>
        /// Read logs where logs have been sent to the database
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Logging_BSO_Read(jsonrpcRequest).Read().Response;
        }
    }
}