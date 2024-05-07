using API;
using System;

namespace PxStat.Security
{    /// <summary>
     /// API for reading performance
     /// </summary>
    [AllowAPICall]
    public class Performance_API
    {

        public static dynamic ReadPrfMemoryAvailable(JSONRPC_API jsonrpcRequest)
        {
            return new Performance_BSO_ReadPrfMemoryAvailable(jsonrpcRequest).Read().Response;
        }

        public static dynamic ReadPrfProcessorPercentage(JSONRPC_API jsonrpcRequest)
        {
            return new Performance_BSO_ReadPrfProcessorPercentage(jsonrpcRequest).Read().Response;
        }

        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new Performance_BSO_Delete(requestApi).Delete().Response;
        }

    }
    // Data Object for holding results shuffled out of the original database response during reads
    internal class IndividualResults
    {
        internal DateTime Datetime { get; set; }
        internal int DataValue { get; set; }
        internal string ServerValue { get; set; }

    }
}
