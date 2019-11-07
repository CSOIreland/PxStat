using FluentValidation;

namespace PxStat.Workflow
{
    /// <summary>
    /// Validator for WorkflowResponse Create
    /// </summary>
    internal class WorkflowResponse_VLD_Create : AbstractValidator<WorkflowResponse_DTO>
    {

        internal WorkflowResponse_VLD_Create()
        {
            //Mandatory - CmmValue
            RuleFor(f => f.CmmValue).Length(1, 1024).When(f => !string.IsNullOrEmpty(f.CmmValue)).WithMessage("Invalid Comment Value").WithName("CommentValueValidation");
            //Mandatory - RspCode
            RuleFor(f => f.RspCode).Length(1, 32).WithMessage("Invalid Response Code").WithName("ResponseCodeValidation");
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty().WithMessage("Invalid Release Code").WithName("ReleaseCodeValidation");
        }
    }

    /// <summary>
    /// Validator for WorkflowResponse update
    /// </summary>
    internal class WorkflowResponse_VLD_Update : AbstractValidator<WorkflowResponse_DTO>
    {
        internal WorkflowResponse_VLD_Update()
        {
            //Mandatory - CmmValue
            RuleFor(f => f.CmmValue).Length(1, 1024).When(f => !string.IsNullOrEmpty(f.CmmValue)).WithMessage("Invalid Comment Value").WithName("CommentValueValidation");
        }
    }

}