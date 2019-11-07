using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Creates a Keyword subject
    /// </summary>
    internal class Keyword_Subject_BSO_Create : BaseTemplate_Create<Keyword_Subject_DTO, Keyword_Subject_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Keyword_Subject_BSO_Create(JSONRPC_API request) : base(request, new Keyword_Subject_VLD_Create())
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
            var adoKeyword_Subject = new Keyword_Subject_ADO(Ado);

            //Create the Keyword_Subject - and retrieve the newly created Id
            int newId = adoKeyword_Subject.Create(DTO);
            if (newId == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }
            else if (newId < 0)
            {
                Response.error = Label.Get("error.duplicate");
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}

