﻿
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
using DeviceDetectorNET;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Net;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Configuration;

namespace PxStat.Resources
{
    /// <summary>
    /// The LanguageManager class must be created as a thread-safe singleton
    /// Because the process of loading dlls can take time, this could give rise to a race condition unless
    /// the thread safety is managed.
    /// </summary>
    public  class LanguageManager
    {
        public static Dictionary<string,ILanguagePlugin> Languages { get; set; }
        private static readonly object Instancelock = new object();

        private LanguageManager()
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
        public static void LoadLanguages(string baseDir = null)
        {

            if (Languages == null)
                Languages = new Dictionary<string, ILanguagePlugin>();
            var x = Directory.GetCurrentDirectory();  //C:\Wspace\8.2.0\server\PxStatCore.Test\bin\Debug\net8.0
            baseDir ??=Directory.GetParent(Directory.GetCurrentDirectory()).FullName + "\\";
            try
            {
                if (Configuration_BSO.serverLanguageResource != null)
                {
                    
                    foreach (var item in Configuration_BSO.serverLanguageResource)
                    {
                        ILanguagePlugin resource = ReadLanguageResource(item.ISO, baseDir + item.PLUGIN_LOCATION, item.NAMESPACE_CLASS, item.TRANSLATION_URL);
                        resource.IsLive = item.IS_LIVE;
                        if (resource != null && !Languages.ContainsKey(item.ISO))
                        {
                            resource.IsLive = resource.IsLive;
                            Languages.TryAdd(item.ISO, resource);
                            Log.Instance.Debug("Language dll added: " + resource.LngIsoCode + " path: " + item.PLUGIN_LOCATION);
                        }
                    }
                }

                
            }
            catch(Exception ex)
            {
                Languages=null; //we don't want a zombie language collection if there has been a problem
                Log.Instance.Fatal("Failed to load languages: " + ex.Message  );
                throw;
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
                LoadLanguages();


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
        /// <summary>
        /// Get a language from the plugins
        /// </summary>
        /// <param name="lngIsocode"></param>
        /// <param name="path"></param>
        /// <param name="namespaceClass"></param>
        /// <returns></returns>
        public static ILanguagePlugin ReadLanguageResource(string lngIsocode, string path, string namespaceClass, string translationUrl)
        {
            Log.Instance.Debug("Current module =" + Process.GetCurrentProcess().MainModule.FileName);
            var dll = Assembly.LoadFile(path);
            var languageType = dll.GetType(namespaceClass);

            string translation;
            if (translationUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Add("User-Agent", Configuration_BSO.GetStaticConfig("APP_USER_AGENT"));
                    translation = GetDownloadString(httpClient, translationUrl).Result;
                }
            }
            else if (translationUrl.StartsWith("file", StringComparison.OrdinalIgnoreCase))
            {
                var filePath = new Uri(translationUrl).LocalPath;
                translation = File.ReadAllText(filePath);
            }
            else
            {
                throw new NotSupportedException("The provided URL scheme is not supported for " + path);
            }

            dynamic languageClass = Activator.CreateInstance(languageType, translation);
            ILanguagePlugin lngPlugin = Impromptu.ActLike<ILanguagePlugin>(languageClass);

            return lngPlugin;
        }

        public static async Task<string> GetDownloadString(HttpClient httpClient, string translationUrl)
        {
            return await httpClient.GetStringAsync(translationUrl);
        }
    }
}