using API;
using FluentValidation;
using PxStat.Security;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Validator for Reason Read
    /// </summary>
    internal class Reason_VLD_Read : AbstractValidator<Reason_DTO_Read>
    {
        internal Reason_VLD_Read()
        {
            //Optional - RsnCode
            RuleFor(f => f.RsnCode).NotEmpty().Length(1, 32).When(f => !string.IsNullOrEmpty(f.RsnCode)).WithMessage("Invalid Rsn code").WithName("ReasonCodeValidation");
            //Optional - RlsCode
            RuleFor(f => f.RlsCode).GreaterThan(0).When(f => f.RlsCode != default(int)).WithMessage("Invalid Release Code").WithName("InvalidRlsCode");
        }
    }

    /// <summary>
    /// Validator for Reason Create
    /// </summary>
    internal class Reason_VLD_Create : AbstractValidator<Reason_DTO_Create>
    {
        internal Reason_VLD_Create()
        {
            string alphaNumericRegex = Configuration_BSO.GetStaticConfig("APP_REGEX_ALPHA_NUMERIC");
            //Mandatory - RsnCode
            RuleFor(f => f.RsnCode).NotEmpty().Length(1, 32).WithMessage("Invalid Rsn code").WithName("ReasonCodeValidation");
            RuleFor(f => f.RsnCode).Matches(alphaNumericRegex).WithMessage("Invalid Rsn code").WithName("ReasonCodeValidationAlphaNumeric");

            //Mandatory - RsnValueExternal
            RuleFor(f => f.RsnValueExternal).NotEmpty().Length(1, 256).WithMessage("Invalid RsnValueExternal").WithName("RsnValueExternalValidation");
            //Mandatory - RsnValueInternal
            RuleFor(f => f.RsnValueInternal).NotEmpty().Length(1, 256).WithMessage("Invalid RsnValueInternal").WithName("RsnValueInternalValidation");
        }
    }

    /// <summary>
    /// Validator for Reason Update
    /// </summary>
    internal class Reason_VLD_Update : AbstractValidator<Reason_DTO_Update>
    {
        internal Reason_VLD_Update()
        {
            //Mandatory - RsnCode
            RuleFor(f => f.RsnCode).NotEmpty().Length(1, 32).WithMessage("Invalid Rsn code").WithName("ReasonCodeValidation");
            //Mandatory - RsnValueExternal
            RuleFor(f => f.RsnValueExternal).NotEmpty().Length(1, 256).WithMessage("Invalid RsnValueExternal").WithName("RsnValueExternalValidation");
            //Mandatory - RsnValueInternal
            RuleFor(f => f.RsnValueInternal).NotEmpty().Length(1, 256).WithMessage("Invalid RsnValueInternal").WithName("RsnValueInternalValidation");
        }
    }

    /// <summary>
    /// Validator for Reason Delete
    /// </summary>
    internal class Reason_VLD_Delete : AbstractValidator<Reason_DTO_Delete>
    {
        internal Reason_VLD_Delete()
        {
            //Mandatory - RsnCode
            RuleFor(f => f.RsnCode).NotEmpty().Length(1, 32).WithMessage("Invalid Rsn code").WithName("ReasonCodeValidation");
        }
    }
}
