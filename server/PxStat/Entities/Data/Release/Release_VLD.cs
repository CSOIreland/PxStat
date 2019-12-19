
using FluentValidation;

namespace PxStat.Data
{
    /// <summary>
    /// Validator class for Release_DTO
    /// </summary>
    internal class ReleaseValidator : AbstractValidator<Release_DTO>
    {

        public ReleaseValidator()
        {
            //Mandatory - GrpCode
            RuleFor(x => x.GrpCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator class for Release_DTO_Read Read
    /// </summary>
    internal class Release_VLD_Read : AbstractValidator<Release_DTO_Read>
    {

        public Release_VLD_Read()
        {
            //Mandatory - RlsCode
            RuleFor(x => x.RlsCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator class for Read List
    /// </summary>
    internal class Release_VLD_ReadList : AbstractValidator<Release_DTO_Read>
    {
        public Release_VLD_ReadList()
        {
            //Mandatory - MtrCode
            RuleFor(x => x.MtrCode).NotEmpty();
            //Optional LngIsoCode
            RuleFor(x => x.LngIsoCode).NotEmpty().Length(2);
        }
    }

    /// <summary>
    /// Validator class for ReadListByProduct
    /// </summary>
    internal class Release_VLD_ReadListByProduct : AbstractValidator<Release_DTO_Read>
    {
        public Release_VLD_ReadListByProduct()
        {
            //Mandatory - PrcCode
            RuleFor(x => x.PrcCode).NotEmpty();
            RuleFor(x => x.LngIsoCode).NotEmpty().Length(2);
        }
    }

    /// <summary>
    /// Validator class for Release Delete
    /// </summary>
    internal class Release_VLD_Delete : AbstractValidator<Release_DTO_Delete>
    {
        public Release_VLD_Delete()
        {
            //Mandatory - RlsCode
            RuleFor(x => x.RlsCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator class for UpdateAnalyticalFlag
    /// </summary>
    internal class Release_VLD_UpdateAnalyticalFlag : AbstractValidator<Release_DTO_Update>
    {
        public Release_VLD_UpdateAnalyticalFlag()
        {
            //Mandatory - RlsCode
            RuleFor(x => x.RlsCode).NotEmpty();
            //Mandatory - RlsAnalyticalFlag
            RuleFor(x => x.RlsAnalyticalFlag).NotNull();
        }
    }

    /// <summary>
    /// Validator class for UpdateDependencyFlag
    /// </summary>
    internal class Release_VLD_UpdateDependencyFlag : AbstractValidator<Release_DTO_Update>
    {
        public Release_VLD_UpdateDependencyFlag()
        {
            //Mandatory - RlsCode
            RuleFor(x => x.RlsCode).NotEmpty();
            //Mandatory - RlsDependencyFlag
            RuleFor(x => x.RlsDependencyFlag).NotNull();
        }
    }

    /// <summary>
    /// Validator class for UpdateProduct
    /// </summary>
    internal class Release_VLD_Product : AbstractValidator<Release_DTO_Update>
    {
        public Release_VLD_Product()
        {
            //Mandatory - RlsCode
            RuleFor(x => x.RlsCode).NotEmpty();
            //Mandatory - PrcCode
            RuleFor(x => x.PrcCode).NotEmpty();
        }
    }

    /// <summary>
    /// Validator class for UpdateComment
    /// </summary>
    internal class Release_VLD_UpdateComment : AbstractValidator<Release_DTO_Update>
    {
        public Release_VLD_UpdateComment()
        {
            //Mandatory - RlsCode
            RuleFor(x => x.RlsCode).NotEmpty();
            //Mandatory - CmmValue
            RuleFor(x => x.CmmValue).Length(1, 1024);
        }
    }

    /// <summary>
    /// Validator class for DeleteComment
    /// </summary>
    internal class Release_VLD_DeleteComment : AbstractValidator<Release_DTO_Update>
    {
        public Release_VLD_DeleteComment()
        {
            //Mandatory - RlsCode
            RuleFor(x => x.RlsCode).NotEmpty();
        }
    }
}
