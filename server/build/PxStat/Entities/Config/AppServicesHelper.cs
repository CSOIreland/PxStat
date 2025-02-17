using API;
using CSO.Firebase;
using Dynamitey;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;


namespace PxStat
{
    public static class StaticConfigurations
    {

    }

    public class AppServicesHelper : ApiServicesHelper
    {
        private  static StaticConfig staticConfig;
        private static CSO.Firebase.Firebase firebase;
        private static IActiveDirectory activeDirectory;
        private static IADO staticAdo;
        private static ICacheD staticCache;
        public static bool Test = false;
        public static IConfiguration appSettings;

    public static void InjectStaticConfig(string injectedConfig)
        {
            staticConfig = JsonConvert.DeserializeObject<StaticConfig>(injectedConfig);       
        }
        

        public static IADO StaticADO
        {
            get
            {
                return new ADO("defaultConnection");
            }
            set { staticAdo = value; }
        }

        public static CSO.Firebase.Firebase Firebase
        {
            get
            {
                if (firebase == null)
                {
                    firebase=new CSO.Firebase.Firebase();
                }
                    return firebase;
                
            }
            set { firebase = value; }

        }

        public static StaticConfig StaticConfig
        {
            get
            {
                if (staticConfig == null)
                    staticConfig = AppServicesHelper.ServiceProvider.GetService<StaticConfig>();

                return staticConfig;
            }
            set { staticConfig = value; }   
        }

        public static IConfiguration AppSettings
        {
            get
            {
                if (appSettings == null)
                {

                    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.RelativeSearchPath ?? "");
                    var configuration = new ConfigurationBuilder().AddJsonFile(path + "appsettings.json");

                    appSettings = configuration.Build();
                }
                return appSettings;
            }
            set { appSettings = value; }
        }
    }
}
