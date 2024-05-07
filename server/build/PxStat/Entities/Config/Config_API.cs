using System;
using Newtonsoft.Json;
using FluentValidation.Results;
using API;
using PxStat.Workflow;

namespace PxStat.Config
{
    /// <summary>
    /// The Config_API class serves as an entry point for the publicly available methods for managing Configuration entities.
    /// APIs to control reading and updating Configurations.
    /// </summary>
    /// 
    [AllowAPICall]
    public class Config_API
    {  
        /// <summary>
        /// The Read method is used to return one or more Configurations.
        /// This has a public anonymous access.
        /// <param name="jsonrpcRequest">The request param</param>
        /// <returns></returns>
        public static dynamic ReadApp(JSONRPC_API requestApi)
        {
            
            return new Config_BSO_ReadApp(requestApi).Read().Response;
        }

        public static dynamic ReadApi(JSONRPC_API requestApi)
        {
            return new Config_BSO_ReadApi(requestApi).Read().Response;
        }

        public static dynamic ReadApp(RESTful_API restfulRequestApi)
        { 
            return new Config_BSO_ReadApp(restfulRequestApi).Read().Response;  
        }

        /// <summary>
        /// This has a public anonymous access.
        /// Updates a Configuration entity.
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        /// 
        [NoCleanseDto]
        public static dynamic Update(JSONRPC_API requestApi)
        {
            return new Config_BSO_Update(requestApi).Create().Response;
        }

        /// <summary>
        /// Administrator or Power user only
        /// Creates a Configuration entity.
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        /// 
        [NoCleanseDto]
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new Config_BSO_Create(requestApi).Create().Response;
        }


        /// <summary>
        /// The Read method is used to return one or more Schemas.
        /// This has a public anonymous access.
        /// <param name="jsonrpcRequest">The request param</param>
        /// <returns></returns>
        public static dynamic ReadSchema(JSONRPC_API requestApi)
        {
            return new Config_BSO_ReadSchema(requestApi).Read().Response;
        }

        public static dynamic Ping(JSONRPC_API requestApi) 
        {
            return new Config_BSO_Ping(requestApi).Read().Response;
        }

        public static dynamic Ping( RESTful_API requestApi)
        {
            return new Config_BSO_Ping(requestApi).Read().Response;
        }

        public static dynamic ReadVersions(JSONRPC_API requestApi)
        {
            return new Config_BSO_ReadVersions(requestApi).Read().Response;
        }

    }
}