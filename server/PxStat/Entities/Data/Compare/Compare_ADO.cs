using API;
using System.Collections.Generic;
using System.Linq;

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
        private IADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Compare_ADO(IADO ado)
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

        /// <summary>
        /// Read the previous release code if the user has rights
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        internal int ReadPreviousReleaseForUser(int releaseCode, string ccnUsername)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@RlsCode", value = releaseCode },
                new ADO_inputParams { name = "@CcnUsername", value = ccnUsername }
            };

            var reader = ado.ExecuteReaderProcedure("Data_Release_ReadPreviousForUser", inputParams);

            if (reader.hasData)
            {
                return reader.data[0].RlsCode;
            }

            return 0;
        }

        /// <summary>
        /// Read the previous release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        internal List<dynamic>  ReadLanguagesForRelease(int releaseCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@RlsCode", value = releaseCode }
            };

            var reader = ado.ExecuteReaderProcedure("Data_Release_LanguagesForRelease", inputParams);

            if (reader.hasData)
            {
                List<dynamic> languages = reader.data.Select(x => x.LngIsoCode).ToList(); 
                return languages;
            }
            return null;
           
        }

    }
}
