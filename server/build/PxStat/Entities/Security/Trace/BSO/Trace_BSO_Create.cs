using API;
using PxStat.Resources;
using System;
using System.Text.RegularExpressions;

namespace PxStat.Security
{
    /// <summary>
    /// Creates a Trace entry
    /// </summary>
    // This BSO does not extend the BaseTemplate in order to avoid circular references
    internal static class Trace_BSO_Create
    {
        /// <summary>
        /// Write the trace information to the trace table
        /// </summary>
        /// <param name="ado"></param>
        /// <param name="request"></param>
        /// <param name="inTransaction"></param>
        /// <returns></returns>
        internal static int Execute(IADO ado, IRequest request,string ccnUsername=null)
        {
            //Check in case the NoTrace attribute is set for the requested API method
            //If so, don't proceed with the trace.
            if (API.MethodReader.MethodHasAttribute(request.method, "NoTrace"))
                return 0;

            Trace_ADO tAdo = new Trace_ADO();
            Trace_DTO_Create dto = new Trace_DTO_Create();

            if (ActiveDirectory.IsAuthenticated(request.userPrincipal))
            {
                dto.CcnUsername = request.userPrincipal.SamAccountName.ToString();
            }

            if (dto.CcnUsername == null)
            {
                if(Resources.MethodReader.DynamicHasProperty(request.parameters,"CcnUsername"))
                    dto.CcnUsername = request.parameters.CcnUsername;
                if (dto.CcnUsername == null && ccnUsername != null)
                    dto.CcnUsername = ccnUsername;
            }

            dto.TrcIp = request.ipAddress;
            dto.TrcMethod = request.method;
            
            dto.TrcParams = MaskParameters(Utility.JsonSerialize_IgnoreLoopingReference(request.parameters));
            dto.TrcUseragent = request.userAgent;

            int created = tAdo.Create(ado, dto);
            if (created == 0)
            {
                Log.Instance.Debug("Can't create trace");
            }
            return created;
        }


        public static string MaskParameters(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }
            // Init the output
            string output = input;

            // Loop trough the parameters to maskAPI_JSONRPC_MASK_PARAMETERS
            foreach (var param in Configuration_BSO.GetStaticConfig("API_JSONRPC_MASK_PARAMETERS").Split(','))
            {
                // https://stackoverflow.com/questions/171480/regex-grabbing-values-between-quotation-marks
                Log.Instance.Info("Masked parameter: " + param);
                output = Regex.Replace(output, "\"" + param + "\"\\s*:\\s*\"(.*?[^\\\\])\"", "\"" + param + "\": \"********\"", RegexOptions.IgnoreCase);
            }

            return output;
        }
    }
}
