using API;
using Dynamitey;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;


namespace PxStat
{
    public class AppServicesHelper : ApiServicesHelper
    {
        private  static StaticConfig staticConfig;
        private static IFirebase firebase;
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
                    var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");

                    appSettings = configuration.Build();
                }
                return appSettings;
            }
            set { appSettings = value; }
        }
    }
}
