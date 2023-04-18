using API;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Updates a subject
    /// </summary>
    class Subject_BSO_Update : BaseTemplate_Update<Subject_DTO, Subject_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        public Subject_BSO_Update(JSONRPC_API request) : base(request, new Subject_VLD_Update())
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
            var adoSubject = new Subject_ADO(Ado);

            int nUpdatedSubjectId = 0;

            //We can't allow duplicate named Subjects, so we must check first
            if (adoSubject.UpdateExists(DTO))
            {
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            if (DTO.LngIsoCode != Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code"))
            {
                SubjectLanguage_BSO subjectLanguageBso = new SubjectLanguage_BSO();
                nUpdatedSubjectId = subjectLanguageBso.CreateOrUpdate(DTO, Ado);
                if (nUpdatedSubjectId == 0)
                {
                    Log.Instance.Debug("Update of SubjectLanguage failed");
                    Response.error = Label.Get("error.update");
                    return false;
                }
            }
            else
                nUpdatedSubjectId = adoSubject.Update(DTO, SamAccountName);

            if (nUpdatedSubjectId == 0)
            {
                Log.Instance.Debug("Update of Subject failed");
                Response.error = Label.Get("error.update");
                return false;
            }

            //We must now delete all of the keywords for the subject
            Keyword_Subject_BSO_Mandatory kbBso = new Keyword_Subject_BSO_Mandatory();
            int nchanged = kbBso.Delete(Ado, DTO, true);
            if (nchanged == 0)
            {
                Log.Instance.Debug("No keywords deleted");

            }
            //We can now recreate the keywords for the subject
            kbBso.Create(Ado, DTO, nUpdatedSubjectId);

            //Reset the relevant caches
            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_NAVIGATION_SEARCH);
            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_NAVIGATION_READ);

            Response.data = JSONRPC.success;

            return true;
        }
    }
}
