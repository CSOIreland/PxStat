using API;
using PxStat.Template;

namespace PxStat.Workflow
{
    /// <summary>
    /// Updates a comment
    /// </summary>
    class Comment_BSO_Update : BaseTemplate_Update<Comment_DTO, Comment_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Comment_BSO_Update(JSONRPC_API request) : base(request, new Comment_VLD())
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

            int commentCode = adoComment.Update(Ado, DTO, SamAccountName);

            if (commentCode == 0)
            {
                //Can't create a comment so we can't proceed
                Log.Instance.Debug("Can't update  comment - Comment update request refused");
                Response.error = Label.Get("error.update");
                return false;
            }

            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}