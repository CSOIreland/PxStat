using FluentValidation;


namespace PxStat.System.Settings
{
    /// <summary>
    /// Validator for Language Read
    /// </summary>
    internal class Language_VLD_Read : AbstractValidator<Language_DTO_Read>
    {

        internal Language_VLD_Read()
        {
            //Optional - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

    /// <summary>
    /// Validator for Language ReadListByRlease
    /// </summary>
    internal class Language_VLD_ReadListByRlease : AbstractValidator<Language_DTO_ReadList>
    {
        internal Language_VLD_ReadListByRlease()
        {
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator for Language Create
    /// </summary>
    internal class Language_VLD_Create : AbstractValidator<Language_DTO_Create>
    {
        internal Language_VLD_Create()
        {
            //Mandatory - LngIsoCode
            //Lower case letters only on the LngIso Code parameter. Must be two characters exactly. 
            RuleFor(f => f.LngIsoCode).NotEmpty().Matches("^[a-z]{2}$").WithMessage("Invalid ISO code").WithName("LngIsoCodeValidation");
            //Mandatory - LngIsoName
            RuleFor(f => f.LngIsoName).NotEmpty().Length(1, 32).WithMessage("Invalid ISO name").WithName("LanguageIsoNameValidation");
        }
    }

    /// <summary>
    /// Validator for Language Update
    /// </summary>
    internal class Language_VLD_Update : AbstractValidator<Language_DTO_Update>
    {
        internal Language_VLD_Update()
        {
            //Mandatory - LngIsoCode
            RuleFor(f => f.LngIsoCode).NotEmpty().Matches("^[a-z]{2}$").WithMessage("Invalid ISO code").WithName("LngIsoCodeValidation");
            //Mandatory - LngIsoName
            RuleFor(f => f.LngIsoName).Length(1, 32).NotEmpty().WithMessage("Invalid ISO name").WithName("LngIsoNameValidation");
        }
    }

    /// <summary>
    /// Validator for Language Delete
    /// </summary>
    internal class Language_VLD_Delete : AbstractValidator<Language_DTO_Delete>
    {
        internal Language_VLD_Delete()
        {
            //Mandatory - LngIsoCode
            RuleFor(f => f.LngIsoCode).NotEmpty().Length(2).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }
}