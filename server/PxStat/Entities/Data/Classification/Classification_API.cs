using API;


namespace PxStat.Data
{
    /// <summary>
    /// These APIs are used for reading and search for Classifications.
    /// </summary>
    [AllowAPICall]
    public class Classification_API
    {
        /// <summary>
        /// Read a Classification based on Classification ID
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Classification_BSO_Read(requestApi).Read().Response;
        }

        /// <summary>
        /// Search for one or more Classifications based on a search term. The search will return any value LIKE a value in the 
        /// Classification or related Variable name.
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Search(JSONRPC_API requestApi)
        {
            return new Classification_BSO_Search(requestApi).Read().Response;
        }
    }
}