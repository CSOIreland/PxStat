using FluentValidation;

namespace PxStat.Workflow
{
    /// <summary>
    /// Validator for WorkflowSignoff Create
    /// </summary>
    internal class WorkflowSignoff_VLD_Create : AbstractValidator<WorkflowSignoff_DTO>
    {

        internal WorkflowSignoff_VLD_Create()
        {
            //Mandatory - CmmValue
            RuleFor(f => f.CmmValue).Length(1, 1024).When(f => !string.IsNullOrEmpty(f.CmmValue)).WithMessage("Invalid Comment Value").WithName("CommentValueValidation");
            //Mandatory - SgnCode
            RuleFor(f => f.SgnCode).Length(1, 32).WithMessage("Invalid Signoff Code").WithName("SignoffCodeValidation");
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty().WithMessage("Invalid Release Code").WithName("ReleaseCodeValidation");
        }
    }

    /// <summary>
    /// Validator for WorkflowSignoff Update
    /// </summary>
    internal class WorkflowSignoff_VLD_Update : AbstractValidator<WorkflowSignoff_DTO>
    {
        internal WorkflowSignoff_VLD_Update()
        {
            //Mandatory - CmmValue
            RuleFor(f => f.CmmValue).Length(1, 1024).When(f => !string.IsNullOrEmpty(f.CmmValue)).WithMessage("Invalid Comment Value").WithName("CommentValueValidation");
        }
    }

}