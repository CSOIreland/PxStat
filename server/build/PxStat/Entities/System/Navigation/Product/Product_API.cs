using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// APIs to control creation, reading, updating and deleting of Products.
    /// </summary>
    [AllowAPICall]
    public class Product_API
    {
        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {
            return new Product_BSO_Create(jsonrpcRequest).Create().Response;
        }

        /// <summary>
        /// Read a product
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Product_BSO_Read(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Update a product
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_NAVIGATION_READ)]
        public static dynamic Update(JSONRPC_API jsonrpcRequest)
        {
            return new Product_BSO_Update(jsonrpcRequest).Update().Response;
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API jsonrpcRequest)
        {
            return new Product_BSO_Delete(jsonrpcRequest).Delete().Response;
        }
    }
}
