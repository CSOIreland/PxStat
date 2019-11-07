
using FluentValidation;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Validator for Subject Create
    /// </summary>
    internal class Subject_VLD_Create : AbstractValidator<Subject_DTO>
    {
        public Subject_VLD_Create()
        {
            //Mandatory - SbjValue
            RuleFor(x => x.SbjValue).NotEmpty();
            //Optional - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode));
        }
    }

    /// <summary>
    /// Validator for Subject Update
    /// </summary>
    internal class Subject_VLD_Update : AbstractValidator<Subject_DTO>
    {
        public Subject_VLD_Update()
        {
            //Mandatory - SbjCode
            RuleFor(x => x.SbjCode).NotEmpty();
            //Mandatory - SbjValue
            RuleFor(x => x.SbjValue).NotEmpty();
            //Optional for API users but this field will be populated by the DTO in all cases - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

    /// <summary>
    /// Validator for Subject Delete
    /// </summary>
    internal class Subject_VLD_Delete : AbstractValidator<Subject_DTO>
    {
        public Subject_VLD_Delete()
        {
            //Mandatory - SbjCode
            RuleFor(x => x.SbjCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator for Subject Read
    /// </summary>
    internal class Subject_VLD_Read : AbstractValidator<Subject_DTO>
    {
        public Subject_VLD_Read()
        {
            //Optional - SbjCode
            RuleFor(f => f.SbjCode).GreaterThan(0).When(f => f.SbjCode != default(int));
            //Optional for API users but this field will be populated by the DTO in all cases - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }
}