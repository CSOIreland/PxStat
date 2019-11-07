using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Read a Keyword Release
    /// </summary>
    internal class Keyword_Release_BSO_Read : BaseTemplate_Read<Keyword_Release_DTO, Keyword_Release_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Keyword_Release_BSO_Read(JSONRPC_API request) : base(request, new Keyword_Release_VLD_Read())
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
            var adoKeywordRelease = new Keyword_Release_ADO();

            var list = adoKeywordRelease.Read(Ado, DTO);
            Response.data = list.data;

            return true;
        }
    }
}