using API;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace PxStat.Data
{
    /// <summary>
    /// ADO classes for Release
    /// </summary>
    internal partial class Release_ADO : DataAdaptor
    {
        /// <summary>
        /// class variable
        /// </summary>
        private ADO ado;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ado"></param>
        public Release_ADO(ADO ado)
        {
            this.ado = ado;
        }

        /// <summary>
        /// Create a Release
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int Create(Release_DTO dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@RlsVersion",value= dto.RlsVersion},
                    new ADO_inputParams() {name ="@RlsRevision",value= dto.RlsRevision},
                    new ADO_inputParams() {name ="@GrpCode",value= dto.GrpCode},

                    new ADO_inputParams() {name ="@RlsLiveFlag",value= dto.RlsLiveFlag},
                    new ADO_inputParams() {name ="@RlsExceptionalFlag",value= dto.RlsExceptionalFlag},
                    new ADO_inputParams() {name ="@RlsReservationFlag",value= dto.RlsReservationFlag},
                    new ADO_inputParams() {name ="@RlsArchiveFlag",value= dto.RlsArchiveFlag},
                    new ADO_inputParams() {name ="@RlsExperimentalFlag",value= dto.RlsExperimentalFlag},
                    new ADO_inputParams() {name ="@RlsAnalyticalFlag",value= dto.RlsAnalyticalFlag},
                    new ADO_inputParams() {name ="@userName",value= userName},

                };

            if (dto.RlsLiveDatetimeFrom > DateTime.MinValue)
            {
                inputParams.Add(new ADO_inputParams() { name = "@RlsLiveDatetimeFrom", value = dto.RlsLiveDatetimeFrom });
            }
            if (dto.RlsLiveDatetimeTo < DateTime.MaxValue)
            {
                inputParams.Add(new ADO_inputParams() { name = "@RlsLiveDatetimeTo", value = dto.RlsLiveDatetimeTo });
            }
            if (dto.PrcCode != null)
            {
                inputParams.Add(new ADO_inputParams() { name = "@PrcCode", value = dto.PrcCode });
            }
            if (dto.CmmCode != default(int))
            {
                inputParams.Add(new ADO_inputParams() { name = "@RlsCmmCode", value = dto.CmmCode });
            }

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Release_Create", inputParams, ref returnParam);

            return ReadInt(returnParam.value);
        }

        /// <summary>
        /// Creates a comment for the Release
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int CreateComment(Release_DTO dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@CcnUsername",value= userName},
                    new ADO_inputParams() {name ="@RlsCode",value=  dto.RlsCode },
                    new ADO_inputParams() {name ="@CmmValue",value= dto.CmmValue }
                };
            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Release_Comment_Create", inputParams, ref returnParam);

            return ReadInt(returnParam.value);
        }

        /// <summary>
        /// Updates a release comment
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int UpdateComment(Release_DTO dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@CcnUsername",value= userName},
                    new ADO_inputParams() {name ="@RlsCode",value= dto.RlsCode},
                    new ADO_inputParams() {name ="@CmmValue",value= dto.CmmValue }
                };
            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Release_Comment_Update", inputParams, ref returnParam);

            return ReadInt(returnParam.value);
        }

        /// <summary>
        /// Deletes the Release Comment
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int DeleteComment(Release_DTO dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@CcnUsername",value= userName},
                    new ADO_inputParams() {name ="@RlsCode",value= dto.RlsCode}
                };
            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Release_Comment_Delete", inputParams, ref returnParam);

            return ReadInt(returnParam.value);
        }



        /// <summary>
        /// Deletes the Release search keywords
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal int DeleteKeywords(int rlsCode, string userName)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@RlsCode",value= rlsCode},
                    new ADO_inputParams() {name ="@userName",value= userName}
                };

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Release_DeleteKeywords", inputParams, ref returnParam);

            return ReadInt(returnParam.value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rlsCode"></param>
        /// <param name="userName"></param>
        /// <param name="groupCode"></param>
        /// <returns></returns>
        internal int IncrementRevision(int rlsCode, string userName, string groupCode = null)
        {
            var inputParams = new List<ADO_inputParams>()
                {
                    new ADO_inputParams() {name ="@RlsCode",value= rlsCode},
                    new ADO_inputParams() {name ="@userName",value= userName}
                };

            if (groupCode != null)
            {
                inputParams.Add(new ADO_inputParams() { name = "@GrpCode", value = groupCode });
            }

            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };

            ado.ExecuteNonQueryProcedure("Data_Release_IncrementRevision", inputParams, ref returnParam);

            return ReadInt(returnParam.value);
        }

        /// <summary>
        /// Read latest release for a matrix code
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal dynamic ReadLatest(Release_DTO_Read dto)
        {
            var inputParams = new List<ADO_inputParams>();
            inputParams.Add(new ADO_inputParams { name = "@MtrCode", value = dto.MtrCode });

            var reader = ado.ExecuteReaderProcedure("Data_Release_ReadLatest", inputParams);
            if (reader.hasData)
            {
                return reader.data[0];

            }

            return null;
        }

        /// <summary>
        /// Get the next scheduled release date
        /// </summary>
        /// <returns></returns>
        internal dynamic ReadNextReleaseDate()
        {
            var inputParams = new List<ADO_inputParams>();

            var reader = ado.ExecuteReaderProcedure("Data_Release_ReadNextReleaseDate", inputParams);
            if (reader.hasData)
            {
                return reader.data[0];

            }

            return null;
        }

        /// <summary>
        /// Read any pending live release for a matrix code
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal dynamic ReadPendingLive(Release_DTO_Read dto)
        {
            var inputParams = new List<ADO_inputParams>();
            inputParams.Add(new ADO_inputParams { name = "@MtrCode", value = dto.MtrCode });

            var reader = ado.ExecuteReaderProcedure("Data_Release_Pending_Live", inputParams);
            if (reader.hasData)
            {
                return reader.data[0];

            }

            return null;
        }

        /// <summary>
        /// Reads record(s) from the TD_RELEASE & dependent tables
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns>dynamic object from the stored procedure output</returns>
        internal dynamic Read(int releaseCode, string userName)
        {
            var inputParams = new List<ADO_inputParams>();
            inputParams.Add(new ADO_inputParams { name = "@RlsCode", value = releaseCode });
            inputParams.Add(new ADO_inputParams { name = "@CcnUsername", value = userName });

            var reader = ado.ExecuteReaderProcedure("Data_Release_Read", inputParams);
            if (reader.hasData)
            {
                return reader.data[0];
            }

            return null;
        }

        /// <summary>
        /// Read a release by Release id
        /// </summary>
        /// <param name="releaseID"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal dynamic ReadID(int releaseID, string userName)
        {
            var inputParams = new List<ADO_inputParams>();
            inputParams.Add(new ADO_inputParams { name = "@RlsID", value = releaseID });
            inputParams.Add(new ADO_inputParams { name = "@CcnUsername", value = userName });

            var reader = ado.ExecuteReaderProcedure("Data_Release_Read", inputParams);
            if (reader.hasData)
            {
                return reader.data[0];
            }

            return null;
        }

        /// <summary>
        /// Converts the record from the select statment into a Release_DTO object
        /// </summary>
        /// <param name="element">the dynamic object with the record from the select stm</param>
        /// <returns>a Release_DTO object</returns>
        internal static Release_DTO GetReleaseDTO(dynamic element)
        {
            if (element == null) return null;

            var release = new Release_DTO();

            release.MtrCode = ReadString(element.MtrCode);

            release.RlsCode = ReadInt(element.RlsCode);
            release.RlsVersion = ReadInt(element.RlsVersion);
            release.RlsRevision = ReadInt(element.RlsRevision);
            release.RlsLiveFlag = ReadBool(element.RlsLiveFlag);
            release.RlsLiveDatetimeFrom = ReadDateTime(element.RlsLiveDatetimeFrom);
            release.RlsLiveDatetimeTo = ReadDateTime(element.RlsLiveDatetimeTo);
            release.RlsExceptionalFlag = ReadBool(element.RlsExceptionalFlag);
            release.RlsReservationFlag = ReadBool(element.RlsReservationFlag);
            release.RlsArchiveFlag = ReadBool(element.RlsArchiveFlag);
            release.RlsExperimentalFlag = ReadBool(element.RlsExperimentalFlag);
            release.RlsAnalyticalFlag = ReadBool(element.RlsAnalyticalFlag);


            if (HasAttr(element, "GrpCode"))
            {
                release.GrpCode = ReadString(element.GrpCode);
            }

            if (HasAttr(element, "GrpName"))
            {
                release.GrpName = ReadString(element.GrpName);
            }

            if (HasAttr(element, "GrpContactName"))
            {
                release.GrpContactName = ReadString(element.GrpContactName);
            }

            if (HasAttr(element, "GrpContactPhone"))
            {
                release.GrpContactPhone = ReadString(element.GrpContactPhone);
            }

            if (HasAttr(element, "GrpContactEmail"))
            {
                release.GrpContactEmail = ReadString(element.GrpContactEmail);
            }

            if (HasAttr(element, "CmmValue"))
            {
                release.CmmValue = ReadString(element.CmmValue);
            }

            if (HasAttr(element, "SbjCode"))
            {
                release.SbjCode = ReadInt(element.SbjCode);
            }

            if (HasAttr(element, "SbjValue"))
            {
                release.SbjValue = ReadString(element.SbjValue);
            }

            if (HasAttr(element, "PrcCode"))
            {
                release.PrcCode = ReadString(element.PrcCode);
            }

            if (HasAttr(element, "PrcValue"))
            {
                release.PrcValue = ReadString(element.PrcValue);
            }

            if (HasAttr(element, "CmmCode"))
            {
                release.CmmCode = ReadInt(element.CmmCode);
            }

            if (HasAttr(element, "CmmValue"))
            {
                release.CmmValue = ReadString(element.CmmValue);
            }

            return release;
        }

        /// <summary>
        /// Test if a dynamic that can be cast to a Dictionary contains a requested key
        /// </summary>
        /// <param name="expando"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static bool HasAttr(ExpandoObject expando, string key)
        {
            return ((IDictionary<string, Object>)expando).ContainsKey(key);
        }

        /// <summary>
        /// Clone a release and return the new id
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="groupCode"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal int Clone(int releaseCode, string groupCode, string username)
        {
            var inputParams = new List<ADO_inputParams>();
            inputParams.Add(new ADO_inputParams { name = "@RlsCode", value = releaseCode });
            inputParams.Add(new ADO_inputParams { name = "@GrpCode", value = groupCode });
            inputParams.Add(new ADO_inputParams { name = "@userName", value = username });

            //Call the stored procedure
            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };
            ado.ExecuteNonQueryProcedure("Data_Release_Clone", inputParams, ref returnParam);
            return ReadInt(returnParam.value);
        }

        /// <summary>
        /// Clone a comment for a release and return the new comment id
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <param name="groupCode"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal int CloneComment(int releaseCode, int newReleaseId, string username)
        {
            var inputParams = new List<ADO_inputParams>();
            inputParams.Add(new ADO_inputParams { name = "@RlsCode", value = releaseCode });
            inputParams.Add(new ADO_inputParams { name = "@CcnUsername", value = username });
            inputParams.Add(new ADO_inputParams { name = "@RlsIdNew", value = newReleaseId });

            //Call the stored procedure
            var returnParam = new ADO_returnParam() { name = "@ReturnVal", value = 0 };
            ado.ExecuteNonQueryProcedure("Data_Release_Clone_Comment", inputParams, ref returnParam);
            return ReadInt(returnParam.value);
        }

        /// <summary>
        /// Read a list of releases
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        internal IList<dynamic> ReadList(Release_DTO_Read dto, string userName)
        {
            var inputParams = new List<ADO_inputParams>() {
                new ADO_inputParams { name = "@CcnUsername", value = userName },
                new ADO_inputParams { name = "@MtrCode", value = dto.MtrCode },
                new ADO_inputParams { name = "@LngIsoCode", value = dto.LngIsoCode },
                new ADO_inputParams{name="@LngIsoCodeDefault",value=Configuration_BSO.GetCustomConfig(ConfigType.global,"language.iso.code")}

            };

            var reader = ado.ExecuteReaderProcedure("Data_Release_ReadList", inputParams);
            if (reader.hasData)
            {
                return reader.data;
            }

            return null;
        }
    }
}
