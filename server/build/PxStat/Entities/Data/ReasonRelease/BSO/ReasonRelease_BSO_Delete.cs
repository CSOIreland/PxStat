using API;
using PxStat.Template;
using PxStat.Workflow;

namespace PxStat.Data
{
    /// <summary>
    /// Delete a Reason for a Release
    /// </summary>
    internal class ReasonRelease_BSO_Delete : BaseTemplate_Delete<ReasonRelease_DTO_Delete, ReasonRelease_VLD_Delete>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal ReasonRelease_BSO_Delete(JSONRPC_API request) : base(request, new ReasonRelease_VLD_Delete())
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
            //Validation of parameters and user have been successful. We may now proceed to read from the database
            var adoReasonRelease = new ReasonRelease_ADO();

            ADO_readerOutput adoReader = new ADO_readerOutput();


            //We must  check if there is a work flow in place for this release. If so, then we don't allow the ReasonRelease delete
            Workflow_DTO workflowDTO = new Workflow_DTO();
            Workflow_ADO workflowADO = new Workflow_ADO();
            workflowDTO.RlsCode = DTO.RlsCode;
            workflowDTO.WrqCurrentFlagOnly = true;

            adoReader = workflowADO.Read(Ado, workflowDTO, SamAccountName);
            if (adoReader.hasData)
            {
                Log.Instance.Debug("Can't delete ReasonRelease because a Work flow is already in place for this Release");
                Response.error = Label.Get("error.create");
                return false;
            }

            Release_ADO adoRelease = new Release_ADO(Ado);

            if (!adoRelease.IsWip(DTO.RlsCode))
            {
                Log.Instance.Debug("Can't delete ReasonRelease - Release is not in WIP status");
                Response.error = Label.Get("error.update");
                return false;
            }

            //Update the Reason
            int deleted = adoReasonRelease.Delete(Ado, DTO, SamAccountName);
            if (deleted == 0)
            {
                Log.Instance.Debug("Can't delete ReasonRelease");
                Response.error = Label.Get("error.update");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}