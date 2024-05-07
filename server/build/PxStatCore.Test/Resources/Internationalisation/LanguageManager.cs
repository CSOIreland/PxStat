
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json.Linq;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ImpromptuInterface;
using API;

namespace PxStat.Resources
{
    /// <summary>
    /// The LanguageManager class must be created as a thread-safe singleton
    /// Because the process of loading dlls can take time, this could give rise to a race condition unless
    /// the thread safety is managed.
    /// </summary>
    public  class LanguageManager
    {
        public static Dictionary<string,ILanguagePlugin>? Languages { get; set; }
        private static readonly object Instancelock = new object();

        public LanguageManager()
        {
            LoadLanguages();
        }
        private static LanguageManager instance = null;

        public static LanguageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (Instancelock)
                    {
                        if (instance == null)
                        {
                            instance = new LanguageManager();
                        }
                    }
                }
                return instance;
            }
        }


        /// <summary>
        /// Load the languages from the plugins and insert them into the Languages property
        /// Locations are found in the server.config "language-resource" tag
        /// </summary>
        public static void LoadLanguages(string baseDir=null)
        {
            if(Languages == null)
                Languages = new Dictionary<string, ILanguagePlugin>();

            if( baseDir==null) baseDir= AppDomain.CurrentDomain.BaseDirectory;

            JArray resources = Configuration_BSO.GetApplicationConfigItem (ConfigType.server, "language-resource");
            foreach(var item in resources)
            {
                string lngIsoCode = item["iso"].ToString();
                if (Languages.ContainsKey(lngIsoCode))
                    continue;

                string path = baseDir + item["plugin-location"].ToString();
                string namespaceClass = item["namespace-class"].ToString();
                bool isLive = (bool)item["is-live"];
                ILanguagePlugin resource = ReadLanguageResource(lngIsoCode, path, namespaceClass);
                if(resource!=null && ! Languages.ContainsKey(lngIsoCode))
                {
                    resource.IsLive = isLive;
                    Languages.Add(lngIsoCode,resource);
                    Log.Instance.Debug("Language dll added: " + lngIsoCode + " path: " + path);
                }
            }
        }

        /// <summary>
        /// Get a single language from Languages
        /// </summary>
        /// <param name="lngIsoCode"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ILanguagePlugin GetLanguage(string lngIsoCode)
        {
            if (Languages == null)
                return null;
            //Check that Languages exist
            if (Languages == null)
                throw new Exception("No language plugin loaded");

            //Get the language if it exists
            if(Languages.ContainsKey(lngIsoCode))
                return Languages[lngIsoCode];
            else
            {
                //otherwise just return the default language and assign it as the resource for the requested language
                var language=Languages[Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code")];
                language.LngIsoCode = lngIsoCode;
                language.IsLive = true;
                return language;

            }
            
        }

        /// <summary>
        /// Get a language from the plugins
        /// </summary>
        /// <param name="lngIsocode"></param>
        /// <param name="path"></param>
        /// <param name="namespaceClass"></param>
        /// <returns></returns>
        public static ILanguagePlugin ReadLanguageResource(string lngIsocode, string path,string namespaceClass)
        {
            var dll = Assembly.LoadFile(path);
            var languageType = dll.GetType(namespaceClass);
            dynamic languageClass = Activator.CreateInstance(languageType);

            //Impromptu will convert the dynamic to an instance of ILanguagePlugin. Must be of the same shape!
            ILanguagePlugin lngPlugin = Impromptu.ActLike<ILanguagePlugin>(languageClass);

            return lngPlugin;
        }
    }
}