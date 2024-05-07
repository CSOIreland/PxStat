using API;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;


// Keep it under the PxStat namespace because it's globally used
namespace PxStat
{
    static public class RequestLanguage
    {
        public static string LngIsoCode { get; set; }
    }

    static public class Label
    {
        public static ILanguagePlugin requestedLanguage;
        public static ILanguagePlugin defaultLanguage;

        /// <summary>
        /// allow a language to be injected, e.g. for unit test
        /// </summary>
        /// <param name="language"></param>
        public static void InsertLanguage(ILanguagePlugin language)
        {
            requestedLanguage = language;
        }

        public static string Get(string path,string lngIsoCode)
        {
            if (requestedLanguage == null)
                requestedLanguage = Resources.LanguageManager.GetLanguage(lngIsoCode);
            if(requestedLanguage.LngIsoCode!=lngIsoCode)
                requestedLanguage = Resources.LanguageManager.GetLanguage(lngIsoCode);

            if (defaultLanguage == null)
                defaultLanguage = Resources.LanguageManager.GetLanguage(Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code"));

            var dict = requestedLanguage.GetLabelValues();
            return ProcessGet(path,dict,lngIsoCode);
        }
        public static string Get(string path)
        {
            if (defaultLanguage == null)
                defaultLanguage = Resources.LanguageManager.GetLanguage(Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code"));
            return Get(path, defaultLanguage.LngIsoCode);
        }
        private static string ProcessGet(string label, dynamic dictionary,string lngIsoCode)
        {
            if (string.IsNullOrEmpty(label))
                return "";
            try
            {
                string[] properties = label.Split('.');
                dynamic output = dictionary; // mergedInstance;
                foreach (string property in properties)
                {
                    if (output[property] != null)
                        output = output[property];
                    else
                    // Label not found, return the Label parameter for feedback
                    {
                        if(lngIsoCode!= Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code"))
                        {
                            dictionary =defaultLanguage.GetLabelValues();
                            return ProcessGet(label, dictionary, defaultLanguage.LngIsoCode);
                        }
                        return label;
                    }
                }

                return output.ToString();
            }
            catch
            {
                return label;
            }
        }

    }


}
