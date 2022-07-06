using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// An Alert is a specific system message to be communicated to users. This API enables the creation and reading of Alerts.
    /// </summary>
    [AllowAPICall]
    public class Alert_API
    {
        /// <summary>
        /// Reads one or more Alerts, including alerts set in the future
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Alert_BSO_Read(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Reads one or more alerts but only if they are in the past
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadLive(JSONRPC_API jsonrpcRequest)
        {
            return new Alert_BSO_ReadLive(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Creates an alert
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {
            return new Alert_BSO_Create(jsonrpcRequest).Create().Response;
        }

        /// <summary>
        /// Updates an alert
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Update(JSONRPC_API jsonrpcRequest)
        {
            return new Alert_BSO_Update(jsonrpcRequest).Update().Response;
        }

        /// <summary>
        /// Deletes an alert
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API jsonrpcRequest)
        {
            return new Alert_BSO_Delete(jsonrpcRequest).Delete().Response;
        }
    }
}