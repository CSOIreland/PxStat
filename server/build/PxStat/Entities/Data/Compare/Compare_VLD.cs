
using FluentValidation;

namespace PxStat.Data
{
    /// <summary>
    /// Validators for Compare functions
    /// </summary>
    internal class Compare_VLD_Read : AbstractValidator<Compare_DTO_Read>
    {
        /// <summary>
        /// Validator for Compare_DTO_Read
        /// </summary>
        public Compare_VLD_Read()
        {
            //Mandatory
            RuleFor(dto => dto.LngIsoCode)
                .Length(2)
                .WithMessage("Invalid language code")
                .WithName("languageValidation");
            //Mandatory
            RuleFor(dto => dto.RlsCode)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Invalid release code")
                .WithName("RlsCode");

        }
    }

    /// <summary>
    /// Validator for Compare_DTO_ReadPrevious
    /// </summary>
    internal class Compare_VLD_ReadPrevious : AbstractValidator<Compare_DTO_ReadPrevious>
    {
        public Compare_VLD_ReadPrevious()
        {
            RuleFor(dto => dto.RlsCode)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Invalid release code")
                .WithName("RlsCode");

        }
    }


}