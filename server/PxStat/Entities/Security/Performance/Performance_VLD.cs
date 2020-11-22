using FluentValidation;
using System;

namespace PxStat.Security
{
    /// <summary>
    /// Validator for performance read
    /// </summary>
    internal class Performance_VLD_Read : AbstractValidator<Performance_DTO_Read>
    {
        internal Performance_VLD_Read()
        {
            //Mandatory - DateFrom
            RuleFor(x => x.PrfDatetimeStart).NotEqual(default(DateTime)).NotEmpty().WithMessage("*Required");
            //Mandatory - DateTo
            RuleFor(x => x.PrfDatetimeEnd).NotEqual(default(DateTime)).NotEmpty().WithMessage("*Required");

            //Enddate mustbe past startdate
            RuleFor(x => x.PrfDatetimeEnd).GreaterThanOrEqualTo(x => x.PrfDatetimeStart).WithMessage("End date must be the same or after start date.");

            //Optional for API users but this field will be populated by the DTO in all cases - LngIsoCode
            RuleFor(x => x.LngIsoCode.Length).Equal(2).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }

    }
    internal class Performance_VLD_Delete : AbstractValidator<Performance_DTO_Delete>
    {
        internal Performance_VLD_Delete()
        {
            //No Check for delete once autorised will accept task
            //Optional for API users but this field will be populated by the DTO in all cases - LngIsoCode
            RuleFor(x => x.LngIsoCode.Length).Equal(2).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

}
