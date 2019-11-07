using API;
using PxStat.Template;


namespace PxStat.Data
{
    /// <summary>
    /// Read a Release
    /// </summary>
    internal class Release_BSO_Read : BaseTemplate_Read<Release_DTO_Read, Release_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Release_BSO_Read(JSONRPC_API request) : base(request, new Release_VLD_Read())
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
            var list = adoRelease.Read(DTO.RlsCode, SamAccountName);
            Response.data = list;

            return true;
        }
    }
}

