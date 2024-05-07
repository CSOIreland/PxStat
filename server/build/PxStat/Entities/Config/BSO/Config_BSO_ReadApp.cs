using API;
using Newtonsoft.Json;
using PxStat.Resources;
using PxStat.Template;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace PxStat.Config
{
    /// <summary>
    /// Reads a Configuration
    /// </summary>
    internal class Config_BSO_ReadApp : BaseTemplate_Read<Config_DTO_Read, Config_VLD_Read>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="request"></param>
        internal Config_BSO_ReadApp(IRequest request) : base(request, new Config_VLD_Read())
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
            Config_ADO cAdo = new Config_ADO(Ado);

            ADO_readerOutput result = null;
            try
            {
                result = cAdo.Read(DTO);
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex.Message);
            }

            if (!result.hasData)
            {
                Response.statusCode = HttpStatusCode.NotFound;
                return false;
            }

            string key;
            dynamic value;
            string json;
            Dictionary<string, dynamic> d = new Dictionary<string, dynamic>();
            if (result.data.Count == 0) return false;

            var appData = (List<dynamic>)result.data[0];
            var data = appData.Where(x => x.APP_KEY.Equals(DTO.name)).FirstOrDefault();

            if (data?.APP_KEY != null)
            {
                key = data.APP_KEY;

                if (data.APP_SENSITIVE_VALUE)
                {
                    Response.data = "*****";
                }
                else
                {
                    if (!Resources.Cleanser.TryParseJson<dynamic>(data.APP_VALUE, out dynamic canParse))
                    {
                        Response.statusCode = HttpStatusCode.NotFound;
                        return false;
                    }

                    value = JsonConvert.DeserializeObject(data.APP_VALUE);
                    d.Add(key, value);


                    json = JsonConvert.SerializeObject(value);
                    Response.data = JsonConvert.DeserializeObject(json);
                }
            }
            else
            {
                Response.statusCode = HttpStatusCode.NotFound;
                return false;
            }
            return true;            
        }
        private dynamic MaskData(dynamic data)
        {
            if (data == null) return null;
            dynamic amendedData = new List<ExpandoObject>();
            foreach (var item in data)
            {
                dynamic dItem = new ExpandoObject();
                dItem.API_KEY = item.API_KEY;
                dItem.API_VALUE = item.API_SENSITIVE_VALUE ? "*****" : item.API_VALUE;
                dItem.API_SENSITIVE_VALUE = item.API_SENSITIVE_VALUE;
                amendedData.Add(dItem);
            }
            return amendedData;
        }
    }
}
