using API;
using PxStat.Resources;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.Workflow
{
    /// <summary>
    /// Creates a Workflow Response
    /// </summary>
    internal class WorkflowResponse_BSO_Create : BaseTemplate_Create<WorkflowResponse_DTO, WorkflowResponse_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal WorkflowResponse_BSO_Create(JSONRPC_API request) : base(request, new WorkflowResponse_VLD_Create())
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

            Response = bso.WorkflowResponseCreate(DTO, Ado, SamAccountName);
            if (Response.error != null)
            {
                return false;
            }

            //If this is a Reject, we must not go any further, no matter what kind of user is involved.
            if (DTO.RspCode == Constants.C_WORKFLOW_STATUS_REJECT) return true;

            //If this is a user with an automatic flow to the next stage, then do the signoff immediately
            if (bso.HasFastrackPermission(Ado, SamAccountName, DTO.RlsCode, "workflow.fastrack.signoff"))
            {
                if (!(IsPowerUser() || IsAdministrator()))
                {
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
                Log.Instance.Debug("Fastrack from Response to Signoff");

                WorkflowSignoff_DTO sgnDTO = new WorkflowSignoff_DTO() { RlsCode = DTO.RlsCode, SgnCode = Constants.C_WORKFLOW_STATUS_APPROVE, CmmValue = Label.Get("auto-approve-signoff") };
                Response = bso.WorkflowSignoffCreate(Ado, sgnDTO, SamAccountName);
                if (Response.error != null)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
