
using FluentValidation;
using PxStat.Security;

namespace PxStat.System.Notification
{
    internal class Email_VLD : AbstractValidator<Email_DTO_GroupMessage>
    {
        internal Email_VLD()
        {
            //Mandatory - Subject
            RuleFor(x => x.Subject).NotEmpty().Length(1, 998);
            //Mandatory - Body
            RuleFor(x => x.Body).NotEmpty().MinimumLength(1);
            RuleForEach(x => x.GroupCodes).SetValidator(new Group_VLD_ReadCode());
        }

    }

}