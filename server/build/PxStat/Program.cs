// This application entry point is based on ASP.NET Core new project templates and is included
// as a starting point for app host configuration.
// This file may need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core hosting, see https://docs.microsoft.com/aspnet/core/fundamentals/host/web-host

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http.Json;
using API;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PxStat.Security;

namespace PxStat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplication app=null;

            //var builder = WebApplication.CreateBuilder(args);
            var builder = WebApplication.CreateBuilder();

         
            
            builder.Services.AddApiLibrary(builder);

            if (ApiServicesHelper.ApplicationLoaded)
            {
                AutoMap.Mapper = API.AutoMap.CreateMapper();
            }

            builder.Services.AddStaticConfiguration(builder.Configuration);
            
            app = builder.Build();
            bool isStateless = Convert.ToBoolean(ApiServicesHelper.ApiConfiguration.Settings["API_STATELESS"]);
            if (!isStateless)
                app.UseSession();
           // app.UseMvcWithDefaultRoute();

            app.UseSimpleResponseMiddleware();

            if (ApiServicesHelper.ApplicationLoaded)
            {
                SetupConfig();
            }


            if (app != null)
                app.Run();
            else
            {
                Log.Instance.Fatal("Abnormal End");
            }

        }


        private static void SetupConfig()
        {
            
            Configuration_BSO.PxStatConfiguration = (IDictionary<string, string>)AppServicesHelper.AppConfiguration.Settings;
            if (Configuration_BSO.serverLanguageResource == null) Configuration_BSO.SetServerLangaugeConfig();
            //setup languages early...
            var lng = PxStat.Resources.LanguageManager.Instance;

        }
    }
}
