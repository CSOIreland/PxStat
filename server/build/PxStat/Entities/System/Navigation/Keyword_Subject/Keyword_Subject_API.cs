using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// A Keyword Subject is a search keyword, related to an individual subject, that is used to enable searches. 
    /// These APIs control the creation and reading of Keyword Subjects.
    /// </summary>
    // should define interface to enforce correct method names and cases
    [AllowAPICall]
    public class Keyword_Subject_API
    {
        /// <summary>
        /// Creates a non-mandatory Subject Keyword
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_NAVIGATION_SEARCH)]
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Subject_BSO_Create(jsonrpcRequest).Create().Response;
        }

        /// <summary>
        /// Reads a subject keyword
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Subject_BSO_Read(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Updates a subject keyword
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_NAVIGATION_SEARCH)]
        public static dynamic Update(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Subject_BSO_Update(jsonrpcRequest).Update().Response;
        }

        /// <summary>
        /// Deletes a subject keyword
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_NAVIGATION_SEARCH)]
        public static dynamic Delete(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Subject_BSO_Delete(jsonrpcRequest).Delete().Response;
        }

    }
}
