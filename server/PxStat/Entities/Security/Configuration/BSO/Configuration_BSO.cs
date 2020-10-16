using API;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Web;

namespace PxStat.Security
{
    internal enum ConfigType { global, server }

    internal static class Configuration_BSO
    {
        internal static dynamic serverFileConfig;
        internal static dynamic globalFileConfig;

        static Configuration_BSO()
        {
            UpdateConfigFromFiles();
        }


        internal static dynamic GetCustomConfig(ConfigType configType, string configName = null)
        {

            if (configType.Equals(ConfigType.global))
            {
                if (globalFileConfig == null)
                    globalFileConfig = GetGlobalConfigFromFile();
                return GetObjectFromDotName(globalFileConfig, configName);

            }
            else if (configType.Equals(ConfigType.server))
            {
                if (serverFileConfig == null)
                    serverFileConfig = GetServerConfigFromFile();
                return GetObjectFromDotName(serverFileConfig, configName);
            }
            else return configName;



        }

        internal static void SetConfigFromFiles()
        {
            serverFileConfig = GetServerConfigFromFile();
            globalFileConfig = GetGlobalConfigFromFile();
        }

        internal static dynamic GetServerConfigFromFile()
        {
            serverFileConfig = Utility.JsonDeserialize_IgnoreLoopingReference(File.ReadAllText(HttpContext.Current.Server.MapPath(Utility.GetCustomConfig("APP_CONFIG_RESOURCES_MAP_PATH")) + Utility.GetCustomConfig("APP_CONFIG_SERVER_JSON_PATH")));
            return Utility.JsonDeserialize_IgnoreLoopingReference(Utility.JsonSerialize_IgnoreLoopingReference(serverFileConfig));
        }

        internal static dynamic GetGlobalConfigFromFile()
        {
            globalFileConfig = Utility.JsonDeserialize_IgnoreLoopingReference(File.ReadAllText(HttpContext.Current.Server.MapPath(Utility.GetCustomConfig("APP_CONFIG_RESOURCES_MAP_PATH")) + Utility.GetCustomConfig("APP_CONFIG_GLOBAL_JSON_PATH")));
            return Utility.JsonDeserialize_IgnoreLoopingReference(Utility.JsonSerialize_IgnoreLoopingReference(globalFileConfig));
        }

        internal static bool UpdateConfigFromFiles()
        {
            dynamic serverConfig = Utility.JsonDeserialize_IgnoreLoopingReference(File.ReadAllText(HttpContext.Current.Server.MapPath(Utility.GetCustomConfig("APP_CONFIG_RESOURCES_MAP_PATH")) + Utility.GetCustomConfig("APP_CONFIG_SERVER_JSON_PATH")));
            dynamic globalConfig = Utility.JsonDeserialize_IgnoreLoopingReference(File.ReadAllText(HttpContext.Current.Server.MapPath(Utility.GetCustomConfig("APP_CONFIG_RESOURCES_MAP_PATH")) + Utility.GetCustomConfig("APP_CONFIG_GLOBAL_JSON_PATH")));

            MemCacheD.Store_BSO("PxStat.Security", "Configuration", "Read", ConfigType.global.ToString(), globalConfig, default(DateTime));
            MemCacheD.Store_BSO("PxStat.Security", "Configuration", "Read", ConfigType.server.ToString(), serverConfig, default(DateTime));

            return true;
        }

        private static dynamic GetObjectFromDotName(dynamic configuration, string dotname = null)
        {
            if (dotname == null) return configuration;
            string[] names = dotname.Split('.');
            if (names.Length == 0) return null;
            dynamic item = configuration[names[0]];
            if (item == null) return null;
            for (int i = 1; i < names.Length; i++)
            {
                item = item[names[i]];
                if (item == null) return null;
            }
            if (item.Type == null) return item;


            if (item.Type == JTokenType.Array)
            {
                if (item.First == null) return null;

                switch (item.First.Type)
                {
                    case JTokenType.Integer:
                        return item.ToObject<int[]>();
                    case JTokenType.String:
                        return item.ToObject<string[]>();
                    case JTokenType.Boolean:
                        return item.ToObject<bool[]>();
                    case JTokenType.Float:
                        return item.ToObject<float[]>();
                    case JTokenType.Object:
                        return item.ToObject<object[]>();
                    case JTokenType.Date:
                        return item.ToObject<DateTime[]>();
                    case JTokenType.Bytes:
                        return item.ToObject<byte[]>();
                    case JTokenType.Array:
                        return item;
                    default:
                        return item;
                }

            }

            else
            {
                switch (item.Type)
                {
                    case JTokenType.Integer:
                        if (item.Value <= 2147483647 && item.Value >= -2147483647)
                            return (int)item.Value;
                        else
                            return item.Value;
                    case JTokenType.String:
                        return (string)item.Value;
                    default:
                        return item.Value;

                }
            }
        }


    }
}
