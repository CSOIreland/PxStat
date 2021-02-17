using API;
using PxStat.Resources;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.Workflow
{
    /// <summary>
    /// Creates a workflow Request
    /// </summary>
    internal class WorkflowRequest_BSO_Create : BaseTemplate_Create<WorkflowRequest_DTO, WorkflowRequest_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal WorkflowRequest_BSO_Create(JSONRPC_API request) : base(request, new WorkflowRequest_VLD_Create())
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

            Workflow_BSO bso = new Workflow_BSO();
            Account_BSO aBso = new Account_BSO(Ado);
            bool responseComplete = false;

            //Create the workflow request
            Response = bso.WorkflowRequestCreate(DTO, Ado, SamAccountName);
            if (Response.error != null)
            {
                return false;
            }

            //If this is a user with an automatic flow to the next stage, then do the response immediately
            if (bso.HasFastrackPermission(Ado, SamAccountName, DTO.RlsCode, "workflow.fastrack.response"))
            {
                Log.Instance.Debug("Fastrack from Request to Response");
                WorkflowResponse_DTO rspDto = new WorkflowResponse_DTO() { RlsCode = DTO.RlsCode, RspCode = Constants.C_WORKFLOW_STATUS_APPROVE, CmmValue = Label.Get("auto-approve-comment") };
                Response = bso.WorkflowResponseCreate(rspDto, Ado, SamAccountName);
                if (Response.error != null)
                {
                    return false;
                }
                responseComplete = true;

            }


            //If this is a user with an automatic flow to the next stage (and the response has been completed), then do the signoff immediately
            if (bso.HasFastrackPermission(Ado, SamAccountName, DTO.RlsCode, "workflow.fastrack.signoff") && responseComplete)
            {

                if (!(IsPowerUser() || IsAdministrator()))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
                Log.Instance.Debug("Fastrack from Request to Signoff via Response");
                WorkflowSignoff_DTO sgnDTO = new WorkflowSignoff_DTO() { RlsCode = DTO.RlsCode, SgnCode = Constants.C_WORKFLOW_STATUS_APPROVE, CmmValue = Label.Get("auto-approve-signoff") };
                Response = bso.WorkflowSignoffCreate(Ado, sgnDTO, SamAccountName);
                if (Response.error != null)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsSelfApprover()
        {
            var adoWorkflowResponse = new WorkflowResponse_ADO();
            var approveRlsList = adoWorkflowResponse.GetApproverAccess(Ado, SamAccountName, true, DTO.RlsCode);
            return (approveRlsList.data.Count > 0);

        }
    }
}
