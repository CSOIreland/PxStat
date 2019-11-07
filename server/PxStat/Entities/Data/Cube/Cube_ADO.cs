using System;
using System.Collections.Generic;
using API;

namespace PxStat.Data
{
    /// <summary>
    /// ADO functions for Cube
    /// </summary>
    internal class Cube_ADO
    {
        /// <summary>
        /// ADO class variable
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Cube_ADO(ADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Fill a Matrix based on available dimensions
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <returns></returns>
        internal Matrix ReadCubeData(Matrix theMatrix)
        {
            List<KeyValuePair<string, string>> vrbList = new List<KeyValuePair<string, string>>();
            foreach (var classification in theMatrix.MainSpec.Classification)
            {
                if (classification.ClassificationFilterWasApplied)
                {
                    foreach (var variable in classification.Variable)
                    {
                        vrbList.Add(new KeyValuePair<string, string>(classification.Code, variable.Code));
                    }

                }

            }

            List<string> sttList = new List<string>();
            if (theMatrix.StatFilterWasApplied)
            {
                foreach (var statistic in theMatrix.MainSpec.Statistic)
                {
                    sttList.Add(statistic.Code);
                }
            }

            List<string> prdList = new List<string>();
            if (theMatrix.TimeFilterWasApplied)
            {
                foreach (var period in theMatrix.MainSpec.Frequency.Period)
                {
                    prdList.Add(period.Code);
                }
            }

            theMatrix.Cells = new Matrix_ADO(ado).ReadDataByRelease(theMatrix.ReleaseCode, theMatrix.TheLanguage, vrbList, prdList, sttList);

            return theMatrix;
        }


        /// <summary>
        /// Reads the latest releases
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="DateFrom"></param>
        /// <returns></returns>
        internal List<dynamic> ReadCollection(string languageCode, DateTime DateFrom)
        {
            var inputParams = new List<ADO_inputParams>();

            inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeDefault", value = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE") });

            if (!string.IsNullOrEmpty(languageCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeRead", value = languageCode });
            }

            if (DateFrom != default(DateTime))
            {
                inputParams.Add(new ADO_inputParams { name = "@DateFrom", value = DateFrom });
            }


            var output = ado.ExecuteReaderProcedure("Data_Release_ReadListLive", inputParams);

            if (output.hasData)
            {
                return output.data;
            }

            return null;
        }

    }
}
