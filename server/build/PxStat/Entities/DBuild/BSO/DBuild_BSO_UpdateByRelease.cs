using API;
using Newtonsoft.Json.Linq;
using PxStat.Data;
using PxStat.Security;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;

namespace PxStat.DBuild
{
    internal class DBuild_BSO_UpdateByRelease : BaseTemplate_Read<DBuild_DTO_UpdateByRelease, DBuild_VLD_UpdateByRelease>
    {
        /// <summary>
        /// 
        /// </summary>
        List<int> divisors = new List<int>();



        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal DBuild_BSO_UpdateByRelease(JSONRPC_API request) : base(request, new DBuild_VLD_UpdateByRelease())
        { }

        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            // Check permissions
            DBuild_BSO dbso = new DBuild_BSO();
            if (!dbso.HasBuildPermission(SamAccountName, "update"))
            {
                Response.error = Label.Get("error.privilege", DTO.LngIsoCode);
                return false;
            }

            // Get the ReleaseDTO
            Release_ADO adoRelease = new Release_ADO(Ado);
            Release_DTO dtoRelease = Release_ADO.GetReleaseDTO(adoRelease.Read(DTO.RlsCode, SamAccountName));

            // Get the matrix from the ReleaseDTO
            IDmatrix dmatrix = new Dmatrix();
            dmatrix = dmatrix.GetMultiLanguageMatrixFromRelease(Ado, dtoRelease);


            Response = dbso.BsoUpdateByRelease(dmatrix, Response, DTO, Ado, null, DTO.LngIsoCode);
            return Response.error != null;
        }



    }
}
