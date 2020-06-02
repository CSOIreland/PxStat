using API;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Create a Subject
    /// </summary>
    internal class Subject_BSO_Create : BaseTemplate_Create<Subject_DTO, Subject_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Subject_BSO_Create(JSONRPC_API request) : base(request, new Subject_VLD_Create())
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
            var adoSubject = new Subject_ADO(Ado);

            //You can only create a subject in the default Language
            DTO.LngIsoCode = Configuration_BSO.GetCustomConfig("language.iso.code");

            //We can't allow duplicate named Subjects, so we must check first
            if (adoSubject.Exists(DTO))
            {
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //Create the Subject - and retrieve the newly created Id
            int newId = adoSubject.Create(DTO, SamAccountName);
            if (newId == 0)
            {
                Response.error = Label.Get("error.create");
                return false;
            }
            //We must create the search keywords for this subject
            Keyword_Subject_BSO_Mandatory bsoKeyword = new Keyword_Subject_BSO_Mandatory();
            bsoKeyword.Create(Ado, DTO, newId);
            Response.data = JSONRPC.success;

            return true;
        }
    }
}
