using FluentValidation;

namespace PxStat.System.Navigation
{
    internal class Theme_VLD_Create : AbstractValidator<Theme_DTO_Create>
    {
        internal Theme_VLD_Create()
        {
            //Mandatory - ThmValue
            RuleFor(x => x.ThmValue).NotEmpty();
            //Optional for API users but this field will be populated by the DTO in all cases - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

    internal class Theme_VLD_Read : AbstractValidator<Theme_DTO_Read>
    {
        internal Theme_VLD_Read()
        {
            //Optional for API users but this field will be populated by the DTO in all cases - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
            //Optional - ThmValue
            RuleFor(f => f.ThmValue).Length(0, 256).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid Theme").WithName("ThmValueValidation");
        }
    }

    internal class Theme_VLD_Delete : AbstractValidator<Theme_DTO_Delete>
    {
        public Theme_VLD_Delete()
        {
            //Mandatory - SbjCode
            RuleFor(x => x.ThmCode).NotEmpty();
        }
    }

    internal class Theme_VLD_Update : AbstractValidator<Theme_DTO_Update>
    {
        internal Theme_VLD_Update()
        {
            //Mandatory - ThmCode
            RuleFor(x => x.ThmCode).NotEmpty();
            //Optional for API users but this field will be populated by the DTO in all cases - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
            //Mandatory - ThmValue
            RuleFor(f => f.ThmValue).Length(0, 256).WithMessage("Invalid Theme Value").WithName("ThmValueValidation");
        }
    }
}
