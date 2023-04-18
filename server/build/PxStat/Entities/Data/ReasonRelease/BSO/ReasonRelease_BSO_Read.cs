using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Read a Reason for a Release
    /// </summary>
    internal class ReasonRelease_BSO_Read : BaseTemplate_Read<ReasonRelease_DTO_Read, ReasonRelease_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal ReasonRelease_BSO_Read(JSONRPC_API request) : base(request, new ReasonRelease_VLD_Read())
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
            var adoReasonRelease = new ReasonRelease_ADO();

            //Reasons are returned as an ADO result
            ADO_readerOutput result = adoReasonRelease.Read(Ado, DTO);

            if (!result.hasData)
            {
                return false;
            }

            Response.data = result.data;
            return true;
        }
    }
}