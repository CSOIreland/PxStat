using FluentValidation;

namespace PxStat.Security
{
    /// <summary>
    /// Validator for Database read
    /// </summary>

    internal class Database_VLD_Read : AbstractValidator<Database_DTO_Read>
    {
        internal Database_VLD_Read()
        {
            RuleFor(x => x.TableName).Length(1, 128);
        }
    }
}
