using FluentValidation;

namespace PxStat.Subscription
{
    internal class Subscriber_VLD_Create : AbstractValidator<Subscriber_DTO_Create>
    {
        internal Subscriber_VLD_Create()
        {
            RuleFor(x => x.LngIsoCode.Length).Equal(2).When(x => !string.IsNullOrEmpty(x.LngIsoCode)).WithMessage("Invalid LngIsoCode");
        }
    }

    internal class Subscriber_VLD_ReadCurrent : AbstractValidator<Subscriber_DTO_ReadCurrent>
    {
        internal Subscriber_VLD_ReadCurrent()
        {
            //validate the Preference here - to be decided - Json only? a particular schema?
        }
    }

    internal class Subscriber_VLD_Read : AbstractValidator<Subscriber_DTO_Read>
    {
        internal Subscriber_VLD_Read()
        {
            RuleFor(x => x.Uid.Length).GreaterThan(2).When(x => !string.IsNullOrEmpty(x.Uid)).WithMessage("Invalid Uid");
        }
    }

    internal class Subscriber_VLD_Update : AbstractValidator<Subscriber_DTO_Update>
    {
        internal Subscriber_VLD_Update()
        {
            RuleFor(x => x.LngIsoCode.Length).Equal(2).When(x => !string.IsNullOrEmpty(x.LngIsoCode)).WithMessage("Invalid LngIsoCode");
        }
    }

    internal class Subscriber_VLD_UpdateKey : AbstractValidator<Subscriber_DTO_UpdateKey>
    {
        internal Subscriber_VLD_UpdateKey()
        {

        }
    }

    internal class Subscriber_VLD_Delete : AbstractValidator<Subscriber_DTO_Delete>
    {
        internal Subscriber_VLD_Delete()
        {
            RuleFor(x => x.Uid).NotNull().NotEmpty().WithMessage("Invalid Uid"); ;
        }
    }
}
