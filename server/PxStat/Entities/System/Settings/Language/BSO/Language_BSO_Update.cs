using Newtonsoft.Json;
using API;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// 
    /// </summary>
    internal class Language_BSO_Update : BaseTemplate_Update<Language_DTO_Update, Language_VLD_Update>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal Language_BSO_Update(JSONRPC_API request) : base(request, new Language_VLD_Update())
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            var adoLanguage = new Language_ADO(Ado);

            //Create the language - and retrieve the number of updated rows
            int nUpdated = adoLanguage.Update(DTO, SamAccountName);
            if (nUpdated == 0)
            {
                Log.Instance.Debug("No record has been updated");
                Ado.RollbackTransaction();
                Response.error = Label.Get("error.update");
                return false;
            }

            Log.Instance.Debug("Language updated: " + JsonConvert.SerializeObject(DTO));
            Ado.CommitTransaction();
            Response.data = JSONRPC.success;

            return true;
        }
    }
}

