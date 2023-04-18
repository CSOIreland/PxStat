using API;
using PxStat.Template;
using PxStat.Workflow;

namespace PxStat.Data
{
    /// <summary>
    /// Create a Reason for a Release
    /// </summary>
    internal class ReasonRelease_BSO_Create : BaseTemplate_Create<ReasonRelease_DTO_Create, ReasonRelease_VLD_Create>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal ReasonRelease_BSO_Create(JSONRPC_API request) : base(request, new ReasonRelease_VLD_Create())
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


            //First we must check if the ReasonRelease exists already (we can't have duplicates)

            ReasonRelease_DTO_Read readDTO = new ReasonRelease_DTO_Read();
            readDTO.RlsCode = DTO.RlsCode;
            readDTO.RsnCode = DTO.RsnCode;
            readDTO.LngIsoCode = DTO.LngIsoCode;

            adoReader = adoReasonRelease.Read(Ado, readDTO);
            if (adoReader.hasData)
            {
                Log.Instance.Debug("Can't create duplicate ReasonRelease");
                Response.error = Label.Get("error.duplicate");
                return false;
            }

            //We must also check if there is a work flow in place for this release. If so, then we don't allow the ReasonRelease create
            Workflow_DTO workflowDTO = new Workflow_DTO();
            Workflow_ADO workflowADO = new Workflow_ADO();
            workflowDTO.RlsCode = DTO.RlsCode;
            workflowDTO.WrqCurrentFlagOnly = true;

            adoReader = workflowADO.Read(Ado, workflowDTO, SamAccountName);
            if (adoReader.hasData)
            {
                Log.Instance.Debug("Can't create ReasonRelease because a Work flow is already in place for this Release");
                Response.error = Label.Get("error.create");
                return false;
            }

            Release_ADO adoRelease = new Release_ADO(Ado);

            if (!adoRelease.IsWip(DTO.RlsCode))
            {
                Log.Instance.Debug("Can't create ReasonRelease - Release is not in WIP status");
                Response.error = Label.Get("error.update");
                return false;
            }

            //Create the Reason - and retrieve the newly created Id
            int newId = adoReasonRelease.Create(Ado, DTO, SamAccountName);
            if (newId == 0)
            {
                Log.Instance.Debug("Can't create ReasonRelease");
                Response.error = Label.Get("error.create");
                return false;
            }

            Response.data = JSONRPC.success;
            return true;
        }
    }
}