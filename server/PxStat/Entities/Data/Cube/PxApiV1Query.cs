using Newtonsoft.Json;
using System.Collections.Generic;

namespace PxStat.Data
{
    public class PxApiV1Query
    {
        [JsonProperty("query", Required = Required.Always)]
        public Query[] Query { get; set; }

        [JsonProperty("response", Required = Required.Always)]
        public Response Response { get; set; }

    }
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

    public class PxApiV1Json
    {
        [JsonProperty("columns", Required = Required.Always)]
        public List<Column> Columns { get; set; }

        [JsonProperty("comments", Required = Required.Always)]
        public List<string> Comments { get; set; }

        [JsonProperty("data", Required = Required.Always)]
        public List<DataItem> DataItems { get; set; }
    }
    public class Column
    {
        [JsonProperty("code", Required = Required.Always)]
        public string Code { get; set; }

        [JsonProperty("text", Required = Required.Always)]
        public string Text { get; set; }

        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }

    }

    public class DataItem
    {
        [JsonProperty("key", Required = Required.Always)]
        public List<string> Key { get; set; }

        [JsonProperty("values", Required = Required.Always)]
        public List<string> Values { get; set; }
    }

    /// <summary>
    /// Class
    /// </summary>
    public static partial class SerializePxApiV1Json
    {
        public static string ToJson(this PxApiV1Json self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

}
