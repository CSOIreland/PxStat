using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Deletes a Keyword Subject
    /// </summary>
    class Keyword_Subject_BSO_Delete : BaseTemplate_Delete<Keyword_Subject_DTO, Keyword_Subject_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Keyword_Subject_BSO_Delete(JSONRPC_API request) : base(request, new Keyword_Subject_VLD_Delete())
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
            var adoKeyword_Subject = new Keyword_Subject_ADO(Ado);

            //attempting to delete. The number of entities deleted are passed to the entitiesDeleted variable (this is 1 for a successful delete)
            int nDeleted = adoKeyword_Subject.Delete(DTO);
            Log.Instance.Debug("Delete operation finished in ADO");

            if (nDeleted == 0)
            {
                Log.Instance.Debug("No record found for delete request");
                Response.error = Label.Get("error.delete");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}

