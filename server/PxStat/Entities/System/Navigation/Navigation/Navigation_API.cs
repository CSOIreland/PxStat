using API;
namespace PxStat.System.Navigation
{
    /// <summary>
    /// Navigation is a set of links used to enhance user experience. These APIs control the reading and creation of Navigation items.
    /// </summary>
    public class Navigation_API
    {

        /// <summary>
        /// Reads a set of data for front end navigation
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Navigation_BSO_Read(requestApi).Read().Response;
        }
        /// <summary>
        /// Search for one or more Releases using search terms and other parameters
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        /// 

        public static dynamic Search(JSONRPC_API requestApi)
        {
            return new Navigation_BSO_Search(requestApi).Read().Response;
        }
    }
}
