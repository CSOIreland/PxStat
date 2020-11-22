using API;
using PxStat.Data;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Notification;
using System;
using System.Collections.Generic;

namespace PxStat.Workflow
{
    internal class Workflow_BSO
    {
        internal Workflow_DTO Populate(ADO ado, Workflow_DTO dto, string ccnUsername)
        {
            Workflow_ADO wAdo = new Workflow_ADO();
            ADO_readerOutput reader = wAdo.Read(ado, dto, ccnUsername, "PUBLISH");
            if (!reader.hasData) return dto;

            return new Workflow_DTO()
            {
                WrqDatetime = DataAdaptor.ReadDateTime(reader.data[0].WrqDatetime),
                WrqExceptionalFlag = DataAdaptor.ReadBool(reader.data[0].WrqExceptionalFlag),
                WrqReservationFlag = DataAdaptor.ReadBool(reader.data[0].WrqReservationFlag),
                WrqArchiveFlag = DataAdaptor.ReadBool(reader.data[0].WrqArchiveFlag),
                WrqExperimentalFlag = DataAdaptor.ReadBool(reader.data[0].WrqExperimentalFlag)
            };
        }
        internal JSONRPC_Output WorkflowRequestCreate(WorkflowRequest_DTO DTO, ADO Ado, string SamAccountName)
        {
            JSONRPC_Output response = new JSONRPC_Output();
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoWorkflowRequest = new WorkflowRequest_ADO();

            //check if this release already has a Current WorkflowRequest
            if (adoWorkflowRequest.IsCurrent(Ado, DTO.RlsCode))
            {
                //Can't create a comment so we can't proceed
                Log.Instance.Debug("Release already has a live Workflow, can't create");
                response.error = Label.Get("error.duplicate");
                return response;
            }
            //Get Release 
            Release_ADO releaseAdo = new Release_ADO(Ado);
            Release_DTO releaseDTO = Release_ADO.GetReleaseDTO(releaseAdo.Read(DTO.RlsCode, SamAccountName));

            if (releaseDTO == null)
            {
                Log.Instance.Debug("Release Code not found");
                response.error = Label.Get("error.duplicate");
                return response;
            }
            Request_ADO adoRequest = new Request_ADO();
            Request_DTO dtoRequest = new Request_DTO
            {
                RlsCode = DTO.RlsCode
            };

            Release_ADO adoRelease = new Release_ADO(Ado);

            //We must validate the request, depending on the RequestCode and the current state of the Release
            switch (DTO.RqsCode)
            {
                case Constants.C_WORKFLOW_REQUEST_PUBLISH:

                    if (!adoRelease.IsWip(dtoRequest.RlsCode))
                    {
                        Log.Instance.Debug("The requested Release Code is not the WIP Release");
                        response.error = Label.Get("error.create");
                        return response;

                    }

                    break;

                case Constants.C_WORKFLOW_REQUEST_PROPERTY:
                    //Release must be CURRENT LIVE - RlsLiveFlag is live only between from and to dates 
                    if (!adoRelease.IsLiveNow(dtoRequest.RlsCode) && !adoRelease.IsLiveNext(dtoRequest.RlsCode))
                    {
                        Log.Instance.Debug("Cannot create a Flag Request. The Release must be either current Live or Next Live");
                        response.error = Label.Get("error.create");
                        return response;
                    }
                    break;

                case Constants.C_WORKFLOW_REQUEST_DELETE:
                    //What about WIP? - not a workflow request but it is part of Release. It will have its own API.
                    //Release must be LIVE NEXT, CURRENT LIVE or WIP
                    if (!adoRelease.IsLiveNow(dtoRequest.RlsCode) && !adoRelease.IsLiveNext(dtoRequest.RlsCode) && !adoRelease.IsWip(dtoRequest.RlsCode))
                    {
                        Log.Instance.Debug("Can't create a DELETE Request on a historical Release");
                        response.error = Label.Get("error.create");
                        return response;
                    }

                    break;

                case Constants.C_WORKFLOW_REQUEST_ROLLBACK:

                    //Release must be CURRENT LIVE OR NEXT LIVE and PREVIOUS must exist
                    if ((!adoRelease.IsLiveNow(dtoRequest.RlsCode) && !adoRelease.IsLiveNext(dtoRequest.RlsCode)) || !adoRelease.HasPrevious(dtoRequest.RlsCode))
                    {
                        Log.Instance.Debug("Can't create a ROLLBACK Request because (a) the Request is neither live nor pending-live or (b) there is no valid Release to roll back to.");
                        response.error = Label.Get("error.create");
                        return response;
                    }



                    break;

                default:
                    Log.Instance.Debug("Invalid Request Code");
                    response.error = Label.Get("error.validation");
                    return response;


            }
            var adoComment = new Comment_ADO();

            int commentCode = adoComment.Create(Ado, DTO, SamAccountName);

            if (commentCode == 0)
            {
                //Can't create a comment so we can't proceed
                Log.Instance.Debug("Can't create a comment - WorkflowRequest create request refused");
                response.error = Label.Get("error.create");
                return response;
            }

            DTO.CmmCode = commentCode;

            //Create the WorkflowRequest - and retrieve the newly created Id
            int newId = adoWorkflowRequest.Create(Ado, DTO, SamAccountName);
            if (newId == 0)
            {

                response.error = Label.Get("error.create");
                return response;
            }



            response.data = JSONRPC.success;

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

            return response;
        }

        /// <summary>
        /// For a given user, do they have permission based on (a) their privilege and (b) the privileges listed in the configuration
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="ccnUsername"></param>
        /// <param name="permissionConfigItem"></param>
        /// <returns></returns>
        internal bool HasFastrackPermission(ADO ado, string ccnUsername, int RlsCode, string workflowStage)
        {
            ADO_readerOutput result = new Account_BSO().ReadCurrentAccess(ado, ccnUsername);
            if (!result.hasData) return false;
            if (result.data == null) return false;
            if (result.data.Count == 0) return false;


            switch (result.data[0].PrvCode)
            {
                case Constants.C_SECURITY_PRIVILEGE_MODERATOR:
                    if (workflowStage.Equals("workflow.fastrack.signoff")) return false;

                    if (Configuration_BSO.GetCustomConfig(ConfigType.global, workflowStage + ".approver"))
                    {
                        return (this.IsModeratorApprover(ado, ccnUsername, RlsCode));
                    }
                    return false;

                case Constants.C_SECURITY_PRIVILEGE_ADMINISTRATOR:
                    return Configuration_BSO.GetCustomConfig(ConfigType.global, workflowStage + ".administrator");
                case Constants.C_SECURITY_PRIVILEGE_POWER_USER:
                    return Configuration_BSO.GetCustomConfig(ConfigType.global, workflowStage + ".poweruser");

            }


            return false;
        }

        internal JSONRPC_Output WorkflowResponseCreate(WorkflowResponse_DTO DTO, ADO Ado, string SamAccountName)
        {
            JSONRPC_Output response = new JSONRPC_Output();
            //We need to get the Request for notification purposes
            WorkflowRequest_ADO adoWrq = new WorkflowRequest_ADO();
            List<WorkflowRequest_DTO> dtoWrqList = adoWrq.Read(Ado, DTO.RlsCode, true);

            if (dtoWrqList.Count > 1)
            {
                //Multiple requests found for this release
                Log.Instance.Debug("More than one request found for this release ");
                response.error = Label.Get("error.create");
                return response;
            }

            if (dtoWrqList.Count == 0)
            {
                //No request found for this release
                Log.Instance.Debug("No request found for this release ");
                response.error = Label.Get("error.create");
                return response;
            }


            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoWorkflowRequest = new WorkflowRequest_ADO();
            if (!adoWorkflowRequest.IsCurrent(Ado, DTO.RlsCode))
            {
                //No workflow found
                Log.Instance.Debug("No Live workflow found for this Release Code");
                response.error = Label.Get("error.create");
                return response;
            }

            var adoWorkflowResponse = new WorkflowResponse_ADO();

            //If this is a Moderator, we need to check if the user is in the same group as the release and has approve rights
            if (Account_BSO_Read.IsModerator(Ado, SamAccountName))
            {
                var approveRlsList = adoWorkflowResponse.GetApproverAccess(Ado, SamAccountName, true, DTO.RlsCode);
                if (approveRlsList.data.Count == 0)
                {
                    Log.Instance.Debug("Insufficient access for a Moderator");
                    response.error = Label.Get("error.authentication");
                    return response;
                }
            }

            //Check that there isn't already a WorkflowResponse for this Release

            if (adoWorkflowResponse.IsInUse(Ado, DTO))
            {
                //Duplicate Workflow exists
                Log.Instance.Debug("Workflow Response exists already, can't create");
                response.error = Label.Get("error.duplicate");
                return response;
            }


            var adoComment = new Comment_ADO();

            int commentCode = adoComment.Create(Ado, DTO, SamAccountName);

            if (commentCode == 0)
            {
                //Can't create a comment so we can't proceed
                Log.Instance.Debug("Can't create comment - create WorkflowResponse refused");
                response.error = Label.Get("error.create");
                return response;
            }

            DTO.CmmCode = commentCode;



            int createResponse = adoWorkflowResponse.Create(Ado, DTO, SamAccountName);
            if (createResponse == 0)
            {
                response.error = Label.Get("error.create");
                return response;
            }

            //If this is a Reject then we must reset the workflow to stop it being current
            if (DTO.RspCode.Equals(Resources.Constants.C_WORKFLOW_STATUS_REJECT))
            {
                WorkflowRequest_DTO_Update dtoRequest = new WorkflowRequest_DTO_Update(DTO.RlsCode)
                {
                    WrqCurrentFlag = false
                };

                adoWorkflowRequest.Update(Ado, dtoRequest, SamAccountName);
            }

            response.data = JSONRPC.success;

            //Get Release 
            Release_ADO releaseAdo = new Release_ADO(Ado);
            Release_DTO releaseDTO = Release_ADO.GetReleaseDTO(releaseAdo.Read(DTO.RlsCode, SamAccountName));

            Security.ActiveDirectory_DTO responseUser = new Security.ActiveDirectory_DTO() { CcnUsername = SamAccountName };
            Security.ActiveDirectory_ADO accAdo = new Security.ActiveDirectory_ADO();
            Security.Account_DTO_Read accDto = new Security.Account_DTO_Read() { CcnUsername = responseUser.CcnUsername };

            DTO.ResponseAccount = accAdo.GetUser(Ado, accDto);

            Email_BSO_NotifyWorkflow notify = new Email_BSO_NotifyWorkflow();
            try
            {
                notify.EmailResponse(dtoWrqList[0], DTO, releaseDTO);
            }
            catch
            { }
            return response;
        }

        internal bool IsModeratorApprover(ADO Ado, string CcnUsername, int RlsCode)
        {
            var adoWorkflowResponse = new WorkflowResponse_ADO();
            if (Account_BSO_Read.IsModerator(Ado, CcnUsername))
            {
                var approveRlsList = adoWorkflowResponse.GetApproverAccess(Ado, CcnUsername, true, RlsCode);
                return approveRlsList.data.Count > 0;

            }
            return false;
        }

        internal JSONRPC_Output WorkflowSignoffCreate(ADO Ado, WorkflowSignoff_DTO DTO, string SamAccountName)
        {
            JSONRPC_Output response = new JSONRPC_Output();
            ADO_readerOutput moderators = new ADO_readerOutput();
            ADO_readerOutput powerUsers = new ADO_readerOutput();

            var adoWorkflowRequest = new WorkflowRequest_ADO();
            var adoWorkflowResponse = new WorkflowResponse_ADO();

            Release_DTO dtoWip = null;


            if (!adoWorkflowResponse.IsInUse(Ado, DTO)) // is current workflow -- this should be the response!!
            {
                //No Live workflow found so we can't proceed
                Log.Instance.Debug("No Current workflow response found for this Release Code");
                response.error = Label.Get("error.create");
                return response;
            }

            //Is this awaiting signoff?
            var adoWorkflow = new Workflow_ADO();
            ADO_readerOutput resultStatus = adoWorkflow.ReadAwaitingSignoff(Ado, SamAccountName, DTO.RlsCode, Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code"));

            if (!resultStatus.hasData)
            {
                //Release not awaiting signoff so we can't proceed
                Log.Instance.Debug("Release not in status Awaiting Signoff");
                response.error = Label.Get("error.update");
                return response;
            }

            Security.ActiveDirectory_DTO signoffUser = new Security.ActiveDirectory_DTO() { CcnUsername = SamAccountName };
            Security.ActiveDirectory_ADO accAdo = new Security.ActiveDirectory_ADO();
            Security.Account_DTO_Read accDto = new Security.Account_DTO_Read() { CcnUsername = signoffUser.CcnUsername };

            DTO.SignoffAccount = accAdo.GetUser(Ado, accDto);


            var adoSignoff = new WorkflowSignoff_ADO();

            //Create a comment
            var adoComment = new Comment_ADO();
            int commentCode = adoComment.Create(Ado, DTO, SamAccountName);
            if (commentCode == 0)
            {
                // Can't create a comment so we can't proceed
                Log.Instance.Debug("Can't create a comment ");
                response.error = Label.Get("error.create");
                return response;
            }

            DTO.CmmCode = commentCode;

            //We must read the Request and in order to see how we are going to proceed
            WorkflowRequest_ADO adoWrq = new WorkflowRequest_ADO();
            List<WorkflowRequest_DTO> dtoWrqList = adoWrq.Read(Ado, DTO.RlsCode, true);

            if (dtoWrqList.Count > 1)
            {
                //Multiple requests found for this release
                Log.Instance.Debug("More than one request found for this release ");
                response.error = Label.Get("error.create");
                return response;
            }

            //there must be exactly one live Workflow request at this point
            WorkflowRequest_DTO dtoWrq = dtoWrqList.Find(x => x.RqsCode != null);

            //Get the current Release
            Release_ADO adoRelease = new Release_ADO(Ado);
            Release_DTO dtoRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(DTO.RlsCode, SamAccountName));

            if (dtoRelease == null)
            {
                Log.Instance.Debug("Release not found");
                response.error = Label.Get("error.create");
                return response;
            }

            Account_BSO aBso = new Account_BSO();

            moderators = aBso.getReleaseUsers(DTO.RlsCode, null);
            powerUsers = aBso.getUsersOfPrivilege(Constants.C_SECURITY_PRIVILEGE_POWER_USER);

            //Is this a Reject?
            if (DTO.SgnCode.Equals(Constants.C_WORKFLOW_STATUS_REJECT))
            {

                int res = adoSignoff.Create(Ado, DTO, SamAccountName);

                if (res == 0)
                {
                    //Can't create a Workflow Signoff so we can't proceed
                    Log.Instance.Debug("Can't create a Workflow Signoff ");
                    response.error = Label.Get("error.create");
                    return response;
                }


                WorkflowRequest_DTO_Update dtoReq = new WorkflowRequest_DTO_Update(DTO.RlsCode);
                dtoReq.WrqCurrentFlag = false;

                //update the request
                int reqUpdate = adoWorkflowRequest.Update(Ado, dtoReq, SamAccountName);
                if (reqUpdate == 0)
                {
                    //Can't save the Request so we can't proceed
                    Log.Instance.Debug("Can't save the Workflow Request");
                    response.error = Label.Get("error.update");
                    return response;
                }

                DTO.MtrCode = dtoRelease.MtrCode; // we need this to see which cache we must flush

                response.data = JSONRPC.success;

                Email_BSO_NotifyWorkflow notifyReject = new Email_BSO_NotifyWorkflow();

                try
                {
                    notifyReject.EmailSignoff(dtoWrq, DTO, dtoRelease, moderators, powerUsers);
                }
                catch { }

                return response;
            }

            //Not a Reject so we proceed...
            switch (dtoWrq.RqsCode)
            {
                case Constants.C_WORKFLOW_REQUEST_PUBLISH:

                    if (String.IsNullOrEmpty(dtoRelease.PrcCode))
                    {
                        //There must be a valid product for this release
                        Log.Instance.Debug("No product found for the release ");
                        response.error = Label.Get("error.publish");
                        return response;
                    }

                    //Update the current release LiveDatetimeTo to the request Date time
                    dtoRelease.RlsLiveDatetimeFrom = dtoWrq.WrqDatetime;

                    //set the release live flag
                    //update the release version and set the current revision to 0
                    DateTime switchDate;
                    switchDate = DateTime.Now > dtoWrq.WrqDatetime ? DateTime.Now : dtoWrq.WrqDatetime;
                    dtoRelease.RlsVersion++;
                    dtoRelease.RlsRevision = 0;
                    dtoRelease.RlsLiveFlag = true;
                    dtoRelease.RlsExceptionalFlag = dtoWrq.WrqExceptionalFlag != null ? dtoWrq.WrqExceptionalFlag.Value : false;
                    dtoRelease.RlsReservationFlag = dtoWrq.WrqReservationFlag != null ? dtoWrq.WrqReservationFlag.Value : false;
                    dtoRelease.RlsArchiveFlag = dtoWrq.WrqArchiveFlag != null ? dtoWrq.WrqArchiveFlag.Value : false;
                    dtoRelease.RlsExperimentalFlag = dtoWrq.WrqExperimentalFlag != null ? dtoWrq.WrqExperimentalFlag.Value : false;
                    dtoRelease.RlsLiveDatetimeFrom = switchDate;



                    //get the current live release

                    Release_DTO releaseDTONow = Release_ADO.GetReleaseDTO(adoRelease.ReadLiveNow(DTO.RlsCode));

                    //Save the changes for the release we're changing
                    int update = adoRelease.Update(dtoRelease, SamAccountName);
                    if (update == 0)
                    {
                        Log.Instance.Debug("Can't update the Release, RlsCode:" + dtoRelease.RlsCode);
                        response.error = Label.Get("error.update");
                        return response;
                    }

                    if (releaseDTONow != null)
                    {
                        //...if there is a previous release
                        if (releaseDTONow.RlsCode != 0)
                        {
                            //Update the  Live LiveDatetimeTo to the request Datetime
                            releaseDTONow.RlsLiveDatetimeTo = switchDate;
                            //Save the changes for the previous release
                            adoRelease.Update(releaseDTONow, SamAccountName);

                        }
                    }


                    break;

                case Constants.C_WORKFLOW_REQUEST_PROPERTY:
                    //update release to transfer all flag values from the request to the release                            
                    dtoRelease.RlsReservationFlag = dtoWrq.WrqReservationFlag != null ? dtoWrq.WrqReservationFlag.Value : false;
                    dtoRelease.RlsArchiveFlag = dtoWrq.WrqArchiveFlag != null ? dtoWrq.WrqArchiveFlag.Value : false;
                    dtoRelease.RlsExperimentalFlag = dtoWrq.WrqExperimentalFlag != null ? dtoWrq.WrqExperimentalFlag.Value : false;

                    //Save the release
                    int updateCount = adoRelease.Update(dtoRelease, SamAccountName);
                    if (updateCount == 0)
                    {
                        //Update of Release failed
                        Log.Instance.Debug("Can't update the Release, RlsCode:" + DTO.RlsCode);
                        response.error = Label.Get("error.update");
                        return response;
                    }



                    //if there is a WIP or a pending live associated with this matrix then we need to update the WIP/Pending Live as well:
                    Release_BSO rBso = new Release_BSO(Ado);
                    dynamic wipForLive = rBso.GetWipForLive(dtoRelease.RlsCode, SamAccountName);
                    if (wipForLive == null)
                    {
                        wipForLive = rBso.GetPendingLiveForLive(dtoRelease.RlsCode, SamAccountName);
                    }
                    if (wipForLive != null)
                    {

                        //if a workflow exists for wipForLive, then we must update the flags on that workflow as well
                        var wfForLive = adoWorkflowRequest.Read(Ado, wipForLive.RlsCode, true);
                        if (wfForLive != null)
                        {
                            if (wfForLive.Count > 0)
                            {
                                adoWorkflowRequest.Update(Ado, new WorkflowRequest_DTO_Update()
                                {
                                    RlsCode = wipForLive.RlsCode,
                                    WrqArchiveFlag = dtoWrq.WrqArchiveFlag,
                                    WrqCurrentFlag = true,
                                    WrqExperimentalFlag = dtoWrq.WrqExperimentalFlag,
                                    WrqReservationFlag = dtoWrq.WrqReservationFlag
                                }, SamAccountName);
                            }
                        }

                        dtoWip = Release_ADO.GetReleaseDTO(adoRelease.Read(wipForLive.RlsCode, SamAccountName));
                        dtoWip.RlsReservationFlag = dtoRelease.RlsReservationFlag;
                        dtoWip.RlsArchiveFlag = dtoRelease.RlsArchiveFlag;
                        dtoWip.RlsExperimentalFlag = dtoRelease.RlsExperimentalFlag;

                        if (adoRelease.Update(dtoWip, SamAccountName) == 0)
                        {
                            Log.Instance.Debug("Failed to update associated WIP " + dtoWip.MtrCode + " " + dtoWip.RlsVersion + '.' + dtoWip.RlsRevision);
                        }

                        //if this wip has a workflow request, then the workflow request details must also be updated

                        List<WorkflowRequest_DTO> wfList = adoWrq.Read(Ado, dtoWip.RlsCode, true);
                        if (wfList.Count > 0)
                        {
                            foreach (var wf in wfList)
                            {
                                wf.WrqReservationFlag = dtoWrq.WrqReservationFlag;
                                wf.WrqArchiveFlag = dtoWrq.WrqArchiveFlag;
                                wf.WrqExperimentalFlag = dtoWrq.WrqExperimentalFlag;
                                adoWrq.Update(Ado, new WorkflowRequest_DTO_Update()
                                {
                                    RlsCode = wf.RlsCode,
                                    WrqCurrentFlag = dtoWrq.WrqCurrentFlag,
                                    WrqArchiveFlag = dtoWrq.WrqArchiveFlag,
                                    WrqExperimentalFlag = dtoWrq.WrqExperimentalFlag,
                                    WrqReservationFlag = dtoWrq.WrqReservationFlag
                                }, SamAccountName);
                            }
                        }
                    }

                    break;

                case Constants.C_WORKFLOW_REQUEST_DELETE:
                    //We can't soft delete the release just yet. We need it to be live until the Request is updated.

                    break;

                case Constants.C_WORKFLOW_REQUEST_ROLLBACK:

                    //Delete the future release if it exists and set the current to_date to null
                    //Otherwise delete the current release and make the previous release current by setting its to_date to null



                    if (adoRelease.IsLiveNext(dtoRelease.RlsCode))//this is a future release so get the previous release to roll back to (even if that previous is now historical)
                    {

                        Compare_ADO cAdo = new Compare_ADO(Ado);
                        Release_DTO dtoNowRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(cAdo.ReadPreviousRelease(DTO.RlsCode), SamAccountName));


                        dtoNowRelease.RlsLiveDatetimeTo = default(DateTime);
                        int rows = adoRelease.Update(dtoNowRelease, SamAccountName);
                        if (rows == 0)
                        {
                            Log.Instance.Debug("Can't update the Release, RlsCode:" + dtoNowRelease.RlsCode);
                            response.error = Label.Get("error.update");
                            return response;
                        }

                        //As things stand, dtoRelease is the requested Release (which is a Live Next). This will be deleted in the Delete section below

                    }
                    else
                    {
                        //This isn't a future release - it had better be a Live Now (with a previous)
                        if (!adoRelease.IsLiveNow(dtoRelease.RlsCode))
                        {
                            //If the request is neither a Live Now release then there's a problem
                            Log.Instance.Debug("Can't delete the Release, RlsCode:" + dtoRelease.RlsCode + ". Release is not current live");
                            response.error = Label.Get("error.delete");
                            return response;
                        }

                        //Find the release that we're trying to rollback to
                        Release_DTO dtoPrevious = Release_ADO.GetReleaseDTO(adoRelease.ReadLivePrevious(dtoRelease.RlsCode));
                        if (dtoPrevious.RlsCode == 0)
                        {
                            //Previous release not found
                            //You can't roll back unless there's something to roll back to, so...
                            Log.Instance.Debug("Can't delete the Release, RlsCode:" + dtoRelease.RlsCode + ". Release is not current live");
                            response.error = Label.Get("error.delete");
                            return response;
                        }


                        //We set the DatetimeTo to null in the previous release
                        dtoPrevious.RlsLiveDatetimeTo = default(DateTime);
                        int rows = adoRelease.Update(dtoPrevious, SamAccountName);
                        if (rows == 0)
                        {
                            Log.Instance.Debug("Can't update the Release, RlsCode:" + dtoPrevious.RlsCode);
                            response.error = Label.Get("error.update");
                            return response;
                        }


                        //Do the rollback of the current release
                        dtoRelease.RlsVersion = dtoPrevious.RlsVersion;
                        dtoRelease.RlsLiveDatetimeFrom = default(DateTime);

                        rows = adoRelease.Update(dtoRelease, SamAccountName);
                        if (rows == 0)
                        {
                            Log.Instance.Debug("Can't update the Release, RlsCode:" + dtoRelease.RlsCode);
                            response.error = Label.Get("error.update");
                            return response;
                        }

                        adoRelease.IncrementRevision(dtoRelease.RlsCode, SamAccountName);
                    }


                    break;

                default:
                    response.error = Label.Get("error.update");
                    return response;

            }

            int signoffID = adoSignoff.Create(Ado, DTO, SamAccountName);

            if (signoffID == 0)
            {
                //Can't create a Workflow Signoff so we can't proceed
                Log.Instance.Debug("Can't create a Workflow Signoff ");
                response.error = Label.Get("error.create");
                return response;
            }

            //In all cases, if we have reached this stage, we must update the request to make it non-current
            WorkflowRequest_DTO_Update dtoRequest = new WorkflowRequest_DTO_Update(DTO.RlsCode);
            dtoRequest.WrqCurrentFlag = false;

            //save the request
            int updated = adoWorkflowRequest.Update(Ado, dtoRequest, SamAccountName);
            if (updated == 0)
            {
                //Can't save the Request so we can't proceed
                Log.Instance.Debug("Can't save the Workflow Signoff");
                response.error = Label.Get("error.update");
                return response;
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

                    if (adoRelease.IsLiveNow(dtoRelease.RlsCode))
                    {
                        Release_DTO dtoNowRelease = Release_ADO.GetReleaseDTO(adoRelease.ReadLiveNow(dtoRequest.RlsCode));

                        //Set the toDate to now, thus setting the release to historical
                        if (dtoNowRelease != null)
                        {
                            dtoNowRelease.RlsLiveDatetimeTo = DateTime.Now;
                            int updateCount = adoRelease.Update(dtoNowRelease, SamAccountName);
                            if (updateCount == 0)
                            {
                                Log.Instance.Debug("Can't update the Release, RlsCode:" + dtoNowRelease.RlsCode);
                                response.error = Label.Get("error.update");
                                return response;
                            }

                        }

                        //Delete the search keywords for this release
                        krbAdo.Delete(Ado, DTO.RlsCode, null, true);

                    }
                    else if (adoRelease.IsLiveNext(dtoRelease.RlsCode) || adoRelease.IsWip(dtoRelease.RlsCode))
                    {
                        //Find the previous list if it exists
                        Compare_ADO cAdo = new Compare_ADO(Ado);
                        Release_DTO dtoPreviousRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(cAdo.ReadPreviousRelease(DTO.RlsCode), SamAccountName));

                        //if there is a previous live set it to historical, but not if we're deleting a WIP
                        if (dtoPreviousRelease != null && !adoRelease.IsWip(dtoRelease.RlsCode))
                        {
                            //Delete the search keywords for the previous release
                            krbAdo.Delete(Ado, dtoPreviousRelease.RlsCode, null, true);

                            dtoPreviousRelease.RlsLiveDatetimeTo = DateTime.Now;
                            int updateCount = adoRelease.Update(dtoPreviousRelease, SamAccountName);
                            if (updateCount == 0)
                            {
                                Log.Instance.Debug("Can't update the Release, RlsCode:" + dtoPreviousRelease.RlsCode);
                                response.error = Label.Get("error.update");
                                return response;
                            }
                        }


                        //Delete the search keywords for this release
                        krbAdo.Delete(Ado, DTO.RlsCode, null, true);

                        // We may now proceed with the soft delete
                        if (bsoDelete.Delete(Ado, DTO.RlsCode, SamAccountName, true) == 0)
                        {
                            Log.Instance.Debug("Can't delete the Release, RlsCode:" + DTO.RlsCode);
                            response.error = Label.Get("error.delete");
                            return response;
                        }



                    }
                    else
                    {
                        //Only LiveNow, LiveNext and WIP releases can be deleted. Anything else means there's a problem.
                        Log.Instance.Debug("Can't delete the Release - invalid release status, RlsCode:" + DTO.RlsCode);
                        response.error = Label.Get("error.delete");
                        return response;
                    }

                    break;

                case Constants.C_WORKFLOW_REQUEST_ROLLBACK:

                    //First, if there is a WIP ahead of this live release then that WIP must be deleted
                    Release_ADO releaseAdo = new Release_ADO(Ado);
                    var releaseDTORead = new Release_DTO_Read() { MtrCode = dtoRelease.MtrCode };
                    var latestRelease = releaseAdo.ReadLatest(releaseDTORead);
                    if (latestRelease != null)
                    {
                        if (dtoRelease.RlsCode != latestRelease.RlsCode)
                        {
                            if (bsoDelete.Delete(Ado, latestRelease.RlsCode, SamAccountName, true) == 0)
                            {
                                Log.Instance.Debug("Can't delete the Release, RlsCode:" + latestRelease.RlsCode);
                                response.error = Label.Get("error.delete");
                                return response;
                            }
                        }
                    }

                    // Only Live Next gets soft deleted, while Live Now is turned historical above
                    if (adoRelease.IsLiveNext(dtoRelease.RlsCode))
                    {
                        if (bsoDelete.Delete(Ado, DTO.RlsCode, SamAccountName, true) == 0)
                        {
                            Log.Instance.Debug("Can't delete the Release, RlsCode:" + DTO.RlsCode);
                            response.error = Label.Get("error.delete");
                            return response;
                        }

                        //Delete the search keywords for this release
                        krbAdo.Delete(Ado, DTO.RlsCode, null, true);
                    }

                    // Delete the requested release (it may have been live but may also have been demoted to WIP by now)
                    if (adoRelease.IsWip(dtoRelease.RlsCode))
                    {
                        if (bsoDelete.Delete(Ado, dtoRelease.RlsCode, SamAccountName, true) == 0)
                        {
                            Log.Instance.Debug("Can't delete the Release, RlsCode:" + dtoRelease.RlsCode);
                            response.error = Label.Get("error.delete");
                            return response;
                        }

                        //Delete the search keywords for this release
                        krbAdo.Delete(Ado, DTO.RlsCode, null, true);
                    }
                    break;

            }


            DTO.MtrCode = dtoRelease.MtrCode; // we need this to see which cache we must flush

            response.data = JSONRPC.success;
            Email_BSO_NotifyWorkflow notify = new Email_BSO_NotifyWorkflow();

            // Clean up caching
            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_ADDITION + DTO.RlsCode);
            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_DELETION + DTO.RlsCode);
            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_AMENDMENT + DTO.RlsCode);

            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET + DTO.RlsCode);
            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA + DTO.RlsCode);

            if (dtoWip != null)
            {
                MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_ADDITION + dtoWip.RlsCode);
                MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_DELETION + dtoWip.RlsCode);
                MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_COMPARE_READ_AMENDMENT + dtoWip.RlsCode);

                MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_DATASET + dtoWip.RlsCode);
                MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_PRE_METADATA + dtoWip.RlsCode);
            }

            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_NAVIGATION_SEARCH);
            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_NAVIGATION_READ);
            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_COLLECTION);

            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + DTO.MtrCode);

            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + DTO.MtrCode);

            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_METADATA + DTO.MtrCode);

            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_COLLECTION_PXAPI);

            try
            {
                notify.EmailSignoff(dtoWrq, DTO, dtoRelease, moderators, powerUsers);
            }
            catch { }



            return response;
        }
    }
}
