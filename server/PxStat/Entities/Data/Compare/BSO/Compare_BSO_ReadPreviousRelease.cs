using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Reads the release previous to the requested release
    /// </summary>
    internal class Compare_BSO_ReadPreviousRelease : BaseTemplate_Read<Compare_DTO_ReadPrevious, Compare_VLD_ReadPrevious>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Compare_BSO_ReadPreviousRelease(JSONRPC_API request) : base(request, new Compare_VLD_ReadPrevious())
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
            // uses authentication and filters privileges according to user access
            var currentRelease = Release_ADO.GetReleaseDTO(new Matrix_ADO(Ado).Read(DTO.RlsCode, Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE"), SamAccountName));
            if (currentRelease == null)
            {
                Response.data = null;
                return true;
            }

            int previousReleaseId = new Compare_ADO(Ado).ReadPreviousRelease(DTO.RlsCode);
            if (previousReleaseId == 0)
            {
                Response.data = null;
            }
            else
            {
                Response.data = previousReleaseId;
            }

            return true;
        }

    }
}
