using API;
using PxStat.Resources;
using PxStat.Template;


namespace PxStat.Data
{
    /// <summary>
    /// Update a comment for a Release
    /// </summary>
    internal class Release_BSO_UpdateComment : BaseTemplate_Update<Release_DTO_Update, Release_VLD_UpdateComment>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Release_BSO_UpdateComment(JSONRPC_API request) : base(request, new Release_VLD_UpdateComment())
        {
        }

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


            DTO.MtrCode = dtoRelease.MtrCode;

            //We can do this now because the MtrCode is available to us
           Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_DATASET + DTO.MtrCode);
           Cas.RunCasFlush(Resources.Constants.C_CAS_DATA_CUBE_READ_METADATA + DTO.MtrCode);

            bool historicalTest = adoRelease.IsHistorical(DTO.RlsCode);
            int updated = 0;
            if (!historicalTest)
            {

                if (dtoRelease.CmmCode == 0)
                {
                    dtoRelease.CmmValue = DTO.CmmValue;
                    updated = adoRelease.CreateComment(dtoRelease, SamAccountName);
                }
                else
                {
                    dtoRelease.CmmValue = DTO.CmmValue;
                    updated = adoRelease.UpdateComment(dtoRelease, SamAccountName);
                }
            }
            if (updated == 0)
            {
                Log.Instance.Debug("Failed to update Release Comment");
                Response.error = Label.Get("error.update");
                return false;
            }
            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;
        }
    }
}
