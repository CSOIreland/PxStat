using API;
using System.Collections.Generic;

namespace PxStat.Data
{
    /// <summary>
    /// ADO for Compare
    /// </summary>
    internal class Compare_ADO
    {
        /// <summary>
        /// 
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Compare_ADO(ADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Read the previous release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        internal int ReadPreviousRelease(int releaseCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@RlsCode", value = releaseCode }
            };

            var reader = ado.ExecuteReaderProcedure("Data_Release_ReadPrevious", inputParams);

            if (reader.hasData)
            {
                return reader.data[0].RlsCode;
            }

            return 0;
        }

    }
}
