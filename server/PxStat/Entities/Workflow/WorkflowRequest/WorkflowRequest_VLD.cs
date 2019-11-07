using FluentValidation;
using PxStat.Resources;
using System;

namespace PxStat.Workflow
{
    /// <summary>
    /// Validator for Workflow Request Create
    /// </summary>
    internal class WorkflowRequest_VLD_Create : AbstractValidator<WorkflowRequest_DTO>
    {

        internal WorkflowRequest_VLD_Create()
        {
            //Mandatory - RqsCode - Mandatory for all RqsCodes
            RuleFor(f => f.RqsCode).Length(1, 32).WithMessage("Invalid Request Code").WithName("RequestCodeValidation");
            //Mandatory - CmmValue - Mandatory for all RqsCodes 
            RuleFor(f => f.CmmValue).Length(1, 1024).When(f => !string.IsNullOrEmpty(f.CmmValue) || f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_ROLLBACK) || f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_DELETE)).WithMessage("Invalid Comment Value").WithName("CommentValueValidation");

            //Mandatory - WrqDatetime - Mandatory for Publish
            RuleFor(f => f.WrqDatetime).Must(f => !(f.Equals(default(DateTime)))).WithMessage("Invalid Request Date").WithName("WrqRequestDateValidation").When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH)); ;
            //Mandatory - RlsCode - Mandatory for All RqsCodes
            RuleFor(f => f.RlsCode).NotEmpty().WithMessage("Invalid Release Code").WithName("WrqReleaseCodeValidation");
            //Mandatory - WrqEmergencyFlag - Mandatory for Publish
            RuleFor(f => f.WrqEmergencyFlag).NotNull().When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH));
            //Mandatory - WrqReservationFlag - Mandatory for Publish and Flag
            RuleFor(f => f.WrqReservationFlag).NotNull().When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH) || f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PROPERTY));
            //Mandatory - WrqArchiveFlag - Mandatory for Publish and Flag
            RuleFor(f => f.WrqArchiveFlag).NotNull().When(f => f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PUBLISH) || f.RqsCode.Equals(Constants.C_WORKFLOW_REQUEST_PROPERTY));

        }
    }

    /// <summary>
    /// Validator for Workflow Request Update
    /// </summary>
    internal class WorkflowRequest_VLD_Update : AbstractValidator<WorkflowRequest_DTO_Update>
    {
        internal WorkflowRequest_VLD_Update()
        {
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty();
            //Mandatory - WrqCurrentFlag
            RuleFor(f => f.WrqCurrentFlag).NotEmpty();
        }

    }

    /// <summary>
    /// Validator for Workflow Request Delete
    /// </summary>
    internal class WorkflowRequest_VLD_Delete : AbstractValidator<WorkflowRequest_DTO>
    {
        internal WorkflowRequest_VLD_Delete()
        {
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty();
        }
    }

}
