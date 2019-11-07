using API;
using PxStat.Data;
using PxStat.Template;
using PxStat.System.Notification;
using System.Collections.Generic;

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
            //We need to get the Request for notification purposes
            WorkflowRequest_ADO adoWrq = new WorkflowRequest_ADO();
            List<WorkflowRequest_DTO> dtoWrqList = adoWrq.Read(Ado, DTO.RlsCode, true);

            if (dtoWrqList.Count > 1)
            {
                //Multiple requests found for this release
                Log.Instance.Debug("More than one request found for this release ");
                Response.error = Label.Get("error.create");
                return false;
            }

            if (dtoWrqList.Count == 0)
            {
                //No request found for this release
                Log.Instance.Debug("No request found for this release ");
                Response.error = Label.Get("error.create");
                return false;
            }


            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoWorkflowRequest = new WorkflowRequest_ADO();

            if (!adoWorkflowRequest.IsCurrent(Ado, DTO.RlsCode))
            {
                //No workflow found
                Log.Instance.Debug("No Live workflow found for this Release Code");
                Response.error = Label.Get("error.create");
                return false;
            }

            var adoWorkflowResponse = new WorkflowResponse_ADO();

            //If this is a Moderator, we need to check if the user is in the same group as the release and has approve rights
            if (IsModerator())
            {
                var approveRlsList = adoWorkflowResponse.GetApproverAccess(Ado, SamAccountName, true, DTO.RlsCode);
                if (approveRlsList.data.Count == 0)
                {
                    Log.Instance.Debug("Insufficient access for a Moderator");
                    Response.error = Label.Get("error.authentication");
                    return false;
                }
            }

            //Check that there isn't already a WorkflowResponse for this Release

            if (adoWorkflowResponse.IsInUse(Ado, DTO))
            {
                //Duplicate Workflow exists
                Log.Instance.Debug("Workflow Response exists already, can't create");
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            var adoComment = new Comment_ADO();

            int commentCode = adoComment.Create(Ado, DTO, SamAccountName);

            if (commentCode == 0)
            {
                //Can't create a comment so we can't proceed
                Log.Instance.Debug("Can't create comment - create WorkflowResponse refused");
                Response.error = Label.Get("error.create");
                return false;
            }

            DTO.CmmCode = commentCode;

            int createResponse = adoWorkflowResponse.Create(Ado, DTO, SamAccountName);
            if (createResponse == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }

            //If this is a Reject then we must reset the workflow to stop it being current
            if (DTO.RspCode.Equals(Resources.Constants.C_WORKFLOW_STATUS_REJECT))
            {
                WorkflowRequest_DTO_Update dtoRequest = new WorkflowRequest_DTO_Update(Request.parameters);
                dtoRequest.WrqCurrentFlag = false;

                adoWorkflowRequest.Update(Ado, dtoRequest, SamAccountName);
            }

            Response.data = JSONRPC.success;

            //Get Release 
            Release_ADO releaseAdo = new Release_ADO(Ado);
            Release_DTO releaseDTO = Release_ADO.GetReleaseDTO(releaseAdo.Read(DTO.RlsCode, SamAccountName));

            Email_BSO_NotifyWorkflow notify = new Email_BSO_NotifyWorkflow();
            notify.EmailResponse(dtoWrqList[0], DTO, releaseDTO);
            return true;
        }
    }
}
