using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Reads group data
    /// </summary>
    internal class Group_BSO_Read : BaseTemplate_Read<Group_DTO_Read, Group_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Group_BSO_Read(JSONRPC_API request) : base(request, new Group_VLD_Read())
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

            //Groups are returned as an ADO result
            ADO_readerOutput result = adoGroup.Read(Ado, DTO);

            if (!result.hasData)
            {

                return false;
            }

            Response.data = result.data;

            return true;
        }
    }
}

