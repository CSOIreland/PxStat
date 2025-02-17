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
            // RuleFor(x => x.NltMaskedIp).Matches(Configuration_BSO.GetApplicationConfigItem("APP_REGEX_MASKED_IP"));
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
            string maskedIp = Configuration_BSO.GetStaticConfig("APP_REGEX_MASKED_IP");
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom);
            RuleFor(x => x.DateTo).LessThanOrEqualTo(DateTime.Now.Date.AddDays(-1));
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
            string maskedIp = Configuration_BSO.GetStaticConfig("APP_REGEX_MASKED_IP");
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom);
            RuleFor(x => x.DateTo).LessThanOrEqualTo(DateTime.Now.Date.AddDays(-1));
        
            
        }
    }

    internal class Analytic_VLD_ReadEnvironmentLanguage : AbstractValidator<Analytic_DTO_Read>
    {
        internal Analytic_VLD_ReadEnvironmentLanguage()
        {
            string maskedIp = Configuration_BSO.GetStaticConfig("APP_REGEX_MASKED_IP");
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom);
            RuleFor(x => x.DateTo).LessThanOrEqualTo(DateTime.Now.Date.AddDays(-1));
            //Optional - MtrCode
            RuleFor(x => x.MtrCode).NotEmpty().Length(1, 20).When(x => !string.IsNullOrEmpty(x.MtrCode));
            //Optional - SbjCode
            //Optional - PrcCode
            //Mandatory LngIsoCode
            RuleFor(x => x.LngIsoCode).NotEmpty().Length(2);
        }
    }


    /// <summary>
    /// Validator for ReadBrowser
    /// </summary>
    internal class Analytic_VLD_ReadBrowser : AbstractValidator<Analytic_DTO_Read>
    {
        internal Analytic_VLD_ReadBrowser()
        {
            string maskedIp = Configuration_BSO.GetStaticConfig("APP_REGEX_MASKED_IP");
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom);
            RuleFor(x => x.DateTo).LessThanOrEqualTo(DateTime.Now.Date.AddDays(-1));
            
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
            RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom);
            RuleFor(x => x.DateTo).LessThanOrEqualTo(DateTime.Now.Date.AddDays(-1));
           

        }
    }

    /// <summary>
    /// Validator for ReadTimeline
    /// </summary>
    internal class Analytic_VLD_ReadTimeline : AbstractValidator<Analytic_DTO_Read>
    {
        internal Analytic_VLD_ReadTimeline()
        {
            string maskedIp = Configuration_BSO.GetStaticConfig("APP_REGEX_MASKED_IP");
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom);
            RuleFor(x => x.DateTo).LessThanOrEqualTo(DateTime.Now.Date.AddDays(-1));
            //Optional - MtrCode
            RuleFor(x => x.MtrCode).NotEmpty().Length(1, 20).When(x => !string.IsNullOrEmpty(x.MtrCode));

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
            string maskedIp = Configuration_BSO.GetStaticConfig("APP_REGEX_MASKED_IP");
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom);
            RuleFor(x => x.DateTo).LessThanOrEqualTo(DateTime.Now.Date.AddDays(-1));
            
        }
    }

    /// <summary>
    /// Validator for ReadLanguage
    /// </summary>
    internal class Analytic_VLD_ReadLanguage : AbstractValidator<Analytic_DTO_Read>
    {
        internal Analytic_VLD_ReadLanguage()
        {
            string maskedIp = Configuration_BSO.GetStaticConfig("APP_REGEX_MASKED_IP");
            //Mandatory - DateFrom
            RuleFor(x => x.DateFrom).NotEqual(default(DateTime));
            //Mandatory - DateTo
            RuleFor(x => x.DateTo).NotEqual(default(DateTime));
            RuleFor(x => x.DateTo).GreaterThanOrEqualTo(x => x.DateFrom);
            RuleFor(x => x.DateTo).LessThanOrEqualTo(DateTime.Now.Date.AddDays(-1));
      
        }
    }
}
