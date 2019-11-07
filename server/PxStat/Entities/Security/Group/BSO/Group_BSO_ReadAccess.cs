using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Returns the Groups to which the user has access
    /// </summary>
    internal class Group_BSO_ReadAccess : BaseTemplate_Read<Group_DTO_Read, Group_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Group_BSO_ReadAccess(JSONRPC_API request) : base(request, new Group_VLD_Read())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoGroup = new Group_ADO();

            ADO_readerOutput result;

            //If we're not reading a specific user, then we assume the read is for the userPrincipal
            if (string.IsNullOrEmpty(DTO.CcnUsername))
                DTO.CcnUsername = SamAccountName;

            //If this is a Power user or admin, we just return everything
            if (Account_BSO_Read.IsPowerUser(Ado, DTO.CcnUsername) || Account_BSO_Read.IsAdministrator(Ado, DTO.CcnUsername))
            {
                DTO.GrpCode = null;
                result = adoGroup.Read(Ado, DTO);
            }
            //otherwise we get only the groups for the specified user
            else
                result = adoGroup.ReadAccess(Ado, DTO.CcnUsername);

            if (!result.hasData)
            {

                return false;
            }

            Response.data = result.data;

            return true;
        }
    }
}