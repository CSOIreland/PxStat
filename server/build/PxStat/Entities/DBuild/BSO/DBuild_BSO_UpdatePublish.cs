using API;
using FluentValidation.Results;
using Ganss.Xss;
using Newtonsoft.Json;
using PxStat.Data;
using PxStat.DataStore;
using PxStat.Resources;
using PxStat.Security;
using PxStat.Template;
using PxStat.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;

namespace PxStat.DBuild
{
    public class DBuild_BSO_UpdatePublish : BaseTemplate_Create<DBuild_DTO_UpdatePublish, DBuild_VLD_UpdatePublish>
    {
      

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal DBuild_BSO_UpdatePublish(JSONRPC_API request) : base(request, new DBuild_VLD_UpdatePublish())
        { }

        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        protected override bool Execute()
        {
            
            
            //Get the latest release for the Matrix code
            Release_ADO releaseAdo = new Release_ADO(Ado);
            var releaseDTORead = new Release_DTO_Read() { MtrCode = DTO.MtrCode };
            Release_DTO release= Release_ADO.GetReleaseDTO(releaseAdo.ReadLatestIgnoreCancelled(releaseDTORead));

            if(release==null)
            {
                Response.error = Label.Get("error.create", DTO.LngIsoCode);
                Log.Instance.Error(String.Format("No release found for MtrCode {0}", DTO.MtrCode));
                return false;
            }

            //Now we must check if the user has access to the group to which the release belongs

            if (IsModerator(SamAccountName))
            {
                Group_ADO adoGroup = new Group_ADO();
                var usersGroups = adoGroup.ReadAccess(Ado, SamAccountName);
                if (usersGroups.hasData)
                {
                    if (!usersGroups.data.Select(x => x.GrpCode).Contains(release.GrpCode))
                    {
                        Response.error = Label.Get("error.privilege", DTO.LngIsoCode);
                        Log.Instance.Error(String.Format("User {0} not found in group {1} for px auto update",SamAccountName, release.GrpCode));
                        return false;
                    }
                }
                else
                {
                    Response.error = Label.Get("error.privilege", DTO.LngIsoCode);
                    Log.Instance.Error(String.Format("No group access found for user {0} in px auto update", SamAccountName));
                    return false;
                }

            }

            if (release == null) 
            {
                Response.error = Label.Get("error.release.not-found",DTO.LngIsoCode);
                Log.Instance.Error(String.Format("Release not found in px auto update"));
                return false;
            }

            //Get a matrix that contains all languages
            IDmatrix matrix = new Dmatrix();
            matrix=matrix.GetMultiLanguageMatrixFromRelease(Ado, DTO.MtrCode, release);

            if (!matrix.Validate())
            {
                Response.error = Label.Get("error.validation") + " " + matrix.ValidationResult.ToString();
                Log.Instance.Error("Matrix Validation failed after db read");
                return false;
            }

            UpdatePublishValidations upv = new();
            if(!upv.ValidateDtoAgainstMatrix(DTO,matrix,matrix.Language))
            {
                Response.error =Label.Get("error.validation") + " " + upv.ValidationResult.ToString();
                Log.Instance.Error("Publish Validation failed after db read");
                return false;
            }

            //Update the matrix with the changes
            DBuild_BSO bbso= new();
            var dto=DTO.Dspecs.Where(x=>x.Language==DTO.LngIsoCode).FirstOrDefault();
            

            matrix = bbso.BsoUpdate(matrix, Response, DTO, Ado, null, DTO.LngIsoCode);
           
           if(!matrix.Validate())
            {
                Response.error = Label.Get("error.validation") + " " + matrix.ValidationResult.ToString();
                Log.Instance.Error("Matrix Validation failed after update");
                return false;   
            }
            //Create a new release with this matrix
            Release_BSO rbso = new Release_BSO(Ado);
            int rlsId = rbso.CreateRelease(matrix, DTO,SamAccountName );

            if (rlsId == 0) 
            {
                Response.error = rbso.ErrorMessage;
                Log.Instance.Error("Release not found after new release create");
                return false;
            }

            //Save a px expression of the matrix as source
            PxFileBuilder pxb = new PxFileBuilder();
            Matrix_ADO mAdo = new Matrix_ADO(Ado);
            string source = pxb.Create(matrix, matrix.Language);
            foreach (string lng in matrix.Languages)
            {
                matrix.Dspecs[lng].Source = source;
                mAdo.Update(matrix.Dspecs[lng]);
            }


            //Get the RlsCode for the current live release
            Release_DTO latestLiveRelease = Release_ADO.GetReleaseDTO(releaseAdo.ReadLatestLive(releaseDTORead));

            if (latestLiveRelease == null) 
            {
                Response.error = Label.Get("error.release.not-found", DTO.LngIsoCode);
                Log.Instance.Error("Latest Live Release not found after new release create");
                return false;
            }
            
            Release_DTO newRelease = Release_ADO.GetReleaseDTO(releaseAdo.ReadLatestIgnoreCancelled(releaseDTORead));

            //Create a ReasonRelease based on the default Release Reason
            string reasonCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.server, "release.defaultReason");

            ReasonRelease_ADO rrAdo = new();
            var checkReason = rrAdo.Read(Ado, new ReasonRelease_DTO_Read() { RlsCode = newRelease.RlsCode, RsnCode = reasonCode, LngIsoCode=DTO.LngIsoCode });
            if(!checkReason.hasData)
                rrAdo.Create(Ado, new ReasonRelease_DTO_Create() { RlsCode = newRelease.RlsCode, RsnCode = reasonCode, LngIsoCode = DTO.LngIsoCode }, SamAccountName);

            WorkflowRequest_ADO wAdo =new WorkflowRequest_ADO();
            List<WorkflowRequest_DTO> dtoWrqList = wAdo.Read(Ado, release.RlsCode); // use the current live release

            if (dtoWrqList == null)
            {
                Response.error = String.Format(Label.Get("error.workflow-request.not-found", DTO.LngIsoCode), DTO.MtrCode);
                return false;
            }
            if (dtoWrqList.Count == 0)
            {
                Response.error = String.Format(Label.Get("error.workflow-request.not-found", DTO.LngIsoCode), DTO.MtrCode);
                return false;
            }

            WorkflowRequest_DTO wDto = dtoWrqList.First();
            wDto.WrqDatetime = DTO.WrqDatetime;
            wDto.RlsCode = newRelease.RlsCode; //use the new wip release
            wDto.CmmValue = Label.Get("workflow.request.auto-create-comment", DTO.LngIsoCode);

            wDto = this.UpdateFromRequest(wDto, newRelease);

            Workflow_BSO wbso = new();
            var response=wbso.WorkflowRequestCreate(wDto, Ado, SamAccountName ,new List<WorkflowRequest_DTO>() { wDto});

            bool responseComplete = false;
            //If this is a user with an automatic flow to the next stage, then do the response immediately
            if (wbso.HasFastrackPermission(Ado, SamAccountName, newRelease.RlsCode, "workflow.fastrack.response"))
            {
                Log.Instance.Debug("Fastrack from Request to Response");
                WorkflowResponse_DTO rspDto = new WorkflowResponse_DTO() { RlsCode = newRelease.RlsCode, RspCode = Constants.C_WORKFLOW_STATUS_APPROVE, CmmValue = Label.Get("auto-approve-comment") };
                Response = wbso.WorkflowResponseCreate(rspDto, Ado, SamAccountName);

                responseComplete = true;
            }

            //If this is a user with an automatic flow to the next stage (and the response has been completed), then do the signoff immediately
            if (wbso.HasFastrackPermission(Ado, SamAccountName, newRelease.RlsCode, "workflow.fastrack.signoff") && responseComplete)
            {

                Log.Instance.Debug("Fastrack from Request to Signoff via Response");
                WorkflowSignoff_DTO sgnDTO = new WorkflowSignoff_DTO() { RlsCode = newRelease.RlsCode, SgnCode = Constants.C_WORKFLOW_STATUS_APPROVE, CmmValue = Label.Get("auto-approve-signoff") };
                Response = wbso.WorkflowSignoffCreate(Ado, sgnDTO, SamAccountName);
   
            }



            

                if (response == null) 
            {
                Response.error = Label.Get("error.exception", DTO.LngIsoCode);
                return false;

            }

            if(response.error != null) 
            { 
                Response.error=response.error;
                return false;
            }

            Response.data= response.data;
            return true;
        }

        private WorkflowRequest_DTO UpdateFromRequest(WorkflowRequest_DTO request,Release_DTO release) 
        {
            request.WrqArchiveFlag = release.RlsArchiveFlag;
            request.WrqArchiveFlag = release.RlsArchiveFlag;
            request.WrqExceptionalFlag = release.RlsExceptionalFlag;
            request.WrqExperimentalFlag = release.RlsExperimentalFlag;
            request.WrqReservationFlag=release.RlsReservationFlag;
            return request;
        
        }

    }

    internal class UpdatePublishValidations
    {
        internal string ValidationResult {  get; set; } 
        internal bool ValidateDtoAgainstMatrix(DBuild_DTO_UpdatePublish dto, IDmatrix matrix, string lngIsoCode)
        {
            if(dto.ChangeData==null)
            {
                ValidationResult = Label.Get("px.build.dimension-code-empty", lngIsoCode);
                return false;
            }
            List<string> dcodes = matrix.Dspecs[lngIsoCode].Dimensions.Select(x => x.Code).ToList<string>();
            List<string> header = dto.ChangeData.ElementAt(0);
            
            if(!dcodes.Intersect(header).Count().Equals(dcodes.Count))
            {
                ValidationResult = Label.Get("px.build.dimension-code-empty", lngIsoCode);
                return false;
            }

            if (!header.Intersect(dcodes).Count().Equals(dcodes.Count))
            {
                ValidationResult = Label.Get("px.build.dimension-code-empty", lngIsoCode);
                return false;
            }

            foreach(var item in dto.ChangeData) 
            { 
                if(item.Count!=header.Count)
                {
                    ValidationResult = Label.Get("px.build.variable-error", lngIsoCode);
                    return false;
                }
            }

            

            return true;
        }
    }

}
