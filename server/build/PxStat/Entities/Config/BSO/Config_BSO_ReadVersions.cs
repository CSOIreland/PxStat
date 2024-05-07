using API;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using PxStat.Resources;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

namespace PxStat.Config
{
    /// <summary>
    /// Reads a Configuration
    /// </summary>
    internal class Config_BSO_ReadVersions : BaseTemplate_Read<Config_DTO_ReadVersions, Config_VLD_ReadVersions>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Config_BSO_ReadVersions(IRequest request) : base(request, new Config_VLD_ReadVersions())
        {
        }

        /// <summary>
        /// 
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
            decimal? version;
            Config_ADO cAdo = new Config_ADO(Ado);
            if (DTO.type.ToUpper().Equals("APP"))
            {
                var result = cAdo.GetLatestVersion(Ado, 2); 
                version = Convert.ToDecimal(result);
            }
            else if (DTO.type.ToUpper().Equals("API"))
            {
                var result = cAdo.GetLatestVersion(Ado, 1);
                version = Convert.ToDecimal(result);
            }
            else
            {
                Response.error = "Incorrect type value";
                return false;
            }
            Response.data = version;
            return true;
        }
    }
}
