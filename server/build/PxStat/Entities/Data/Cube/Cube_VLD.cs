
using API;
using FluentValidation;
using PxStat.Build;
using PxStat.DBuild;
using PxStat.Entities.DBuild;
using PxStat.Resources;
using PxStat.System.Settings;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using PxStat.Security;
using System.Net;

namespace PxStat.Data
{

    internal class Cube_VLD_ReadXlsx : AbstractValidator<Cube_DTO_Read>
    {
        public Cube_VLD_ReadXlsx()
        {
        }
    }

    /// <summary>
    /// Validator for Cube_DTO_Read
    /// </summary>
    public class Cube_VLD_Read : AbstractValidator<ICube_DTO_Read>
    {
        /// <summary>
        /// 
        /// </summary>
        public Cube_VLD_Read()
        {

            //Mandatory
            RuleFor(dto => dto.matrix)
                .NotEmpty()
                .WithMessage("Invalid matrix code")
                .WithName("matrixValidation");

            RuleFor(f => f.Format.FrmType).NotEmpty().WithMessage("FrmType is empty");
            RuleFor(f => f.Format.FrmVersion).NotEmpty().WithMessage("FrmVersion is empty");
            RuleFor(dto => dto.Format).Must(CustomValidations.FormatExists).WithMessage("Format does not exist");
        }
    }

    /// <summary>
    /// Validator for Cube_VLD_Read
    /// </summary>
    public class Cube_VLD_ReadMetadata : Cube_VLD_Read
    {
        public Cube_VLD_ReadMetadata() : base()
        {
            RuleFor(dto => dto.Format).Must(CustomValidations.FormatForReadMetadata);
        }
    }

    internal class Cube_VLD_ReadMetadataHEAD : AbstractValidator<Cube_DTO_ReadMatrixMetadata>
    {
        public Cube_VLD_ReadMetadataHEAD()
        {
        }
    }

    /// <summary>
    /// Validator for Cube_VLD_Read
    /// </summary>
    internal class Cube_VLD_ReadDataset : Cube_VLD_Read
    {
        public Cube_VLD_ReadDataset() : base()
        {
            RuleFor(dto => dto.Format).Must(CustomValidations.FormatForReadDataset).WithMessage("Invalid format parameters for ReadDataset");

        }
    }

    internal class Cube_VLD_ReadMatrixMetadata : AbstractValidator<Cube_DTO_ReadMatrixMetadata>
    {
        public Cube_VLD_ReadMatrixMetadata()
        {
            RuleFor(dto => dto.language.Length)
                .Equal(2)
                .When(dto => !string.IsNullOrEmpty(dto.language))
                .WithMessage("Invalid language code")
                .WithName("languageValidation");

            RuleFor(dto => dto.matrix)
                .NotNull().NotEmpty()
                .WithMessage("Invalid matrix code");

        }
    }

    /// <summary>
    /// Validator for Cube_DTO_ReadCollection
    /// </summary>
    public class Cube_VLD_ReadCollection : AbstractValidator<Cube_DTO_ReadCollection>
    {
        public Cube_VLD_ReadCollection()
        {
            //Optional
            RuleFor(dto => dto.language.Length)
                .Equal(2)
                .When(dto => !string.IsNullOrEmpty(dto.language))
                .WithMessage("Invalid language code")
                .WithName("languageValidation");
            RuleFor(dto => dto.product.Length)
                .GreaterThan(0)
                .When(dto => !string.IsNullOrEmpty(dto.product))
                .WithMessage("Invalid product code")
                .WithName("productValidation");
        }
    }


    /// <summary>
    /// Validator for Cube_DTO_ReadCollection
    /// </summary>
    internal class Cube_VLD_ReadCollectionSummary : AbstractValidator<Cube_DTO_ReadCollectionSummary>
    {
        public Cube_VLD_ReadCollectionSummary()
        {
            //Optional
            RuleFor(dto => dto.language.Length)
                .Equal(2)
                .When(dto => !string.IsNullOrEmpty(dto.language))
                .WithMessage("Invalid language code")
                .WithName("languageValidation");
            RuleFor(dto => dto.product.Length)
                .GreaterThan(0)
                .When(dto => !string.IsNullOrEmpty(dto.product))
                .WithMessage("Invalid product code")
                .WithName("productValidation");
        }
    }

    /// <summary>
    /// Validator for Cube_DTO_Read
    /// </summary>
    internal class Cube_VLD_ReadPre : AbstractValidator<ICube_DTO_Read>
    {
        public Cube_VLD_ReadPre() : base()
        {
            //Mandatory
            RuleFor(dto => dto.release)
                .NotEmpty()
                .WithMessage("Invalid release code")
                .WithName("releaseValidation");

            //Optional
            RuleFor(dto => dto.language.Length)
                .Equal(2)
                .When(dto => !string.IsNullOrEmpty(dto.language))
                .WithMessage("Invalid language code")
                .WithName("languageValidation");

            //Optional
            RuleFor(dto => dto.Format.FrmType)
                .Must((dto, format)
                    => format?.CompareTo(DatasetFormat.JsonStat) == 0
                    || format?.CompareTo(DatasetFormat.Px) == 0
                    || format?.CompareTo(DatasetFormat.Csv) == 0
                    | format?.CompareTo(DatasetFormat.Xlsx) == 0)
                .WithMessage("Invalid format")
                .WithName("formatValidation");
            RuleFor(f => f.Format.FrmType).NotEmpty();
            RuleFor(f => f.Format.FrmVersion).NotEmpty();
            RuleFor(dto => dto.Format).Must(CustomValidations.FormatExists);
            RuleFor(dto => dto.Format).Must(CustomValidations.FormatExists);

        }
    }

    /// <summary>
    /// Validator for ReadPreMetadata
    /// </summary>
    internal class Cube_VLD_ReadPreMetadata : Cube_VLD_ReadPre
    {
        public Cube_VLD_ReadPreMetadata() : base()
        {
            RuleFor(dto => dto.Format).Must(CustomValidations.FormatForReadPreMetadata);
        }
    }

    /// <summary>
    /// Validator for ReadPreDataset
    /// </summary>
    internal class Cube_VLD_ReadPreDataset : Cube_VLD_ReadPre
    {
        public Cube_VLD_ReadPreDataset() : base()
        {
            RuleFor(dto => dto.Format).Must(CustomValidations.FormatForReadPreDataset);
        }
    }

    internal class Cube_VLD_REST_ReadDataset : AbstractValidator<IRequest>
    {
        internal Cube_VLD_REST_ReadDataset()
        {
            RuleFor(x => x).Must(CustomValidations.ReadDatasetHasEnoughParameters).WithMessage("Not enough parameters in the RESTful request");
            RuleFor(x => x).Must(CustomValidations.FormatExistsReadDataset).WithMessage("Requested format not found");
            RuleFor(x => x).Must(CustomValidations.LanguageCode).WithMessage("Invalid language code");
        }


    }

    internal class Cube_VLD_REST_ReadMetadata : AbstractValidator<RESTful_API>
    {
        internal Cube_VLD_REST_ReadMetadata()
        {
            RuleFor(x => x).Must(CustomValidations.ReadMetadataHasEnoughParameters).WithMessage("Not enough parameters in the RESTful request");
            RuleFor(x => x).Must(CustomValidations.LanguageCode).WithMessage("Invalid language code");
        }


    }

    internal class Cube_VLD_REST_ReadCollection : AbstractValidator<RESTful_API>
    {
        internal Cube_VLD_REST_ReadCollection()
        {
            RuleFor(x => x).Must(CustomValidations.ReadCollectionHasEnoughParameters).WithMessage("Not enough parameters in the RESTful request");
            RuleFor(x => x).Must(CustomValidations.LanguageCode).WithMessage("Invalid language code");
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

        internal static bool ReadDatasetHasEnoughParameters(IRequest api)
        {
            return api.parameters.Count >= Convert.ToInt32(Configuration_BSO.GetStaticConfig("APP_REST_READ_DATASET_PARAMETER_COUNT"));
        }

        internal static bool ReadMetadataHasEnoughParameters(IRequest api)
        {
            return api.parameters.Count >= Convert.ToInt32(Configuration_BSO.GetStaticConfig("APP_REST_READ_METADATA_PARAMETER_COUNT"));
        }

        internal static bool ReadCollectionHasEnoughParameters(IRequest api)
        {
            return api.parameters.Count >= Convert.ToInt32(Configuration_BSO.GetStaticConfig("APP_REST_READ_COLLECTION_PARAMETER_COUNT"));
        }

        internal static bool FormatExistsReadDataset(IRequest api)
        {
            if (api.parameters.Count < Convert.ToInt32(Configuration_BSO.GetStaticConfig("APP_REST_READ_DATASET_PARAMETER_COUNT"))) return false;
            Format_DTO_Read dto = new Format_DTO_Read() { FrmType = api.parameters[2], FrmVersion = api.parameters[3] == "" ? "none" : api.parameters[3] };
            return CustomValidations.FormatExists(dto);
        }



        internal static bool LanguageCode(IRequest api)
        {
            if (api.parameters.Count >= 5)
            {
                if (api.parameters[4].Length != 2) return false;
            }
            return true;
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
            using (Format_BSO bso = new Format_BSO(AppServicesHelper.StaticADO))
            {
                exists = bso.Exists(dto);
            }
            return exists;


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





        /// <summary>
        /// Checks if a format type and version is represented in the database
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal static bool FormatExists(Format_DTO_Read dto)
        {
            bool exists = false;

            using (IADO fAdo = AppServicesHelper.StaticADO)
            {
                Format_ADO adoFormat = new Format_ADO();
                Format_DTO_Read dtoFormat = new Format_DTO_Read();
                dtoFormat.FrmType = dto.FrmType;
                dtoFormat.FrmVersion = dto.FrmVersion;
                dtoFormat.FrmDirection = dto.FrmDirection;
                var result = adoFormat.Read(fAdo, dtoFormat);
                exists = result.hasData;
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
