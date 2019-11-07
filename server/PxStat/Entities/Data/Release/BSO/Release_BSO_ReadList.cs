using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Reads a list of releases for a given matrix code and username
    /// </summary>
    internal class Release_BSO_ReadList : BaseTemplate_Read<Release_DTO_Read, Release_VLD_ReadList>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Release_BSO_ReadList(JSONRPC_API request) : base(request, new Release_VLD_ReadList())
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
            var adoRelease = new Release_ADO(Ado);
            var list = adoRelease.ReadList(DTO, SamAccountName);
            Response.data = list;

            return true;
        }
    }
}