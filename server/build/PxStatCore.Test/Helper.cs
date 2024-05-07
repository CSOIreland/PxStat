using AngleSharp.Common;
using API;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.Resources;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxStat;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PxStatCore.Test
{
    /// <summary>
    /// You need to setup a user environment variable LANGUAGES_LOCATION
    /// whose value is the location of the languages dlls, for the environment, where you want the unit test to run.
    /// To set it up you need to run a similar command as follows from the Command Prompt:
    /// setx LANGUAGES_LOCATION C:\Wspace\6.0.0\server\
    /// </summary>
    internal class Helper
    {
        public static bool setupComplete;

        internal static string GetResourceFolder()
        {
            var baseDir = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
            var currentParent = Directory.GetParent(baseDir.TrimEnd(Path.DirectorySeparatorChar)).FullName;
            var baseParent = Directory.GetParent(currentParent).FullName;
            return baseParent + "\\Resources\\";
        }


        internal static void SetupApiArtifacts()
        {
            var builder = WebApplication.CreateBuilder();          
            builder.Logging.AddLog4Net();
            builder.Services.AddApiLibrary(builder);
            builder.Services.AddStaticConfiguration(builder.Configuration);
            var app = builder.Build();
            app.UseSimpleResponseMiddleware();
            var serverJson = File.ReadAllText(GetLanguagesLocation() + "PxStatCore.Test\\Resources\\Config\\config.server.json");
            Configuration_BSO.serverFileConfig = JObject.Parse(Utility.JsonDeserialize_IgnoreLoopingReference(Utility.JsonSerialize_IgnoreLoopingReference(serverJson)));
            var globalJson = File.ReadAllText(GetLanguagesLocation() + "PxStatCore.Test\\Resources\\Config\\config.global.json");
            Configuration_BSO.globalFileConfig = JObject.Parse(Utility.JsonDeserialize_IgnoreLoopingReference(Utility.JsonSerialize_IgnoreLoopingReference(globalJson)));
            var staticJson = File.ReadAllText(GetLanguagesLocation() + "PxStatCore.Test\\Resources\\Config\\static.json");
            Configuration_BSO.PxStatStaticConfig = (StaticConfig) JsonConvert.DeserializeObject<StaticConfig>(staticJson);
            API.AutoMap.Mapper = API.AutoMap.CreateMapper();
            

            Configuration_BSO.PxStatConfiguration = (IDictionary<string, string>)AppServicesHelper.AppConfiguration.Settings;
            if (Configuration_BSO.serverLanguageResource == null) Configuration_BSO.SetServerLangaugeConfig();

            //setup languages early...
            var lng = PxStat.Resources.LanguageManager.Instance;

        }

        internal static void SetupTests()
        {
            if (Helper.setupComplete) return;


            SetupApiArtifacts();


            if (PxStat.Resources.LanguageManager.Languages.Count == 0)
            {
                
                //LanguageManager.LoadLanguages(GetLanguagesLocation());
                LanguageManager.LoadLanguages("");
            }

            Helper.setupComplete = true;
        }
        private static string GetLanguagesLocation()
        {
            return Environment.GetEnvironmentVariable("LANGUAGES_LOCATION", EnvironmentVariableTarget.User);
        }

        private static Dictionary<string, object> GetHttpPost(string methodText, Dictionary<string, object> parameters)
        {
            Dictionary<string, object> hpost = new()
            {
                { "jsonrpc", "2.0" },
                { "method", methodText },
                { "params", parameters}
            };

            return hpost;
        }

        internal static JSONRPC_API GetRequest(string methodText, Dictionary<string, object> parameters)
        {
            JSONRPC_API request = new JSONRPC_API()
            {
                userPrincipal = JObject.FromObject(GetUserPrincipal()),
                httpPOST = Utility.JsonSerialize_IgnoreLoopingReference(GetHttpPost(methodText, parameters)),
                ipAddress = "3.15.14.158",
                userAgent = Utility.JsonSerialize_IgnoreLoopingReference(GetUserPrincipal()),
                scheme = "https",
                requestType = "POST",
                parameters = JObject.FromObject(parameters),
                method = methodText,
                requestHeaders= new HeaderDictionary(new Dictionary<String, StringValues>{{ "Key", "Value"}}) as IHeaderDictionary

        };

            return request;
        }

        private static Dictionary<string, object> GetUserPrincipal()
        {
            string? GivenName = ApiServicesHelper.Configuration.GetSection("Test:GivenName").Value;
            string? Surname = ApiServicesHelper.Configuration.GetSection("Test:Surname").Value;
            string? EmailAddress = ApiServicesHelper.Configuration.GetSection("Test:EmailAddress").Value;
            string? Name = ApiServicesHelper.Configuration.GetSection("Test:Name").Value;
            string? SamAccountName = ApiServicesHelper.Configuration.GetSection("Test:SamAccountName").Value;

            Dictionary<string, object> uagent = new Dictionary<string, object>();
            uagent.Add("GivenName", GivenName);
            uagent.Add("Surname", Surname);
            uagent.Add("EmailAddress", EmailAddress);
            uagent.Add("Name", Name);
            uagent.Add("SamAccountName", SamAccountName);
            return uagent;
        }

    }
}
