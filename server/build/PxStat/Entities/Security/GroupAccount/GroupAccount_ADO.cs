using System.Collections.Generic;
using API;
using Newtonsoft.Json;
using System.Data;

namespace PxStat.Security
{
    /// <summary>
    /// IADO classes for GroupAccount
    /// </summary>
    internal class GroupAccount_ADO
    {
        /// <summary>
        /// Reads Group Account data (TM_GROUP_ACCOUNT table)
        /// Both the GroupID and CcnUsername parameters are optional
        /// If no parameters are sent, all data is returned
        /// If just the GroupId parameter is sent then all user/group combinations for that group are returned
        /// If just the CcnUsername parameter is sent then all user/group combinations for that user are returned
        /// If both parameters are sent, then the user/group combination for that user and group are returned (if it exists)
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="groupAccount"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(IADO ado, GroupAccount_DTO_Read groupAccount)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();

            if (!string.IsNullOrEmpty(groupAccount.CcnUsername))
                paramList.Add(new ADO_inputParams() { name = "@CcnUsername", value = groupAccount.CcnUsername });
            if (!string.IsNullOrEmpty(groupAccount.GrpCode))
                paramList.Add(new ADO_inputParams() { name = "@GrpCode", value = groupAccount.GrpCode });

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Security_GroupAccount_Read", paramList);

            //Read the result of the call to the database
            if (output.hasData)
            {
                Log.Instance.Debug("Data found");
            }
            else
            {
                //No data found
                Log.Instance.Debug("No data found");
            }

            //return the list of entities that have been found
            return output;
        }


        /// <summary>
        /// Reads Group and Account information for a supplied list of Group Codes
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="groups"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadMultiple(IADO ado, List<string> groups)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();

            DataTable dt = new DataTable();
            //dt.Columns.Add("K_VALUE");
            //dt.Columns.Add("Key");
            dt.Columns.Add("Value");

            //We can now add each search term as a row in the table
            foreach (string group in groups)
            {
                var row = dt.NewRow();
                //row["K_VALUE"] = word;
                //row["Key"] = null;
                row["Value"] = group;
                dt.Rows.Add(row);

            }

            ADO_inputParams param = new ADO_inputParams() { name = "@GroupList", value = dt };
            param.typeName = "ValueVarchar";
            paramList.Add(param);

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Security_GroupAccount_ReadMultiple", paramList);

            //Read the result of the call to the database
            if (output.hasData)
            {
                Log.Instance.Debug("Data found");
            }
            else
            {
                //No data found
                Log.Instance.Debug("No data found");
            }

            //return the list of entities that have been found
            return output;
        }

        /// <summary>
        /// Updates a Group account entry, i.e. changes the value of the Approver Flag
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="groupAccount"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Update(IADO ado, GroupAccount_DTO_Update groupAccount, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsernameUpdater",value=ccnUsername},
                new ADO_inputParams() {name= "@CcnUsernameUpdatedUser",value=groupAccount.CcnUsername},
                new ADO_inputParams() {name= "@GrpCode",value=groupAccount.GrpCode},
                new ADO_inputParams() {name= "@GrpApproveFlag",value=groupAccount.GccApproveFlag}

            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_GroupAccount_Update", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Creates a new username /group combination
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="groupAccount"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Create(IADO ado, GroupAccount_DTO_Create groupAccount, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsernameAdder",value=ccnUsername},
                new ADO_inputParams() {name= "@CcnUsernameAddedUser",value=groupAccount.CcnUsername},
                new ADO_inputParams() {name= "@GrpCode",value=groupAccount.GrpCode},
                new ADO_inputParams() {name= "@GrpApproveFlag",value=groupAccount.GccApproveFlag}

            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_GroupAccount_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }
        /// <summary>
        /// Deletes a username/group combination, i.e. removes an entity from the TM_GROUP_ACCOUNT table
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="groupAccount"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Delete(IADO ado, GroupAccount_DTO_Delete groupAccount, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsernameDeleter",value=ccnUsername},
                new ADO_inputParams() {name= "@CcnUsernameDeletedUser",value=groupAccount.CcnUsername},
                new ADO_inputParams() {name= "@GrpCode",value=groupAccount.GrpCode}

            };

            //Create a return parameter
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "@return";
            retParam.value = 0;

            //Executing the stored procedure
            ado.ExecuteNonQueryProcedure("Security_GroupAccount_Delete", inputParamList, ref retParam);


            Log.Instance.Debug("Number of records affected: " + retParam.value);
            return retParam.value;
        }

        /// <summary>
        /// Checks if a user/group combination already exists in the database
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        internal bool Exists(IADO ado, string ccnUsername, string grpCode)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsername",value=ccnUsername},
                new ADO_inputParams() {name= "@GrpCode",value=grpCode}

            };

            ADO_readerOutput output = ado.ExecuteReaderProcedure("Security_GroupAccount_Read", paramList);
            if (output.hasData)
            {
                return true; // the entity in question exists in the database already
            }
            else
            {
                return false; // the entity in question doesn't already exist in the database
            }
        }


    }
}