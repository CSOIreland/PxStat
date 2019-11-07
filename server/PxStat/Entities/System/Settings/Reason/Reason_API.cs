using API;

namespace PxStat.System.Settings
{
    /// <summary>
    /// APIs to control creating, deleting, updating and reading Reasons.
    /// </summary>
    public class Reason_API
    {
        /// <summary>
        /// Entry point to the API method - Reads a Reason
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Reason_BSO_Read(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method -  Creates a Reason
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new Reason_BSO_Create(requestApi).Create().Response;
        }

        /// <summary>
        /// Updates a Reason
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Update(JSONRPC_API requestApi)
        {
            return new Reason_BSO_Update(requestApi).Update().Response;
        }

        /// <summary>
        /// Entry point to the API method - Deletes a Reason
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new Reason_BSO_Delete(requestApi).Delete().Response;
        }
    }
}