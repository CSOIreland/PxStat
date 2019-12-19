using System;
using System.Collections.Generic;
using API;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using PxParser.Resources.Parser;

namespace PxStat.Data.Px
{
    /// <summary>
    /// class
    /// </summary>
    public static partial class ValidatorExtensions
    {
        /// <summary>
        /// HaveSupportedLanguages
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="ado"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> HaveSupportedLanguages<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, ADO ado)
        {
            return ruleBuilder.SetValidator(new HaveSupportedLanguagesValidator(ado));

        }

        /// <summary>
        /// HaveSupportedSources
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="ruleBuilder"></param>
        /// <param name="ado"></param>
        /// <returns></returns>
        public static IRuleBuilderOptions<T, TProperty> HaveSupportedSources<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, ADO ado)
        {
            return ruleBuilder.SetValidator(new HaveSupportedSourcesValidator(ado));

        }

    }

    /// <summary>
    /// class
    /// </summary>
    public class HaveSupportedSourcesValidator : PropertyValidator
    {
        /// <summary>
        /// class variable
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public HaveSupportedSourcesValidator(ADO ado) : base((string)Label.Get("px.setting.source-invalid"))
        {
            this.ado = ado;
        }

        /// <summary>
        /// IsValid
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var list = context.PropertyValue as IList<Matrix.Specification>;

            if (list != null && list.Count > 0)
            {
                foreach (var e in list)
                {
                    if (!CheckIsAValidSource(e.Source, context.MessageFormatter))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// CheckIsAValidSource
        /// </summary>
        /// <param name="source"></param>
        /// <param name="messageFormatter"></param>
        /// <returns></returns>
        private bool CheckIsAValidSource(string source, MessageFormatter messageFormatter)
        {
            try
            {
                if (!Matrix.SourceIsSupported(ado, source))
                {
                    //TODO: do we need translation here?
                    messageFormatter.AppendArgument("Source", source);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Instance.ErrorFormat("Error @ CheckIsAValidSource for id = {0}", source);
                Log.Instance.Error(e);
                return false;
            }
        }
    }

    /// <summary>
    /// Class
    /// </summary>
    public class HaveSupportedLanguagesValidator : PropertyValidator
    {
        /// <summary>
        /// class variable
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public HaveSupportedLanguagesValidator(ADO ado) : base((string)Label.Get("px.setting.languages-invalid"))
        {
            this.ado = ado;
        }

        /// <summary>
        /// IsValid
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var list = context.PropertyValue as IList<IPxSingleElement>;
            bool success = true;

            if (list != null)
            {
                foreach (var e in list)
                {
                    success &= CheckIsAValidLanguage(e.SingleValue, context.MessageFormatter);
                }
            }

            return success;
        }

        /// <summary>
        /// CheckIsAValidLanguage
        /// </summary>
        /// <param name="language"></param>
        /// <param name="messageFormatter"></param>
        /// <returns></returns>
        private bool CheckIsAValidLanguage(string language, MessageFormatter messageFormatter)
        {
            try
            {
                if (!Matrix.LanguageIsSupported(ado, language))
                {
                    //TODO: do we need translation here?
                    messageFormatter.AppendArgument("Language", language);
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Instance.ErrorFormat("Error @ CheckIsAValidLanguage for id = {0}", language);
                Log.Instance.Error(e);
                return false;
            }
        }
    }

    /// <summary>
    /// class
    /// </summary>
    class PxSettingsValidator : AbstractValidator<Matrix>
    {
        /// <summary>
        /// class variable
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        internal PxSettingsValidator(ADO ado, bool includeSource = true)
        {

            this.ado = ado;
            RuleFor(x => x.FormatVersion).Must(v => (Matrix.AxisVersionIsSupported(ado, Resources.Constants.C_SYSTEM_PX_NAME, v) || Matrix.JsonStatVersionIsSupported(ado, Resources.Constants.C_SYSTEM_JSON_STAT_NAME, v))).WithMessage((string)Label.Get("px.setting.axisversion-invalid"));
            if (includeSource)
            {
                RuleFor(x => x.MainSpec.Source).Must(s => Matrix.SourceIsSupported(ado, s)).WithMessage((string)Label.Get("px.setting.source-invalid"));
            }
            RuleFor(x => x.OtherLanguageSpec).HaveSupportedSources(ado).When(x => x.OtherLanguageSpec != null && x.OtherLanguageSpec.Count > 0).WithMessage((string)Label.Get("px.setting.sources-invalid"));
            RuleFor(x => x.TheLanguage).Must(lang => Matrix.LanguageIsSupported(ado, lang)).When(x => x.Languages == null || x.Languages.Count == 0).WithMessage((string)Label.Get("px.setting.language-invalid"));
            RuleFor(x => x.Languages).HaveSupportedLanguages(ado).When(x => x.Languages != null && x.Languages.Count > 0).WithMessage((string)Label.Get("px.setting.languages-invalid"));
        }
    }
    /// <summary>
    /// Validator specifically for Build
    /// </summary>
    class PxSettingsValidatorBuild : AbstractValidator<Matrix>
    {
        /// <summary>
        /// class variable
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        internal PxSettingsValidatorBuild(ADO ado)
        {
            this.ado = ado;
            RuleFor(x => x.FormatVersion).Must(v => Matrix.AxisVersionIsSupported(ado, Resources.Constants.C_SYSTEM_PX_NAME, v)).WithMessage((string)Label.Get("px.setting.axisversion-invalid"));
            RuleFor(x => x.OtherLanguageSpec).HaveSupportedSources(ado).When(x => x.OtherLanguageSpec != null && x.OtherLanguageSpec.Count > 0).WithMessage((string)Label.Get("px.setting.sources-invalid"));
            RuleFor(x => x.TheLanguage).Must(lang => Matrix.LanguageIsSupported(ado, lang)).When(x => x.Languages == null || x.Languages.Count == 0).WithMessage((string)Label.Get("px.setting.language-invalid"));
            RuleFor(x => x.Languages).HaveSupportedLanguages(ado).When(x => x.Languages != null && x.Languages.Count > 0).WithMessage((string)Label.Get("px.setting.languages-invalid"));
        }
    }
}
