using API;
using System.Collections.Generic;

namespace PxStat.Security
{
    /// <summary>
    /// ADO methods for Group
    /// </summary>
    internal class Group_ADO
    {
        internal Group_ADO() { }
        /// <summary>
        /// Reads one or more Groups
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(ADO ado, Group_DTO_Read group)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            if (!string.IsNullOrEmpty(group.GrpCode))
            {
                //We will only have one input parameter and we pass the value of the GrpCode to it:
                paramList.Add(new ADO_inputParams() { name = "@GrpCode", value = group.GrpCode });


            }
            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Security_Group_Read", paramList);

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
        /// Returns the Groups to which the ccnUsername has access
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal ADO_readerOutput ReadAccess(ADO ado, string ccnUsername)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername }

            };

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("Security_Group_ReadAccess", paramList);

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
        /// Creates a new Group
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="group"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Create(ADO ado, Group_DTO_Create group, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();
            inputParamList.Add(new ADO_inputParams() { name = "@GrpCode", value = group.GrpCode });
            inputParamList.Add(new ADO_inputParams() { name = "@GrpName", value = group.GrpName });
            inputParamList.Add(new ADO_inputParams() { name = "@CcnUsername", value = ccnUsername });
            if (!string.IsNullOrEmpty(group.GrpContactName))
                inputParamList.Add(new ADO_inputParams() { name = "@GrpContactName", value = group.GrpContactName });
            if (!string.IsNullOrEmpty(group.GrpContactPhone))
                inputParamList.Add(new ADO_inputParams() { name = "@GrpContactPhone", value = group.GrpContactPhone });
            if (!string.IsNullOrEmpty(group.GrpContactEmail))
                inputParamList.Add(new ADO_inputParams() { name = "@GrpContactEmail", value = group.GrpContactEmail });


            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Group_Create", inputParamList, ref retParam);
            Log.Instance.Debug("Called stored procedure Security_Group_Create");

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Updates an existing Group
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="group"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Update(ADO ado, Group_DTO_Update group, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>();
            inputParamList.Add(new ADO_inputParams() { name = "@GrpCodeOld", value = group.GrpCodeOld });
            inputParamList.Add(new ADO_inputParams() { name = "@GrpCodeNew", value = group.GrpCodeNew });
            inputParamList.Add(new ADO_inputParams() { name = "@GrpName", value = group.GrpName });
            inputParamList.Add(new ADO_inputParams() { name = "@UpdateCcnUsername", value = ccnUsername });
            if (!string.IsNullOrEmpty(group.GrpContactName))
                inputParamList.Add(new ADO_inputParams() { name = "@GrpContactName", value = group.GrpContactName });
            if (!string.IsNullOrEmpty(group.GrpContactPhone))
                inputParamList.Add(new ADO_inputParams() { name = "@GrpContactPhone", value = group.GrpContactPhone });
            if (!string.IsNullOrEmpty(group.GrpContactEmail))
                inputParamList.Add(new ADO_inputParams() { name = "@GrpContactEmail", value = group.GrpContactEmail });

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("Security_Group_Update", inputParamList, ref retParam);
            Log.Instance.Debug("Called stored procedure Security_Group_Update");

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Deletes a Group
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="group"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Delete(ADO ado, Group_DTO_Delete group, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@GrpCode", value = group.GrpCode },
                new ADO_inputParams() { name = "@DeleteCcnUsername", value = ccnUsername}
            };

            //Create a return parameter
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "@return";
            retParam.value = 0;

            //Executing the stored procedure
            ado.ExecuteNonQueryProcedure("Security_Group_Delete", inputParamList, ref retParam);


            Log.Instance.Debug("Number of records affected: " + retParam.value);
            return retParam.value;
        }

        /// <summary>
        /// Checks if a Group exists
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="grpCode"></param>
        /// <returns></returns>
        internal bool Exists(ADO ado, string grpCode)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@GrpCode", value = grpCode }
            };

            ADO_readerOutput output = ado.ExecuteReaderProcedure("Security_Group_Read", paramList);
            return output.hasData;

        }

        /// <summary>
        /// Checks if a Group is used by a referenced entity
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="grpCode"></param>
        /// <returns></returns>
        internal bool IsInUse(ADO ado, string grpCode)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@GrpCode", value = grpCode }
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            ado.ExecuteNonQueryProcedure("Security_Group_Usage", paramList, ref retParam);

            return retParam.value == 0;

        }
    }
}
