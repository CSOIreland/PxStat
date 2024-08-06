using API;
using FluentValidation;
using PxStat.Data;
using PxStat.Entities.DBuild;
using PxStat.Resources;
using PxStat.Security;
using PxStat.System.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PxStat.DBuild
{
    public class DBuild_VLD_UpdatePublish : AbstractValidator<DBuild_DTO_UpdatePublish>
    {
        public DBuild_VLD_UpdatePublish() 
        {
            RuleFor(x => x.MtrCode).Length(1, 20).WithMessage("Matrix code must not be empty");
            RuleFor(x => x.MtrCode).Matches((string)Configuration_BSO.GetStaticConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
            RuleFor(f => f.WrqDatetime).Must(f => !(f.Equals(default(DateTime)))).WithMessage("Invalid Request Date");
            RuleFor(f => f.Dspecs).NotNull();
            RuleFor(x => x).Must(NoSpecsHaveDuplicateLngIsoCodes).WithMessage(Label.Get("error.validation"));
        }

        public bool NoSpecsHaveDuplicateLngIsoCodes(DBuild_DTO_UpdatePublish dto)
        {
            if (dto.Dspecs == null) return false;
            if (dto.Dspecs.Count == 0) return false;
            bool isOk = true;
            List<string> lcodes = dto.Dspecs.Select(x => x.Language).ToList();
            foreach(var item in dto.Dspecs )
            {
                if (lcodes.Where(x => x.Equals(item.Language)).Count() > 1) isOk=false;
            }
            return isOk;
        }
    }

    public class DBuild_VLD_Update : AbstractValidator<DBuild_DTO_Update>
    {
        public DBuild_VLD_Update()
        {
            RuleFor(x => x.MtrInput).NotEmpty().WithMessage("Matrix data must not be empty");
            RuleFor(x => x.MtrCode).Length(1, 20).When(f => !string.IsNullOrEmpty(f.MtrCode)).WithMessage("Matrix code must not be empty");
            RuleFor(x => x.MtrCode).Matches((string)Configuration_BSO.GetStaticConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(x => x.CprCode).Length(1, 20).When(f => !string.IsNullOrEmpty(f.CprCode)).WithMessage("Copyright code must not be empty");
            RuleFor(x => x.CprCode).Matches((string)Configuration_BSO.GetStaticConfig("APP_REGEX_NO_WHITESPACE"));

            RuleFor(x => x).Must(CustomValidations.SpecsNotRepeated).WithMessage("One or spec structures have been repeated");
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval));
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval));
            RuleFor(f => f.FrqCodeTimeval).Matches((string)Configuration_BSO.GetStaticConfig("APP_REGEX_NO_WHITESPACE"));

            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");
            RuleFor(f => f).Must(CustomValidations.FormatExists).WithMessage("Requested format/version not found in the system");
            RuleFor(f => f).Must(CustomValidations.SignatureMatch).WithMessage("MtrInput does not match the supplied signature");
            RuleFor(f => f.Elimination).NotEmpty().WithMessage("Elimination object not supplied in request");
            RuleFor(f => f.Format.FrmType).NotEmpty();
            RuleFor(f => f.Format.FrmVersion).NotEmpty();
            RuleFor(f => f).Must(CustomValidations.FormatExists);

            RuleFor(dto => dto.Format).Must(CustomValidations.FormatForBuildUpdate);
            RuleForEach(f => f.Dspecs).SetValidator(new DSpec_VLD_Update());
        }

    }

    public class DBuild_VLD_UpdateByRelease : AbstractValidator<DBuild_DTO_UpdateByRelease>
    {
        public DBuild_VLD_UpdateByRelease()
        {
            RuleFor(x => x.CprCode).Length(1, 20).When(f => !string.IsNullOrEmpty(f.CprCode)).WithMessage("Copyright code must not be empty");
            RuleFor(x => x.CprCode).Matches((string)Configuration_BSO.GetStaticConfig("APP_REGEX_NO_WHITESPACE"));

            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval));
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval));
            RuleFor(f => f.FrqCodeTimeval).Matches((string)Configuration_BSO.GetStaticConfig("APP_REGEX_NO_WHITESPACE"));

            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");
            RuleFor(f => f).Must(CustomValidations.FormatExists).WithMessage("Requested format/version not found in the system");
            RuleFor(f => f.Elimination).NotEmpty().WithMessage("Elimination object not supplied in request");
            RuleFor(f => f.Format.FrmType).NotEmpty();
            RuleFor(f => f.Format.FrmVersion).NotEmpty();
            RuleFor(f => f).Must(CustomValidations.FormatExists);

            RuleFor(dto => dto.Format).Must(CustomValidations.FormatForBuildUpdate);
            RuleForEach(f => f.Dspecs).SetValidator(new DSpec_VLD_Update());
            RuleFor(f => f.RlsCode).NotEmpty().WithMessage("Release code must not be empty");
        }
    }

    public class DSpec_VLD_Update : AbstractValidator<DSpec_DTO>
    {
        public DSpec_VLD_Update()
        {
            
                RuleFor(f => f.MtrTitle).Must(CustomValidations.ValidateIgnoreEscapeChars).WithMessage("Invalid MtrTitle");
           

            RuleFor(f => f.MtrTitle).NotEmpty().Length(1, 256);
           
                RuleFor(f => f.ContentVariable).Must(CustomValidations.ValidateIgnoreEscapeChars).When(f => !string.IsNullOrEmpty(f.ContentVariable)).WithMessage("Invalid ContentVariable");


            RuleFor(x => x).Must(CustomValidations.PeriodsNotRepeated).WithMessage("Periods repeated in request");

            RuleForEach(f => f.StatDimensions).SetValidator(new StatDimension_VLD_Update());


        }

    }

    public class StatDimension_VLD_Update : AbstractValidator<StatDimension_DTO>
    {
        public StatDimension_VLD_Update()
        {
            //RuleFor(x => x.Code).NotNull().NotEmpty().WithMessage("Dimension code must not be empty");
            RuleFor(x => x.Value).NotNull().NotEmpty().WithMessage("Dimension value must not be empty");

            RuleForEach(x => x.Variables).SetValidator(new DimensionVariable_VLD_Update());
        }
    }

    //DimensionVariable_DTO
    public class DimensionVariable_VLD_Update : AbstractValidator<DimensionVariable_DTO>
    {
        public DimensionVariable_VLD_Update()
        {
            RuleFor(x => x.Code).NotNull().NotEmpty().WithMessage("Variable code must not be empty");
            RuleFor(x => x.Value).NotNull().NotEmpty().WithMessage("Variable value must not be empty");

        }

    }



    public static class CustomValidations
    {
        public static bool ValidateCsvHeader(DBuild_DTO_Update dto, IDmatrix dmatrix, List<string> headList)
        {
            if (dto.ChangeData == null) return true;
            if (dto.ChangeData.Count == 0) return true;
            List<string> codeDimensionList = dmatrix.Dspecs[dto.LngIsoCode].Dimensions.Select(x => x.Code).ToList();

            //header contains something that is not a dimension code for the matrix?
            foreach (string heading in headList)
            {
                if (!codeDimensionList.Contains(heading) && !heading.Equals(Configuration_BSO.GetStaticConfig("APP_CSV_VALUE")) && !heading.Equals(Configuration_BSO.GetStaticConfig("APP_CSV_UNIT")))
                {
                    Log.Instance.Error($"Invalid CSV header item: {heading}");
                    return false;
                }

            }

            //Duplicates in header?
            var groupedHeaders = headList.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            if (groupedHeaders.Count > 0)
            {
                Log.Instance.Error("One or more items repeated in the CSV header");
                return false;
            }

            return true;
        }

        public static bool ValidateCsvHeader(DBuild_DTO_UpdateByRelease dto, IDmatrix dmatrix, List<string> headList)
        {
            if (dto.ChangeData == null) return true;
            if (dto.ChangeData.Count == 0) return true;
            List<string> codeDimensionList = dmatrix.Dspecs[dto.LngIsoCode].Dimensions.Select(x => x.Code).ToList();

            //header contains something that is not a dimension code for the matrix?
            foreach (string heading in headList)
            {
                if (!codeDimensionList.Contains(heading) && !heading.Equals(Configuration_BSO.GetStaticConfig("APP_CSV_VALUE")) && !heading.Equals(Configuration_BSO.GetStaticConfig("APP_CSV_UNIT")))
                {
                    Log.Instance.Error($"Invalid CSV header item: {heading}");
                    return false;
                }

            }

            //Duplicates in header?
            var groupedHeaders = headList.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            if (groupedHeaders.Count > 0)
            {
                Log.Instance.Error("One or more items repeated in the CSV header");
                return false;
            }

            return true;
        }


        public static bool ValidateCsvHeader(DBuild_DTO_UpdatePublish dto, IDmatrix dmatrix, List<string> headList)
        {
            if (dto.ChangeData == null) return true;
            if (dto.ChangeData.Count == 0) return true;
            List<string> codeDimensionList = dmatrix.Dspecs[dto.LngIsoCode].Dimensions.Select(x => x.Code).ToList();

            //header contains something that is not a dimension code for the matrix?
            foreach (string heading in headList)
            {
                if (!codeDimensionList.Contains(heading) && !heading.Equals(Configuration_BSO.GetStaticConfig("APP_CSV_VALUE")) && !heading.Equals(Configuration_BSO.GetStaticConfig("APP_CSV_UNIT")))
                {
                    Log.Instance.Error($"Invalid CSV header item: {heading}");
                    return false;
                }

            }

            //Duplicates in header?
            var groupedHeaders = headList.GroupBy(x => x)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            if (groupedHeaders.Count > 0)
            {
                Log.Instance.Error("One or more items repeated in the CSV header");
                return false;
            }

            return true;
        }

        internal static bool FormatForBuildCreate(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.UPLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.PX)) return false;
            return true;

        }
        /// <summary>
        /// Checks if a format type and version is represented in the database
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool FormatExists(Format_DTO_Read dto)
        {
            bool exists = false;
            using (IADO ado = AppServicesHelper.StaticADO)

            {
                Format_ADO adoFormat = new Format_ADO();
                Format_DTO_Read dtoFormat = new Format_DTO_Read();
                dtoFormat.FrmType = dto.FrmType;
                dtoFormat.FrmVersion = dto.FrmVersion;
                dtoFormat.FrmDirection = dto.FrmDirection;
                var result = adoFormat.Read(ado, dtoFormat);
                exists = result.hasData;
            };


            return exists;

        }


        /// <summary>
        /// The main language must be represented in one and only one dimension
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool MainLanguageRepresented(DBuild_DTO_Create dto)
        {
            var query = from vars in dto.Dspecs where vars.Language == dto.LngIsoCode select vars;

            return query.Count() == 1;
        }
        /// <summary>
        /// A languaguage may not be represented in more than one spec
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool LanguagesUnique(IList<DSpec_DTO> dto)
        {
            var query = from vars in dto
                        group vars by vars.Language into g
                        select new
                        {
                            Language = g.First().Language,
                            LanguageCount = g.Count()
                        };
            var result = from vars in query where vars.LanguageCount > 1 select vars;
            return result.Count() == 0;
        }

        public static bool SpecsNotRepeated(DBuild_DTO_Update dto)
        {
            var spec = from specs in dto.Dspecs
                       group specs by specs.Language into g
                       select new
                       {
                           code = g.First().Language,
                           codeCount = g.Count()
                       };
            var result = from s in spec where s.codeCount > 1 select s;
            return result.Count() == 0;
        }

        internal static bool ValidateIgnoreEscapeChars(string readString)
        {
            if (readString == null) return true;

            return Regex.IsMatch(readString, Configuration_BSO.GetStaticConfig("APP_BUILD_REGEX_FORBIDDEN_CHARS"));

        }

        internal static bool FrequencyCodeExists(DBuild_DTO_Update dto)
        {
            Frequency_BSO bso = new Frequency_BSO();
            Frequency_DTO dtoFreq = bso.Read(dto.FrqCodeTimeval);
            return (dtoFreq.FrqCode != null);

        }
        internal static bool FrequencyCodeExists(DBuild_DTO_UpdateByRelease dto)
        {
            Frequency_BSO bso = new Frequency_BSO();
            Frequency_DTO dtoFreq = bso.Read(dto.FrqCodeTimeval);
            return (dtoFreq.FrqCode != null);

        }



        internal static bool FrequencyCodeExists(DBuild_DTO_ReadTemplate dto)
        {
            Frequency_BSO bso = new Frequency_BSO();
            Frequency_DTO dtoFreq = bso.Read(dto.FrqCodeTimeval);
            return (dtoFreq.FrqCode != null);
        }

        internal static bool FrequencyCodeExists(DBuild_DTO_ReadTemplateByRelease dto)
        {
            Frequency_BSO bso = new Frequency_BSO();
            Frequency_DTO dtoFreq = bso.Read(dto.FrqCodeTimeval);
            return (dtoFreq.FrqCode != null);
        }


        internal static bool FrequencyCodeExists(DBuild_DTO_Read dto)
        {
            Frequency_BSO bso = new Frequency_BSO();
            Frequency_DTO dtoFreq = bso.Read(dto.FrqCodeTimeval);
            return (dtoFreq.FrqCode != null);

        }

        internal static bool FormatExists(DBuild_DTO_Update dto)
        {
            bool exists = false;
            using (IADO Ado = AppServicesHelper.StaticADO)
            {
                Format_ADO adoFormat = new Format_ADO();
                Format_DTO_Read dtoFormat = new Format_DTO_Read();
                dtoFormat.FrmType = dto.Format.FrmType;
                dtoFormat.FrmVersion = dto.Format.FrmVersion;
                var result = adoFormat.Read(Ado, dtoFormat);
                exists = result.hasData;
            };
            return exists;
        }

        internal static bool FormatExists(DBuild_DTO_UpdateByRelease dto)
        {
            bool exists = false;
            using (IADO Ado = AppServicesHelper.StaticADO)
            {
                Format_ADO adoFormat = new Format_ADO();
                Format_DTO_Read dtoFormat = new Format_DTO_Read();
                dtoFormat.FrmType = dto.Format.FrmType;
                dtoFormat.FrmVersion = dto.Format.FrmVersion;
                var result = adoFormat.Read(Ado, dtoFormat);
                exists = result.hasData;
            };
            return exists;
        }


        internal static bool SignatureMatch(DBuild_DTO_Update dto)
        {
            var signature = Utility.GetMD5(Configuration_BSO.GetStaticConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(dto.GetSignatureDTO()));
            return (signature == dto.Signature);
        }

        internal static bool SignatureMatch(DBuild_DTO_ReadTemplate dto)
        {
            var signature = Utility.GetMD5(Configuration_BSO.GetStaticConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(dto.GetSignatureDTO()));
            return (signature == dto.Signature);
        }
        //

        internal static bool SignatureMatch(DBuild_DTO_Read dto)
        {
            var signature = Utility.GetMD5(Configuration_BSO.GetStaticConfig("APP_SALSA") + Utility.JsonSerialize_IgnoreLoopingReference(dto.GetSignatureDTO()));
            return (signature == dto.Signature);
        }

        internal static bool FormatForBuildUpdate(Format_DTO_Read dto)
        {
            if (dto.FrmDirection != Format_DTO_Read.FormatDirection.UPLOAD.ToString()) return false;
            if (dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.JSONstat) && dto.FrmType != EnumInfo.GetEnumDescription(Format_DTO_Read.FormatType.PX)) return false;
            return true;

        }

        internal static bool PeriodsNotRepeated(DSpec_DTO dto)
        {
            List<StatDimension_DTO> timeDimension = dto.StatDimensions.Where(x => x.Role.Equals(Constants.C_DATA_DIMENSION_ROLE_TIME))?.ToList();
            if (timeDimension == null) return false;
            if (timeDimension.Count != 1) return false;

            var query = from prds in timeDimension[0].Variables
                        group prds by prds.Code into g
                        select new
                        {
                            code = g.First().Code,
                            codeCount = g.Count()
                        };
            var result = from pers in query where pers.codeCount > 1 select pers;
            return result.Count() == 0;
        }

        internal static bool OneAndOnlyOnePeriodDimension(DSpec_DTO dto)
        {
            return true;
        }

        /// <summary>
        /// Specs must not differ between different language versions apart from language specific details
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool SpecsAreValid(IList<DSpec_DTO> dto)
        {
            if (dto.Count < 2) return true;

            List<DSpec_DTO> cloneList = dto.ToList();

            foreach (var d in dto)
            {

                foreach (var c in cloneList)
                {
                    if (!d.IsEquivalent(c)) return false;
                }
            }

            return true;
        }
    }

    public class DBuild_VLD_Validate : AbstractValidator<DBuild_DTO_Validate>
    {
        public DBuild_VLD_Validate()
        { }
    }

    /// <summary>
    /// 
    /// </summary>
    internal class DBuild_Read_VLD : AbstractValidator<DBuild_DTO_Read>
    {
        /// <summary>
        /// 
        /// </summary>
        internal DBuild_Read_VLD()
        {
            RuleFor(f => f.MtrInput).NotEmpty().WithMessage("MtrInput must not be empty");
            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval)).WithMessage("You must supply a FrqCode with a FrqValue");
            RuleFor(f => f).Must(CustomValidations.SignatureMatch).WithMessage("MtrInput does not match the supplied signature");
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("You must supply a FrqValue with a FrqCode");
        }
    }

    internal class DBuild_VLD_BuildReadDataset : AbstractValidator<DBuild_DTO_Update>
    {
        internal DBuild_VLD_BuildReadDataset()
        {
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval));
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval));
            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");

            RuleFor(f => f.Dspecs).Must(CustomValidations.LanguagesUnique).When(f => f.Dspecs != null).WithMessage("Non unique language");
            RuleFor(f => f).Must(CustomValidations.SignatureMatch).WithMessage("MtrInput does not match the supplied signature");
            RuleFor(f => f.Dspecs).NotNull();
            RuleForEach(f => f.Dspecs).SetValidator(new Dimension_VLD_UltraLite()).When(f => f.Dspecs != null);
        }
    }

    internal class DBuild_ReadDatasetByRelease_VLD : AbstractValidator<DBuild_DTO_UpdateByRelease>
    {
        /// <summary>
        /// 
        /// </summary>
        internal DBuild_ReadDatasetByRelease_VLD()
        {
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval));
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval));
            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");

            RuleFor(f => f.Dspecs).Must(CustomValidations.LanguagesUnique).When(f => f.Dspecs != null).WithMessage("Non unique language");
            RuleFor(f => f.Dspecs).NotNull();
            RuleForEach(f => f.Dspecs).SetValidator(new Dimension_VLD_UltraLite()).When(f => f.Dspecs != null);
        }
    }

    internal class Dimension_VLD_UltraLite : AbstractValidator<DSpec_DTO>
    {
        internal Dimension_VLD_UltraLite()
        {
            //RuleFor(f => f.Frequency.Period).NotNull();

            RuleForEach(f => f.StatDimensions).SetValidator(new StatDimension_VLD_Update());
        }
    }

    internal class DBuild_VLD_ReadTemplate : AbstractValidator<DBuild_DTO_ReadTemplate>
    {
        /// <summary>
        /// 
        /// </summary>
        internal DBuild_VLD_ReadTemplate()
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

    internal class DBuild_VLD_ReadTemplateByRelease : AbstractValidator<DBuild_DTO_ReadTemplateByRelease>
    {
        /// <summary>
        /// 
        /// </summary>
        internal DBuild_VLD_ReadTemplateByRelease()
        {
            //Required
            RuleFor(f => f.RlsCode).NotEmpty().WithMessage("Release code must not be empty");
            //Optional
            //Optional - LngIsoCode
            RuleFor(f => f.LngIsoCode.Length).Equal(2).When(f => !string.IsNullOrEmpty(f.LngIsoCode)).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
            RuleFor(f => f.FrqValueTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqValueTimeval));
            RuleFor(f => f.FrqCodeTimeval).Length(1, 256).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval));
            RuleFor(f => f).Must(CustomValidations.FrequencyCodeExists).When(f => !string.IsNullOrEmpty(f.FrqCodeTimeval)).WithMessage("Invalid Frequency Code");
        }
    }

    public class DBuild_VLD_Create : AbstractValidator<DBuild_DTO_Create>
    {
        /// <summary>
        /// Top level validator
        /// </summary>
        public DBuild_VLD_Create()
        {
            RuleFor(f => f.MtrCode).NotEmpty().Length(1, 20);
            RuleFor(f => f.MtrCode).Matches((string)Configuration_BSO.GetStaticConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(f => f.CprCode).NotEmpty().Length(1, 32);
            RuleFor(f => f.CprCode).Matches((string)Configuration_BSO.GetStaticConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(f => f.FrqCode).NotEmpty().Length(1, 256);
            RuleFor(f => f.FrqCode).Matches((string)Configuration_BSO.GetStaticConfig("APP_REGEX_NO_WHITESPACE"));
            RuleFor(f => f.LngIsoCode).NotEmpty().Matches("^[a-z]{2}$");
            RuleFor(f => f.Elimination).NotEmpty().WithMessage("Elimination object not supplied in request");

            RuleFor(f => f.Dspecs).Must(CustomValidations.SpecsAreValid).WithMessage("Invalid Dimensions");
            RuleFor(f => f.Dspecs).Must(CustomValidations.LanguagesUnique).WithMessage("Non unique language");
            RuleFor(f => f).Must(CustomValidations.MainLanguageRepresented).WithMessage("Main language not contained in any dimension");
            RuleForEach(f => f.Dspecs).SetValidator(x => new DSpec_VLD_Update());
            RuleFor(f => f.Format.FrmType).NotEmpty().Length(1, 32);
            RuleFor(f => f.Format.FrmVersion).NotEmpty().Length(1, 32);
           
            RuleFor(f => f.Format).NotEmpty();



            RuleFor(dto => dto.Format).Must(CustomValidations.FormatForBuildCreate);

        }
    }
}
