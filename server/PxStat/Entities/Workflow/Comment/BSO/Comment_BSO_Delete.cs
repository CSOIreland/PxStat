using API;
using PxStat.Template;

namespace PxStat.Workflow
{
    /// <summary>
    /// Deletes a comment
    /// </summary>
    class Comment_BSO_Delete : BaseTemplate_Delete<Comment_DTO, Comment_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Comment_BSO_Delete(JSONRPC_API request) : base(request, new Comment_VLD())
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

            int commentCode = adoComment.Delete(Ado, DTO, SamAccountName);

            if (commentCode == 0)
            {
                //Can't delete a comment so we can't proceed
                Log.Instance.Debug("Can't delete  comment - request refused");
                Response.error = Label.Get("error.delete");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}