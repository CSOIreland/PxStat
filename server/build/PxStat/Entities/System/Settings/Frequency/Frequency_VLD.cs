
using FluentValidation;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Validator for Frequency Read
    /// </summary>
    internal class Frequency_VLD_Read : AbstractValidator<Frequency_DTO>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        internal Frequency_VLD_Read() { }
    }
}