
using API;
using FluentValidation;
using PxStat.Security;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Validator for Product Create
    /// </summary>
    internal class Product_VLD_Create : AbstractValidator<Product_DTO>
    {
        public Product_VLD_Create()
        {
            //Mandatory - PrcValue
            RuleFor(x => x.PrcValue).NotEmpty();
            //Mandatory - SbjCode
            RuleFor(x => x.SbjCode).NotEmpty();
            //Mandatory - ProductCode
            RuleFor(x => x.PrcCode).NotEmpty();

            string productCodeRegex = (string) Configuration_BSO.GetCustomConfig(ConfigType.global, "regex.product-code");
            RuleFor(x => x.PrcCode).Matches(productCodeRegex);
            RuleFor(x => x.PrcCodeNew).Matches(productCodeRegex);
        }
    }

    /// <summary>
    /// Validator for Product Update
    /// </summary>
    internal class Product_VLD_Update : AbstractValidator<Product_DTO>
    {
        public Product_VLD_Update()
        {
            //Mandatory - PrcCode
            RuleFor(x => x.PrcCode).NotEmpty();
            //Mandatory - SbjCode
            RuleFor(x => x.SbjCode).NotEmpty();
            //Mandatory - PrcValue
            RuleFor(x => x.PrcValue).NotEmpty();
            //Optional for API users but this field will be populated by the DTO in all cases - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");

            string productCodeRegex = (string)Configuration_BSO.GetCustomConfig(ConfigType.global, "regex.product-code");
            RuleFor(x => x.PrcCode).Matches(productCodeRegex);
            RuleFor(x => x.PrcCodeNew).Matches(productCodeRegex);
        }
    }

    /// <summary>
    /// Validator for Product Delete
    /// </summary>
    internal class Product_VLD_Delete : AbstractValidator<Product_DTO>
    {
        public Product_VLD_Delete()
        {
            //Mandatory - PrcCode
            RuleFor(x => x.PrcCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator for Product Read
    /// </summary>
    internal class Product_VLD_Read : AbstractValidator<Product_DTO>
    {
        public Product_VLD_Read()
        {
            //Optional - SbjCode
            RuleFor(f => f.SbjCode).GreaterThan(0).When(f => f.SbjCode != default(int));
            //Optional - PrcCode
            RuleFor(f => f.PrcCode).Length(0, 32).When(f => !string.IsNullOrEmpty(f.PrcCode)).WithMessage("Invalid Product code").WithName("ProductCodeValidation");
            //Optional for API users but this field will be populated by the DTO in all cases - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }
}