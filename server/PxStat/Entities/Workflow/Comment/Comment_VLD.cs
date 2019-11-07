using FluentValidation;

namespace PxStat.Workflow
{
    /// <summary>
    /// 
    /// </summary>
    internal class Comment_VLD : AbstractValidator<Comment_DTO>
    {
        /// <summary>
        /// 
        /// </summary>
        internal Comment_VLD()
        {
            //Mandatory - CmmValue
            RuleFor(f => f.CmmValue).Length(1, 1024).WithMessage("Invalid Comment").WithName("CommentValueValidation");
        }
    }
}