using API;
using PxStat.Resources;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace PxStat.Data
{
    /// <summary>
    /// ADO methods for Matrix
    /// </summary>
    internal class Matrix_ADO : DataAdaptor
    {
        /// <summary>
        /// class variable
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Matrix_ADO(ADO ado)
        {
            this.ado = ado;
        }



        /// <summary>
        /// Creates a Matrix
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="releaseId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int CreateMatrixRecord(Matrix_DTO dto, int releaseId, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@MtrCode",value= dto.MtrCode},
                    new ADO_inputParams() {name ="@MtrTitle",value= dto.MtrTitle},
                    new ADO_inputParams() {name ="@LngIsoCode",value= dto.LngIsoCode},
                    new ADO_inputParams() {name ="@CprValue",value= dto.CprValue},
                    new ADO_inputParams() {name ="@FrmType",value= dto.FrmType},
                    new ADO_inputParams() {name ="@FrmVersion",value= dto.FrmVersion},
                    new ADO_inputParams() {name ="@MtrOfficialFlag",value= dto.MtrOfficialFlag},
                    new ADO_inputParams() {name ="@MtrNote",value= dto.MtrNote},
                    new ADO_inputParams() {name ="@MtrInput",value= dto.MtrInput},
                    new ADO_inputParams() {name ="@MtrRlsId",value= releaseId},

                    new ADO_inputParams() {name ="@userName",value= userName},
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Matrix_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        /// <summary>
        ///  Get a list of Matrix codes based on usage of products
        /// </summary>
        /// <param name="prcCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadByProduct(string prcCode, string lngIsoCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@PrcCode", value = prcCode },
                new ADO_inputParams { name = "@LngIsoCode", value = lngIsoCode },
                new ADO_inputParams { name = "@LngIsoCodeDefault", value = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE") }

            };

            return ado.ExecuteReaderProcedure("Data_Matrix_ReadByProduct", inputParams);
        }

        /// <summary>
        /// Reads a Matrix
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="languageCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal dynamic Read(int releaseCode, string languageCode, string userName)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@CcnUsername", value = userName },
                new ADO_inputParams { name = "@RlsCode", value = releaseCode },
                new ADO_inputParams { name = "@LngIsoCode", value = languageCode }
            };

            var reader = ado.ExecuteReaderProcedure("Data_Matrix_Read", inputParams);

            if (reader.hasData)
            {
                if (reader.data.Count == 1)
                    return reader.data[0];
                else if (reader.data.Count > 1)
                {
                    //We will return the matrix in the preferred language if possible, otherwise we will just revert to the version in the default language.
                    var vPreferred =
                    from x in reader.data
                    where x.LngIsoCode == languageCode
                    select x;

                    if (vPreferred.ToList().Count > 0) return vPreferred.ToList()[0];

                    var vDefault =
                    from x in reader.data
                    where x.LngIsoCode == Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE")
                    select x;

                    if (vDefault.ToList().Count > 0)
                        return vDefault.ToList()[0];

                }
            }

            return null;
        }

        /// <summary>
        /// Gets all matrices for a specific Release Code - there is usually just one non-deleted matrix per release 
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal IList<dynamic> ReadAllForRelease(int rlsCode, string userName)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@CcnUsername", value = userName },
                new ADO_inputParams { name = "@RlsCode", value = rlsCode }
            };

            var reader = ado.ExecuteReaderProcedure("Data_Matrix_ReadAllForRelease", inputParams);

            return reader.data;
        }

        /// <summary>
        /// Runs an SQL Job to clean up old data after an updated release
        /// </summary>
        /// <param name="processName"></param>
        internal void DeleteEntities(string processName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@job_name",value= processName}
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("sp_start_job", inputParams, ref returnParam);


        }

        /// <summary>
        /// Returns the SqlConnection
        /// </summary>
        /// <returns></returns>
        internal string getDbName()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString).Database;

        }

        /// <summary>
        /// Soft deletes a Matrix
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Delete(int rlsCode, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@RlsCode",value= rlsCode},
                    new ADO_inputParams() {name ="@CcnUsername",value= userName}
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Matrix_Delete", inputParams, ref returnParam);

            return ReadInt(returnParam.value);
        }

        /// <summary>
        /// Returns the list of matrices base on the user profile
        /// </summary>
        /// <param name="jsonrpcRequest">The request param</param>
        /// <returns>TODO</returns>
        internal IList<dynamic> ReadCodeList(string userName)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams { name = "@CcnUsername", value = userName }
            };

            var reader = ado.ExecuteReaderProcedure("Data_Matrix_ReadCodeList", inputParams);

            return reader.data;
        }

        /// <summary>
        /// Creates a Statistic object in the database
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="matrixId"></param>
        /// <returns></returns>
        internal int CreateStatisticalRecord(StatisticalRecordDTO_Create dto, int matrixId)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@SttMtrId",value= matrixId},
                    new ADO_inputParams() {name ="@SttValue",value= dto.Value},
                    new ADO_inputParams() {name ="@SttCode",value= dto.Code},
                    new ADO_inputParams() {name ="@SttUnit",value= dto.Unit},
                    new ADO_inputParams() {name ="@SttDecimal",value = dto.Decimal }
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Statistic_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        /// <summary>
        /// Creates a Classfication record in the database
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="matrixId"></param>
        /// <returns></returns>
        internal int CreateClassificationRecord(ClassificationRecordDTO_Create dto, int matrixId)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@ClsMtrId",value= matrixId},
                    new ADO_inputParams() {name ="@ClsValue",value= dto.Value},
                    new ADO_inputParams() {name ="@ClsCode",value= dto.Code},
                    new ADO_inputParams() {name ="@ClsGeoFlag",value= dto.GeoFlag}
                };

            if (!string.IsNullOrEmpty(dto.GeoUrl))
            {
                inputParams.Add(new ADO_inputParams() { name = "@ClsGeoUrl", value = dto.GeoUrl });
            }

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Classification_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }


        /// <summary>
        /// Bulk uploads the Variables to the database
        /// </summary>
        /// <param name="dt"></param>
        internal void CreateVariableRecordBulk(DataTable dt)
        {
            var maps = new List<SqlBulkCopyColumnMapping>()
                {
                    new SqlBulkCopyColumnMapping("VRB_CODE", "VRB_CODE"),
                    new SqlBulkCopyColumnMapping("VRB_VALUE", "VRB_VALUE"),
                    new SqlBulkCopyColumnMapping("VRB_CLS_ID", "VRB_CLS_ID")
                };

            ado.ExecuteBulkCopy("TD_VARIABLE", maps, dt, true);
        }

        /// <summary>
        /// Bulk uploads the Periods to the database
        /// </summary>
        /// <param name="dt"></param>
        internal void CreatePeriodRecordBulk(DataTable dt)
        {
            var maps = new List<SqlBulkCopyColumnMapping>()
                {
                    new SqlBulkCopyColumnMapping("PRD_CODE", "PRD_CODE"),
                    new SqlBulkCopyColumnMapping("PRD_VALUE", "PRD_VALUE"),
                    new SqlBulkCopyColumnMapping("PRD_FRQ_ID", "PRD_FRQ_ID")
                };

            ado.ExecuteBulkCopy("TD_PERIOD", maps, dt, true);
        }

        /// <summary>
        /// Reads variables
        /// </summary>
        /// <param name="clsID"></param>
        /// <returns></returns>
        internal List<VariableRecordDTO_Create> ReadClassificationVariables(int clsID)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@ClsID",value= clsID}
                };

            var clsVars = ado.ExecuteReaderProcedure("Data_Variable_Read", inputParams);

            var dtoList = new List<VariableRecordDTO_Create>();

            foreach (var item in clsVars.data)
            {
                VariableRecordDTO_Create dto = new VariableRecordDTO_Create();
                dto.ClassificationVariableId = ReadInt(item.VrbID);
                dto.Code = ReadString(item.VrbCode);
                dto.Value = ReadString(item.VrbValue);
                dtoList.Add(dto);
            }

            return dtoList;
        }

        /// <summary>
        /// Createws the Frequency record in the database
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="matrixId"></param>
        /// <returns></returns>
        internal int CreateFrequencyRecord(FrequencyRecordDTO_Create dto, int matrixId)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@FrqValue",value= dto.Value},
                    new ADO_inputParams() {name ="@FrqCode",value= dto.Code},
                    new ADO_inputParams() {name ="@FrqMtrId",value= matrixId},
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Frequency_Create", inputParams, ref returnParam);

            return (int)returnParam.value;
        }

        internal Task CreateTdtDataAndCellRecords(DataTable dataTable, DataTable dataCellTable)
        {

            var mappings = new List<SqlBulkCopyColumnMapping>()
                {
                    new SqlBulkCopyColumnMapping("TDT_VALUE", "TDT_VALUE"),
                    new SqlBulkCopyColumnMapping("TDT_STT_ID", "TDT_STT_ID"),
                    new SqlBulkCopyColumnMapping("TDT_PRD_ID", "TDT_PRD_ID"),
                    new SqlBulkCopyColumnMapping("TDT_IX", "TDT_IX"), // only exists with [TM_DATA_CELL]
                    new SqlBulkCopyColumnMapping("TDT_MTR_ID", "TDT_MTR_ID")
                };

            ado.ExecuteBulkCopy("TD_DATA", mappings, dataTable, true);

            var mappingCells = new List<SqlBulkCopyColumnMapping>()
                {
                    new SqlBulkCopyColumnMapping("DTC_TDT_IX", "DTC_TDT_IX"),
                    new SqlBulkCopyColumnMapping("DTC_MTR_ID", "DTC_MTR_ID"),
                    new SqlBulkCopyColumnMapping("DTC_VRB_ID", "DTC_VRB_ID")
                };


            ado.ExecuteBulkCopy("TM_DATA_CELL", mappingCells, dataCellTable, true);

            dataTable = null;
            dataCellTable = null;

            return Task.CompletedTask;
        }

        internal void MarkMatrixAsContainingData(int mtrId, bool containsData)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@MtrDataFlag",value= containsData},
                    new ADO_inputParams() {name ="@MtrId",value= mtrId},
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Matrix_Update_Data_Flag", inputParams, ref returnParam);

        }


        /// <summary>
        /// Gets a list of Periods for a matrix
        /// </summary>
        /// <param name="matrixId"></param>
        /// <returns></returns>
        internal IList<dynamic> ReadPeriodsByMatrixId(int matrixId)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@MatrixId", value = matrixId },

            };


            var reader = ado.ExecuteReaderProcedure("Data_Matrix_ReadPeriodByMatrix", inputParams);

            return reader.data;
        }


        /// <summary>
        /// Gets Statistics for a Release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="languageIsoCode"></param>
        /// <returns></returns>
        internal IList<dynamic> ReadStatisticByRelease(int releaseCode, string languageIsoCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@RlsCode", value = releaseCode },

            };

            if (!String.IsNullOrEmpty(languageIsoCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCode", value = languageIsoCode });
            }

            var reader = ado.ExecuteReaderProcedure("Data_Matrix_ReadStatisticByRelease", inputParams);

            return reader.data;
        }

        /// <summary>
        /// Gets Classfications for a Release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="languageIsoCode"></param>
        /// <returns></returns>
        internal IList<dynamic> ReadClassificationByRelease(int releaseCode, string languageIsoCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@RlsCode", value = releaseCode },
            };

            if (!String.IsNullOrEmpty(languageIsoCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCode", value = languageIsoCode });
            }

            var reader = ado.ExecuteReaderProcedure("Data_Matrix_ReadClassificationByRelease", inputParams);

            return reader.data;
        }

        /// <summary>
        /// Get Variables for a release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="languageIsoCode"></param>
        /// <param name="classificationCode"></param>
        /// <returns></returns>
        internal IList<dynamic> ReadVariableByRelease(int releaseCode, string languageIsoCode, string classificationCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@RlsCode", value = releaseCode },
            };

            if (!String.IsNullOrEmpty(languageIsoCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCode", value = languageIsoCode });
            }

            if (!String.IsNullOrEmpty(classificationCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@ClsCode", value = classificationCode });
            }

            var reader = ado.ExecuteReaderProcedure("Data_Matrix_ReadVariableByRelease", inputParams);

            return reader.data;
        }

        /// <summary>
        /// Get Periods for a Release
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="languageIsoCode"></param>
        /// <param name="frequencyCode"></param>
        /// <returns></returns>
        internal IList<dynamic> ReadPeriodByRelease(int releaseCode, string languageIsoCode, string frequencyCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@RlsCode", value = releaseCode },
            };

            if (!String.IsNullOrEmpty(languageIsoCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@LngIsoCode", value = languageIsoCode });
            }

            if (!String.IsNullOrEmpty(frequencyCode))
            {
                inputParams.Add(new ADO_inputParams { name = "@FrqCode", value = frequencyCode });
            }

            var reader = ado.ExecuteReaderProcedure("Data_Matrix_ReadPeriodByRelease", inputParams);

            return reader.data;
        }

        /// <summary>
        /// Gets a Matrix_DTO based on a read from the database
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        static internal Matrix_DTO GetMatrixDTO(dynamic element)
        {
            if (element == null) return null;

            var matrix = new Matrix_DTO();

            matrix.MtrCode = ReadString(element.MtrCode);
            matrix.MtrTitle = ReadString(element.MtrTitle);
            matrix.MtrNote = ReadString(element.MtrNote);
            matrix.MtrOfficialFlag = ReadBool(element.MtrOfficialFlag);
            matrix.MtrInput = ReadString(element.MtrInput);
            matrix.CcnUsername = ReadString(element.CcnUsernameCreate);
            matrix.DtgCreateDatetime = ReadDateTime(element.DtgCreateDatetime);
            matrix.CcnUsernameUpdate = ReadString(element.CcnUsernameUpdate);
            matrix.DtgUpdateDatetime = ReadDateTime(element.DtgUpdateDatetime);
            matrix.RlsCode = ReadInt(element.RlsCode);
            matrix.LngIsoCode = ReadString(element.LngIsoCode);
            matrix.LngIsoName = ReadString(element.LngIsoName);
            matrix.CprValue = ReadString(element.CprValue);
            matrix.CprCode = ReadString(element.CprCode);
            matrix.CprUrl = ReadString(element.CprUrl);

            matrix.FrqValue = ReadString(element.FrqValue);
            matrix.FrqCode = ReadString(element.FrqCode);
            matrix.FrmType = ReadString(element.FrmType);
            matrix.FrmVersion = ReadString(element.FrmVersion);

            return matrix;
        }

        /// <summary>
        /// Reads a matrix by Release Code
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        internal dynamic ReadByRelease(int releaseCode, string languageCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@RlsCode", value = releaseCode },
                new ADO_inputParams { name = "@LngIsoCode", value = languageCode }
            };

            var output = ado.ExecuteReaderProcedure("Data_Matrix_ReadByRelease", inputParams);

            if (output.hasData)
            {
                return output.data[0];
            }

            return null;
        }




        /// <summary>
        /// Reads a history of release uploads based on either the user's group or the user's privilege
        /// </summary>
        /// <param name="ccnUsername"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        internal dynamic ReadHistory(string ccnUsername, Matrix_DTO_ReadHistory dto)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@CcnUsername", value = ccnUsername },
                new ADO_inputParams { name = "@DateFrom", value = dto.DateFrom   },
                new ADO_inputParams { name = "@DateTo", value = dto.DateTo },
                new ADO_inputParams { name = "@LngIsoCode", value = dto.LngIsoCode   },
                new ADO_inputParams { name = "@LngIsoCodeDefault", value = Utility.GetCustomConfig("APP_DEFAULT_LANGUAGE")  }
            };

            var output = ado.ExecuteReaderProcedure("Data_Matrix_ReadHistory", inputParams);

            if (output.hasData)
            {
                return output.data;
            }

            return null;
        }

        /// <summary>
        /// Gets ADO parameters based on column names
        /// </summary>
        /// <param name="aList"></param>
        /// <param name="keyValueColumnNames"></param>
        /// <param name="paramName"></param>
        /// <param name="paramType"></param>
        /// <returns></returns>
        private ADO_inputParams GetDataTableParam(IList<KeyValuePair<string, string>> aList, KeyValuePair<string, string> keyValueColumnNames, string paramName, string paramType)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(keyValueColumnNames.Key);
            dt.Columns.Add(keyValueColumnNames.Value);

            foreach (var item in aList)
            {
                var row = dt.NewRow();
                row[keyValueColumnNames.Key] = item.Key;
                row[keyValueColumnNames.Value] = item.Value;
                dt.Rows.Add(row);
            }

            ADO_inputParams param = new ADO_inputParams() { name = paramName, value = dt, typeName = paramType };

            return param;
        }

        /// <summary>
        /// Gets Data parameters based on column and parameter
        /// </summary>
        /// <param name="aList"></param>
        /// <param name="columnName"></param>
        /// <param name="paramName"></param>
        /// <param name="paramType"></param>
        /// <returns></returns>
        internal ADO_inputParams GetDataTableParam(IList<string> aList, string columnName, string paramName, string paramType)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(columnName);

            foreach (string item in aList)
            {
                var row = dt.NewRow();
                row[columnName] = item;
                dt.Rows.Add(row);
            }

            ADO_inputParams param = new ADO_inputParams() { name = paramName, value = dt, typeName = paramType };

            return param;
        }

        /// <summary>
        /// Tests if an SQL Job is running
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        internal bool IsProcessRunning(string processName)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@JobName", value = processName } };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Matrix_IsJobRunning", inputParams, ref returnParam);

            return returnParam.value != 0;
        }

        /// <summary>
        /// Search for Release data based on Release code, returning the data restricted to the dimensions in vrbCodes, prdCodes and sttCodes
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="languageCode"></param>
        /// <param name="vrbCodes"></param>
        /// <param name="prdCodes"></param>
        /// <param name="sttCodes"></param>
        /// <returns></returns>
        internal IList<dynamic> ReadDataByRelease(int releaseCode, string languageCode, IList<KeyValuePair<string, string>> clsVrbCodes, IList<string> prdCodes, IList<string> sttCodes)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@RlsCode", value = releaseCode }
            };

            inputParams.Add(GetDataTableParam(sttCodes, "Value", "@SttCodeList", "ValueVarchar"));
            inputParams.Add(GetDataTableParam(prdCodes, "Value", "@PrdCodeList", "ValueVarchar"));
            inputParams.Add(GetDataTableParam(clsVrbCodes, new KeyValuePair<string, string>("Key", "Value"), "@ClsVrbCodeList", "KeyValueVarchar"));

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var reader = ado.ExecuteReaderProcedure("Data_Matrix_ReadDataByRelease", inputParams);

            sw.Stop();
            Log.Instance.Info(string.Format("Data read from DB in {0} ms", Math.Round((double)sw.ElapsedMilliseconds)));

            if (reader.hasData)
            {
                foreach (var v in reader.data)
                {
                    if (v.TdtValue == null)
                    {
                        v.TdtValue = (DBNull)v.TdtValue;
                    }
                }

                var returnVal = reader.data.OrderBy(x => x.SttId).ThenBy(x => x.PrdId).ThenBy(x => x.TdtId).ToList();
                return returnVal;
            }

            return new List<dynamic>();
        }
    }
}
