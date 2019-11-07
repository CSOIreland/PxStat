using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Updates the value of the Release Dependency flag
    /// </summary>
    internal class Release_BSO_UpdateProduct : BaseTemplate_Update<Release_DTO_Update, Release_VLD_Product>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Release_BSO_UpdateProduct(JSONRPC_API request) : base(request, new Release_VLD_Product())
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
            dtoRelease.PrcCode = DTO.PrcCode;
            DTO.MtrCode = dtoRelease.MtrCode;

            //We can do this now because the MtrCode is available to us
            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + DTO.MtrCode);
            MemCacheD.CasRepositoryFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_METADATA + DTO.MtrCode);
            int updated = adoRelease.Update(dtoRelease, SamAccountName);
            if (updated == 0)
            {
                Log.Instance.Debug("Failed to update Product Code");
                Response.error = Label.Get("error.update");
                return false;
            }
            Response.data = JSONRPC.success;
            return true;
        }
    }
}