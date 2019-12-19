using API;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Reads one or more Copyright
    /// </summary>
    internal class Copyright_BSO_Read : BaseTemplate_Read<Copyright_DTO_Read, Copyright_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Copyright_BSO_Read(JSONRPC_API request) : base(request, new Copyright_VLD_Read())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoCopyright = new Copyright_ADO();

            //Copyrights are returned as an ADO result
            ADO_readerOutput result = adoCopyright.Read(Ado, DTO);

            if (!result.hasData)
            {

                return false;
            }

            Response.data = result.data;

            return true;
        }
    }
}
