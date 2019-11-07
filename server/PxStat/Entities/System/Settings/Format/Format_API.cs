using API;

namespace PxStat.System.Settings
{
    /// <summary>
    /// APIs to control  reading Formats.
    /// </summary>
    public class Format_API
    {
        /// <summary>
        /// Entry point to the API method - Reads a Format entry
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Format_BSO_Read(requestApi).Read().Response;
        }
    }
}