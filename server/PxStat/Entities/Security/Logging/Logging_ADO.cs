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

        internal void Create(ADO ado, string lggThread, string lggLevel, string lggClass, string lggMethod, string lggLine, string lggMessage, string lggException = null)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LggThread",value=lggThread},
                new ADO_inputParams() {name= "@LggLevel",value=lggLevel},

                new ADO_inputParams() {name= "@LggClass",value=lggClass},
                new ADO_inputParams() {name= "@LggMethod",value=lggMethod},
                new ADO_inputParams() {name= "@LggLine",value=lggLine},
                new ADO_inputParams() {name= "@LggMessage",value=lggMessage}

            };

            if (lggException != null)
                inputParamList.Add(new ADO_inputParams() { name = "@LggException", value = lggException });

            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            ado.ExecuteNonQueryProcedure("Security_Logging_Create", inputParamList, ref retParam);
        }
    }
}
