using System;
using System.Collections;
using System.Collections.Generic;
using API;

namespace PxStat.Config
{
    /// <summary>
    /// 
    /// </summary>
    internal class Config_ADO
    {
        /// <summary>
        /// 
        /// </summary>
        private IADO ado;

        internal Config_ADO(IADO theAdo)
        {
            ado = theAdo;
        }

        /// <summary>
        /// Reads the configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal ADO_readerOutput Read(Config_DTO_Read configuration)
        {
            ADO_readerOutput output = new ADO_readerOutput();

            // APP is the default type setting
            if (string.IsNullOrEmpty(configuration.type))
            {
                configuration.type = "APP";
            }

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();

            if (configuration.type.Equals("APP"))
            {
                // If configuration.version is not null or empty, use the configuration.version parameter that was passed into the API,
                // otherwise, get the latest version of the APP Settings from the database
                configuration.version = !string.IsNullOrEmpty(configuration.version) ? configuration.version : GetLatestVersion(ado, 2); 
                paramList.Add(new ADO_inputParams() { name = "@app_settings_version", value = configuration.version });
                output = ado.ExecuteReaderProcedure("App_Settings_Read", paramList);
                    
            }
            else
            {
                // If configuration.version is not null or empty, use the configuration.version parameter that was passed into the API,
                // otherwise, get the latest version of the API Settings from the database
                configuration.version = !string.IsNullOrEmpty(configuration.version) ? configuration.version : GetLatestVersion(ado, 1);
                paramList.Add(new ADO_inputParams() { name = "@app_settings_version", value = configuration.version });
                output = ado.ExecuteReaderProcedure("Api_Settings_Read", paramList);
            }
            return output;
        }

        /// <summary>
        /// Create the configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        internal string Create(Config_DTO_Create configuration, Config_VLD_Create validation, string username)
        {
            // Check if create is for APP or API settings
            if (configuration.type == "API")
            {
                return CreateAPI(configuration, username);
            }
            Config_BSO cBso = new Config_BSO();
            ADO_readerOutput output = new ADO_readerOutput();
            string latestVersion = GetLatestVersion(ado, 2);

            if (string.IsNullOrEmpty(configuration.description))
            {
                configuration.description = "";
            }

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;
 
            // Insert new version in the database            
            string newVersion = cBso.CalculateNewVersion(latestVersion);
            paramList.Clear();
            paramList.Add(new ADO_inputParams() { name = "@version_id", value = newVersion });
            paramList.Add(new ADO_inputParams() { name = "@cst_id", value = "2" });
            ado.ExecuteReaderProcedure("App_Settings_Insert_Version", paramList);
            paramList.Clear();

            paramList.Add(new ADO_inputParams() { name = "@appversion", value = latestVersion });
            paramList.Add(new ADO_inputParams() { name = "@appkey", value = configuration.name });
            paramList.Add(new ADO_inputParams() { name = "@appvalue", value = configuration.value });
            paramList.Add(new ADO_inputParams() { name = "@appdescription", value = configuration.description });
            paramList.Add(new ADO_inputParams() { name = "@appsensitivevalue", value = configuration.sensitive });
            paramList.Add(new ADO_inputParams() { name = "@username", value = username });
            ado.ExecuteNonQueryProcedure("App_Settings_Write", paramList, ref retParam);

            // Write the new version to the Cache
            cBso.WriteNewAPPVersion(ado, newVersion);
            return newVersion;
        }

        /// <summary>
        /// Create the API configuration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        internal string CreateAPI(Config_DTO_Create configuration, string username)
        {
            Config_BSO cBso = new Config_BSO();
            string latestVersion = GetLatestVersion(ado, 1);
            ADO_readerOutput output = new ADO_readerOutput();
            if (string.IsNullOrEmpty(configuration.description))
            {
                configuration.description = "";
            }

            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            ADO_returnParam retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;

            // Insert new version in the database            
            string newVersion = cBso.CalculateNewVersion(latestVersion);
            paramList.Clear();
            paramList.Add(new ADO_inputParams() { name = "@version_id", value = newVersion });
            paramList.Add(new ADO_inputParams() { name = "@cst_id", value = "1" });
            var test = ado.ExecuteReaderProcedure("App_Settings_Insert_Version", paramList);
            paramList.Clear();

            paramList.Add(new ADO_inputParams() { name = "@apiversion", value = latestVersion });
            paramList.Add(new ADO_inputParams() { name = "@apikey", value = configuration.name });
            paramList.Add(new ADO_inputParams() { name = "@apivalue", value = configuration.value });
            paramList.Add(new ADO_inputParams() { name = "@apidescription", value = configuration.description });
            paramList.Add(new ADO_inputParams() { name = "@apisensitivevalue", value = configuration.sensitive });
            paramList.Add(new ADO_inputParams() { name = "@username", value = username });
            ado.ExecuteNonQueryProcedure("Api_Settings_Write", paramList, ref retParam);

            // Write the new version to the Cache
            cBso.WriteNewAPIVersion(ado, newVersion);
            return newVersion;
        }

        /// <summary>
        /// Get current version for create
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="cstId"></param>
        /// <returns></returns>
        public string GetLatestVersion(IADO ado, int cstId)
        {
            ADO_readerOutput output = new ADO_readerOutput();
            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            paramList.Add(new ADO_inputParams() { name = "@cst_id", value = cstId });
            output = ado.ExecuteReaderProcedure("App_Settings_Read_Version", paramList);
            paramList.Clear();
            return output.data[0].ASV_VERSION.ToString();
        }
    }
}