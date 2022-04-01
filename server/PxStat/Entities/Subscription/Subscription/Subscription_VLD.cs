using FluentValidation;

namespace PxStat.Subscription
{
    internal class Subscription_VLD_ChannelSubscriptionCreate : AbstractValidator<Subscription_DTO_ChannelSubscriptionCreate>
    {
        internal Subscription_VLD_ChannelSubscriptionCreate()
        {
            RuleFor(x => x.ChnCode).NotNull().NotEmpty().WithMessage("Invalid Channel Code");
        }
    }
    internal class Subscription_VLD_TableSubscriptionCreate : AbstractValidator<Subscription_DTO_TableSubscriptionCreate>
    {
        internal Subscription_VLD_TableSubscriptionCreate()
        {
            RuleFor(x => x.TsbTable).NotEmpty().NotNull().WithMessage("Invalid Table Name");
        }
    }

    internal class Subscription_VLD_ChannelSubscriptionDelete : AbstractValidator<Subscription_DTO_ChannelSubscriptionDelete>
    {
        internal Subscription_VLD_ChannelSubscriptionDelete()
        {
            RuleFor(x => x.ChnCode).NotNull().NotEmpty().WithMessage("Invalid Channel Code");
        }
    }

    internal class Subscription_VLD_ChannelSubscriptionReadCurrent : AbstractValidator<Subscription_DTO_ChannelSubscriptionReadCurrent>
    {
        internal Subscription_VLD_ChannelSubscriptionReadCurrent()
        {
            RuleFor(x => x.LngIsoCode.Length).Equal(2).When(x => !string.IsNullOrEmpty(x.LngIsoCode)).WithMessage("Invalid LngIsoCode");
        }
    }

    internal class Subscription_VLD_ChannelSubscriptionRead : AbstractValidator<Subscription_DTO_ChannelSubscriptionRead>
    {
        internal Subscription_VLD_ChannelSubscriptionRead()
        {
            RuleFor(x => x.ChnCode).NotNull().NotEmpty().When(x => !string.IsNullOrEmpty(x.ChnCode)).WithMessage("Invalid Channel Name");
            RuleFor(x => x.SbrUserId).NotNull().NotEmpty().When(x => !string.IsNullOrEmpty(x.SbrUserId)).WithMessage("Invalid User Id");
            RuleFor(x => x.LngIsoCode.Length).Equal(2).When(x => !string.IsNullOrEmpty(x.LngIsoCode)).WithMessage("Invalid LngIsoCode");
            RuleFor(x => x.SbrUserId).Null().When(x => !string.IsNullOrEmpty(x.CcnUsername)).WithMessage("Duplicate identifier");
            RuleFor(x => x.CcnUsername).Null().When(x => !string.IsNullOrEmpty(x.SbrUserId)).WithMessage("Duplicate identifier");
        }
    }

    internal class Subscription_VLD_TableSubscriptionDelete : AbstractValidator<Subscription_DTO_TableSubscriptionDelete>
    {
        internal Subscription_VLD_TableSubscriptionDelete()
        {
            RuleFor(x => x.TsbTable).NotEmpty().NotNull().WithMessage("Invalid Table Name");
        }
    }

    internal class Subscription_VLD_TableSubscriptionReadCurrent : AbstractValidator<Subscription_DTO_TableSubscriptionReadCurrent>
    {
        internal Subscription_VLD_TableSubscriptionReadCurrent() { }
    }

    internal class Subscription_VLD_TableSubscriptionRead : AbstractValidator<Subscription_DTO_TableSubscriptionRead>

    {
        internal Subscription_VLD_TableSubscriptionRead()
        {
            RuleFor(x => x.TsbTable).NotEmpty().NotNull().When(x => !string.IsNullOrEmpty(x.TsbTable)).WithMessage("Invalid Table Name");
        }
    }

}
