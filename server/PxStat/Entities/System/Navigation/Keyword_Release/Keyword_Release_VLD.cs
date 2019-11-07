using API;
using FluentValidation;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Validator for Keyword Release Create
    /// </summary>
    internal class Keyword_Release_VLD_Create : AbstractValidator<Keyword_Release_DTO>
    {

        internal Keyword_Release_VLD_Create()
        {
            string regexNoWhiteSpace = Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE");
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty();
            //Mandatory - KrlValue
            RuleFor(f => f.KrlValue).NotEmpty();
            //No white spaces allowed in keywords
            RuleFor(x => x.KrlValue).Matches(regexNoWhiteSpace).WithMessage("No white spaces allowed").WithName("UrlValidation");
            //Optional LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code");
        }

    }

    /// <summary>
    /// Validator for Keyword Release Read
    /// </summary>
    internal class Keyword_Release_VLD_Read : AbstractValidator<Keyword_Release_DTO>
    {
        internal Keyword_Release_VLD_Read()
        {
            //Optional - RlsCode
            RuleFor(f => f.RlsCode).GreaterThan(0).When(f => f.RlsCode != default(int)).WithMessage("Invalid Release Code").WithName("InvalidRlsCode");
            //Optional  - KrlCode
            RuleFor(f => f.KrlCode).GreaterThan(0).When(f => f.KrlCode != default(int)).WithMessage("Invalid Keyword Release Code").WithName("InvalidKrlCode");
        }
    }

    /// <summary>
    /// Validator for Keyword Release Delete
    /// </summary>
    internal class Keyword_Release_VLD_Delete : AbstractValidator<Keyword_Release_DTO>
    {
        internal Keyword_Release_VLD_Delete()
        {
            //Mandatory - KrlCode
            RuleFor(f => f.KrlCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator for Keyword Release Update
    /// </summary>
    internal class Keyword_Release_VLD_Update : AbstractValidator<Keyword_Release_DTO>
    {
        internal Keyword_Release_VLD_Update()
        {
            //Mandatory - KrlCode
            RuleFor(f => f.KrlCode).NotEmpty();
            //Mandatory - KrlValue
            RuleFor(f => f.KrlValue).NotEmpty();
            //Optional LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code");

        }
    }
}
