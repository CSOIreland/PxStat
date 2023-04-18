using API;

using PxStat.Template;

using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using System.Collections.Generic;
using System;
using PxStat.System.Notification;
using PxStat.Workflow;

namespace PxStat.Data
{
    /// <summary>
    /// 
    /// </summary>
    internal class Compare_BSO_TestStub : BaseTemplate_Read<Release_DTO_Read, Release_VLD_Read>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        internal Compare_BSO_TestStub(JSONRPC_API request) : base(request, new Release_VLD_Read())
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            return true;
        }

    }
}