using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// APIs to control creation, reading, updating and deleting of Subjects.
    /// </summary>
    [AllowAPICall]
    public class Subject_API
    {
        /// <summary>
        /// Creates a subject
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {
            return new Subject_BSO_Create(jsonrpcRequest).Create().Response;
        }

        /// <summary>
        /// Reads one or more subject
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Subject_BSO_Read(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Updates a subject
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_NAVIGATION_READ)]
        public static dynamic Update(JSONRPC_API jsonrpcRequest)
        {
            return new Subject_BSO_Update(jsonrpcRequest).Update().Response;
        }

        /// <summary>
        /// Deletes a subject
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API jsonrpcRequest)
        {
            return new Subject_BSO_Delete(jsonrpcRequest).Delete().Response;
        }
    }
}
