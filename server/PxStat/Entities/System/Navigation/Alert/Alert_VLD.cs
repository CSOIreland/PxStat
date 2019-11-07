using FluentValidation;
using System;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Validator for Alert Read
    /// </summary>
    internal class Alert_VLD_Read : AbstractValidator<Alert_DTO>
    {
        internal Alert_VLD_Read()
        {
            //Optional - LrtCode
            RuleFor(f => f.LrtCode).GreaterThan(0).When(f => f.LrtCode != default(int));
        }
    }

    /// <summary>
    /// Validator for Alert Create
    /// </summary>
    internal class Alert_VLD_Create : AbstractValidator<Alert_DTO>
    {
        internal Alert_VLD_Create()
        {
            //Mandatory - LrtMessage
            RuleFor(f => f.LrtMessage).NotEmpty().Length(1, 1024);
            //Mandatory - LrtDateTime 
            RuleFor(f => f.LrtDatetime).Must(f => !(f.Equals(default(DateTime))));
            //Optional - will default to Default language if not supplied
            RuleFor(f => f.LngIsoCode.Length).Equal(2).WithMessage("Invalid ISO code").WithName("LanguageIsoCodeValidation");
        }
    }

    /// <summary>
    /// Validator for Alert Update
    /// </summary>
    internal class Alert_VLD_Update : AbstractValidator<Alert_DTO>
    {
        internal Alert_VLD_Update()
        {
            //Mandatory - LrtCode
            RuleFor(f => f.LrtCode).NotEmpty();
            //Mandatory - LrtMessage
            RuleFor(f => f.LrtMessage).NotEmpty().Length(1, 1024);
            //Mandatory - LrtDateTime 
            RuleFor(f => f.LrtDatetime).Must(f => !(f.Equals(default(DateTime))));

        }
    }

    /// <summary>
    /// Validator for Alert Delete
    /// </summary>
    internal class Alert_VLD_Delete : AbstractValidator<Alert_DTO>
    {
        internal Alert_VLD_Delete()
        {
            //Mandatory - LrtCode
            RuleFor(f => f.LrtCode).NotEmpty();
        }
    }
}