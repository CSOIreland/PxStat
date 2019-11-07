using API;

namespace PxStat.Workflow
{
    /// <summary>
    /// A Workflow Request is a user request to begin the workflow to create a live release. These APIs controls the Workflow Request process.
    /// </summary>
    public class Request_API
    {
        /// <summary>
        /// Entry point to the API method - Reads a Request
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Request_BSO_Read(jsonrpcRequest).Read().Response;
        }
    }
}