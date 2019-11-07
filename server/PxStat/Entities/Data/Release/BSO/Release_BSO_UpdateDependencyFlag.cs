using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Updates the value of the Release Dependency flag
    /// </summary>
    internal class Release_BSO_UpdateDependencyFlag : BaseTemplate_Update<Release_DTO_Update, Release_VLD_UpdateDependencyFlag>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Release_BSO_UpdateDependencyFlag(JSONRPC_API request) : base(request, new Release_VLD_UpdateDependencyFlag())
        { }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Release_ADO adoRelease = new Release_ADO(Ado);
            Release_DTO dtoRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(DTO.RlsCode, SamAccountName));
            dtoRelease.RlsDependencyFlag = DTO.RlsDependencyFlag.Value;
            DTO.MtrCode = dtoRelease.MtrCode;
            int updated = adoRelease.Update(dtoRelease, SamAccountName);
            if (updated == 0)
            {
                Log.Instance.Debug("Failed to update Dependency flag");
                Response.error = Label.Get("error.update");
                return false;
            }
            Response.data = JSONRPC.success;
            return true;
        }
    }
}