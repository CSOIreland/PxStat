using API;

namespace PxStat.Data
{
    /// <summary>
    /// Release Product APIs for associated tables.
    /// </summary>
    [AllowAPICall]
    public class ReleaseProductAssociation_API
    {
        /// <summary>
        /// Entry point to the API method - Reads Release Product mapping
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new ReleaseProductAssociation_BSO_Read(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Creates a Release Product mapping
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new ReleaseProductAssociation_BSO_Create(requestApi).Create().Response;
        }

        /// <summary>
        /// Entry point to the API method - Deletes a Release Product mapping
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new ReleaseProductAssociation_BSO_Delete(requestApi).Delete().Response;
        }
    }
}