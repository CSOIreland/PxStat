
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace PxStat.JsonQuery
{
    public class JsonStatQueryPxApiV1
    {
        [JsonProperty ("query", Required=Required.Always)]
        public  List<JsonStatPxApiV1Query> Query { get; set; }
        [JsonProperty("response",Required=Required.Always)]
        public JsonStatPxApiV1Response Response { get; set; }   
    }

    public class JsonStatPxApiV1Query
    {
        [JsonProperty ("code",Required=Required.Always)]    
        public string Code { get; set; }
        [JsonProperty ("selection",Required=Required.Always)]
        public JsonStatPxApiV1Selection Selection { get; set; }
    }

    public class JsonStatPxApiV1Selection
    {
        [JsonProperty("filter",Required=Required.Always)]
        public JsonStatQueryPxApiV1Filter Filter { get; set; }
        [JsonProperty("values",Required=Required.Always)]
        public List<dynamic> Values { get; set; }

        
    }

    public class JsonStatPxApiV1Response
    {
        [JsonProperty("format",Required=Required.Always)]
        public string Format { get; set; }
        [JsonProperty("pivot",Required=Required.Default)]
        public string Pivot { get; set; }
        [JsonProperty("codes", Required = Required.Default)]
        public bool Codes { get; set; }
    }



    public enum JsonStatQueryPxApiV1Filter { item,top,fluid}
}