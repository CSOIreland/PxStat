using FluentValidation;

namespace PxStat.Security.Logging
{
    /// <summary>
    /// Validator for logging read
    /// </summary>
    internal class Logging_VLD_Read : AbstractValidator<Logging_DTO>
    {
        internal Logging_VLD_Read()
        {
            //Mandatory
            RuleFor(x => x.LggDatetimeStart).NotEmpty();
            //Mandatory
            RuleFor(x => x.LggDatetimeEnd).NotEmpty();
        }
    }
}