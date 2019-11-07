using FluentValidation;

namespace PxStat.Workflow
{
    /// <summary>
    /// Validator for Request
    /// </summary>
    internal class Request_VLD : AbstractValidator<Request_DTO>
    {
        internal Request_VLD()
        {
            //Optional - RlsCode
            RuleFor(f => f.RlsCode).GreaterThan(0).When(f => f.RlsCode != default(int)).WithMessage("Invalid Release Code").WithName("InvalidRlsCode");
        }
    }
}