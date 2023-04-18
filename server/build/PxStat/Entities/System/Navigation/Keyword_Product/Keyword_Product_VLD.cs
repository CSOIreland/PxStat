using API;
using FluentValidation;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Validator for Keyword Product Create
    /// </summary>
    internal class Keyword_Product_VLD_Create : AbstractValidator<Keyword_Product_DTO>
    {
        public Keyword_Product_VLD_Create()
        {
            string regexNoWhiteSpace = Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE");
            //Mandatory - KprValue
            RuleFor(x => x.KprValue).NotEmpty();
            //Mandatory - PrcCode
            RuleFor(x => x.PrcCode).NotEmpty();
            //No white spaces allowed in keywords
            RuleFor(x => x.KprValue).Matches(regexNoWhiteSpace).WithMessage("No white spaces allowed").WithName("UrlValidation");
            //Optional - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code");
        }

    }

    /// <summary>
    /// Validator for Keyword Product Update
    /// </summary>
    internal class Keyword_Product_VLD_Update : AbstractValidator<Keyword_Product_DTO>
    {
        public Keyword_Product_VLD_Update()
        {
            //Mandatory - KprValue 
            RuleFor(x => x.KprValue).NotEmpty();
            //Mandatory - KprCode
            RuleFor(x => x.KprCode).NotEmpty();
            //Optional - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code");
        }
    }

    /// <summary>
    /// Validator for Keyword Product Delete
    /// </summary>
    internal class Keyword_Product_VLD_Delete : AbstractValidator<Keyword_Product_DTO>
    {
        public Keyword_Product_VLD_Delete()
        {
            //Mandatory - KprCode
            RuleFor(x => x.KprCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator for Keyword Product Read
    /// </summary>
    internal class Keyword_Product_VLD_Read : AbstractValidator<Keyword_Product_DTO>
    {
        public Keyword_Product_VLD_Read()
        {
            //Optional - Sbjcode
            RuleFor(f => f.SbjCode).GreaterThan(0).When(f => f.SbjCode != default(int));
            //Optional - PrcCode
            RuleFor(f => f.PrcCode).Length(1, 32).When(f => !string.IsNullOrEmpty(f.PrcCode)).WithMessage("Invalid Product code").WithName("ProductCodeValidation");
            //Optional - KprCode
            RuleFor(f => f.KprCode).GreaterThan(0).When(f => f.KprCode != default(int));

        }
    }
}
