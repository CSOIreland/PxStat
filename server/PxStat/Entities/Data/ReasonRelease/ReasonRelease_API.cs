using API;

namespace PxStat.Data
{
    /// <summary>
    /// These APIs are used to Create, Read, Update and Delete the Reason data for a Release.
    /// </summary>
    [AllowAPICall]
    public class ReasonRelease_API
    {
        /// <summary>
        /// Reads a ReasonRelease entity
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new ReasonRelease_BSO_Read(requestApi).Read().Response;
        }

        /// <summary>
        /// Creates a ReasonRelease entity
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new ReasonRelease_BSO_Create(requestApi).Create().Response;
        }

        /// <summary>
        /// Updates a ReasonRelease entity
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Update(JSONRPC_API requestApi)
        {
            return new ReasonRelease_BSO_Update(requestApi).Update().Response;
        }
        /// <summary>
        /// Deletes a ReasonRelease entity
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new ReasonRelease_BSO_Delete(requestApi).Delete().Response;
        }
    }
}
