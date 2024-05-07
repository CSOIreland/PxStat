using API;
using Newtonsoft.Json;
using PxStat.Template;
using System.Collections.Generic;
using System.Net;
using System;
using System.Linq;
using System.Dynamic;
using PxStat.Security;

namespace PxStat.Config
{
    internal class Config_BSO_Ping : BaseTemplate_Read<Config_DTO_Ping, Config_VLD_Ping>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Config_BSO_Ping(IRequest request) : base(request, new Config_VLD_Ping())
        {
        }

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
        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Response.data= ApiServicesHelper.ApiConfiguration.Settings["API_SUCCESS"];
            if (Request.GetType() == typeof(RESTful_API))
            {
                Response.mimeType = Configuration_BSO.GetStaticConfig("APP_TEXT_MIMETYPE");
            }
            else Response.mimeType = Configuration_BSO.GetStaticConfig("APP_JSON_MIMETYPE");
            return true;
        }


    }
}
