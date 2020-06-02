using API;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Web;

namespace PxStat.Security
{
    internal static class Configuration_BSO
    {
        internal static dynamic configuration;
        internal static dynamic configurationGlobal;
        static Configuration_BSO()
        {
            UpdateStaticConfig();
        }
        internal static dynamic GetCustomConfig(string configName = null, bool updateFromFile = false)
        {
            if (updateFromFile) UpdateStaticConfig();
            return GetObjectFromDotName(configuration, configName ?? null);
            //return Utility.JsonSerialize_IgnoreLoopingReference(GetObjectFromDotName(configuration, configName ?? null)).ToString();
        }


        internal static bool UpdateStaticConfig()
        {
            configuration = Utility.JsonDeserialize_IgnoreLoopingReference(File.ReadAllText(HttpContext.Current.Server.MapPath("~/Resources/Config") + "\\config.server.json"));
            configurationGlobal = Utility.JsonDeserialize_IgnoreLoopingReference(File.ReadAllText(HttpContext.Current.Server.MapPath("~/Resources/Config") + "\\config.global.json"));


            configuration.Merge(configurationGlobal, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });
            return true;
        }






        private static dynamic GetObjectFromDotName(dynamic json, string dotname = null)
        {

            if (dotname == null) return json;
            string[] names = dotname.Split('.');
            if (names.Length == 0) return null;
            dynamic item = json[names[0]];
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
