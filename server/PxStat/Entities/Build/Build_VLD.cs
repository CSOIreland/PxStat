
using API;
using FluentValidation;
using PxStat.Data;
using PxStat.Entities.BuildData;
using PxStat.Resources;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxStat.Build
{



    /// <summary>
    /// 
    /// </summary>
    internal class Build_Update_VLD : AbstractValidator<List<DataItem_DTO>>
    {
        /// <summary>
        /// 
        /// </summary>

        private bool NoDuplicatesExist(List<DataItem_DTO> csv)
        {
            if (csv.GroupBy(x => x).Any(grp => grp.Count() > 1)) return false;
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="theMatrixData"></param>
        /// <param name="csv"></param>
        internal Build_Update_VLD()
        {
            RuleFor(x => x).Must(NoDuplicatesExist).WithMessage("Duplicate items exist in the csv data");


        }




    }

    /// <summary>
    /// 
    /// </summary>
    internal class Build_VLD_Periods : AbstractValidator<PxPeriodListDTO>
    {
        internal Build_VLD_Periods()
        {
            RuleFor(x => x).Must(CustomValidations.PeriodsNotRepeated).WithMessage("One or more period codes have been repeated");
            RuleForEach(x => x.Periods).SetValidator(new PxBuild_VLD_Period());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class PxBuild_VLD_Period : AbstractValidator<PeriodRecordDTO_Create>
    {
        internal PxBuild_VLD_Period()
        {
            RuleFor(x => x.Code).NotEmpty();
            RuleFor(x => x.Value).NotEmpty();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Build_VLD_UpdateMetadata : AbstractValidator<Build_DTO>
    {
        internal Build_VLD_UpdateMetadata()
        {
            RuleFor(f => f.MtrCode).NotEmpty().Length(1, 20);
            RuleFor(f => f.CprCode).NotEmpty().Length(1, 32);
            RuleFor(f => f.FrqCode).NotEmpty().Length(1, 256);
            RuleFor(f => f.DimensionList).Must(CustomValidations.LanguagesUnique).WithMessage("Non unique language");
            RuleFor(f => f.FrqValue).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValue));
            RuleForEach(f => f.DimensionList).SetValidator(new Dimension_VLD_Lite());

            RuleFor(f => f).Must(CustomValidations.CopyrightCodeExists).WithMessage("Copyright code does not exist in the system");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Build_VLD_Create : AbstractValidator<Build_DTO>
    {
        /// <summary>
        /// Top level validator
        /// </summary>
        internal Build_VLD_Create()
        {
            RuleFor(f => f.MtrCode).NotEmpty().Length(1, 20);
            RuleFor(f => f.MtrCode).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(f => f.CprCode).NotEmpty().Length(1, 32);
            RuleFor(f => f.CprCode).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(f => f.FrqCode).NotEmpty().Length(1, 256);
            RuleFor(f => f.FrqCode).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(f => f.LngIsoCode).NotEmpty().Matches("^[a-z]{2}$");
            RuleFor(f => f.Elimination).NotEmpty().WithMessage("Elimination object not supplied in request");
            RuleFor(f => f.DimensionList).Must(CustomValidations.DimensionsValid).WithMessage("Invalid Dimensions");
            RuleFor(f => f.DimensionList).Must(CustomValidations.LanguagesUnique).WithMessage("Non unique language");
            RuleFor(f => f).Must(CustomValidations.MainLanguageRepresented).WithMessage("Main language not contained in any dimension");
            RuleForEach(f => f.DimensionList).SetValidator(x => new Dimension_VLD(x.Elimination ?? new Dictionary<string, string>()));
            RuleFor(f => f.Format.FrmType).NotEmpty().Length(1, 32);
            RuleFor(f => f.Format.FrmVersion).NotEmpty().Length(1, 32);
            RuleFor(f => f.Format).Must(CustomValidations.FormatExists).WithMessage("Requested format/version/direction not found in the system");
            RuleFor(f => f.Format).NotEmpty();



            RuleFor(dto => dto.Format).Must(CustomValidations.FormatForBuildCreate);

        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Build_Read_VLD : AbstractValidator<Build_DTO_Read>
    {
        /// <summary>
        /// 
        /// </summary>
        internal Build_Read_VLD()
        {
            RuleFor(f => f.MtrInput).NotEmpty().WithMessage("MtrInput must not be empty");
            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval)).WithMessage("You must supply a FrqCode with a FrqValue");
            RuleFor(f => f).Must(CustomValidations.SignatureMatch).WithMessage("MtrInput does not match the supplied signature");
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("You must supply a FrqValue with a FrqCode");

        }



    }

    internal class Build_Validate_VLD : AbstractValidator<Build_DTO_Read>
    {
        /// <summary>
        /// 
        /// </summary>
        internal Build_Validate_VLD()
        {

            RuleFor(f => f.MtrInput).NotEmpty().WithMessage("MtrInput must not be empty");
            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval)).WithMessage("You must supply a FrqCode with a FrqValue");
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("You must supply a FrqValue with a FrqCode");
        }



    }



    /// <summary>
    /// Validator for a lighter version of dimension - e.g. that used in the PxBuild UpdateMetadata
    /// </summary>
    internal class Dimension_VLD_Lite : AbstractValidator<Dimension_DTO>
    {
        /// <summary>
        /// 
        /// </summary>
        internal Dimension_VLD_Lite()
        {
            RuleFor(f => f.CprValue).NotEmpty().Length(1, 256);
            RuleFor(f => f.FrqCode).NotEmpty().Length(1, 256);
            RuleFor(f => f.FrqValue).NotEmpty().Length(1, 256);
            RuleFor(f => f.LngIsoCode).NotEmpty().Matches("^[a-z]{2}$");
            RuleFor(f => f.MtrTitle).NotEmpty().Length(1, 256);
        }
    }

    /// <summary>
    /// Dimension validator. This calls validators for Classification and Statistic objects
    /// </summary>
    internal class Dimension_VLD : AbstractValidator<Dimension_DTO>
    {
        readonly string fchars = Utility.GetCustomConfig("APP_BUILD_REGEX_FORBIDDEN_CHARS");
        /// <summary>
        /// 
        /// </summary>
        internal Dimension_VLD(Dictionary<string, string> elimination)
        {
            RuleFor(f => f.CprValue).NotEmpty().Length(1, 256);

            RuleFor(f => f.LngIsoCode).NotEmpty().Matches("^[a-z]{2}$");
            RuleFor(f => f.MtrTitle).NotEmpty().Length(1, 256);

            RuleFor(f => f.Contents).NotEmpty().Length(1, 256);
            RuleFor(f => f.StatisticLabel).NotEmpty().Length(1, 256);
            RuleFor(f => f.StatisticLabel).NotEqual(f => f.FrqValue).WithMessage("Statistic Label must not be the same as the Frequency Value");
            RuleFor(f => f.Statistics).Must(CustomValidations.StatisticCodeNotRepeated).WithMessage("Non unique Statistic Code");
            RuleFor(f => f.Statistics).Must(CustomValidations.StatisticValueNotRepeated).WithMessage("Non unique Statistic Value");
            RuleFor(f => f.Statistics.Count).GreaterThan(0).WithMessage("You must have at least one Statistic");
            RuleFor(f => f.Classifications).Must(CustomValidations.ClassificationCodeNotRepeated).WithMessage("Non unique Classification Code");
            RuleFor(f => f.Classifications).Must(CustomValidations.ClassificationValueNotRepeated).WithMessage("Non unique Classification Value");
            RuleFor(f => f).Must(CustomValidations.ClassificationValueNotTheSameAsFrequencyValue).WithMessage("Classification value the same as Frequency value");
            RuleFor(f => f).Must(CustomValidations.StatisticsValueNotTheSameAsStatisticsLabel).WithMessage("Statistic value must not be the same as Statistics Label");
            RuleFor(f => f.Classifications.Count).GreaterThan(0).WithMessage("You must have at least one classification");
            RuleFor(f => f.Frequency.Period.Count).GreaterThan(0).WithMessage("You must have at least one period");
            RuleForEach(f => f.Classifications).SetValidator(new Classification_VLD());
            RuleForEach(f => f.Classifications).SetValidator(new Classification_VLD_CodeOnly(elimination));
            RuleForEach(f => f.Statistics).SetValidator(new Statistic_VLD());
            RuleFor(f => f.Frequency).SetValidator(new Frequency_VLD());

            //Does not contain inverted commas


            RuleFor(f => f.MtrTitle).Must(CustomValidations.ValidateIgnoreEscapeChars);
            RuleFor(f => f.MtrNote).Must(CustomValidations.ValidateIgnoreEscapeChars);
            RuleFor(f => f.StatisticLabel).Matches(fchars);
            RuleFor(f => f.FrqValue).Matches(fchars);
        }


    }

    //FrequencyRecordDTO_Create

    internal class Frequency_VLD : AbstractValidator<FrequencyRecordDTO_Create>
    {
        readonly string fchars = Utility.GetCustomConfig("APP_BUILD_REGEX_FORBIDDEN_CHARS");
        internal Frequency_VLD()
        {
            RuleFor(x => x.Value).NotEmpty().Length(1, 256);
            RuleFor(x => x.Code).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));
            RuleForEach(x => x.Period).SetValidator(new Period_VLD());
            RuleFor(x => x.Period).Must(CustomValidations.PeriodCodeNotRepeatedInList).WithMessage("Duplicate period codes ");
            RuleFor(x => x.Period).Must(CustomValidations.PeriodValueNotRepeatedInList).WithMessage("Duplicate period codes ");
            //Does not contain inverted commas
            RuleFor(f => f.Value).Matches(fchars);
        }
    }

    //

    internal class Period_VLD : AbstractValidator<PeriodRecordDTO_Create>
    {
        readonly string fchars = Utility.GetCustomConfig("APP_BUILD_REGEX_FORBIDDEN_CHARS");
        internal Period_VLD()
        {
            RuleFor(x => x.Code).NotEmpty().Length(1, 256);
            RuleFor(x => x.Value).NotEmpty().Length(1, 256);
            RuleFor(x => x.Code).Must(CustomValidations.NotReservedWord);
            RuleFor(x => x.Code).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));

            //Does not contain inverted commas
            RuleFor(f => f.Code).Matches(fchars);
            RuleFor(f => f.Value).Matches(fchars);
        }
    }

    /// <summary>
    /// Validates a Classification. This calls the validator for variables as required
    /// </summary>
    internal class Classification_VLD : AbstractValidator<ClassificationRecordDTO_Create>
    {
        readonly string fchars = Utility.GetCustomConfig("APP_BUILD_REGEX_FORBIDDEN_CHARS");
        /// <summary>
        /// 
        /// </summary>
        internal Classification_VLD()
        {
            string geoCodeRegex = Utility.GetCustomConfig("APP_REGEX_URL");
            RuleFor(f => f.Code).NotEmpty().Length(1, 256);
            RuleFor(f => f.Code).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(f => f.Value).NotEmpty().Length(1, 256);
            RuleFor(f => f.GeoUrl).NotEmpty().Length(1, 2048).When(f => !string.IsNullOrEmpty(f.GeoUrl));
            RuleFor(f => f.GeoUrl).Matches(geoCodeRegex).When(f => !string.IsNullOrEmpty(f.GeoUrl));
            RuleFor(f => f.Variable).Must(CustomValidations.VariableCodeNotRepeated).WithMessage("Non unique Variable Code");
            RuleFor(f => f.Variable).Must(CustomValidations.VariableValueNotRepeated).WithMessage("Non unique Variable Value");//
            RuleForEach(f => f.Variable).SetValidator(new Variable_VLD());
            RuleFor(f => f.Code).Must(CustomValidations.NotReservedWord);

            //Does not contain inverted commas
            RuleFor(f => f.Code).Matches(fchars);
            RuleFor(f => f.Value).Matches(fchars);
        }

    }


    /// <summary>
    /// Classification validator for codes only
    /// </summary>
    internal class Classification_VLD_CodeOnly : AbstractValidator<ClassificationRecordDTO_Create>
    {
        readonly string fchars = Utility.GetCustomConfig("APP_BUILD_REGEX_FORBIDDEN_CHARS");
        /// <summary>
        /// 
        /// </summary>
        internal Classification_VLD_CodeOnly()
        {
            RuleFor(f => f.Code).NotEmpty().Length(1, 256);

            RuleFor(f => f.Code).Must(CustomValidations.NotReservedWord);
            RuleFor(f => f.Code).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));

            //Does not contain inverted commas
            RuleFor(f => f.Code).Matches(fchars);

        }

        internal Classification_VLD_CodeOnly(Dictionary<string, string> elimination)
        {
            RuleFor(x => x).Must(x => elimination.ContainsKey(x.Code)).WithMessage("Classification not found in Elimination");
        }

    }

    /// <summary>
    /// Variable validator
    /// </summary>
    internal class Variable_VLD : AbstractValidator<VariableRecordDTO_Create>
    {
        readonly string fchars = Utility.GetCustomConfig("APP_BUILD_REGEX_FORBIDDEN_CHARS");
        /// <summary>
        /// 
        /// </summary>
        internal Variable_VLD()
        {
            RuleFor(f => f.Code).NotEmpty().Length(1, 256);
            RuleFor(f => f.Value).NotEmpty().Length(1, 256);

            //Does not contain inverted commas
            RuleFor(f => f.Code).Matches(fchars);
            RuleFor(f => f.Code).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(f => f.Value).Matches(fchars);
        }
    }

    /// <summary>
    /// Statistic validator
    /// </summary>
    internal class Statistic_VLD : AbstractValidator<StatisticalRecordDTO_Create>
    {
        readonly string fchars = Utility.GetCustomConfig("APP_BUILD_REGEX_FORBIDDEN_CHARS");
        internal Statistic_VLD()
        {
            RuleFor(f => f.Code).NotEmpty().Length(1, 256);
            RuleFor(f => f.Code).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(f => f.Value).NotEmpty().Length(1, 256);
            RuleFor(f => f.Unit).NotEmpty().Length(1, 256); ;
            RuleFor(f => Convert.ToInt32(f.Decimal)).InclusiveBetween(0, 6);
            RuleFor(f => f.Code).Must(CustomValidations.NotReservedWord);

            //Does not contain inverted commas
            RuleFor(f => f.Code).Matches(fchars);
            RuleFor(f => f.Value).Matches(fchars);
            RuleFor(f => f.Unit).Matches(fchars);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    internal class Statistic_VLD_Partial : AbstractValidator<Statistic_DTO>
    {
        internal Statistic_VLD_Partial()
        {
            RuleForEach(f => f.Statistic).SetValidator(new Statistic_VLD());
            RuleFor(f => f.Statistic).Must(CustomValidations.StatisticCodeNotRepeated).WithMessage("Non unique SttCode");
        }
    }

    /// <summary>
    /// Used for partial validation. This validates the top level items in the build request
    /// </summary>
    internal class Build_VLD_Partial : AbstractValidator<Build_DTO>
    {
        internal Build_VLD_Partial()
        {
            RuleFor(f => f.MtrCode).NotEmpty().Length(1, 20);
            RuleFor(f => f.LngIsoCode).NotEmpty().Matches("^[a-z]{2}$");
        }
    }



    /// <summary>
    /// Used for partial validation. This validates the top level items in the dimension
    /// </summary>
    internal class Dimension_VLD_Partial : AbstractValidator<Dimension_DTO>
    {
        /// <summary>
        /// 
        /// </summary>
        internal Dimension_VLD_Partial()
        {
            RuleFor(f => f.CprValue).NotEmpty().Length(1, 256).WithMessage("Value not found for required Copyright Code");
            RuleFor(f => f.FrqCode).NotEmpty().Length(1, 256);
            RuleFor(f => f.FrqValue).NotEmpty().Length(1, 256);
            RuleFor(f => f.LngIsoCode).NotEmpty().Matches("^[a-z]{2}$");
            RuleFor(f => f.MtrTitle).NotEmpty().Length(1, 256);
            RuleFor(f => f.StatisticLabel).NotEmpty().Length(1, 256);

        }
    }

    internal class Dimension_VLD_UltraLite : AbstractValidator<Dimension_DTO>
    {
        internal Dimension_VLD_UltraLite()
        {
            RuleFor(f => f.Frequency.Period).NotNull();
            RuleForEach(f => f.Frequency.Period).SetValidator(new Period_VLD());
        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Build_VLD_ReadTemplate : AbstractValidator<Build_DTO_ReadTemplate>
    {
        /// <summary>
        /// 
        /// </summary>
        internal Build_VLD_ReadTemplate()
        {
            //Required
            RuleFor(x => x.MtrInput).NotEmpty().WithMessage("Matrix data must not be empty");
            //Optional
            //Optional - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval));
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval));
            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");
            RuleFor(f => f).Must(CustomValidations.SignatureMatch).WithMessage("MtrInput does not match the supplied signature");

        }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class Build_VLD_Update : AbstractValidator<BuildUpdate_DTO>
    {
        internal Build_VLD_Update()
        {
            RuleFor(x => x.MtrInput).NotEmpty().WithMessage("Matrix data must not be empty");
            RuleFor(x => x.MtrCode).Length(1, 20).When(f => !string.IsNullOrEmpty(f.MtrCode)).WithMessage("Matrix code must not be empty");
            RuleFor(x => x.MtrCode).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(x => x.CprCode).Length(1, 20).When(f => !string.IsNullOrEmpty(f.CprCode)).WithMessage("Copyright code must not be empty");
            RuleFor(x => x.CprCode).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));

            RuleFor(x => x).Must(CustomValidations.PeriodsNotRepeated).WithMessage("One or more period codes have been repeated");
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval));
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval));
            RuleFor(f => f.FrqCodeTimeval).Matches(Utility.GetCustomConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");
            RuleFor(f => f.Dimension).Must(CustomValidations.LanguagesUnique).WithMessage("Non unique language");
            RuleFor(f => f).Must(CustomValidations.FormatExists).WithMessage("Requested format/version not found in the system");
            RuleFor(f => f).Must(CustomValidations.SignatureMatch).WithMessage("MtrInput does not match the supplied signature");
            RuleFor(f => f.Elimination).NotEmpty().WithMessage("Elimination object not supplied in request");
            RuleFor(f => f.Format.FrmType).NotEmpty();
            RuleFor(f => f.Format.FrmVersion).NotEmpty();
            RuleFor(dto => dto.Format).Must(CustomValidations.FormatExists);

            RuleFor(dto => dto.Format).Must(CustomValidations.FormatForBuildUpdate);

            RuleForEach(f => f.Dimension).SetValidator(new Build_VLD_BuildUpdateDimension());
            RuleForEach(f => f.Dimension).SetValidator(x => new Build_VLD_BuildUpdateDimension(x.Elimination ?? new Dictionary<string, string>()));

        }

    }


    /// <summary>
    /// Dimension validator for Build Update
    /// </summary>
    internal class Build_VLD_BuildUpdateDimension : AbstractValidator<Dimension_DTO>
    {


        internal Build_VLD_BuildUpdateDimension()
        {

            RuleFor(f => f.MtrTitle).Must(CustomValidations.ValidateIgnoreEscapeChars);
            RuleFor(f => f.MtrTitle).NotEmpty().Length(1, 256);
            RuleFor(f => f.MtrNote).Must(CustomValidations.ValidateIgnoreEscapeChars);
            RuleFor(f => f.StatisticLabel).Must(CustomValidations.ValidateIgnoreEscapeChars).When(f => !string.IsNullOrEmpty(f.StatisticLabel));

            RuleFor(f => f.Frequency).SetValidator(new Frequency_VLD()).When(f => f.Frequency != null);
            RuleForEach(f => f.Classifications).SetValidator(new Classification_VLD_CodeOnly());

        }

        internal Build_VLD_BuildUpdateDimension(Dictionary<string, string> elimination)
        {
            RuleForEach(f => f.Classifications).SetValidator(new Classification_VLD_CodeOnly(elimination));
        }

    }


    /// <summary>
    /// 
    /// </summary>
    internal class Build_VLD_BuildRead : AbstractValidator<BuildUpdate_DTO>
    {
        internal Build_VLD_BuildRead()
        {

            RuleFor(x => x).Must(CustomValidations.PeriodsNotRepeated).WithMessage("One or more period codes have been repeated");
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval));
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval));
            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");

            RuleFor(f => f.Dimension).Must(CustomValidations.LanguagesUnique).WithMessage("Non unique language");
            RuleFor(f => f).Must(CustomValidations.SignatureMatch).WithMessage("MtrInput does not match the supplied signature");
            RuleFor(f => f.Dimension).NotNull();

        }
    }

    internal class Build_VLD_BuildReadNewPeriods : AbstractValidator<BuildUpdate_DTO>
    {
        internal Build_VLD_BuildReadNewPeriods()
        {

            RuleFor(x => x).Must(CustomValidations.PeriodsNotRepeated).WithMessage("One or more period codes have been repeated");
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval));
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval));
            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");

            RuleFor(f => f.Dimension).Must(CustomValidations.LanguagesUnique).When(f => f.Dimension != null).WithMessage("Non unique language");
            RuleFor(f => f).Must(CustomValidations.SignatureMatch).WithMessage("MtrInput does not match the supplied signature");
            RuleFor(f => f.Dimension).NotNull();
            RuleForEach(f => f.Dimension).SetValidator(new Dimension_VLD_UltraLite()).When(f => f.Dimension != null);
        }
    }


    internal class Build_VLD_BuildExistingPeriods : AbstractValidator<BuildUpdate_DTO>
    {
        internal Build_VLD_BuildExistingPeriods()
        {

            RuleFor(x => x).Must(CustomValidations.PeriodsNotRepeated).WithMessage("One or more period codes have been repeated");
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval));
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval));
            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");

            RuleFor(f => f).Must(CustomValidations.SignatureMatch).WithMessage("MtrInput does not match the supplied signature");
            RuleFor(f => f.Dimension).NotNull();

        }
    }

    /// <summary>
    /// Custom validations for  PxBuild
    /// </summary>
    internal static class CustomValidations
    {

        public static object Enumerations { get; private set; }

        internal static bool NotReservedWord(string strWord)
        {

            return (!Constants.C_SYSTEM_RESERVED_WORD().Contains(strWord.ToLower()));

        }

        internal static bool ReadDatasetHasEnoughParameters(RESTful_API api)
        {
            return api.parameters.Count >= Convert.ToInt32(Utility.GetCustomConfig("APP_REST_READ_DATASET_PARAMETER_COUNT"));
        }

        internal static bool ReadMetadataHasEnoughParameters(RESTful_API api)
        {
            return api.parameters.Count >= Convert.ToInt32(Utility.GetCustomConfig("APP_REST_READ_METADATA_PARAMETER_COUNT"));
        }

        internal static bool ReadCollectionHasEnoughParameters(RESTful_API api)
        {
            return api.parameters.Count >= Convert.ToInt32(Utility.GetCustomConfig("APP_REST_READ_COLLECTION_PARAMETER_COUNT"));
        }

        internal static bool FormatExistsReadDataset(RESTful_API api)
        {
            if (api.parameters.Count < Convert.ToInt32(Utility.GetCustomConfig("APP_REST_READ_DATASET_PARAMETER_COUNT"))) return false;
            Format_DTO_Read dto = new Format_DTO_Read() { FrmType = api.parameters[2], FrmVersion = api.parameters[3] == "" ? "none" : api.parameters[3] };
            return CustomValidations.FormatExists(dto);
        }



        internal static bool LanguageCode(RESTful_API api)
        {
            if (api.parameters.Count >= 5)
            {
                if (api.parameters[4].Length != 2) return false;
            }
            return true;
        }

        internal static bool BelowLimitDatapoints(Matrix matrix)
        {
            long points = matrix.MainSpec.Statistic.Count * matrix.MainSpec.Frequency.Period.Count;
            foreach (var cls in matrix.MainSpec.Classification)
            {
                points *= cls.Variable.Count;
            }

            return points <= Convert.ToInt32(Utility.GetCustomConfig("APP_PX_DATAPOINT_THRESHOLD"));
        }

        internal static bool ValidateIgnoreEscapeChars(string readString)
        {

            return Regex.IsMatch(Regex.Escape(readString), Utility.GetCustomConfig("APP_BUILD_REGEX_FORBIDDEN_CHARS"));

        }

        internal static bool FormatForReadPreMetadata(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.DOWNLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat)) return false;
            return true;

        }

        internal static bool FormatForReadMetadata(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.DOWNLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat)) return false;
            return true;

        }

        internal static bool FormatForReadPreDataset(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.DOWNLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.PX) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.CSV) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.XLSX)) return false;
            return true;

        }


        internal static bool FormatForReadDataset(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.DOWNLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.PX) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.CSV) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.XLSX) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.SDMX)) return false;

            bool exists;
            using (Format_BSO bso = new Format_BSO(new ADO("defaultConnection")))
            {
                exists = bso.Exists(dto);
            }
            return exists;


        }

        internal static bool FormatForBuildUpdate(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.UPLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.PX)) return false;
            return true;

        }

        internal static bool FormatForBuildCreate(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.UPLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.PX)) return false;
            return true;

        }

        internal static bool SignatureMatch(Build_DTO_ReadTemplate dto)
        {

            var signature = Utility.GetMD5(Utility.GetCustomConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(dto.GetSignatureDTO()));
            return (signature == dto.Signature);
        }
        internal static bool SignatureMatch(Build_DTO_Read dto)
        {

            var signature = Utility.GetMD5(Utility.GetCustomConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(dto.GetSignatureDTO()));
            return (signature == dto.Signature);
        }

        internal static bool SignatureMatch(dynamic dto)
        {
            var v = dto.GetSignatureDTO();
            var signature = Utility.GetMD5(Utility.GetCustomConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(dto.GetSignatureDTO()));
            return (signature == dto.Signature);
        }

        internal static bool CopyrightCodeExists(Build_DTO dto)
        {
            Copyright_BSO bso = new Copyright_BSO();
            Copyright_DTO_Create dtoCpr = bso.Read(dto.CprCode);
            return (dtoCpr.CprCode != null);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool PeriodsNotRepeated(dynamic dto)
        {
            foreach (var per in dto.Periods)
            {
                if (!CustomValidations.PeriodCodeNotRepeatedInList(dto.Periods)) return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool PeriodsNotRepeated(PxPeriodListDTO dto)
        {
            foreach (var per in dto.Periods)
            {
                if (!CustomValidations.PeriodCodeNotRepeatedInList(dto.Periods) || !CustomValidations.PeriodValueNotRepeatedInList(dto.Periods)) return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="periods"></param>
        /// <returns></returns>
        internal static bool PeriodCodeNotRepeatedInList(List<PeriodRecordDTO_Create> periods)
        {
            var query = from prds in periods
                        group prds by prds.Code into g
                        select new
                        {
                            code = g.First().Code,
                            codeCount = g.Count()
                        };
            var result = from pers in query where pers.codeCount > 1 select pers;
            return result.Count() == 0;
        }


        internal static bool PeriodValueNotRepeatedInList(List<PeriodRecordDTO_Create> periods)
        {
            var query = from prds in periods
                        group prds by prds.Value into g
                        select new
                        {
                            code = g.First().Value,
                            codeCount = g.Count()
                        };
            var result = from pers in query where pers.codeCount > 1 select pers;
            return result.Count() == 0;
        }
        /// <summary>
        /// Validates that a variable code is not repeated within a given classification
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool VariableCodeNotRepeated(IList<VariableRecordDTO_Create> dto)
        {
            var query = from vars in dto
                        group vars by vars.Code into g
                        select new
                        {
                            Vrb = g.First().Code,
                            VrbCount = g.Count()
                        };
            var result = from vars in query where vars.VrbCount > 1 select vars;
            return result.Count() == 0;
        }

        /// <summary>
        /// Validates that a variable value is not repeated within a given classification
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool VariableValueNotRepeated(IList<VariableRecordDTO_Create> dto)
        {
            var query = from vars in dto
                        group vars by vars.Value into g
                        select new
                        {
                            Vrb = g.First().Value,
                            VrbCount = g.Count()
                        };
            var result = from vars in query where vars.VrbCount > 1 select vars;
            return result.Count() == 0;
        }

        /// <summary>
        /// Validates that a statistic code is not repeated within a given dimension
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool StatisticCodeNotRepeated(IList<StatisticalRecordDTO_Create> dto)
        {
            if (dto == null) return true;

            var query = from vars in dto
                        group vars by vars.Code into g
                        select new
                        {
                            Stat = g.First().Code,
                            StatCount = g.Count()
                        };
            var result = from vars in query where vars.StatCount > 1 select vars;
            return result.Count() == 0;
        }
        /// <summary>
        /// Validates that a statistic value is not repeated within a given dimension
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool StatisticValueNotRepeated(IList<StatisticalRecordDTO_Create> dto)
        {
            if (dto == null) return true;

            var query = from vars in dto
                        group vars by vars.Value into g
                        select new
                        {
                            Stat = g.First().Value,
                            StatCount = g.Count()
                        };
            var result = from vars in query where vars.StatCount > 1 select vars;
            return result.Count() == 0;
        }
        /// <summary>
        /// Validates that a Classification code is not repeated within a given dimension
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool ClassificationCodeNotRepeated(IList<ClassificationRecordDTO_Create> dto)
        {
            var query = from vars in dto
                        group vars by vars.Code into g
                        select new
                        {
                            Clas = g.First().Code,
                            ClasCount = g.Count()
                        };
            var result = from vars in query where vars.ClasCount > 1 select vars;
            return result.Count() == 0;
        }

        /// <summary>
        /// Validates that a Classification code is not repeated within a given dimension
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool ClassificationValueNotRepeated(IList<ClassificationRecordDTO_Create> dto)
        {
            var query = from vars in dto
                        group vars by vars.Value into g
                        select new
                        {
                            Clas = g.First().Value,
                            ClasCount = g.Count()
                        };
            var result = from vars in query where vars.ClasCount > 1 select vars;
            return result.Count() == 0;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool ClassificationValueNotTheSameAsFrequencyValue(Dimension_DTO dto)
        {
            var query = from vars in dto.Classifications
                        group vars by vars.Code into g
                        select new
                        {
                            Clas = g.First().Value

                        };
            var result = from vars in query where vars.Clas.Equals(dto.FrqValue) select vars;
            return result.Count() == 0;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool StatisticsValueNotTheSameAsStatisticsLabel(Dimension_DTO dto)
        {
            var query = from vars in dto.Statistics
                        group vars by vars.Code into g
                        select new
                        {
                            Stat = g.First().Value

                        };
            var result = from vars in query where vars.Stat.Equals(dto.StatisticLabel) select vars;
            return result.Count() == 0;
        }

        /// <summary>
        /// Dimensions must not differ between different language versions apart from language specific details
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool DimensionsValid(IList<Dimension_DTO> dto)
        {
            if (dto.Count < 2) return true;

            List<Dimension_DTO> cloneList = dto.ToList();

            foreach (var d in dto)
            {

                foreach (var c in cloneList)
                {
                    if (!d.IsEquivalent(c)) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// A languaguage may not be represented in more than one dimension
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool LanguagesUnique(IList<Dimension_DTO> dto)
        {
            var query = from vars in dto
                        group vars by vars.LngIsoCode into g
                        select new
                        {
                            Language = g.First().LngIsoCode,
                            LanguageCount = g.Count()
                        };
            var result = from vars in query where vars.LanguageCount > 1 select vars;
            return result.Count() == 0;
        }

        /// <summary>
        /// The main language must be represented in one and only one dimension
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool MainLanguageRepresented(Build_DTO dto)
        {
            var query = from vars in dto.DimensionList where vars.LngIsoCode == dto.matrixDto.LngIsoCode select vars;

            return query.Count() == 1;
        }

        /// <summary>
        /// Checks if a format type and version is represented in the database
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool FormatExists(Format_DTO_Read dto)
        {
            bool exists = false;
            ADO Ado = new ADO("defaultconnection");
            try
            {
                Format_ADO adoFormat = new Format_ADO();
                Format_DTO_Read dtoFormat = new Format_DTO_Read();
                dtoFormat.FrmType = dto.FrmType;
                dtoFormat.FrmVersion = dto.FrmVersion;
                dtoFormat.FrmDirection = dto.FrmDirection;
                var result = adoFormat.Read(Ado, dtoFormat);
                exists = result.hasData;
            }
            catch
            {
                exists = false;
            }
            finally
            {
                Ado.Dispose();
            }

            return exists;

        }

        internal static bool FormatExists(dynamic dto)
        {
            bool exists = false;
            ADO Ado = new ADO("defaultconnection");
            try
            {
                Format_ADO adoFormat = new Format_ADO();
                Format_DTO_Read dtoFormat = new Format_DTO_Read();
                dtoFormat.FrmType = dto.Format.FrmType;
                dtoFormat.FrmVersion = dto.Format.FrmVersion;
                var result = adoFormat.Read(Ado, dtoFormat);
                exists = result.hasData;
            }
            catch
            {
                exists = false;
            }
            finally
            {
                Ado.Dispose();
            }

            return exists;

        }

        //Build_DTO_ReadTemplate



        internal static bool FrequencyCodeExists(dynamic dto)
        {
            Frequency_BSO bso = new Frequency_BSO();
            Frequency_DTO dtoFreq = bso.Read(dto.FrqCodeTimeval);
            return (dtoFreq.FrqCode != null);

        }


    }
}
