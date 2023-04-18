using API;

namespace PxStat.Security
{
    /// <summary>
    /// Group Account APIs control user membership of Groups.
    /// </summary>
    [AllowAPICall]
    public class GroupAccount_API
    {
        /// <summary>
        /// Entry point to the API method - Reads Group Membership
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new GroupAccount_BSO_Read(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Creates a Group Membership
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new GroupAccount_BSO_Create(requestApi).Create().Response;
        }

        /// <summary>
        /// Entry point to the API method - Updates a Group Membership
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Update(JSONRPC_API requestApi)
        {
            return new GroupAccount_BSO_Update(requestApi).Create().Response;
        }

        /// <summary>
        /// Entry point to the API method - Deletes a Group Membership
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new GroupAccount_BSO_Delete(requestApi).Delete().Response;
        }
    }
}