using API;

namespace PxStat.System.Settings
{
    /// <summary>
    /// APIs to control creating, deleting, updating and reading Copyrights.
    /// </summary>
    public class Copyright_API
    {
        /// <summary>
        /// Entry point to the API method - Reads Copyright
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Copyright_BSO_Read(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Creates a new Copyright
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new Copyright_BSO_Create(requestApi).Create().Response;
        }
        /// <summary>
        /// Entry point to the API method - Updates an existing copyright
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Update(JSONRPC_API requestApi)
        {
            return new Copyright_BSO_Update(requestApi).Update().Response;
        }

        /// <summary>
        /// Entry point to the API method - Deletes a Copyright
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new Copyright_BSO_Delete(requestApi).Delete().Response;
        }
    }
}