using API;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using PxParser.Resources.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class ValidatorExtensions
    {
        /// <summary>
        /// HaveFactsAndDimensionsMatching
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> HaveFactsAndDimensionsMatching<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new FactsAndDimensionsMatchValidator());

        }

        /// <summary>
        /// IRuleBuilderOptions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> DataIsGood<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new DataIsGoodValidator());

        }
    }

    public class DataIsGoodValidator : PropertyValidator
    {
        /// <summary>
        /// DataIsGoodValidator
        /// </summary>
        public DataIsGoodValidator() : base(Label.Get("px.integrity.data"))
        {

        }

        /// <summary>
        /// IsValid
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            return true;


        }

        /// <summary>
        /// CheckDataIsGood
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="messageFormatter"></param>
        /// <returns></returns>
        private bool CheckDataIsGood(dynamic cell, MessageFormatter messageFormatter)
        {
            try
            {
                IPxElement pxElement = cell;
                string theStringValue = pxElement.Value.ToString();
                double doubleValue = 0;

                if (String.IsNullOrEmpty(theStringValue) || theStringValue == Utility.GetCustomConfig("APP_PX_CONFIDENTIAL_VALUE"))
                {
                    return true;
                }
                else if (double.TryParse(theStringValue, out doubleValue))
                {
                    return true;
                }
                //String values are now allowed in data cells, so this must always be true
                else return true;

            }
            catch (Exception e)
            {
                Log.Instance.ErrorFormat("Error @ CheckDataIsGood for cell = {0}", cell);
                Log.Instance.Error(e);
                return false;
            }
        }
    }

    /// <summary>
    /// FactsAndDimensionsMatchValidator
    /// </summary>
    public class FactsAndDimensionsMatchValidator : PropertyValidator
    {

        public FactsAndDimensionsMatchValidator() : base(Label.Get("px.integrity.size"))
        {

        }

        /// <summary>
        /// IsValid
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            bool success = true;
            Matrix theMatrix = context.Instance as Matrix;

            var specList = context.PropertyValue as IList<Matrix.Specification>;
            if (specList != null)
            {
                foreach (var spec in specList)
                {
                    success &= CheckFactsAndDimensionsMatch(theMatrix.Cells.Count, spec, spec.Language, context.MessageFormatter);
                }
            }
            else
            {
                var mainSpec = context.PropertyValue as Matrix.Specification;
                if (mainSpec != null)
                {
                    success &= CheckFactsAndDimensionsMatch(theMatrix.Cells.Count, mainSpec, mainSpec.Language, context.MessageFormatter);
                }


            }

            return success;
        }

        /// <summary>
        /// CheckFactsAndDimensionsMatch
        /// </summary>
        /// <param name="totalCells"></param>
        /// <param name="spec"></param>
        /// <param name="language"></param>
        /// <param name="messageFormatter"></param>
        /// <returns></returns>
        private bool CheckFactsAndDimensionsMatch(int totalCells, Matrix.Specification spec, string language, MessageFormatter messageFormatter)
        {
            try
            {
                var cubeSize = 0;
                var statisticCount = spec.Statistic.Count;
                var periodCount = spec.Frequency.Period.Count;
                var variableCombinations = 1;


                foreach (var classification in spec.Classification)
                {
                    variableCombinations = variableCombinations * classification.Variable.Count;
                }

                cubeSize = variableCombinations * periodCount * statisticCount;


                if (cubeSize != totalCells)
                {
                    //TODO: do we need translation here?
                    messageFormatter.AppendArgument("Language", language);
                    messageFormatter.AppendArgument("Expected", cubeSize);
                    messageFormatter.AppendArgument("Found", totalCells);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Instance.ErrorFormat("Error @ CheckFactsAndDimensionsMatch for language = {0}", language);
                Log.Instance.Error(e);
                return false;
            }
        }
    }

    /// <summary>
    /// class
    /// </summary>
    class PxIntegrityValidator : AbstractValidator<Matrix>
    {
        public PxIntegrityValidator()
        {

            RuleFor(x => x.MainSpec).HaveFactsAndDimensionsMatching().WithMessage(Label.Get("px.integrity.size"));
            RuleFor(x => x.OtherLanguageSpec).HaveFactsAndDimensionsMatching().When(x => x.OtherLanguageSpec != null && x.OtherLanguageSpec.Count > 0).WithMessage(Label.Get("px.integrity.size"));
            RuleFor(x => x.Cells).DataIsGood().When(x => x.Cells != null && x.Cells.Count > 0).WithMessage(FormatDataIsGood);
            RuleFor(x => x).Must(NoDuplicateDomains).WithMessage(Label.Get("px.integrity.codes"));
            //RuleFor(x => x.MainSpec).SetValidator(new FactsAndDimensionsMatchValidator());
        }



        private static bool NoDuplicateDomains(Matrix matrix)
        {


            return !matrix.MainSpec.Classification.GroupBy(x => x.Code).Any(g => g.Count() > 1);
        }

        private static string FormatDataIsGood(Matrix matrix)
        {
            return String.Format(Label.Get("px.integrity.data"), matrix.Code);
        }

    }
}
