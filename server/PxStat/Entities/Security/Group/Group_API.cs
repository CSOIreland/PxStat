using API;

namespace PxStat.Security
{
    /// <summary>
    /// Group APIs enable control over application user groups.
    /// </summary>
    public class Group_API
    {
        /// <summary>
        /// Reads one or more Groups. 
        /// If no Group Code is passed in, it returns a list of all groups
        /// If a Group Code is passed in, it returns the requested Group
        /// The user must be authenticated in Active Directory and registered as a user of this application
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Group_BSO_Read(requestApi).Read().Response;
        }

        /// <summary>
        /// Reads the groups for which the user has access. A CcnUsername is taken as a paramters
        /// If no CcnUsername is supplied then the assumption is the request is for the current user, i.e. the user prinicipal
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic ReadAccess(JSONRPC_API requestApi)
        {
            return new Group_BSO_ReadAccess(requestApi).Read().Response;
        }

        /// <summary>
        /// Creates a new Group
        /// Requires a valid Group Code, Group Name and Group Contact
        /// User must be either an Admin or Power User
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Create(JSONRPC_API requestApi)
        {
            return new Group_BSO_Create(requestApi).Create().Response;
        }

        /// <summary>
        /// Updates an existing Group entry
        /// Requires a valid Group Code, Group Name and Group Contact
        /// Group code must correspond to an existing Group
        /// User must be either an Admin or Power User
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Update(JSONRPC_API requestApi)
        {
            return new Group_BSO_Update(requestApi).Update().Response;
        }

        /// <summary>
        /// /// Deletes an existing Group entry (Soft Delete)
        /// Requires a valid Group Code
        /// Group code must correspond to an existing Group
        /// User must be either an Admin or Power User
        /// If the Group Code is used by any user then the Delete will be refused
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Delete(JSONRPC_API requestApi)
        {
            return new Group_BSO_Delete(requestApi).Delete().Response;
        }

    }
}