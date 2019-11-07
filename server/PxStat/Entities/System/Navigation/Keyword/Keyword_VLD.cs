using FluentValidation;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Validate for Keyword search
    /// </summary>
    internal class Keyword_VLD_Search : AbstractValidator<Keyword_DTO_Search>
    {
        internal Keyword_VLD_Search()
        {
            //Mandatory - LngIsoCode
            RuleFor(f => f.LngIsoCode).NotEmpty().Length(2).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }
}