using API;
using PxStat.Security;
using PxStat.Template;
using System.Dynamic;

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
            var currentRelease = Release_ADO.GetReleaseDTO(new Matrix_ADO(Ado).Read(DTO.RlsCode, Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code"), SamAccountName));
            if (currentRelease == null)
            {
                Response.data = null;
                return true;
            }

            int previousReleaseCode = new Compare_ADO(Ado).ReadPreviousReleaseForUser(DTO.RlsCode, SamAccountName);

            dynamic output = new ExpandoObject();

            if (previousReleaseCode == 0)
            {
                Response.data = null;
            }
            else
            {
                output.RlsCode = previousReleaseCode;
                output.LngIsoCode = new Compare_ADO(Ado).ReadLanguagesForRelease(previousReleaseCode);
                Response.data = output;
            }

            return true;
        }

    }
}
