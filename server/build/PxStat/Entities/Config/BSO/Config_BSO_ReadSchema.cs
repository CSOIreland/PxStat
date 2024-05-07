using API;
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
    internal class Config_BSO_ReadSchema : BaseTemplate_Read<Config_DTO_ReadSchema, Config_VLD_ReadSchema>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Config_BSO_ReadSchema(IRequest request) : base(request, new Config_VLD_ReadSchema())
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
            var directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            string fileName;

            if (DTO.name.Contains("config"))
            {
                // Replace config with schema in name
                fileName = DTO.name.Replace("config", "schema");
            }
            else
            {
                // Name is FirebaseKey.json
                fileName = "schema." + DTO.name;
            }
            string schema = File.ReadAllText(directory + @"\Resources\Schemas\" + fileName);
            Response.data = JsonConvert.DeserializeObject(schema);
            return true;

        }
    }
}
