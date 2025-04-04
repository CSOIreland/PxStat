﻿using API;
using PxStat.Template;
using System;
using System.Net;
using System.Web;

namespace PxStat.Security
{
    internal class Login_BSO_Logout : BaseTemplate_Update<Login_DTO_Logout, Login_VLD_Logout>
    {
        internal Login_BSO_Logout(JSONRPC_API request) : base(request, new Login_VLD_Logout())
        { }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return true;
        }

        protected override bool HasUserToBeAuthenticated()
        {
            return false;
        }

        protected override bool Execute()
        {
            Login_ADO lAdo = new Login_ADO(Ado);
            if (Request.sessionCookie != null)
            {
                lAdo.Logout(Request.sessionCookie.Value);
                if(!String.IsNullOrEmpty(Request.sessionCookie.Name))
                    Response.sessionCookie = new Cookie(Request.sessionCookie.Name, "") { Expires = DateTime.Now };
            }
            Response.data = ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            return true;

        }

    }
}
