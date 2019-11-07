using API;
using System;
using System.Collections.Generic;

namespace PxStat.Security.Logging
{
    /// <summary>
    /// ADO methods for database Logging
    /// </summary>
    internal class Logging_ADO
    {
        /// <summary>
        /// Read logs from database
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="lggDatetimeStart"></param>
        /// <param name="lggDatetimeEnd"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(ADO ado, DateTime lggDatetimeStart, DateTime lggDatetimeEnd)
        {


            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LggDatetimeStart",value=lggDatetimeStart},
                new ADO_inputParams() {name= "@LggDatetimeEnd",value=lggDatetimeEnd}
            };

            var reader = ado.ExecuteReaderProcedure("Security_Logging_Read", inputParamList);

            return reader;

        }
    }
}