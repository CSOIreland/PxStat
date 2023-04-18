using API;
using PxStat.Template;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Read one or more themes
    /// </summary>
    internal class Theme_BSO_Read : BaseTemplate_Read<Theme_DTO_Read, Theme_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Theme_BSO_Read(JSONRPC_API request) : base(request, new Theme_VLD_Read())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {

            var adoTheme = new Theme_ADO(Ado);
            var list = adoTheme.Read(DTO);
            Response.data = list;

            return true;
        }
    }
}
