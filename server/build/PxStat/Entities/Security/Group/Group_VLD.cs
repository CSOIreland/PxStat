using API;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using System;
using System.Collections.Generic;

namespace PxStat.Security
{
    /// <summary>
    /// Validator for Group Read
    /// </summary>
    internal class Group_VLD_Read : AbstractValidator<Group_DTO_Read>
    {
        internal Group_VLD_Read()
        {
            string alphaNumericRegex = Configuration_BSO.GetStaticConfig("APP_REGEX_ALPHA_NUMERIC");
            //Optional - GrpCode
            RuleFor(f => f.GrpCode).Matches(alphaNumericRegex).NotEmpty().Length(1, 32).When(f => !string.IsNullOrEmpty(f.GrpCode)).WithMessage("Invalid Group Code");
        }
    }


    internal class Group_VLD_ReadCode : AbstractValidator<Group_DTO_Read>
    {
        internal Group_VLD_ReadCode()
        {
            string alphaNumericRegex = Configuration_BSO.GetStaticConfig("APP_REGEX_ALPHA_NUMERIC");
            //Optional - GrpCode
            RuleFor(f => f.GrpCode).Matches(alphaNumericRegex).NotEmpty().Length(1, 32).When(f => !string.IsNullOrEmpty(f.GrpCode)).WithMessage("Invalid Group Code");

        }
    }

    /// <summary>
    /// Validator for Group Create
    /// </summary>
    internal class Group_VLD_Create : AbstractValidator<Group_DTO_Create>
    {
        internal Group_VLD_Create()
        {
            string phoneRegex = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "regex.phone.pattern");
            string emailRegex = Configuration_BSO.GetStaticConfig("APP_REGEX_EMAIL");
            string alphaNumericRegex = Configuration_BSO.GetStaticConfig("APP_REGEX_ALPHA_NUMERIC");
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
            RuleFor(f => f.GrpContactEmail).Matches(emailRegex).WithMessage("Invalid Group Contact Email").WithName("GrpContactEmailValidation");
        }
    }

    /// <summary>
    /// Validator for Group Update
    /// </summary>
    internal class Group_VLD_Update : AbstractValidator<Group_DTO_Update>
    {
        internal Group_VLD_Update()
        {
            string phoneRegex = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "regex.phone.pattern");
            string emailRegex = Configuration_BSO.GetStaticConfig("APP_REGEX_EMAIL");
            string alphaNumericRegex = Configuration_BSO.GetStaticConfig("APP_REGEX_ALPHA_NUMERIC");

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
            RuleFor(f => f.GrpContactEmail).NotEmpty().Matches(emailRegex).WithMessage("Invalid Group Contact Email").WithName("GrpContactEmailValidation");
        }
    }

    /// <summary>
    /// Validator for Group Delete
    /// </summary>
    internal class Group_VLD_Delete : AbstractValidator<Group_DTO_Delete>
    {
        internal Group_VLD_Delete()
        {
            string alphaNumericRegex = Configuration_BSO.GetStaticConfig("APP_REGEX_ALPHA_NUMERIC");
            //Mandatory - GrpCode
            RuleFor(f => f.GrpCode).Matches(alphaNumericRegex).NotEmpty().Length(1, 32).WithMessage("Invalid Group Code").WithName("GroupCodeValidation");
        }
    }




    internal static class CustomValidations
    {

        internal static bool CheckGroupExists(dynamic dto)
        {
            using (IADO ado = AppServicesHelper.StaticADO)
            {
                try
                {
                    if (dto.GrpCode == null) return false;
                    Group_ADO gAdo = new Group_ADO();
                    return gAdo.Exists(ado, dto.GrpCode);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
