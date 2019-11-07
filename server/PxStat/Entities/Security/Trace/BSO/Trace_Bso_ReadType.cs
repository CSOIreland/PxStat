using API;
using PxStat.Template;
using PxStat.Resources;
using System.Collections.Generic;

namespace PxStat.Security
{
    /// <summary>
    /// Returns a list of Trace Types
    /// </summary>
    internal class Trace_BSO_ReadType : BaseTemplate_Read<Trace_DTO_Read, Trace_VLD_ReadType>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Trace_BSO_ReadType(JSONRPC_API request) : base(request, new Trace_VLD_ReadType())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            List<string> traceList = Constants.C_SECURITY_TRACE_TYPE();

            List<Trace_DTO_ReadType> dtoList = new List<Security.Trace_DTO_ReadType>();

            foreach (string item in traceList)
            {
                Trace_DTO_ReadType dto = new Trace_DTO_ReadType(item);
                dtoList.Add(dto);
            }

            Response.data = dtoList;

            if (Response.data.Count > 0)
                return true;
            else
                return false;
        }
    }
}