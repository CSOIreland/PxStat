using API;
using DeviceDetectorNET;
using DeviceDetectorNET.Cache;
using PxStat.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;


namespace PxStat.Security
{
    /// <summary>
    /// Creates an Analytic Entry
    /// </summary>
    internal static class Analytic_BSO_Create
    {


        /// <summary>
        /// Creates the analytic entry if one is deemed to be necessary
        /// This method relies on DeviceDetector.NET. Details at https://github.com/totpero/DeviceDetector.NET
        /// It is advisable to frequently check for updates, especially to the regexes folder (situated in the Resources folder of this project)
        /// </summary>
        /// <param name="Ado"></param>
        /// <param name="requestDTO"></param>
        /// <param name="hRequest"></param>
        /// <param name="request"></param>
        internal static void Create(ADO Ado, dynamic requestDTO, HttpRequest hRequest, IRequest request)
        {
            //If this method doesn't require analytic logging then exit the function here
            if (!MethodReader.MethodHasAttribute(request.method, "Analytic")) return;

            Analytic_DTO aDto = new Analytic_DTO();


            if (hRequest.UserLanguages != null)
            {
                if (hRequest.UserLanguages.Count() > 0)
                    if (hRequest.UserLanguages[0].Length >= 2)
                        aDto.EnvironmentLngIsoCode = hRequest.UserLanguages[0].Substring(0, 2);
            }

            //Get a masked version of the ip address
            aDto.NltMaskedIp = getMaskedIp(request.ipAddress);

            //Get the matrix field from the calling DTO
            if (MethodReader.DynamicHasProperty(requestDTO, "jStatQueryExtension")) aDto.matrix = requestDTO.jStatQueryExtension.extension.Matrix;

            // Get the Referer
            aDto.NltReferer = hRequest.UrlReferrer == null || String.IsNullOrEmpty(hRequest.UrlReferrer.Host) ? null : hRequest.UrlReferrer.Host;

            //The m2m parameter will not be translated into a DTO property so we just read it from the request parameters if it exists
            if (MethodReader.DynamicHasProperty(requestDTO, "m2m"))
            {
                aDto.NltM2m = requestDTO.m2m;


            }
            else aDto.NltM2m = true;

            if (MethodReader.DynamicHasProperty(requestDTO, "widget"))
            {
                aDto.NltWidget = requestDTO.widget;
            }

            if (MethodReader.DynamicHasProperty(requestDTO, "user"))
            {
                aDto.NltUser = requestDTO.user;
            }

            // Get the DateTime
            aDto.NltDate = DateTime.Now;


            //Get Format information
            if (MethodReader.DynamicHasProperty(requestDTO, "jStatQueryExtension"))
            {
                if (MethodReader.DynamicHasProperty(requestDTO.jStatQueryExtension.extension.Format, "Type") && MethodReader.DynamicHasProperty(requestDTO.jStatQueryExtension.extension.Format, "Version"))
                {
                    aDto.FrmType = requestDTO.jStatQueryExtension.extension.Format.Type;
                    aDto.FrmVersion = requestDTO.jStatQueryExtension.extension.Format.Version;
                }
            }


            //Get the device detector and populate the dto attributes
            DeviceDetector deviceDetector = GetDeviceDetector(request.userAgent);

            aDto.NltBotFlag = deviceDetector.IsBot();


            //If it's a bot, then that overrules everything else
            if (aDto.NltBotFlag)
            {
                aDto.NltM2m = false;
                aDto.NltUser = false;
                aDto.NltWidget = false;
            }

            if (deviceDetector.GetBrowserClient().Match != null)
            {
                aDto.NltBrowser = deviceDetector.GetBrowserClient().Match.Name;
            }

            if (deviceDetector.GetOs().Match != null)
                aDto.NltOs = deviceDetector.GetOs().Match.Name;


            var valids = new Analytic_VLD().Validate(aDto);

            //validate whatever has been returned
            if (!valids.IsValid)
            {
                foreach (var fail in valids.Errors)
                {
                    Log.Instance.Debug("Analytic method failed validation:" + request.method + " :" + fail.ErrorMessage);
                }
                return;
            }

            //Create the analytic entry
            Analytic_ADO ado = new Analytic_ADO(Ado);

            if (ado.Create(aDto) == 0)
            {
                Log.Instance.Debug("Failed to create Analytic:" + request.method);
                return;
            }

            return;
        }


        /// <summary>
        /// Setup and return the device detector
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        private static DeviceDetector GetDeviceDetector(string userAgent)
        {
            DeviceDetectorSettings.RegexesDirectory = HostingEnvironment.MapPath(@"~\Resources\");
            var deviceDetector = new DeviceDetector(userAgent);
            deviceDetector.SetCache(new DictionaryCache());
            deviceDetector.Parse();
            return deviceDetector;
        }

        /// <summary>
        /// Get an ip address without the final part
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static string getMaskedIp(string ip)
        {
            List<string> readIp = ip.Split('.').ToList<string>();
            if (readIp.Count <= 1) return "";
            readIp.RemoveAt(readIp.Count - 1);
            return string.Join(".", readIp.ToArray());
        }


    }
}
