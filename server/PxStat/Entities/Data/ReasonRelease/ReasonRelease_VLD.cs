using FluentValidation;

namespace PxStat.Data
{
    /// <summary>
    /// Validator for ReasonRelease_DTO_Read
    /// </summary>
    internal class ReasonRelease_VLD_Read : AbstractValidator<ReasonRelease_DTO_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal ReasonRelease_VLD_Read()
        {
            //Optional - RsnCode
            RuleFor(f => f.RsnCode).Length(1, 32).When(f => !string.IsNullOrEmpty(f.RsnCode)).WithMessage("Invalid Rsn code").WithName("RsnCodeValidation");
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty();
        }

    }

    /// <summary>
    /// Validator for ReasonRelease_DTO_Create
    /// </summary>
    internal class ReasonRelease_VLD_Create : AbstractValidator<ReasonRelease_DTO_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal ReasonRelease_VLD_Create()
        {
            //Mandatory - RsnCode
            RuleFor(f => f.RsnCode).NotEmpty().Length(1, 32).WithMessage("Invalid Reason Code").WithName("ReasonCodeValidation");
            //Mandatory - CmmValue
            RuleFor(f => f.CmmValue).NotEmpty().Length(1, 1024).WithMessage("Invalid ReasonRelease Comment").WithName("ReasonReleaseCommentValidation");
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator for ReasonRelease_DTO_Update
    /// </summary>
    internal class ReasonRelease_VLD_Update : AbstractValidator<ReasonRelease_DTO_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal ReasonRelease_VLD_Update()
        {
            //Mandatory - RsnCode
            RuleFor(f => f.RsnCode).NotEmpty().Length(1, 32).WithMessage("Invalid Reason Code").WithName("ReasonCodeValidation");
            //Mandatory - CmmValue
            RuleFor(f => f.CmmValue).NotEmpty().Length(1, 1024).WithMessage("Invalid ReasonRelease Comment").WithName("ReasonReleaseCommentValidation");
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator for ReasonRelease_DTO_Delete
    /// </summary>
    internal class ReasonRelease_VLD_Delete : AbstractValidator<ReasonRelease_DTO_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal ReasonRelease_VLD_Delete()
        {
            //Mandatory - RsnCode
            RuleFor(f => f.RsnCode).NotEmpty().Length(1, 32).WithMessage("Invalid Reason Code").WithName("ReasonCodeValidation");
            //Mandatory - RlsCode
            RuleFor(f => f.RlsCode).NotEmpty();
        }
    }
}