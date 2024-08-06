using API;
using PxStat.Data;
using PxStat.Data.Px;
using PxStat.Template;
using System.Data;
using System.Dynamic;

namespace PxStat.DBuild
{
    internal class DBuild_BSO_ReadDatasetByRelease : BaseTemplate_Read<DBuild_DTO_UpdateByRelease, DBuild_ReadDatasetByRelease_VLD>
    {
        internal DBuild_BSO_ReadDatasetByRelease(JSONRPC_API request) : base(request, new DBuild_ReadDatasetByRelease_VLD())
        {

        }
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
            // Get the ReleaseDTO
            Release_ADO adoRelease = new Release_ADO(Ado);
            Release_DTO dtoRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(DTO.RlsCode, SamAccountName));

            // Get the matrix from the ReleaseDTO
            IDmatrix dmatrix = new Dmatrix();
            dmatrix = dmatrix.GetMultiLanguageMatrixFromRelease(Ado,  dtoRelease); 

            FlatTableBuilder ftb = new FlatTableBuilder();
            DataTable dt = null;

            if (!DTO.Labels)
            {
                dt = ftb.GetMatrixDataTableCodesOnly(dmatrix, DTO.Dspecs[0].Language, true);
            }
            else
            {
                dt = ftb.GetMatrixDataTableCodesAndLabels(dmatrix, DTO.Dspecs[0].Language, true);
            }

            dynamic result = new ExpandoObject();
            result.csv = ftb.GetCsv(dt, "\"", null, true);
            result.MtrCode = dmatrix.Code;
            Response.data = result;
            return true;
        }

    }
}
