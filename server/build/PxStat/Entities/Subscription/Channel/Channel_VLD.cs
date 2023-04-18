using FluentValidation;

namespace PxStat.Subscription
{
    internal class Subscription_VLD_ChannelRead : AbstractValidator<Channel_DTO_Read>
    {
        internal Subscription_VLD_ChannelRead()
        {
            RuleFor(x => x.ChnCode).NotNull().NotEmpty().When(x => !string.IsNullOrEmpty(x.ChnCode)).WithMessage("Invalid Channel Name");
            RuleFor(x => x.LngIsoCode.Length).Equal(2).When(x => !string.IsNullOrEmpty(x.LngIsoCode)).WithMessage("Invalid LngIsoCode");
        }
    }

}
