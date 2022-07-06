
using API;

namespace PxStat.Security
{
    /// <summary>
    /// The Privilege APIs control user rights.
    /// </summary>
    [AllowAPICall]
    public class Privilege_API
    {
        /// <summary>
        /// Entry point to the API method - Reads a Privilege
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Privilege_BSO_Read(requestApi).Read().Response;
        }
    }
}