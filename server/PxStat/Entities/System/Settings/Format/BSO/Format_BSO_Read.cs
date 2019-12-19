using API;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Reads one or more Formats
    /// </summary>
    internal class Format_BSO_Read : BaseTemplate_Read<Format_DTO_Read, Format_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Format_BSO_Read(JSONRPC_API request) : base(request, new Format_VLD_Read())
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
            var adoFormat = new Format_ADO();

            ADO_readerOutput result = adoFormat.Read(Ado, DTO);

            if (!result.hasData)
            {
                return false;
            }

            Response.data = result.data;
            return true;
        }
    }
}
