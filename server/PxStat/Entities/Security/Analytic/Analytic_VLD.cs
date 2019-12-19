using API;
using FluentValidation;
using System;

namespace PxStat.Security
{
    /// <summary>
    /// Note - The Analytic_VLD class is not visible externally
    /// </summary>
    internal class Analytic_VLD : AbstractValidator<Analytic_DTO>
    {

        internal Analytic_VLD()
        {
            //Mandatory - matrix
            RuleFor(x => x.matrix).NotEmpty().Length(1, 20);
            //Mandatory - NltMaskedIp
            // RuleFor(x => x.NltMaskedIp).Matches(Utility.GetCustomConfig("APP_REGEX_MASKED_IP"));
            //Optional - NltOs
            RuleFor(x => x.NltOs).NotEmpty().Length(1, 64).When(x => !string.IsNullOrEmpty(x.NltOs));
            //Optional - NltBrowser
            RuleFor(x => x.NltBrowser).Length(1, 64).When(x => !string.IsNullOrEmpty(x.NltBrowser));
            //Optional - NltReferer
            RuleFor(x => x.NltReferer).NotEmpty().Length(1, 256).When(x => !string.IsNullOrEmpty(x.NltReferer));
        }
    }

    /// <summary>
    /// This class is visible externally via the Analytic_API class
    /// </summary>
    internal class Analytic_VLD_Read : AbstractValidator<Analytic_DTO_Read>
    {
        internal Analytic_VLD_Read()
        {
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            //Optional - NltInternalNetworkMask
            RuleFor(x => x.NltInternalNetworkMask).Matches(Utility.GetCustomConfig("APP_REGEX_MASKED_IP")).When(x => !string.IsNullOrEmpty(x.NltInternalNetworkMask));
            //Optional - SbjCode
            //Optional - PrcCode
            //Optional - ExcludeInternal
        }
    }

    /// <summary>
    ///  Validator for ReadOs
    /// </summary>
    internal class Analytic_VLD_ReadOs : AbstractValidator<Analytic_DTO_Read>
    {
        internal Analytic_VLD_ReadOs()
        {
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            //Optional - MtrCode
            RuleFor(x => x.MtrCode).NotEmpty().Length(1, 20).When(x => !string.IsNullOrEmpty(x.MtrCode));
            //Optional - NltInternalNetworkMask
            //RuleFor(x => x.NltInternalNetworkMask).Length(1, 15).When(x => !string.IsNullOrEmpty(x.NltInternalNetworkMask));
            RuleFor(x => x.NltInternalNetworkMask).Matches(Utility.GetCustomConfig("APP_REGEX_MASKED_IP")).When(x => !string.IsNullOrEmpty(x.NltInternalNetworkMask));
            //Optional - SbjCode
            //Optional - PrcCode
        }
    }

    /// <summary>
    /// Validator for ReadBrowser
    /// </summary>
    internal class Analytic_VLD_ReadBrowser : AbstractValidator<Analytic_DTO_Read>
    {
        internal Analytic_VLD_ReadBrowser()
        {
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            //Optional - MtrCode
            RuleFor(x => x.MtrCode).NotEmpty().Length(1, 20).When(x => !string.IsNullOrEmpty(x.MtrCode));
            //Optional - NltInternalNetworkMask
            RuleFor(x => x.NltInternalNetworkMask).Matches(Utility.GetCustomConfig("APP_REGEX_MASKED_IP")).When(x => !string.IsNullOrEmpty(x.NltInternalNetworkMask));
            //Optional - SbjCode
            //Optional - PrcCode
        }
    }


    internal class Analytic_VLD_ReadFormat : AbstractValidator<Analytic_DTO_Read>
    {
        internal Analytic_VLD_ReadFormat()
        {
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            //Optional FrmType
            RuleFor(x => x.FrmType).Length(1, 32).When(x => !string.IsNullOrEmpty(x.FrmType));

            RuleFor(x => x.FrmVersion).Length(1, 32).When(x => !string.IsNullOrEmpty(x.FrmVersion));

            //FrmVersion must be empty when FrmType is empty
            RuleFor(x => x.FrmVersion).Empty().When(x => string.IsNullOrEmpty(x.FrmType));

        }
    }

    /// <summary>
    /// Validator for ReadTimeline
    /// </summary>
    internal class Analytic_VLD_ReadTimeline : AbstractValidator<Analytic_DTO_Read>
    {
        internal Analytic_VLD_ReadTimeline()
        {
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            //Optional - MtrCode
            RuleFor(x => x.MtrCode).NotEmpty().Length(1, 20).When(x => !string.IsNullOrEmpty(x.MtrCode));
            //Optional - NltInternalNetworkMask
            RuleFor(x => x.NltInternalNetworkMask).Matches(Utility.GetCustomConfig("APP_REGEX_MASKED_IP")).When(x => !string.IsNullOrEmpty(x.NltInternalNetworkMask));

            //Optional - SbjCode
            //Optional - PrcCode
        }
    }

    /// <summary>
    /// Validator for ReadReferrer
    /// </summary>
    internal class Analytic_VLD_ReadReferrer : AbstractValidator<Analytic_DTO_Read>
    {
        internal Analytic_VLD_ReadReferrer()
        {
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            //Optional - MtrCode
            RuleFor(x => x.MtrCode).NotEmpty().Length(1, 20).When(x => !string.IsNullOrEmpty(x.MtrCode));
            //Optional - NltInternalNetworkMask
            RuleFor(x => x.NltInternalNetworkMask).Matches(Utility.GetCustomConfig("APP_REGEX_MASKED_IP")).When(x => !string.IsNullOrEmpty(x.NltInternalNetworkMask));

            //Optional - SbjCode
            //Optional - PrcCode
        }
    }

    /// <summary>
    /// Validator for ReadLanguage
    /// </summary>
    internal class Analytic_VLD_ReadLanguage : AbstractValidator<Analytic_DTO_Read>
    {
        internal Analytic_VLD_ReadLanguage()
        {
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            //Optional - MtrCode
            RuleFor(x => x.MtrCode).NotEmpty().Length(1, 20).When(x => !string.IsNullOrEmpty(x.MtrCode));
            //Optional - NltInternalNetworkMask
            RuleFor(x => x.NltInternalNetworkMask).Matches(Utility.GetCustomConfig("APP_REGEX_MASKED_IP")).When(x => !string.IsNullOrEmpty(x.NltInternalNetworkMask));
            //Optional - LngIsoCode
            RuleFor(x => x.LngIsoCode).Length(2).When(x => !string.IsNullOrEmpty(x.LngIsoCode));
            //Optional - SbjCode
            //Optional - PrcCode
        }
    }
}
