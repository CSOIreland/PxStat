using API;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.Workflow
{
    /// <summary>
    /// Read Workflows for which there are no approvals yet
    /// </summary>
    internal class Workflow_BSO_ReadLive : BaseTemplate_Read<Workflow_DTO, Workflow_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Workflow_BSO_ReadLive(JSONRPC_API request) : base(request, new Workflow_VLD())
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
            var adoWorkflow = new Workflow_ADO();



            ADO_readerOutput result = adoWorkflow.ReadLive(Ado, SamAccountName, DTO);



            if (!result.hasData)
            {
                return false;
            }

            var adoAd = new ActiveDirectory_ADO();

            adoAd.MergeAdToUsers(ref result);

            Response.data = result.data;



            return true;
        }
    }
}
