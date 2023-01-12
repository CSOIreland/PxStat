using FluentValidation;

namespace PxStat.Data
{
    /// <summary>
    /// Validator for Group Account Read
    /// </summary>
    internal class ReleaseProductAssociation_VLD_Read : AbstractValidator<ReleaseProduct_DTO_Read>
    {
        internal ReleaseProductAssociation_VLD_Read()
        {
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).GreaterThan(0).WithMessage("Invalid RlsCode").WithName("ReleaseCodeValidation");
        }
    }

    /// <summary>
    /// Validator for Group Account Create
    /// </summary>
    internal class ReleaseProductAssociation_VLD_Create : AbstractValidator<ReleaseProduct_DTO_Create>
    {
        internal ReleaseProductAssociation_VLD_Create()
        {
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).GreaterThan(0).WithMessage("Invalid RlsCode").WithName("ReleaseCodeValidation");
            //Mandatory - PrcCode
            RuleFor(f => f.PrcCode).NotEmpty().Length(1, 256).WithMessage("Invalid PrcCode").WithName("ProductCodeValidation");
        }
    }

    /// <summary>
    /// Validator for Group Account Delete
    /// </summary>
    internal class ReleaseProductAssociation_VLD_Delete : AbstractValidator<ReleaseProductAssociation_DTO_Delete>
    {
        internal ReleaseProductAssociation_VLD_Delete()
        {
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).GreaterThan(0).WithMessage("Invalid RlsCode").WithName("ReleaseCodeValidation");
            //Mandatory - PrcCode
            RuleFor(f => f.PrcCode).NotEmpty().Length(1, 256).WithMessage("Invalid PrcCode").WithName("ProductCodeValidation");
        }
    }
}