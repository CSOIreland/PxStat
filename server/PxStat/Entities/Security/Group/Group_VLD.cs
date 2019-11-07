using API;
using FluentValidation;

namespace PxStat.Security
{
    /// <summary>
    /// Validator for Group Read
    /// </summary>
    internal class Group_VLD_Read : AbstractValidator<Group_DTO_Read>
    {
        internal Group_VLD_Read()
        {
            string alphaNumericRegex = Utility.GetCustomConfig("APP_REGEX_ALPHA_NUMERIC");
            //Optional - GrpCode
            RuleFor(f => f.GrpCode).Matches(alphaNumericRegex).NotEmpty().Length(1, 32).When(f => !string.IsNullOrEmpty(f.GrpCode)).WithMessage("Invalid Group Code").WithName("GroupCodeValidation");
        }
    }


    internal class Group_VLD_ReadCode : AbstractValidator<Group_DTO_Read>
    {
        internal Group_VLD_ReadCode()
        {
            string alphaNumericRegex = Utility.GetCustomConfig("APP_REGEX_ALPHA_NUMERIC");
            //Optional - GrpCode
            RuleFor(f => f.GrpCode).Matches(alphaNumericRegex).NotEmpty().Length(1, 32).When(f => !string.IsNullOrEmpty(f.GrpCode)).WithMessage("Invalid Group Code").WithName("GroupCodeValidation");

        }
    }

    /// <summary>
    /// Validator for Group Create
    /// </summary>
    internal class Group_VLD_Create : AbstractValidator<Group_DTO_Create>
    {
        internal Group_VLD_Create()
        {
            string phoneRegex = Utility.GetCustomConfig("APP_REGEX_PHONE");
            string emailRegex = Utility.GetCustomConfig("APP_REGEX_EMAIL");
            string alphaNumericRegex = Utility.GetCustomConfig("APP_REGEX_ALPHA_NUMERIC");
            //Mandatory - GrpCode
            RuleFor(f => f.GrpCode).NotEmpty().Length(1, 32).WithMessage("Invalid Group Code").WithName("GroupCodeValidation");
            RuleFor(f => f.GrpCode).Matches(alphaNumericRegex).WithMessage("Invalid Group Code").WithName("GroupCodeValidationAlphaNumeric");
            //Mandatory - GrpName
            RuleFor(f => f.GrpName).NotEmpty().Length(1, 256).WithMessage("Invalid Group Name").WithName("GroupNameValidation");
            //Optional - GrpContactName
            RuleFor(f => f.GrpContactName).Length(1, 256).When(f => !string.IsNullOrEmpty(f.GrpContactName)).WithMessage("Invalid Group Contact Name").WithName("GroupContactNameValidation");
            //Optional - GrpContactPhone
            RuleFor(f => f.GrpContactPhone).Matches(phoneRegex).When(f => !string.IsNullOrEmpty(f.GrpContactPhone)).WithMessage("Invalid Group Contact Phone Number").WithName("GrpContactPhoneValidation");
            //Optional - GrpContactEmail
            RuleFor(f => f.GrpContactEmail).Matches(emailRegex).When(f => !string.IsNullOrEmpty(f.GrpContactEmail)).WithMessage("Invalid Group Contact Email").WithName("GrpContactEmailValidation");
        }
    }

    /// <summary>
    /// Validator for Group Update
    /// </summary>
    internal class Group_VLD_Update : AbstractValidator<Group_DTO_Update>
    {
        internal Group_VLD_Update()
        {
            string phoneRegex = Utility.GetCustomConfig("APP_REGEX_PHONE");
            string emailRegex = Utility.GetCustomConfig("APP_REGEX_EMAIL");
            string alphaNumericRegex = Utility.GetCustomConfig("APP_REGEX_ALPHA_NUMERIC");

            //Mandatory - GrpCodeOld
            RuleFor(f => f.GrpCodeOld).NotEmpty().Length(1, 32).WithMessage("Invalid Group Code").WithName("GroupCodeValidation");
            RuleFor(f => f.GrpCodeOld).Matches(alphaNumericRegex).WithMessage("Invalid Group Code").WithName("GroupCodeOldValidationAlphaNumeric");
            //Mandatory - GrpCodeNew
            RuleFor(f => f.GrpCodeNew).NotEmpty().Length(1, 32).WithMessage("Invalid Group Code").WithName("GroupCodeValidation");
            RuleFor(f => f.GrpCodeNew).Matches(alphaNumericRegex).WithMessage("Invalid Group Code").WithName("GroupCodeNewValidationAlphaNumeric");

            //Mandatory - GrpName
            RuleFor(f => f.GrpName).NotEmpty().Length(1, 256).WithMessage("Invalid Group Name").WithName("GroupNameValidation");
            //Optional - GrpContactName
            RuleFor(f => f.GrpContactName).Length(1, 256).When(f => !string.IsNullOrEmpty(f.GrpContactName)).WithMessage("Invalid Group Contact Name").WithName("GroupContactNameValidation");
            //Optional - GrpContactPhone
            RuleFor(f => f.GrpContactPhone).Matches(phoneRegex).When(f => !string.IsNullOrEmpty(f.GrpContactPhone)).WithMessage("Invalid Group Contact Phone Number").WithName("GrpContactPhoneValidation");
            //Optional - GrpContactEmail
            RuleFor(f => f.GrpContactEmail).Matches(emailRegex).When(f => !string.IsNullOrEmpty(f.GrpContactEmail)).WithMessage("Invalid Group Contact Email").WithName("GrpContactEmailValidation");
        }
    }

    /// <summary>
    /// Validator for Group Delete
    /// </summary>
    internal class Group_VLD_Delete : AbstractValidator<Group_DTO_Delete>
    {
        internal Group_VLD_Delete()
        {
            string alphaNumericRegex = Utility.GetCustomConfig("APP_REGEX_ALPHA_NUMERIC");
            //Mandatory - GrpCode
            RuleFor(f => f.GrpCode).Matches(alphaNumericRegex).NotEmpty().Length(1, 32).WithMessage("Invalid Group Code").WithName("GroupCodeValidation");
        }
    }
}