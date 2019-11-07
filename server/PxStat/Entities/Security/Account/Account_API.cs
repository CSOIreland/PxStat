
using API;


namespace PxStat.Security
{
    /// <summary>
    /// The Account APIs control the access to the application.
    /// </summary>
    public static class Account_API
    {
        /// <summary>
        /// Entry point to the API method - Reads Account
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Account_BSO_Read(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Reads the currently logged in user
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadCurrent(JSONRPC_API requestApi)
        {
            return new Acccount_BSO_ReadCurrent(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Reads the currently logged in user similar to ReadCurrent but
        /// in this case it doesn't return group membership information
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadCurrentAccess(JSONRPC_API requestApi)
        {
            return new Acccount_BSO_ReadCurrentAccess(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Creates an Account
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new Account_BSO_Create(requestApi).Create().Response;
        }
        /// <summary>
        /// Entry point to the API method - Deletes an Account
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new Account_BSO_Delete(requestApi).Delete().Response;
        }

        /// <summary>
        /// Entry point to the API method - Updates an Account
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Update(JSONRPC_API requestApi)
        {
            return new Account_BSO_Update(requestApi).Update().Response;
        }

        //
        public static dynamic UpdateCurrent(JSONRPC_API requestApi)
        {
            return new Account_BSO_UpdateCurrent(requestApi).Update().Response;
        }

        /// <summary>
        /// Entry point to the API method - Tests if a user is an administrator
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadIsAdministrator(JSONRPC_API requestApi)
        {
            return new Account_BSO_ReadIsAdministrator(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Tests if a user is a Moderator
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadIsModerator(JSONRPC_API requestApi)
        {
            return new Account_BSO_ReadIsModerator(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Tests if a user is a power user
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadIsPowerUser(JSONRPC_API requestApi)
        {
            return new Account_BSO_ReadIsPowerUser(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Tests if a user is registered on the system
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadIsRegistered(JSONRPC_API requestApi)
        {
            return new Account_BSO_ReadIsRegistered(requestApi).Read().Response;
        }

        /// <summary>
        /// Entry point to the API method - Tests if a user is in Active Directory
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadIsInAD(JSONRPC_API requestApi)
        {
            return new Account_BSO_ReadIsInAD(requestApi).Read().Response;
        }

        /// <summary>
        /// Tests if a user is an approver for a given Release Code
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadIsApprover(JSONRPC_API requestApi)
        {
            return new Account_BSO_ReadIsApprover(requestApi).Read().Response;
        }
    }
}