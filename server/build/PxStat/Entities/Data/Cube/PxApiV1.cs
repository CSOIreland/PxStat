using Newtonsoft.Json;

namespace PxStat.Data
{
    //public class PxApiV1
    //{
    //    [JsonProperty("query", Required = Required.Always)]
    //    public Query[] Query { get; set; }

    //    [JsonProperty("response", Required = Required.Always)]
    //    public Response Response { get; set; }

    //}
    public class Query
    {
        [JsonProperty("code", Required = Required.Always)]
        public string Code { get; set; }

        [JsonProperty("selection", Required = Required.Always)]
        public Selection Selection { get; set; }
    }

    public class Selection
    {
        [JsonProperty("filter", Required = Required.Always)]
        public string Filter { get; set; }

        [JsonProperty("values", Required = Required.Always)]
        public string[] Values { get; set; }
    }

    public class Response
    {
        [JsonProperty("format", Required = Required.Always)]
        public string Format { get; set; }
    }
}
