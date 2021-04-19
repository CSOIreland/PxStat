
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Data
{
    internal class Matrix_VLD : AbstractValidator<Matrix>
    {
        internal Matrix_VLD()
        {
            RuleFor(x => x).Must(ValidateCurrentDimensions).WithMessage("Dimension names or codes are invalid");
        }

        /// <summary>
        /// Check that there are no duplicate dimension codes or names in the Matrix
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        internal bool ValidateCurrentDimensions(Matrix m)
        {
            bool isValid = ValidateSpec(m.MainSpec);
            if (!isValid) return false;
            if (m.OtherLanguageSpec != null)
            {
                foreach (Matrix.Specification spec in m.OtherLanguageSpec)
                {
                    if (!ValidateSpec(spec)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check that there are no duplicate dimension codes or names in the Specification
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        private bool ValidateSpec(Matrix.Specification spec)
        {
            List<string> comparisons = new List<string>();
            comparisons.Add(spec.ContentVariable.ToUpper());
            comparisons.Add(spec.Frequency.Code.ToUpper());
            foreach (var cls in spec.Classification)
            {
                comparisons.Add(cls.Code.ToUpper());
            }

            if (hasDupes(comparisons)) return false;

            comparisons = new List<string>();
            comparisons.Add(spec.ContentVariable.ToUpper());
            comparisons.Add(spec.Frequency.Value.ToUpper());
            foreach (var cls in spec.Classification)
            {
                comparisons.Add(cls.Value.ToUpper());
            }

            return !hasDupes(comparisons);

        }

        private bool hasDupes(IEnumerable<string> list)
        {
            List<string> dupes = list.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            return dupes.Count > 0;
        }
    }
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

    internal class Matrix_VLD_ReadByGroup : AbstractValidator<Matrix_DTO_ReadByGroup>
    {
        public Matrix_VLD_ReadByGroup()
        {
            RuleFor(x => x.GrpCode).NotEmpty().Length(0, 32);
            RuleFor(x => x.LngIsoCode).NotEmpty().Length(2).When(x => !string.IsNullOrEmpty(x.LngIsoCode));
        }
    }

    internal class Matrix_VLD_ReadByCopyright : AbstractValidator<Matrix_DTO_ReadByCopyright>
    {
        public Matrix_VLD_ReadByCopyright()
        {
            RuleFor(x => x.CprCode).NotEmpty().Length(0, 32);
            RuleFor(x => x.LngIsoCode).NotEmpty().Length(2).When(x => !string.IsNullOrEmpty(x.LngIsoCode));
        }
    }

    internal class Matrix_VLD_ReadByLanguage : AbstractValidator<Matrix_DTO_ReadByLanguage>
    {
        public Matrix_VLD_ReadByLanguage()
        {
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
