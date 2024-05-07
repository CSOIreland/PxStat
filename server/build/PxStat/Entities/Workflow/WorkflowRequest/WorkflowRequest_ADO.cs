using API;
using DocumentFormat.OpenXml.Wordprocessing;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;

namespace PxStat.Workflow
{

    /// <summary>
    /// 
    /// </summary>
    internal class WorkflowRequest_ADO : DataAdaptor
    {
        /// <summary>
        /// Reads all workflow requests for a given Release Code. You may also optionally filter by CurrentFlag (true or false)
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="RlsCode"></param>
        /// <param name="CurrentFlag"></param>
        /// <returns></returns>
        internal List<WorkflowRequest_DTO> Read(IADO ado, int RlsCode, bool? CurrentFlag = null)
        {
            var inputParams = new List<ADO_inputParams>();

            inputParams.Add(new ADO_inputParams { name = "@RlsCode", value = RlsCode });
            if (CurrentFlag != null)
                inputParams.Add(new ADO_inputParams { name = "@WrqCurrentFlag", value = CurrentFlag });


            List<WorkflowRequest_DTO> resultList = new List<WorkflowRequest_DTO>();
            var reader = ado.ExecuteReaderProcedure("Workflow_WorkflowRequest_Read", inputParams);

            foreach (var element in reader.data)
            {
                Security.ActiveDirectory_DTO requestUser = new Security.ActiveDirectory_DTO() { CcnUsername = ReadString(element.CcnUsername) };
                Security.ActiveDirectory_ADO accAdo = new Security.ActiveDirectory_ADO();
                Security.Account_DTO_Read accDto = new Security.Account_DTO_Read() { CcnUsername = requestUser.CcnUsername };

                requestUser = accAdo.GetUser(ado, accDto);

                //This may be a local user...
                if (requestUser.CcnUsername == null)
                {
                    requestUser = GetRequestUser(ado, element, requestUser);
                }
                if (requestUser != null)
                {
                    resultList.Add(
                         new WorkflowRequest_DTO()
                         {
                             WrqDatetime = ReadDateTime(element.WrqDatetime),
                             WrqExceptionalFlag = ReadBool(element.WrqExceptionalFlag),
                             WrqReservationFlag = ReadBool(element.WrqReservationFlag),
                             WrqArchiveFlag = ReadBool(element.WrqArchiveFlag),
                             WrqExperimentalFlag = ReadBool(element.WrqExperimentalFlag),
                             WrqCurrentFlag = ReadBool(element.WrqCurrentFlag),
                             RqsCode = ReadString(element.RqsCode),
                             RqsValue = ReadString(element.RqsValue),
                             RequestAccount = requestUser
                         }
                     );
                }

            }
            return resultList;
        }

        /// <summary>
        /// Get the request user
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="element"></param>
        /// <param name="requestUser"></param>
        /// <returns></returns>
        public ActiveDirectory_DTO GetRequestUser(IADO ado, dynamic element, ActiveDirectory_DTO requestUser)
        {
            Account_ADO account_ADO = new Account_ADO();
            var ac = account_ADO.Read(ado, ReadString(element.CcnUsername));
            if (ac.hasData)
            {
                if (!((IDictionary<string, object>)ac.data[0]).ContainsKey("CcnUsername"))
                {
                    // This is an edge case. It will only happen if the account of the original creator of the workflow
                    // has been removed from the database, before the workflow has been released 
                    Log.Instance.Error("User is not an AD User");
                    return null;
                }
                var displayName = (ac.data[0].CcnDisplayName.Equals( DBNull.Value)) ? "" : ac.data[0].CcnDisplayName;
                var email = (ac.data[0].CcnEmail.Equals(DBNull.Value)) ? "" : ac.data[0].CcnEmail;
                requestUser = new Security.ActiveDirectory_DTO() { CcnUsername = ac.data[0].CcnUsername, CcnDisplayName = displayName, CcnEmail = email };
            }
            return requestUser;
        }

        /// <summary>
        /// Creates a Workflow Request
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Create(IADO ado, WorkflowRequest_DTO dto, string ccnUsername)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername},
                new ADO_inputParams() {name ="@RqsCode",value= dto.RqsCode},
                new ADO_inputParams() {name ="@CmmCode",value= dto.CmmCode},
                new ADO_inputParams() {name ="@RlsCode",value= dto.RlsCode} };

            if (dto.WrqDatetime != default(DateTime))
                inputParams.Add(new ADO_inputParams() { name = "@WrqDatetime", value = dto.WrqDatetime });
            if (dto.WrqExceptionalFlag != null)
                inputParams.Add(new ADO_inputParams() { name = "@WrqExceptionalFlag", value = dto.WrqExceptionalFlag });
            if (dto.WrqReservationFlag != null)
                inputParams.Add(new ADO_inputParams() { name = "@WrqReservationFlag", value = dto.WrqReservationFlag });
            if (dto.WrqArchiveFlag != null)
                inputParams.Add(new ADO_inputParams() { name = "@WrqArchiveFlag", value = dto.WrqArchiveFlag });
            if (dto.WrqExperimentalFlag != null)
                inputParams.Add(new ADO_inputParams() { name = "@WrqExperimentalFlag", value = dto.WrqExperimentalFlag });


            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Workflow_WorkflowRequest_Create", inputParams, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Updates a workflow Request - in practice this means setting the CurrentFlag to true or false
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Update(IADO ado, WorkflowRequest_DTO_Update dto, string ccnUsername)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername},
                new ADO_inputParams() {name ="@RlsCode",value= dto.RlsCode },
                new ADO_inputParams() {name ="@WrqCurrentFlag",value= dto.WrqCurrentFlag  }
            };

            if (dto.WrqArchiveFlag != null)
            {
                inputParams.Add(new ADO_inputParams() { name = "@WrqArchiveFlag", value = dto.WrqArchiveFlag });
            }

            if (dto.WrqReservationFlag != null)
            {
                inputParams.Add(new ADO_inputParams() { name = "@WrqReservationFlag", value = dto.WrqReservationFlag });
            }

            if (dto.WrqExperimentalFlag != null)
            {
                inputParams.Add(new ADO_inputParams() { name = "@WrqExperimentalFlag", value = dto.WrqExperimentalFlag });
            }

            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Workflow_WorkflowRequest_Update", inputParams, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Checks if a Current Workflow Request already exists
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal bool IsCurrent(IADO ado, int rlsCode)
        {
            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@RlsCode",value= rlsCode},

            };
            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Workflow_ReadIsCurrent", inputParams, ref retParam);

            //Assign the returned value for checking and output

            return (retParam.value != default(int));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="rlsCode"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Delete(IADO ado, int rlsCode, string ccnUsername)
        {
            //Only allow a delete if there is no response for the workflow (that's been checked in the BSO)
            //Workflow_WorkflowRequest_Delete @CcnUsername VARCHAR(256),@RlsCode INT

            var inputParams = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name ="@CcnUsername",value= ccnUsername },
                new ADO_inputParams() {name ="@RlsCode",value= rlsCode}
            };

            var retParam = new ADO_returnParam() { name = "return", value = 0 };

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Workflow_WorkflowRequest_Delete", inputParams, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }
    }
}
