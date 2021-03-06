﻿using API;
using PxStat.Security;
using System;
using System.Collections.Generic;

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

        internal Dictionary<string, string> ReadDimensionRoles(string contvariable, string matrix = null, int rlsCode = 0, string lngIsoCode = null)
        {
            var inputParams = new List<ADO_inputParams>();

            Dictionary<string, string> dimDictionary = new Dictionary<string, string>();

            inputParams.Add(new ADO_inputParams { name = "@ContentVariable", value = contvariable });

            if (matrix != null)
            {
                inputParams.Add(new ADO_inputParams { name = "@MtrCode", value = matrix });
            }
            else
            {
                inputParams.Add(new ADO_inputParams { name = "@RlsCode", value = rlsCode });

            }

            if (lngIsoCode != null)
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCode", value = lngIsoCode });
            }
            else
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCode", value = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code") });

            var output = ado.ExecuteReaderProcedure("Data_Matrix_ReadDimensionRole", inputParams);

            foreach (var role in output.data)
            {

                if (!DBNull.Value.Equals(role.ROLE))
                {
                    if (!dimDictionary.ContainsKey(role.ROLE))
                        dimDictionary.Add(role.ROLE, role.CODE);
                }
            }

            return dimDictionary;


        }

        /// <summary>
        /// Fill a Matrix based on available dimensions
        /// </summary>
        /// <param name="theMatrix"></param>
        /// <returns></returns>
        internal Matrix ReadCubeData(Matrix theMatrix)
        {
            List<KeyValuePair<string, string>> clsVrbList = new List<KeyValuePair<string, string>>();
            foreach (var classification in theMatrix.MainSpec.Classification)
            {
                if (classification.ClassificationFilterWasApplied)
                {
                    foreach (var variable in classification.Variable)
                    {
                        clsVrbList.Add(new KeyValuePair<string, string>(classification.Code, variable.Code));
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

            theMatrix.Cells = new Matrix_ADO(ado).ReadDataByRelease(theMatrix.ReleaseCode, theMatrix.TheLanguage, clsVrbList, prdList, sttList);

            return theMatrix;
        }


        /// <summary>
        /// Reads the latest releases
        /// </summary>
        /// <param name="languageCode"></param>
        /// <param name="DateFrom"></param>
        /// <returns></returns>
        internal List<dynamic> ReadCollection(string languageCode, DateTime DateFrom, int SbjCode, string PrcCode = null)
        {
            var inputParams = new List<ADO_inputParams>();

            inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code") });

            if (!string.IsNullOrEmpty(languageCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeRead", value = languageCode });
            }

            if (DateFrom != default)
            {
                inputParams.Add(new ADO_inputParams { name = "@DateFrom", value = DateFrom });
            }


            inputParams.Add(new ADO_inputParams { name = "@SbjCode", value = SbjCode });
            inputParams.Add(new ADO_inputParams { name = "@PrcCode", value = PrcCode });


            var output = ado.ExecuteReaderProcedure("Data_Release_ReadListLive", inputParams);

            if (output.hasData)
            {
                return output.data;
            }

            return null;
        }

        internal List<dynamic> ReadCollectionMetadata(string languageCode, DateTime DateFrom, string PrcCode = null, bool meta = true)
        {
            var inputParams = new List<ADO_inputParams>();

            inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code") });

            if (!string.IsNullOrEmpty(languageCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCodeRead", value = languageCode });
            }

            if (DateFrom != default(DateTime))
            {
                inputParams.Add(new ADO_inputParams { name = "@DateFrom", value = DateFrom });
            }

            if (PrcCode != null)
            {
                inputParams.Add(new ADO_inputParams { name = "@PrcCode", value = PrcCode });
            }

            ADO_readerOutput output;
            if (meta)
                output = ado.ExecuteReaderProcedure("Data_Release_ReadMetaCollection", inputParams);
            else
                output = ado.ExecuteReaderProcedure("Data_Release_ReadCollection", inputParams);

            if (output.hasData)
            {
                return output.data;
            }

            return null;
        }


    }
}
