using API;
using PxStat.Data;
using PxStat.Resources;
using PxStat.System.Notification;
using PxStat.Template;
using System.Collections.Generic;

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
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoWorkflowRequest = new WorkflowRequest_ADO();

            //check if this release already has a Current WorkflowRequest
            if (adoWorkflowRequest.IsCurrent(Ado, DTO.RlsCode))
            {
                //Can't create a comment so we can't proceed
                Log.Instance.Debug("Release already has a live Workflow, can't create");
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //Get Release 
            Release_ADO releaseAdo = new Release_ADO(Ado);
            Release_DTO releaseDTO = Release_ADO.GetReleaseDTO(releaseAdo.Read(DTO.RlsCode, SamAccountName));

            if (releaseDTO == null)
            {
                Log.Instance.Debug("Release Code not found");
                Response.error = Label.Get("error.create");
                return false;
            }

            Request_ADO adoRequest = new Request_ADO();
            Request_DTO dtoRequest = new Request_DTO();
            dtoRequest.RlsCode = DTO.RlsCode;

            Release_ADO adoRelease = new Release_ADO(Ado);

            //We must validate the request, depending on the RequestCode and the current state of the Release
            switch (DTO.RqsCode)
            {
                case Constants.C_WORKFLOW_REQUEST_PUBLISH:

                    if (!adoRelease.IsWip(dtoRequest.RlsCode))
                    {
                        Log.Instance.Debug("The requested Release Code is not the WIP Release");
                        Response.error = Label.Get("error.create");
                        return false;

                    }

                    break;

                case Constants.C_WORKFLOW_REQUEST_PROPERTY:
                    //Release must be CURRENT LIVE - RlsLiveFlag is live only between from and to dates 
                    if (!adoRelease.IsLiveNow(dtoRequest.RlsCode) && !adoRelease.IsLiveNext(dtoRequest.RlsCode))
                    {
                        Log.Instance.Debug("Cannot create a Flag Request. The Release must be either current Live or Next Live");
                        Response.error = Label.Get("error.create");
                        return false;
                    }
                    break;

                case Constants.C_WORKFLOW_REQUEST_DELETE:
                    //What about WIP? - not a workflow request but it is part of Release. It will have its own API.
                    //Release must be LIVE NEXT, CURRENT LIVE or WIP
                    if (!adoRelease.IsLiveNow(dtoRequest.RlsCode) && !adoRelease.IsLiveNext(dtoRequest.RlsCode) && !adoRelease.IsWip(dtoRequest.RlsCode))
                    {
                        Log.Instance.Debug("Can't create a DELETE Request on a historical Release");
                        Response.error = Label.Get("error.create");
                        return false;
                    }

                    break;

                case Constants.C_WORKFLOW_REQUEST_ROLLBACK:

                    //Release must be CURRENT LIVE OR NEXT LIVE and PREVIOUS must exist
                    if ((!adoRelease.IsLiveNow(dtoRequest.RlsCode) && !adoRelease.IsLiveNext(dtoRequest.RlsCode)) || !adoRelease.HasPrevious(dtoRequest.RlsCode))
                    {
                        Log.Instance.Debug("Can't create a ROLLBACK Request because (a) the Request is neither live nor pending-live or (b) there is no valid Release to roll back to.");
                        Response.error = Label.Get("error.create");
                        return false;
                    }



                    break;

                default:
                    Log.Instance.Debug("Invalid Request Code");
                    Response.error = Label.Get("error.validation");
                    return false;

            }

            var adoComment = new Comment_ADO();

            int commentCode = adoComment.Create(Ado, DTO, SamAccountName);

            if (commentCode == 0)
            {
                //Can't create a comment so we can't proceed
                Log.Instance.Debug("Can't create a comment - WorkflowRequest create request refused");
                Response.error = Label.Get("error.create");
                return false;
            }

            DTO.CmmCode = commentCode;

            //Create the WorkflowRequest - and retrieve the newly created Id
            int newId = adoWorkflowRequest.Create(Ado, DTO, SamAccountName);
            if (newId == 0)
            {

                Response.error = Label.Get("error.create");
                return false;
            }



            Response.data = JSONRPC.success;

            List<WorkflowRequest_DTO> dtoWrqList = adoWorkflowRequest.Read(Ado, DTO.RlsCode, true);
            if (dtoWrqList.Count == 1)
            {
                Email_BSO_NotifyWorkflow notify = new Email_BSO_NotifyWorkflow();
                try
                {
                    notify.EmailRequest(dtoWrqList[0], releaseDTO);
                }
                catch { }
            }
            else
                Log.Instance.Error("Email failed");

            return true;
        }
    }
}
