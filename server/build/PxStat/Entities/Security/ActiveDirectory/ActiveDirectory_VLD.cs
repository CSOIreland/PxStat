using FluentValidation;

namespace PxStat.Security
{
    /// <summary>
    /// Validator for Active Directory DTO
    /// </summary>
    internal class ActiveDirectory_VLD : AbstractValidator<ActiveDirectory_DTO>
    {
        public ActiveDirectory_VLD()
        {
            //Optional - CcnUsername
            RuleFor(f => f.CcnUsername).NotEmpty().Length(1, 256).When(f => !string.IsNullOrEmpty(f.CcnUsername)).WithMessage("Invalid Username").WithName("CcnUsernameValidation");
        }
    }
}