using API;
using PxStat.Data;
using PxStat.Data.Px;
using PxStat.Entities.DBuild;
using PxStat.Security;
using PxStat.Template;
using System.Data;
using System.Dynamic;

namespace PxStat.DBuild
{
    internal class DBuild_BSO_ReadTemplateByRelease : BaseTemplate_Read<DBuild_DTO_ReadTemplateByRelease, DBuild_VLD_ReadTemplate>
    {
        /// <summary>
        /// PxFileBuild_BSO_ReadTemplate
        /// </summary>
        /// <param name="request"></param>
        internal DBuild_BSO_ReadTemplateByRelease(JSONRPC_API request) : base(request, new DBuild_VLD_ReadTemplateByRelease())
        {
        }

        /// <summary>
        /// HasPrivilege
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

            // Get the ReleaseDTO
            Release_ADO adoRelease = new Release_ADO(Ado);
            Release_DTO dtoRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(DTO.RlsCode, SamAccountName));

            // Get the matrix from the ReleaseDTO
            IDmatrix dmatrix = new Dmatrix();
            dmatrix = dmatrix.GetMultiLanguageMatrixFromRelease(Ado,  dtoRelease);
            FlatTableBuilder ftb = new FlatTableBuilder();

            DataTable dt = ftb.GetMatrixDataTableCodesOnly(dmatrix, DTO.LngIsoCode, true, true);

            dynamic result = new ExpandoObject();
            result.FrqValue = "";
            result.template = ftb.GetCsv(dt, "\"",null,true);
            result.MtrCode = dmatrix.Code;
            Response.data = result;

            return true;
        }
    }
}
