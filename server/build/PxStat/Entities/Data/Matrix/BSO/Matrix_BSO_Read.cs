using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Class to read a Matrix
    /// </summary>
    internal class Matrix_BSO_Read : BaseTemplate_Read<Matrix_DTO_Read, Matrix_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Matrix_BSO_Read(JSONRPC_API request) : base(request, new Matrix_VLD_Read())
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
            var list = adoMatrix.Read(DTO.RlsCode, DTO.LngIsoCode, SamAccountName);
            Response.data = list;

            return true;
        }
    }
}

