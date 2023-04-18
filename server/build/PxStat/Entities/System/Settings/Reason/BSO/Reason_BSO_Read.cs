using API;
using PxStat.Template;

namespace PxStat.System.Settings
{
    /// <summary>
    /// Read one or more Reasons
    /// </summary>
    internal class Reason_BSO_Read : BaseTemplate_Read<Reason_DTO_Read, Reason_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Reason_BSO_Read(JSONRPC_API request) : base(request, new Reason_VLD_Read())
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
            var adoReason = new Reason_ADO();

            //Reasons are returned as an ADO result
            ADO_readerOutput result = adoReason.Read(Ado, DTO);

            if (!result.hasData)
            {
                return false;
            }

            Response.data = result.data;
            return true;
        }
    }
}