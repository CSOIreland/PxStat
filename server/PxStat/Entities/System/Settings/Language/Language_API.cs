using System;
using Newtonsoft.Json;
using FluentValidation.Results;
using API;

namespace PxStat.System.Settings
{
    /// <summary>
    /// The Language_BSO class serves as an entry point for the publicly available methods for managing Language entities.
    /// APIs to control creating, deleting, updating and reading Languages.
    /// </summary>
    /// 
    [AllowAPICall]
    public class Language_API
    {
        /// <summary>
        /// Returns the list of languages for a release code
        /// </summary>
        /// <param name="jsonrpcRequest">The request param</param>
        /// <returns>TODO</returns>
        public static dynamic ReadListByRelease(JSONRPC_API jsonrpcRequest)
        {
            return new Language_BSO_ReadListByRelease(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// The Read method is used to return one or more Languages.
        /// This has a public anonymous access.
        /// If a valid Language ISO code is passed in, the method will attempt to read a corresponding entry from the database
        /// If no ISO code is passed in, the method will return all languages.
        /// If no data found, a null is returned.
        /// <param name="jsonrpcRequest">The request param</param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Language_BSO_Read(requestApi).Read().Response;
        }

        /// <summary>
        /// The Create method must receive LngIsoCode and LngIsoName as parameters
        /// This has restricted access to Aministrator or Power User only.
        /// If these are validated successfully, and the LngIsoCode doesn't already exist 
        /// then this method will create the new entry.
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new Language_BSO_Create(requestApi).Create().Response;
        }

        /// <summary>
        /// Aministrator or Power user only
        /// Updates a language entity. Only the LngIsoName can be changed.
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        /// 
        public static dynamic Update(JSONRPC_API requestApi)
        {
            return new Language_BSO_Update(requestApi).Update().Response;
        }

        /// <summary>
        /// Aministrator or Power user only
        /// This method will delete an entity. 
        /// A valid LngIsoCode must be supplied by the client.
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new Language_BSO_Delete(requestApi).Delete().Response;
        }
    }
}