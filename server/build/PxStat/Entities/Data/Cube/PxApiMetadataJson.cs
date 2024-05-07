using Newtonsoft.Json;
using System.Collections.Generic;

namespace PxStat.Data
{
    public partial class PxApiMetadataJson
    {
        [JsonProperty("title", Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty("variables", Required = Required.Always)]
        public List<Variable> Variables { get; set; }
    }

    public partial class PxApiMetadataJsonDmatrix
    {
        [JsonProperty("title", Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty("variables", Required = Required.Always)]
        public List<DVariable> Variables { get; set; }
    }

    public partial class Variable
    {
        //code text values
        [JsonProperty("code", Required = Required.Always)]
        public string Code { get; set; }
        [JsonProperty("text", Required = Required.Always)]
        public string Text { get; set; }
        [JsonProperty("values", Required = Required.Always)]
        public List<string> Values { get; set; }
        public List<string> valueTexts { get; set; }

        [JsonProperty("elimination", Required = Required.AllowNull)]
        public bool Elimination { get; set; }

    }

    public class DVariable
    {
        //code text values
        [JsonProperty("code", Required = Required.Always)]
        public string Code { get; set; }
        [JsonProperty("text", Required = Required.Always)]
        public string Text { get; set; }
        [JsonProperty("values", Required = Required.Always)]
        public List<string> Values { get; set; }
        public List<string> valueTexts { get; set; }

        [JsonProperty("elimination", Required = Required.AllowNull)]
        public bool Elimination { get; set; }

    }


    internal class ApiMetadata
    {
        internal PxApiMetadataJsonDmatrix GetApiMetadata(IDmatrix dmatrix, string LngIsoCode)
        {
            if (!dmatrix.Dspecs.ContainsKey(LngIsoCode)) return null;
            Dspec theSpec = dmatrix.Dspecs[LngIsoCode];


            PxApiMetadataJsonDmatrix json = new PxApiMetadataJsonDmatrix();
            json.Title = theSpec.Title;

            json.Variables = new List<DVariable>();



            foreach (var dim in theSpec.Dimensions)
            {
                DVariable dimvar = new DVariable() { Code = dim.Code, Text = dim.Value };

                dimvar.Values = new List<string>();
                dimvar.valueTexts = new List<string>();
                foreach (var v in dim.Variables)
                {
                    dimvar.Values.Add(v.Code);
                    dimvar.valueTexts.Add(v.Value);
                }
                json.Variables.Add(dimvar);
            }

            return json;
        }

    }
}
