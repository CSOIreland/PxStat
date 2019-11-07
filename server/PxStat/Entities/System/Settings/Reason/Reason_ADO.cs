using System.Collections.Generic;
using API;

namespace PxStat.System.Settings
{
    /// <summary>
    /// ADO methods for Reason 
    /// </summary>
    internal class Reason_ADO
    {
        /// <summary>
        /// Reads a Reson entity
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(ADO ado, Reason_DTO_Read reason)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();

            if (reason.RlsCode != default(int))
            {
                paramList.Add(new ADO_inputParams() { name = "@RlsCode", value = reason.RlsCode });
            }

            if (reason.RsnCode != null)
            {
                paramList.Add(new ADO_inputParams() { name = "@RsnCode", value = reason.RsnCode });
            }

            paramList.Add(new ADO_inputParams() { name = "@LngIsoCode", value = reason.LngIsoCode });

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("System_Settings_Reason_Read", paramList);

            //return the list of entities that have been found
            return output;
        }

        /// <summary>
        /// Creates a new Reason
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="reason"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Create(ADO ado, Reason_DTO_Create reason, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@RsnCode",value=reason.RsnCode},
                new ADO_inputParams() {name= "@RsnValueInternal",value=reason.RsnValueInternal},
                new ADO_inputParams() {name= "@RsnValueExternal",value=reason.RsnValueExternal},
                new ADO_inputParams() {name="@LngIsoCode",value=reason.LngIsoCode },
                new ADO_inputParams() {name= "@CcnUsername",value=ccnUsername},

            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("System_Settings_Reason_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        internal int CreateOrUpdateReasonLanguage(ADO ado, Reason_DTO_Update reason)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@RsnCode",value=reason.RsnCode},
                new ADO_inputParams() {name= "@RlgValueInternal",value=reason.RsnValueInternal},
                new ADO_inputParams() {name= "@RlgValueExternal",value=reason.RsnValueExternal},
                new ADO_inputParams() {name="@LngIsoCode",value=reason.LngIsoCode }

            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("System_Settings_ReasonLanguage_CreateOrUpdate", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Updates a Reason
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="reason"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Update(ADO ado, Reason_DTO_Update reason, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@RsnCode",value=reason.RsnCode},
                new ADO_inputParams() {name= "@RsnValueInternal",value=reason.RsnValueInternal},
                new ADO_inputParams() {name= "@RsnValueExternal",value=reason.RsnValueExternal},
                new ADO_inputParams() {name= "@CcnUsername",value=ccnUsername},
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("System_Settings_Reason_Update", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Deletes a Reason
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="reason"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Delete(ADO ado, Reason_DTO_Delete reason, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@RsnCode",value=reason.RsnCode},
                new ADO_inputParams() {name= "@CcnUsername",value=ccnUsername},
            };

            //Create a return parameter
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "@return";
            retParam.value = 0;

            //Executing the stored procedure
            ado.ExecuteNonQueryProcedure("System_Settings_Reason_Delete", inputParamList, ref retParam);

            return retParam.value;
        }

        /// <summary>
        /// Checks if a Reason exists
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="rsnCode"></param>
        /// <returns></returns>
        internal bool Exists(ADO ado, string rsnCode)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@RsnCode",value=rsnCode}
            };

            //Create a return parameter
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "@return";
            retParam.value = 0;

            ado.ExecuteNonQueryProcedure("System_Settings_Reason_Exists", inputParamList, ref retParam);

            return retParam.value != default(int);
        }
    }
}