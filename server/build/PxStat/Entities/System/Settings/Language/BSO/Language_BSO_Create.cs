using API;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Creates a Language
    /// </summary>
    internal class Language_BSO_Create : BaseTemplate_Create<Language_DTO_Create, Language_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Language_BSO_Create(JSONRPC_API request) : base(request, new Language_VLD_Create())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoLanguage = new Language_ADO(Ado);



            //First we must check if the language exists already (we can't have duplicates)
            if (adoLanguage.Exists(DTO.LngIsoCode))
            {
                //This language exists already, we can't proceed
                Log.Instance.Debug("The ISO Code '" + DTO.LngIsoCode + "' exists already.");
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //Create the language - and retrieve the newly created Id
            int newId = adoLanguage.Create(DTO, SamAccountName);
            if (newId == 0)
            {
                Log.Instance.Debug("Can't Create Language");
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}
