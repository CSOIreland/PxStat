using FluentValidation;
using PxStat.Resources;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Data
{

    public class DMatrix_VLD : AbstractValidator<IDmatrix>
    {
        public DMatrix_VLD()
        {
            RuleFor(x => x.Code).NotNull().NotEmpty().WithMessage("Code must not be empty");
            RuleFor(x => x.Copyright).NotNull().NotEmpty().WithMessage("Copyright must not be empty");
            RuleFor(x => x.Languages).NotNull().NotEmpty().WithMessage("No Language list found in the matrix");
            RuleFor(x => x.Languages.Count).GreaterThan(0).When(x => x.Languages != null).WithMessage("At least one language must be exist");
            RuleFor(x => x).Must(ValidateCellCount).WithMessage(x => $"Actual and expected count of values do not match - actual: {x.Cells.Count}");
            RuleFor(x => x.Dspecs).NotNull().NotEmpty().WithMessage("Specifcation list not found");
            RuleFor(x => x.Dspecs.Count).GreaterThan(0).When(x => x.Dspecs != null).WithMessage("There must be at least 1 spec in the matrix");
            RuleFor(x => x).Must(LanguagesSpecsMatch).WithMessage("Mismatch between spec languages and the languages listed in the Languages property");
            RuleFor(x => x).Must(LanguageInLanguagesMatch).WithMessage("Language property value not found in Languages property");
            RuleFor(x => x).Must(SpecsHaveSameDimensionCodes).WithMessage("Inconsistent dimension codes across specs");

            RuleForEach(x => x.Languages).Length(2).WithMessage((String, x) => $"Language code {x} is invalid");

            When(x => x.Dspecs != null, () =>
           {
               RuleForEach(x => x.Dspecs.Values).NotNull().When(x => x.Dspecs != null).SetValidator(new Dspec_VLD());
           });
            When(x => x.Dspecs?.Count > 1, () =>
                 {
                     RuleFor(x => x).Must(SpecsAreEquivalent).WithMessage("Inconsistent dimensions across specs");
                 });
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
                foreach (var otherSpec in specs.Where(x => x.Language != spec.Language))
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
        public Dspec_VLD()
        {
            RuleFor(x => x.Title).NotNull().NotEmpty().WithMessage(x => $"No spec title for spec {x.Language}");
            RuleFor(x => x.Contents).NotNull().NotEmpty().WithMessage(x => $"No spec Contents field for spec {x.Language}");
            RuleFor(x => x.Source).NotNull().NotEmpty().WithMessage(x => $"No Source field found for spec {x.Language}");
            RuleFor(x => x).Must(NoDimCodeDupe).WithMessage((IDspec, x) => $"Error {x.Language} spec has duplicate dimension codes");
            RuleFor(x => x).Must(NoDimValueDupe).WithMessage((IDspec, x) => $"Error {x.Language} spec has duplicate dimension values");
            RuleFor(x => x).Must(OneTimeDimension).WithMessage((IDspec, x) => $"Error {x.Language} spec has a wrong time dimension count");
            RuleFor(x => x).Must(OneStatDimension).WithMessage((IDspec, x) => $"Error {x.Language} spec has a wrong statistic dimension count");
            RuleFor(x => x).Must(SomeClassificationDimensions).WithMessage((IDspec, x) => $"Error {x.Language} spec has a wrong classification dimension count");
            RuleFor(x => x).Must(DimensionsAreOrdered).WithMessage((IDspec, x) => $"Error {x.Language} spec has invalid dimension sequencing");

            RuleForEach(x => x.Dimensions).SetValidator(new Dimension_VLD()).WithMessage("Dimension error - see individual message");
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
        public Dimension_VLD()
        {
            RuleFor(x => x.Variables.Count).GreaterThan(0).When(x => x.Variables != null).WithMessage(x => $"No variables found for dimension {x.Code}");
            RuleFor(x => x.Variables).NotEmpty().NotNull().WithMessage(x => $"Missing variable set in dimension {x.Code}");
            RuleFor(x => x.Role).NotNull().NotEmpty().WithMessage(x => $"Dimension {x.Code} has no Role assigned to it");
            RuleForEach(x => x.Variables).SetValidator(new DimensionVariable_VLD()).WithMessage("Variable Error - see individual message");
            RuleFor(x => x).Must(DimensionVariablesAreOrdered).WithMessage(x => $"Variable sequencing error for dimension {x.Code}");
            RuleFor(x => x.Code).NotNull().NotEmpty().WithMessage("Dimension Code must not be empty");
            RuleFor(x => x.Value).NotNull().NotEmpty().WithMessage("Dimension Value must not be empty");
            RuleFor(x => x.GeoUrl).NotNull().NotEmpty().When(x => x.GeoFlag).WithMessage("A Geo url must be provided when the Geo flag is set");

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
        public DimensionVariable_VLD()
        {
            RuleFor(x => x.Code).NotNull().NotEmpty().WithMessage("Variable missing a variable code");
            RuleFor(x => x.Value).NotNull().NotEmpty().WithMessage("Variable missing a variable value");
        }
    }
}
