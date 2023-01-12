using API;
using PxStat.Resources;

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
        internal static int Execute(ADO ado, IRequest request,string ccnUsername=null)
        {
            //Check in case the NoTrace attribute is set for the requested API method
            //If so, don't proceed with the trace.
            if (Resources.MethodReader.MethodHasAttribute(request.method, "NoTrace"))
                return 0;

            Trace_ADO tAdo = new Trace_ADO();
            Trace_DTO_Create dto = new Trace_DTO_Create();

            if (ActiveDirectory.IsAuthenticated(request.userPrincipal))
            {
                dto.CcnUsername = request.userPrincipal.SamAccountName.ToString();
            }

            if (dto.CcnUsername == null)
            {
                if(MethodReader.DynamicHasProperty(request.parameters,"CcnUsername"))
                    dto.CcnUsername = request.parameters.CcnUsername;
                if (dto.CcnUsername == null && ccnUsername != null)
                    dto.CcnUsername = ccnUsername;
            }

            dto.TrcIp = request.ipAddress;
            dto.TrcMethod = request.method;
            dto.TrcParams = JSONRPC.MaskParameters(request.parameters.ToString());
            dto.TrcUseragent = request.userAgent;

            int created = tAdo.Create(ado, dto);
            if (created == 0)
            {
                Log.Instance.Debug("Can't create trace");
            }
            return created;
        }
    }
}
