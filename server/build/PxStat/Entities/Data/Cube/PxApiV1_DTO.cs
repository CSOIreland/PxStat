
using API;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PxStat.JsonQuery;
using PxStat.Resources;
using PxStat.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace PxStat.Data
{
    public class PxApiV1_DTO
    {
        public JsonStatQueryPxApiV1 jsonStatQueryPxApiV1 { get; set; }
        public string LngIsoCode { get; set; }
        public string MtrCode { get; set; }

        public PxApiV1_DTO(dynamic parameters) 
        {
            try
            {
                if (parameters.queryString != null)
                    jsonStatQueryPxApiV1 = Utility.JsonDeserialize_IgnoreLoopingReference<JsonStatQueryPxApiV1>(parameters.queryString);
            }
            catch
            {
                throw new Exception(Label.Get("error.validation"));
            }

            if (parameters.LngIsoCode != null)
                LngIsoCode = parameters.LngIsoCode;

            if(parameters.MtrCode != null)
                MtrCode = parameters.MtrCode;   
        }
    }

}