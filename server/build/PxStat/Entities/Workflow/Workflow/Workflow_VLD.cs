using FluentValidation;

namespace PxStat.Workflow
{
    /// <summary>
    /// Validator for Workflow
    /// </summary>
    internal class Workflow_VLD : AbstractValidator<Workflow_DTO>
    {

        internal Workflow_VLD()
        {
            //Optional - RlsCode
            RuleFor(f => f.RlsCode).GreaterThan(0).When(f => f.RlsCode != default(int)).WithMessage("Invalid Release Code").WithName("InvalidRlsCode");
            RuleFor(x => x.LngIsoCode).NotEmpty().Length(2);
        }
    }

    internal class Workflow_VLD_CancelPendingLive : AbstractValidator<Workflow_DTO_CancelPendingLive>
    {
        internal Workflow_VLD_CancelPendingLive()
        {
            RuleFor(f => f.RlsCode).GreaterThan(0).WithMessage("Invalid Release Code").WithName("InvalidRlsCode");
            RuleFor(f => f.CmmValue).Length(1, 1024).When(f => !string.IsNullOrEmpty(f.CmmValue)).WithMessage("Invalid Workflow Pending Live Cancel Comment").WithName("CommentValidation");
        }
    }
}
