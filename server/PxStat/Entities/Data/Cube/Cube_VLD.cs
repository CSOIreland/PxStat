
using FluentValidation;

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
            int x = 1;
            //Mandatory
            RuleFor(dto => dto.matrix)
                .NotEmpty()
                .WithMessage("Invalid matrix code")
                .WithName("matrixValidation");
            //Optional
            RuleFor(dto => dto.language.Length)
                .Equal(2)
                .When(dto => !string.IsNullOrEmpty(dto.language))
                .WithMessage("Invalid language code")
                .WithName("languageValidation");
            //Optional
            RuleFor(dto => dto.format)
                .Must((dto, format)
                    => format?.CompareTo(DatasetFormat.JsonStat) == 0
                    || format?.CompareTo(DatasetFormat.Px) == 0
                    || format?.CompareTo(DatasetFormat.Csv) == 0)
                .When(dto => !string.IsNullOrEmpty(dto.format))
                .WithMessage("Invalid format")
                .WithName("formatValidation");
        }
    }

    /// <summary>
    /// Validator for Cube_VLD_Read
    /// </summary>
    internal class Cube_VLD_ReadMetadata : Cube_VLD_Read
    {
        public Cube_VLD_ReadMetadata() : base()
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
            RuleFor(dto => dto.format)
                .Must((dto, format)
                    => format?.CompareTo(DatasetFormat.JsonStat) == 0
                    || format?.CompareTo(DatasetFormat.Px) == 0
                    || format?.CompareTo(DatasetFormat.Csv) == 0)
                .When(dto => !string.IsNullOrEmpty(dto.format))
                .WithMessage("Invalid format")
                .WithName("formatValidation");
        }
    }

    /// <summary>
    /// Validator for ReadPreMetadata
    /// </summary>
    internal class Cube_VLD_ReadPreMetadata : Cube_VLD_ReadPre
    {
        public Cube_VLD_ReadPreMetadata() : base()
        {

        }
    }

    /// <summary>
    /// Validator for ReadPreDataset
    /// </summary>
    internal class Cube_VLD_ReadPreDataset : Cube_VLD_ReadPre
    {
        public Cube_VLD_ReadPreDataset() : base()
        {
        }
    }
}
