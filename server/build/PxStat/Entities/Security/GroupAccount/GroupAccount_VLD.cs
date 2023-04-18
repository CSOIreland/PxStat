using FluentValidation;

namespace PxStat.Security
{
    /// <summary>
    /// Validator for Group Account Read
    /// </summary>
    internal class GroupAccount_VLD_Read : AbstractValidator<GroupAccount_DTO_Read>
    {
        internal GroupAccount_VLD_Read()
        {
            //Optional - GrpCode
            RuleFor(f => f.GrpCode).NotEmpty().Length(1, 32).When(f => !string.IsNullOrEmpty(f.GrpCode)).WithMessage("Invalid Group Code").WithName("GroupCodeValidation");
            //Optional - CcnUsername
            RuleFor(f => f.CcnUsername).NotEmpty().Length(1, 256).When(f => !string.IsNullOrEmpty(f.CcnUsername)).WithMessage("Invalid Username").WithName("CcnUsernameValidation");
        }
    }

    /// <summary>
    /// Validator for Group Account Create
    /// </summary>
    internal class GroupAccount_VLD_Create : AbstractValidator<GroupAccount_DTO_Create>
    {
        internal GroupAccount_VLD_Create()
        {
            //Mandatory - GrpCode
            RuleFor(f => f.GrpCode).NotEmpty().Length(1, 32).WithMessage("Invalid Group Code").WithName("GroupCodeValidation");
            //Mandatory - CcnUsername
            RuleFor(f => f.CcnUsername).NotEmpty().Length(1, 256).WithMessage("Invalid Username").WithName("CcnUsernameValidation");
            //Mandatory - GccApproveFlag
            RuleFor(f => f.GccApproveFlag).NotNull();
        }
    }

    /// <summary>
    /// Validator for Group Account Update
    /// </summary>
    internal class GroupAccount_VLD_Update : AbstractValidator<GroupAccount_DTO_Update>
    {
        internal GroupAccount_VLD_Update()
        {
            //Mandatory - GrpCode
            RuleFor(f => f.GrpCode).NotEmpty().Length(1, 32).WithMessage("Invalid Group Code").WithName("GroupCodeValidation");
            //Mandatory - CcnUsername
            RuleFor(f => f.CcnUsername).NotEmpty().Length(1, 256).WithMessage("Invalid Username").WithName("CcnUsernameValidation");
            //Mandatory - GccApproveFlag
            RuleFor(f => f.GccApproveFlag).NotNull();
        }
    }

    /// <summary>
    /// Validator for Group Account Delete
    /// </summary>
    internal class GroupAccount_VLD_Delete : AbstractValidator<GroupAccount_DTO_Delete>
    {
        internal GroupAccount_VLD_Delete()
        {
            //Mandatory - GrpCode
            RuleFor(f => f.GrpCode).NotEmpty().Length(1, 32).WithMessage("Invalid Group Code").WithName("GroupCodeValidation");
            //Mandatory - CcnUsername
            RuleFor(f => f.CcnUsername).NotEmpty().Length(1, 256).WithMessage("Invalid Username").WithName("CcnUsernameValidation");
        }
    }
}