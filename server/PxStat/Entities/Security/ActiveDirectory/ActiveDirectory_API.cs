using API;

namespace PxStat.Security
{
    /// <summary>
    /// Active Directory APIs enable an interface to an Active Directory.
    /// </summary>
    [AllowAPICall]
    public static class ActiveDirectory_API
    {
        /// <summary>
        /// Entry point to the API method - reads Active Directory 
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new ActiveDirectory_BSO_Read(requestApi).Read().Response;
        }
    }
}