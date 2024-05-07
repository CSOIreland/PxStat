using API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace PxStat.Config
{
    /// <summary>
    /// Configuration methods for use outside of API's 
    /// </summary>
    internal class Config_BSO
    {
        virtual public ICacheD GetCache()
        {
            return ApiServicesHelper.CacheD;
        }

        public string CalculateNewVersion(string val)
        {
            float f = float.Parse(val, CultureInfo.InvariantCulture.NumberFormat);
            int i = (int)Math.Round(f) + 1;
            return i.ToString();
        }

        public void WriteNewAPPVersion(IADO ado, string version)
        {
            DeployUpdate(ado, version, "APP");

            // Update memcache
            ICacheD cache = GetCache();
            cache.Store_BSO<dynamic>("APP", "Configuration", "Version", "app_config_version", version, DateTime.Today.AddDays(30));  
        }

        public void WriteNewAPIVersion(IADO ado, string version)
        {
            DeployUpdate(ado, version, "API");

            // Update memcache
            ICacheD cache = GetCache();
            cache.Store_BSO<dynamic>("API", "Configuration", "Version", "api_config_version", version, DateTime.Today.AddDays(30));
        }

        public void DeployUpdate(IADO ado, string version, string type)
        {
            var inputParamList = new List<ADO_inputParams>()
            {
                new ADO_inputParams() {name= "@app_settings_version", value = version},
                new ADO_inputParams() {name= "@config_setting_type", value = type},
                new ADO_inputParams() { name = "@auto_version", value = 1 }
            };
            var retParam = new ADO_returnParam();
            retParam.name = "return";
            retParam.value = 0;
            ado.ExecuteNonQueryProcedure("App_Setting_Deploy_Update", inputParamList, ref retParam);
        }
    }
}