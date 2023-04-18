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

    internal class Format_VLD_Read_Object : AbstractValidator<Format_DTO_Read>
    {
        internal Format_VLD_Read_Object()
        {
            //Mandatory - FrmType
            RuleFor(f => f.FrmType).NotNull().NotEmpty().WithMessage("Invalid Format Type").WithName("FrmTypeValidation");
            //Mandatory - FrmVersion
            RuleFor(f => f.FrmVersion).NotNull().NotEmpty().WithMessage("Invalid Format Version").WithName("FrmVersionValidation");
            //Mandatory - FrmMimetype
            RuleFor(f => f.FrmMimetype).NotNull().NotEmpty().WithMessage("Invalid Mimetype").WithName("MimetypeValidation");
        }

    }
}
