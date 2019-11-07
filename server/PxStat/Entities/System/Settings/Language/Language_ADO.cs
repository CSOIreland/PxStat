using System.Collections.Generic;
using API;

namespace PxStat.System.Settings
{
    /// <summary>
    /// 
    /// </summary>
    internal class Language_ADO
    {
        /// <summary>
        /// 
        /// </summary>
        private ADO ado;
        internal Language_ADO(ADO theAdo)
        {
            ado = theAdo;
        }

        /// <summary>
        /// This method will delete an entity from the database. For this to happen, the entity must already exist in the database
        /// The method returns the number of entities deleted
        /// </summary>
        /// <param name="language"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Delete(Language_DTO_Delete language, string ccnUsername)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LngIsoCode",value=language.LngIsoCode},
                new ADO_inputParams() {name= "@DeleteCcnUsername",value=ccnUsername}
            };

            //Create a return parameter
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "@return";
            retParam.value = 0;

            //Executing the stored procedure
            ado.ExecuteNonQueryProcedure("System_Settings_Language_Delete", inputParamList, ref retParam);

            return retParam.value;
        }

        /// <summary>
        /// Updates a language entity. Only the LngIsoName can be changed.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Update(Language_DTO_Update language, string ccnUsername)
        {

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LngIsoCode",value=language.LngIsoCode},
                new ADO_inputParams() {name= "@LngIsoName",value=language.LngIsoName},
                new ADO_inputParams() {name= "@UpdateCcnUsername",value=ccnUsername}
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("System_Settings_Language_Update", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// </summary>
        /// <param name="releaseCode"></param>
        /// <returns></returns>
        internal IList<dynamic> ReadListByRelease(int releaseCode)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            //The ExecuteReaderProcedure method requires that the parameters be contained in a List<ADO_inputParams>
            List<ADO_inputParams> paramList = new List<ADO_inputParams>();

            paramList.Add(new ADO_inputParams() { name = "@RlsCode", value = releaseCode });

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("System_Settings_Language_ReadListByRelease", paramList);

            //return the list of entities that have been found
            return output.data;
        }

        /// <summary>
        /// The Read method uses the Language_DTO_Read object and passes the LngIsoCode property to the stored procedure
        /// If the LngIsoCode property is null, this is recognised by the stored procedure as a request for a list of ALL language entities
        /// The method returns a list of Language_DTO objects, which may be empty (i.e. if no corresponding data is found)
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(Language_DTO_Read language)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            //The ExecuteReaderProcedure method requires that the parameters be contained in a List<ADO_inputParams>
            List<ADO_inputParams> paramList = new List<ADO_inputParams>();

            if (!string.IsNullOrEmpty(language.LngIsoCode))
                paramList.Add(new ADO_inputParams() { name = "@LngIsoCode", value = language.LngIsoCode });

            //Call the stored procedure
            output = ado.ExecuteReaderProcedure("System_Settings_Language_Read", paramList);

            //return the list of entities that have been found
            return output;
        }

        /// <summary>
        /// Creates a new language with the validated input parameters as supplied by the client
        /// The parameters must include LngIsoCode and LngIsoName
        /// If a language already exists i.e. its LngIsoCode is already in the database, the Create request is refused
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="ccnUsername"></param>
        /// <returns></returns>
        internal int Create(Language_DTO_Create language, string ccnUsername)
        {

            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LngIsoCode",value=language.LngIsoCode},
                new ADO_inputParams() {name= "@LngIsoName",value=language.LngIsoName},
                new ADO_inputParams() {name= "@CcnUsername",value=ccnUsername}
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            //Attempting to create the new entity
            ado.ExecuteNonQueryProcedure("System_Settings_Language_Create", inputParamList, ref retParam);

            //Assign the returned value for checking and output
            return retParam.value;
        }

        /// <summary>
        /// Sometimes we need to check if an entity exists before we can proceed (e.g. before a Create - to prevent creation of duplicate entities)
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="isoCode"></param>
        /// <returns></returns>
        internal bool Exists(string isoCode)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LngIsoCode",value=isoCode}
            };

            ADO_readerOutput output = ado.ExecuteReaderProcedure("System_Settings_Language_Read", inputParamList);
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
        /// This method tells us whether or not a language is in use, i.e. is there one or more entries in the TD_MATRIX table referencing that language
        /// 
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="isoCode"></param>
        /// <returns></returns>
        internal bool IsInUse(string isoCode)
        {
            List<ADO_inputParams> inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@LngIsoCode",value=isoCode}
            };

            // A return parameter is required for the operation
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            ado.ExecuteNonQueryProcedure("System_Settings_Language_Usage", inputParamList, ref retParam);

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