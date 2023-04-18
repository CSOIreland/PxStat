
using API;
using FluentValidation;
using PxStat.Build;

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
    internal class Cube_VLD_Read : AbstractValidator<ICube_DTO_Read>
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
    internal class Cube_VLD_ReadMetadata : Cube_VLD_Read
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
    internal class Cube_VLD_ReadCollection : AbstractValidator<Cube_DTO_ReadCollection>
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







}
