using System.Linq;
using System.Collections.Generic;
using FluentValidation;
using PxParser.Resources.Parser;
using API;

namespace PxStat.Data.Px
{
    /// <summary>
    /// Schema Validator
    /// </summary>
    public class KeywordValidator : AbstractValidator<IPxKeywordElement>
    {
        /// <summary>
        /// SetRuleLenghtMinMax
        /// </summary>
        /// <param name="identifierName"></param>
        /// <param name="minLen"></param>
        /// <param name="maxLen"></param>
        /// <param name="errorMessage"></param>
        private void SetRuleLenghtMinMax(string identifierName, int minLen, int maxLen, string errorMessage)
        {
            RuleFor(x => x.Element.ToPxValue().Trim())
                .Length(minLen, maxLen)
                .When(x => x.Key.Identifier == identifierName)
                .WithMessage(string.Format(errorMessage, identifierName, minLen, maxLen));
        }

        /// <summary>
        /// SetRuleLenghtMin
        /// </summary>
        /// <param name="identifierName"></param>
        /// <param name="minLen"></param>
        /// <param name="errorMessage"></param>
        private void SetRuleLenghtMin(string identifierName, int minLen, string errorMessage)
        {
            RuleFor(x => x.Element.ToPxValue().Trim())
                .MinimumLength(minLen)
                .When(x => x.Key.Identifier == identifierName)
                .WithMessage(string.Format(errorMessage, identifierName, minLen));
        }

        /// <summary>
        /// SetRuleNumericMinMax
        /// </summary>
        /// <param name="identifierName"></param>
        /// <param name="minLen"></param>
        /// <param name="maxLen"></param>
        /// <param name="errorMessage"></param>
        private void SetRuleNumericMinMax(string identifierName, int minLen, int maxLen, string errorMessage)
        {
            RuleFor(x => x.Element.ToDoubleValue())
                .InclusiveBetween(minLen, maxLen)
                .When(x => x.Key.Identifier == identifierName)
                .WithMessage(string.Format(errorMessage, identifierName, minLen, maxLen));
        }

        /// <summary>
        /// SetRuleBoolean
        /// </summary>
        /// <param name="identifierName"></param>
        /// <param name="errorMessage"></param>
        private void SetRuleBoolean(string identifierName, string errorMessage)
        {
            RuleFor(x => x.Element.ToPxValue().ToUpper())
                .Must(x => x == Utility.GetCustomConfig("APP_PX_TRUE") || x == Utility.GetCustomConfig("APP_PX_FALSE"))
                .When(x => x.Key.Identifier == identifierName)
                .WithMessage(string.Format(errorMessage, identifierName));
        }

        /// <summary>
        /// SetRuleLenghtMinMaxList
        /// </summary>
        /// <param name="identifierName"></param>
        /// <param name="minLen"></param>
        /// <param name="maxLen"></param>
        /// <param name="errorMessage"></param>
        private void SetRuleLenghtMinMaxList(string identifierName, int minLen, int maxLen, string errorMessage)
        {
            RuleForEach(x => x.Element.ToList())
                .Length(minLen, maxLen)
                .When(x => x.Key.Identifier == identifierName)
                .WithMessage(string.Format(errorMessage, identifierName, minLen, maxLen));
        }

        /// <summary>
        /// SetRuleRegEx
        /// </summary>
        /// <param name="identifierName"></param>
        /// <param name="regExpression"></param>
        /// <param name="errorMessage"></param>
        private void SetRuleRegEx(string identifierName, string regExpression, string errorMessage)
        {
            RuleFor(x => x.Element.ToPxValue())
                .Matches(regExpression)
                .When(x => x.Key.Identifier == identifierName)
                .WithMessage(string.Format(errorMessage, identifierName));
        }

        /// <summary>
        /// KeywordValidator
        /// </summary>
        public KeywordValidator()
        {
            string errorMessageLenMinMax = Label.Get("px.schema.key-length-min-max");
            string errorMessageLenMin = Label.Get("px.schema.key-length-min");
            string errorMessageBoolean = Label.Get("px.schema.key-boolean");
            string errorMessageNumericMinMax = Label.Get("px.schema.key-numeric-min-max");
            string errorMessageAlphaNumeric = Label.Get("px.schema.key-alphanumeric");
            string regularExpressionAphaNumeric = Utility.GetCustomConfig("APP_REGEX_ALPHA_NUMERIC"); //Do not use translate

            SetRuleLenghtMinMax("AXIS-VERSION", 1, 20, errorMessageLenMinMax);
            SetRuleLenghtMinMax("CONTENTS", 1, 256, errorMessageLenMinMax);
            SetRuleLenghtMinMax("CONTVARIABLE", 1, 256, errorMessageLenMinMax); // list but with subkeys
            SetRuleLenghtMinMax("DOMAIN", 1, 256, errorMessageLenMinMax); // list but with subkeys
            SetRuleLenghtMinMax("LANGUAGE", 2, 2, errorMessageLenMinMax);

            SetRuleLenghtMinMax("MATRIX", 1, 20, errorMessageLenMinMax);
            SetRuleRegEx("MATRIX", regularExpressionAphaNumeric, errorMessageAlphaNumeric);


            SetRuleLenghtMinMax("SOURCE", 1, 256, errorMessageLenMinMax);
            SetRuleLenghtMinMax("UNITS", 1, 256, errorMessageLenMinMax);  // list only with subkeys or languages

            SetRuleNumericMinMax("DECIMAL", 0, 15, errorMessageNumericMinMax);
            SetRuleNumericMinMax("DECIMALS", 0, 15, errorMessageNumericMinMax); // list with subkeys 
            SetRuleNumericMinMax("PRECISION", 1, 6, errorMessageNumericMinMax);

            SetRuleBoolean("OFFICIAL-STATISTICS", errorMessageBoolean);

            //SetRuleLenghtMin("NOTEX", 1, errorMessageLenMin);
            SetRuleLenghtMin("MAP", 1, errorMessageLenMin);

            SetRuleLenghtMinMaxList("HEADING", 1, 256, errorMessageLenMinMax); // list
            SetRuleLenghtMinMaxList("CODES", 1, 256, errorMessageLenMinMax); //list
            SetRuleLenghtMinMaxList("LANGUAGES", 2, 2, errorMessageLenMinMax); // list 
            SetRuleLenghtMinMaxList("STUB", 1, 256, errorMessageLenMinMax); // list but with subkeys
            SetRuleLenghtMinMaxList("TIMEVAL", 1, 256, errorMessageLenMinMax); // list but with subkeys
            SetRuleLenghtMinMaxList("VALUES", 1, 256, errorMessageLenMinMax); // list
        }
    }

    /// <summary>
    /// class
    /// </summary>
    class PxSchemaValidator : AbstractValidator<PxDocument>
    {
        /// <summary>
        /// class variable 
        /// </summary>
        private ADO ado;

        /// <summary>
        /// constructor
        /// </summary>
        internal PxSchemaValidator()
        {
            // mandatory
            PxElementMustExist("AXIS-VERSION");
            PxElementMustExist("CONTENTS");
            PxElementMustExist("DATA");
            OneOfPxElementMustExist("DECIMAL", "DECIMALS");
            OneOfPxElementMustExist("HEADING", "STUB");
            PxElementMustExist("MATRIX");
            PxElementMustExist("UNITS");
            PxElementMustExist("VALUES");

            RuleForEach(x => x.Keywords).SetValidator(new KeywordValidator());
        }

        /// <summary>
        /// PxSchemaValidator
        /// </summary>
        /// <param name="ado"></param>
        internal PxSchemaValidator(ADO ado) : this()
        {
            this.ado = ado;
        }

        /// <summary>
        /// OneOfPxElementMustExist
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private IRuleBuilderOptions<PxDocument, IList<IPxKeywordElement>> OneOfPxElementMustExist(string v1, string v2)
        {
            return
                        RuleFor(doc => doc.Keywords)
                            .Must(keys => keys.Any(item => item.Key.Identifier == v1 || item.Key.Identifier == v2))
                            .WithMessage((string)string.Format(Label.Get("px.schema.key-any-of"), v1, v2));
        }

        /// <summary>
        /// PxElementMustExist
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        private IRuleBuilderOptions<PxDocument, IList<IPxKeywordElement>> PxElementMustExist(string identifier)
        {
            return
                        RuleFor(doc => doc.Keywords)
                            .Must(keys => keys.Any(item => item.Key.Identifier == identifier))
                            .WithMessage((string)string.Format(Label.Get("px.schema.key-mandatory"), identifier));
        }
    }
}
