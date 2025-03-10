﻿using API;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// A Keyword Product is a search keyword, related to an individual product, that is used to enable searches. 
    /// These APIs control the creation and reading of Keyword Products.
    /// </summary>
    [AllowAPICall]
    public class Keyword_Product_API
    {
        /// <summary>
        /// Creates a non-mandatory product keyword
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_NAVIGATION_SEARCH)]
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Product_BSO_Create(jsonrpcRequest).Create().Response;
        }

        /// <summary>
        /// Reads product keywords
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Product_BSO_Read(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Updates a product keyword
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_NAVIGATION_SEARCH)]
        public static dynamic Update(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Product_BSO_Update(jsonrpcRequest).Update().Response;
        }

        /// <summary>
        /// Deletes a product keyword
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        [CacheFlush(CAS_REPOSITORY_DOMAIN_LIST = Resources.Constants.C_CAS_NAVIGATION_SEARCH)]
        public static dynamic Delete(JSONRPC_API jsonrpcRequest)
        {
            return new Keyword_Product_BSO_Delete(jsonrpcRequest).Delete().Response;
        }
    }
}
