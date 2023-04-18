using System.Collections.Generic;
using System.Dynamic;
using API;
using PxStat.Template;

namespace PxStat.Data
{
    /// <summary>
    /// Reads a Group Account
    /// </summary>
    internal class ReleaseProductAssociation_BSO_Read : BaseTemplate_Read<ReleaseProduct_DTO_Read, ReleaseProductAssociation_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal ReleaseProductAssociation_BSO_Read(JSONRPC_API request) : base(request, new ReleaseProductAssociation_VLD_Read())
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
            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams() { name = "@RlsCode", value = DTO.RlsCode });

            var adoReleaseProduct = new ReleaseProductAssocaition_ADO();

            // Get the Products associated with the release
            ADO_readerOutput associations = adoReleaseProduct.Read(Ado, DTO, SamAccountName );

            // Concatenate the Release with the Products to return the response
            dynamic result = new ExpandoObject();
            result = associations.hasData ? associations.data : null;
            Response.data = result;
            return true;
        }
    }
}