using API;
using Newtonsoft.Json;
using PxStat.Template;
using System.Collections.Generic;
using System.Net;
using System;
using System.Linq;
using System.Dynamic;

namespace PxStat.Config
{
    internal class Config_BSO_ReadApi : BaseTemplate_Read<Config_DTO_Read, Config_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Config_BSO_ReadApi(IRequest request) : base(request, new Config_VLD_Read())
        {
        }

        /// <summary>
        /// Test privilege
        /// </summary>
        /// <returns></returns>
        override protected bool HasPrivilege()
        {
            return  IsAdministrator();
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        protected override bool Execute()
        {
            Config_ADO cAdo = new Config_ADO(Ado);

            ADO_readerOutput result = null;

                if (DTO.type.Equals("APP") || string.IsNullOrEmpty(DTO.type))
                {
                    DTO.type = "API";
                }
                result = cAdo.Read(DTO);
         
            if (!result.hasData)
            {
                Response.statusCode = HttpStatusCode.NotFound;
                return false;
            }

            var appData = (List<dynamic>)result.data[0];
            if (DTO.name != null)
            {               
                var data = appData.Where(x => x.API_KEY.Equals(DTO.name)).FirstOrDefault();
                if (data?.API_VALUE != null)
                    Response.data = data.API_VALUE;
                return true;

            }
            else
            {
                var rdata = appData.Select(x => new { x.API_KEY, x.API_VALUE, x.API_SENSITIVE_VALUE });
                Response.data = MaskData(rdata);
                return true;
            }            
        }

        private dynamic MaskData(dynamic data)
        {
            if (data == null) return null;
            dynamic amendedData = new List<ExpandoObject>();
            foreach(var item in data) 
            {
                dynamic dItem =new ExpandoObject();
                dItem.API_KEY=item.API_KEY;
                dItem.API_VALUE= item.API_SENSITIVE_VALUE ? "*****" :item.API_VALUE;
                dItem.API_SENSITIVE_VALUE = item.API_SENSITIVE_VALUE;
                amendedData.Add(dItem);
            }
            return amendedData;
        }
    }
}
