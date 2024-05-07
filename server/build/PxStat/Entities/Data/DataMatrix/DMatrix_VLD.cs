using API;
using DocumentFormat.OpenXml.Spreadsheet;
using FluentValidation;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxStat.Data
{

    public class DMatrix_VLD : AbstractValidator<IDmatrix>
    {
        readonly string _lng;
        public DMatrix_VLD(IADO ado=null, string language=null)
        {
            _lng = language ?? Configuration_BSO.GetApplicationConfigItem(ConfigType.global,"language.iso.code");

            RuleFor(x => x.Code).NotNull().NotEmpty().WithMessage(x=>Label.Get("px.build.code-empty",_lng));
            RuleFor(x => x.Copyright).NotNull().NotEmpty().WithMessage(x=>Label.Get("px.build.copyright-empty",_lng));
            RuleFor(x => x.Languages).NotNull().NotEmpty().WithMessage(x=>Label.Get("px.build.no-language-list",_lng));
            RuleFor(x => x.Languages.Count).GreaterThan(0).When(x => x.Languages != null).WithMessage(x => Label.Get("px.build.minimum-language", _lng));
            RuleFor(x => x).Must(ValidateCellCount).WithMessage(x =>String.Format( Label.Get("px.build.cell-match", _lng), x.Cells.Count));
            RuleFor(x => x.Dspecs).NotNull().NotEmpty().WithMessage(x => Label.Get("px.build.no-spec-list", _lng)); ;
            RuleFor(x => x.Dspecs.Count).GreaterThan(0).When(x => x.Dspecs != null).WithMessage(x => Label.Get("px.build.minimum-spec", _lng));
            RuleFor(x => x).Must(LanguagesSpecsMatch).WithMessage(x => Label.Get("px.build.language-mismatch", _lng));
            RuleFor(x => x).Must(LanguageInLanguagesMatch).WithMessage(x => Label.Get("px.build.no-language-property", _lng));
            RuleFor(x => x).Must(SpecsHaveSameDimensionCodes).WithMessage(x => Label.Get("px.build.dimension-codes-inconsistent", _lng));
            RuleFor(x => x).Must(CopyrightExists).WithMessage(x => Label.Get("px.build.copyright-not-found", _lng));

            RuleForEach(x => x.Languages).Length(2).WithMessage((String, x) => $"Language code {x} is invalid");

            When(x => x.Dspecs != null, () =>
           {
               RuleForEach(x => x.Dspecs.Values).NotNull().When(x => x.Dspecs != null).SetValidator(new Dspec_VLD( ado, _lng));
           });
            When(x => x.Dspecs?.Count > 1, () =>
                 {
                     RuleFor(x => x).Must(SpecsAreEquivalent).WithMessage(x => Label.Get("px.build.dimension-codes-inconsistent", _lng));
                 });
        }


        public bool CopyrightExists(IDmatrix matrix)
        {
            
            
            if (matrix.Copyright?.CprValue == null) return false;
            using (IADO ado = AppServicesHelper.StaticADO)
            {
                Copyright_ADO cAdo = new Copyright_ADO();
                var readCpr=cAdo.Read(ado, new Copyright_DTO_Read() { CprValue = matrix.Copyright.CprValue });
                return readCpr.hasData;
            }
           
        }

        /// <summary>
        /// Spec dimensions are consistent across specs
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool SpecsAreEquivalent(IDmatrix matrix)
        {
            var specs = matrix.Dspecs.Values.ToList();
            foreach (var spec in specs)
            {
                foreach (var otherSpec in specs.Where(x => _lng != spec.Language))
                {
                    for (int i = 0; i < spec.Dimensions.Count; i++)
                    {
                        if (!spec.Dimensions.ToList()[i].IsEquivalent(otherSpec.Dimensions.ToList()[i])) return false;
                    }

                }
            }
            return true;
        }

        public bool SpecsHaveSameDimensionCodes(IDmatrix matrix)
        {
            if (matrix.Dspecs == null) return true;
            foreach (var spec in matrix.Dspecs)
            {
                var codeList = spec.Value.Dimensions.Select(x => x.Code);
                foreach (var otherSpec in matrix.Dspecs)
                {
                    var otherCodeList = otherSpec.Value.Dimensions.Select(x => x.Code);
                    if (codeList.Intersect(otherCodeList).Count() != codeList.Count())
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Language property value not found in Languages property
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool LanguageInLanguagesMatch(IDmatrix matrix)
        {
            if (matrix.Dspecs == null) return false;
            if (matrix.Languages == null) return false;
            return matrix.Languages.Contains(matrix.Language);
        }

        /// <summary>
        /// Mismatch between spec languages and the languages listed in the Languages property
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool LanguagesSpecsMatch(IDmatrix matrix)
        {
            if (matrix.Dspecs == null) return false;
            if (matrix.Languages == null) return false;
            List<string> specLangs = matrix.Dspecs.Select(x => x.Key).ToList();
            if (specLangs == null) return false;
            return specLangs.Intersect(matrix.Languages).Count() == specLangs.Count && specLangs.Intersect(matrix.Languages).Count() == matrix.Languages.Count;
        }

        /// <summary>
        /// Actual and expected count of values must match"
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public bool ValidateCellCount(IDmatrix matrix)
        {
            if (matrix.Dspecs == null) return false;
            foreach (var spec in matrix.Dspecs)
            {
                int actual = 1;
                foreach (var dim in spec.Value.Dimensions)
                {
                    if (dim.Variables == null) return false;
                    actual *= dim.Variables.Count;

                }
                if (actual != matrix.Cells.Count) return false;
            }
            return true;
        }
    }

    //public class Dspec_VLD : AbstractValidator<Dictionary<string,IDspec>>

    public class Dspec_VLD : AbstractValidator<IDspec>
    {
        readonly string _lng;
        private IADO ado;

        public Dspec_VLD( IADO ado=null, string language=null)
        {
            _lng = language ?? Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
            this.ado = ado;

            RuleFor(x => x.Title).NotNull().NotEmpty().WithMessage(x => String.Format(Label.Get("px.build.no-spec-title", _lng),_lng));
            RuleFor(x => x.Contents).NotNull().NotEmpty().WithMessage(x => String.Format(Label.Get("px.build.no-spec-contents", _lng), _lng));
            RuleFor(x => x.Source).NotNull().NotEmpty().WithMessage(x => String.Format(Label.Get("px.build.no-source-field", _lng), _lng));
            RuleFor(x => x).Must(NoDimCodeDupe).WithMessage((IDspec, x) => String.Format(Label.Get("px.build.dupe-spec-dimension-codes", _lng), _lng));
            RuleFor(x => x).Must(NoDimValueDupe).WithMessage((IDspec, x) => String.Format(Label.Get("px.build.dupe-spec-dimension-values", _lng), _lng));

            RuleFor(x => x).Must(OneTimeDimension).WithMessage((IDspec, x) => String.Format(Label.Get("px.build.time-dimension-count", _lng), _lng));
            RuleFor(x => x).Must(OneStatDimension).WithMessage((IDspec, x) => String.Format(Label.Get("px.build.statistic-dimension-count", _lng), _lng));

            RuleFor(x => x).Must(SomeClassificationDimensions).WithMessage((IDspec, x) => String.Format(Label.Get("px.build.classification-dimension-count", _lng), _lng));
            RuleFor(x => x).Must(DimensionsAreOrdered).WithMessage((IDspec, x) => String.Format(Label.Get("px.build.dimension-sequence", _lng), _lng));
            RuleFor(x=>x).Must(LanguageMustExist).WithMessage((IDspec, x) => String.Format(Label.Get("px.build.language-not-exist", _lng), _lng));

            RuleForEach(x => x.Dimensions).SetValidator(new Dimension_VLD( _lng)).WithMessage(x=>Label.Get("px.build.dimension-error", _lng));
        }

        public bool LanguageMustExist(IDspec spec)
        {

            using(IADO tADO = AppServicesHelper.StaticADO)
            {
                Language_ADO langAdo = new Language_ADO(tADO);
                return langAdo.Exists(spec.Language);
            }
        }

        /// <summary>
        /// There is a consistent order of dimension in the sequence properties
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public bool DimensionsAreOrdered(IDspec spec)
        {
            //Check for dupes in the sequences
            List<int> sequences = spec.Dimensions.Select(x => x.Sequence).ToList();
            var dupes = sequences.GroupBy(x => x).Where(y => y.Count() > 1).ToList();
            if (dupes.Count > 0) return false;

            //Ensure all dimensions have a sequence
            if (sequences.Count != spec.Dimensions.Count) return false;

            return true;
        }

        /// <summary>
        /// There must be at lease one time dimension
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public bool OneTimeDimension(IDspec spec)
        {
            return spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_TIME).Count() == 1;
        }

        /// <summary>
        /// There must be at least one statistic dimension
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public bool OneStatDimension(IDspec spec)
        {
            return spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_STATISTIC).Count() == 1;
        }

        /// <summary>
        /// There must be at least one dimension with a CLASSIFICATION role
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public bool SomeClassificationDimensions(IDspec spec)
        {
            int scount = spec.Dimensions.Where(x => x.Role == Constants.C_DATA_DIMENSION_ROLE_CLASSIFICATION).Count();
            return scount > 0 && scount < 15;
        }


        /// <summary>
        /// No duplicates of dimension code
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public bool NoDimCodeDupe(IDspec spec)
        {

            List<string> codes = spec.Dimensions.Select(x => x.Code).ToList();
            var dupes = codes.GroupBy(x => x).Where(y => y.Count() > 1).Select(z => z.Key).ToList();
            if (dupes.Count > 0) return false;

            return true;
        }

        /// <summary>
        /// No duplicates of dimension value
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        public bool NoDimValueDupe(IDspec spec)
        {

            List<string> codes = spec.Dimensions.Select(x => x.Value).ToList();
            var dupes = codes.GroupBy(x => x).Where(y => y.Count() > 1).Select(z => z.Key).ToList();
            if (dupes.Count > 0) return false;

            return true;
        }
    }

    /// <summary>
    /// Dimension validator class
    /// </summary>
    /// 

    public class Dimension_VLD : AbstractValidator<IStatDimension>
    {
        readonly string _lng;
        public Dimension_VLD( string language=null)
        {
            _lng = language ?? Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

            RuleFor(x => x.Variables.Count).GreaterThan(0).When(x => x.Variables != null).WithMessage(x=>String.Format(Label.Get("px.build.no-variables", _lng), x.Code));

            RuleFor(x => x.Variables.Count).GreaterThan(0).When(x => x.Variables != null).WithMessage(x => String.Format(Label.Get("px.build.no-variables", _lng), x.Code));
            RuleFor(x => x.Variables).NotEmpty().NotNull().WithMessage(x => String.Format(Label.Get("px.build.missing-variable", _lng), x.Code));
            RuleFor(x => x.Role).NotNull().NotEmpty().WithMessage(x => String.Format(Label.Get("px.build.dimension-no-role", _lng), x.Code));
            RuleForEach(x => x.Variables).SetValidator(new DimensionVariable_VLD( _lng)).WithMessage(Label.Get("px.build.variable-error", _lng));
            RuleFor(x => x).Must(DimensionVariablesAreOrdered).WithMessage(x => String.Format(Label.Get("px.build.variable-sequence", _lng), x.Code));
            RuleFor(x => x.Code).NotNull().NotEmpty().WithMessage(Label.Get("px.build.dimension-code-empty", _lng));
            RuleFor(x => x.Value).NotNull().NotEmpty().WithMessage(Label.Get("px.build.dimension-value-empty", _lng));
            RuleFor(x => x.GeoUrl).NotNull().NotEmpty().When(x => x.GeoFlag).WithMessage(Label.Get("px.build.no-geo-url", _lng));
        }


        /// <summary>
        /// Check sequences in the variables
        /// </summary>
        /// <param name="dim"></param>
        /// <returns></returns>
        public bool DimensionVariablesAreOrdered(IStatDimension dim)
        {
            if (dim.Variables == null) return false;
            //Check for dupes in the sequences
            List<int> sequences = dim.Variables.Select(x => x.Sequence).ToList();
            var dupes = sequences.GroupBy(x => x).Where(y => y.Count() > 1).ToList();
            if (dupes.Count > 0) return false;

            //Ensure all variables have a sequence
            if (sequences.Count != dim.Variables.Count) return false;
            return true;
        }
    }

    /// <summary>
    /// Variable validation class
    /// </summary>
    /// 

    public class DimensionVariable_VLD : AbstractValidator<IDimensionVariable>
    {
        readonly string _lng;
        public DimensionVariable_VLD( string language=null)
        {
            _lng = language ?? Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");

            RuleFor(x => x.Code).NotNull().NotEmpty().WithMessage(Label.Get("px.build.no-variable-code", _lng));
            RuleFor(x => x.Value).NotNull().NotEmpty().WithMessage(Label.Get("px.build.no-variable-value", _lng));
        }
    }
}
