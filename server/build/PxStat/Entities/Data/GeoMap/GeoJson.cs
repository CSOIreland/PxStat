using Newtonsoft.Json;
using System.Collections.Generic;

namespace PxStat.Data
{
    public class GeoJson
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }
        [JsonProperty("features", Required = Required.Always)]
        public Feature[] Features { get; set; }
    }
    public class FeatureCollection

    {
        public List<Feature> Features { get; set; }
    }
    public class Feature
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }

        [JsonProperty("properties", Required = Required.Always)]
        public Dictionary<string, string> Properties { get; set; }

        [JsonProperty("geometry", Required = Required.Always)]
        public Geometry Geometry { get; set; }


    }

    public class Geometry
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }

        [JsonProperty("coordinates", Required = Required.Always)]
        public object[] CoOrdinates { get; set; }


    }



}
