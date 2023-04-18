using API;
using PxStat.Template;
using System.Collections.Generic;
using System.Dynamic;

namespace PxStat.Report
{


    /// <summary>
    /// Read database release Audit Report
    /// </summary>
    internal class TableAudit_BSO_Read : BaseTemplate_Read<TableAudit_DTO_Read, TableAudit_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal TableAudit_BSO_Read(JSONRPC_API request) : base(request, new TableAudit_VLD_Read())
        { }

        /// <summary>
        /// Test Privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return IsPowerUser() || IsModerator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            List<ADO_inputParams> paramList = new List<ADO_inputParams>();
            var releaseAuditAdo = new TableAudit_ADO();
            dynamic result = new ExpandoObject();
            result = releaseAuditAdo.Read(Ado, DTO, SamAccountName);
            Response.data = result.data;
            return true;
        }
    }
}