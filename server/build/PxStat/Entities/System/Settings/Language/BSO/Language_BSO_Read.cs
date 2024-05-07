using API;
using PxStat.Security;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Reads a Language
    /// </summary>
    internal class Language_BSO_Read : BaseTemplate_Read<Language_DTO_Read, Language_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Language_BSO_Read(JSONRPC_API request) : base(request, new Language_VLD_Read())
        {
        }

        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {

            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoLanguage = new Language_ADO(Ado);

            //Languages are returned as an IADO result
            ADO_readerOutput result = adoLanguage.Read(DTO);


            if (!result.hasData)
            {

                return false;
            }


            Response.data = result.data;

            return true;
        }
    }
}
