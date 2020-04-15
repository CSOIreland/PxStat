using API;
using FluentValidation;
using PxStat.System.Settings;

namespace PxStat.JsonStatSchema
{
    internal class JsonStatQueryLive_VLD : AbstractValidator<CubeQuery_DTO>
    {
        internal JsonStatQueryLive_VLD()
        {
            string cultureRegex = Utility.GetCustomConfig("APP_LANGUAGE_CULTURE");
            RuleFor(x => x.jStatQuery.Class.Equals(Utility.GetCustomConfig("APP_JSON_STAT_QUERY")));
            RuleFor(x => x.jStatQuery.Version.Equals(Utility.GetCustomConfig("APP_JSON_STAT_QUERY_VERSION")));

            RuleFor(x => x.jStatQueryExtension.extension.Matrix).NotNull().Length(0, 20);
            RuleFor(x => x.jStatQueryExtension.extension.Language.Code.Length.Equals(2));
            RuleFor(x => x.jStatQueryExtension.extension.Language.Culture).Matches(cultureRegex).When(x => !string.IsNullOrEmpty(x.jStatQueryExtension.extension.Language.Culture));

            RuleFor(x => x.jStatQueryExtension.extension.Format).Must(ValidateFormat);
        }

        private bool ValidateFormat(dynamic frm)
        {

            Format_DTO_Read format = new Format_DTO_Read() { FrmType = frm.Type, FrmVersion = frm.Version, FrmDirection = "DOWNLOAD" };
            return Build.CustomValidations.FormatForReadDataset(format);
        }

    }

    internal class JsonStatQueryWip_VLD : AbstractValidator<CubeQuery_DTO>
    {
        internal JsonStatQueryWip_VLD()
        {
            string cultureRegex = Utility.GetCustomConfig("APP_LANGUAGE_CULTURE");
            RuleFor(x => x.jStatQuery.Class.Equals(Utility.GetCustomConfig("APP_JSON_STAT_QUERY")));
            RuleFor(x => x.jStatQuery.Version.Equals(Utility.GetCustomConfig("APP_JSON_STAT_QUERY_VERSION")));

            RuleFor(x => x.jStatQueryExtension.extension.RlsCode).GreaterThan(0);
            RuleFor(x => x.jStatQueryExtension.extension.Language.Code.Length.Equals(2));
            RuleFor(x => x.jStatQueryExtension.extension.Language.Culture).Matches(cultureRegex).When(x => !string.IsNullOrEmpty(x.jStatQueryExtension.extension.Language.Culture));

            RuleFor(x => x.jStatQueryExtension.extension.Format).Must(ValidateFormat);
        }

        private bool ValidateFormat(dynamic frm)
        {
            Format_DTO_Read format = new Format_DTO_Read() { FrmType = frm.Type, FrmVersion = frm.Version, FrmDirection = "DOWNLOAD" };
            return Build.CustomValidations.FormatForReadDataset(format);
        }

    }



    internal class JsonStatExtension_VLD : AbstractValidator<Extension>
    {
        internal JsonStatExtension_VLD()
        {
            string cultureRegex = Utility.GetCustomConfig("APP_LANGUAGE_CULTURE");
            RuleFor(x => x.Matrix).NotNull().Length(0, 20);
            RuleFor(x => x.Language).NotNull();
            RuleFor(x => x.Language.Code.Length.Equals(2));
            RuleFor(x => x.Language.CultureCode).Matches(cultureRegex);
            RuleFor(x => x.Format).Must(ValidateFormat);
        }

        private bool ValidateFormat(Format ex)
        {
            Format_DTO_Read format = new Format_DTO_Read() { FrmType = ex.Type, FrmVersion = ex.Version };
            return Build.CustomValidations.FormatForReadDataset(format);
        }
    }



}
