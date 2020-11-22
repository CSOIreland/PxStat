using API;
using System;
using System.Collections.Generic;

namespace PxStat.Security
{
    /// <summary>
    /// ADO methods for database Logging
    /// </summary>
    internal class Performance_ADO
    {
        /// <summary>
        /// Read logs from database
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="prfDatetimeStart"></param>
        /// <param name="prfDatetimeEnd"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(ADO ado, DateTime prfDatetimeStart, DateTime prfDatetimeEnd)
        {


            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@PrfDatetimeStart",value=prfDatetimeStart.AddTicks(-(prfDatetimeStart.Ticks % TimeSpan.TicksPerMinute))},
                new ADO_inputParams() {name= "@PrfDatetimeEnd",value=prfDatetimeEnd.AddTicks(-(prfDatetimeEnd.Ticks % TimeSpan.TicksPerMinute))}
            };

            var reader = ado.ExecuteReaderProcedure("Security_Performance_Read", inputParamList);

            return reader;

        }

        /// <summary>
        /// Delete performance entries
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Delete(ADO ado, dynamic dto, string ccnUsername)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                // No parameters required as will be truncating whole table
            };
            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Performance_Delete", inputParams, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }
    }

}
