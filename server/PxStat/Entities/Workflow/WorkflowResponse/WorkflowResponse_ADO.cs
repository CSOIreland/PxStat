using System.Collections.Generic;
using API;

namespace PxStat.Workflow
{
    /// <summary>
    /// ADO for Workflow Response
    /// </summary>
    internal class WorkflowResponse_ADO
    {
        /// <summary>
        /// Creates a Workflow Response
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Create(ADO ado, WorkflowResponse_DTO dto, string ccnUsername)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername},
                new ADO_inputParams() {name ="@RspCode",value= dto.RspCode},
                new ADO_inputParams() {name ="@CmmCode",value= dto.CmmCode},
                new ADO_inputParams() {name ="@RlsCode",value= dto.RlsCode}
            };
            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Workflow_WorkflowResponse_Create", inputParams, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Reads approval access
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="user"></param>
        /// <param name="isApprover"></param>
        /// <param name="rlsCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput GetApproverAccess(ADO ado, string user, bool isApprover, int rlsCode = default(int))
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= user},
                new ADO_inputParams() {name ="@GccApproveFlag", value = isApprover }
            };
            if (rlsCode != default(int))
                inputParams.Add(new ADO_inputParams() { name = "@RlsCode", value = rlsCode });

            return ado.ExecuteReaderProcedure("Workflow_WorkflowResponse_ReadApprovalAccess", inputParams);

        }

        /// <summary>
        /// Checks if there is already a Workflow Response for a given Release Code
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal bool IsInUse(ADO ado, dynamic dto)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@RlsCode",value= dto.RlsCode},

            };
            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Workflow_Response_Usage", inputParams, ref retParam);

            //Assign the returned value for checking and output

            return (retParam.value > 0);
        }



    }
}