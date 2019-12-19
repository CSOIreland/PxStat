using API;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.Workflow
{
    /// <summary>
    /// Read the approval/rejection history for a specific workflow
    /// </summary>
    internal class Workflow_BSO_ReadWorkInProgress : BaseTemplate_Read<Workflow_DTO, Workflow_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Workflow_BSO_ReadWorkInProgress(JSONRPC_API request) : base(request, new Workflow_VLD())
        { }

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
            var adoWorkflow = new Workflow_ADO();

            var result = adoWorkflow.ReadWorkInProgress(Ado, SamAccountName, DTO.LngIsoCode);

            var adoAd = new ActiveDirectory_ADO();

            adoAd.MergeAdToUsers(ref result);

            Response.data = result.data;

            return true;
        }


    }

}
