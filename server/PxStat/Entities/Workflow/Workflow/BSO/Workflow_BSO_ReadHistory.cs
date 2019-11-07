using API;
using PxStat.Template;

namespace PxStat.Workflow
{
    /// <summary>
    /// Read the approval/rejection history for a specific workflow
    /// </summary>
    internal class Workflow_BSO_ReadHistory : BaseTemplate_Read<Workflow_DTO, Workflow_VLD>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Workflow_BSO_ReadHistory(JSONRPC_API request) : base(request, new Workflow_VLD())
        { }

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
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoWorkflow = new Workflow_ADO();

            ADO_readerOutput result = adoWorkflow.ReadHistory(Ado, DTO.RlsCode, SamAccountName);

            if (!result.hasData)
            {
                return false;
            }

            Response.data = result.data;

            return true;
        }
    }

}