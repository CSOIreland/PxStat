using API;
using FluentValidation;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Validator for Copyright Read
    /// </summary>
    internal class Copyright_VLD_Read : AbstractValidator<Copyright_DTO_Read>
    {
        internal Copyright_VLD_Read()
        {
            //Optional - CprCode
            RuleFor(f => f.CprCode).NotEmpty().When(f => !string.IsNullOrEmpty(f.CprCode)).Length(1, 32).WithMessage("Invalid CprCode").WithName("CprCodeValidation");
        }
    }

    /// <summary>
    /// Validator for Copyright Create
    /// </summary>
    internal class Copyright_VLD_Create : AbstractValidator<Copyright_DTO_Create>
    {
        internal Copyright_VLD_Create()
        {
            string urlRegex = Utility.GetCustomConfig("APP_REGEX_URL");
            string alphaNumericRegex = Utility.GetCustomConfig("APP_REGEX_ALPHA_NUMERIC");

            //Mandatory - CprCode
            RuleFor(f => f.CprCode).NotEmpty().Length(1, 32).WithMessage("Invalid CprCode").WithName("CprCodeValidation");
            RuleFor(f => f.CprCode).Matches(alphaNumericRegex).WithMessage("Invalid CprCode").WithName("CprCodeValidationAlphaOnly");
            //Mandatory - CprValue
            RuleFor(f => f.CprValue).NotEmpty().Length(1, 256).WithMessage("Invalid CprValue").WithName("CprValueValidation");
            //Mandatory - CprUrl
            RuleFor(f => f.CprUrl).NotEmpty().Length(1, 2048).WithMessage("Invalid CprUrl").WithName("UrlValidation");
            RuleFor(f => f.CprUrl).Matches(urlRegex).WithMessage("Invalid url").WithName("UrlValidation");
        }
    }

    /// <summary>
    /// Validator for Copyright Update
    /// </summary>
    internal class Copyright_VLD_Update : AbstractValidator<Copyright_DTO_Update>
    {
        internal Copyright_VLD_Update()
        {
            string urlRegex = Utility.GetCustomConfig("APP_REGEX_URL");
            string alphaNumericRegex = Utility.GetCustomConfig("APP_REGEX_ALPHA_NUMERIC");
            //Mandatory - CprCodeOld
            RuleFor(f => f.CprCodeOld).NotEmpty().Length(1, 32).WithMessage("Invalid CprCodeOld").WithName("CprCodeOldValidation");
            RuleFor(f => f.CprCodeOld).Matches(alphaNumericRegex).WithMessage("Invalid CprcodeOld").WithName("CprCodeOldValidationAlphaNumeric");
            //Mandatory - CprCodeNew
            RuleFor(f => f.CprCodeNew).NotEmpty().Length(1, 32).WithMessage("Invalid CprCodeNew").WithName("CprCodeNewValidation");
            RuleFor(f => f.CprCodeNew).Matches(alphaNumericRegex).WithMessage("Invalid CprcodeNew").WithName("CprCodeNewValidationAlphaNumeric");
            //Mandatory - CprValue
            RuleFor(f => f.CprValue).NotEmpty().Length(1, 256).WithMessage("Invalid CprValue").WithName("CprValueValidation");
            //Mandatory - CprUrl
            RuleFor(f => f.CprUrl).NotEmpty().Length(1, 2048).WithMessage("Invalid CprUrl").WithName("UrlValidation");
            RuleFor(f => f.CprUrl).Matches(urlRegex).WithMessage("Invalid url").WithName("UrlValidation");
        }
    }

    /// <summary>
    /// Validator for Copyright Delete
    /// </summary>
    internal class Copyright_VLD_Delete : AbstractValidator<Copyright_DTO_Delete>
    {
        internal Copyright_VLD_Delete()
        {
            //Mandatory - CprCode
            RuleFor(f => f.CprCode).NotEmpty().Length(1, 32).WithMessage("Invalid CprCode").WithName("CprCodeValidation");
        }
    }
}