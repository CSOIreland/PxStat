using API;
using PxStat.Template;

namespace PxStat.Data
{
    internal class Release_BSO_ReadWipForLive : BaseTemplate_Create<Release_DTO_ReadWipForLive, Release_VLD_ReadWipForLive>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Release_BSO_ReadWipForLive(JSONRPC_API request) : base(request, new Release_VLD_ReadWipForLive())
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
            Release_BSO rBso = new Release_BSO(Ado);

            Response.data = rBso.IsLiveWithWip(DTO.RlsCode, SamAccountName);
            return true;
        }

    }
}
