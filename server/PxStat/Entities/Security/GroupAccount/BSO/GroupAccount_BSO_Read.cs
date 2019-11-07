using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Reads a Group Account
    /// </summary>
    internal class GroupAccount_BSO_Read : BaseTemplate_Read<GroupAccount_DTO_Read, GroupAccount_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal GroupAccount_BSO_Read(JSONRPC_API request) : base(request, new GroupAccount_VLD_Read())
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
            var adoGroupAccount = new GroupAccount_ADO();

            //GroupAccounts are returned as an ADO result
            ADO_readerOutput result = adoGroupAccount.Read(Ado, DTO);

            if (!result.hasData)
            {

                return false;
            }
            else
            {
                ActiveDirectory_ADO adAdo = new Security.ActiveDirectory_ADO();
                adAdo.MergeAdToUsers(ref result);
            }

            Response.data = result.data;
            return true;
        }
    }
}

