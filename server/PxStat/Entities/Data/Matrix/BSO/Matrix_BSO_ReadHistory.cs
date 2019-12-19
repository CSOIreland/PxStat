using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Reads Matrix History
    /// </summary>
    internal class Matrix_BSO_ReadHistory : BaseTemplate_Read<Matrix_DTO_ReadHistory, Matrix_VLD_ReadHistory>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Matrix_BSO_ReadHistory(JSONRPC_API request) : base(request, new Matrix_VLD_ReadHistory())
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
            var adoMatrix = new Matrix_ADO(Ado);
            var list = adoMatrix.ReadHistory(SamAccountName, DTO);
            Response.data = list;

            return true;
        }
    }
}
