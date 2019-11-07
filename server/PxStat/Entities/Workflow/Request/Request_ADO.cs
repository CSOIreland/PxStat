using System.Collections.Generic;
using API;

namespace PxStat.Workflow
{
    /// <summary>
    /// ADO for Request
    /// </summary>
    internal class Request_ADO
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(ADO ado, string rqsCode = null)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            var inputParams = new List<ADO_inputParams>();

            if (rqsCode != null)
            {
                inputParams.Add(new ADO_inputParams() { name = "@RqsCode", value = rqsCode });
            }

            output = ado.ExecuteReaderProcedure("Workflow_Request_ReadRequest", inputParams);

            //return the list of entities that have been found
            return output;

        }
    }
}