using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Reads a list of releases for a given product
    /// </summary>
    internal class Release_BSO_ReadListByProduct : BaseTemplate_Read<Release_DTO_Read, Release_VLD_ReadListByProduct>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Release_BSO_ReadListByProduct(JSONRPC_API request) : base(request, new Release_VLD_ReadListByProduct())
        {

        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            var adoRelease = new Release_ADO(Ado);
            var list = adoRelease.ReadListByProduct(DTO.PrcCode, SamAccountName);
            Response.data = list;

            return true;
        }
    }
}