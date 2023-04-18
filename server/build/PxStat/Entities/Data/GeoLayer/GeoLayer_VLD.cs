using FluentValidation;

namespace PxStat.Data
{
    internal class GeoLayer_VLD_Create : AbstractValidator<GeoLayer_DTO_Create>
    {
        internal GeoLayer_VLD_Create()
        {
            RuleFor(x => x.GlrName).Length(0, 256).WithMessage("GlrName invalid length");
        }

    }

    internal class GeoLayer_VLD_Read : AbstractValidator<GeoLayer_DTO_Read>
    {
        internal GeoLayer_VLD_Read()
        {
            RuleFor(x => x.GlrCode).Length(0, 256).When(x => !string.IsNullOrEmpty(x.GlrCode)).WithMessage("Invalid GlrCode");
        }
    }

    internal class GeoLayer_VLD_Update : AbstractValidator<GeoLayer_DTO_Update>
    {
        internal GeoLayer_VLD_Update()
        {
            RuleFor(x => x.GlrName).Length(0, 256).WithMessage("GlrName invalid length");
            RuleFor(x => x.GlrCode).Length(0, 256).WithMessage("GlrCode invalid length");
        }
    }

    internal class GeoLayer_VLD_Delete : AbstractValidator<GeoLayer_DTO_Delete>
    {
        internal GeoLayer_VLD_Delete()
        {
            RuleFor(x => x.GlrCode).Length(0, 256).WithMessage("GlrCode invalid length");
        }
    }
}
