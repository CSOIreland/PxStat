using API;
using PxStat.DBuild;

namespace PxStat.Build
{
    /// <summary>
    /// PxFileBuild is a set of APIs for creating new data.
    /// </summary>
    /// 
    [AllowAPICall]
    public class Build_API
    {
        /// <summary>
        /// Creates a new px file or json-stat file based on supplied metadata
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        /// 

        [NoCleanseDto]
        public static dynamic Create(JSONRPC_API jsonrpcRequest)
        {
            //return new Build_BSO_Create(jsonrpcRequest).Read().Response;
            return new DBuild_BSO_Create(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        /// 
        [NoCleanseDto]
        public static dynamic Update(JSONRPC_API jsonrpcRequest)
        {
            //return new Build_BSO_Update(jsonrpcRequest).Read().Response;
            return new DBuild_BSO_Update(jsonrpcRequest).Read().Response;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadTemplate(JSONRPC_API jsonrpcRequest)
        {
            //return new Build_BSO_ReadTemplate(jsonrpcRequest).Read().Response;
            return new DBuild_BSO_ReadTemplate(jsonrpcRequest).Read().Response;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            //return new Build_BSO_Read(jsonrpcRequest).Read().Response;
            return new DBuild_BSO_Read(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Validate the px and other data
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic Validate(JSONRPC_API jsonrpcRequest)
        {
            //return new Build_BSO_Validate(jsonrpcRequest).Read().Response;
            return new DBuild_BSO_Validate(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Get a csv template including only current periods of the px file
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        public static dynamic ReadDataset(JSONRPC_API jsonrpcRequest)
        {
            //return new Build_BSO_ReadDataset(jsonrpcRequest).Read().Response;
            return new DBuild_BSO_ReadDataset(jsonrpcRequest).Read().Response;
        }

        /// <summary>
        /// Get a csv template including only current periods of the px file
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        //public static dynamic ReadDatasetByExistingPeriods(JSONRPC_API jsonrpcRequest)
        //{
        //    return new Build_BSO_ReadDatasetByExistingPeriods(jsonrpcRequest).Read().Response;
        //}

        /// <summary>
        /// Get a csv template with the current periods (showing data) and the new periods with blank data
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        //public static dynamic ReadDatasetByAllPeriods(JSONRPC_API jsonrpcRequest)
        //{
        //    return new Build_BSO_ReadDatasetByAllPeriods(jsonrpcRequest).Read().Response;
        //}

        /// <summary>
        /// Get a csv template with only the new periods, showing blank data
        /// </summary>
        /// <param name="jsonrpcRequest"></param>
        /// <returns></returns>
        //public static dynamic ReadDatasetByNewPeriods(JSONRPC_API jsonrpcRequest)
        //{
        //    return new Build_BSO_ReadDatasetByNewPeriods(jsonrpcRequest).Read().Response;
        //}
    }
}
