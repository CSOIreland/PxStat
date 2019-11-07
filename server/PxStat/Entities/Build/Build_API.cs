using API;

namespace PxStat.Build
{
    /// <summary>
    /// PxFileBuild is a set of APIs for creating new data.
    /// </summary>
    public class Build_API
    {
        /// <summary>
        /// Creates a new px file or json-stat file based on supplied metadata
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {
            return new Build_BSO_Create(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Update(JSONRPC_API jsonrpcRequest)
        {
            return new Build_BSO_Update(jsonrpcRequest).Read().Response;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadTemplate(JSONRPC_API jsonrpcRequest)
        {
            return new Build_BSO_ReadTemplate(jsonrpcRequest).Read().Response;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new Build_BSO_Read(jsonrpcRequest).Read().Response;
        }

        public static dynamic Validate(JSONRPC_API jsonrpcRequest)
        {
            return new Build_BSO_Validate(jsonrpcRequest).Read().Response;
        }
    }
}
