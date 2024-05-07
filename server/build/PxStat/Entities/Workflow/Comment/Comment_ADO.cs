using System.Collections.Generic;
using API;

namespace PxStat.Workflow
{
    /// <summary>
    /// IADO methods for Comment
    /// </summary>
    internal class Comment_ADO
    {
        /// <summary>
        /// Create a comment
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Create(IADO ado, dynamic dto, string ccnUsername)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername},
                new ADO_inputParams() {name ="@CmmValue",value= dto.CmmValue}
            };
            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Workflow_Comment_Create", inputParams, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Update a comment
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Update(IADO ado, dynamic dto, string ccnUsername)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername},
                new ADO_inputParams() {name ="@CmmCode",value= dto.CmmCode},
                new ADO_inputParams() {name ="@CmmValue",value= dto.CmmValue}
            };
            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Workflow_Comment_Update", inputParams, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Delete a comment
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Delete(IADO ado, dynamic dto, string ccnUsername)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername},
                new ADO_inputParams() {name ="@CmmCode",value= dto.CmmCode}
            };
            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Workflow_Comment_Delete", inputParams, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }
    }
}