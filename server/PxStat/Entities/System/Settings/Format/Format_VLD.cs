using FluentValidation;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Validator for Format Read 
    /// </summary>
    internal class Format_VLD_Read : AbstractValidator<Format_DTO_Read>
    {
        internal Format_VLD_Read()
        {
            //Optional - FrmType
            RuleFor(f => f.FrmType).NotEmpty().When(f => !string.IsNullOrEmpty(f.FrmType)).Length(1, 32).WithMessage("Invalid Format Type").WithName("FrmTypeValidation");
            //Optional - FrmVersion
            RuleFor(f => f.FrmVersion).NotEmpty().When(f => !string.IsNullOrEmpty(f.FrmVersion)).Length(1, 32).WithMessage("Invalid Format Version").WithName("FrmVersionValidation");
            //Optional - FrmDirection
            RuleFor(f => f.FrmDirection).NotEmpty().When(f => !string.IsNullOrEmpty(f.FrmDirection)).Length(1, 32).WithMessage("Invalid Format Direction").WithName("FrmDirectionValidation");
        }
    }
}
