using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Gets Matrices and Releases that a user has access to
    /// </summary>
    internal class Matrix_BSO_ReadCodeList : BaseTemplate_Read<Matrix_DTO_Read, Matrix_VLD_ReadCodeList>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Matrix_BSO_ReadCodeList(JSONRPC_API request) : base(request, new Matrix_VLD_ReadCodeList())
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
            var list = adoMatrix.ReadCodeList(SamAccountName);
            Response.data = list;

            return true;
        }
    }
}
