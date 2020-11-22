using API;
using System.Collections.Generic;
namespace PxStat.System.Settings
{
    /// <summary>
    /// ADO class for Copyright
    /// </summary>
    internal class Copyright_ADO
    {
        /// <summary>
        /// Reads a Copyright
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="copyright"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(ADO ado, Copyright_DTO_Read copyright)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            if (!string.IsNullOrEmpty(copyright.CprCode))
                paramList.Add(new ADO_inputParams() { name = "@CprCode", value = copyright.CprCode });

            if (!string.IsNullOrEmpty(copyright.CprValue))
                paramList.Add(new ADO_inputParams() { name = "@CprValue", value = copyright.CprValue });


            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("System_Settings_Copyright_Read", paramList);

            //return the list of entities that have been found
            return output;
        }

        /// <summary>
        /// Creates a Copyright
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="copyright"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Create(ADO ado, Copyright_DTO_Create copyright, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsername",value=ccnUsername},
                new ADO_inputParams() {name= "@CprCode",value=copyright.CprCode},
                new ADO_inputParams() {name= "@CprValue",value=copyright.CprValue},
                new ADO_inputParams() {name= "@CprUrl",value=copyright.CprUrl}
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("System_Settings_Copyright_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Updates a Copyright
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="copyright"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Update(ADO ado, Copyright_DTO_Update copyright, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsername",value=ccnUsername},
                new ADO_inputParams() {name= "@CprCodeOld",value=copyright.CprCodeOld},
                new ADO_inputParams() {name= "@CprCodeNew",value=copyright.CprCodeNew},
                new ADO_inputParams() {name= "@CprValue",value=copyright.CprValue},
                new ADO_inputParams() {name= "@CprUrl",value=copyright.CprUrl}
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("System_Settings_Copyright_Update", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Deletes a Copyright
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="copyright"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Delete(ADO ado, Copyright_DTO_Delete copyright, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@CcnUsername",value=ccnUsername},
                new ADO_inputParams() {name= "@CprCode",value=copyright.CprCode}
            };

            //Create a return parameter
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "@return";
            retParam.value = 0;

            //Executing the stored procedure
            ado.ExecuteNonQueryProcedure("System_Settings_Copyright_Delete", inputParamList, ref retParam);

            Log.Instance.Debug("Number of records affected: " + retParam.value);
            return retParam.value;
        }

        /// <summary>
        /// Checks if a Copyright exists
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="cprCode"></param>
        /// <returns></returns>
        internal bool Exists(ADO ado, string cprCode)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@CprCode", value = cprCode }
            };

            ADO_readerOutput output = ado.ExecuteReaderProcedure("System_Settings_Copyright_Read", paramList);
            if (output.hasData)
            {
                return true; // the entity in question exists in the database already
            }
            else
            {
                return false; // the entity in question doesn't already exist in the database
            }

        }

        /// <summary>
        /// Checks if a copyright is used by a referenced entity
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="cprCode"></param>
        /// <returns></returns>
        internal bool IsInUse(ADO ado, string cprCode)
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() { name = "@CprCode", value = cprCode }
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            ado.ExecuteNonQueryProcedure("System_Settings_Copyright_Usage", paramList, ref retParam);

            if (retParam.value == 0)
            {
                //This record is not in use 
                return false;
            }
            else
            {
                //This record is in use 
                return true;
            }
        }
    }
}
