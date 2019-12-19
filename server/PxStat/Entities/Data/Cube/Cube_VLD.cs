
using FluentValidation;
using PxStat.Build;

namespace PxStat.Data
{
    /// <summary>
    /// Validator for Cube_DTO_Read
    /// </summary>
    internal class Cube_VLD_Read : AbstractValidator<Cube_DTO_Read>
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
            //Optional
            //RuleFor(dto => dto.language.Length)
            //    .Equal(2)
            //    .When(dto => !string.IsNullOrEmpty(dto.language))
            //    .WithMessage("Invalid language code")
            //    .WithName("languageValidation");
            ////Optional
            //RuleFor(dto => dto.Format.FrmType)
            //    .Must((dto, format)
            //        => format?.CompareTo(DatasetFormat.JsonStat) == 0
            //        || format?.CompareTo(DatasetFormat.Px) == 0
            //        || format?.CompareTo(DatasetFormat.Csv) == 0)
            //    .WithMessage("Invalid format")
            //    .WithName("formatValidation");
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
        }
    }

    /// <summary>
    /// Validator for Cube_DTO_Read
    /// </summary>
    internal class Cube_VLD_ReadPre : AbstractValidator<Cube_DTO_Read>
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
                    || format?.CompareTo(DatasetFormat.Csv) == 0)
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
}
