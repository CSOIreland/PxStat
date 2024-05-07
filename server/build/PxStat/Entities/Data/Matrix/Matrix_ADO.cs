using API;
using PxStat.Resources;
using PxStat.Security;
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
    /// IADO methods for Matrix
    /// </summary>
    internal class Matrix_ADO : DataAdaptor,IDisposable
    {
        /// <summary>
        /// class variable
        /// </summary>
        private IADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Matrix_ADO(IADO ado)
        {
            this.ado = ado;
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
                new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global,"language.iso.code")  }
            };

            var output = ado.ExecuteReaderProcedure("Data_Matrix_ReadHistory", inputParams);

            if (output.hasData)
            {
                return output.data;
            }

            return null;
        }


        /// <summary>
        ///  Get a list of Matrix codes based on rights of user groups
        /// </summary>
        /// <param name="prcCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadByGroup(string grpCode, string lngIsoCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@GrpCode", value = grpCode },
                new ADO_inputParams { name = "@LngIsoCode", value = lngIsoCode },
                new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global,"language.iso.code") }

            };

            return ado.ExecuteReaderProcedure("Data_Matrix_ReadByGroup", inputParams);
        }

        /// <summary>
        ///  Get a list of Matrix codes based on copyrights
        /// </summary>
        /// <param name="prcCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadByCopyright(string cprCode, string lngIsoCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@CprCode", value = cprCode },
                new ADO_inputParams { name = "@LngIsoCode", value = lngIsoCode },
                new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global,"language.iso.code") }

            }; return ado.ExecuteReaderProcedure("Data_Matrix_ReadByCopyright", inputParams);
        }



        /// <summary>
        ///  Get a list of Matrix codes based on copyrights
        /// </summary>
        /// <param name="prcCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadByLanguage(string lngIsoCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@LngIsoCode", value = lngIsoCode },
                new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global,"language.iso.code") }

            };

            return ado.ExecuteReaderProcedure("Data_Matrix_ReadByLanguage", inputParams);
        }

        /// <summary>
        ///  Get a list of Matrix codes based on usage of products
        /// </summary>
        /// <param name="prcCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadByProduct(string prcCode, string lngIsoCode, bool associatedOnly)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@PrcCode", value = prcCode },
                new ADO_inputParams { name = "@LngIsoCode", value = lngIsoCode },
                new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global,"language.iso.code") },
                new ADO_inputParams { name = "@AssociatedOnly", value = associatedOnly ? 1 : 0 }
            };

            return ado.ExecuteReaderProcedure("Data_Matrix_ReadByProduct", inputParams);
        }

        /// <summary>
        ///  Get a list of Matrix codes based on usage of maps
        /// </summary>
        /// <param name="prcCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadByGeoMap(string gmpCode, string lngIsoCode)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@GmpCode", value = gmpCode },
                new ADO_inputParams { name = "@LngIsoCode", value = lngIsoCode },
                new ADO_inputParams { name = "@LngIsoCodeDefault", value = Configuration_BSO.GetApplicationConfigItem(ConfigType.global,"language.iso.code") },
                new ADO_inputParams() {name ="@UrlStub",value= Configuration_BSO.GetApplicationConfigItem(ConfigType.global ,"url.api.static") + "/PxStat.Data.GeoMap_API.Read/"}

            };

            return ado.ExecuteReaderProcedure("Data_Matrix_ReadByGeoMap", inputParams);
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
                    where x.LngIsoCode == Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code")
                    select x;

                    if (vDefault.ToList().Count > 0)
                        return vDefault.ToList()[0];

                }
            }

            return null;
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
            return "";
            //return new SqlConnection(ConfigurationManager.ConnectionStrings["defaultConnection"].ConnectionString).Database;

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

        public void Dispose()
        {
            ado?.Dispose();
        }
    }



}

    internal class DatasetAdo : IDisposable
    {
        IADO _ado;
        internal DatasetAdo(IADO ado)
        {
            _ado = ado;
        }

        /// <summary>
        ///  Get the locked state of a dataset
        /// </summary>
        /// <param name="mtrCode"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadDatasetLocked(string mtrCode)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams { name = "@DttMtrCode", value = mtrCode }
            };

            return _ado.ExecuteReaderProcedure("Data_Dataset_Read", inputParams);
        }

        /// <summary>
        /// Locks or unlocks the Dataset lock record
        /// </summary>
        /// <param name="mtrCode"></param>
        /// <param name="datetimeLocked"></param>
        /// <returns></returns>
        internal bool DatasetLockUpdate(string mtrCode, DateTime datetimeLocked)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@DttMtrCode", value = mtrCode  },
            };

            if (datetimeLocked != default)
            {
                inputParams.Add(
                    new ADO_inputParams { name = "@DttDatetimeLocked", value = datetimeLocked }
                    );
            }

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            _ado.ExecuteNonQueryProcedure("Data_Dataset_Lock_Update", inputParams, ref returnParam);

            return returnParam.value != 0;
        }

        public void Dispose()
        {
        }
    }

