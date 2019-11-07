using System;
using System.Collections.Generic;
using FluentValidation;

namespace PxStat.Security
{
    /// <summary>
    /// Validation for Trace Read
    /// </summary>
    internal class Trace_VLD_Read : AbstractValidator<Trace_DTO_Read>
    {

        internal Trace_VLD_Read()
        {
            List<string> typeList = Resources.Constants.C_SECURITY_TRACE_TYPE();

            //Mandatory - StartDate
            RuleFor(f => f.StartDate).Must(f => !(f.Equals(default(DateTime)))).WithMessage("Invalid Start Date").WithName("StartDateValidation");
            //Mandatory - EndDate
            RuleFor(f => f.EndDate).Must(f => !(f.Equals(default(DateTime)))).WithMessage("Invalid End Date").WithName("EndDateValidation");
            //Optional - AuthenticationType
            RuleFor(f => f.AuthenticationType).Must(f => (typeList.Contains(f))).When(f => !string.IsNullOrEmpty(f.AuthenticationType)).WithMessage("Invalid Authentication Type").WithName("AuthenticationTypeValidation");
            //Optional - TrcUsername
            RuleFor(f => f.TrcUsername).Length(1, 256).When(f => !string.IsNullOrEmpty(f.TrcUsername)).WithMessage("Invalid TrcUsername");
            //Optional - TrcIp
            RuleFor(f => f.TrcIp).Length(7, 15).When(f => !string.IsNullOrEmpty(f.TrcIp)).WithMessage("Invalid TrcIp");
        }
    }

    /// <summary>
    /// Validation for Trace Read Type
    /// </summary>
    internal class Trace_VLD_ReadType : AbstractValidator<Trace_DTO_Read>
    {
        internal Trace_VLD_ReadType()
        {
            //No parameters are passed hence no validation takes place
        }
    }
}