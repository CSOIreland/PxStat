using Newtonsoft.Json;
using PxStat.Resources;
using System;
using System.Collections.Generic;

namespace PxStat.JsonQuery
{

    public class JsonStatQuery
    {

        public JsonStatQuery() { }
        private string _version;
        private string _class;

        [JsonProperty("id", Required = Required.Always)]
        public List<string> Id { get; set; }

        [JsonProperty("version", Required = Required.Always)]
        public string Version { get { return _version; } set { if (value != Constants.C_JSON_STAT_QUERY_VERSION) throw new Exception("Cannot unmarshal type Version"); else _version = Constants.C_JSON_STAT_QUERY_VERSION; } }

        [JsonProperty("class", Required = Required.Always)]
        public string Class { get { return _class; } set { if (value != Constants.C_JSON_STAT_QUERY_CLASS) throw new Exception("Cannot unmarshal type Class"); else _class = Constants.C_JSON_STAT_QUERY_CLASS; } }

        [JsonProperty("extension", Required = Required.Always)]
        public Dictionary<string, object> Extension { get; set; }

        [JsonProperty("dimension", Required = Required.Always)]
        public Dictionary<string, Dimension> Dimensions { get; set; }
    }

    public class JsonStatQueryExtension
    {
        [JsonProperty("extension", Required = Required.Always)]
        public Extension extension { get; set; }
    }

    public class Extension
    {
        [JsonProperty("matrix", Required = Required.Default)]
        public string Matrix { get; set; }
        [JsonProperty("language", Required = Required.Always)]
        public Language Language { get; set; }
        [JsonProperty("format", Required = Required.Always)]
        public Format Format { get; set; }
        [JsonProperty("release", Required = Required.Default)]
        public int RlsCode { get; set; }
        [JsonProperty("pivot", Required = Required.Default)]
        public string Pivot { get; set; }
        [JsonProperty("codes", Required = Required.Default)]
        public bool? Codes { get; set; }
    }

    public class Language
    {
        [JsonProperty("code", Required = Required.Always)]
        public string Code { get; set; }
        [JsonProperty("culture", Required = Required.Default)]
        public string Culture { get; set; }
    }

    public class Format
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }
        [JsonProperty("version", Required = Required.Always)]
        public string Version { get; set; }
    }

    public class Dimension
    {

        [JsonProperty("category", Required = Required.Always)]
        public Category Category { get; set; }
        public string Id { get; set; }
    }



    public class Category
    {
        [JsonProperty("index", Required = Required.Always)]
        public List<string> Index { get; set; }
    }





}
