
using FluentValidation;

namespace PxStat.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class Matrix_VLD_ReadCodeList : AbstractValidator<Matrix_DTO_Read>
    {
        public Matrix_VLD_ReadCodeList()
        {

        }
    }

    internal class Matrix_VLD_ReadByProduct : AbstractValidator<Matrix_DTO_ReadByProduct>
    {
        public Matrix_VLD_ReadByProduct()
        {
            RuleFor(x => x.PrcCode).NotEmpty().Length(0, 32);
            RuleFor(x => x.LngIsoCode).NotEmpty().Length(2);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Matrix_VLD_Read : AbstractValidator<Matrix_DTO_Read>
    {
        public Matrix_VLD_Read()
        {
            RuleFor(x => x.LngIsoCode).NotEmpty().Length(2);
            RuleFor(x => x.RlsCode).NotEmpty().GreaterThan(0);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class PxUploadValidator : AbstractValidator<PxUpload_DTO>
    {
        public PxUploadValidator()
        {
            RuleFor(x => x.MtrInput).NotEmpty();
            RuleFor(x => x.GrpCode).NotEmpty();
            RuleFor(x => x.FrqValueTimeval).Length(1, 256).When(x => !string.IsNullOrEmpty(x.FrqValueTimeval));
            RuleFor(x => x.FrqCodeTimeval).Length(1, 256).When(x => !string.IsNullOrEmpty(x.FrqCodeTimeval));
            RuleFor(x => x.LngIsoCode.Length).Equal(2).When(x => !string.IsNullOrEmpty(x.LngIsoCode));
            RuleFor(x => x).Must(Security.CustomValidations.CheckGroupExists).WithMessage("Group does not exist");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class PxBuildValidator : AbstractValidator<PxUpload_DTO>
    {
        public PxBuildValidator()
        {
            RuleFor(x => x.MtrInput).NotEmpty();
            RuleFor(x => x.FrqValueTimeval).Length(1, 256).When(x => !string.IsNullOrEmpty(x.FrqValueTimeval));
            RuleFor(x => x.FrqCodeTimeval).Length(1, 256).When(x => !string.IsNullOrEmpty(x.FrqCodeTimeval));
            RuleFor(x => x.LngIsoCode.Length).Equal(2).When(x => !string.IsNullOrEmpty(x.LngIsoCode));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class PxCreateMatrixValidator : AbstractValidator<PxUpload_DTO>
    {
        public PxCreateMatrixValidator()
        {
            RuleFor(x => x.MtrInput).NotEmpty();
            RuleFor(x => x.GrpCode).NotEmpty();
            RuleFor(x => x.Signature).NotEmpty();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Matrix_VLD_ReadHistory : AbstractValidator<Matrix_DTO_ReadHistory>
    {
        public Matrix_VLD_ReadHistory()
        {
            RuleFor(x => x.DateFrom).NotEmpty();
            RuleFor(x => x.DateTo).NotEmpty();
            RuleFor(f => f.LngIsoCode).NotEmpty().Length(2);
        }
    }
}
