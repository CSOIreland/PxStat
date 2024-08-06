using API;
using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json.Linq;
using PxStat.Security;
using System.Dynamic;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace PxStat.System.Navigation
{
    /// <summary>
    /// Navigation is a set of links used to enhance user experience. These APIs control the reading and creation of Navigation items.
    /// </summary>
    [AllowAPICall]
    public class Navigation_API
    {

        /// <summary>
        /// Reads a set of data for front end navigation
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        public static dynamic Read(JSONRPC_API requestApi)
        {
            return new Navigation_BSO_Read(requestApi).Read().Response;
        }
        /// <summary>
        /// Search for one or more Releases using search terms and other parameters
        /// </summary>
        /// <param name="requestApi"></param>
        /// <returns></returns>
        /// 

        public static dynamic Search(JSONRPC_API requestApi)
        {

            return new Navigation_BSO_Search(requestApi).Read().Response;
        }

        public static dynamic Search(RESTful_API  requestApi)
        {
            dynamic dto = new JObject();
            switch (requestApi.parameters.Count)
            {
                case 0:
                    dto.LngIsoCode = Configuration_BSO.GetApplicationConfigItem(ConfigType.global, "language.iso.code");
                    break;
                case 1:
                    break;
                case 2:
                    dto.LngIsoCode = requestApi.parameters[1];
                    break;
                case 3:
                    dto.LngIsoCode = string.IsNullOrEmpty( requestApi.parameters[1]) ? null : requestApi.parameters[1];
                    dto.ThmCode = string.IsNullOrEmpty(requestApi.parameters[2]) ? null : requestApi.parameters[2];
                    break;
                case 4:
                    dto.LngIsoCode = string.IsNullOrEmpty(requestApi.parameters[1]) ? null : requestApi.parameters[1];
                    dto.ThmCode = string.IsNullOrEmpty(requestApi.parameters[2]) ? null : requestApi.parameters[2];
                    dto.SbjCode = string.IsNullOrEmpty(requestApi.parameters[3]) ? null : requestApi.parameters[3];
                    break;
                case 5:
                    dto.LngIsoCode = string.IsNullOrEmpty(requestApi.parameters[1]) ? null : requestApi.parameters[1];
                    dto.ThmCode = string.IsNullOrEmpty(requestApi.parameters[2]) ? null : requestApi.parameters[2];
                    dto.SbjCode = string.IsNullOrEmpty(requestApi.parameters[3]) ? null : requestApi.parameters[3];
                    dto.PrcCode = string.IsNullOrEmpty(requestApi.parameters[4]) ? null : requestApi.parameters[4];
                    break;
                default:
                    dto.LngIsoCode = string.IsNullOrEmpty(requestApi.parameters[1]) ? null : requestApi.parameters[1];
                    dto.ThmCode = string.IsNullOrEmpty(requestApi.parameters[2]) ? null : requestApi.parameters[2];
                    dto.SbjCode = string.IsNullOrEmpty(requestApi.parameters[3]) ? null : requestApi.parameters[3];
                    dto.PrcCode = string.IsNullOrEmpty(requestApi.parameters[4]) ? null : requestApi.parameters[4];
                    dto.MtrCode = string.IsNullOrEmpty(requestApi.parameters[5]) ? null : requestApi.parameters[5];
                    break;


            }
            JObject pdto = JObject.Parse(Utility.JsonSerialize_IgnoreLoopingReference(dto));
            requestApi.parameters = pdto;
            var output = new Navigation_BSO_Search(requestApi).Read().Response;
            output.response = Utility.JsonSerialize_IgnoreLoopingReference(output.data);
            return output;
        }
    }
}
