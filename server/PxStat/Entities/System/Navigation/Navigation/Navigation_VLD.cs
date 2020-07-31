using FluentValidation;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Validator for Navigation Read
    /// </summary>
    internal class Navigation_VLD_Read : AbstractValidator<Navigation_DTO_Read>
    {
        internal Navigation_VLD_Read()
        {
            //Optional for API users but this field will be populated by the DTO in all cases - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

    /// <summary>
    /// Validator for Navigation Search
    /// </summary>
    internal class Navigation_VLD_Search : AbstractValidator<Navigation_DTO_Search>
    {
        /// <summary>
        /// Validation for Navigation Search
        /// </summary>
        internal Navigation_VLD_Search()
        {
            //Optional - LngIsoCode - Note: If this is not supplied as a parameter, the default Language Iso code will be assigned to this property
            RuleFor(f => f.LngIsoCode.Length).Equal(2).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
            //Optional - MtrCode
            RuleFor(f => f.MtrCode).Length(1, 20).When(f => !string.IsNullOrEmpty(f.MtrCode)).WithMessage("Invalid MtrCode");
            //Optional - MtrOfficialFlag
            //Optional - SbjCode
            //Optional - PrcCode
            //Optional - CprCode
            RuleFor(f => f.CprCode).Length(1, 32).WithMessage("Invalid CprCode");
            //Optional - RlsExceptionalFlag
            //Optional - RlsReservationFlag
            //Optional - RlsArchiveFlag
            //Optional - RlsAnalyticalFlag
            //Optional - Search
            RuleFor(f => f.Search).Length(1, 1024).When(f => !string.IsNullOrEmpty(f.MtrCode)).WithMessage("Invalid Search term");

            //RuleFor(f => f.Search).NotNull().WithMessage("Invalid Search term");
        }
    }
}
