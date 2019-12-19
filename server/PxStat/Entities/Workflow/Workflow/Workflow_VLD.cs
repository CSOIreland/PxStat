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
}
