using FluentValidation;

namespace PxStat.Security
{
    /// <summary>
    /// Validator for Account Read
    /// </summary>
    internal class Account_VLD_Read : AbstractValidator<Account_DTO_Read>
    {
        internal Account_VLD_Read()
        {
            //Optional - CcnUsername
            RuleFor(f => f.CcnUsername).NotEmpty().Length(1, 256).When(f => !string.IsNullOrEmpty(f.CcnUsername)).WithMessage("Invalid Username").WithName("CcnUsernameValidation");
        }
    }

    /// <summary>
    /// Validator for Account ReadCurrent
    /// </summary>
    internal class Account_VLD_ReadCurrent : AbstractValidator<Account_DTO_Read>
    {
        internal Account_VLD_ReadCurrent() { }
    }

    /// <summary>
    /// Validator for Account Create
    /// </summary>
    internal class Account_VLD_Create : AbstractValidator<Account_DTO_Create>
    {
        internal Account_VLD_Create()
        {
            //Mandatory - CcnUsername
            RuleFor(f => f.CcnUsername).NotEmpty().Length(1, 256).WithMessage("Invalid Username").WithName("CcnUsernameValidation");
            //Mandatory - PrvCode
            RuleFor(f => f.PrvCode).NotEmpty().Length(1, 32).WithMessage("Invalid PrvCode").WithName("AccountPrivilegeCodeValidation");
        }
    }

    /// <summary>
    /// Validator for Account Delete
    /// </summary>
    internal class Account_VLD_Delete : AbstractValidator<Account_DTO_Delete>
    {
        internal Account_VLD_Delete()
        {
            //Mandatory - CcnUsername
            RuleFor(f => f.CcnUsername).NotEmpty().Length(1, 256).WithMessage("Invalid Username").WithName("CcnUsernameValidation");
        }
    }

    /// <summary>
    /// Validator for Account Update
    /// </summary>
    internal class Account_VLD_Update : AbstractValidator<Account_DTO_Update>
    {
        internal Account_VLD_Update()
        {
            //Mandatory - CcnUsername
            RuleFor(f => f.CcnUsername).NotEmpty().Length(1, 256).WithMessage("Invalid Username").WithName("CcnUsernameValidation");
            //Mandatory - PrvCode
            RuleFor(f => f.PrvCode).NotEmpty().Length(1, 32).WithMessage("Invalid PrvCode").WithName("AccountPrivilegeCodeValidation");
        }
    }

    internal class Account_VLD_UpdateCurrent : AbstractValidator<Account_DTO_Update>
    {
        internal Account_VLD_UpdateCurrent()
        {

        }
    }


    /// <summary>
    /// Validagte for Account ReadIsApprover
    /// </summary>
    internal class Account_VLD_ReadIsApprover : AbstractValidator<Account_DTO_ReadIsApprover>
    {
        internal Account_VLD_ReadIsApprover()
        {
            //Optional - CcnUsername - However, if CcnUsername is not supplied then the current user will be inserted into this value
            RuleFor(f => f.CcnUsername).NotEmpty().Length(1, 256).When(f => !string.IsNullOrEmpty(f.CcnUsername)).WithMessage("Invalid Username").WithName("CcnUsernameValidation");
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty();
        }
    }
}