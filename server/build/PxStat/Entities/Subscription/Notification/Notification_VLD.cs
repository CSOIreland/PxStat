using FluentValidation;

namespace PxStat.Subscription
{
    internal class Notification_VLD_Create : AbstractValidator<Notification_DTO_Create>
    {
        internal Notification_VLD_Create()
        {
            //validate the Preference here - to be decided - Json only? a particular schema?
        }
    }
}
