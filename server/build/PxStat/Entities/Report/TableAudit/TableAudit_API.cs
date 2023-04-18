using API;
using System;

namespace PxStat.Report
{    /// <summary>
     /// API for reading Release Audits
     /// </summary>
    [AllowAPICall]
    public class TableAudit_API
    {
        public static dynamic Read(JSONRPC_API jsonrpcRequest)
        {
            return new TableAudit_BSO_Read(jsonrpcRequest).Read().Response;
        }
    }

}
