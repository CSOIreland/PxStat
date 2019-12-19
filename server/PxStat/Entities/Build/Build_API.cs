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


        public static dynamic BuildRead(JSONRPC_API jsonrpcRequest)
        {
            return new Build_BSO_BuildRead(jsonrpcRequest).Read().Response;
        }

        public static dynamic ReadDatasetByExistingPeriods(JSONRPC_API jsonrpcRequest)
        {
            return new Build_BSO_ReadDatasetByExistingPeriods(jsonrpcRequest).Read().Response;
        }

        public static dynamic ReadDatasetByAllPeriods(JSONRPC_API jsonrpcRequest)
        {
            return new Build_BSO_ReadDatasetByAllPeriods(jsonrpcRequest).Read().Response;
        }


        public static dynamic ReadDatasetByNewPeriods(JSONRPC_API jsonrpcRequest)
        {
            return new Build_BSO_ReadDatasetByNewPeriods(jsonrpcRequest).Read().Response;
        }
    }
}
