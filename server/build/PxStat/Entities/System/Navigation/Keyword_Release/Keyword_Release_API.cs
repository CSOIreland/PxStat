using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// A Keyword Release is a search keyword, related to an individual Release, that is used to enable searches. 
    /// These APIs control the creation and reading of Keyword Releases.
    /// </summary>
    [AllowAPICall]
    public class Keyword_Release_API
    {
        /// <summary>
        /// Creates a non-mandatory Release Keyword
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Release_BSO_Create(jsonrpcRequest).Create().Response;
        }

        /// <summary>
        /// Reads a Release Keyword
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Release_BSO_Read(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Deletes a Release Keyword
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Release_BSO_Delete(jsonrpcRequest).Delete().Response;
        }

        /// <summary>
        /// Updates a ReleaseKeyword
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Update(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Release_BSO_Update(jsonrpcRequest).Update().Response;
        }

    }
}