using API;
using System.Collections.Generic;

namespace PxStat.Security
{
    /// <summary>
    /// IADO classes for Trace
    /// </summary>
    internal class Trace_ADO
    {
        /// <summary>
        /// Reads a Trace
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="trace"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(IADO ado, Trace_DTO_Read trace)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams() { name = "@StartDate", value = trace.StartDate });
            paramList.Add(new ADO_inputParams() { name = "@EndDate", value = trace.EndDate });
            if (!string.IsNullOrEmpty(trace.AuthenticationType))
                paramList.Add(new ADO_inputParams() { name = "@AuthenticationType", value = trace.AuthenticationType });
            if (!string.IsNullOrEmpty(trace.TrcIp))
                paramList.Add(new ADO_inputParams() { name = "@TrcIp", value = trace.TrcIp });
            if (!string.IsNullOrEmpty(trace.TrcUsername))
                paramList.Add(new ADO_inputParams() { name = "@TrcUsername", value = trace.TrcUsername });

            paramList.Add(new ADO_inputParams() { name = "@Const_AUTHENTICATED", value = Label.Get("authentication.authenticated") });
            paramList.Add(new ADO_inputParams() { name = "@Const_REGISTERED", value = Label.Get("authentication.registered") });
            paramList.Add(new ADO_inputParams() { name = "@Const_ANONYMOUS", value = Label.Get("authentication.anonymous") });
            paramList.Add(new ADO_inputParams() { name = "@Const_ANY", value = Label.Get("authentication.any") });

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Security_Trace_Read", paramList);

            //Read the result of the call to the database
            if (output.hasData)
            {
                Log.Instance.Debug("Data found");
            }
            else
            {
                //No data found
                Log.Instance.Debug("No data found");
            }

            //return the list of entities that have been found
            return output;
        }

        /// <summary>
        /// Creates a Trace
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="trace"></param>
        /// <param name="inTransaction"></param>
        /// <returns></returns>
        internal int Create(IADO ado, Trace_DTO_Create trace)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@TrcMethod",value=trace.TrcMethod},
                new ADO_inputParams() {name= "@TrcParams",value=trace.TrcParams},
                new ADO_inputParams() {name= "@TrcIp",value=trace.TrcIp},
                new ADO_inputParams() {name= "@TrcUseragent",value=trace.TrcUseragent}
            };

            if (!string.IsNullOrEmpty(trace.CcnUsername))
                inputParamList.Add(new ADO_inputParams() { name = "@CcnUsername", value = trace.CcnUsername });

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Executing the stored procedure
            ado.ExecuteNonQueryProcedure("Security_Trace_Create", inputParamList, ref retParam);

            return retParam.value;
        }
    }
}
