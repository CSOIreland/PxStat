using API;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxStat.Data
{
    /// <summary>
    /// ADO methods for Release
    /// </summary>
    internal partial class Release_ADO : DataAdaptor
    {
        /// <summary>
        /// Read live releases for a given release code
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        internal dynamic ReadLiveNow(int releaseCode)
        {
            return ReadLiveNow(releaseCode, null, null);
        }

        /// <summary>
        /// Read live releases for a given matrix code and language code
        /// </summary>
        /// <param name="matrixCode"></param>
        /// <returns></returns>
        internal dynamic ReadLiveNow(string matrixCode, string languageIsoCode)
        {
            return ReadLiveNow(0, matrixCode, languageIsoCode);

        }

        /// <summary>
        /// Read live releases for a given release, matrix and language codes
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="matrixCode"></param>
        /// <returns></returns>
        internal dynamic ReadLiveNow(int releaseCode, string matrixCode, string languageIsoCode)
        {
            var paramList = new List<ADO_inputParams>();

            if (releaseCode > 0)
            {
                paramList.Add(new ADO_inputParams { name = "@RlsCode", value = releaseCode });
            }

            if (!String.IsNullOrEmpty(matrixCode))
            {
                paramList.Add(new ADO_inputParams { name = "@MtrCode", value = matrixCode });
            }

            if (!String.IsNullOrEmpty(languageIsoCode))
            {
                paramList.Add(new ADO_inputParams { name = "@LngIsoCode", value = languageIsoCode });
            }

            ADO_readerOutput output = ado.ExecuteReaderProcedure("Data_Release_Live_Now", paramList);
            if (output.hasData)
            {
                if (output.data.Count == 1)
                    return output.data[0];
                else if (output.data.Count > 1)
                {
                    //We will return the matrix in the preferred language if possible, otherwise we will just revert to the version in the default language.
                    var vPreferred =
                    from x in output.data
                    where x.LngIsoCode == languageIsoCode
                    select x;

                    if (vPreferred.ToList().Count > 0) return vPreferred.ToList()[0];

                    var vDefault =
                    from x in output.data
                    where x.LngIsoCode == Configuration_BSO.GetCustomConfig("language.iso.code")
                    select x;

                    if (vDefault.ToList().Count > 0)
                        return vDefault.ToList()[0];

                }
            }

            return null;
        }

        /// <summary>
        /// Reads the previous live release (based on version number)
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        internal dynamic ReadLivePrevious(int releaseCode)
        {
            var paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams { name = "@RlsCode", value = releaseCode });

            ADO_readerOutput output = ado.ExecuteReaderProcedure("Data_Release_Live_Previous", paramList);
            if (output.hasData)
            {
                return output.data[0];
            }

            return null;
        }

        /// <summary>
        /// updates the Release
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Update(Release_DTO dto, string userName)
        {
            if (dto.RlsLiveDatetimeFrom == default) dto.RlsLiveFlag = false;



            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@RlsCode",value= dto.RlsCode},
                    new ADO_inputParams() {name ="@RlsVersion",value= dto.RlsVersion},
                    new ADO_inputParams() {name ="@RlsRevision",value= dto.RlsRevision},
                    new ADO_inputParams() {name ="@RlsLiveFlag",value= dto.RlsLiveFlag},
                    new ADO_inputParams() {name ="@RlsDependencyFlag",value= dto.RlsDependencyFlag},
                    new ADO_inputParams() {name ="@RlsExceptionalFlag",value= dto.RlsExceptionalFlag},
                    new ADO_inputParams() {name ="@RlsReservationFlag",value= dto.RlsReservationFlag},
                    new ADO_inputParams() {name ="@RlsArchiveFlag",value= dto.RlsArchiveFlag},
                    new ADO_inputParams() {name ="@RlsAnalyticalFlag",value= dto.RlsAnalyticalFlag},
                    new ADO_inputParams() {name ="@CcnUsername",value= userName},
                    new ADO_inputParams() {name="@PrcCode",value=dto.PrcCode!=null? dto.PrcCode : null  }
                };

            if (!dto.RlsLiveDatetimeFrom.Equals(default(DateTime)))
            {
                inputParams.Add(new ADO_inputParams() { name = "@RlsLiveDatetimeFrom", value = dto.RlsLiveDatetimeFrom });
            }
            if (!dto.RlsLiveDatetimeTo.Equals(default(DateTime)))
            {
                inputParams.Add(new ADO_inputParams() { name = "@RlsLiveDatetimeTo", value = dto.RlsLiveDatetimeTo });
            }

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Release_Update", inputParams, ref returnParam);

            return ReadInt(returnParam.value);

        }

        /// <summary>
        /// Deletes the release
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Delete(int rlsCode, string userName)
        {
            var paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams() { name = "@CcnUsername", value = userName });
            paramList.Add(new ADO_inputParams() { name = "@RlsCode", value = rlsCode });

            var retParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            //Executing the stored procedure
            ado.ExecuteNonQueryProcedure("Data_Release_Delete", paramList, ref retParam);

            Log.Instance.Debug("Number of records affected: " + retParam.value);
            return retParam.value;
        }

        /// <summary>
        /// Checks if this Release is a Work in Progress, i.e. if it is not a ".0" release
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="rlsCode"></param>
        /// <returns></returns>
        internal bool IsWip(int rlsCode)
        {
            if (rlsCode == 0) return false;

            var paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams { name = "@RlsCode", value = rlsCode });

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Release_IsWIP", paramList, ref returnParam);

            return Convert.ToBoolean(returnParam.value);
        }

        /// <summary>
        /// Checks if this Release is the current and live release for its matrix code
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <returns></returns>
        internal bool IsLiveNow(int rlsCode)
        {
            if (rlsCode == 0) return false;

            var paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams { name = "@RlsCode", value = rlsCode });

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Release_IsLiveNow", paramList, ref returnParam);

            return Convert.ToBoolean(returnParam.value);
        }
        /// <summary>
        /// Does this release have a previously live release?
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <returns></returns>
        internal bool HasPrevious(int rlsCode)
        {
            if (rlsCode == 0) return false;

            var paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams { name = "@RlsCode", value = rlsCode });


            return (ado.ExecuteReaderProcedure("Data_Release_ReadPrevious", paramList).hasData);


        }

        /// <summary>
        /// Checks if this Release is the latest future live release
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="rlsCode"></param>
        /// <returns></returns>
        internal bool IsLiveNext(int rlsCode)
        {
            if (rlsCode == 0) return false;

            var paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams { name = "@RlsCode", value = rlsCode });

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Release_IsLiveNext", paramList, ref returnParam);

            return Convert.ToBoolean(returnParam.value);
        }
    }
}
