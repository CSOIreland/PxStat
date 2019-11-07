using API;

namespace PxStat.Security
{
    /// <summary>
    /// The trace APIs are used to read user interactions with APIs.
    /// </summary>
    public class Trace_API
    {
        /// <summary>
        /// Entry point to the API method - Reads trace entries
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        [NoTrace]
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Trace_BSO_Read(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Gets a list of the different Trace criteria
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        [NoTrace]
        public static dynamic ReadType(JSONRPC_API requestApi)
        {
            return new Trace_BSO_ReadType(requestApi).Read().Response;
        }
    }
}