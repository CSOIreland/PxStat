using Newtonsoft.Json;
using API;
using PxStat.Template;

namespace PxStat.Security
{
    /// <summary>
    /// Updates a Group
    /// </summary>
    internal class Group_BSO_Update : BaseTemplate_Update<Group_DTO_Update, Group_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Group_BSO_Update(JSONRPC_API request) : base(request, new Group_VLD_Update())
        {
        }

        /// <summary>
        /// Test privilege
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
            var adoGroup = new Group_ADO();

            if (!DTO.GrpCodeNew.Equals(DTO.GrpCodeOld))
            {
                //We are changing the Group Code but first we must check if the new Group Code exists already (we can't have duplicates)
                if (adoGroup.Exists(Ado, DTO.GrpCodeNew))
                {
                    //This Group exists already, we can't proceed
                    Log.Instance.Debug("Group exists already - create request refused");
                    Response.error = Label.Get("error.duplicate");
                    return false;
                }
            }

            //Update the Group - and retrieve the number of updated rows
            int nUpdated = adoGroup.Update(Ado, DTO, SamAccountName);
            if (nUpdated == 0)
            {
                Log.Instance.Debug("Can't update the Group");
                Response.error = Label.Get("error.update");
                return false;
            }

            Log.Instance.Debug("Group updated: " + JsonConvert.SerializeObject(DTO));

            Response.data = JSONRPC.success;

            return true;
        }
    }
}

