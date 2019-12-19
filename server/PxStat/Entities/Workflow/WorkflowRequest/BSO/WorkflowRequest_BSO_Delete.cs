using API;
using PxStat.Template;

namespace PxStat.Workflow
{
    /// <summary>
    /// Delete a workflow request
    /// </summary>
    internal class WorkflowRequest_BSO_Delete : BaseTemplate_Delete<WorkflowRequest_DTO, WorkflowRequest_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal WorkflowRequest_BSO_Delete(JSONRPC_API request) : base(request, new WorkflowRequest_VLD_Delete())
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
            //only if the WorkflowRequest is AwaitingResponse

            Workflow_ADO wfAdo = new Workflow_ADO();

            ADO_readerOutput output = wfAdo.ReadAwaitingResponse(Ado, SamAccountName, DTO.RlsCode, Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE"));
            if (!output.hasData)
            {
                Log.Instance.Debug("No valid AwaitingResponse workflow found for RlsCode " + DTO.RlsCode);
                return false;
            }

            WorkflowRequest_ADO wfReqAdo = new WorkflowRequest_ADO();

            int deleted = wfReqAdo.Delete(Ado, DTO.RlsCode, SamAccountName);

            if (deleted == 0)
            {
                Log.Instance.Debug("Can't delete the workflow for RlsCode " + DTO.RlsCode);
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}
