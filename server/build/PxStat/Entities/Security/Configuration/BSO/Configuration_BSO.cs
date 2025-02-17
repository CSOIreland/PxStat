using AngleSharp.Css;
using API;
using DocumentFormat.OpenXml.Bibliography;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxStat.Data;
using PxStat.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PxStat.Security
{
    public enum ConfigType { global, server }

    public class Configuration_BSO
    {
        public static dynamic  serverFileConfig;
        public static dynamic globalFileConfig;
        public static List<LanguageResource> serverLanguageResource;


        private static Configuration_BSO instance;
        private static IPathProvider PathProvider;

        public static IDictionary<string, string> PxStatConfiguration
            { get; set; }   

        public static StaticConfig PxStatStaticConfig
        { get; set; }

        public static Configuration_BSO GetInstance( IPathProvider pathProvider)
        {
            
            PathProvider = pathProvider;
            if (instance == null)
            {
                instance = new Configuration_BSO();
            }
            return instance;
        }
        public static void InjectConfig(IDictionary<string, string> config)
        {
            PxStatConfiguration = config;
        }

        public static void SetServerLangaugeConfig(string appsettingsFile=null)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "");
            using StreamReader reader = new(appsettingsFile == null ? path + "appsettings.json" : appsettingsFile);

            string json = reader.ReadToEnd();
            dynamic fullConfig = JsonConvert.DeserializeObject(json);
            var cfg = fullConfig.LanguageResource.languages;
            serverLanguageResource = new List<LanguageResource>();
            foreach (var item in cfg)
            {
                LanguageResource resource = new();
                resource.ISO = (string)item.ISO;
                resource.TRANSLATION_URL= (string)item.TRANSLATION_URL;
                resource.PLUGIN_LOCATION= (string)item.PLUGIN_LOCATION; 
                resource.NAMESPACE_CLASS= (string)item.NAMESPACE_CLASS; 
                resource.IS_LIVE=(bool)item.IS_LIVE;  
                serverLanguageResource.Add(resource);
            }
        }

        public static dynamic GetStaticConfig(string itemKey)
        {
            if(PxStatStaticConfig==null)
                PxStatStaticConfig = AppServicesHelper.StaticConfig;
            PropertyInfo property = PxStatStaticConfig.GetType().GetProperty(itemKey);
            if (property == null) return "";
            return property.GetValue(PxStatStaticConfig, null);         
        }

        public static dynamic GetApplicationConfigItem(ConfigType configType, string itemKey)
        {

            if (PxStatConfiguration == null)
                PxStatConfiguration = (IDictionary<string,string>)AppServicesHelper.AppConfiguration.Settings;//.APPConfiguration.Settings;
           

            if (configType.Equals(ConfigType.server))
            {
                if (Configuration_BSO.serverFileConfig == null)
                {
                    dynamic config = PxStatConfiguration.Where(x => x.Key.Equals("config.server.json")).FirstOrDefault().Value;
                    serverFileConfig = JObject.Parse(Utility.JsonDeserialize_IgnoreLoopingReference(Utility.JsonSerialize_IgnoreLoopingReference(config)));
                }
                return GetObjectFromDotName(Configuration_BSO.serverFileConfig, itemKey) ?? itemKey;
            }
            else if (configType.Equals(ConfigType.global))
            {
                if (Configuration_BSO.globalFileConfig == null)
                {
                    dynamic config = PxStatConfiguration.Where(x => x.Key.Equals("config.global.json")).FirstOrDefault().Value;
                    Configuration_BSO.globalFileConfig = JObject.Parse(Utility.JsonDeserialize_IgnoreLoopingReference(Utility.JsonSerialize_IgnoreLoopingReference(config)));
                }
                return GetObjectFromDotName( Configuration_BSO.globalFileConfig, itemKey) ?? itemKey;
            }
            return itemKey;
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
