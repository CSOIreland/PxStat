using FluentValidation;

namespace PxStat.Security
{
    /// <summary>
    /// Validator class for Privilege Read
    /// </summary>
    internal class Privilege_VLD : AbstractValidator<Privilege_DTO>
    {
        internal Privilege_VLD()
        {
            //Optional - PrvCode
            RuleFor(f => f.PrvCode).NotEmpty().Length(1, 32).When(f => !string.IsNullOrEmpty(f.PrvCode)).WithMessage("Invalid Privilege Code").WithName("PrvCodeValidator");
        }
    }
}