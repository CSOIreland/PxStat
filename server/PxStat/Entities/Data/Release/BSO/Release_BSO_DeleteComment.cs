using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Delete Comment for a Release
    /// </summary>
    internal class Release_BSO_DeleteComment : BaseTemplate_Delete<Release_DTO_Update, Release_VLD_DeleteComment>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Release_BSO_DeleteComment(JSONRPC_API request) : base(request, new Release_VLD_DeleteComment())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Release_ADO adoRelease = new Release_ADO(Ado);

            Release_DTO dtoRelease = new Release_DTO();
            dtoRelease.RlsCode = DTO.RlsCode;
            int deleted = adoRelease.DeleteComment(dtoRelease, SamAccountName);

            if (deleted == 0)
            {
                //Can't create a comment so we can't proceed
                Log.Instance.Debug("Can't delete  comment - request refused");
                Response.error = Label.Get("error.delete");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}