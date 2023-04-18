using API;
using PxStat.Template;

namespace PxStat.Workflow
{
    /// <summary>
    /// Creates a comment
    /// </summary>
    class Comment_BSO_Create : BaseTemplate_Create<Comment_DTO, Comment_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Comment_BSO_Create(JSONRPC_API request) : base(request, new Comment_VLD())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //Validation of parameters and user have been successful. We may now proceed to read from the database

            var adoComment = new Comment_ADO();

            int commentID = adoComment.Create(Ado, DTO, SamAccountName);

            if (commentID == 0)
            {
                //Can't create a comment so we can't proceed
                Log.Instance.Debug("Can't create a comment - Comment create request refused");
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}