using API;
using PxStat.Entities;
using PxStat.Data;
using System.Collections.Generic;
using System;
using PxStat.Resources;
using PxStat.System.Notification;

namespace PxStat.Workflow
{
    /// <summary>
    /// Rules for Workflow Signoff:
    /// 
    /// The actions will depend on (a) the Request type and (b) the current status of the Release
    /// 
    /// Request type PUBLISH
    /// - Release must be Work In Progress (WIP)
    /// - Set the DatetimeFrom of the release to the WorkflowRequest Datetime
    /// - Update the Emergency flag, Reservation flag, Archive flag, Alert flag values of the release to the corresponding values of the Workflow Request 
    /// - If there is a previous release for that Matrix Code, set the DatetimeTo to the WorkflowRequest Datetime
    /// 
    /// Request type FLAG
    /// - Release must either be LiveNow or LiveNext
    /// - Update the Emergency flag, Reservation flag, Archive flag, Alert flag values of the release to the corresponding values of the Workflow Request
    /// 
    /// Request type DELETE (not to be confused with Rollback - see below)
    /// - Release must either be LiveNow or LiveNext or WIP
    /// - If the requested release is LiveNext, set the corresponding LiveNow release DatetimeTo to NOW. Then soft delete the LiveNext release.
    /// - Alternatively, if the requested release is LiveNow, set the corresponding LiveNow release DatetimeTo to NOW.
    /// - Alternatively, if the requested release is WIP, soft delete the WIP release.
    /// - If the request release is any other status, output an error.
    /// 
    /// Request type ROLLBACK
    /// - Release must either be LiveNow or LiveNext
    /// - If the requested release is LiveNext, soft delete the LiveNext release. Then set the corresponding LiveNow release DatetimeTo to null
    /// - If the requested release is LiveNow, find the LivePrevious and soft delete the LiveNow release. Then set the DatetimeTo of LivePrevious to null. 
    /// - If no LivePrevious is found then return an error.
    /// - If the request release is any other status, output an error.
    /// </summary>
    class WorflowSignoff_BSO_Create : BaseTemplate_Create<WorkflowSignoff_DTO, WorkflowSignoff_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal WorflowSignoff_BSO_Create(JSONRPC_API request) : base(request, new WorkflowSignoff_VLD_Create())
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
            var adoWorkflowRequest = new WorkflowRequest_ADO();
            var adoWorkflowResponse = new WorkflowResponse_ADO();


            if (!adoWorkflowResponse.IsInUse(Ado, DTO)) // is current workflow -- this should be the response!!
            {
                //No Live workflow found so we can't proceed
                Log.Instance.Debug("No Current workflow response found for this Release Code");
                Response.error = Error.Create;
                return false;
            }

            //Is this awaiting signoff?
            var adoWorkflow = new Workflow_ADO();
            ADO_readerOutput resultStatus = adoWorkflow.ReadAwaitingSignoff(Ado, Username, DTO.RlsCode);

            if (!resultStatus.hasData)
            {
                //Release not awaiting signoff so we can't proceed
                Log.Instance.Debug("Release not in status Awaiting Signoff");
                Response.error = Error.Update;
                return false;
            }

            var adoSignoff = new WorkflowSignoff_ADO();

            //Create a comment
            var adoComment = new Comment_ADO();
            int commentCode = adoComment.Create(Ado, DTO, Request.userPrincipal);
            if (commentCode == 0)
            {
                // Can't create a comment so we can't proceed
                Log.Instance.Debug("Can't create a comment ");
                Response.error = Error.Create;
                return false;
            }

            DTO.CmmCode = commentCode;

            //We must read the Request and in order to see how we are going to proceed
            WorkflowRequest_ADO adoWrq = new WorkflowRequest_ADO();
            List<WorkflowRequest_DTO> dtoWrqList = adoWrq.Read(Ado, DTO.RlsCode, true);

            if (dtoWrqList.Count > 1)
            {
                //Multiple requests found for this release
                Log.Instance.Debug("More than one request found for this release ");
                Response.error = Error.Create;
                return false;
            }

            //there must be exactly one live Workflow request at this point
            WorkflowRequest_DTO dtoWrq = dtoWrqList.Find(x => x.RqsCode != null);

            //Get the current Release
            Release_ADO adoRelease = new Release_ADO(Ado);
            Release_DTO dtoRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(DTO.RlsCode, Username));

            if (dtoRelease == null)
            {
                Log.Instance.Debug("Release not found");
                Response.error = Error.Create;
                return false;
            }

            //Is this a Reject?
            if (DTO.SgnCode.Equals("REJECTED"))
            {
                int res = adoSignoff.Create(Ado, DTO, Request.userPrincipal);

                if (res == 0)
                {
                    //Can't create a Workflow Signoff so we can't proceed
                    Log.Instance.Debug("Can't create a Workflow Signoff ");
                    Response.error = Error.Create;
                    return false;
                }


                WorkflowRequest_DTO_Update dtoReq = new WorkflowRequest_DTO_Update(Request.parameters);
                dtoReq.WrqCurrentFlag = false;

                //update the request
                int reqUpdate = adoWorkflowRequest.Update(Ado, dtoReq, Request.userPrincipal);
                if (reqUpdate == 0)
                {
                    //Can't save the Request so we can't proceed
                    Log.Instance.Debug("Can't save the Workflow Request");
                    Response.error = Error.Update;
                    return false;
                }

                DTO.MtrCode = dtoRelease.MtrCode; // we need this to see which cache we must flush

                Response.data = JSONRPC.success;

                return true;
            }

            //Not a Reject so we proceed...
            switch (dtoWrq.RqsCode)
            {
                case Constants.C_WORKFLOW_REQUEST_PUBLISH:

                    if (dtoRelease.PrcCode == 0)
                    {
                        //There must be a valid product for this release
                        Log.Instance.Debug("No product found for the release ");
                        Response.error = Error.Publish;
                        return false;
                    }

                    //Update the current release LiveDatetimeTo to the request Datetime
                    dtoRelease.RlsLiveDatetimeFrom = dtoWrq.WrqDatetime;

                    //set the release live flag
                    //update the release version and set the current revision to 0
                    DateTime switchDate;
                    switchDate = DateTime.Now > dtoWrq.WrqDatetime ? DateTime.Now : dtoWrq.WrqDatetime;
                    dtoRelease.RlsVersion++;
                    dtoRelease.RlsRevision = 0;
                    dtoRelease.RlsLiveFlag = true;
                    dtoRelease.RlsEmergencyFlag = dtoWrq.WrqEmergencyFlag != null ? dtoWrq.WrqEmergencyFlag.Value : false;
                    dtoRelease.RlsReservationFlag = dtoWrq.WrqReservationFlag != null ? dtoWrq.WrqReservationFlag.Value : false;
                    dtoRelease.RlsArchiveFlag = dtoWrq.WrqArchiveFlag != null ? dtoWrq.WrqArchiveFlag.Value : false;
                    dtoRelease.RlsAlertFlag = dtoWrq.WrqArchiveFlag != null ? dtoWrq.WrqArchiveFlag.Value : false;
                    dtoRelease.RlsLiveDatetimeFrom = switchDate;

                    //get the current live release

                    Release_DTO releaseDTONow = Release_ADO.GetReleaseDTO(adoRelease.ReadLiveNow(DTO.RlsCode));

                    //Save the changes for the release we're changing
                    int update = adoRelease.Update(dtoRelease, Request.userPrincipal.Name.ToString());
                    if (update == 0)
                    {
                        Log.Instance.Debug("Can't update the Release, RlsCode:" + dtoRelease.RlsCode);
                        Response.error = Error.Update;
                        return false;
                    }

                    if (releaseDTONow != null)
                    {
                        //...if there is a previous release
                        if (releaseDTONow.RlsCode != 0)
                        {
                            //Update the  Live LiveDatetimeTo to the request Datetime
                            releaseDTONow.RlsLiveDatetimeTo = switchDate;
                            //Save the changes for the previous release
                            adoRelease.Update(releaseDTONow, Request.userPrincipal.Name.ToString());

                        }
                    }

                    break;

                case Constants.C_WORKFLOW_REQUEST_FLAG:
                    //update release to transfer all flag values from the request to the release                            
                    dtoRelease.RlsReservationFlag = dtoWrq.WrqReservationFlag != null ? dtoWrq.WrqReservationFlag.Value : false;
                    dtoRelease.RlsArchiveFlag = dtoWrq.WrqArchiveFlag != null ? dtoWrq.WrqArchiveFlag.Value : false;
                    dtoRelease.RlsAlertFlag = dtoWrq.WrqArchiveFlag != null ? dtoWrq.WrqArchiveFlag.Value : false;

                    //Save the release
                    int updateCount = adoRelease.Update(dtoRelease, Request.userPrincipal.Name.ToString());
                    if (updateCount == 0)
                    {
                        //Update of Release failed
                        Log.Instance.Debug("Can't update the Release, RlsCode:" + DTO.RlsCode);
                        Response.error = Error.Update;
                        return false;
                    }

                    break;

                case Constants.C_WORKFLOW_REQUEST_DELETE:
                    //We can't soft delete the release just yet. We need it to be live until the Request is updated.

                    break;

                case Constants.C_WORKFLOW_REQUEST_ROLLBACK:

                    //Delete the future release if it exists and set the current to_date to null
                    //Otherwise delete the current release and make the previous release current by setting its to_date to null

                    if (adoRelease.IsLiveNext(dtoRelease.RlsCode))//this is a future release
                    {
                        Release_DTO dtoNowRelease = Release_ADO.GetReleaseDTO(adoRelease.ReadLiveNow(DTO.RlsCode));

                        dtoNowRelease.RlsLiveDatetimeTo = default(DateTime);
                        int rows = adoRelease.Update(dtoNowRelease, Request.userPrincipal.Name.ToString());
                        if (rows == 0)
                        {
                            Log.Instance.Debug("Can't update the Release, RlsCode:" + dtoNowRelease.RlsCode);
                            Response.error = Error.Update;
                            return false;
                        }

                        //As things stand, dtoRelease is the requested Release (which is a Live Next). This will be deleted in the Delete section below

                    }
                    else
                    {
                        //This isn't a future release - it had better be a Live Now (with a previous)
                        if (!adoRelease.IsLiveNow(dtoRelease.RlsCode))
                        {
                            //If the request is neither a Live Now nor a Live Next release then there's a problem
                            Log.Instance.Debug("Can't delete the Release, RlsCode:" + dtoRelease.RlsCode + ". Release is not current live");
                            Response.error = Error.Delete;
                            return false;
                        }

                        //Find the release that we're trying to rollback to
                        Release_DTO dtoPrevious = Release_ADO.GetReleaseDTO(adoRelease.ReadLivePrevious(dtoRelease.RlsCode));
                        if (dtoPrevious.RlsCode == 0)
                        {
                            //Previous release not found
                            //You can't roll back unless there's something to roll back to, so...
                            Log.Instance.Debug("Can't delete the Release, RlsCode:" + dtoRelease.RlsCode + ". Release is not current live");
                            Response.error = Error.Delete;
                            return false;
                        }


                        //We set the DatetimeTo to null in the previous release
                        dtoPrevious.RlsLiveDatetimeTo = default(DateTime);
                        int rows = adoRelease.Update(dtoPrevious, Request.userPrincipal.Name.ToString());
                        if (rows == 0)
                        {
                            Log.Instance.Debug("Can't update the Release, RlsCode:" + dtoPrevious.RlsCode);
                            Response.error = Error.Update;
                            return false;
                        }

                    }
                    break;

                default:
                    return false;

            }

            int signoffID = adoSignoff.Create(Ado, DTO, Request.userPrincipal);

            if (signoffID == 0)
            {
                //Can't create a Workflow Signoff so we can't proceed
                Log.Instance.Debug("Can't create a Workflow Signoff ");
                Response.error = Error.Create;
                return false;
            }

            //In all cases, if we have reached this stage, we must update the request to make it non-current
            WorkflowRequest_DTO_Update dtoRequest = new WorkflowRequest_DTO_Update(Request.parameters);
            dtoRequest.WrqCurrentFlag = false;

            //save the request
            int updated = adoWorkflowRequest.Update(Ado, dtoRequest, Request.userPrincipal);
            if (updated == 0)
            {
                //Can't save the Request so we can't proceed
                Log.Instance.Debug("Can't save the Workflow Signoff");
                Response.error = Error.Update;
                return false;
            }

            // We may now proceed with the soft delete
            Release_BSO_Delete bsoDelete = new Release_BSO_Delete();
            System.Navigation.Keyword_Release_ADO krbAdo = new System.Navigation.Keyword_Release_ADO();

            switch (dtoWrq.RqsCode)
            {
                case Constants.C_WORKFLOW_REQUEST_DELETE:
                    //Soft delete the Release. We had to hold this over to last because the Request updates wouldn't work without a live Release           
                    dtoRelease.RlsCode = DTO.RlsCode;
                    Request_ADO adoRequest = new Request_ADO();

                    if (adoRelease.IsLiveNext(dtoRelease.RlsCode)) //this is a future release
                    {
                        //Is this a future release?
                        //If so we must set the Date To in the release to now. This effectively closes the current release

                        //We must get the Live Now release for the Live next
                        Release_DTO dtoNowRelease = Release_ADO.GetReleaseDTO(adoRelease.ReadLiveNow(dtoRequest.RlsCode));

                        //There might not be a Live Now release if the LiveNext release is the first ever release
                        if (dtoNowRelease != null)
                        {

                            dtoNowRelease.RlsLiveDatetimeTo = DateTime.Now;
                            int updateCount = adoRelease.Update(dtoNowRelease, Request.userPrincipal.Name.ToString());
                            if (updateCount == 0)
                            {
                                Log.Instance.Debug("Can't update the Release, RlsCode:" + dtoNowRelease.RlsCode);
                                Response.error = Error.Update;
                                return false;
                            }

                        }

                        //dtoRelease is the currently requested release. As things stand, this will be deleted in the delete section below
                    }
                    else if (adoRelease.IsLiveNow(dtoRelease.RlsCode))
                    {



                        dtoRelease = Release_ADO.GetReleaseDTO(adoRelease.ReadLiveNow(dtoRequest.RlsCode));

                        //dtoRelease will be deleted in the delete section below. This deletes the Live Now version

                    }
                    else if (!adoRelease.IsWip(dtoRelease.RlsCode))
                    {
                        //Only LiveNow, LiveNext and WIP releases can be deleted. Anything else means there's a problem.
                        Log.Instance.Debug("Can't delete the Release - invalid release status, RlsCode:" + DTO.RlsCode);
                        Response.error = Error.Delete;
                        return false;

                    }

                    // We may now proceed with the soft delete
                    if (bsoDelete.Delete(Ado, DTO.RlsCode, Request.userPrincipal.Name.ToString(), true) == 0)
                    {
                        Log.Instance.Debug("Can't delete the Release, RlsCode:" + DTO.RlsCode);
                        Response.error = Error.Delete;
                        return false;
                    }

                    //Delete the search keywords for this release
                    krbAdo.Delete(Ado, DTO.RlsCode, null, true);
                    break;

                case Constants.C_WORKFLOW_REQUEST_ROLLBACK:

                    if (bsoDelete.Delete(Ado, DTO.RlsCode, Request.userPrincipal.Name.ToString(), true) == 0)
                    {
                        Log.Instance.Debug("Can't delete the Release, RlsCode:" + DTO.RlsCode);
                        Response.error = Error.Delete;
                        return false;
                    }

                    //Delete the search keywords for this release
                    krbAdo.Delete(Ado, DTO.RlsCode, null, true);
                    break;

            }


            DTO.MtrCode = dtoRelease.MtrCode; // we need this to see which cache we must flush

            Response.data = JSONRPC.success;
            Email_BSO_Notify notify = new Email_BSO_Notify();
            notify.EmailSignoff(dtoWrq, DTO, dtoRelease);
            return true;
        }
    }
}