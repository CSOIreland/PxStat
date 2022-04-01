using API;
using Newtonsoft.Json.Linq;
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
    // Static variable that must be initialized at run time.
    static public class Label
    {
        // Mandatory language merged with the default one (fall back)
        internal static readonly dynamic mergedInstance;

        internal static readonly dynamic mergedInstances = new Dictionary<string, dynamic>();

        //The en (English) is the mandatory language, always existing
        internal static readonly dynamic mandatoryInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString("en"));
        internal static readonly dynamic defaultInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString(Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code")));

        /// <summary>
        /// Initiate Label, merge the default language with the mandatory English one which is always available (fall back)
        /// 
        /// Static constructor is called at most one time, before any instance constructor is invoked or member is accessed.
        /// </summary>
        /// <returns></returns>
        static Label()
        {
            // Initiate the merged label with the mandatory one
            mergedInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Utility.JsonSerialize_IgnoreLoopingReference(mandatoryInstance));
            mergedInstance.Merge(defaultInstance, new JsonMergeSettings
            {
                // union array values together to avoid duplicates
                MergeArrayHandling = MergeArrayHandling.Union
            });

            // Get language code resources and merge with mandatory one
            LanguageUtility languageUtility = new LanguageUtility();
            List<string> languageCodes = languageUtility.GetLanguageCodes();

            foreach (string languageCode in languageCodes)
            {
                var instance = Utility.JsonDeserialize_IgnoreLoopingReference(Properties.Resources.ResourceManager.GetString(languageCode));
                var newMergedInstance = Utility.JsonDeserialize_IgnoreLoopingReference(Utility.JsonSerialize_IgnoreLoopingReference(mandatoryInstance));
                newMergedInstance.Merge(instance, new JsonMergeSettings
                {
                    // union array values together to avoid duplicates
                    MergeArrayHandling = MergeArrayHandling.Union
                });
                mergedInstances[languageCode] = newMergedInstance;
            }
        }

        static public string GetFromRequestLanguage(string label)
        {
            return Get(label, RequestLanguage.LngIsoCode == null ? Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code") : RequestLanguage.LngIsoCode);
        }

        static public string Get(string label, string lngIsoCode)
        {
            if (lngIsoCode.Equals(Configuration_BSO.GetCustomConfig(ConfigType.global, "language.iso.code")))
            {
                return Get(label);
            }
            else
            {
                if (mergedInstances.ContainsKey(lngIsoCode))
                {
                    return ProcessGet(label, mergedInstances[lngIsoCode]);
                }
            }
            return label;
        }

        /// <summary>
        /// Get the Label in the mergedInstance
        /// </summary>
        static public string Get(string label)
        {
            return ProcessGet(label, mergedInstance);
        }

        private static string ProcessGet(string label, dynamic outputObject)
        {
            if (string.IsNullOrEmpty(label))
                return "";
            try
            {
                string[] properties = label.Split('.');
                dynamic output = outputObject; // mergedInstance;
                foreach (string property in properties)
                {
                    if (output[property] != null)
                        output = output[property];
                    else
                        // Label not found, return the Label parameter for feedback
                        return label;
                }

                return output.ToString();
            }
            catch
            {
                return label;
            }
        }
    }

    public class LanguageUtility
    {
        private const string JSON_EXTENSION = ".json";

        public LanguageUtility()
        {
        }

        /// <summary>
        /// Check if languageCode is a valid ISO 639-1 two letter language code
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        private bool isLanguageCodeValid(string languageCode)
        {
            return CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .Any(ci => ci.TwoLetterISOLanguageName == languageCode);
        }

        /// <summary>
        /// Get the language codes from the resources directory
        /// </summary>
        /// <returns></returns>
        public List<string> GetLanguageCodes()
        {
            List<string> languageCodes = new List<string>();
            string resourceDirectory = ConfigurationManager.AppSettings["API_RESOURCE_I18N_DIRECTORY"];
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DirectoryInfo directoryInfo = new DirectoryInfo(Path.GetFullPath(Path.Combine(baseDirectory + resourceDirectory)));
            foreach (var file in directoryInfo.GetFiles("*" + JSON_EXTENSION))
            {
                var languageCode = Path.GetFileNameWithoutExtension(file.Name);
                if (isLanguageCodeValid(languageCode))
                {
                    languageCodes.Add(languageCode);
                }
                else
                {
                    Log.Instance.Info($"Language code {languageCode} is not a valid ISO-639-1 language code");
                }
            }
            return languageCodes;
        }
    }
}
