using API;
using PxStat.Template;
using PxStat.Workflow;

namespace PxStat.Data
{
    /// <summary>
    /// Update a Reason for a Release
    /// </summary>
    internal class ReasonRelease_BSO_Update : BaseTemplate_Update<ReasonRelease_DTO_Update, ReasonRelease_VLD_Update>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal ReasonRelease_BSO_Update(JSONRPC_API request) : base(request, new ReasonRelease_VLD_Update())
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


            //We must  check if there is a workflow in place for this release. If so, then we don't allow the ReasonRelease update
            Workflow_DTO workflowDTO = new Workflow_DTO();
            Workflow_ADO workflowADO = new Workflow_ADO();
            workflowDTO.RlsCode = DTO.RlsCode;
            workflowDTO.WrqCurrentFlagOnly = true;

            adoReader = workflowADO.Read(Ado, workflowDTO, SamAccountName);
            if (adoReader.hasData)
            {
                Log.Instance.Debug("Can't update ReasonRelease because a Workflow is already in place for this Release");
                Response.error = Label.Get("error.create");
                return false;
            }

            Release_ADO adoRelease = new Release_ADO(Ado);

            if (!adoRelease.IsWip(DTO.RlsCode))
            {
                Log.Instance.Debug("Can't update ReasonRelease - Release is not in WIP status");
                Response.error = Label.Get("error.update");
                return false;
            }

            //Update the Reason
            int updated = adoReasonRelease.Update(Ado, DTO, SamAccountName);
            if (updated == 0)
            {
                Log.Instance.Debug("Can't update ReasonRelease");
                Response.error = Label.Get("error.update");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}