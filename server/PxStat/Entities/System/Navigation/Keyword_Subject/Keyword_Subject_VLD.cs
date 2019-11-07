
using API;
using FluentValidation;
using PxStat.Resources;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Validator for keyword subject create
    /// </summary>
    internal class Keyword_Subject_VLD_Create : AbstractValidator<Keyword_Subject_DTO>
    {
        public Keyword_Subject_VLD_Create()
        {
            string regexNoWhiteSpace = Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE");
            //Mandatory- KsbValue
            RuleFor(x => x.KsbValue).NotEmpty();
            //Mandatory - SbjCode
            RuleFor(x => x.SbjCode).NotEmpty();
            //No white spaces allowed in keywords
            RuleFor(x => x.KsbValue).Matches(regexNoWhiteSpace).WithMessage("No white spaces allowed").WithName("UrlValidation");
            //Optional - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code");
        }
    }

    /// <summary>
    /// Validator for Keyword subject update
    /// </summary>
    internal class Keyword_Subject_VLD_Update : AbstractValidator<Keyword_Subject_DTO>
    {
        /// <summary>
        /// 
        /// </summary>
        public Keyword_Subject_VLD_Update()
        {
            //Mandatory- KsbValue
            RuleFor(x => x.KsbValue).NotEmpty();
            //Mandatory - KsbCode
            RuleFor(x => x.KsbCode).NotEmpty();
            //Optional - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code");
        }
    }

    /// <summary>
    /// Validator for Keyword subject Delete
    /// </summary>
    internal class Keyword_Subject_VLD_Delete : AbstractValidator<Keyword_Subject_DTO>
    {
        public Keyword_Subject_VLD_Delete()
        {
            //Mandatory - KsbCode
            RuleFor(x => x.KsbCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator for Keyword Subject Read
    /// </summary>
    internal class Keyword_Subject_VLD_Read : AbstractValidator<Keyword_Subject_DTO>
    {

        public Keyword_Subject_VLD_Read()
        {
            //Optional - SbjCode
            RuleFor(x => x.SbjCode).IsOptional();
        }
    }

}
