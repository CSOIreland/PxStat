using FluentValidation;

namespace PxStat.Subscription
{
    internal class Query_VLD_Create : AbstractValidator<Query_DTO_Create>
    {
        internal Query_VLD_Create()
        {
            RuleFor(x => x.TagName).NotNull().NotEmpty().WithMessage("Empty or null TagName");
        }
    }

    internal class Query_VLD_Delete : AbstractValidator<Query_DTO_Delete>
    {
        internal Query_VLD_Delete()
        {
        }
    }

    internal class Query_VLD_Read : AbstractValidator<Query_DTO_Read>
    {
        internal Query_VLD_Read()
        {
            RuleFor(x => x.Uid.Length).GreaterThan(2).When(x => !string.IsNullOrEmpty(x.Uid)).WithMessage("Invalid Uid");
        }
    }
}
