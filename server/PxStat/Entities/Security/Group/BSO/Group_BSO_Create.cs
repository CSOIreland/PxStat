using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Creates a user group
    /// </summary>
    internal class Group_BSO_Create : BaseTemplate_Create<Group_DTO_Create, Group_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Group_BSO_Create(JSONRPC_API request) : base(request, new Group_VLD_Create())
        {
        }

        /// <summary>
        /// Test Privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoGroup = new Group_ADO();

            //First we must check if the Group exists already (we can't have duplicates)
            if (adoGroup.Exists(Ado, DTO.GrpCode))
            {
                //This Group exists already, we can't proceed
                Log.Instance.Debug("Group exists already - create request refused");
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //Create the Group - and retrieve the newly created Id
            int newId = adoGroup.Create(Ado, DTO, SamAccountName);
            if (newId == 0)
            {
                Log.Instance.Debug("Can't create Group");
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}

