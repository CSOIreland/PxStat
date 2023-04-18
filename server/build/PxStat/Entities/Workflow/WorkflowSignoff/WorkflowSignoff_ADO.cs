using System.Collections.Generic;
using API;

namespace PxStat.Workflow
{
    /// <summary>
    /// ADO for WorkflowSignoff
    /// </summary>
    internal class WorkflowSignoff_ADO
    {
        /// <summary>
        /// Creates a Workflow Signoff
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Create(ADO ado, WorkflowSignoff_DTO dto, string ccnUsername)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername},
                new ADO_inputParams() {name ="@SgnCode",value= dto.SgnCode},
                new ADO_inputParams() {name ="@CmmCode",value= dto.CmmCode},
                new ADO_inputParams() {name ="@RlsCode",value= dto.RlsCode}
            };

            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Workflow_WorkflowSignoff_Create", inputParams, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }


    }
}